﻿using DevExpress.Mvvm;
using System;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class MainViewModel
  {
    public virtual System.Windows.Visibility NumpadVisible { get; set; }
    public virtual System.Windows.Visibility NumpadButtonVisible { get; set; }
    public virtual System.Windows.Visibility KeyboardVisible { get; set; }
    public virtual ObservableCollection<string> EventCountField { get; set; }
    public virtual ObservableCollection<string> EventCountCurrent { get; set; }
    public virtual ObservableCollection<string> EventCountLocal { get; set; }
    public virtual System.Windows.Visibility EventCountVisible { get; set; }
    public virtual System.Windows.Visibility StartButtonsVisible { get; set; }
    public virtual System.Windows.Visibility ServiceVisibility { get; set; }
    public int ServiceVisibilityCheck { get; set; }

    public static MainViewModel Instance { get; private set; }
    private INavigationService NavigationService => this.GetService<INavigationService>();

    protected MainViewModel()
    {
      App.NumpadShow = (this.GetType().GetProperty(nameof(NumpadVisible)), this);
      App.KeyboardShow = (this.GetType().GetProperty(nameof(KeyboardVisible)), this);
      NumpadButtonVisible = System.Windows.Visibility.Visible;
      NumpadVisible = System.Windows.Visibility.Hidden;
      KeyboardVisible = System.Windows.Visibility.Hidden;
      EventCountVisible = System.Windows.Visibility.Visible;
      StartButtonsVisible = System.Windows.Visibility.Visible;
      ServiceVisibility = System.Windows.Visibility.Hidden;
      EventCountCurrent = new ObservableCollection<string> { "0" };
      EventCountLocal = new ObservableCollection<string> { "0" };
      EventCountField = EventCountCurrent;
      Instance = this;
      ServiceVisibilityCheck = 0;
    }

    public static MainViewModel Create()
    {
      return ViewModelSource.Create(() => new MainViewModel());
    }

    public void NavigateExperiment()
    {
      App.ResetFocusedTextbox();
      App.HideNumpad();
      App.HideKeyboard();
      NumpadButtonVisible = System.Windows.Visibility.Visible;
      EventCountVisible = System.Windows.Visibility.Visible;
      StartButtonsVisible = System.Windows.Visibility.Visible;
      NavigationService.Navigate("ExperimentView", null, this);
      App.Device.InitSTab("readertab");
    }

    public void NavigateResults()
    {
      App.ResetFocusedTextbox();
      App.HideNumpad();
      App.HideKeyboard();
      NumpadButtonVisible = System.Windows.Visibility.Hidden;
      EventCountVisible = System.Windows.Visibility.Visible;
      StartButtonsVisible = System.Windows.Visibility.Visible;
      NavigationService.Navigate("ResultsView", null, this);
    }

    public void NavigateMaintenance()
    {
      App.ResetFocusedTextbox();
      App.HideNumpad();
      App.HideKeyboard();
      NumpadButtonVisible = System.Windows.Visibility.Visible;
      StartButtonsVisible = System.Windows.Visibility.Hidden;
      EventCountVisible = System.Windows.Visibility.Hidden;
      NavigationService.Navigate("MaintenanceView", null, this);
    }

    public void NavigateSettings()
    {
      App.ResetFocusedTextbox();
      App.HideNumpad();
      App.HideKeyboard();
      NumpadButtonVisible = System.Windows.Visibility.Visible;
      StartButtonsVisible = System.Windows.Visibility.Hidden;
      EventCountVisible = System.Windows.Visibility.Hidden;
      NavigationService.Navigate("ServiceView", null, this);
    }

    public void NumpadToggleButton()
    {
      if (NumpadVisible == System.Windows.Visibility.Visible)
      {
        NumpadVisible = System.Windows.Visibility.Hidden;
      }
      else
      {
        //var p = tb.PointToScreen(new System.Windows.Point(0, 0));
        //double shiftX = 200;
        //double shiftY = tb.Height + 5;
        //MainWindow.Instance.Npd.Margin = new System.Windows.Thickness(p.X - shiftX, p.Y + shiftY + 5, 0, 0);
        NumpadVisible = System.Windows.Visibility.Visible;
      }
    }

    public void KeyboardToggle(System.Windows.Controls.TextBox tb)
    {
      var p = tb.PointToScreen(new System.Windows.Point(0, 0));
      double shiftX;
      double shiftY;
      double Elheight = tb.Height;
      double KbdHeight = 410;
      if (p.X > 300)
        shiftX = 300;
      else if (p.X > 250)
        shiftX = 250;
      else if (p.X > 200)
        shiftX = 200;
      else if (p.X > 150)
        shiftX = 150;
      else if (p.X > 100)
        shiftX = 100;
      else if (p.X > 50)
        shiftX = 50;
      else
        shiftX = 0;

      if (MainWindow.Instance.wndw.Height - p.Y > KbdHeight + Elheight + 5)
        shiftY = tb.Height + 5;
      else
        shiftY = p.Y - KbdHeight - 5;

      MainWindow.Instance.Kbd.Margin = new System.Windows.Thickness(p.X - shiftX, p.Y + shiftY + 5, 0, 0);
      KeyboardVisible = System.Windows.Visibility.Visible;
    }

    public void InitChildren()
    {
      NavigateExperiment();
      NavigateResults();
      NavigateMaintenance();
      NavigateSettings();
    }

    public void LogoClick()
    {
      ServiceVisibilityCheck++;
      if (ServiceVisibility == System.Windows.Visibility.Hidden && ServiceVisibilityCheck > 2)
      {
        ServiceVisibility = System.Windows.Visibility.Visible;
        ServiceVisibilityCheck = 0;
      }
      else if (ServiceVisibility == System.Windows.Visibility.Visible && ServiceVisibilityCheck > 2)
      {
        ServiceVisibility = System.Windows.Visibility.Hidden;
        ServiceVisibilityCheck = 0;
      }
    }
  }
}