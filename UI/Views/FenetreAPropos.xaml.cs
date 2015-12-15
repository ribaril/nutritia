using Nutritia.UI.Pages;
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
using System.Windows.Shapes;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for FenetreAPropos.xaml
    /// </summary>
    public partial class FenetreAPropos : Window
    {
        public FenetreAPropos()
        {
            InitializeComponent();
            FrmNavigation.Navigate(new Information());
        }

        private void btnInformation_Click(object sender, RoutedEventArgs e)
        {
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/Information.xaml", UriKind.Relative));
        }

        private void btnDon_Click(object sender, RoutedEventArgs e)
        {
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/EnvoiDon.xaml", UriKind.Relative));
        }

        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            btnDon.Background = Brushes.Transparent;
            btnInformation.Background = Brushes.Transparent;
            if (sender is Button)
            {
                Button btn = sender as Button;
                btn.Background = Brushes.Gray;
            }
        }
    }
}
