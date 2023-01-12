﻿using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DIOS.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ei_Dimension.Controllers;
using System.Security.Policy;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class MainButtonsViewModel
  {
    public virtual bool StartButtonEnabled { get; set; }
    public static MainButtonsViewModel Instance { get; private set; }
    public virtual ObservableCollection<string> ActiveList { get; set; }
    public virtual ObservableCollection<string> Flavor { get; set; }
    protected MainButtonsViewModel()
    {
      StartButtonEnabled = true;
      ActiveList = new ObservableCollection<string>();
      Flavor = new ObservableCollection<string> { null };
      Instance = this;
    }

    public static MainButtonsViewModel Create()
    {
      return ViewModelSource.Create(() => new MainButtonsViewModel());
    }

    public void LoadButtonClick()
    {
      UserInputHandler.InputSanityCheck();
      App.Device.LoadPlate();
    }

    public void EjectButtonClick()
    {
      UserInputHandler.InputSanityCheck();
      App.Device.EjectPlate();
    }

    public void StartButtonClick()
    {
      UserInputHandler.InputSanityCheck();
      //TODO: bad design, contains 0 can never happen here
      if (App.Device.TerminationType == Termination.MinPerRegion
          && !MapRegionsController.AreThereActiveRegions()) 
      {
        var msg = Language.Resources.ResourceManager.GetString(nameof(Language.Resources.Messages_MinPerReg_RequiresAtLeast1),
          Language.TranslationSource.Instance.CurrentCulture);
        Notification.Show(msg);
        return;
      }

      var wells = WellsSelectViewModel.Instance.OutputWells();
      if (wells.Count == 0)
      {
        var msg = Language.Resources.ResourceManager.GetString(nameof(Language.Resources.Messages_NoWellsOrTube_Selected),
          Language.TranslationSource.Instance.CurrentCulture);
        Notification.Show(msg);
        return;
      }
      App.Device.WellController.Init(wells);

      HashSet<int> regions = null;
      switch (App.Device.Mode)
      {
        case OperationMode.Normal:
          if (!MapRegionsController.AreThereActiveRegions())
          {
            //this adds region0 to ActiveRegionNums
            SelectNullRegion();
          }
          MapRegionsController.ActiveRegionNums.Add(0);
          //DefaultRegionNaming();
          regions = MapRegionsController.ActiveRegionNums;
          break;
        case OperationMode.Calibration:
          App.MapRegions.RemoveNullTextBoxes();
          CalibrationViewModel.Instance.CalJustFailed = true;
          ResultsViewModel.Instance.ShowSinglePlexResults();
          break;
        case OperationMode.Verification:
          if (MapRegionsController.ActiveVerificationRegionNums.Count != 4)
          {
            Notification.ShowError($"{MapRegionsController.ActiveVerificationRegionNums.Count} " +
                                   "out of 4 Verification Regions selected\nPlease select 4 Verification Regions");
            return;
          }
          App.MapRegions.RemoveNullTextBoxes();
          MakeNewVerificator();
          break;
      }
      MainViewModel.Instance.NavigationSelector(1);

      App.Device.Results.SetupRunRegions(regions);
      StartButtonEnabled = false;
      ResultsViewModel.Instance.ClearGraphs();
      PlatePictogramViewModel.Instance.PlatePictogram.Clear();
      ResultsViewModel.Instance.PlotCurrent();
      PlatePictogramViewModel.Instance.PlatePictogram.SetWellsForReading(wells);
      for(var i = 0; i < 10; i++)
      {
        ResultsViewModel.Instance.CurrentMfiItems[i] = "";
        ResultsViewModel.Instance.CurrentCvItems[i] = "";
      }
      if (App.Device.Normalization.IsEnabled)
        App.Logger.Log("Normalization Enabled");
      else
        App.Logger.Log("Normalization Disabled");
      App.DiosApp.Publisher.ResultsFile.MakeNew();
      App.Device.StartOperation();
    }

    public void EndButtonClick()
    {
      UserInputHandler.InputSanityCheck();
      if (!App.Device.IsMeasurementGoing)  //end button press before start, cancel work order
      {
        if (DashboardViewModel.Instance.SelectedSystemControlIndex == 1)
        {
          DashboardViewModel.Instance.WorkOrder[0] = ""; //actually questionable if not in workorder operation
        }
      }
      else
      {
        if (App.DiosApp.RunPlateContinuously)
        {
          ComponentsViewModel.Instance.ContinuousModeOn = false;
          ComponentsViewModel.Instance.ContinuousModeToggle();
        }
        App.Device.PrematureStop();
      }
    }

    private static void MakeNewVerificator()
    {
      var regions = new List<(int regionNum, double InputReporter)>();
      for (var i = 1; i < MapRegionsController.RegionsList.Count; i++)
      {
        if (MapRegionsController.ActiveVerificationRegionNums.Contains(MapRegionsController.RegionsList[i].Number))
        {
          int reg = MapRegionsController.RegionsList[i].Number;
          var inputReporter = double.Parse(MapRegionsController.RegionsList[i].TargetReporterValue[0]);
          inputReporter /= App.Device.ReporterScaling;  //adjust for scaling factor
          regions.Add((reg, inputReporter));
        }
      }
      App.Device.Verificator.Reset(regions);
    }

    private static void DefaultRegionNaming()
    {
      for (var i = 1; i < MapRegionsController.RegionsList.Count; i++)
      {
        if (MapRegionsController.ActiveRegionNums.Contains(MapRegionsController.RegionsList[i].Number) && MapRegionsController.RegionsList[i].Name[0] == "")
        {
          MapRegionsController.RegionsList[i].Name[0] = MapRegionsController.RegionsList[i].NumberString;
        }
      }
    }

    private static void SelectNullRegion()
    {
      App.MapRegions.ShowNullTextBoxes();
    }
  }
}