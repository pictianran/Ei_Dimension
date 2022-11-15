﻿using System;
using System.Threading.Tasks;

namespace DIOS.Core
{
  internal class StateMachine
  {
    private readonly Device _device;
    private State _state;

    public bool Report { get; set; }

    public StateMachine(Device device, bool report)
    {
      _device = device;
      Report = report;
    }

    public void Start()
    {
      if(_state == State.Reset)
        _state = State.Start;
      Console.WriteLine("SM start");
    }

    public void Action()
    {
      //TODO: allow only one instance
      switch (_state)
      {
        case State.Reset:
          _device.RequestParameterUpdate(DeviceParameterType.BeadConcentration);
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
          Action4();
          break;
      }
      if (Report)
        ReportState();
      Advance();
    }
    
    private void Action1()
    {
      _device.OnFinishedReadingWell();
    }

    private void Action2()
    {
      _ = Task.Run(() => { _device.Publisher.PublishEverything(); });
      GetRunStatistics();
    }
    
    private bool Action3()
    {
      if (!_device.SystemActivity[11])  //does not contain Washing
      {
        return true;
      }
      _device.RequestParameterUpdate(DeviceParameterType.SystemActivityStatus);
      return false;
    }
    
    private void Action4()
    {
      _device.MainCommand("FlushCmdQueue");
      _device.MainCommand("Set Property", code: 0xc3); //clear empty syringe token
      _device.MainCommand("Set Property", code: 0xcb); //clear sync token to allow next sequence to execute
      if(_device.EndBeadRead())
        _device.OnFinishedMeasurement();
      else
      {
        _device.SetupRead();
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
      Console.WriteLine($"SM advance to {_state}");
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