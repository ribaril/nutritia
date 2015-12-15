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
    /// Fenêtre de paramètres du logiciel
    /// </summary>
    public partial class FenetreParametres : Window
    {


        public FenetreParametres()
        {
            InitializeComponent();
            //Désactive le bouton menant à l'option Connexion si l'utilisateur n'est pas connecté ni administrateur.
            btnConnexion.IsEnabled = App.MembreCourant.EstAdministrateur;
            //Page de départ pour le Frame
            FrmNavigation.Navigate(new MenuGeneral());
        }

        private void btnGeneral_Click(object sender, RoutedEventArgs e)
        {
            //Navigue sur la Page MenuGeneral lorsque le bouton est appuyé.
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/MenuGeneral.xaml", UriKind.Relative));
        }

        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            //Navigue sur la Page MenuConnexion lorsque le bouton est appuyé.
            FrmNavigation.NavigationService.Navigate(new Uri("UI/Pages/MenuConnexion.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Event Handler sur tout les boutons du stackpanel qui contient tout les boutons de menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            //Retourne la couleur de background des boutons par défault.
            btnConnexion.Background = Brushes.Transparent;
            btnGeneral.Background = Brushes.Transparent;
            if (sender is Button)
            {
                //Change la couleur de background du bouton appuyé en gris.
                Button btn = sender as Button;
                btn.Background = Brushes.Gray;
            }
        }
    }
}
