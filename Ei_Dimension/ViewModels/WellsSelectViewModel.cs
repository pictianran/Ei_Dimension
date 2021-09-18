﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Ei_Dimension.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class WellsSelectViewModel
  {
    public virtual Visibility Table96Visible { get; set; }
    public virtual Visibility Table384Visible { get; set; }
    public virtual ObservableCollection<WellTableRow> Table96Wells { get; set; }
    public virtual ObservableCollection<WellTableRow> Table384Wells { get; set; }
    public int CurrentTableSize { get; set; }
    private List<(int row, int col)> _selectedWell96Indices;
    private List<(int row, int col)> _selectedWell384Indices;
    public static WellsSelectViewModel Instance { get; private set; }

    protected WellsSelectViewModel()
    {
      InitTables();

      _selectedWell96Indices = new List<(int, int)>();
      _selectedWell384Indices = new List<(int, int)>();
      CurrentTableSize = 0;

      Instance = this;
    }

    public static WellsSelectViewModel Create()
    {
      return ViewModelSource.Create(() => new WellsSelectViewModel());
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
          Table384Visible = Visibility.Hidden;
          Table96Visible = Visibility.Hidden;
          break;
        case 96:
          Table384Visible = Visibility.Hidden;
          Table96Visible = Visibility.Visible;
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
          Table384Visible = Visibility.Visible;
          Table96Visible = Visibility.Hidden;
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
  }
}