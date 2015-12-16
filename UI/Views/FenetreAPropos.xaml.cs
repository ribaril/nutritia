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
    /// Fenêtre contenant des informations variés sur le logiciel
    /// </summary>
    public partial class FenetreAPropos : Window
    {
        public FenetreAPropos()
        {
            InitializeComponent();
            //Configure Information comme page par défault du Frame
            FrmNavigation.Navigate(new Information());
        }

        private void btnInformation_Click(object sender, RoutedEventArgs e)
        {
            //Change le contenu du Frame pour la page Information
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/Information.xaml", UriKind.Relative));
        }

        private void btnDon_Click(object sender, RoutedEventArgs e)
        {
            //Change le contenu du Frame pour la page EnvoiDon
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/EnvoiDon.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Event Handler sur tout les boutons pour changer la couleur selon son état de sélection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            //Change la couleur de background de tout les boutons à transparent.
            btnDon.Background = Brushes.Transparent;
            btnInformation.Background = Brushes.Transparent;
            if (sender is Button)
            {
                //Change la couleur de background du bouton sélectionné en gris.
                Button btn = sender as Button;
                btn.Background = Brushes.Gray;
            }
        }
    }
}
