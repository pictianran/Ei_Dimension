﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class FileSaveViewModel
  {
    public virtual ObservableCollection<string> BaseFileName { get; set; }
    public virtual ObservableCollection<bool> Checkboxes { get; set; }
    private IFolderBrowserDialogService FolderBrowserDialogService => this.GetService<IFolderBrowserDialogService>();

    protected FileSaveViewModel()
    {
      BaseFileName = new ObservableCollection<string> { Settings.Default.SaveFileName };
      Checkboxes = new ObservableCollection<bool>
      {
        Settings.Default.Everyevent,
        Settings.Default.RMeans,
        Settings.Default.PlateReport,
        false,
        false,
        false
      };
      Program.SplashScreen.Close(TimeSpan.FromMilliseconds(300));
    }
    
    public static FileSaveViewModel Create()
    {
      return ViewModelSource.Create(() => new FileSaveViewModel());
    }

    public void CheckedBox(int num)
    {
      switch (num)
      {
        case 0:
          App.Device.Everyevent = Checkboxes[num];
          Settings.Default.Everyevent = Checkboxes[num];
          break;
        case 1:
          App.Device.RMeans = Checkboxes[num];
          Settings.Default.RMeans = Checkboxes[num];
          break;
        case 2:
          App.Device.PltRept = Checkboxes[num];
          Settings.Default.PlateReport = Checkboxes[num];
          break;
        case 3:
          App.Device.OnlyClassified = !Checkboxes[num];
          break;
        case 4:
          break;
        case 5:
          break;
      }
      Settings.Default.Save();
    }

    public void SelectOutFolder()
    {
       FolderBrowserDialogService.StartPath = App.Device.Outdir;
      if (FolderBrowserDialogService.ShowDialog())
        App.Device.Outdir = FolderBrowserDialogService.ResultPath;
    }

    public void FocusedBox(int num)
    {
      switch (num)
      {
        case 0:
          App.SelectedTextBox = (this.GetType().GetProperty(nameof(BaseFileName)), this, 0);
          break;
      }
    }

    public void TextChanged(TextChangedEventArgs e)
    {
      App.InjectToFocusedTextbox(((TextBox)e.Source).Text, true);
    }
  }
}