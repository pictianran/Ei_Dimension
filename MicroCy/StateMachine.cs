﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroCy
{
  internal class StateMachine
  {
    private readonly MicroCyDevice _device;
    private State _state;
    public bool Report { get; set; }

    public StateMachine(MicroCyDevice device, bool report)
    {
      _device = device;
      Report = report;
    }

    public void Start()
    {
      if(_state == State.Reset)
        _state = State.Start;
    }

    public void Action()
    {
      switch (_state)
      {
        case State.Reset:
          //Skip the tick
          return;
        case State.Start:
          Action1();
          break;
        case State.State2:
          Action2();
          break;
        case State.State3:
          if (!Action3())
            return;
          break;
        case State.End:
          Action5();
          break;
      }
      if (Report)
        ReportState();
      Advance();
    }
    
    private void Action1()
    {
      StopWellMeasurement();
      _device.OnFinishedReadingWell();
    }

    private void StopWellMeasurement()
    {
      _device._beadProcessor.SavBeadCount = _device.BeadCount;   //save for stats
      ResultsPublisher.SavingWell = _device.WellController.CurrentWell; //save the index of the currrent well for background file save
      _device.MainCommand("End Sampling");    //sends message to instrument to stop sampling
    }

    private void Action2()
    {
      var tempres = new List<WellResult>(_device.WellResults.Count);
      for(var i = 0; i < _device.WellResults.Count; i++)
      {
        var r = new WellResult();
        r.RP1vals = new List<float>(_device.WellResults[i].RP1vals);
        r.RP1bgnd = new List<float>(_device.WellResults[i].RP1bgnd);
        r.regionNumber = _device.WellResults[i].regionNumber;
        tempres.Add(r);
      }
      _ = Task.Run(() =>
      {
        _device.Publisher.SaveBeadFile(tempres);

        if (_device.RMeans
            && _device.WellController.IsLastWell
            && _device.PlateReportActive)    //end of read and json results requested)
          _device.Publisher.OutputPlateReport();
      });
      GetRunStatistics();
    }
    
    private bool Action3()
    {
      if (!_device.SystemActivity[11])  //does not contain Washing
      {
        return true;
      }
      _device.MainCommand("Get Property", code: 0xcc);
      return false;
    }
    
    private void Action5()
    {
      _device.MainCommand("FlushCmdQueue");
      _device.MainCommand("Set Property", code: 0xc3); //clear empty syringe token
      _device.MainCommand("Set Property", code: 0xcb); //clear sync token to allow next sequence to execute
      if(_device.EndBeadRead())
        _device.OnFinishedMeasurement();
      else
      {
        _device.SetupRead();
        _device.InitBeadRead();
      }
      Task.Run(()=>
      {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      });
    }

    private void Advance()
    {
      if (_state < State.End)
        _state++;
      else
        _state = State.Reset;
    }

    private void ReportState()
    {
      string str = null;
      switch (_state)
      {
        case State.Start:
          str = $"{DateTime.Now.ToString()} Reporting End Sampling";
          break;
        case State.State2:
          str = $"{DateTime.Now.ToString()} Reporting Background File Save Init";
          break;
        case State.State3:
          str = $"{DateTime.Now.ToString()} Reporting Washing Complete";
          break;
        case State.End:
          str = $"{DateTime.Now.ToString()} Reporting End of current well";
          break;
      }
      Console.WriteLine(str);
    }

    private void GetRunStatistics()
    {
      _device._beadProcessor.CalculateGStats();
      _device._beadProcessor.CalculateBackgroundAverages();
      _device.OnNewStatsAvailable();
    }

    private enum State
    {
      Reset,
      Start,
      State2,
      State3,
      End
    }
  }
}