﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ei_Dimension.Views
{
  /// <summary>
  /// Interaction logic for StaticMapView.xaml
  /// </summary>
  public partial class StaticMapView : UserControl
  {
    public StaticMapView()
    {
      InitializeComponent();
      byte[,] mep = App.Device.GetStaticMap();
      for(var i = 0; i < 256; i++)
      {
        for (var j = 0; j < 256; j++)
        {
          var rect1 = new Rectangle();
          rect1.Width = 2;
          rect1.Height = 2;  
          rect1.Margin= new Thickness(j * 2, (255 * 2) - (i * 2), 0, 0);
          (byte r, byte g, byte b) color = (0,0,0);
          if(mep[i, j] != 0)
          {
            color = (255,255,255);
          }
          rect1.Fill = new SolidColorBrush(Color.FromRgb(color.r,color.g,color.b));
          Canv.Children.Add(rect1);
        }
      }
    }
  }
}
