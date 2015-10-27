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
    /// Interaction logic for GestionRepertoire.xaml
    /// </summary>
    public partial class GestionRepertoire : UserControl
    {
        public bool Ajout { get; set; }
        public bool Modification { get; set; }
        public bool Aliment { get; set; }
        public bool Plat { get; set; }

        public GestionRepertoire()
        {
            InitializeComponent();

            ModifierContexte();
        }

        private void rdb_ajout_Checked(object sender, RoutedEventArgs e)
        {
            Ajout = true;
            Modification = false;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void rdb_modif_Checked(object sender, RoutedEventArgs e)
        {
            Ajout = false;
            Modification = true;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void rdb_aliment_Checked(object sender, RoutedEventArgs e)
        {
            Aliment = true;
            Plat = false;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void rdb_plat_Checked(object sender, RoutedEventArgs e)
        {
            Aliment = false;
            Plat = true;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void ModifierContexte()
        {
            if (Ajout)
            {
                if (Aliment)
                {
                    grid_ajout_aliment.Visibility = Visibility.Visible;
                    grid_modif_aliment.Visibility = Visibility.Hidden;
                    grid_ajout_plat.Visibility = Visibility.Hidden;
                    grid_modification_plat.Visibility = Visibility.Hidden;
                }
                else
                {
                    grid_modification_plat.Visibility = Visibility.Hidden;
                    grid_ajout_aliment.Visibility = Visibility.Hidden;
                    grid_modif_aliment.Visibility = Visibility.Hidden;
                    grid_ajout_plat.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (Aliment)
                {
                    grid_ajout_aliment.Visibility = Visibility.Hidden;
                    grid_modif_aliment.Visibility = Visibility.Visible;
                    grid_ajout_plat.Visibility = Visibility.Hidden;
                    grid_modification_plat.Visibility = Visibility.Hidden;
                }
                else
                {
                    grid_ajout_plat.Visibility = Visibility.Hidden;
                    grid_ajout_aliment.Visibility = Visibility.Hidden;
                    grid_modif_aliment.Visibility = Visibility.Hidden;
                    grid_modification_plat.Visibility = Visibility.Visible;
                }
            }
        }
    }
}