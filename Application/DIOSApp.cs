﻿using System;
using System.Collections.Generic;
using System.IO;
using DIOS.Application.FileIO;
using DIOS.Core;

namespace DIOS.Application
{ 
  public class DIOSApp
  {
    public MapController MapController { get; }
    public DirectoryInfo RootDirectory { get; private set; } = new DirectoryInfo(Path.Combine(@"C:\Emissioninc", Environment.MachineName));
    public bool RunPlateContinuously { get; set; }
    public readonly string BUILD = "1.5.1.1";
    public ILogger Logger { get; }

    public DIOSApp()
    {
      SetSystemDirectories();
      Logger = new Logger(RootDirectory.FullName);
      MapController = new MapController($"{RootDirectory.FullName}\\Config", Logger);
    }

    private void SetSystemDirectories()
    {
      List<string> subDirectories = new List<string>(8) { "Config", "WorkOrder", "SavedImages", "Archive", "Status" };
      try
      {
        foreach (var d in subDirectories)
        {
          RootDirectory.CreateSubdirectory(d);
        }
        Directory.CreateDirectory(RootDirectory.FullName + @"\Result" + @"\Detail");
      }
      catch
      {
        Console.WriteLine("Directory Creation Failed");
      }
    }
  }
}