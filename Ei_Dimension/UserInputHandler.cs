﻿using Ei_Dimension.ViewModels;
using DIOS.Core;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using Ei_Dimension.Models;
using DevExpress.XtraPrinting.Native;
using DIOS.Core.HardwareIntercom;

namespace Ei_Dimension
{
  internal static class UserInputHandler
  {
    public static (PropertyInfo prop, object VM, int index, TextBox tb) SelectedTextBox
    {
      get { return _selectedTextBox; }
      set {
        if ((value.prop != null) && ((value.prop != _selectedTextBox.prop) || (value.index != _selectedTextBox.index)))
        {
          InputSanityCheck();
        }
        _selectedTextBox = value;
        if (value.prop != null)
        {
          _tempOldString = ((ObservableCollection<string>)_selectedTextBox.prop.GetValue(_selectedTextBox.VM))[_selectedTextBox.index];
          value.tb.Background = (System.Windows.Media.Brush)App.Current.Resources["MenuButtonBackgroundActive"];
        }
        else
          _tempOldString = null;
      }
    }
    private static (PropertyInfo prop, object VM, int index, TextBox tb) _selectedTextBox;
    private static string _tempOldString;
    private static string _tempNewString;
    private static bool _cancelKeyboardInjectionFlag;

    public static void InjectToFocusedTextbox(string input, bool keyboardinput = false)
    {
      if (SelectedTextBox.prop != null && !_cancelKeyboardInjectionFlag)
      {
        if (keyboardinput)
        {
          _tempNewString = input;
        }
        else
        {
          _cancelKeyboardInjectionFlag = true;
          _tempNewString = ((ObservableCollection<string>)SelectedTextBox.prop.GetValue(SelectedTextBox.VM))[SelectedTextBox.index];
          if (input == "")
          {
            if (_tempNewString.Length > 0)
              ((ObservableCollection<string>)SelectedTextBox.prop.GetValue(SelectedTextBox.VM))[SelectedTextBox.index] = _tempNewString = _tempNewString.Remove(_tempNewString.Length - 1, 1);
          }
          else
            ((ObservableCollection<string>)SelectedTextBox.prop.GetValue(SelectedTextBox.VM))[SelectedTextBox.index] = _tempNewString += input;
          _cancelKeyboardInjectionFlag = false;
        }
      }
    }

    public static void InputSanityCheck()
    {
      bool failed = false;
      if (SelectedTextBox.prop != null && _tempNewString != null)
      {
        float fRes;
        double dRes;
        int iRes;
        ushort usRes;
        byte bRes;
        string ErrorMessage = null;
        switch (SelectedTextBox.prop.Name)
        {
          case nameof(CalibrationViewModel.Instance.CompensationPercentageContent):
            if (float.TryParse(_tempNewString, out fRes))
            {
              if (fRes >= 0 && fRes <= 10)
              {
                App.Device.Compensation = fRes;
                break;
              }
            }
            failed = true;
            ErrorMessage = "[0-10]";
            break;
          case nameof(CalibrationViewModel.Instance.DNRContents):
            if (SelectedTextBox.index == 0)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 1 && fRes <= 300)
                {
                  App.Device.HDnrCoef = fRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-300]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 1 && fRes <= 40000)
                {
                  App.Device.HdnrTrans = fRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-40000]";
            }
            break;
          case nameof(DashboardViewModel.Instance.EndRead):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1)
                {
                  App.Device.MinPerRegion = iRes;
                  Settings.Default.MinPerRegion = iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[>0]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1)
                {
                  App.Device.BeadsToCapture = iRes;
                  Settings.Default.BeadsToCapture = iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[>0]";
            }
            break;
          case nameof(DashboardViewModel.Instance.Volumes):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 10 && iRes <= 100)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.Volume, VolumeType.Sample, (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[10-100]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1 && iRes <= 150)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.Volume, VolumeType.Wash, (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-150]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1 && iRes <= 500)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.Volume, VolumeType.Agitate, (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-500]";
            }
            break;
          case nameof(DashboardViewModel.Instance.Repeats):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1 && iRes <= 10)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.WashRepeatsAmount, (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-10]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1 && iRes <= 10)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.AgitateRepeatsAmount, (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-10]";
            }
            break;
          case nameof(CalibrationViewModel.Instance.EventTriggerContents):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1 && iRes <= 2000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationParameter, CalibrationParameter.Height, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-2000]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 20000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationParameter, CalibrationParameter.MinSSC, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 30000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationParameter, CalibrationParameter.MaxSSC, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-30000]";
            }
            break;
          case nameof(CalibrationViewModel.Instance.ClassificationTargetsContents):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 30000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationTarget, CalibrationTarget.CL0, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-30000]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 30000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationTarget, CalibrationTarget.CL1, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-30000]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 30000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationTarget, CalibrationTarget.CL2, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-30000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 30000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationTarget, CalibrationTarget.CL3, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-30000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 30000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationTarget, CalibrationTarget.RP1, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-30000]";
            }
            break;
          case nameof(CalibrationViewModel.Instance.AttenuationBox):
            if (int.TryParse(_tempNewString, out iRes))
            {
              if (iRes >= 0 && iRes <= 100)
              {
                App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationParameter, CalibrationParameter.Attenuation, iRes);
                break;
              }
            }
            failed = true;
            ErrorMessage = "[0-100]";
            break;
          case nameof(SyringeSpeedsViewModel.Instance.SheathSyringeParameters):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 1000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.Normal, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-1000]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 1000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.HiSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-1000]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 1000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.HiSensitivity, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-1000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 8000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.Flush, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-8000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 8000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.Pickup, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-8000]";
            }
            if (SelectedTextBox.index == 5)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 8000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.MaxSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-8000]";
            }
            break;
          case nameof(SyringeSpeedsViewModel.Instance.SamplesSyringeParameters):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 1000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.Normal, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-1000]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 1000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.HiSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-1000]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 1000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.HiSensitivity, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-1000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 8000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.Flush, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-8000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 8000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.Pickup, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-8000]";
            }
            if (SelectedTextBox.index == 5)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 8000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.MaxSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1-8000]";
            }
            break;
          case "SiPMTempCoeff":
            if (float.TryParse(_tempNewString, out fRes))
            {
              if (fRes >= -10.0000000001 && fRes <= 10.00000000000001)
              {
                App.Device.Hardware.SetHardwareParameter(DeviceParameterType.SiPMTempCoeff, fRes);
                break;
              }
            }
            failed = true;
            ErrorMessage = "[-10.0 - 10.0]";
            break;
          case "CalibrationMargin":
            if (float.TryParse(_tempNewString, out fRes))
            {
              if (fRes >= 0 && fRes < 0.1)
              {
                App.Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationMargin, fRes);
                break;
              }
            }
            failed = true;
            ErrorMessage = "[0.0 - 0.1]";
            break;
          case "ReporterScale":
            if (float.TryParse(_tempNewString, out fRes))
            {
              if (fRes > 0)
              {
                App.Device.ReporterScaling = fRes;
                Settings.Default.ReporterScaling = fRes;
                MainViewModel.Instance.SetScalingMarker(fRes);
                break;
              }
            }
            failed = true;
            ErrorMessage = "[>0]";
            break;
          case nameof(ChannelsViewModel.Instance.Bias30Parameters):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.GreenA, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.GreenB, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.GreenC, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedA, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedB, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 5)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedC, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 6)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedD, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 7)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.VioletA, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 8)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.VioletB, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            if (SelectedTextBox.index == 9)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 3500) || (App.Device.BoardVersion >= 1 && iRes <= 10000)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.ForwardScatter, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-10000]" : "[0-3500]";
            }
            break;
          case nameof(ChannelOffsetViewModel.Instance.ChannelsOffsetParameters):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.GreenA, iRes);
                  ChannelOffsetViewModel.Instance.OverrideSliderChange = true;
                  ChannelOffsetViewModel.Instance.SliderValue1 = (double) iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.GreenB, iRes);
                  ChannelOffsetViewModel.Instance.OverrideSliderChange = true;
                  ChannelOffsetViewModel.Instance.SliderValue2 = (double) iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.GreenC, iRes);
                  ChannelOffsetViewModel.Instance.OverrideSliderChange = true;
                  ChannelOffsetViewModel.Instance.SliderValue3 = (double) iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedA, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedB, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 5)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedC, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 6)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedD, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 7)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.VioletA, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 8)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.VioletB, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 9)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.ChannelOffset, Channel.ForwardScatter, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            break;
          /*
          case "ChannelsBaseline":
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0xb8, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0xb9, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0xba, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0xbb, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0xbc, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 5)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0xbd, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 6)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0xbe, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 7)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0x82, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 8)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0x83, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            if (SelectedTextBox.index == 9)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.MainCommand("Set Property", code: 0x85, parameter: (ushort)iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            break;
          */
          case nameof(MotorsViewModel.Instance.ParametersX):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 65535)
                {
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-65535]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 3000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.Slope, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-3000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 5000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.StartSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-5000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 10000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.RunSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-10000]";
            }
            if (SelectedTextBox.index == 6)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 200 && iRes <= 2000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.EncoderSteps, iRes);
                  Settings.Default.StepsPerRevX = (ushort)iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[200-2000]";
            }
            if (SelectedTextBox.index == 7)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.CurrentLimit, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            break;
          case nameof(MotorsViewModel.Instance.ParametersY):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 65535)
                {
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-65535]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 3000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.Slope, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-3000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 5000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.StartSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-5000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 10000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.RunSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-10000]";
            }
            if (SelectedTextBox.index == 6)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 200 && iRes <= 2000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.EncoderSteps, iRes);
                  Settings.Default.StepsPerRevY = (ushort)iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[200-2000]";
            }
            if (SelectedTextBox.index == 7)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.CurrentLimit, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            break;
          case nameof(MotorsViewModel.Instance.ParametersZ):
            if (SelectedTextBox.index == 0)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && iRes <= 65535)
                {
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-65535]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 3000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.Slope, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-3000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 5000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.StartSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-5000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 1000 && iRes <= 10000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.RunSpeed, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[1000-10000]";
            }
            if (SelectedTextBox.index == 6)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 200 && iRes <= 2000)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.EncoderSteps, iRes);
                  Settings.Default.StepsPerRevZ = (ushort)iRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[200-2000]";
            }
            if (SelectedTextBox.index == 7)
            {
              if (int.TryParse(_tempNewString, out iRes))
              {
                if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.CurrentLimit, iRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            }
            break;
          case nameof(MotorsViewModel.Instance.StepsParametersX):
            if (SelectedTextBox.index == 0)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate96Column1, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate96Column12, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate384Column1, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate384Column24, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Tube, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            break;
          case nameof(MotorsViewModel.Instance.StepsParametersY):
            if (SelectedTextBox.index == 0)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate96RowA, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate96RowH, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate384RowA, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate384RowP, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 20000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Tube, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-20000]";
            }
            break;
          case nameof(MotorsViewModel.Instance.StepsParametersZ):
            if (SelectedTextBox.index == 0)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 1000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.A1, fRes);
                  PlateCustomizationViewModel.Instance.DefaultPlate.A1 = fRes;
                  PlateCustomizationViewModel.Instance.UpdateDefault();
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-1000]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 1000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.A12, fRes);
                  PlateCustomizationViewModel.Instance.DefaultPlate.A12 = fRes;
                  PlateCustomizationViewModel.Instance.UpdateDefault();
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-1000]";
            }
            if (SelectedTextBox.index == 2)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 1000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.H1, fRes);
                  PlateCustomizationViewModel.Instance.DefaultPlate.H1 = fRes;
                  PlateCustomizationViewModel.Instance.UpdateDefault();
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-1000]";
            }
            if (SelectedTextBox.index == 3)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 1000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.H12, fRes);
                  PlateCustomizationViewModel.Instance.DefaultPlate.H12 = fRes;
                  PlateCustomizationViewModel.Instance.UpdateDefault();
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-1000]";
            }
            if (SelectedTextBox.index == 4)
            {
              if (float.TryParse(_tempNewString, out fRes))
              {
                if (fRes >= 0 && fRes <= 1000.0000000001)
                {
                  App.Device.Hardware.SetHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.Tube, fRes);
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-1000]";
            }
            break;
          case nameof(ComponentsViewModel.Instance.IdexTextBoxInputs):
            if (SelectedTextBox.index == 0)
            {
              if (byte.TryParse(_tempNewString, out bRes))
              {
                if (bRes >= 0 && bRes <= 255)
                {
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-255]";
            }
            if (SelectedTextBox.index == 1)
            {
              if (ushort.TryParse(_tempNewString, out usRes))
              {
                if (usRes >= 0 && usRes <= 65535)
                {
                  break;
                }
              }
              failed = true;
              ErrorMessage = "[0-65535]";
            }
            break;
          case nameof(AlignmentViewModel.Instance.ValidationTolerances):
            if (SelectedTextBox.index == 0)
            {
              if (double.TryParse(_tempNewString, out dRes))
              {
                if (dRes >= 0)
                {
                  Settings.Default.ValidatorToleranceReporter = dRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "Non-Negative";
            }
            if (SelectedTextBox.index == 1)
            {
              if (double.TryParse(_tempNewString, out dRes))
              {
                if (dRes >= 0)
                {
                  Settings.Default.ValidatorToleranceClassification = dRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "Non-Negative";
            }
            if (SelectedTextBox.index == 2)
            {
              if (double.TryParse(_tempNewString, out dRes))
              {
                if (dRes >= 0)
                {
                  Settings.Default.ValidatorToleranceMisclassification = dRes;
                  break;
                }
              }
              failed = true;
              ErrorMessage = "Non-Negative";
            }
            break;
          case nameof(FileSaveViewModel.Instance.BaseFileName):
            App.Device.Publisher.Outfilename = _tempNewString;
            Settings.Default.SaveFileName = _tempNewString;
            break;
          case nameof(ComponentsViewModel.Instance.MaxPressureBox):
            if (float.TryParse(_tempNewString, out fRes))
            {
              if (Settings.Default.PressureUnitsPSI)
              {
                if (fRes >= 2 && fRes <= 40)
                {
                  Settings.Default.MaxPressure = fRes;
                  App.Device.MaxPressure = fRes;
                  break;
                }
              }
              else
              {
                if (fRes >= 2 * ComponentsViewModel.TOKILOPASCALCOEFFICIENT && fRes <= 40 * ComponentsViewModel.TOKILOPASCALCOEFFICIENT)
                {
                  fRes /= (float)ComponentsViewModel.TOKILOPASCALCOEFFICIENT;
                  Settings.Default.MaxPressure = fRes;
                  App.Device.MaxPressure = fRes;
                  break;
                }
              }
            }
            failed = true;
            ErrorMessage = Settings.Default.PressureUnitsPSI ? "[2-40]" :
              $"[{(2 * ComponentsViewModel.TOKILOPASCALCOEFFICIENT).ToString("f3")}-{(40 * ComponentsViewModel.TOKILOPASCALCOEFFICIENT).ToString("f3")}]";
            break;
          case nameof(TemplateSelectViewModel.Instance.TemplateSaveName):
            TemplateSelectViewModel.Instance.TemplateSaveName[0] = _tempNewString;
            break;
          case nameof(MaintenanceViewModel.Instance.SanitizeSecondsContent):
            if (int.TryParse(_tempNewString, out iRes))
            {
              if (iRes >= 1 && iRes <= 100)
              {
                break;
              }
            }
            failed = true;
            ErrorMessage = "[1-100]";
            break; 
          case nameof(NormalizationViewModel.Instance.NormalizationFactor):
            if (float.TryParse(_tempNewString, out fRes))
            {
              if (fRes >= 0.9 && fRes <= 0.99)
              {
                break;
              }
            }
            failed = true;
            ErrorMessage = "[0.9-0.99]";
            break;
          case nameof(MapRegionData.MFIValue):
            if (int.TryParse(_tempNewString, out iRes))
            {
              if (iRes >= 0)// && iRes <= 100)
              {
                break;
              }
            }
            failed = true;
            ErrorMessage = ">=0";// "[1-100]";
            break;
          case nameof(ComponentsViewModel.Instance.StatisticsCutoffBox):
            if (double.TryParse(_tempNewString, out dRes))
            {
              if (dRes >= 0 && dRes <= 45)
              {
                Settings.Default.StatisticsTailDiscardPercentage = dRes / 100;
                StatisticsExtension.TailDiscardPercentage = dRes / 100;
                break;
              }
            }
            failed = true;
            ErrorMessage = "[0-45]";
            break;
          case nameof(PlateCustomizationViewModel.Instance.DACCurrentLimit):
            if (int.TryParse(_tempNewString, out iRes))
            {
              if (iRes >= 0 && ((App.Device.BoardVersion == 0 && iRes <= 4095) || (App.Device.BoardVersion >= 1 && iRes <= 65535)))
              {
                break;
              }
            }
            failed = true;
            ErrorMessage = App.Device.BoardVersion >= 1 ? "[0-65535]" : "[0-4095]";
            break;
        }
        if(VerificationViewModel.Instance.isActivePage)
        {
          if (int.TryParse(_tempNewString, out iRes))
          {
            if (iRes < 0 || iRes > 1000000)
            {
              failed = true;
              ErrorMessage = "[0-1000000]";
            }
          }
          else
          {
            failed = true;
            ErrorMessage = "[0-1000000]";
          }
        }
        Settings.Default.Save();
        if (failed)
        {
          ((ObservableCollection<string>)SelectedTextBox.prop.GetValue(SelectedTextBox.VM))[SelectedTextBox.index] = _tempOldString;
          //Notification.Show(ErrorMessage);
          SelectedTextBox.tb.Background = System.Windows.Media.Brushes.Red;
          MainViewModel.Instance.HintShow(ErrorMessage, SelectedTextBox.tb);
        }
        else
        {
          MainViewModel.Instance.HintHide();
          if (_tempNewString.TrimStart('0') != "")
          {
            var trimmed =_tempNewString.TrimStart('0');
            ((ObservableCollection<string>)SelectedTextBox.prop.GetValue(SelectedTextBox.VM))[SelectedTextBox.index] = trimmed;
            if (trimmed[0] == '.')
              ((ObservableCollection<string>)SelectedTextBox.prop.GetValue(SelectedTextBox.VM))[SelectedTextBox.index] = $"0{trimmed}";
          }
          else
            ((ObservableCollection<string>)SelectedTextBox.prop.GetValue(SelectedTextBox.VM))[SelectedTextBox.index] = "0";
          SelectedTextBox.tb.Background = (System.Windows.Media.Brush)App.Current.Resources["AppBackground"];
        }
        _tempNewString = null;
        _tempOldString = null;
      }
      else if (SelectedTextBox.prop != null && _tempNewString == null)
      {
        SelectedTextBox.tb.Background = (System.Windows.Media.Brush)App.Current.Resources["AppBackground"];
      }
      SelectedTextBox = (null, null, 0, null);
      App.UnfocusUIElement();
    }

  }
}
