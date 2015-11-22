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
    /// Logique d'interaction pour FenetreSauvegarderMenu.xaml
    /// </summary>
    public partial class FenetreSauvegarderMenu : Window
    {
        private bool Erreur { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreSauvegarderMenu()
        {
            InitializeComponent();
            Erreur = false;
        }

        /// <summary>
        /// Méthode permettant de valider le nom du menu.
        /// </summary>
        private void ValiderNom()
        {
            Erreur = false;
            string nom = txtNom.Text;

            if(!string.IsNullOrEmpty(nom))
            {
                if(nom.Length < 2 || nom.Length > 10)
                {
                    Erreur = true;
                    lblNom.Content = Nutritia.UI.Ressources.Localisation.FenetreSauvegarderMenu.ErreurLongueurNom;
                    lblNom.Foreground = Brushes.Red;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ServiceFactory.Instance.GetService<IMenuService>().Retrieve(new RetrieveMenuArgs { IdMembre = App.MembreCourant.IdMembre, Nom = nom }).Nom))
                    {
                        Erreur = true;
                        lblNom.Content = Nutritia.UI.Ressources.Localisation.FenetreSauvegarderMenu.ErreurDuplication;
                        lblNom.Foreground = Brushes.Red;
                    }
                }
            }
            else
            {
                Erreur = true;
                lblNom.Content = Nutritia.UI.Ressources.Localisation.FenetreSauvegarderMenu.ErreurVide;
                lblNom.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Sauvegarder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            ValiderNom();

            if(!Erreur)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
