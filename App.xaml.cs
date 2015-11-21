using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nutritia
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public Membre MembreCourant = new Membre();
        static public CultureInfo culture = new CultureInfo(String.Empty);
        static public int APP_WIDTH = 650;
        static public int APP_HEIGHT = 550;
    }

}
