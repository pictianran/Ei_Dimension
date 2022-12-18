﻿using System.Reflection;
using System;
using System.Windows;
using Ei_Dimension.ViewModels;
using DIOS.Core;
using System.IO;
using System.Configuration;
using System.Text;
using DIOS.Core.HardwareIntercom;
using Ei_Dimension.Cache;
using Ei_Dimension.Controllers;
using DIOSApplication;

namespace Ei_Dimension
{
  public partial class App : Application
  {
    public static DIOSApp DiosApp { get; } = new DIOSApp();
    public static (PropertyInfo prop, object VM) NumpadShow { get; set; }
    public static (PropertyInfo prop, object VM) KeyboardShow { get; set; }
    public static Device Device { get; private set; }
    public static ResultsCache Cache { get; } = new ResultsCache();
    public static MapRegionsController MapRegions { get; set; }
    public static bool _nextWellWarning;
    public static bool MakeLegacyPlateReport { get; set; } = Settings.Default.LegacyPlateReport;

    private static bool _workOrderPending;
    private static FileSystemWatcher _workOrderWatcher;
    private static readonly TextBoxHandler _textBoxHandler = new TextBoxHandler();

    public App()
    {
      CorruptSettingsChecker();
      InitApp(null);
    }

    public App(Device device)
    {
      CorruptSettingsChecker();
      InitApp(device);
    }

    private static void CorruptSettingsChecker()
    {
      try
      {
        Settings.Default.Reload();
        var value = Settings.Default.DefaultMap;
      }
      catch (ConfigurationErrorsException ex)
      {
        string filename = ((ConfigurationErrorsException)ex.InnerException).Filename;
        File.Delete(filename);
        StartupFinalizer.SettingsWiped = true;
        MessageBox.Show("Settings file is Corrupted.\n\nPlease Restart the Application.");
        Environment.Exit(1);
      }
    }

    public static int GetMapIndex(string mapName)
    {
      var mapList = DiosApp.MapController.MapList;
      int i = 0;
      for (; i < mapList.Count; i++)
      {
        if (mapList[i].mapName == mapName)
          break;
      }
      if (i == mapList.Count)
        i = -1;
      return i;
    }

    public static void SetActiveMap(string mapName)
    {
      for (var i = 0; i < DiosApp.MapController.MapList.Count; i++)
      {
        if (DiosApp.MapController.MapList[i].mapName == mapName)
        {
          DiosApp.MapController.SetMap(DiosApp.MapController.MapList[i]);
          Settings.Default.DefaultMap = i;
          Settings.Default.Save();
          break;
        }
      }
    }

    public static void SetSystemControl(byte num)
    {
      Device.Control = (SystemControl)num;
      Settings.Default.SystemControl = num;
      Settings.Default.Save();
    }

    public static void SetTerminationType(byte num)
    {
      Device.TerminationType = (Termination)num;
      Settings.Default.EndRead = num;
      Settings.Default.Save();
    }

    public static void Export(DevExpress.Xpf.Charts.ChartControlBase chart, in int dpi)
    {
      var options = new DevExpress.XtraPrinting.ImageExportOptions
      {
        TextRenderingMode = DevExpress.XtraPrinting.TextRenderingMode.SingleBitPerPixelGridFit,
        Resolution = dpi,
        Format = new System.Drawing.Imaging.ImageFormat(System.Drawing.Imaging.ImageFormat.Png.Guid)
      };
      string date = DateTime.Now.ToString("dd.MM.yyyy.hh-mm-ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
      try
      {
        App.Device.Publisher.OutDirCheck();
        if (!Directory.Exists(App.Device.Publisher.Outdir + "\\SavedImages"))
          Directory.CreateDirectory(App.Device.Publisher.Outdir + "\\SavedImages");
        chart.ExportToImage(App.Device.Publisher.Outdir + @"\SavedImages\" + date + ".png", options);
      }
      catch
      {
        App.Current.Dispatcher.Invoke(() =>
          Notification.Show("Save failed"));
      }
    }

    public static void HideNumpad()
    {
      UserInputHandler.InputSanityCheck();
      NumpadShow.prop.SetValue(NumpadShow.VM, Visibility.Hidden);
    }

    public static void HideKeyboard()
    {
      UserInputHandler.InputSanityCheck();
      KeyboardShow.prop.SetValue(KeyboardShow.VM, Visibility.Hidden);
    }

    public static void UnfocusUIElement()
    {
      if (Ei_Dimension.MainWindow.Instance == null)
        return;
      System.Windows.Input.FocusManager.SetFocusedElement(System.Windows.Input.FocusManager.GetFocusScope(Ei_Dimension.MainWindow.Instance), null);
      System.Windows.Input.Keyboard.ClearFocus();
    }

    /*
    private static void WorkOrderHandler()
    {
      //see if work order is available
      if (DashboardViewModel.Instance.SelectedSystemControlIndex != 0)
      {
        if (Device.IsNewWorkOrder())
        {
          DashboardViewModel.Instance.WorkOrder[0] = Device.WorkOrderName;
          _workOrderPending = true;
        }
      }
      if (_workOrderPending)
      {
        if (DashboardViewModel.Instance.SelectedSystemControlIndex == 1)  //no barcode required so allow start
        {
          MainButtonsViewModel.Instance.StartButtonEnabled = true;
          _workOrderPending = false;
        }
        else if (DashboardViewModel.Instance.SelectedSystemControlIndex == 2)    //barcode required
        {
          //if (videogoing & (video != null))
          //{
          //  BarcodeResult plateGUID = BarcodeReader.QuicklyReadOneBarcode(video, BarcodeEncoding.PDF417, true);
          //  if (plateGUID != null)
          //    pltNumbertb.Text = plateGUID.Value;
          //}
          //else
          //  DashboardViewModel.Instance.ValidateBCodeButtonEnabled = true;
          //  if (DashboardViewModel.Instance.WorkOrder[0] == pltNumbertb.Text)
          //  {
          //    DashboardViewModel.Instance.StartButtonEnabled = true;
          //    _workOrderPending = false;
          //    Device.MainCommand("Set Property", code: 0x17); //leds off
          //    DashboardViewModel.Instance.ValidateBCodeButtonEnabled = false;
          //  }
          //  else
          //  {
          //    //  pltNumbertb.BackColor = Color.Red;
          //  }
        }
      }
    }
    */
    
    private void StartingToReadWellEventHandler(object sender, ReadingWellEventArgs e)
    {
      MultiTube.GetModifiedWellIndexes(e, out var row, out var col);

      //ResultsViewModel.Instance.PlatePictogramIsCovered = Visibility.Visible; //TODO: temporary solution

      PlatePictogramViewModel.Instance.PlatePictogram.CurrentlyReadCell = (row, col);
      PlatePictogramViewModel.Instance.PlatePictogram.ChangeState(row, col, Models.WellType.NowReading, GetWarningState(), FilePath: e.FilePath);
      
      App.Current.Dispatcher.Invoke(() =>
      {
        PlatePictogramViewModel.Instance.CornerButtonClick(Models.DrawingPlate.CalculateCorner(row, col));
        ResultsViewModel.Instance.ClearGraphs();
      });
      ActiveRegionsStatsController.Instance.ResetCurrentActiveRegionsDisplayedStats();
    }

    private static Models.WellWarningState GetWarningState()

    {
      if (_nextWellWarning)
      {
        _nextWellWarning = false;
        return Models.WellWarningState.YellowWarning;
      }
      return Models.WellWarningState.OK;
    }

    public void FinishedReadingWellEventHandler(object sender, ReadingWellEventArgs e)
    {
      var type = GetWellStateForPictogram();
      
      MultiTube.GetModifiedWellIndexes(e, out var row, out var col, proceed:true);

      //Cache.Store(row, col);
      //ResultsViewModel.Instance.PlatePictogramIsCovered = Visibility.Hidden; //TODO: temporary solution

      PlatePictogramViewModel.Instance.PlatePictogram.CurrentlyReadCell = (-1, -1);
      PlatePictogramViewModel.Instance.PlatePictogram.ChangeState(row, col, type);
      SavePlateState();
      if (Device.Control == SystemControl.WorkOrder)
      {
        App.Current.Dispatcher.Invoke(() => DashboardViewModel.Instance.WorkOrder[0] = ""); //actually questionable if not in workorder operation
      }
    }

    private static Models.WellType GetWellStateForPictogram()
    {
      var type = Models.WellType.Success;

      if (Device.Mode != OperationMode.Normal
          || Device.TerminationType != Termination.MinPerRegion)
        return type;

      var lacking = Device.Results.MinPerRegionAchieved();
      //not achieved
      if (lacking < 0)
      {
        //if lacking more then 25% of minperregion beads 
        if (-lacking > Device.MinPerRegion * 0.25)
        {
          type = Models.WellType.Fail;
        }
        else
        {
          type = Models.WellType.LightFail;
        }
      }
      return type;
    }

    public void FinishedMeasurementEventHandler(object sender, EventArgs e)
    {
      MainButtonsViewModel.Instance.StartButtonEnabled = true;
      PlatePictogramViewModel.Instance.PlatePictogram.CurrentlyReadCell = (-1, -1);
      switch (Device.Mode)
      {
        case OperationMode.Normal:
          OutputLegacyReport();
          if (DiosApp.RunPlateContinuously)
          {
            MainButtonsViewModel.Instance.StartButtonClick();
          }
          break;
        case OperationMode.Calibration:
          CalibrationViewModel.Instance.CalibrationFailCheck();
          break;
        case OperationMode.Verification:
          if (VerificationViewModel.AnalyzeVerificationResults(out var errorMsg))
          {
            _ = Current.Dispatcher.BeginInvoke((Action)VerificationViewModel.VerificationSuccess);
          }
          else
          {
            Current.Dispatcher.Invoke(()=>Notification.ShowError(errorMsg, 26));
          }
          Verificator.PublishReport();
          Current.Dispatcher.Invoke(DashboardViewModel.Instance.ValModeToggle);
          //Notification.ShowLocalizedError(nameof(Language.Resources.Validation_Fail));
          break;
      }
    }

    public void NewStatsAvailableEventHandler(object sender, StatsEventArgs e)
    {
      _ = Current.Dispatcher.BeginInvoke((Action)(() =>
      {
        ResultsViewModel.Instance.DecodeCalibrationStats(e.Stats, current:true);
        ChannelOffsetViewModel.Instance.DecodeBackgroundStats(e.BgStats);
      }));
    }

    public void MapChangedEventHandler(object sender, CustomMap map)
    {
      Device.SetMap(map);
      _ = Current.Dispatcher.BeginInvoke((Action)(() =>
      {
        ResultsViewModel.Instance.FillWorldMaps();

        if (CalibrationViewModel.Instance != null)
          CalibrationViewModel.Instance.OnMapChanged(map);

        if (ChannelsViewModel.Instance != null)
          ChannelsViewModel.Instance.OnMapChanged(map);

        if (DashboardViewModel.Instance != null)
          DashboardViewModel.Instance.OnMapChanged(map);

        if (NormalizationViewModel.Instance != null)
          NormalizationViewModel.Instance.OnMapChanged(map);
      }));
    }

    public static void OutputLegacyReport()
    {
      if (!MakeLegacyPlateReport)
      {
        Console.WriteLine("Legacy Plate Report Inactive");
        return;
      }

      var bldr = new StringBuilder();
      bldr.AppendLine("Program,\"DIOS\"");
      bldr.AppendLine($"Build,\"{Program.BUILD}\",Firmware,\"{Device.FirmwareVersion}\"");
      bldr.AppendLine($"Date,\"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-GB"))}\"\n");

      bldr.AppendLine($"Instrument,\"{Environment.MachineName}\"");
      bldr.AppendLine($"Session,\"{Device.Publisher.Outfilename}\"\n\n\n\n\n\n\n");

      bldr.AppendLine($"Samples,\"{WellsSelectViewModel.Instance.CurrentTableSize}\"\n");
      var header = MapRegionsController.GetLegacyReportHeader();
      bldr.Append(Device.Results.PlateReport.LegacyReport(header));


      string rfilename = Device.Control == SystemControl.Manual ? Device.Publisher.Outfilename : Device.WorkOrder.plateID.ToString();
      var directoryName = $"{Device.Publisher.Outdir}\\AcquisitionData";
      try
      {
        if (!Directory.Exists(directoryName))
          _ = Directory.CreateDirectory(directoryName);
      }
      catch
      {
        Console.WriteLine($"Failed to create {directoryName}");
        return;
      }

      try
      {
        var fileName = $"{directoryName}" +
                       "\\LxResults_" + rfilename + "_" + Device.Publisher.Date + ".csv";
        using (TextWriter jwriter = new StreamWriter(fileName))
        {
          jwriter.Write(bldr.ToString());
          Console.WriteLine($"Legacy Plate Report saved as {fileName}");
        }
      }
      catch
      {
        Console.WriteLine("Failed to create Legacy Plate Report");
      }
    }

    public static void SetLogOutput()
    {
      string folder = $"{DiosApp.RootDirectory.FullName}\\SystemLogs";
      string fileName = "EventLog";
      if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);
      string logPath = $"{folder}\\{fileName}";
      string logFilePath = logPath + ".txt";
    
      string backFilePath = logPath + ".bak";
      if (File.Exists(logFilePath))
      {
        File.Delete(backFilePath);
        File.Move(logFilePath, backFilePath);
      }
      var fs = new FileStream(logFilePath, FileMode.Create);
      var logWriter = new StreamWriter(fs);
      logWriter.AutoFlush = true;
      Console.SetOut(logWriter);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      DevExpress.Xpf.Core.ApplicationThemeHelper.ApplicationThemeName = DevExpress.Xpf.Core.Theme.Office2019ColorfulName;
      base.OnStartup(e);
    }

    public static void SavePlateState()
    {
      //overwrite the whole thing
      try
      {
        string contents = PlatePictogramViewModel.Instance.PlatePictogram.GetSerializedPlate();
        File.WriteAllText($"{DiosApp.RootDirectory.FullName}\\Status\\StatusFile.json", contents);
      }
      catch(Exception e)
      {
        Notification.Show($"Problem with status file save, Please report this issue to the Manufacturer {e.Message}");
      }
    }

    public static void InitSTab(string tabname)
    {
      //Removing this can lead to unforseen crucial bugs in instrument operation. If so - do with extra care
      //one example is a check in CommandLists.Readertab for changed plate parameter,which could happen in manual well selection in motors tab
      Action actionList = null;
      switch (tabname)
      {
        case "readertab":
          actionList = () =>
          {
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.Volume, VolumeType.Wash);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.Volume, VolumeType.Sample);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.Volume, VolumeType.Agitate);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.PlateType); // plate type needed here?really? same question for the rest, actually
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.WellReadingSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.WellReadingOrder);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelConfiguration);
          };
          break;
        case "reportingtab":
          actionList = () =>
          {
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ValveCuvetDrain); //this all makes no sense. just check the commands. probably
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ValveFan2);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ValveFan1);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.Pressure);
          };
          break;
        case "calibtab":
          actionList = () =>
          {
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.Normal);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.HiSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.HiSensitivity);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.Flush);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.Pickup);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSheath, SyringeSpeed.MaxSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.Normal);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.HiSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.HiSensitivity);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.Flush);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.Pickup);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SyringeSpeedSample, SyringeSpeed.MaxSpeed);
          };
          break;
        case "channeltab":
          actionList = () =>
          {
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.GreenA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.GreenB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.GreenC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.RedD);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.VioletA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.VioletB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelBias30C, Channel.ForwardScatter);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.GreenA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.GreenB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.GreenC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.RedA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.RedB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.RedC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.RedD);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.VioletA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.VioletB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelCompensationBias, Channel.ForwardScatter);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.GreenA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.GreenB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.GreenC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.RedA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.RedB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.RedC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.RedD);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.VioletA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.VioletB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelTemperature, Channel.ForwardScatter);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.GreenA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.GreenB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.GreenC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedC);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.RedD);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.VioletA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.VioletB);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ChannelOffset, Channel.ForwardScatter);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.CalibrationMargin);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.SiPMTempCoeff);
          };
          break;
        case "motorstab":
          actionList = () =>
          {
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.Slope);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.StartSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.RunSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.CurrentStep);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorX, MotorParameterType.CurrentLimit);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.Slope);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.StartSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.RunSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.CurrentStep);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorY, MotorParameterType.CurrentLimit);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.Slope);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.StartSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.RunSpeed);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.CurrentStep);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorZ, MotorParameterType.CurrentLimit);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Tube);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate96Column1);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate96Column12);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate384Column1);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsX, MotorStepsX.Plate384Column24);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Tube);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate96RowA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate96RowH);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate384RowA);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsY, MotorStepsY.Plate384RowP);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.Tube);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.A1);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.A12);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.H1);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.MotorStepsZ, MotorStepsZ.H12);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.PollStepActivity);
          };
          break;
        case "componentstab":
          actionList = () =>
          {
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ValveCuvetDrain);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ValveFan2);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.ValveFan1);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.PollStepActivity);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.IsInputSelectorAtPickup);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.Pressure);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.IsLaserActive);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.LaserPower, LaserType.Violet);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.LaserPower, LaserType.Green);
            Device.Hardware.RequestHardwareParameter(DeviceParameterType.LaserPower, LaserType.Red);
          };
          break;
        default:
          return;
      }

      if (actionList != null)
        actionList.Invoke();
    }

    public void OnNewWorkOrder(object sender, FileSystemEventArgs e)
    {
      var name = Path.GetFileNameWithoutExtension(e.Name);
      Device.Publisher.WorkOrderPath = e.FullPath;
      if (!ParseWorkOrder())
        return;
      
      _workOrderPending = true;
      DashboardViewModel.Instance.WorkOrder[0] = name;  //check for already existing one 
      // if WO already selected -> allow start. else the WO checking action should perform the same check
      if (DashboardViewModel.Instance.SelectedSystemControlIndex == 1)  //no barcode required so allow start
      {
        MainButtonsViewModel.Instance.StartButtonEnabled = true;
        _workOrderPending = false;  //questionable logic. can check on non empty textbox
      }
    }

    public static void CheckAvailableWorkOrders()
    {
      string[] fileEntries = Directory.GetFiles($"{DiosApp.RootDirectory.FullName}\\WorkOrder", "*.txt");
      if (fileEntries.Length == 0)
        return;
      var name = Path.GetFileNameWithoutExtension(fileEntries[0]);
      Device.Publisher.WorkOrderPath = fileEntries[0];
      int i = 1;
      while (!ParseWorkOrder())
      {
        if (i < fileEntries.Length)
        {
          Device.Publisher.WorkOrderPath = fileEntries[i];
          name = Path.GetFileNameWithoutExtension(fileEntries[i]);
          i++;
        }
        else
          return;
      }

      DashboardViewModel.Instance.WorkOrder[0] = name;  //should be first succesfully parsed
      _workOrderPending = true;
      // if WO already selected -> allow start. else the WO checking action should perform the same check
      if (DashboardViewModel.Instance.SelectedSystemControlIndex == 1)  //no barcode required so allow start
      {
        MainButtonsViewModel.Instance.StartButtonEnabled = true;
        _workOrderPending = false;
      }
    }

    private static bool ParseWorkOrder()
    {
      try
      {
        using (TextReader reader = new StreamReader(Device.Publisher.WorkOrderPath))
        {
          var contents = reader.ReadToEnd();
          Device.WorkOrder = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkOrder>(contents);
        }
      }
      catch
      {
        return false;
      }
      return true;
    }

    private void InitApp(Device device)
    {
      StatisticsExtension.TailDiscardPercentage = Settings.Default.StatisticsTailDiscardPercentage;
      Device = device ?? new Device(new USBConnection(), DiosApp.RootDirectory.FullName);

      SetLogOutput();
      DiosApp.MapController.OnAppLoaded(Settings.Default.DefaultMap);
      Device.StartingToReadWell += StartingToReadWellEventHandler;
      Device.FinishedReadingWell += FinishedReadingWellEventHandler;
      Device.FinishedMeasurement += FinishedMeasurementEventHandler;
      Device.NewStatsAvailable += NewStatsAvailableEventHandler;
      DiosApp.MapController.ChangedActiveMap += MapChangedEventHandler;
      Device.ParameterUpdate += _textBoxHandler.ParameterUpdateEventHandler;
      _workOrderPending = false;
      _nextWellWarning = false;
      _workOrderWatcher = new FileSystemWatcher($"{DiosApp.RootDirectory.FullName}\\WorkOrder");
      _workOrderWatcher.NotifyFilter = NotifyFilters.FileName;
      _workOrderWatcher.Filter = "*.txt";
      _workOrderWatcher.EnableRaisingEvents = true;
      _workOrderWatcher.Created += OnNewWorkOrder;
    }

    public static void SetupDevice()
    {
      if (Device == null)
        throw new Exception("Device not initialized");

      Device.Init();

      if (Directory.Exists(Settings.Default.LastOutFolder))
        Device.Publisher.Outdir = Settings.Default.LastOutFolder;
      else
      {
        Settings.Default.LastOutFolder = Device.Publisher.Outdir;
        Settings.Default.Save();
      }
      Device.Publisher.Outfilename = Settings.Default.SaveFileName;
      Device.Publisher.MakePlateReport = Settings.Default.PlateReport;
      Device.Control = (SystemControl)Settings.Default.SystemControl;
      Device.SaveIndividualBeadEvents = Settings.Default.Everyevent;
      Device.RMeans = Settings.Default.RMeans;
      Device.TerminationType = (Termination)Settings.Default.EndRead;
      Device.MinPerRegion = Settings.Default.MinPerRegion;
      Device.BeadsToCapture = Settings.Default.BeadsToCapture;
      Device.OnlyClassifiedInBeadEventFile = Settings.Default.OnlyClassifed;
      Device.SensitivityChannel = Settings.Default.SensitivityChannelB ? HiSensitivityChannel.GreenB : HiSensitivityChannel.GreenC;
      Device.ReporterScaling = Settings.Default.ReporterScaling;
      Device.Hardware.SetHardwareParameter(DeviceParameterType.CalibrationParameter, CalibrationParameter.Attenuation, DiosApp.MapController.ActiveMap.calParams.att);
      Device.HdnrTrans = DiosApp.MapController.ActiveMap.calParams.DNRTrans;
      Device.Compensation = DiosApp.MapController.ActiveMap.calParams.compensation;
      Device.Hardware.RequestHardwareParameter(DeviceParameterType.SampleSyringeType);
      //Device.MainCommand("Set Property", code: 0x97, parameter: 1170);  //set current limit of aligner motors if leds are off //0x97 no longer used
      Device.MaxPressure = Settings.Default.MaxPressure;
    }
  }
}