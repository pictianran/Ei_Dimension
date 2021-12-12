﻿using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;
using System.Collections.ObjectModel;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class VerificationViewModel
  {
    public static VerificationViewModel Instance { get; private set; }
    public bool isActivePage { get; set; }
    protected VerificationViewModel()
    {
      Instance = this;
      isActivePage = false;
    }

    public static VerificationViewModel Create()
    {
      return ViewModelSource.Create(() => new VerificationViewModel());
    }

    public void AddValidationRegion(byte num)
    {
      //App.MapRegions.AddValidationRegion(num);
    }

    public void LoadClick()
    {
      var idx = App.Device.MapList.FindIndex(x => x.mapName == App.Device.ActiveMap.mapName);
      var map = App.Device.MapList[idx];
      for (var i = 0; i < map.regions.Count; i++)
      {
        App.MapRegions.VerificationRegions[i] = true;
        App.MapRegions.AddValidationRegion(i);
        App.MapRegions.VerificationReporterList[i] = "";
        if (map.regions[i].isValidator)
        {
          App.MapRegions.AddValidationRegion(i);
          if(map.regions[i].VerificationTargetReporter > -0.1)
            App.MapRegions.VerificationReporterList[i] = map.regions[i].VerificationTargetReporter.ToString();
        }
      }
    }

    public void SaveClick()
    {
      var idx = App.Device.MapList.FindIndex(x => x.mapName == App.Device.ActiveMap.mapName);
      var map = App.Device.MapList[idx];
      for (var i = 0; i < App.MapRegions.RegionsList.Count; i++)
      {
        var index = map.regions.FindIndex(x => x.Number == int.Parse(App.MapRegions.RegionsList[i]));
        if(index != -1)
        {
          double temp = -1;
          if (App.MapRegions.VerificationRegions[i])
          {
            map.regions[index].isValidator = true;
            if (App.MapRegions.VerificationReporterList[i] != "" && double.TryParse(App.MapRegions.VerificationReporterList[i], out temp))
            {
              if(temp < 0)
                map.regions[index].VerificationTargetReporter = -1;
              else
                map.regions[index].VerificationTargetReporter = temp;
            }
            else
              map.regions[index].VerificationTargetReporter = temp;
          }
          else
          {
            map.regions[index].isValidator = false;
            map.regions[index].VerificationTargetReporter = -1;
          }
        }
      }
      App.Device.SaveCalVals(new MicroCy.MapCalParameters
      {
        TempCl0 = -1,
        TempCl1 = -1,
        TempCl2 = -1,
        TempCl3 = -1,
        TempRedSsc = -1,
        TempGreenSsc = -1,
        TempVioletSsc = -1,
        TempRpMaj = -1,
        TempRpMin = -1,
        TempFsc = -1,
        Compensation = -1,
        Gating = -1,
        Height = -1,
        MinSSC = -1,
        MaxSSC = -1,
        DNRCoef = -1,
        DNRTrans = -1,
        Attenuation = -1,
        CL0 = -1,
        CL1 = -1,
        CL2 = -1,
        CL3 = -1,
        RP1 = -1,
        Caldate = null,
        Valdate = null
      });
    }

    public static void VerificationSuccess()
    {
      App.Device.SaveCalVals(new MicroCy.MapCalParameters
      {
        TempCl0 = -1,
        TempCl1 = -1,
        TempCl2 = -1,
        TempCl3 = -1,
        TempRedSsc = -1,
        TempGreenSsc = -1,
        TempVioletSsc = -1,
        TempRpMaj = -1,
        TempRpMin = -1,
        TempFsc = -1,
        Compensation = -1,
        Gating = -1,
        Height = -1,
        MinSSC = -1,
        MaxSSC = -1,
        DNRCoef = -1,
        DNRTrans = -1,
        Attenuation = -1,
        CL0 = -1,
        CL1 = -1,
        CL2 = -1,
        CL3 = -1,
        RP1 = -1,
        Caldate = null,
        Valdate = DateTime.Now.ToString("dd.MM.yyyy", new System.Globalization.CultureInfo("en-GB"))
      });
      DashboardViewModel.Instance.ValidDateBox[0] = App.Device.ActiveMap.valtime;
      App.ShowLocalizedNotification(nameof(Language.Resources.Validation_Success), System.Windows.Media.Brushes.Green);
    }

    public bool ValMapInfoReady()
    {
      if (App.MapRegions.VerificationRegionNums.Count == 0)
      {
        App.ShowNotification("No Verification regions selected");
        return false;
      }

      for (var i = 0; i < App.MapRegions.RegionsList.Count; i++)
      {
        if (App.MapRegions.VerificationRegions[i])
        {
          if (App.MapRegions.VerificationReporterList[i] == "")
          {
            App.ShowNotification($"Verification region {App.MapRegions.RegionsList[i]} Reporter is not specified");
            return false;
          }
        }
      }
      return true;
    }

    public static bool AnalyzeVerificationResults()
    {
      bool passed = true;
      if (App.Device.Verificator.TotalClassifiedBeads < App.Device.TotalBeads * 0.8)
      {
        Console.WriteLine("Verification Fail: Less than 80% of beads hit the regions");
        passed = false;
      }

      double reporterErrorMargin = 0.2;
      for (var i = 0; i < App.MapRegions.RegionsList.Count; i++)
      {
        if (App.MapRegions.VerificationRegions[i])
        {
          int regionNum = int.Parse(App.MapRegions.RegionsList[i]);
          double inputReporter = double.Parse(App.MapRegions.VerificationReporterList[i]);
          int validatorIndex = App.Device.Verificator.RegionalStats.FindIndex(x => x.Region == regionNum);

          if (App.Device.Verificator.RegionalStats[validatorIndex].Stats[0].mfi <= inputReporter * (1 - reporterErrorMargin) &&
            App.Device.Verificator.RegionalStats[validatorIndex].Stats[0].mfi >= inputReporter * (1 + reporterErrorMargin))
          {
            Console.WriteLine($"Verification Fail: Reporter value ({App.Device.Verificator.RegionalStats[validatorIndex].Stats[0].mfi}) deviation is more than 20% from the target ({App.MapRegions.VerificationReporterList[i]})");
            passed = false;
          }
        }
      }
      return passed;
    }

    public class DropDownButtonContents
    {
      public string Content { get; set; }
      public string Locale { get; }
      private static VerificationViewModel _vm;
      public byte Index { get; set; }
      private static byte _nextIndex = 0;
      public DropDownButtonContents(string content, string locale, VerificationViewModel vm = null)
      {
        if (_vm == null)
        {
          _vm = vm;
        }
        Content = content;
        Locale = locale;
        Index = _nextIndex++;
      }

      //public void Click()
      //{
      //  _vm.SelectedLanguage = Content;
      //  App.SetLanguage(Locale);
      //  Settings.Default.Language = Index;
      //  Settings.Default.Save();
      //}
    }
  }
}