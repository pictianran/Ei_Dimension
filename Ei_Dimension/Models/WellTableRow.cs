﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ei_Dimension.Models
{
  public class WellTableRow
  {
    public int Index { get; }
    public string Header { get; }
    public ObservableCollection<string> Images { get; set; }
    public List<WellType> Types { get; set; }

    private int _size;
    public WellTableRow(int rowIndex, int size)
    {
      Index = rowIndex;
      Header = Convert.ToChar('A' + Index).ToString();
      Images = new ObservableCollection<string>();
      Types = new List<WellType>(size);
      _size = size;
      for (var i = 0; i < size; i++)
      {
        Images.Add(Image(WellType.Empty));
        Types.Add(WellType.Empty);
      }
    }

    public void SetType(int index, WellType type)
    {
      Types[index] = type;
      Images[index] = Image(type);
    }

    public void Reset()
    {
      for (var i = 0; i < _size; i++)
      {
        SetType(i, WellType.Empty);
      }
    }
    
    private static string Image(WellType type)
    {
      string ret = "";
      switch (type)
      {
        case WellType.Empty:
          ret = @"/Icons/Empty_Well.png";
          break;
        case WellType.Standard:
          ret = @"/Icons/Filled_Well.png";
          break;
        case WellType.Control:
          ret = @"pack://application:,,,/DevExpress.Images.v21.1;component/Images/Media/Media_32x32.png";
          break;
        case WellType.Unknown:
          ret = @"pack://application:,,,/DevExpress.Images.v21.1;component/SvgImages/Icon Builder/Actions_Question.svg";
          break;
        case WellType.ReadyForReading:
          ret = @"pack://application:,,,/DevExpress.Images.v21.1;component/SvgImages/DiagramIcons/BindingEditorHelpIcon.svg";
          break;
        case WellType.NowReading:
          ret = @"pack://application:,,,/DevExpress.Images.v21.1;component/SvgImages/Outlook Inspired/GettingStarted.svg";
          break;
        case WellType.Success:
          ret = @"pack://application:,,,/DevExpress.Images.v21.1;component/SvgImages/Icon Builder/Actions_CheckCircled.svg";
          break;
        case WellType.LightFail:
          ret = @"pack://application:,,,/DevExpress.Images.v21.1;component/SvgImages/Icon Builder/Security_WarningCircled2.svg";
          break;
        case WellType.Fail:
          ret = @"pack://application:,,,/DevExpress.Images.v21.1;component/SvgImages/XAF/State_Validation_Invalid.svg";
          break;
      }
      return ret;
    }
  }
}
