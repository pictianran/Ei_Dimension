﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIOS.Core.SelfTests
{
  internal class SelfTestData
  {
    internal bool ResultReady
    {
      get
      {
        return Pressure != null
               && StartupPressure != null
               && MotorX != null
               && MotorY != null
               && MotorZ != null;
      }
    }

    internal float? StartupPressure
    {
      get
      {
        return _startupPressure;
      }
      private set
      {
        _startupPressure = value;
        if (_startupPressure > 1.0)
          ResultMessage += $"Startup OverPressure [{_startupPressure}]\n";
        Console.WriteLine($"Startup pressure: {value}");
      }
    }

    internal float? Pressure
    {
      get { return _pressure; }
      private set
      {
        _pressure = value;
        if (_pressure > _device.MaxPressure)
          ResultMessage += $"OverPressure {_pressure}\n";
        Console.WriteLine($"Pressure: {value}");
      }
    }

    internal float? MotorX {
      get { return _motorX; }
      set
      {
        _motorX = value;
        if (_motorX < 485 || _motorX > 505)
        {
          ResultMessage += "Out Of Position: Motor X\n";
        }
        Console.WriteLine($"Motor X: {value}");
      }
    }

    internal float? MotorY
    {
      get { return _motorY; }
      set
      {
        _motorY = value;
        if (_motorY < 485 || _motorY > 505)
        {
          ResultMessage += "Out Of Position: Motor Y\n";
        }
        Console.WriteLine($"Motor Y: {value}");
      }
    }

    internal float? MotorZ
    {
      get { return _motorZ; }
      set
      {
        _motorZ = value;
        if (_motorZ < 17 || _motorZ > 21)
        {
          ResultMessage += "Out Of Position: Motor Z\n";
        }
        Console.WriteLine($"Motor Z: {value}");
      }
    }

    internal string ResultMessage { get; private set; }

    private float? _motorX;
    private float? _motorY;
    private float? _motorZ;
    private float? _pressure;
    private float? _startupPressure;
    private bool _startup = true;
    private Device _device;

    internal SelfTestData(Device device)
    {
      _device = device;
    }

    internal void SetPressure(float pressure)
    {
      if (_startup)
      {
        StartupPressure = pressure;
        _startup = false;
        return;
      }
      Pressure = pressure;
    }
    
  }
}
