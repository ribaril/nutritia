using Nutritia.UI.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for FenetreParametres.xaml
    /// </summary>
    public partial class FenetreParametres : Window
    {
      

        public FenetreParametres()
        {
            InitializeComponent();
            btnConnexion.IsEnabled = App.MembreCourant.EstAdministrateur;
            FrmNavigation.Navigate(new MenuGeneral());
        }

        private void btnGeneral_Click(object sender, RoutedEventArgs e)
        {
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/MenuGeneral.xaml", UriKind.Relative));
        }

        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/MenuConnexion.xaml", UriKind.Relative));
        }

        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            btnConnexion.Background = Brushes.Transparent;
            btnGeneral.Background = Brushes.Transparent;
            if (sender is Button)
            {
                Button btn = sender as Button;
                btn.Background = Brushes.Gray;
            }
        }
    }
}
