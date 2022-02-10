﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ei_Dimension
{
  public class Program
  {
    internal static CompanyID SpecializedVer = CompanyID.China;
    public static SplashScreen SplashScreen { get; private set; }
    [STAThread]
    public static void Main(string[] args)
    {
      if (SpecializedVer == CompanyID.US)
        SplashScreen = new SplashScreen(@"/Icons/Splash.png");
      else if (SpecializedVer == CompanyID.China)
        SplashScreen = new SplashScreen(@"/Icons/SplashCh.png");
#if !DEBUG
      SplashScreen.Show(false, true);
#endif
      var app = new App();
      app.InitializeComponent();
      _ = app.Run();
    }
  }
}