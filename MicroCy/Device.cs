﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DIOS.Core.InstrumentParameters;
using DIOS.Core.SelfTests;

/*
 * Most commands on the host side parallel the Properties and Methods document fo QB-1000
 * The only complex action is reading a region of wells defined by the READ SECTION parameters and button
 * They define an rectangular area of the plate that may include the entire plate. The complexity is
 * furthered by the fact that a read can be terminated in many different ways:
 * 1. Manually with the END SECTION READ button
 * 2. Manually with the END READ button which just ends the current well read and goes on the the next well
 * 3. OUT OF SHEATH condition, the sheath syringe is a position 0
 * 4. OUT OF SAMPLE condition, the sample syringe (either A or B) is at position 0
 * 5. Required number of beads read
 * 6. Required number of beads read in each region.
 * 7. Instrument fault (bubbles, plunger overload, clog, low laser power, etc)
 * 
 * When the instrument detects one of these end conditions: QB_cmd_proc.c / SyringeEmpty()
 * 1. The command Queue is cleared (the queue is only holding instructions for the currently read well)
 * 2. The sync token is cleared allowing new commands to execute immediately
 * 3. An FD or FE is sent to the host to tell it to save the data file and then it sends an EE or EF
 * 4. An EE or EF sequence is executed in the instrument, flushing the remaining sample and resetting syringes
 * 5. If regioncount is 1-- == 0 the last sample is read, if > 0 the next well is also aspirated
 * 
 * When the host initiates an end condition (isDone== true)
 * This version is being used at MRBM
 */

namespace DIOS.Core
{
  public class Device
  {
    public ResultsPublisher Publisher { get; }
    public MapController MapCtroller { get; }
    public RunResults Results { get; }
    public WorkOrder WorkOrder { get; set; }
    public ConcurrentQueue<CommandStruct> Commands { get; } = new ConcurrentQueue<CommandStruct>();
    public ConcurrentQueue<BeadInfoStruct> DataOut { get; } = new ConcurrentQueue<BeadInfoStruct>();
    public WellController WellController { get; } = new WellController();
    public BitArray SystemActivity { get; } = new BitArray(16, false);
    public event EventHandler<ReadingWellEventArgs> StartingToReadWell;
    public event EventHandler<ReadingWellEventArgs> FinishedReadingWell;
    public event EventHandler FinishedMeasurement;
    public event EventHandler<StatsEventArgs> NewStatsAvailable;
    public OperationMode Mode { get; set; }
    public SystemControl Control { get; set; }
    public Gate ScatterGate
    {
      get
      {
        return _scatterGate;
      }
      set
      {
        _scatterGate = value;
        MainCommand("Set Property", code: 0xCA, parameter: (ushort)_scatterGate);
      }
    }
    public Termination TerminationType { get; set; }
    public int BoardVersion { get; internal set; }
    public float ReporterScaling { get; set; }
    public int BeadsToCapture { get; set; }
    public int BeadCount { get; internal set; }
    public int TotalBeads { get; internal set; }
    public int MinPerRegion { get; set; }
    public bool IsMeasurementGoing { get; private set; }
    public bool Everyevent { get; set; }
    public bool RMeans { get; set; }
    public bool PlateReportActive { get; set; }
    public bool OnlyClassified { get; set; }
    public bool Reg0stats { get; set; }
    public HiSensitivityChannel SensitivityChannel
    {
      get
      {
        return _sensitivityChannel;
      }
      set
      {
        _sensitivityChannel = value;
        MainCommand("Set Property", code: 0x1E, parameter: (ushort)_sensitivityChannel);
      }
    }
    public float HdnrTrans
    {
      get
      {
        return _hdnrTrans;
      }
      set
      {
        _hdnrTrans = value;
        MainCommand("Set FProperty", code: 0x0A, fparameter: _hdnrTrans);
      }
    }

    public float HDnrCoef
    {
      get
      {
        return _hdnrCoef;
      }
      set
      {
        _hdnrCoef = value;
        MainCommand("Set FProperty", code: 0x20, fparameter: _hdnrCoef);
      }
    }
    public float Compensation { get; set; }
    public DirectoryInfo RootDirectory { get; private set; }

    private bool _readingA;
    private Gate _scatterGate;
    private HiSensitivityChannel _sensitivityChannel;
    private float _hdnrTrans;
    private float _hdnrCoef;
    private readonly DataController _dataController;
    private readonly StateMachine _stateMach;
    internal SelfTester SelfTester { get; }
    internal readonly BeadProcessor _beadProcessor;

    public Device(ISerial connection)
    {
      SetSystemDirectories();
      _dataController = new DataController(this, connection);
      _stateMach = new StateMachine(this, true);
      Publisher = new ResultsPublisher(this);
      MapCtroller = new MapController(this);
      SelfTester = new SelfTester(this);
      Results = new RunResults(this);
      _beadProcessor = new BeadProcessor(this);
      MainCommand("Sync");
      TotalBeads = 0;
      Mode = OperationMode.Normal;
      MapCtroller.MoveMaps();
      MapCtroller.LoadMaps();
      Reg0stats = false;
      IsMeasurementGoing = false;
      ReporterScaling = 1;
      MainCommand("Get Property", code: 0x01);  //get board version
    }

    public void UpdateStateMachine()
    {
      _stateMach.Action();
    }

    public void PrematureStop()
    {
      WellController.PreparePrematureStop();
      _stateMach.Start();
    }

    /// <summary>
    /// Starts a sequence of commands to finalize well measurement. The sequence is in a form of state machine that takes several timer ticks
    /// </summary>
    public void StartStateMachine()
    {
      _stateMach.Start();
    }

    public void StartSelfTest()
    {
      SelfTester.FluidicsTest();
    }

    /// <summary>
    /// Polls the device once per 100 ms until the test result is ready
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetSelfTestResultAsync()
    {
      var t = new Task<string>(() =>
      {
        string msg;
        while (!SelfTester.GetResult(out msg))
        {
          System.Threading.Thread.Sleep(100);
        }
#if DEBUG
        Console.WriteLine($"\nSelfTest Finished\n\n{msg}\n");
#endif
        return msg;
      });
      t.Start();
      return await t;
    }

    internal void SetupRead()
    {
      SetReadingParamsForWell();
      WellController.Advance();
      Results.Reset();
      Publisher.GetNewFileName();
      ResultsPublisher.StartNewWellReport();
      BeadCount = 0;

      OnStartingToReadWell();
      if (WellController.IsLastWell)
      {
        if (_readingA)
          MainCommand("Read B");
        else
          MainCommand("Read A");
      }
      else
      {
        SetAspirateParamsForWell();
        if (_readingA)
          MainCommand("Read B Aspirate A");
        else
          MainCommand("Read A Aspirate B");
      }
    }

    public void StartOperation()
    {
      MainCommand("Set Property", code: 0xce, parameter: MapCtroller.ActiveMap.calParams.minmapssc);  //set ssc gates for this map
      MainCommand("Set Property", code: 0xcf, parameter: MapCtroller.ActiveMap.calParams.maxmapssc);
      _beadProcessor.ConstructClassificationMap(MapCtroller.ActiveMap);
      //read section of plate
      MainCommand("Get FProperty", code: 0x58);
      MainCommand("Get FProperty", code: 0x68);
      ResultsPublisher.StartNewPlateReport();
      Publisher.ResetSummary();
      SetAspirateParamsForWell();  //setup for first read
      SetReadingParamsForWell();
      WellController.Advance();
      Results.Reset();
      Publisher.GetNewFileName();
      ResultsPublisher.StartNewWellReport();
      BeadCount = 0;
      OnStartingToReadWell();
      MainCommand("Set Property", code: 0x19, parameter: 1); //bubble detect on
      MainCommand("Position Well Plate"); //move motors. next position is set in properties 0xad and 0xae
      MainCommand("Aspirate Syringe A"); //handles down and pickup sample
      TotalBeads = 0;

      if (WellController.IsLastWell)
        MainCommand("Read A");
      else
      {
        SetAspirateParamsForWell();
        MainCommand("Read A Aspirate B");
      }

      if (TerminationType != Termination.TotalBeadsCaptured) //set some limit for running to eos or if regions are wrong
        BeadsToCapture = 100000;
    }

    private void SetReadingParamsForWell()
    {
      MainCommand("Set Property", code: 0xaa, parameter: (ushort)WellController.NextWell.RunSpeed);
      MainCommand("Set Property", code: 0xc2, parameter: (ushort)WellController.NextWell.ChanConfig);
      BeadsToCapture = WellController.NextWell.BeadsToCapture;
      MinPerRegion = WellController.NextWell.MinPerRegion;
      TerminationType = WellController.NextWell.TermType;
    }

    private void SetAspirateParamsForWell()
    {
      MainCommand("Set Property", code: 0xad, parameter: (ushort)WellController.NextWell.RowIdx);
      MainCommand("Set Property", code: 0xae, parameter: (ushort)WellController.NextWell.ColIdx);
      MainCommand("Set Property", code: 0xaf, parameter: (ushort)WellController.NextWell.SampVol);
      MainCommand("Set Property", code: 0xac, parameter: (ushort)WellController.NextWell.WashVol);
      MainCommand("Set Property", code: 0xc4, parameter: (ushort)WellController.NextWell.AgitateVol);
    }

    internal bool EndBeadRead()
    {
      if (_readingA)
        MainCommand("End Bead Read A");
      else
        MainCommand("End Bead Read B");
      return WellController.IsLastWell;
    }

    public void MainCommand(string command, byte? cmd = null, byte? code = null, ushort? parameter = null, float? fparameter = null)
    {
      CommandStruct cs = CommandLists.MainCmdTemplatesDict[command];
      cs.Command = cmd ?? cs.Command;
      cs.Code = code ?? cs.Code;
      cs.Parameter = parameter ?? cs.Parameter;
      cs.FParameter = fparameter ?? cs.FParameter;
      switch (command)
      {
        case "Read A":
          _readingA = true;
          break;
        case "Read A Aspirate B":
          _readingA = true;
          break;
        case "Read B":
          _readingA = false;
          break;
        case "Read B Aspirate A":
          _readingA = false;
          break;
        case "FlushCmdQueue":
          cs.Command = 0x02;
          break;
        case "Idex":
          cs.Command = Idex.Pos;
          cs.Parameter = Idex.Steps;
          cs.FParameter = Idex.Dir;
          break;
      }
      DataController.OutCommands.Enqueue((command, cs));
      #if DEBUG
      Console.Error.WriteLine($"{DateTime.Now.ToString()} Enqueued [{command}]: {cs.ToString()}");
      #endif
      _dataController.NotifyCommandReceived();
    }

    private void SetSystemDirectories()
    {
      RootDirectory = new DirectoryInfo(Path.Combine(@"C:\Emissioninc", Environment.MachineName));
      List<string> subDirectories = new List<string>(7) { "Config", "WorkOrder", "SavedImages", "Archive", "Result", "Status", "AcquisitionData", "SystemLogs" };
      try
      {
        foreach (var d in subDirectories)
        {
          RootDirectory.CreateSubdirectory(d);
        }
        Directory.CreateDirectory(RootDirectory.FullName + @"\Result" + @"\Summary");
        Directory.CreateDirectory(RootDirectory.FullName + @"\Result" + @"\Detail");
      }
      catch
      {
        Console.WriteLine("Directory Creation Failed");
      }
    }

    private void OnStartingToReadWell()
    {
      IsMeasurementGoing = true;
      StartingToReadWell?.Invoke(this, new ReadingWellEventArgs(WellController.CurrentWell.RowIdx, WellController.CurrentWell.ColIdx,
        ResultsPublisher.FullFileName));
      MainCommand("Set FProperty", code: 0x06);  //reset totalbeads in firmware
    }

    internal void OnFinishedReadingWell()
    {
      FinishedReadingWell?.Invoke(this, new ReadingWellEventArgs(WellController.CurrentWell.RowIdx, WellController.CurrentWell.ColIdx));
      MainCommand("Get FProperty", code: 0x06);  //get totalbeads from firmware
    }

    internal void OnFinishedMeasurement()
    {
      IsMeasurementGoing = false;
      Results.EndOfOperationReset();
      MainCommand("Set Property", code: 0x19);  //bubble detect off
      if (Mode ==  OperationMode.Verification)
        Verificator.CalculateResults();
      FinishedMeasurement?.Invoke(this, EventArgs.Empty);
    }

    internal void OnNewStatsAvailable()
    {
      NewStatsAvailable?.Invoke(this, new StatsEventArgs(_beadProcessor.Stats, _beadProcessor.AvgBg));
    }

    #if DEBUG
    public void SetBoardVersion(int v)
    {
      BoardVersion = v;
    }
    #endif
  }
}