﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;
using Ei_Dimension.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Reflection;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class DashboardViewModel
  {
    public virtual ObservableCollection<string> EndRead { get; set; }
    public virtual ObservableCollection<string> Volumes { get; set; }
    public virtual string SelectedSpeedContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> SpeedItems { get; set; }
    public virtual string SelectedClassiMapContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> ClassiMapItems { get; set; }
    public virtual string SelectedChConfigContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> ChConfigItems { get; set; }
    public virtual string SelectedOrderContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> OrderItems { get; set; }
    public virtual string SelectedSysControlContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> SysControlItems { get; set; }
    public virtual string SelectedEndReadContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> EndReadItems { get; set; }
    public virtual bool StartButtonEnabled { get; set; }
    public virtual ObservableCollection<string> PressureMon { get; set; }
    public virtual bool PressureMonToggleButtonState { get; set; }
    public double MaxPressure { get; set; }
    public double MinPressure { get; set; }
    public virtual ObservableCollection<string> ActiveList { get; set; }
    public virtual ObservableCollection<string> RegionsList { get; set; }
    public virtual ObservableCollection<string> RegionsNamesList { get; set; }
    public List<bool> ActiveRegions { get; set; }
    public int? SelectedRegionTextboxIndex { get; set; }

    public static DashboardViewModel Instance { get; private set; }

    public byte SelectedSystemControlIndex { get; set; }
    public byte SelectedSpeedIndex { get; set; }
    public byte SelectedChConfigIndex { get; set; }
    public byte SelectedOrderIndex { get; set; }
    public byte SelectedEndReadIndex { get; set; }
    public virtual ObservableCollection<Visibility> EndReadVisibility { get; set; }

    private bool _firstLoadflag;

    protected DashboardViewModel()
    {
      EndRead = new ObservableCollection<string> { "100", "500" };

      Volumes = new ObservableCollection<string> { "0", "", "" };

      var RM = Language.Resources.ResourceManager;
      var curCulture = Language.TranslationSource.Instance.CurrentCulture;
      SpeedItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Normal), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Hi_Speed), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Hi_Sens), curCulture), this)
      };
      SelectedSpeedIndex = 0;
      SelectedSpeedContent = SpeedItems[SelectedSpeedIndex].Content;
      DropDownButtonContents.ResetIndex();

      ClassiMapItems = new ObservableCollection<DropDownButtonContents>();
      if (App.Device.MapList.Count > 0)
      {
        foreach (var map in App.Device.MapList)
        {
          ClassiMapItems.Add(new DropDownButtonContents(map.mapName, this));
        }
      }
      else
        ClassiMapItems.Add(new DropDownButtonContents("No maps available", this));
      SelectedClassiMapContent = App.Device.ActiveMap.mapName;
      DropDownButtonContents.ResetIndex();

      ChConfigItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Standard), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Cells), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_FM3D), curCulture), this)
      };
      SelectedChConfigIndex = 0;
      SelectedChConfigContent = ChConfigItems[SelectedChConfigIndex].Content;
      DropDownButtonContents.ResetIndex();

      OrderItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Column), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Row), curCulture), this),
      };
      SelectedOrderIndex = 0; //Column
      SelectedOrderContent = OrderItems[SelectedOrderIndex].Content;
      DropDownButtonContents.ResetIndex();

      SysControlItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Experiment_Manual), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Experiment_Work_Order), curCulture), this),
        //new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Experiment_Work_Order_Plus_Bcode), curCulture), this),
      };
      SelectedSystemControlIndex = Settings.Default.SystemControl;
      SelectedSysControlContent = SysControlItems[SelectedSystemControlIndex].Content;
      DropDownButtonContents.ResetIndex();

      EndReadItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Experiment_Min_Per_Reg), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Experiment_Total_Events), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Experiment_End_of_Sample), curCulture), this),
      };
      SelectedEndReadIndex = App.Device.TerminationType;
      SelectedEndReadContent = EndReadItems[SelectedEndReadIndex].Content;
      EndReadVisibility = new ObservableCollection<Visibility>
      {
        Visibility.Hidden,
        Visibility.Hidden
      };
      EndReadVisibilitySwitch();
      DropDownButtonContents.ResetIndex();

      if (SelectedSystemControlIndex == 0)
        StartButtonEnabled = true;
      else
        StartButtonEnabled = false;

      PressureMonToggleButtonState = false;
      PressureMon = new ObservableCollection<string> {"","",""};
      ActiveList = new ObservableCollection<string>();
      RegionsList = new ObservableCollection<string>();
      RegionsNamesList = new ObservableCollection<string>();
      SelectedRegionTextboxIndex = null;
      ActiveRegions = new List<bool>();
      _firstLoadflag = false;
      Instance = this;
    }

    public static DashboardViewModel Create()
    {
      return ViewModelSource.Create(() => new DashboardViewModel());
    }

    public void PressureMonToggleButtonClick()
    {
      PressureMonToggleButtonState = !PressureMonToggleButtonState;
      MaxPressure = 0;
      MinPressure = 9999999999;
    }

    public void SetFixedVolumeButtonClick(ushort num)
    {
      App.Device.MainCommand("Set Property", code: 0xaf, parameter: num);
      Volumes[0] = num.ToString();
    }

    public void LoadButtonClick()
    {
      App.Device.MainCommand("Load Plate");
    }

    public void EjectButtonClick()
    {
      App.Device.MainCommand("Eject Plate");
    }

    public void StartButtonClick()
    {
      //read section of plate
      App.Device.ReadActive = true;
      App.Device.MainCommand("Get FProperty", code: 0x58);
      App.Device.MainCommand("Get FProperty", code: 0x68);
      App.Device.PlateReport = new MicroCy.PlateReport(); //TODO: optimize, not needed here
      App.Device.MainCommand("Get FProperty", code: 0x20); //get high dnr property
      App.Device.ClearGraphingArrays();
      ResultsViewModel.Instance.ClearGraphs();
      App.Device.SetupActiveRegions(ActiveRegions);

      SetWellsInOrder();

      //find number of wells to read
      if (App.Device.WellsInOrder.Count < 1)
        return;
      //btnEndRead.BackColor = Color.Tomato;
      StartButtonEnabled = false;

      App.Device.WellsToRead = App.Device.WellsInOrder.Count - 1;    //make zero based like well index is
      App.Device.SetAspirateParamsForWell(0);  //setup for first read
      App.Device.SetReadingParamsForWell(0);
      App.Device.MainCommand("Set Property", code: 0x19, parameter: 1); //bubble detect on
      App.Device.MainCommand("Position Well Plate");   //move motors. next position is set in properties 0xad and 0xae
      App.Device.MainCommand("Aspirate Syringe A"); //handles down and pickup sample
      App.Device.WellNext();   //save well numbers for file name
      App.Device.InitBeadRead(App.Device.ReadingRow, App.Device.ReadingCol);   //gets output file redy
      App.Device.ClearSummary();

      if (App.Device.WellsToRead == 0)    //only one well in region
        App.Device.MainCommand("Read A");
      else
      {
        App.Device.SetAspirateParamsForWell(1);
        App.Device.MainCommand("Read A Aspirate B");
      }
      App.Device.CurrentWellIdx = 0;
      if (App.Device.TerminationType != 1)    //set some limit for running to eos or if regions are wrong
        App.Device.BeadsToCapture = 100000;
      MainViewModel.Instance.NavigateResults();
    }

    public void EndButtonClick()
    {
      if (!App.Device.ReadActive)  //end button press before start, cancel work order
      {
        App.Device.MainCommand("Set Property", code: 0x17); //leds off
      }
      else
      {
        App.Device.EndState = 1;
        StartButtonEnabled = true;
        if (App.Device.WellsToRead > 0)   //if end read on tube or single well, nothing else is aspirated otherwise
          App.Device.WellsToRead = App.Device.CurrentWellIdx + 1; //just read the next well in order since it is already aspirated
      }
    }
    public void FluidicsButtonClick(int i)
    {
      string cmd = "";
      switch (i)
      {
        case 0:
          cmd = "Prime";
          break;
        case 1:
          cmd = "Wash A";
          break;
        case 2:
          cmd = "Wash B";
          break;
      }
      App.Device.MainCommand("Set Property", code: 0x19, parameter: 1); //bubble detect on
      App.Device.MainCommand(cmd);
      App.Device.MainCommand("Set Property", code: 0x19, parameter: 0); //bubble detect off
    }

    public void FocusedBox(int num)
    {
      switch (num)
      {
        case 0:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(EndRead)), this, 0);
          break; 
        case 1:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(EndRead)), this, 1);
          break;
        case 2:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(Volumes)), this, 0);
          break;
        case 3:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(Volumes)), this, 1);
          break;
        case 4:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(Volumes)), this, 2);
          break;
      }
    }

    public void TextChanged(TextChangedEventArgs e)
    {
      App.InjectToFocusedTextbox(((TextBox)e.Source).Text, true);
    }

    private MicroCy.Wells MakeWell(byte row, byte col)
    {
      MicroCy.Wells newwell = new MicroCy.Wells();
      newwell.rowIdx = row;
      newwell.colIdx = col;
      newwell.runSpeed = SelectedSpeedIndex;
      short sRes = 0;
      _ = short.TryParse(Volumes[0], out sRes);
      newwell.sampVol = sRes;
      sRes = 0;
      _ = short.TryParse(Volumes[1], out sRes);
      newwell.washVol = sRes;
      sRes = 0;
      _ = short.TryParse(Volumes[2], out sRes);
      newwell.agitateVol = sRes;
      newwell.termType = App.Device.TerminationType;
      newwell.chanConfig = SelectedChConfigIndex;
      newwell.regTermCnt = App.Device.MinPerRegion;
      newwell.termCnt = App.Device.BeadsToCapture;
      newwell.thisWellsMap = App.Device.ActiveMap;
      return newwell;
    }

    private void SetWellsInOrder()
    {
      App.Device.WellsInOrder.Clear();
      if (WellsSelectViewModel.Instance.CurrentTableSize > 1)    //TODO: platetype can be removed from device fields, as soon as workorder stuff is done
      {
        ObservableCollection<WellTableRow> plate = WellsSelectViewModel.Instance.CurrentTableSize == 96 ?
          WellsSelectViewModel.Instance.Table96Wells : WellsSelectViewModel.Instance.Table384Wells;
        if (SelectedSystemControlIndex == 0)  //manual control of plate //TODO: SystemControl can be removed from device fields maybe?
        {
          for (byte r = 0; r < plate.Count; r++)
          {
            for (byte c = 0; c < plate[r].Types.Count; c++)
            {
              if (plate[r].Types[c] != WellType.Empty)
                App.Device.WellsInOrder.Add(MakeWell(r, c));
            }
          }
          if (SelectedOrderIndex == 0)
          {
            //sort list by col/row
            App.Device.WellsInOrder.Sort((x, y) => x.colIdx.CompareTo(y.colIdx));
          }
        }
        else    //Work Order control of plate
        {
          //fill wells from work order
          App.Device.WellsInOrder = App.Device.WorkOrder.woWells;
        }
        App.Device.WellsToRead = App.Device.WellsInOrder.Count;
      }
      else if (WellsSelectViewModel.Instance.CurrentTableSize == 1)  //tube
        App.Device.WellsInOrder.Add(MakeWell(0, 0));    //a 1 record work order

    }

    public void FillRegions(bool loadByPage = false)
    {
      if (_firstLoadflag && loadByPage)
        return;
      _firstLoadflag = true;
      Views.ResultsView.Instance.ClearTable();
      Views.DashboardView.Instance.ClearTextBoxes();
      RegionsList.Clear();
      RegionsNamesList.Clear();
      ActiveRegions.Clear();
      ResultsViewModel.Instance.ActiveRegionsCount.Clear();
      ResultsViewModel.Instance.ActiveRegionsMean.Clear();
      var i = 0;
      foreach (var region in App.Device.ActiveMap.mapRegions)
      {
        RegionsList.Add(region.regionNumber.ToString());
        RegionsNamesList.Add("");
        ActiveRegions.Add(false);
        ResultsViewModel.Instance.ActiveRegionsCount.Add("");
        ResultsViewModel.Instance.ActiveRegionsMean.Add("");
        Views.DashboardView.Instance.AddTextboxes($"RegionsList[{i}]", $"RegionsNamesList[{i}]");
        i++;
      }
    }

    public void AddActiveRegion(byte num)
    {
      if (SelectedRegionTextboxIndex != null)
      {
        if (num == 1 && !ActiveRegions[(int)SelectedRegionTextboxIndex])
        {
          Views.DashboardView.Instance.ShiftTextBox(true);
          ActiveRegions[(int)SelectedRegionTextboxIndex] = true;
        }
        else if(num == 0 && ActiveRegions[(int)SelectedRegionTextboxIndex])
        {
          ActiveRegions[(int)SelectedRegionTextboxIndex] = false;
          Views.DashboardView.Instance.ShiftTextBox(false);
        }
        SelectedRegionTextboxIndex = null;
      }
    }

    private void EndReadVisibilitySwitch()
    {
      switch (SelectedEndReadIndex)
      {
        case 0:
          EndReadVisibility[0] = Visibility.Visible;
          EndReadVisibility[1] = Visibility.Hidden;
          break;
        case 1:
          EndReadVisibility[0] = Visibility.Hidden;
          EndReadVisibility[1] = Visibility.Visible;
          break;
        default:
          EndReadVisibility[0] = Visibility.Hidden;
          EndReadVisibility[1] = Visibility.Hidden;
          break;
      }
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
      private static DashboardViewModel _vm;
      public DropDownButtonContents(string content, DashboardViewModel vm = null)
      {
        if (_vm == null)
        {
          _vm = vm;
        }
        Content = content;
        Index = _nextIndex++;
      }

      public void Click(int num)
      {
        switch (num)
        {
          case 1:
            _vm.SelectedSpeedContent = Content;
            _vm.SelectedSpeedIndex = Index;
            App.Device.MainCommand("Set Property", code: 0xaa, parameter: (ushort)Index);
            break;
          case 2:
            _vm.SelectedClassiMapContent = Content;
            App.SetActiveMap(Content);
            App.Device.MainCommand("Set Property", code: 0xa9, parameter: (ushort)Index);
            _vm.FillRegions();
            break;
          case 3:
            _vm.SelectedChConfigContent = Content;
            _vm.SelectedChConfigIndex = Index;
            App.Device.MainCommand("Set Property", code: 0xc2, parameter: (ushort)Index);
            break;
          case 4:
            _vm.SelectedOrderContent = Content;
            _vm.SelectedOrderIndex = Index;
            App.Device.MainCommand("Set Property", code: 0xa8, parameter: (ushort)Index);
            break;
          case 5:
            _vm.SelectedSysControlContent = Content;
            _vm.SelectedSystemControlIndex = Index;
            App.SetSystemControl(Index);
            if (Index != 0)
            {
              ExperimentViewModel.Instance.WellSelectVisible = Visibility.Hidden;
              _vm.StartButtonEnabled = false;
            }
            else
            {
              ExperimentViewModel.Instance.WellSelectVisible = Visibility.Visible;
              _vm.StartButtonEnabled = true;
            }
            break;
          case 6:
            _vm.SelectedEndReadContent = Content;
            _vm.SelectedEndReadIndex = Index;
            App.SetTerminationType(Index);
            _vm.EndReadVisibilitySwitch();
            break;
        }
      }

      public void ForAppUpdater(int num)
      {
        switch (num)
        {
          case 1:
            _vm.SelectedSpeedContent = Content;
            _vm.SelectedSpeedIndex = Index;
            break;
          case 2:
            _vm.SelectedClassiMapContent = Content;
            _vm.FillRegions();
            break;
          case 3:
            _vm.SelectedChConfigContent = Content;
            _vm.SelectedChConfigIndex = Index;
            break;
          case 4:
            _vm.SelectedOrderContent = Content;
            _vm.SelectedOrderIndex = Index;
            break;
          case 5:
            _vm.SelectedSysControlContent = Content;
            _vm.SelectedSystemControlIndex = Index;
            App.SetSystemControl(Index);
            if (Index != 0)
            {
              ExperimentViewModel.Instance.WellSelectVisible = Visibility.Hidden;
              _vm.StartButtonEnabled = false;
            }
            else
            {
              ExperimentViewModel.Instance.WellSelectVisible = Visibility.Visible;
              _vm.StartButtonEnabled = true;
            }
            break;
          case 6:
            _vm.SelectedEndReadContent = Content;
            _vm.SelectedEndReadIndex = Index;
            App.SetTerminationType(Index);
            _vm.EndReadVisibilitySwitch();
            break;
        }
      }

      public static void ResetIndex()
      {
        _nextIndex = 0;
      }
    }
  }
}