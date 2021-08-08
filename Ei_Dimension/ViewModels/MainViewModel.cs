﻿using DevExpress.Mvvm;
using System;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace Ei_Dimension.ViewModels
{
//  [POCOViewModel]
  public class MainViewModel : ViewModelBase
  {
    private INavigationService NavigationService { get { return this.GetService<INavigationService>(); } }

    public void OnViewLoaded()
    {
      NavigationService.Navigate("ResultsView", null, this);
    }

    public void NavigateDashboard()
    {
      NavigationService.Navigate("DashboardView", null, this);
    }

    public void NavigateExperiment()
    {
      NavigationService.Navigate("ExperimentView", null, this);
    }

    public void NavigateResults()
    {
      NavigationService.Navigate("ResultsView", null, this);
    }

    public void NavigateMaintenance()
    {
      NavigationService.Navigate("MaintenanceView", null, this);
    }

    public void NavigateSettings()
    {
      NavigationService.Navigate("SettingsView", null, this);
    }

  }
}