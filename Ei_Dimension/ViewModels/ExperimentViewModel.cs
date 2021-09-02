﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;
using System;
using Ei_Dimension.Models;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class ExperimentViewModel
  {
    public virtual System.Windows.Visibility Table96Visible { get; set; }
    public virtual System.Windows.Visibility Table384Visible { get; set; }
    public virtual ObservableCollection<WellTableRow> Table96Wells { get; set; }
    public virtual ObservableCollection<WellTableRow> Table384Wells { get; set; }
    public virtual bool Selected96 { get; set; }
    public virtual bool Selected384 { get; set; }

    public virtual ObservableCollection<string> EndRead { get; set; }
    public virtual ObservableCollection<bool> SystemControlSelectorState { get; set; }
    public virtual string OrderSelectorState { get; set; }
    public virtual string EndReadSelectorState { get; set; }
    public virtual ObservableCollection<string> Volumes { get; set; }
    public virtual string SelectedSpeedContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> SpeedItems { get; set; }
    public virtual string SelectedClassiMapContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> ClassiMapItems { get; set; }
    public virtual string SelectedChConfigContent { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> ChConfigItems { get; set; }

    public static ExperimentViewModel Instance { get; private set; }

    private int _currentTableSize;
    private List<(int row, int col)> _selectedWell96Indices;
    private List<(int row, int col)> _selectedWell384Indices;

    protected ExperimentViewModel()
    {
      InitTables();
      ChangeWellTableSize(384);

      EndRead = new ObservableCollection<string> { "100", "500" };

      SystemControlSelectorState = new ObservableCollection<bool> { false, false, false };
      SystemControlSelectorState[App.Device.SystemControl] = true;

      OrderSelectorState = "Row";
      EndReadSelectorState = "MPR";
      Volumes = new ObservableCollection<string> { "0", "", "" };


      var RM = Language.Resources.ResourceManager;
      var curCulture = Language.TranslationSource.Instance.CurrentCulture;
      SpeedItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Normal), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Hi_Speed), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Hi_Sens), curCulture), this)
      };
      SelectedSpeedContent = SpeedItems[0].Content;

      ClassiMapItems = new ObservableCollection<DropDownButtonContents>();
      foreach (var map in App.Device.MapList)
      {
        ClassiMapItems.Add(new DropDownButtonContents(map.mapName,this));
      }
      SelectedClassiMapContent = ClassiMapItems[0].Content;

      ChConfigItems = new ObservableCollection<DropDownButtonContents>
      {
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Standard), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_Cells), curCulture), this),
        new DropDownButtonContents(RM.GetString(nameof(Language.Resources.Dropdown_FM3D), curCulture), this)
      };
      SelectedChConfigContent = ChConfigItems[0].Content;

      _selectedWell96Indices = new List<(int, int)>();
      _selectedWell384Indices = new List<(int, int)>();
      Instance = this;
    }

    public static ExperimentViewModel Create()
    {
      return ViewModelSource.Create(() => new ExperimentViewModel());
    }

    public void SelectIndices(SelectedCellsChangedEventArgs e)
    {
      IList<DataGridCellInfo> selectedCells = e.AddedCells;
      foreach (var cell in selectedCells)
      {
        var columnIndex = cell.Column.DisplayIndex;
        var rowIndex = ((WellTableRow)cell.Item).Index;
        if(_currentTableSize == 96)
          _selectedWell96Indices.Add((rowIndex, columnIndex));
        else if (_currentTableSize == 384)
          _selectedWell384Indices.Add((rowIndex, columnIndex));
      }
      IList<DataGridCellInfo> removedCells = e.RemovedCells;
      foreach (var cell in removedCells)
      {
        var columnIndex = cell.Column.DisplayIndex;
        var rowIndex = ((WellTableRow)cell.Item).Index;
        if (_currentTableSize == 96)
          _ = _selectedWell96Indices.Remove((rowIndex, columnIndex));
        else if (_currentTableSize == 384)
          _ = _selectedWell384Indices.Remove((rowIndex, columnIndex));
      }
    }

    public void AssignWellTypeButtonClick(int num)
    {
      if(_currentTableSize == 96)
      {
        foreach (var ind in _selectedWell96Indices)
        {
          Table96Wells[ind.row].SetType(ind.col, (WellType)num);
        }
      }
      else if (_currentTableSize == 384)
      {
        foreach (var ind in _selectedWell384Indices)
        {
          Table384Wells[ind.row].SetType(ind.col, (WellType)num);
        }
      }
    }

    public void ChangeWellTableSize(int num)
    {
      if (_currentTableSize == num)
        return;
      _currentTableSize = num;
      if (_currentTableSize == 96)
      {
        Table96Visible = System.Windows.Visibility.Visible;
        Table384Visible = System.Windows.Visibility.Hidden;
        Selected96 = true;
        Selected384 = false;
        foreach (var row in Table384Wells)
        {
          for(var i = 0; i < 24; i++)
          {
            row.SetType(i, WellType.Empty);
          }
        }
      }
      else if(_currentTableSize == 384)
      {
        Table96Visible = System.Windows.Visibility.Hidden;
        Table384Visible = System.Windows.Visibility.Visible;
        Selected96 = false;
        Selected384 = true;
        foreach (var row in Table96Wells)
        {
          for (var i = 0; i < 12; i++)
          {
            row.SetType(i, WellType.Empty);
          }
        }
      }
    }

    public void SystemControlSelector(byte num)
    {
      SystemControlSelectorState[0] = false;
      SystemControlSelectorState[1] = false;
      SystemControlSelectorState[2] = false;
      SystemControlSelectorState[num] = true;
      Settings.Default.SystemControl = num;
      Settings.Default.Save();
    }

    public void OrderSelector(string s)
    {
      OrderSelectorState = s;
    }

    public void EndReadSelector(string s)
    {
      switch (s)
      {
        case "MPR":
          break;
        case "Total":
          break;
        case "EOS":
          break;
      }
    }

    private void InitTables()
    {
      if(_currentTableSize == 96)
      {
        Table96Visible = System.Windows.Visibility.Visible;
        Table384Visible = System.Windows.Visibility.Hidden;
      }
      else if (_currentTableSize == 384)
      {
        Table96Visible = System.Windows.Visibility.Hidden;
        Table384Visible = System.Windows.Visibility.Visible;
      }
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
      private string _content;
      private static ExperimentViewModel _vm;
      public DropDownButtonContents(string content, ExperimentViewModel vm = null)
      {
        if (_vm == null)
        {
          _vm = vm;
        }
        Content = content;
      }

      public void Click(int num)
      {
        switch (num)
        {
          case 1:
            _vm.SelectedSpeedContent = Content;
            break;
          case 2:
            _vm.SelectedClassiMapContent = Content;
            break;
          case 3:
            _vm.SelectedChConfigContent = Content;
            break;
        }
      }
    }
  }
}