﻿using DevExpress.Xpf.Charts;
using System;
using System.Collections.Generic;
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
  /// Interaction logic for ResultsView.xaml
  /// </summary>
  public partial class ResultsView : UserControl
  {
    public static ResultsView Instance;
    public ResultsView()
    {
      InitializeComponent();
      Instance = this;
      ChC.AxisX.WholeRange.AutoSideMargins = false;
      ChC.AxisX.WholeRange.SetMinMaxValues(10, 35000);
      ChC.AxisY.WholeRange.AutoSideMargins = false;
      ChC.AxisY.WholeRange.SetMinMaxValues(10, 35000);
    }
    public void AddXYPoint(double x, double y, SolidColorBrush brush)
    {
      var Point = new SeriesPoint(x, y);
      Point.Brush = brush;
      HeatMap.Points.Add(Point);
    }
    public void ChangePointColor(int index, SolidColorBrush brush)
    {
      HeatMap.Points[index].Brush = brush;
    }
    public void ClearPoints()
    {
      HeatMap.Points.Clear();
    }

    public void PrintXY(int Resoultion_dpi = 800)
    {
      var options = new DevExpress.XtraPrinting.ImageExportOptions();
      options.TextRenderingMode = DevExpress.XtraPrinting.TextRenderingMode.SingleBitPerPixelGridFit;
      options.Resolution = Resoultion_dpi; options.Format = new System.Drawing.Imaging.ImageFormat(System.Drawing.Imaging.ImageFormat.Png.Guid);
      string date = DateTime.Now.ToString("dd.MM.yyyy.hhtt-mm-ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
      XYPlot.ExportToImage(App.Device.RootDirectory + @"\SavedImages\" + date + ".png", options);
    }
    public void PrintScatter(int Resoultion_dpi = 800)
    {
      var options = new DevExpress.XtraPrinting.ImageExportOptions();
      options.TextRenderingMode = DevExpress.XtraPrinting.TextRenderingMode.SingleBitPerPixelGridFit;
      options.Resolution = Resoultion_dpi; options.Format = new System.Drawing.Imaging.ImageFormat(System.Drawing.Imaging.ImageFormat.Png.Guid);
      string date = DateTime.Now.ToString("dd.MM.yyyy.hhtt-mm-ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
      ScatterPlot.ExportToImage(App.Device.RootDirectory + @"\SavedImages\" + date + ".png", options);
    }
  }
}
