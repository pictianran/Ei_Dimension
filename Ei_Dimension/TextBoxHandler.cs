﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Ei_Dimension.ViewModels;
using MicroCy;

namespace Ei_Dimension
{
  internal class TextBoxHandler
  {
    private static readonly string[] SyncElements = { "SHEATH", "SAMPLE_A", "SAMPLE_B", "FLASH", "END_WELL", "VALVES", "X_MOTOR",
      "Y_MOTOR", "Z_MOTOR", "PROXIMITY", "PRESSURE", "WASHING", "FAULT", "ALIGN MOTOR", "MAIN VALVE", "SINGLE STEP" };

    private static readonly object ConditionVar = new object();
    public static void Update()
    {
      while (MicroCyDevice.Commands.TryDequeue(out var exe))
      {
        Action update = null;
        switch (exe.Code)
        {
          case 0x01:
            MicroCyDevice.BoardVersion = exe.Parameter;
#if  DEBUG
          Console.Error.WriteLine($"Detected Board Rev v{MicroCyDevice.BoardVersion}");  
#endif
            if (MicroCyDevice.BoardVersion > 1)
              update = HideChannels;
            break;
          case 0x02:
            update = () => ChannelOffsetViewModel.Instance.SiPMTempCoeff[0] = exe.FParameter.ToString();
            break;
          case 0x04:
            update = () => ComponentsViewModel.Instance.IdexTextBoxInputs[0] = exe.Parameter.ToString("X2");
            break;
#if DEBUG
          case 0x06:
            update = () => MainViewModel.Instance.TotalBeadsInFirmware[0] = exe.FParameter.ToString();
            break;
#endif
          case 0x10:  //cuvet drain cb
            update = () => ComponentsViewModel.Instance.ValvesStates[2] = exe.Parameter == 1;
            break;
          case 0x11:  //Fan
            update = () => ComponentsViewModel.Instance.ValvesStates[3] = exe.Parameter == 1;
            break;
          case 0x12:  //sample A valve cb LEGACY, use 0x18 to switch with IDEX
            update = () => ComponentsViewModel.Instance.ValvesStates[0] = exe.Parameter == 1;
            break;
          //case 0x13:  //sample b LEGACY
          //  ComponentsViewModel.Instance.ValvesStates[1] = exe.Parameter == 1;
          //  break;
          case 0x14:
            switch (exe.Parameter)
            {
              case 0:
                update = () => ComponentsViewModel.Instance.GetPositionTextBoxInputs[0] = exe.FParameter.ToString();
                break;
              case 1:
                update = () => ComponentsViewModel.Instance.GetPositionTextBoxInputs[1] = exe.FParameter.ToString();
                break;
              case 2:
                update = () => ComponentsViewModel.Instance.GetPositionTextBoxInputs[2] = exe.FParameter.ToString();
                break;
            }
            break;
          case 0x15:
            update = () => ComponentsViewModel.Instance.GetPositionToggleButtonStateBool[0] = exe.Parameter == 1;
            break;
          case 0x16:
            update = () => MotorsViewModel.Instance.PollStepActive[0] = exe.Parameter == 1;
            break;
          case 0x18:
            update = () =>
            {
              if (exe.Parameter == 1)
              {
                if (ComponentsViewModel.Instance.IInputSelectorState == 0)
                {
                  var temp = ComponentsViewModel.Instance.InputSelectorState[0];
                  ComponentsViewModel.Instance.InputSelectorState[0] = ComponentsViewModel.Instance.InputSelectorState[1];
                  ComponentsViewModel.Instance.InputSelectorState[1] = temp;
                  ComponentsViewModel.Instance.IInputSelectorState = 1;
                }
              }
              else
              {
                if (ComponentsViewModel.Instance.IInputSelectorState == 1)
                {
                  var temp = ComponentsViewModel.Instance.InputSelectorState[0];
                  ComponentsViewModel.Instance.InputSelectorState[0] = ComponentsViewModel.Instance.InputSelectorState[1];
                  ComponentsViewModel.Instance.InputSelectorState[1] = temp;
                  ComponentsViewModel.Instance.IInputSelectorState = 0;
                }
              }
            };
            break;
          //case 0x1a:
          //  ProxOncb.Checked = (exe.Parameter == 1);
          //  break;
          case 0x1b:
            if (exe.Parameter == 0)
            {
              CalibrationViewModel.Instance.CalJustFailed = false;
              _ = App.Current.Dispatcher.BeginInvoke((Action)(() =>
              {
                CalibrationViewModel.Instance.CalibrationSuccess();
              }));
            }
            break;
          case 0x20:
            update = () => CalibrationViewModel.Instance.DNRContents[0] = exe.FParameter.ToString();
            MicroCy.InstrumentParameters.Calibration.HDnrCoef = exe.FParameter;
            break;
          case 0x22:  //pressure
            update = () =>
            {
              double dd = exe.FParameter;
              DashboardViewModel.Instance.PressureMon[0] = dd.ToString("f3");
              if (dd < DashboardViewModel.Instance.MinPressure)
                DashboardViewModel.Instance.MinPressure = dd;
              if (dd > DashboardViewModel.Instance.MaxPressure)
                DashboardViewModel.Instance.MaxPressure = dd;
              DashboardViewModel.Instance.PressureMon[1] = DashboardViewModel.Instance.MaxPressure.ToString("f3");
              DashboardViewModel.Instance.PressureMon[2] = DashboardViewModel.Instance.MinPressure.ToString("f3");
            };
            break;
          case 0x24:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[9] = exe.Parameter.ToString();
            break;
          case 0x25:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[7] = exe.Parameter.ToString();
            break;
          case 0x26:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[8] = exe.Parameter.ToString();
            break;
          case 0x28:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[0] = exe.Parameter.ToString();
            break;
          case 0x29:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[1] = exe.Parameter.ToString();
            break;
          case 0x2a:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[2] = exe.Parameter.ToString();
            break;
          case 0x2c:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[3] = exe.Parameter.ToString();
            break;
          case 0x2d:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[4] = exe.Parameter.ToString();
            break;
          case 0x2e:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[5] = exe.Parameter.ToString();
            break;
          case 0x2f:
            update = () => ChannelsViewModel.Instance.Bias30Parameters[6] = exe.Parameter.ToString();
            break;
          case 0x30:
            update = () => SyringeSpeedsViewModel.Instance.SheathSyringeParameters[0] = exe.Parameter.ToString();
            break;
          case 0x31:
            update = () => SyringeSpeedsViewModel.Instance.SheathSyringeParameters[1] = exe.Parameter.ToString();
            break;
          case 0x32:
            update = () => SyringeSpeedsViewModel.Instance.SheathSyringeParameters[2] = exe.Parameter.ToString();
            break;
          case 0x33:
            update = () => SyringeSpeedsViewModel.Instance.SheathSyringeParameters[3] = exe.Parameter.ToString();
            break;
          case 0x34:
            update = () => SyringeSpeedsViewModel.Instance.SheathSyringeParameters[4] = exe.Parameter.ToString();
            break;
          case 0x35:
            update = () => SyringeSpeedsViewModel.Instance.SheathSyringeParameters[5] = exe.Parameter.ToString();
            break;
          case 0x38:
            update = () => SyringeSpeedsViewModel.Instance.SamplesSyringeParameters[0] = exe.Parameter.ToString();
            break;
          case 0x39:
            update = () => SyringeSpeedsViewModel.Instance.SamplesSyringeParameters[1] = exe.Parameter.ToString();
            break;
          case 0x3a:
            update = () => SyringeSpeedsViewModel.Instance.SamplesSyringeParameters[2] = exe.Parameter.ToString();
            break;
          case 0x3b:
            update = () => SyringeSpeedsViewModel.Instance.SamplesSyringeParameters[3] = exe.Parameter.ToString();
            break;
          case 0x3c:
            update = () => SyringeSpeedsViewModel.Instance.SamplesSyringeParameters[4] = exe.Parameter.ToString();
            break;
          case 0x3d:
            update = () => SyringeSpeedsViewModel.Instance.SamplesSyringeParameters[5] = exe.Parameter.ToString();
            break;
          case 0x41:
            update = () => MotorsViewModel.Instance.ParametersZ[3] = exe.Parameter.ToString();
            break;
          case 0x42:
            update = () => MotorsViewModel.Instance.ParametersZ[4] = exe.Parameter.ToString();
            break;
          case 0x43:
            update = () => MotorsViewModel.Instance.ParametersZ[2] = exe.Parameter.ToString();
            break;
          case 0x44:
            update = () => MotorsViewModel.Instance.ParametersZ[5] = exe.FParameter.ToString();
            break;
          case 0x46:
            update = () => MotorsViewModel.Instance.StepsParametersZ[4] = exe.FParameter.ToString();
            break;
          case 0x48:
            update = () => MotorsViewModel.Instance.StepsParametersZ[0] = exe.FParameter.ToString();
            break;
          case 0x4a:
            update = () => MotorsViewModel.Instance.StepsParametersZ[1] = exe.FParameter.ToString();
            break;
          case 0x4c:
            update = () => MotorsViewModel.Instance.StepsParametersZ[2] = exe.FParameter.ToString();
            break;
          case 0x4e:
            update = () => MotorsViewModel.Instance.StepsParametersZ[3] = exe.FParameter.ToString();
            break;
          case 0x51:
            update = () => MotorsViewModel.Instance.ParametersX[3] = exe.Parameter.ToString();
            break;
          case 0x52:
            update = () => MotorsViewModel.Instance.ParametersX[4] = exe.Parameter.ToString();
            break;
          case 0x53:
            update = () => MotorsViewModel.Instance.ParametersX[2] = exe.Parameter.ToString();
            break;
          case 0x54:
            update = () => MotorsViewModel.Instance.ParametersX[5] = exe.FParameter.ToString();
            break;
          case 0x56:
            update = () => MotorsViewModel.Instance.StepsParametersX[4] = exe.FParameter.ToString();
            break;
          case 0x58:
            update = () => MotorsViewModel.Instance.StepsParametersX[0] = exe.FParameter.ToString();
            break;
          case 0x5a:
            update = () => MotorsViewModel.Instance.StepsParametersX[1] = exe.FParameter.ToString();
            break;
          case 0x5c:
            update = () => MotorsViewModel.Instance.StepsParametersX[2] = exe.FParameter.ToString();
            break;
          case 0x5e:
            update = () => MotorsViewModel.Instance.StepsParametersX[3] = exe.FParameter.ToString();
            break;
          case 0x61:
            update = () => MotorsViewModel.Instance.ParametersY[3] = exe.Parameter.ToString();
            break;
          case 0x62:
            update = () => MotorsViewModel.Instance.ParametersY[4] = exe.Parameter.ToString();
            break;
          case 0x63:
            update = () => MotorsViewModel.Instance.ParametersY[2] = exe.Parameter.ToString();
            break;
          case 0x64:
            update = () => MotorsViewModel.Instance.ParametersY[5] = exe.FParameter.ToString();
            break;
          case 0x66:
            update = () => MotorsViewModel.Instance.StepsParametersY[4] = exe.FParameter.ToString();
            break;
          case 0x68:
            update = () => MotorsViewModel.Instance.StepsParametersY[0] = exe.FParameter.ToString();
            break;
          case 0x6a:
            update = () => MotorsViewModel.Instance.StepsParametersY[1] = exe.FParameter.ToString();
            break;
          case 0x6c:
            update = () => MotorsViewModel.Instance.StepsParametersY[2] = exe.FParameter.ToString();
            break;
          case 0x6e:
            update = () => MotorsViewModel.Instance.StepsParametersY[3] = exe.FParameter.ToString();
            break;
          case 0x80:
            update = () => ChannelsViewModel.Instance.TempParameters[7] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0x81:
            update = () => ChannelsViewModel.Instance.TempParameters[8] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0x82:
            update = () => ChannelOffsetViewModel.Instance.ChannelsBaseline[7] = exe.Parameter.ToString();
            break;
          case 0x83:
            update = () => ChannelOffsetViewModel.Instance.ChannelsBaseline[8] = exe.Parameter.ToString();
            break;
          case 0x84:
            update = () => ChannelsViewModel.Instance.TempParameters[9] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0x85:
            update = () => ChannelOffsetViewModel.Instance.ChannelsBaseline[9] = exe.Parameter.ToString();
            break;
          case 0x8b:
            //update = () => CalibrationViewModel.Instance.ClassificationTargetsContents[0] = exe.Parameter.ToString();
            break;
          case 0x8c:
            //update = () => CalibrationViewModel.Instance.ClassificationTargetsContents[1] = exe.Parameter.ToString();
            break;
          case 0x8d:
            //update = () => CalibrationViewModel.Instance.ClassificationTargetsContents[2] = exe.Parameter.ToString();
            break;
          case 0x90:
            update = () => MotorsViewModel.Instance.ParametersX[7] = exe.Parameter.ToString();
            break;
          case 0x91:
            update = () => MotorsViewModel.Instance.ParametersY[7] = exe.Parameter.ToString();
            break;
          case 0x92:
            update = () => MotorsViewModel.Instance.ParametersZ[7] = exe.Parameter.ToString();
            break;
          case 0x93:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[9] = exe.Parameter.ToString();
            break;
          case 0x94:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[8] = exe.Parameter.ToString();
            break;
          case 0x95:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[7] = exe.Parameter.ToString();
            break;
          case 0x96:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[6] = exe.Parameter.ToString();
            break;
          case 0x98:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[4] = exe.Parameter.ToString();
            break;
          case 0x99:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[3] = exe.Parameter.ToString();
            break;
          case 0x9a:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[1] = exe.Parameter.ToString();
            break;
          case 0x9b:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[2] = exe.Parameter.ToString();
            break;
          case 0x9c:
            update = () => ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[8] = exe.Parameter.ToString();
            break;
          case 0x9d:
            update = () => ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[7] = exe.Parameter.ToString();
            break;
          case 0x9e:
            update = () => ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[9] = exe.Parameter.ToString();
            break;
          case 0x9f:
            update = () => ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[6] = exe.Parameter.ToString();
            break;
          case 0xa0:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[0] = exe.Parameter.ToString();
              ChannelOffsetViewModel.Instance.OverrideSliderChange = true;
              ChannelOffsetViewModel.Instance.SliderValue1 = (double)(exe.Parameter);
            };
            break;
          case 0xa1:
            update = () => ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[5] = exe.Parameter.ToString();
            break;
          case 0xa2:
            update = () => ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[4] = exe.Parameter.ToString();
            break;
          case 0xa3:
            update = () => ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[3] = exe.Parameter.ToString();
            break;
          case 0xa4:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[1] = exe.Parameter.ToString();
              ChannelOffsetViewModel.Instance.OverrideSliderChange = true;
              ChannelOffsetViewModel.Instance.SliderValue2 = (double)(exe.Parameter);
            };
            break;
          case 0xa5:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsOffsetParameters[2] = exe.Parameter.ToString();
              ChannelOffsetViewModel.Instance.OverrideSliderChange = true;
              ChannelOffsetViewModel.Instance.SliderValue3 = (double)(exe.Parameter);
            };
            break;
          case 0xa6:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[0] = exe.Parameter.ToString();
            break;
          case 0xa7:
            update = () => ChannelsViewModel.Instance.TcompBiasParameters[5] = exe.Parameter.ToString();
            break;
          case 0xa8:
            update = () => DashboardViewModel.Instance.OrderItems[exe.Parameter].ForAppUpdater(4);
            break;
          case 0xa9:
            update = () => DashboardViewModel.Instance.ClassiMapItems[exe.Parameter].ForAppUpdater(2);
            break;
          case 0xaa:  //read speed
            update = () => DashboardViewModel.Instance.SpeedItems[exe.Parameter].ForAppUpdater(1);
            break;
          case 0xac:
            update = () => DashboardViewModel.Instance.Volumes[1] = exe.Parameter.ToString();
            break;
          case 0xad:  //TODO: remove?
            if (exe.Parameter > 15)
              exe.Parameter = 0;
            //update = () => MotorsViewModel.Instance.WellRowButtonItems[exe.Parameter].ForAppUpdater(1);
            MicroCyDevice.PlateRow = (byte)exe.Parameter;
            break;
          case 0xae:  //TODO: remove?
            if (exe.Parameter > 24)
              exe.Parameter = 0;
            //update = () => MotorsViewModel.Instance.WellColumnButtonItems[exe.Parameter].ForAppUpdater(2);
            MicroCyDevice.PlateCol = (byte)exe.Parameter;  //TODO: it doesn't accout for 96well; can go overboard and crash
            break;
          case 0xaf:
            update = () => DashboardViewModel.Instance.Volumes[0] = exe.Parameter.ToString();
            break;
          case 0xb0:
            update = () => ChannelsViewModel.Instance.TempParameters[0] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0xb1:
            update = () => ChannelsViewModel.Instance.TempParameters[1] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0xb2:
            update = () => ChannelsViewModel.Instance.TempParameters[2] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0xb3:
            update = () => ChannelsViewModel.Instance.TempParameters[3] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0xb4:
            update = () => ChannelsViewModel.Instance.TempParameters[4] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0xb5:
            update = () => ChannelsViewModel.Instance.TempParameters[5] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0xb6:
            update = () => ChannelsViewModel.Instance.TempParameters[6] = (exe.Parameter / 10.0).ToString("N1");
            break;
          case 0xb8:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsBaseline[0] = exe.Parameter.ToString();
            };
            break;
          case 0xb9:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsBaseline[1] = exe.Parameter.ToString();
            };
            break;
          case 0xba:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsBaseline[2] = exe.Parameter.ToString();
            };
            break;
          case 0xbb:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsBaseline[3] = exe.Parameter.ToString();
            };
            break;
          case 0xbc:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsBaseline[4] = exe.Parameter.ToString();
            };
            break;
          case 0xbd:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsBaseline[5] = exe.Parameter.ToString();
            };
            break;
          case 0xbe:
            update = () =>
            {
              ChannelOffsetViewModel.Instance.ChannelsBaseline[6] = exe.Parameter.ToString();
            };
            break;
          case 0xc0:
            update = () =>
            {
              ComponentsViewModel.Instance.LasersActive[0] = (exe.Parameter & 0x01) == 1;
              ComponentsViewModel.Instance.LasersActive[1] = (exe.Parameter & 0x02) == 2;
              ComponentsViewModel.Instance.LasersActive[2] = (exe.Parameter & 0x04) == 4;
            };
            break;
          case 0xc4:
            update = () => DashboardViewModel.Instance.Volumes[2] = exe.FParameter.ToString();
            break;
          case 0xc7:
            update = () =>
            {
              var g = (float)(exe.Parameter / 4096.0 / 0.040 * 3.3);
              ComponentsViewModel.Instance.LaserVioletPowerValue[0] = g.ToString("N1") + " mw";
            };
            break;
          case 0xc8:
            update = () =>
            {
              var g = (float)(exe.Parameter / 4096.0 / 0.04 * 3.3);
              ComponentsViewModel.Instance.LaserGreenPowerValue[0] = g.ToString("N1") + " mw";
            };
            break;
          case 0xc9:
            update = () =>
            {
              var g = (float)(exe.Parameter / 4096.0 / 0.04 * 3.3);
              ComponentsViewModel.Instance.LaserRedPowerValue[0] = g.ToString("N1") + " mw";
            };
            break;
          case 0xca:  //TODO: remove?
            update = () => CalibrationViewModel.Instance.GatingItems[exe.Parameter].ForAppUpdater();
            break;
          case 0xcc:  //sync pending
            update = () =>
            {
              var list = MainButtonsViewModel.Instance.ActiveList;
              for (var i = 0; i < 16; i++)
              {
                if ((exe.Parameter & (1 << i)) != 0)
                {
                  if (!list.Contains(SyncElements[i]))
                    list.Add(SyncElements[i]);
                }
                else if (list.Contains(SyncElements[i]))
                  list.Remove(SyncElements[i]);
              }
            };
            break;
          case 0xcd:
            //update = () => CalibrationViewModel.Instance.EventTriggerContents[0] = exe.Parameter.ToString();
            break;
          case 0xce:
            //update = () => CalibrationViewModel.Instance.EventTriggerContents[1] = exe.Parameter.ToString();
            break;
          case 0xcf:
            //update = () => CalibrationViewModel.Instance.EventTriggerContents[2] = exe.Parameter.ToString();
            break;
          case 0xf1:
            if (exe.Command == 1)  //sheath empty
            {
              App.Device.MainCommand("Set Property", code: 0xcb, parameter: 0x1000);
              App.Device.MainCommand("Sheath"); //halt 
              App.Device.MainCommand("Set Property", code: 0xc1, parameter: 1);  //switch to recovery command buffer #1

              void Act()
              {
                App.Device.MainCommand("Sheath Empty Prime");
                App.Device.MainCommand("Set Property", code: 0xcb); //clear sync token to allow recovery to run
                lock (ConditionVar)
                {
                  Monitor.Pulse(ConditionVar);
                }
              }

              App.Current.Dispatcher.Invoke(() => Notification.Show("Sheath Empty\nRefill and press OK", Act, "OK"));
              lock (ConditionVar)
              {
                Monitor.Wait(ConditionVar);
              }
            }
            else if (exe.Command == 2) //pressure overload
            {
              if (exe.FParameter > int.Parse(ComponentsViewModel.Instance.MaxPressureBox))
              {
                void Act()
                {
                  Environment.Exit(0);
                  lock (ConditionVar)
                  {
                    Monitor.Pulse(ConditionVar);
                  }
                }
                App.Current.Dispatcher.Invoke(() =>
                  Notification.Show("Pressure Overload\nCheck for waste line obstructions", Act, "Power Off\n  System"));
                lock (ConditionVar)
                {
                  Monitor.Wait(ConditionVar);
                }
              }
            }
            break;
          case 0xf2:
            string ws;
            if (exe.Command > 0x63)
            {
              if (exe.Parameter == 0x501)  //sample syringe A
              {
                ws = "Sample syringe A Error " + exe.Command.ToString();
              }
              else ws = "Sample syringe B Error " + exe.Command.ToString();
              
              void Act()
              {
                var tempres = new List<WellResults>(MicroCyDevice.WellResults.Count);
                for (var i = 0; i < MicroCyDevice.WellResults.Count; i++)
                {
                  var r = new WellResults();
                  r.RP1vals = new List<float>(MicroCyDevice.WellResults[i].RP1vals);
                  r.RP1bgnd = new List<float>(MicroCyDevice.WellResults[i].RP1bgnd);
                  r.regionNumber = MicroCyDevice.WellResults[i].regionNumber;
                  tempres.Add(r);
                }
                MicroCyDevice.WellsToRead = MicroCyDevice.CurrentWellIdx;
                ResultReporter.SaveBeadFile(tempres);
                Environment.Exit(0);
                lock (ConditionVar)
                {
                  Monitor.Pulse(ConditionVar);
                }
              }
              App.Current.Dispatcher.Invoke(() => Notification.Show(ws + "\nPower Off System", Act, "OK"));
              lock (ConditionVar)
              {
                Monitor.Wait(ConditionVar);
              }
            }
            break;
          //  case 0xf8:
          //    string tabnam = tabControl1.SelectedTab.Name;
          //    App.Device.InitSTab(tabnam);
          //    break;
          // these are processed in GetCommandFromBuffer
          case 0xfd:
          //  if (MicroCyDevice.EndState == 0)
          //    MicroCyDevice.EndState = 1; //start the end of well state machine
            break;
          case 0xfe:
          //  if (MicroCyDevice.EndState == 0)
          //    MicroCyDevice.EndState = 1;
            break;
          case 0xbf:
            //CalibrationViewModel.Instance.AttenuationBox[0] = exe.Parameter.ToString();
            break;
          case 0xf3:
            if (!ComponentsViewModel.Instance.SuppressWarnings)
            {
              update = () =>
              {
                ResultsViewModel.Instance.PlatePictogram.ChangeState(MicroCyDevice.ReadingRow, MicroCyDevice.ReadingCol,
                  warning: Models.WellWarningState.YellowWarning);
                if (MicroCyDevice.CurrentWellIdx < MicroCyDevice.WellsToRead) //aspirating next
                  App._nextWellWarning = true;
              };
            }
            break;
        }
        if (update != null)
          App.Current.Dispatcher.Invoke(update);
      }
      App.Current.Dispatcher.Invoke(UpdatePressureMonitor);
    }

    public static void UpdateEventCounter()
    {
      App.Current.Dispatcher.Invoke(() => MainViewModel.Instance.EventCountCurrent[0] = MicroCyDevice.BeadCount.ToString());
    }

    private static void UpdatePressureMonitor()
    {
      if (DashboardViewModel.Instance.PressureMonToggleButtonState)
        App.Device.MainCommand("Get FProperty", code: 0x22);
    }

    private static void HideChannels()
    {
      ChannelOffsetViewModel.Instance.OldBoardOffsetsVisible = Visibility.Hidden;
      Views.ChannelOffsetView.Instance.SlidersSP.Visibility = Visibility.Visible;
      Views.ChannelOffsetView.Instance.BaselineSP.Width = 180;
    }
  }
}