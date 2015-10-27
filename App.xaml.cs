using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.Threading;

namespace Nutritia
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public Membre MembreCourant = new Membre();
        static public CultureInfo culture = new CultureInfo(String.Empty);
        protected override void OnStartup(StartupEventArgs e)
        {
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("fr");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
        }
    }
}
