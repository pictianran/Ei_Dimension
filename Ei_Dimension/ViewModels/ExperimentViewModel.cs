﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;
using System.Windows;
using Ei_Dimension.Models;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Linq;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class ExperimentViewModel
  {
    public virtual Visibility Table96Visible { get; set; }
    public virtual Visibility Table384Visible { get; set; }
    public virtual Visibility TableSelectorVisible { get; set; }
    public virtual Visibility WellSelectorVisible { get; set; }
    public virtual Visibility ButtonsVisible { get; set; }
    public virtual Visibility ValidateBCodeButtonVisible { get; set; }
    public virtual ObservableCollection<WellTableRow> Table96Wells { get; set; }
    public virtual ObservableCollection<WellTableRow> Table384Wells { get; set; }
    public virtual bool Selected96 { get; set; }
    public virtual bool Selected384 { get; set; }
    public int CurrentTableSize { get; private set; }

    public virtual ObservableCollection<string> EndRead { get; set; }
    public virtual ObservableCollection<bool> SystemControlSelectorState { get; set; }
    public virtual bool ValidateBCodeButtonEnabled { get; set; }
    public virtual byte OrderSelectorState { get; set; }
    public virtual ObservableCollection<bool> OrderSelectorStateBool { get; set; }
    public virtual ObservableCollection<bool> EndReadSelectorState { get; set; }
    public virtual ObservableCollection<string> Volumes { get; set; }
    public virtual string SelectedSpeedContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> SpeedItems { get; set; }
    public virtual string SelectedClassiMapContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> ClassiMapItems { get; set; }
    public virtual string SelectedChConfigContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> ChConfigItems { get; set; }

    public virtual ObservableCollection<string> EventCountField { get; set; }
    public virtual bool StartButtonEnabled { get; set; }

    public static ExperimentViewModel Instance { get; private set; }

    private byte _systemControlSelectorIndex;
    private byte _selectedSpeedIndex;
    private byte _selectedChConfigIndex;
    private List<(int row, int col)> _selectedWell96Indices;
    private List<(int row, int col)> _selectedWell384Indices;
    private INavigationService NavigationService => this.GetService<INavigationService>();

    protected ExperimentViewModel()
    {
      InitTables();
      Selected96 = false;
      Selected384 = false;
      WellSelectorVisible = Visibility.Hidden;
      ButtonsVisible = Visibility.Visible;
      EndRead = new ObservableCollection<string> { "100", "500" };

      SystemControlSelectorState = new ObservableCollection<bool> { false, false, false };
      SystemControlSelector(Settings.Default.SystemControl);
      if(_systemControlSelectorIndex == 2)
        ValidateBCodeButtonVisible = Visibility.Visible;
      else
        ValidateBCodeButtonVisible = Visibility.Hidden;
      ValidateBCodeButtonEnabled = true;
      if (_systemControlSelectorIndex == 0)
        StartButtonEnabled = true;
      else
        StartButtonEnabled = false;
      TableSelectorVisible = Settings.Default.SystemControl == 0 ? Visibility.Visible : Visibility.Hidden;

      OrderSelectorState = 0; //Column
      OrderSelectorStateBool = new ObservableCollection<bool> { true, false };
      EndReadSelectorState = new ObservableCollection<bool> { false, false, false };
      EndReadSelector(App.Device.TerminationType);

      Volumes = new ObservableCollection<string> { "0", "", "" };


      var RM = Language.Resources.ResourceManager;
      var curCulture = Language.TranslationSource.Instance.CurrentCulture;
      SpeedItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Normal), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Hi_Speed), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Hi_Sens), curCulture), this)
      };
      _selectedSpeedIndex = 0;
      SelectedSpeedContent = SpeedItems[_selectedSpeedIndex].Content;
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
      _selectedChConfigIndex = 0;
      SelectedChConfigContent = ChConfigItems[_selectedChConfigIndex].Content;
      DropDownButtonContents.ResetIndex();

      _selectedWell96Indices = new List<(int, int)>();
      _selectedWell384Indices = new List<(int, int)>();
      CurrentTableSize = 0;

      EventCountField = new ObservableCollection<string> { "" };

      Instance = this;
    }

    public static ExperimentViewModel Create()
    {
      return ViewModelSource.Create(() => new ExperimentViewModel());
    }

    public void NavigateDashboard()
    {
      App.ResetFocusedTextbox();
      App.HideNumpad();
      NavigationService.Navigate("DashboardView", null, this);
    }

    public void SelectIndices(SelectedCellsChangedEventArgs e)
    {
      IList<DataGridCellInfo> selectedCells = e.AddedCells;
      foreach (var cell in selectedCells)
      {
        var columnIndex = cell.Column.DisplayIndex;
        var rowIndex = ((WellTableRow)cell.Item).Index;
        if (CurrentTableSize == 96)
          _selectedWell96Indices.Add((rowIndex, columnIndex));
        else if (CurrentTableSize == 384)
          _selectedWell384Indices.Add((rowIndex, columnIndex));
      }
      IList<DataGridCellInfo> removedCells = e.RemovedCells;
      foreach (var cell in removedCells)
      {
        var columnIndex = cell.Column.DisplayIndex;
        var rowIndex = ((WellTableRow)cell.Item).Index;
        if (CurrentTableSize == 96)
          _ = _selectedWell96Indices.Remove((rowIndex, columnIndex));
        else if (CurrentTableSize == 384)
          _ = _selectedWell384Indices.Remove((rowIndex, columnIndex));
      }
    }

    public void AssignWellTypeButtonClick(int num)
    {
      if (CurrentTableSize == 96)
      {
        foreach (var ind in _selectedWell96Indices)
        {
          Table96Wells[ind.row].SetType(ind.col, (WellType)num);
        }
      }
      else if (CurrentTableSize == 384)
      {
        foreach (var ind in _selectedWell384Indices)
        {
          Table384Wells[ind.row].SetType(ind.col, (WellType)num);
        }
      }
    }

    public void ChangeWellTableSize(int num)
    {
      switch (num)
      {
        case 1:
          App.Device.MainCommand("Set Property", code: 0xab, parameter: 2);
          App.Device.PlateType = 2;
          break;
        case 96:
          Selected384 = false;
          Table384Visible = Visibility.Hidden;
          if (Selected96)
          {
            Selected96 = false;
            Table96Visible = Visibility.Hidden;
            WellSelectorVisible = Visibility.Hidden;
            ButtonsVisible = Visibility.Visible;
          }
          else
          {
            Selected96 = true;
            Table96Visible = Visibility.Visible;
            WellSelectorVisible = Visibility.Visible;
            ButtonsVisible = Visibility.Hidden;
          }
          if (CurrentTableSize != num)
          {
            foreach (var row in Table384Wells)
            {
              for (var i = 0; i < 24; i++)
              {
                row.SetType(i, WellType.Empty);
              }
            }
            App.Device.MainCommand("Set Property", code: 0xab, parameter: 0);
            App.Device.PlateType = 0;
          }
          break;
        case 384:
          Selected96 = false;
          Table96Visible = Visibility.Hidden;
          if (Selected384)
          {
            Selected384 = false;
            Table384Visible = Visibility.Hidden;
            WellSelectorVisible = Visibility.Hidden;
            ButtonsVisible = Visibility.Visible;
          }
          else
          {
            Selected384 = true;
            Table384Visible = Visibility.Visible;
            WellSelectorVisible = Visibility.Visible;
            ButtonsVisible = Visibility.Hidden;
          }
          if (CurrentTableSize != num)
          {
            foreach (var row in Table96Wells)
            {
              for (var i = 0; i < 12; i++)
              {
                row.SetType(i, WellType.Empty);
              }
            }
            App.Device.MainCommand("Set Property", code: 0xab, parameter: 1);
            App.Device.PlateType = 1;
          }
          break;
      }
      CurrentTableSize = num;
    }

    public void SystemControlSelector(byte num)
    {
      SystemControlSelectorState[0] = false;
      SystemControlSelectorState[1] = false;
      SystemControlSelectorState[2] = false;
      SystemControlSelectorState[num] = true;
      _systemControlSelectorIndex = num;
      App.SetSystemControl(num);
      if (num != 0)
      {
        TableSelectorVisible = Visibility.Hidden;
        Table96Visible = Visibility.Hidden;
        Table384Visible = Visibility.Hidden;
        WellSelectorVisible = Visibility.Hidden;
        Selected96 = false;
        Selected384 = false;
        if (num == 2)
        {
          ValidateBCodeButtonVisible = Visibility.Visible;
        }
        else
          ValidateBCodeButtonVisible = Visibility.Hidden;
      }
      else
      {
        TableSelectorVisible = Visibility.Visible;
        ValidateBCodeButtonVisible = Visibility.Hidden;
      }
    }

    public void OrderSelector(byte num)
    {
      OrderSelectorState = num;
      OrderSelectorStateBool[0] = false;
      OrderSelectorStateBool[1] = false;
      OrderSelectorStateBool[num] = true;
      App.Device.MainCommand("Set Property", code: 0xa8, parameter: num);
    }

    public void EndReadSelector(byte num)
    {
      EndReadSelectorState[0] = false;
      EndReadSelectorState[1] = false;
      EndReadSelectorState[2] = false;
      EndReadSelectorState[num] = true;
      App.SetTerminationType(num);
    }

    public void SetFixedVolumeButtonClick(ushort num)
    {
      App.Device.MainCommand("Set Property", code: 0xaf, parameter: num);
      Volumes[0] = num.ToString();
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
      App.Device.MainCommand("Get FProperty", code: 0x20);   //get high dnr property
      Array.Clear(App.Device.SscData, 0, 256);
      Array.Clear(App.Device.Rp1Data, 0, 256);
      //  chart1.Series["SSC"].Points.DataBindY(m_MicroCy.sscdata);
      //  chart3.Series["RP1"].Points.DataBindY(m_MicroCy.rp1data);
      SetWellsInOrder();

      //find number of wells to read
      if (App.Device.WellsInOrder.Count < 1)
        return;
      //btnEndRead.BackColor = Color.Tomato;  //TODO: ask about it
      StartButtonEnabled = false;

      App.Device.WellsToRead = App.Device.WellsInOrder.Count - 1;    //make zero based like well index is
      App.Device.SetAspirateParamsForWell(0);  //setup for first read
      App.Device.SetReadingParamsForWell(0);
      App.Device.MainCommand("Set Property", code: 0x19, parameter: 1); //bubble detect on
      App.Device.MainCommand("Position Well Plate");   //move motors. next position is set in properties 0xad and 0xae
      App.Device.MainCommand("Aspirate Syringe A"); //handles down and pickup sample
      App.Device.WellNext();   //save well numbers for file neame
      App.Device.InitBeadRead(App.Device.ReadingRow, App.Device.ReadingCol);   //gets output file redy
      App.Device.PrepareSummaryFile(); //TODO : try to move to initbeadread, there is an issue, if during runtime some box is clicked

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
        if (App.Device.WellsToRead > 0)   //if end read on tube or single well, nothing else is aspirated otherwise
          App.Device.WellsToRead = App.Device.CurrentWellIdx + 1; //just read the next well in order since it is already aspirated
      }
    }

    public void ValidateBCodeButtonClick()
    {
      App.Device.MainCommand("Set Property", code: 0xad, parameter: 1); //move plate to best image distance
      App.Device.ReadingRow = MotorsViewModel.Instance.RowColIndex.rowIndex;
      App.Device.MainCommand("Set Property", code: 0xae);
      App.Device.ReadingCol = MotorsViewModel.Instance.RowColIndex.colIndex;
      App.Device.MainCommand("Set Property", code: 0x17, parameter: 1); //leds on
      App.Device.MainCommand("Position Well Plate");
      //if (videogoing == false)
      //{
      //  FinalVideo = new VideoCaptureDevice(VideoCaptureDevices[comboBox1.SelectedIndex].MonikerString);
      //  FinalVideo.VideoResolution = FinalVideo.VideoCapabilities[7]; //It selects the default size
      //  FinalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
      //  videogoing = true;
      //}
      //
      //FinalVideo.Start();
    }

    private MicroCy.Wells MakeWell(byte row, byte col)
    {
      MicroCy.Wells newwell = new MicroCy.Wells();
      newwell.rowIdx = row;
      newwell.colIdx = col;
      newwell.runSpeed = _selectedSpeedIndex;
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
      newwell.chanConfig = _selectedChConfigIndex;
      newwell.regTermCnt = App.Device.MinPerRegion;
      newwell.termCnt = App.Device.BeadsToCapture;
      newwell.thisWellsMap = App.Device.ActiveMap;
      return newwell;
    }

    private void SetWellsInOrder()
    {
      App.Device.WellsInOrder.Clear();
      if (CurrentTableSize > 1)    //TODO: platetype can be removed from device fields, as soon as workorder stuff is done
      {
        ObservableCollection<WellTableRow> plate = CurrentTableSize == 96 ? Table96Wells : Table384Wells;
        if (_systemControlSelectorIndex == 0)  //manual control of plate //TODO: SystemControl can be removed from device fields maybe?
        {
          for (byte r = 0; r < plate.Count; r++)
          {
            for (byte c = 0; c < plate[r].Types.Count; c++)
            {
              if (plate[r].Types[c] != WellType.Empty)
                App.Device.WellsInOrder.Add(MakeWell(r, c));
            }
          }
          if (OrderSelectorState == 0)
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
      else if (CurrentTableSize == 1)  //tube
        App.Device.WellsInOrder.Add(MakeWell(0, 0));    //a 1 record work order

    }

    private void InitTables()
    {
      Table96Visible = Visibility.Hidden;
      Table384Visible = Visibility.Hidden;
      Table96Wells = new ObservableCollection<WellTableRow>();
      Table384Wells = new ObservableCollection<WellTableRow>();
      Table96Wells.Add(new WellTableRow(0, 12)); //A
      Table96Wells.Add(new WellTableRow(1, 12)); //B
      Table96Wells.Add(new WellTableRow(2, 12)); //C
      Table96Wells.Add(new WellTableRow(3, 12)); //D
      Table96Wells.Add(new WellTableRow(4, 12)); //E
      Table96Wells.Add(new WellTableRow(5, 12)); //F
      Table96Wells.Add(new WellTableRow(6, 12)); //G
      Table96Wells.Add(new WellTableRow(7, 12)); //H

      Table384Wells.Add(new WellTableRow(0, 24)); //A
      Table384Wells.Add(new WellTableRow(1, 24)); //B
      Table384Wells.Add(new WellTableRow(2, 24)); //C
      Table384Wells.Add(new WellTableRow(3, 24)); //D
      Table384Wells.Add(new WellTableRow(4, 24)); //E
      Table384Wells.Add(new WellTableRow(5, 24)); //F
      Table384Wells.Add(new WellTableRow(6, 24)); //G
      Table384Wells.Add(new WellTableRow(7, 24)); //H
      Table384Wells.Add(new WellTableRow(8, 24)); //I
      Table384Wells.Add(new WellTableRow(9, 24)); //J
      Table384Wells.Add(new WellTableRow(10, 24)); //K
      Table384Wells.Add(new WellTableRow(11, 24)); //L
      Table384Wells.Add(new WellTableRow(12, 24)); //M
      Table384Wells.Add(new WellTableRow(13, 24)); //N
      Table384Wells.Add(new WellTableRow(14, 24)); //O
      Table384Wells.Add(new WellTableRow(15, 24)); //P
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
      private static ExperimentViewModel _vm;
      public DropDownButtonContents(string content, ExperimentViewModel vm = null)
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
            _vm._selectedSpeedIndex = Index;
            App.Device.MainCommand("Set Property", code: 0xaa, parameter: (ushort)Index);
            break;
          case 2:
            _vm.SelectedClassiMapContent = Content;
            App.SetActiveMap(Content);
            App.Device.MainCommand("Set Property", code: 0xa9, parameter: (ushort)Index);
            break;
          case 3:
            _vm.SelectedChConfigContent = Content;
            _vm._selectedChConfigIndex = Index;
            App.Device.MainCommand("Set Property", code: 0xc2, parameter: (ushort)Index);
            break;
        }
      }

      public void ForAppUpdater(int num)
      {
        switch (num)
        {
          case 1:
            _vm.SelectedSpeedContent = Content;
            _vm._selectedSpeedIndex = Index;
            break;
          case 2:
            _vm.SelectedClassiMapContent = Content;
            break;
          case 3:
            _vm.SelectedChConfigContent = Content;
            _vm._selectedChConfigIndex = Index;
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