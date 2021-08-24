﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;
using System.Collections.ObjectModel;

namespace Ei_Dimension.ViewModels
{
  [POCOViewModel]
  public class MaintenanceViewModel
  {
    public virtual bool LEDsToggleButtonState { get; set; }
    public virtual object LEDSliderValue { get; set; }
    public virtual string SanitizeSecondsContent { get; set; }

    public virtual ObservableCollection<DropDownButtonContents> LanguageItems { get; set; }
    public virtual string SelectedLanguage { get; set; }

    private INavigationService NavigationService { get { return this.GetService<INavigationService>(); } }

    protected MaintenanceViewModel()
    {
      LEDSliderValue = 0;
      LEDsToggleButtonState = false;
      LanguageItems = new ObservableCollection<DropDownButtonContents>();
      foreach(var lang in Language.Supported.Languages)
      {
        LanguageItems.Add(new DropDownButtonContents(lang.Item1, lang.Item2, this));
      }
      SelectedLanguage = LanguageItems[0].Content;
    }

    public static MaintenanceViewModel Create()
    {
      return ViewModelSource.Create(() => new MaintenanceViewModel());
    }

    public void LEDsButtonClick()
    {
      LEDsToggleButtonState = !LEDsToggleButtonState;
    }

    public void LEDSliderValueChanged()
    {

    }

    public void UVCSanitizeClick()
    {

    }

    public void NavigateCalibration()
    {
      NavigationService.Navigate("CalibrationView", null, this);
    }

    public void NavigateChannels()
    {
      NavigationService.Navigate("ChannelsView", null, this);
    }

    public class DropDownButtonContents
    {
      public string Content { get; set; }
      private string _locale;
      private static MaintenanceViewModel _vm;
      public DropDownButtonContents(string content, string locale, MaintenanceViewModel vm = null)
      {
        if (_vm == null)
        {
          _vm = vm;
        }
        Content = content;
        _locale = locale;
      }

      public void Click()
      {
        _vm.SelectedLanguage = Content;
        App.SetLanguage(_locale);
      }
    }
  }
}