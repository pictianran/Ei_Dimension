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
  /// Interaction logic for ServiceView.xaml
  /// </summary>
  public partial class ServiceView : UserControl
  {
    public ServiceView()
    {
      InitializeComponent();
#if DEBUG
      Console.Error.WriteLine("#8 ServiceView Loaded");
#endif
    }
  }
}
