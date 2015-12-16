using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nutritia.UI.Pages
{
    /// <summary>
    /// Interaction logic for Information.xaml
    /// Page d'information général du logiciel
    /// </summary>
    public partial class Information : Page
    {
        public Information()
        {
            InitializeComponent();
            //Change le contenu de lblVersion pour le numéro de version du logiciel.
            lblVersion.Content = " " + FileVersionInfo.GetVersionInfo(App.ResourceAssembly.Location).FileVersion;
        }
    }
}
