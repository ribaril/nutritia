using System;
using System.Collections.Generic;
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

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for AjoutPlatAdmin.xaml
    /// </summary>
    public partial class AjoutPlatAdmin : UserControl
    {
        public AjoutPlatAdmin()
        {
            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = "Nutritia - Ajout de plats";
        }
    }
}
