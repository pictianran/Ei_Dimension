﻿using Ei_Dimension.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ei_Dimension.Core
{
  public static class DataProcessor
  //helper class for ResultsViewModel
  {
    static public async Task<List<string>> GetDataFromFileAsync(string path)
    {
      var str = new List<string>();
      using (System.IO.FileStream fin = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
      using (System.IO.StreamReader sr = new System.IO.StreamReader(fin))
      {
        sr.ReadLine();
        while (!sr.EndOfStream)
        {
          str.Add(await sr.ReadLineAsync());
        }
      }
      return str;
    }

    static public MicroCy.BeadInfoStruct ParseRow(string data)
    {
      MicroCy.BeadInfoStruct Binfo;
      string[] words = data.Split(',');
      Binfo.Header = uint.Parse(words[0]);
      Binfo.EventTime = uint.Parse(words[1]);
      Binfo.fsc_bg = byte.Parse(words[2]);
      Binfo.vssc_bg = byte.Parse(words[3]);
      Binfo.cl0_bg = byte.Parse(words[4]);
      Binfo.cl1_bg = byte.Parse(words[5]);
      Binfo.cl2_bg = byte.Parse(words[6]);
      Binfo.cl3_bg = byte.Parse(words[7]);
      Binfo.rssc_bg = byte.Parse(words[8]);
      Binfo.gssc_bg = byte.Parse(words[9]);
      Binfo.greenB_bg = ushort.Parse(words[10]);
      Binfo.greenC_bg = ushort.Parse(words[11]);
      Binfo.greenB = ushort.Parse(words[12]);
      Binfo.greenC = ushort.Parse(words[13]);
      Binfo.l_offset_rg = byte.Parse(words[14]);
      Binfo.l_offset_gv = byte.Parse(words[15]);
      Binfo.region = ushort.Parse(words[16]);
      Binfo.fsc = float.Parse(words[17], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.violetssc = float.Parse(words[18], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.cl0 = float.Parse(words[19], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.redssc = float.Parse(words[20], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.cl1 = float.Parse(words[21], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.cl2 = float.Parse(words[22], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.cl3 = float.Parse(words[23], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.greenssc = float.Parse(words[24], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      Binfo.reporter = float.Parse(words[25], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
      return Binfo;
    }

    static public List<HistogramData> LinearizeDictionary(SortedDictionary<int,int> dict)
    {
      var result = new List<HistogramData>();
      foreach (var x in dict)
      {
        result.Add(new HistogramData(x.Value, x.Key));
      }
      return result;
    }

    public static int[] GenerateLogSpace(int min, int max, int logBins, bool baseE = false)
    {
      double logarithmicBase = 10;
      double logMin = Math.Log10(min);
      double logMax = Math.Log10(max);
      if (baseE)
      {
        logarithmicBase = Math.E;
        logMin = Math.Log(min);
        logMax = Math.Log(max);
      }
      double delta = (logMax - logMin) / logBins;
      double accDelta = delta;
      int[] Result = new int[logBins];
      for (int i = 1; i <= logBins; ++i)
      {
        Result[i - 1] = (int)Math.Round(Math.Pow(logarithmicBase, logMin + accDelta));
        accDelta += delta;
      }
      return Result;
    }
    public static double[] GenerateLogSpaceD(int min, int max, int logBins, bool baseE = false)
    {
      double logarithmicBase = 10;
      double logMin = Math.Log10(min);
      double logMax = Math.Log10(max);
      if (baseE)
      {
        logarithmicBase = Math.E;
        logMin = Math.Log(min);
        logMax = Math.Log(max);
      }
      double delta = (logMax - logMin) / logBins;
      double accDelta = delta;
      double[] Result = new double[logBins];
      for (int i = 1; i <= logBins; ++i)
      {
        Result[i - 1] = Math.Pow(logarithmicBase, logMin + accDelta);
        accDelta += delta;
      }
      return Result;
    }

    public static void BinData(List<MicroCy.BeadInfoStruct> list, bool fromFile = false)
    {
      var ResVM = ViewModels.ResultsViewModel.Instance;
      var MaxValue = ResVM.CurrentReporter[ResVM.CurrentReporter.Count - 1].Argument;
      var reporter = new int[ResVM.CurrentReporter.Count];
      var fsc = new int[ResVM.CurrentReporter.Count];
      var red = new int[ResVM.CurrentReporter.Count];
      var green = new int[ResVM.CurrentReporter.Count];
      var violet = new int[ResVM.CurrentReporter.Count];
      foreach (var beadD in list)
      {
        var bead = beadD;
        //overflow protection
        bead.fsc = bead.fsc < MaxValue ? bead.fsc : MaxValue;
        bead.violetssc = bead.violetssc < MaxValue ? bead.violetssc : MaxValue;
        bead.redssc = bead.redssc < MaxValue ? bead.redssc : MaxValue;
        bead.greenssc = bead.greenssc < MaxValue ? bead.greenssc : MaxValue;
        bead.reporter = bead.reporter < MaxValue ? bead.reporter : MaxValue;
        bool fscDone = false;
        bool violetDone = false;
        bool redDone = false;
        bool greenDone = false;
        bool reporterDone = false;

        for (var i = 0; i < reporter.Length; i++)
        {
          var currentValue = HistogramData.Bins[i];
          if (!fscDone && bead.fsc <= currentValue)
          {
            fsc[i]++;
            fscDone = true;
          }
          if (!violetDone && bead.violetssc <= currentValue)
          {
            violet[i]++;
            violetDone = true;
          }
          if (!redDone && bead.redssc <= currentValue)
          {
            red[i]++;
            redDone = true;
          }
          if (!greenDone && bead.greenssc <= currentValue)
          {
            green[i]++;
            greenDone = true;
          }
          if (!reporterDone && bead.reporter <= currentValue)
          {
            reporter[i]++;
            reporterDone = true;
          }
          if (fscDone && violetDone && redDone && greenDone && reporterDone)
            break;
        }
      }
      _ = App.Current.Dispatcher.BeginInvoke((Action)(() =>
      {
        if (fromFile)
        {
          for (var i = 0; i < ResVM.BackingReporter.Count; i++)
          {
            ResVM.BackingReporter[i].Value += reporter[i];
            ResVM.BackingForwardSsc[i].Value += fsc[i];
            ResVM.BackingRedSsc[i].Value += red[i];
            ResVM.BackingGreenSsc[i].Value += green[i];
            ResVM.BackingVioletSsc[i].Value += violet[i];
          }
        }
        else
        {
          for (var i = 0; i < ResVM.CurrentReporter.Count; i++)
          {
            ResVM.CurrentReporter[i].Value += reporter[i];
            ResVM.CurrentForwardSsc[i].Value += fsc[i];
            ResVM.CurrentRedSsc[i].Value += red[i];
            ResVM.CurrentGreenSsc[i].Value += green[i];
            ResVM.CurrentVioletSsc[i].Value += violet[i];
          }
        }
      }));

    }
  }
}
