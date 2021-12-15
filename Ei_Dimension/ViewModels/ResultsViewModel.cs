﻿using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Ei_Dimension.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class ResultsViewModel
  {
    public virtual System.Windows.Visibility MultiPlexVisible { get; set; }
    public virtual System.Windows.Visibility SinglePlexVisible { get; set; }
    public virtual System.Windows.Visibility ValidationCoverVisible { get; set; }
    public virtual bool ResultsWaitIndicatorVisibility { get; set; }
    public virtual bool ChartWaitIndicatorVisibility { get; set; }
    public virtual ObservableCollection<bool> ScatterSelectorState { get; set; }
    public virtual ObservableCollection<HistogramData> CurrentForwardSsc { get; set; }
    public virtual ObservableCollection<HistogramData> CurrentVioletSsc { get; set; }
    public virtual ObservableCollection<HistogramData> CurrentRedSsc { get; set; }
    public virtual ObservableCollection<HistogramData> CurrentGreenSsc { get; set; }
    public virtual ObservableCollection<HistogramData> CurrentReporter { get; set; }
    public virtual ObservableCollection<HistogramData> DisplayedForwardSsc { get; set; }
    public virtual ObservableCollection<HistogramData> DisplayedVioletSsc { get; set; }
    public virtual ObservableCollection<HistogramData> DisplayedRedSsc { get; set; }
    public virtual ObservableCollection<HistogramData> DisplayedGreenSsc { get; set; }
    public virtual ObservableCollection<HistogramData> DisplayedReporter { get; set; }
    public virtual ObservableCollection<HistogramData> BackingForwardSsc { get; set; }
    public virtual ObservableCollection<HistogramData> BackingVioletSsc { get; set; }
    public virtual ObservableCollection<HistogramData> BackingRedSsc { get; set; }
    public virtual ObservableCollection<HistogramData> BackingGreenSsc { get; set; }
    public virtual ObservableCollection<HistogramData> BackingReporter { get; set; }
    public virtual ObservableCollection<HeatMapData> WorldMap { get; set; }
    public List<HeatMapData> World01Map { get; private set; }
    public List<HeatMapData> World02Map { get; private set; }
    public List<HeatMapData> World03Map { get; private set; }
    public List<HeatMapData> World12Map { get; private set; }
    public List<HeatMapData> World13Map { get; private set; }
    public List<HeatMapData> World23Map { get; private set; }
    public List<HeatMapData> CalibrationWorldMap { get; set; }
    public bool DisplaysCurrentmap { get; private set; }
    public bool FlipMapAnalysis { get; private set; }
    public List<HeatMapData> DisplayedMap { get; set; }
    public List<HeatMapData> CurrentCL01Map { get; private set; }
    public List<HeatMapData> CurrentCL02Map { get; private set; }
    public List<HeatMapData> CurrentCL03Map { get; private set; }
    public List<HeatMapData> CurrentCL12Map { get; private set; }
    public List<HeatMapData> CurrentCL13Map { get; private set; }
    public List<HeatMapData> CurrentCL23Map { get; private set; }
    public List<HeatMapData> BackingCL01Map { get; private set; }
    public List<HeatMapData> BackingCL02Map { get; private set; }
    public List<HeatMapData> BackingCL03Map { get; private set; }
    public List<HeatMapData> BackingCL12Map { get; private set; }
    public List<HeatMapData> BackingCL13Map { get; private set; }
    public List<HeatMapData> BackingCL23Map { get; private set; }
    public Dictionary<(int x, int y), int> CurrentCL01Dict { get; private set; }
    public Dictionary<(int x, int y), int> CurrentCL02Dict { get; private set; }
    public Dictionary<(int x, int y), int> CurrentCL03Dict { get; private set; }
    public Dictionary<(int x, int y), int> CurrentCL12Dict { get; private set; }
    public Dictionary<(int x, int y), int> CurrentCL13Dict { get; private set; }
    public Dictionary<(int x, int y), int> CurrentCL23Dict { get; private set; }
    public Dictionary<(int x, int y), int> BackingCL01Dict { get; private set; }
    public Dictionary<(int x, int y), int> BackingCL02Dict { get; private set; }
    public Dictionary<(int x, int y), int> BackingCL03Dict { get; private set; }
    public Dictionary<(int x, int y), int> BackingCL12Dict { get; private set; }
    public Dictionary<(int x, int y), int> BackingCL13Dict { get; private set; }
    public Dictionary<(int x, int y), int> BackingCL23Dict { get; private set; }
    public virtual ObservableCollection<DoubleHeatMapData> DisplayedAnalysisMap { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> CurrentAnalysis01Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> CurrentAnalysis02Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> CurrentAnalysis03Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> CurrentAnalysis12Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> CurrentAnalysis13Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> CurrentAnalysis23Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> BackingAnalysis01Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> BackingAnalysis02Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> BackingAnalysis03Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> BackingAnalysis12Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> BackingAnalysis13Map { get; set; }
    public virtual ObservableCollection<DoubleHeatMapData> BackingAnalysis23Map { get; set; }
    public List<MicroCy.WellResults> BackingWResults { get; set; }
    public virtual DrawingPlate PlatePictogram { get; set; }
    public virtual System.Windows.Visibility Buttons384Visible { get; set; }
    public virtual System.Windows.Visibility LeftLabel384Visible { get; set; }
    public virtual System.Windows.Visibility RightLabel384Visible { get; set; }
    public virtual System.Windows.Visibility TopLabel384Visible { get; set; }
    public virtual System.Windows.Visibility BottomLabel384Visible { get; set; }
    public virtual System.Windows.Visibility AnalysisVisible { get; set; }
    public virtual System.Windows.Visibility Analysis2DVisible { get; set; }
    public virtual System.Windows.Visibility Analysis3DVisible { get; set; }
    public virtual ObservableCollection<string> MfiItems { get; set; }
    public virtual ObservableCollection<string> CvItems { get; set; }
    public virtual string PlexButtonString { get; set; }
    public virtual ObservableCollection<bool> CornerButtonsChecked { get; set; }
    public virtual ObservableCollection<bool> CLButtonsChecked { get; set; }
    public virtual ObservableCollection<string> CLAxis { get; set; }
    public virtual ObservableCollection<string> XYCutOffString { get; set; }
    public int XYCutoff { get; set; }
    public static ResultsViewModel Instance { get; private set; }

    protected ResultsViewModel()
    {
      ScatterSelectorState = new ObservableCollection<bool> { false, false, false, false, false };
      ResultsWaitIndicatorVisibility = false;
      ChartWaitIndicatorVisibility = false;
      MultiPlexVisible = System.Windows.Visibility.Visible;
      SinglePlexVisible = System.Windows.Visibility.Hidden;
      ValidationCoverVisible = System.Windows.Visibility.Hidden;
      PlexButtonString = Language.Resources.ResourceManager.GetString(nameof(Language.Resources.Experiment_Active_Regions), Language.TranslationSource.Instance.CurrentCulture);
      byte temp = Settings.Default.ScatterGraphSelector;
      if (temp >= 16)
      {
        ScatterSelectorState[4] = true;
        temp -= 16;
      }
      if (temp >= 8)
      {
        ScatterSelectorState[3] = true;
        temp -= 8;
      }
      if (temp >= 4)
      {
        ScatterSelectorState[2] = true;
        temp -= 4;
      }
      if (temp >= 2)
      {
        ScatterSelectorState[1] = true;
        temp -= 2;
      }
      if (temp >= 1)
      {
        ScatterSelectorState[0] = true;
      }

      Instance = this;
      CurrentForwardSsc = new ObservableCollection<HistogramData>();
      CurrentVioletSsc = new ObservableCollection<HistogramData>();
      CurrentRedSsc = new ObservableCollection<HistogramData>();
      CurrentGreenSsc = new ObservableCollection<HistogramData>();
      CurrentReporter = new ObservableCollection<HistogramData>();
      BackingForwardSsc = new ObservableCollection<HistogramData>();
      BackingVioletSsc = new ObservableCollection<HistogramData>();
      BackingRedSsc = new ObservableCollection<HistogramData>();
      BackingGreenSsc = new ObservableCollection<HistogramData>();
      BackingReporter = new ObservableCollection<HistogramData>();

      CurrentCL01Dict = new Dictionary<(int x, int y), int>();
      CurrentCL02Dict = new Dictionary<(int x, int y), int>();
      CurrentCL03Dict = new Dictionary<(int x, int y), int>();
      CurrentCL12Dict = new Dictionary<(int x, int y), int>();
      CurrentCL13Dict = new Dictionary<(int x, int y), int>();
      CurrentCL23Dict = new Dictionary<(int x, int y), int>();
      BackingCL01Dict = new Dictionary<(int x, int y), int>();
      BackingCL02Dict = new Dictionary<(int x, int y), int>();
      BackingCL03Dict = new Dictionary<(int x, int y), int>();
      BackingCL12Dict = new Dictionary<(int x, int y), int>();
      BackingCL13Dict = new Dictionary<(int x, int y), int>();
      BackingCL23Dict = new Dictionary<(int x, int y), int>();

      for (var i = 0; i < HistogramData.Bins.Length; i++)
      {
        CurrentForwardSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        CurrentVioletSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        CurrentRedSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        CurrentGreenSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        CurrentReporter.Add(new HistogramData(0, HistogramData.Bins[i]));

        BackingForwardSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        BackingVioletSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        BackingRedSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        BackingGreenSsc.Add(new HistogramData(0, HistogramData.Bins[i]));
        BackingReporter.Add(new HistogramData(0, HistogramData.Bins[i]));
      }

      CurrentCL01Map = new List<HeatMapData>();
      CurrentCL02Map = new List<HeatMapData>();
      CurrentCL03Map = new List<HeatMapData>();
      CurrentCL12Map = new List<HeatMapData>();
      CurrentCL13Map = new List<HeatMapData>();
      CurrentCL23Map = new List<HeatMapData>();
      BackingCL01Map = new List<HeatMapData>();
      BackingCL02Map = new List<HeatMapData>();
      BackingCL03Map = new List<HeatMapData>();
      BackingCL12Map = new List<HeatMapData>();
      BackingCL13Map = new List<HeatMapData>();
      BackingCL23Map = new List<HeatMapData>();

      World01Map = new List<HeatMapData>();
      World02Map = new List<HeatMapData>();
      World03Map = new List<HeatMapData>();
      World12Map = new List<HeatMapData>();
      World13Map = new List<HeatMapData>();
      World23Map = new List<HeatMapData>();

      WorldMap = new ObservableCollection<HeatMapData>();
      DisplayedForwardSsc = CurrentForwardSsc;
      DisplayedVioletSsc = CurrentVioletSsc;
      DisplayedRedSsc = CurrentRedSsc;
      DisplayedGreenSsc = CurrentGreenSsc;
      DisplayedReporter = CurrentReporter;
      DisplaysCurrentmap = true;

      CurrentAnalysis01Map = new ObservableCollection<DoubleHeatMapData>();
      CurrentAnalysis02Map = new ObservableCollection<DoubleHeatMapData>();
      CurrentAnalysis03Map = new ObservableCollection<DoubleHeatMapData>();
      CurrentAnalysis12Map = new ObservableCollection<DoubleHeatMapData>();
      CurrentAnalysis13Map = new ObservableCollection<DoubleHeatMapData>();
      CurrentAnalysis23Map = new ObservableCollection<DoubleHeatMapData>();
      BackingAnalysis01Map = new ObservableCollection<DoubleHeatMapData>();
      BackingAnalysis02Map = new ObservableCollection<DoubleHeatMapData>();
      BackingAnalysis03Map = new ObservableCollection<DoubleHeatMapData>();
      BackingAnalysis12Map = new ObservableCollection<DoubleHeatMapData>();
      BackingAnalysis13Map = new ObservableCollection<DoubleHeatMapData>();
      BackingAnalysis23Map = new ObservableCollection<DoubleHeatMapData>();
      BackingWResults = new List<MicroCy.WellResults>();

      DisplayedAnalysisMap = CurrentAnalysis12Map;

      PlatePictogram = DrawingPlate.Create();
      Buttons384Visible = System.Windows.Visibility.Hidden;
      CornerButtonsChecked = new ObservableCollection<bool> { true, false, false, false };
      CLButtonsChecked = new ObservableCollection<bool> { false, false, true, false, false, true, false, false };
      CLAxis = new ObservableCollection<string> { "CL1", "CL2" };
      DisplayedMap = CurrentCL12Map;
      FlipMapAnalysis = false;
      LeftLabel384Visible = System.Windows.Visibility.Visible;
      RightLabel384Visible = System.Windows.Visibility.Hidden;
      TopLabel384Visible = System.Windows.Visibility.Visible;
      BottomLabel384Visible = System.Windows.Visibility.Hidden;
      AnalysisVisible = System.Windows.Visibility.Hidden;
      Analysis2DVisible = System.Windows.Visibility.Visible;
      Analysis3DVisible = System.Windows.Visibility.Hidden;

      MfiItems = new ObservableCollection<string>();
      CvItems = new ObservableCollection<string>();
      for (var i = 0; i < 10; i++)
      {
        MfiItems.Add("");
        CvItems.Add("");
      }
      XYCutoff = Settings.Default.XYCutOff;
      XYCutOffString = new ObservableCollection<string> { XYCutoff.ToString() };
    }

    public static ResultsViewModel Create()
    {
      return ViewModelSource.Create(() => new ResultsViewModel());
    }

    public void CLButtonClick(int CL)
    {
      if (CL < 4)
      {
        CLButtonsChecked[0] = false;
        CLButtonsChecked[1] = false;
        CLButtonsChecked[2] = false;
        CLButtonsChecked[3] = false;
        CLAxis[1] = $"CL {CL}";
      }
      else
      {
        CLButtonsChecked[4] = false;
        CLButtonsChecked[5] = false;
        CLButtonsChecked[6] = false;
        CLButtonsChecked[7] = false;
        CLAxis[0] = $"CL {CL - 4}";
      }
      CLButtonsChecked[CL] = true;
      SetDisplayedMap();
      _ = App.Current.Dispatcher.BeginInvoke((Action)(() =>
        {
          Core.DataProcessor.AnalyzeHeatMap(DisplayedMap);
        }));
    }

    public void CornerButtonClick(int corner)
    {
      switch (corner)
      {
        case 1:
          LeftLabel384Visible = System.Windows.Visibility.Visible;
          RightLabel384Visible = System.Windows.Visibility.Hidden;
          TopLabel384Visible = System.Windows.Visibility.Visible;
          BottomLabel384Visible = System.Windows.Visibility.Hidden;
          break;
        case 2:
          LeftLabel384Visible = System.Windows.Visibility.Hidden;
          RightLabel384Visible = System.Windows.Visibility.Visible;
          TopLabel384Visible = System.Windows.Visibility.Visible;
          BottomLabel384Visible = System.Windows.Visibility.Hidden;
          break;
        case 3:
          LeftLabel384Visible = System.Windows.Visibility.Visible;
          RightLabel384Visible = System.Windows.Visibility.Hidden;
          TopLabel384Visible = System.Windows.Visibility.Hidden;
          BottomLabel384Visible = System.Windows.Visibility.Visible;
          break;
        case 4:
          LeftLabel384Visible = System.Windows.Visibility.Hidden;
          RightLabel384Visible = System.Windows.Visibility.Visible;
          TopLabel384Visible = System.Windows.Visibility.Hidden;
          BottomLabel384Visible = System.Windows.Visibility.Visible;
          break;
      }
      Views.ResultsView.Instance.DrawingPlate.UnselectAllCells();
      CornerButtonsChecked[0] = false;
      CornerButtonsChecked[1] = false;
      CornerButtonsChecked[2] = false;
      CornerButtonsChecked[3] = false;
      CornerButtonsChecked[corner - 1] = true;
      PlatePictogram.ChangeCorner(corner);
    }

    public void ToCurrentButtonClick()
    {
      PlotCurrent();
      PlatePictogram.FollowingCurrentCell = true;

      int tempCorner = 1;
      if (PlatePictogram.CurrentlyReadCell.row < 8)
        tempCorner = PlatePictogram.CurrentlyReadCell.col < 12 ? 1 : 2;
      else
        tempCorner = PlatePictogram.CurrentlyReadCell.col < 12 ? 3 : 4;
      CornerButtonClick(tempCorner);
    }

    public void ClearGraphs(bool current = true)
    {
      if (current)
      {
        for (var i = 0; i < CurrentReporter.Count; i++)
        {
          CurrentForwardSsc[i].Value = 0;
          CurrentVioletSsc[i].Value = 0;
          CurrentRedSsc[i].Value = 0;
          CurrentGreenSsc[i].Value = 0;
          CurrentReporter[i].Value = 0;
        }
        CurrentCL01Map.Clear();
        CurrentCL02Map.Clear();
        CurrentCL03Map.Clear();
        CurrentCL12Map.Clear();
        CurrentCL13Map.Clear();
        CurrentCL23Map.Clear();
        CurrentCL01Dict.Clear();
        CurrentCL02Dict.Clear();
        CurrentCL03Dict.Clear();
        CurrentCL12Dict.Clear();
        CurrentCL13Dict.Clear();
        CurrentCL23Dict.Clear();
        CurrentAnalysis01Map.Clear();
        CurrentAnalysis02Map.Clear();
        CurrentAnalysis03Map.Clear();
        CurrentAnalysis12Map.Clear();
        CurrentAnalysis13Map.Clear();
        CurrentAnalysis23Map.Clear();
      }
      else
      {
        for (var i = 0; i < BackingReporter.Count; i++)
        {
          BackingForwardSsc[i].Value = 0;
          BackingVioletSsc[i].Value = 0;
          BackingRedSsc[i].Value = 0;
          BackingGreenSsc[i].Value = 0;
          BackingReporter[i].Value = 0;
        }
        BackingCL01Map.Clear();
        BackingCL02Map.Clear();
        BackingCL03Map.Clear();
        BackingCL12Map.Clear();
        BackingCL13Map.Clear();
        BackingCL23Map.Clear();
        BackingCL01Dict.Clear();
        BackingCL02Dict.Clear();
        BackingCL03Dict.Clear();
        BackingCL12Dict.Clear();
        BackingCL13Dict.Clear();
        BackingCL23Dict.Clear();
        BackingAnalysis01Map.Clear();
        BackingAnalysis02Map.Clear();
        BackingAnalysis03Map.Clear();
        lock (BackingAnalysis12Map)
        {
          BackingAnalysis12Map.Clear();
        }
        BackingAnalysis13Map.Clear();
        BackingAnalysis23Map.Clear();
        for (var i = 0; i < App.MapRegions.BackingActiveRegionsCount.Count; i++)
        {
          App.MapRegions.BackingActiveRegionsCount[i] = "0";
          App.MapRegions.BackingActiveRegionsMean[i] = "0";
        }
      }
      Views.ResultsView.Instance.ClearPoints();
    }

    public void SelectedCellChanged()
    {
      try
      {
        var temp = PlatePictogram.GetSelectedCell();
        if (temp.row == -1)
          return;
        PlatePictogram.SelectedCell = temp;
        if (temp == PlatePictogram.CurrentlyReadCell)
        {
          ToCurrentButtonClick();
          return;
        }
        PlotCurrent(false);
        ClearGraphs(false);
        FillAllData();
        PlatePictogram.FollowingCurrentCell = false;
      }
      catch { }
    }

    public void ChangeScatterLegend(int num)  //TODO: For buttons
    {
      ScatterSelectorState[num] = !ScatterSelectorState[num];
      byte res = 0;
      res += ScatterSelectorState[0] ? (byte)1 : (byte)0;
      res += ScatterSelectorState[1] ? (byte)2 : (byte)0;
      res += ScatterSelectorState[2] ? (byte)4 : (byte)0;
      res += ScatterSelectorState[3] ? (byte)8 : (byte)0;
      res += ScatterSelectorState[4] ? (byte)16 : (byte)0;
      Settings.Default.ScatterGraphSelector = res;
      Settings.Default.Save();
    }

    private async Task ParseBeadInfoAsync(string path, List<MicroCy.BeadInfoStruct> beadstructs)
    {
      List<string> LinesInFile = await Core.DataProcessor.GetDataFromFileAsync(path);
      if (LinesInFile.Count == 1 && LinesInFile[0] == " ")
      {
        App.ShowNotification("File is empty");
        return;
      }
      for (var i = 0; i < LinesInFile.Count; i++)
      {
        beadstructs.Add(Core.DataProcessor.ParseRow(LinesInFile[i]));
      }
    }

    public void FillAllData()
    {
      _ = Task.Run(async () =>
      {
        var path = PlatePictogram.GetSelectedFilePath();  //@"C:\Emissioninc\KEIZ0R-LEGION\AcquisitionData\val speed test 2E7_0.csv"; //
        if (path == null)
        {
          ResultsWaitIndicatorVisibility = false;
          ChartWaitIndicatorVisibility = false;
          return;
        }
        FillBackingWellResults();
        var beadStructslist = new List<MicroCy.BeadInfoStruct>();
        await ParseBeadInfoAsync(path, beadStructslist);
        _ = Task.Run(() => Core.DataProcessor.BinScatterData(beadStructslist, fromFile: true));
        Core.DataProcessor.BinMapData(beadStructslist, current: false);
        //DisplayedMap.Sort((x, y) => x.A.CompareTo(y.A));
        _ = App.Current.Dispatcher.BeginInvoke((Action)(() =>
        {
          Core.DataProcessor.AnalyzeHeatMap(DisplayedMap);
          FillBackingAnalysis();
        }));
        MainViewModel.Instance.EventCountLocal[0] = beadStructslist.Count.ToString();
      });
    }

    private void FillBackingWellResults()
    {
      BackingWResults.Clear();
      foreach(var reg in App.MapRegions.ActiveRegionNums)
      {
        BackingWResults.Add(new MicroCy.WellResults { regionNumber = (ushort)reg});
      }
    }

    private void FillBackingAnalysis()
    {
      foreach (var result in BackingWResults)
      {
        var RegionIndex = App.Device.ActiveMap.regions.FindIndex(r => r.Number == result.regionNumber);
        if (RegionIndex != -1)
        {
          var x = HeatMapData.bins[App.Device.ActiveMap.regions[RegionIndex].Center.x];
          var y = HeatMapData.bins[App.Device.ActiveMap.regions[RegionIndex].Center.y];
          lock (BackingAnalysis12Map)
          {
            if (result.RP1vals.Count > 0)
            {
              BackingAnalysis12Map.Add(new DoubleHeatMapData(x, y, result.RP1vals.Average()));
            }
          }
        }
      }
      ChartWaitIndicatorVisibility = false;
    }

    public void PlotCurrent(bool current = true)
    {
      DisplaysCurrentmap = current;
      SetDisplayedMap();
      if (current)
      {
        DisplayedForwardSsc = CurrentForwardSsc;
        DisplayedVioletSsc = CurrentVioletSsc;
        DisplayedRedSsc = CurrentRedSsc;
        DisplayedGreenSsc = CurrentGreenSsc;
        DisplayedReporter = CurrentReporter;
        if (App.MapRegions != null)
        {
          App.MapRegions.DisplayedActiveRegionsCount = App.MapRegions.CurrentActiveRegionsCount;
          App.MapRegions.DisplayedActiveRegionsMean = App.MapRegions.CurrentActiveRegionsMean;
        }

        _ = App.Current.Dispatcher.BeginInvoke((Action)(() =>
        {
          Core.DataProcessor.AnalyzeHeatMap(DisplayedMap);
        }));
        MainViewModel.Instance.EventCountField = MainViewModel.Instance.EventCountCurrent;
      }
      else
      {
        DisplayedForwardSsc = BackingForwardSsc;
        DisplayedVioletSsc = BackingVioletSsc;
        DisplayedRedSsc = BackingRedSsc;
        DisplayedGreenSsc = BackingGreenSsc;
        DisplayedReporter = BackingReporter;
        if (App.MapRegions != null)
        {
          ResultsWaitIndicatorVisibility = true;
          ChartWaitIndicatorVisibility = true;
          App.MapRegions.DisplayedActiveRegionsCount = App.MapRegions.BackingActiveRegionsCount;
          App.MapRegions.DisplayedActiveRegionsMean = App.MapRegions.BackingActiveRegionsMean;
        }
        MainViewModel.Instance.EventCountField = MainViewModel.Instance.EventCountLocal;
      }
    }
    /// <summary>
    /// Called on Activemap change to fill all world maps with data from files
    /// </summary>
    public void FillWorldMaps()
    {
      _ = App.Current.Dispatcher.BeginInvoke((Action)(() =>
      {
        World12Map.Clear();
        foreach (var region in App.Device.ActiveMap.regions)
        {
          foreach (var point in region.Points)
          {
            World12Map.Add(new HeatMapData((int)HeatMapData.bins[point.x], (int)HeatMapData.bins[point.y], region.Number));
          }
        }
        PlotCurrent(DisplaysCurrentmap);
      }));
    }
    
    private void SetWorldMap(List<HeatMapData> Map)
    {
      Action BuildWmap = null;
      switch (App.Device.Mode)
      {
        case MicroCy.OperationMode.Normal:
          BuildWmap = () => {
            foreach (var point in Map)
            {
              if (App.MapRegions.ActiveRegionNums.Contains(point.Region))
              {
                if (FlipMapAnalysis)
                  WorldMap.Add(new HeatMapData(point.Y, point.X));
                else
                  WorldMap.Add(new HeatMapData(point.X, point.Y));
              }
            }
          };
          break;
        case MicroCy.OperationMode.Calibration:
          Map = CalibrationWorldMap;
          BuildWmap = () => {
            foreach (var point in Map)
            {
              WorldMap.Add(new HeatMapData(point.X, point.Y));
            }
          };
          break;
        case MicroCy.OperationMode.Verification:
          BuildWmap = () => {
            foreach (var point in Map)
            {
              if (App.MapRegions.VerificationRegionNums.Contains(point.Region))
              {
                if (FlipMapAnalysis)
                  WorldMap.Add(new HeatMapData(point.Y, point.X));
                else
                  WorldMap.Add(new HeatMapData(point.X, point.Y));
              }
            }
          };
          break;
      }
      //cal worldmap is unique instance, that is produced by special function.
      //regular maps are produced in a regular way, so they can be switched with cl0-cl3 switches
      if (Map != null)
      {
        WorldMap.Clear();
        _ = App.Current.Dispatcher.BeginInvoke(BuildWmap);
      }
    }

    private void SetDisplayedMap()
    {
      FlipMapAnalysis = false;
      List<HeatMapData> WldMap = null;
      if (CLButtonsChecked[4])
      {
        if (CLButtonsChecked[1])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL01Map;
            DisplayedAnalysisMap = CurrentAnalysis01Map;
          }
          else
          {
            DisplayedMap = BackingCL01Map;
            DisplayedAnalysisMap = BackingAnalysis01Map;
          }
          WldMap = World01Map;
        }
        else if (CLButtonsChecked[2])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL02Map;
            DisplayedAnalysisMap = CurrentAnalysis02Map;
          }
          else
          {
            DisplayedMap = BackingCL02Map;
            DisplayedAnalysisMap = BackingAnalysis02Map;
          }
          WldMap = World02Map;
        }
        else if (CLButtonsChecked[3])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL03Map;
            DisplayedAnalysisMap = CurrentAnalysis03Map;
          }
          else
          {
            DisplayedMap = BackingCL03Map;
            DisplayedAnalysisMap = BackingAnalysis03Map;
          }
          WldMap = World03Map;
        }
        else
        {
          DisplayedMap = null;
          DisplayedAnalysisMap = null;
          Views.ResultsView.Instance.ClearPoints();
          WorldMap.Clear();
        }
      }
      else if (CLButtonsChecked[5])
      {
        if (CLButtonsChecked[0])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL01Map;
            DisplayedAnalysisMap = CurrentAnalysis01Map;
          }
          else
          {
            DisplayedMap = BackingCL01Map;
            DisplayedAnalysisMap = BackingAnalysis01Map;
          }
          FlipMapAnalysis = true;
          WldMap = World01Map;
        }
        else if (CLButtonsChecked[2])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL12Map;
            DisplayedAnalysisMap = CurrentAnalysis12Map;
          }
          else
          {
            DisplayedMap = BackingCL12Map;
            DisplayedAnalysisMap = BackingAnalysis12Map;
          }
          WldMap = World12Map;
        }
        else if (CLButtonsChecked[3])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL13Map;
            DisplayedAnalysisMap = CurrentAnalysis13Map;
          }
          else
          {
            DisplayedMap = BackingCL13Map;
            DisplayedAnalysisMap = BackingAnalysis13Map;
          }
          WldMap = World13Map;
        }
        else
        {
          DisplayedMap = null;
          DisplayedAnalysisMap = null;
          Views.ResultsView.Instance.ClearPoints();
          WorldMap.Clear();
        }
      }
      else if (CLButtonsChecked[6])
      {
        if (CLButtonsChecked[0])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL02Map;
            DisplayedAnalysisMap = CurrentAnalysis02Map;
          }
          else
          {
            DisplayedMap = BackingCL02Map;
            DisplayedAnalysisMap = BackingAnalysis02Map;
          }
          FlipMapAnalysis = true;
          WldMap = World02Map;
        }
        else if (CLButtonsChecked[1])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL12Map;
            DisplayedAnalysisMap = CurrentAnalysis12Map;
          }
          else
          {
            DisplayedMap = BackingCL12Map;
            DisplayedAnalysisMap = BackingAnalysis12Map;
          }
          FlipMapAnalysis = true;
          WldMap = World12Map;
        }
        else if (CLButtonsChecked[3])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL23Map;
            DisplayedAnalysisMap = CurrentAnalysis23Map;
          }
          else
          {
            DisplayedMap = BackingCL23Map;
            DisplayedAnalysisMap = BackingAnalysis23Map;
          }
          WldMap = World23Map;
        }
        else
        {
          DisplayedMap = null;
          DisplayedAnalysisMap = null;
          Views.ResultsView.Instance.ClearPoints();
          WorldMap.Clear();
        }
      }
      else if (CLButtonsChecked[7])
      {
        if (CLButtonsChecked[0])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL03Map;
            DisplayedAnalysisMap = CurrentAnalysis03Map;
          }
          else
          {
            DisplayedMap = BackingCL03Map;
            DisplayedAnalysisMap = BackingAnalysis03Map;
          }
          FlipMapAnalysis = true;
          WldMap = World03Map;
        }
        else if (CLButtonsChecked[1])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL13Map;
            DisplayedAnalysisMap = CurrentAnalysis13Map;
          }
          else
          {
            DisplayedMap = BackingCL13Map;
            DisplayedAnalysisMap = BackingAnalysis13Map;
          }
          FlipMapAnalysis = true;
          WldMap = World13Map;
        }
        else if (CLButtonsChecked[2])
        {
          if (DisplaysCurrentmap)
          {
            DisplayedMap = CurrentCL23Map;
            DisplayedAnalysisMap = CurrentAnalysis23Map;
          }
          else
          {
            DisplayedMap = BackingCL23Map;
            DisplayedAnalysisMap = BackingAnalysis23Map;
          }
          FlipMapAnalysis = true;
          WldMap = World23Map;
        }
        else
        {
          DisplayedMap = null;
          DisplayedAnalysisMap = null;
          Views.ResultsView.Instance.ClearPoints();
          WorldMap.Clear();
        }
      }
      SetWorldMap(WldMap);
    }

    public void PlexButtonClick()
    {
      if (MultiPlexVisible == System.Windows.Visibility.Visible)
      {
        ShowSinglePlexResults();
      }
      else
      {
        ShowMultiPlexResults();
      }
    }

    public void ShowSinglePlexResults()
    {
      PlexButtonString = Language.Resources.ResourceManager.GetString(nameof(Language.Resources.Experiment_Stats), Language.TranslationSource.Instance.CurrentCulture);
      MultiPlexVisible = System.Windows.Visibility.Hidden;
      SinglePlexVisible = System.Windows.Visibility.Visible;
    }

    public void ShowMultiPlexResults()
    {
      PlexButtonString = Language.Resources.ResourceManager.GetString(nameof(Language.Resources.Experiment_Active_Regions), Language.TranslationSource.Instance.CurrentCulture);
      MultiPlexVisible = System.Windows.Visibility.Visible;
      SinglePlexVisible = System.Windows.Visibility.Hidden;
    }

    public void Show3D()
    {
      Analysis3DVisible = System.Windows.Visibility.Visible;
      Analysis2DVisible = System.Windows.Visibility.Hidden;
    }

    public void Show2D()
    {
      Views.ResultsView.Instance.ShowLargeXYPlot();
      Analysis2DVisible = System.Windows.Visibility.Visible;
      Analysis3DVisible = System.Windows.Visibility.Hidden;
    }

    public void ShowAnalysis()
    {
      Views.ResultsView.Instance.Plot3DButton.IsChecked = true;
      AnalysisVisible = System.Windows.Visibility.Visible;
      Show3D();
    }

    public void ShowResults()
    {
      Views.ResultsView.Instance.ShowSmallXYPlot();
      AnalysisVisible = System.Windows.Visibility.Hidden;
      Analysis2DVisible = System.Windows.Visibility.Visible;
      Analysis3DVisible = System.Windows.Visibility.Hidden;
    }

    public void XYprint()
    {
      Views.ResultsView.Instance.PrintXY();
    }

    public void Scatterprint()
    {
      Views.ResultsView.Instance.PrintScatter();
    }

    public void AnalysisPrint()
    {
      Views.ResultsView.Instance.Print3D();
    }
  }
}