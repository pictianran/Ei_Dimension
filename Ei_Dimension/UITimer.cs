﻿using DIOS.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using Ei_Dimension.Controllers;

namespace Ei_Dimension
{
  internal static class UITimer
  {
    private static int _uiUpdateIsActive;
    private static Timer _timer;
    private static bool _started;

    #if DEBUG
    private static List<CommandStruct> DEBUGCommandList = new List<CommandStruct>
    {
      new CommandStruct{ Code = 0x80, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x81, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x84, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB0, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB1, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB2, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB3, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB4, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB5, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB6, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x93, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x94, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x95, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x96, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x98, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x99, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x9A, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x9B, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA6, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA7, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x9C, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x9D, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x9E, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0x9F, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA0, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA1, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA2, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA3, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA4, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xA5, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xAC, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xAF, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xC4, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xB8, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xC0, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xC7, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xC8, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xC9, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xCC, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xF1, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xF2, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xF3, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xF4, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xF9, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xDE, Command = 0x00, Parameter = 1, FParameter = 32},
      new CommandStruct{ Code = 0xFD, Command = 0x00, Parameter = 1, FParameter = 32}
      
      //Fill me up and test
    };
    #endif

    public static void Start()
    {
      if (_started)
        throw new Exception("UITimer is already Started");

      _timer = new Timer(Tick);
      _ = _timer.Change(new TimeSpan(0, 0, 0, 0, 100),
        new TimeSpan(0, 0, 0, 0, 500));
      _started = true;
    }

    private static void Tick(object state)
    {
      if (Interlocked.CompareExchange(ref _uiUpdateIsActive, 1, 0) == 1)
        return;
      
      if (App.Device.IsMeasurementGoing)
      {
        GraphsController.Instance.Update();
        ActiveRegionsStatsController.Instance.UpdateCurrentStats();
        TextBoxHandler.UpdateEventCounter();
        App.Device.UpdateStateMachine();
      }
      ServiceMenuEnabler.Update();
      _uiUpdateIsActive = 0;

      #if DEBUG
      App.Current.Dispatcher.Invoke(() => {
        if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.J))
        {
          App.Device.DEBUGJBeadADD();
        }
        if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.F3))
        {
          //App.Device.DEBUGCommandTest(DEBUGCommandList[DEBUGCommandCounter++]);
          foreach (var cs in DEBUGCommandList)
          {
            App.Device.DEBUGCommandTest(cs);
          }
        }
      });
      #endif
    }
  }
}
