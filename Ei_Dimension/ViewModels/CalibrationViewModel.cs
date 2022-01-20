﻿using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Ei_Dimension.Models;
using System.Collections.Generic;
using System.Windows.Media;
using System;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class CalibrationViewModel
  {
    public virtual string SelectedGatingContent { get; set; }
    public byte SelectedGatingIndex { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> GatingItems { get; }
    public virtual ObservableCollection<string> EventTriggerContents { get; set; }
    public virtual ObservableCollection<string> ClassificationTargetsContents { get; set; }
    public virtual ObservableCollection<string> CompensationPercentageContent { get; set; }
    public virtual ObservableCollection<string> DNRContents { get; set; }
    public virtual ObservableCollection<string> AttenuationBox { get; set; }
    public byte CalFailsInARow { get; set; }
    public bool CalJustFailed { get; set; }

    public static CalibrationViewModel Instance { get; private set; }


    protected CalibrationViewModel()
    {
      var RM = Language.Resources.ResourceManager;
      var curCulture = Language.TranslationSource.Instance.CurrentCulture;
      GatingItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_None), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Green_SSC), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Red_SSC), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Green_Red_SSC), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Rp_bg), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Green_Rp_bg), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Red_Rp_bg), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Green_Red_Rp_bg), curCulture), this)
      };
      SelectedGatingIndex = 0;
      SelectedGatingContent = GatingItems[SelectedGatingIndex].Content;
      EventTriggerContents = new ObservableCollection<string> {"", App.Device.MapCtroller.ActiveMap.calParams.minmapssc.ToString(), App.Device.MapCtroller.ActiveMap.calParams.maxmapssc.ToString()};

      ClassificationTargetsContents = new ObservableCollection<string> { "1", "1", "1", "1", "3500"};

      CompensationPercentageContent = new ObservableCollection<string> { MicroCy.InstrumentParameters.Calibration.Compensation.ToString() };
      DNRContents = new ObservableCollection<string> { "", MicroCy.InstrumentParameters.Calibration.HdnrTrans.ToString() };

      AttenuationBox = new ObservableCollection<string> { App.Device.MapCtroller.ActiveMap.calParams.att.ToString() };

      CalFailsInARow = 0;
      CalJustFailed = true;

      Instance = this;
    }

    public static CalibrationViewModel Create()
    {
      return ViewModelSource.Create(() => new CalibrationViewModel());
    }

    public void CalibrationSuccess()
    {
      System.Threading.Thread.Sleep(1000);  //not really needed
      App.Device.MainCommand("Get Property", code: 0x24);
      App.Device.MainCommand("Get Property", code: 0x25);
      App.Device.MainCommand("Get Property", code: 0x26);
      App.Device.MainCommand("Get Property", code: 0x28);
      App.Device.MainCommand("Get Property", code: 0x29);
      App.Device.MainCommand("Get Property", code: 0x2a);
      App.Device.MainCommand("Get Property", code: 0x2c);
      App.Device.MainCommand("Get Property", code: 0x2d);
      App.Device.MainCommand("Get Property", code: 0x2e);
      App.Device.MainCommand("Get Property", code: 0x2f);
      System.Threading.Thread.Sleep(1000);
      Action Cancel = () =>
      {
        DashboardViewModel.Instance.CalModeOn = false;
        DashboardViewModel.Instance.CalModeToggle();
      };
      Action Save = () =>
      {
        var res = App.Device.MapCtroller.SaveCalVals(new MicroCy.MapCalParameters
        {
          TempCl0 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[8]),
          TempCl1 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[5]),
          TempCl2 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[6]),
          TempCl3 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[3]),
          TempRedSsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[4]),
          TempGreenSsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[0]),
          TempVioletSsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[7]),
          TempRpMaj = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[1]),
          TempRpMin = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[2]),
          TempFsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[9]),
          Compensation = float.Parse(CompensationPercentageContent[0]),
          Gating = SelectedGatingIndex,
          Height = short.Parse(EventTriggerContents[0]),
          MinSSC = ushort.Parse(EventTriggerContents[1]),
          MaxSSC = ushort.Parse(EventTriggerContents[2]),
          DNRCoef = float.Parse(DNRContents[0]),
          DNRTrans = float.Parse(DNRContents[1]),
          Attenuation = int.Parse(AttenuationBox[0]),
          CL0 = int.Parse(ClassificationTargetsContents[0]),
          CL1 = int.Parse(ClassificationTargetsContents[1]),
          CL2 = int.Parse(ClassificationTargetsContents[2]),
          CL3 = int.Parse(ClassificationTargetsContents[3]),
          RP1 = int.Parse(ClassificationTargetsContents[4]),
          Caldate = DateTime.Now.ToString("dd.MM.yyyy", new System.Globalization.CultureInfo("en-GB")),
          Valdate = null
        });
        if (!res)
        {
          App.Current.Dispatcher.Invoke(() =>
            Notification.Show("Save failed"));
          return;
        }
        DashboardViewModel.Instance.CaliDateBox[0] = App.Device.MapCtroller.ActiveMap.caltime;
        Cancel.Invoke();
      };
      Notification.ShowLocalized(nameof(Language.Resources.Calibration_Success), Save, nameof(Language.Resources.Calibration_Save_Calibration_To_Map),
        Cancel, nameof(Language.Resources.Calibration_Cancel_Calibration), Brushes.Green);
    }

    public void MakeCalMap()
    {
      ResultsViewModel.Instance.WrldMap.CalibrationMap = new List<HeatMapData>();
      int cl1Index = Array.BinarySearch(HeatMapData.bins, int.Parse(ClassificationTargetsContents[1]));
      if (cl1Index < 0)
        cl1Index = ~cl1Index;
      int cl2Index = Array.BinarySearch(HeatMapData.bins, int.Parse(ClassificationTargetsContents[2]));
      if (cl2Index < 0)
        cl2Index = ~cl2Index;
      for (var i = -5; i < 6; i++)
      {
        for (var j = -6; j < 7; j++)
        {
          if(Math.Pow(i, 2) + Math.Pow(j, 2) <= 16
            && cl1Index + i >= 0 && cl1Index + i < 256 && cl2Index + j >= 0 && cl2Index + j < 256)
            ResultsViewModel.Instance.WrldMap.CalibrationMap.Add(
              new HeatMapData((int)HeatMapData.bins[cl1Index + i], (int)HeatMapData.bins[cl2Index + j]));
        }
      }
    }

    public void SaveCalButtonClick()
    {
      UserInputHandler.InputSanityCheck();
      App.Device.MainCommand("Get Property", code: 0x24);
      App.Device.MainCommand("Get Property", code: 0x25);
      App.Device.MainCommand("Get Property", code: 0x26);
      App.Device.MainCommand("Get Property", code: 0x28);
      App.Device.MainCommand("Get Property", code: 0x29);
      App.Device.MainCommand("Get Property", code: 0x2a);
      App.Device.MainCommand("Get Property", code: 0x2c);
      App.Device.MainCommand("Get Property", code: 0x2d);
      App.Device.MainCommand("Get Property", code: 0x2e);
      App.Device.MainCommand("Get Property", code: 0x2f);
      System.Threading.Thread.Sleep(1000);
      var res = App.Device.MapCtroller.SaveCalVals(new MicroCy.MapCalParameters
      {
        TempCl0 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[8]),
        TempCl1 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[5]),
        TempCl2 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[6]),
        TempCl3 = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[3]),
        TempRedSsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[4]),
        TempGreenSsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[0]),
        TempVioletSsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[7]),
        TempRpMaj = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[1]),
        TempRpMin = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[2]),
        TempFsc = int.Parse(ChannelsViewModel.Instance.Bias30Parameters[9]),
        Compensation = float.Parse(CompensationPercentageContent[0]),
        Gating = SelectedGatingIndex,
        Height = short.Parse(EventTriggerContents[0]),
        MinSSC = ushort.Parse(EventTriggerContents[1]),
        MaxSSC = ushort.Parse(EventTriggerContents[2]),
        DNRCoef = float.Parse(DNRContents[0]),
        DNRTrans = float.Parse(DNRContents[1]),
        Attenuation = int.Parse(AttenuationBox[0]),
        CL0 = int.Parse(ClassificationTargetsContents[0]),
        CL1 = int.Parse(ClassificationTargetsContents[1]),
        CL2 = int.Parse(ClassificationTargetsContents[2]),
        CL3 = int.Parse(ClassificationTargetsContents[3]),
        RP1 = int.Parse(ClassificationTargetsContents[4]),
        Caldate = null,
        Valdate = null
      });
      if (!res)
      {
        App.Current.Dispatcher.Invoke(() =>
          Notification.Show("Save failed"));
        return;
      }
      Notification.Show($"Calibration Parameters Saved to map {App.Device.MapCtroller.ActiveMap.mapName}");
    }

    public void FocusedBox(int num)
    {
      switch (num)
      {
        case 0:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(CompensationPercentageContent)), this, 0, Views.CalibrationView.Instance.TB0);
          MainViewModel.Instance.NumpadToggleButton(Views.CalibrationView.Instance.TB0);
          break;
        case 1:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(EventTriggerContents)), this, 0, Views.CalibrationView.Instance.TB1);
          MainViewModel.Instance.NumpadToggleButton(Views.CalibrationView.Instance.TB1);
          break;
        case 2:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(EventTriggerContents)), this, 1, Views.CalibrationView.Instance.TB2);
          MainViewModel.Instance.NumpadToggleButton(Views.CalibrationView.Instance.TB2);
          break;
        case 3:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(EventTriggerContents)), this, 2, Views.CalibrationView.Instance.TB3);
          MainViewModel.Instance.NumpadToggleButton(Views.CalibrationView.Instance.TB3);
          break;
        case 4:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(DNRContents)), this, 0, Views.CalibrationView.Instance.TB4);
          MainViewModel.Instance.NumpadToggleButton(Views.CalibrationView.Instance.TB4);
          break;
        case 5:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(DNRContents)), this, 1, Views.CalibrationView.Instance.TB5);
          MainViewModel.Instance.NumpadToggleButton(Views.CalibrationView.Instance.TB5);
          break;
        case 6:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(ClassificationTargetsContents)), this, 0, (TextBox)Views.CalibrationView.Instance.targetsSP.Children[0]);
          MainViewModel.Instance.NumpadToggleButton((TextBox)Views.CalibrationView.Instance.targetsSP.Children[0]);
          break;
        case 7:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(ClassificationTargetsContents)), this, 1, (TextBox)Views.CalibrationView.Instance.targetsSP.Children[1]);
          MainViewModel.Instance.NumpadToggleButton((TextBox)Views.CalibrationView.Instance.targetsSP.Children[1]);
          break;
        case 8:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(ClassificationTargetsContents)), this, 2, (TextBox)Views.CalibrationView.Instance.targetsSP.Children[2]);
          MainViewModel.Instance.NumpadToggleButton((TextBox)Views.CalibrationView.Instance.targetsSP.Children[2]);
          break;
        case 9:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(ClassificationTargetsContents)), this, 3, (TextBox)Views.CalibrationView.Instance.targetsSP.Children[3]);
          MainViewModel.Instance.NumpadToggleButton((TextBox)Views.CalibrationView.Instance.targetsSP.Children[3]);
          break;
        case 10:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(ClassificationTargetsContents)), this, 4, (TextBox)Views.CalibrationView.Instance.targetsSP.Children[4]);
          MainViewModel.Instance.NumpadToggleButton((TextBox)Views.CalibrationView.Instance.targetsSP.Children[4]);
          break;
        case 11:
          UserInputHandler.SelectedTextBox = (this.GetType().GetProperty(nameof(AttenuationBox)), this, 0, Views.CalibrationView.Instance.TB10);
          MainViewModel.Instance.NumpadToggleButton(Views.CalibrationView.Instance.TB10);
          break;
      }
    }

    public void TextChanged(TextChangedEventArgs e)
    {
      UserInputHandler.InjectToFocusedTextbox(((TextBox)e.Source).Text, true);
    }

    public void DropPress()
    {
      UserInputHandler.InputSanityCheck();
    }

    public class DropDownButtonContents : Core.ObservableObject
    {
      public string Content
      {
        get => _content;
        set
        {
          _content = value;
          OnPropertyChanged();
        }
      }
      public byte Index { get; set; }
      private static byte _nextIndex = 0;
      private string _content;
      private static CalibrationViewModel _vm;
      public DropDownButtonContents(string content, CalibrationViewModel vm = null)
      {
        if (_vm == null)
        {
          _vm = vm;
        }
        Content = content;
        Index = _nextIndex++;
      }

      public void Click()
      {
        _vm.SelectedGatingContent = Content;
        _vm.SelectedGatingIndex = Index;
        App.Device.MainCommand("Set Property", code: 0xca, parameter: (ushort)Index);
        App.Device.ScatterGate = Index;
      }

      public void ForAppUpdater()
      {
        _vm.SelectedGatingContent = Content;
        App.Device.ScatterGate = Index;
        _vm.SelectedGatingIndex = Index;
      }

      public static void ResetIndex()
      {
        _nextIndex = 0;
      }
    }
  }
}