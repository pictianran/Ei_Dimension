﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class MotorsViewModel
  {
    public virtual bool PollStepActive { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> WellRowButtonItems { get; set; }
    public virtual ObservableCollection<DropDownButtonContents> WellColumnButtonItems { get; set; }
    public virtual string SelectedWellRow { get; set; }
    public virtual string SelectedWellColumn { get; set; }
    public virtual ObservableCollection<string> ParametersX { get; set; }
    public virtual ObservableCollection<string> ParametersY { get; set; }
    public virtual ObservableCollection<string> ParametersZ { get; set; }
    public virtual ObservableCollection<string> StepsParametersX { get; set; }
    public virtual ObservableCollection<string> StepsParametersY { get; set; }
    public virtual ObservableCollection<string> StepsParametersZ { get; set; }

    private int _amountOfWells;

    protected MotorsViewModel()
    {
      PollStepActive = false;
      ParametersX = new ObservableCollection<string>();
      ParametersY = new ObservableCollection<string>();
      ParametersZ = new ObservableCollection<string>();
      StepsParametersX = new ObservableCollection<string>();
      StepsParametersY = new ObservableCollection<string>();
      StepsParametersZ = new ObservableCollection<string>();
      for (var i = 0; i < 7; i++)
      {
        ParametersX.Add("");
        ParametersY.Add("");
        ParametersZ.Add("");
      }
      for (var i = 0; i < 5; i++)
      {
        StepsParametersX.Add("");
        StepsParametersY.Add("");
        StepsParametersZ.Add("");
      }
      ParametersX[1] = "Left";
      ParametersY[1] = "Front";
      ParametersZ[1] = "Up";
      _amountOfWells = 96;
      SelectedWellRow = "A";
      SelectedWellColumn = "1";
      WellRowButtonItems = new ObservableCollection<DropDownButtonContents> { new DropDownButtonContents("A", this) };
      for (var i = 1; i < 8; i++)
      {
        WellRowButtonItems.Add(new DropDownButtonContents(Convert.ToChar('A' + i).ToString()));
      }
      WellColumnButtonItems = new ObservableCollection<DropDownButtonContents>();
      for (var i = 1; i < 13; i++)
      {
        WellColumnButtonItems.Add(new DropDownButtonContents(i.ToString()));
      }
    }

    public static MotorsViewModel Create()
    {
      return ViewModelSource.Create(() => new MotorsViewModel());
    }

    public void ChangeAmountOfWells(int num)
    {
      if (_amountOfWells == num)
      {
        return;
      }
      _amountOfWells = num;
      WellRowButtonItems.Clear();
      WellColumnButtonItems.Clear();
      SelectedWellRow = "A";
      SelectedWellColumn = "1";
      switch (num)
      {
        case 96:
          WellRowButtonItems = new ObservableCollection<DropDownButtonContents> { new DropDownButtonContents("A", this) };
          for (var i = 1; i < 8; i++)
          {
            WellRowButtonItems.Add(new DropDownButtonContents(Convert.ToChar('A' + i).ToString()));
          }
          WellColumnButtonItems = new ObservableCollection<DropDownButtonContents>();
          for (var i = 1; i < 13; i++)
          {
            WellColumnButtonItems.Add(new DropDownButtonContents(i.ToString()));
          }
          break;
        case 384:
          WellRowButtonItems = new ObservableCollection<DropDownButtonContents> { new DropDownButtonContents("A", this) };
          for (var i = 1; i < 16; i++)
          {
            WellRowButtonItems.Add(new DropDownButtonContents(Convert.ToChar('A' + i).ToString()));
          }
          WellColumnButtonItems = new ObservableCollection<DropDownButtonContents>();
          for (var i = 1; i < 25; i++)
          {
            WellColumnButtonItems.Add(new DropDownButtonContents(i.ToString()));
          }
          break;
      }
    }

    public void RunMotorButtonClick(string s)
    {
      switch (s)
      {
        case "x":
          break;
        case "y":
          break;
        case "z":
          break;
      }
    }

    public void HaltMotorButtonClick(string s)
    {
      switch (s)
      {
        case "x":
          break;
        case "y":
          break;
        case "z":
          break;
      }
    }

    public void GoToWellButtonClick()
    {

    }

    public void PollStepToggleButtonClick()
    {
      PollStepActive = !PollStepActive;
    }

    public void PollStepSelector(string s)
    {
      switch (s)
      {
        case "Left":
          ParametersX[1] = s;
          break;
        case "Right":
          ParametersX[1] = s;
          break;
        case "Back":
          ParametersY[1] = s;
          break;
        case "Front":
          ParametersY[1] = s;
          break;
        case "Up":
          ParametersZ[1] = s;
          break;
        case "Down":
          ParametersZ[1] = s;
          break;
      }
    }

    public void FocusedBox(int num)
    {
      switch (num)
      {
        case 0:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersX)), this, 0);
          break;
        case 1:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersX)), this, 2);
          break;
        case 2:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersX)), this, 3);
          break;
        case 3:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersX)), this, 4);
          break;
        case 4:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersX)), this, 5);
          break;
        case 5:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersX)), this, 6);
          break;
        case 6:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersY)), this, 0);
          break;
        case 7:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersY)), this, 2);
          break;
        case 8:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersY)), this, 3);
          break;
        case 9:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersY)), this, 4);
          break;
        case 10:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersY)), this, 5);
          break;
        case 11:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersY)), this, 6);
          break;
        case 12:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersZ)), this, 0);
          break;
        case 13:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersZ)), this, 2);
          break;
        case 14:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersZ)), this, 3);
          break;
        case 15:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersZ)), this, 4);
          break;
        case 16:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersZ)), this, 5);
          break;
        case 17:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(ParametersZ)), this, 6);
          break;
        case 18:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersX)), this, 0);
          break;
        case 19:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersX)), this, 1);
          break;
        case 20:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersX)), this, 2);
          break;
        case 21:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersX)), this, 3);
          break;
        case 22:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersX)), this, 4);
          break;
        case 23:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersY)), this, 0);
          break;
        case 24:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersY)), this, 1);
          break;
        case 25:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersY)), this, 2);
          break;
        case 26:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersY)), this, 3);
          break;
        case 27:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersY)), this, 4);
          break;
        case 28:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersZ)), this, 0);
          break;
        case 29:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersZ)), this, 1);
          break;
        case 30:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersZ)), this, 2);
          break;
        case 31:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersZ)), this, 3);
          break;
        case 32:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(StepsParametersZ)), this, 4);
          break;
      }
    }

    public class DropDownButtonContents
    {
      public string Content { get; set; }
      private static MotorsViewModel _vm;
      public DropDownButtonContents(string content, MotorsViewModel vm = null)
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
            _vm.SelectedWellRow = Content;
            break;
          case 2:
            _vm.SelectedWellColumn = Content;
            break;
        }
      }
    }
  }
}