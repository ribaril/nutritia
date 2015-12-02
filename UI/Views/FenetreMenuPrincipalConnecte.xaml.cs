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
using Infralution.Localization.Wpf;
using Nutritia.UI.Views;
using System.Globalization;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour MenuPrincipalConnecte.xaml
    /// </summary>
    public partial class MenuPrincipalConnecte : UserControl
    {
        public MenuPrincipalConnecte()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreMenuPrincipalConnecte.Titre;

            if(App.MembreCourant.EstAdministrateur)
            {
                lbl_nom_membre.Visibility = Visibility.Hidden;
                btnGestAdmin.Visibility = Visibility.Visible;
            }
            else
            {
                btnGestAdmin.Visibility = Visibility.Hidden;
                lbl_nom_membre.Visibility = Visibility.Visible;
                lbl_nom_membre.Content = Nutritia.UI.Ressources.Localisation.FenetreMenuPrincipalConnecte.Bienvenue + " " + App.MembreCourant.NomUtilisateur;
            }
        }

        private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            App.MembreCourant = new Membre();
            CultureManager.UICulture = new CultureInfo(App.LangueInstance.IETF);
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipal());
       
        }

        private void btnProfil_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new ModificationProfil());
        }

		private void btnCalculatrice_Click(object sender, RoutedEventArgs e)
		{
			ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreCalculatriceNutritionelle());
		}

		private void btnMenuListe_Click(object sender, RoutedEventArgs e)
		{
			ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreGenerateurMenus());
		}

		private void btnAjoutPlat_Click(object sender, RoutedEventArgs e)
		{
			ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new AjoutPlat());
		}

		private void btnVoterPlat_Click(object sender, RoutedEventArgs e)
		{
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreVotes());
		}

        private void btnGestAdmin_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuAdministrateur());
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreMenuPrincipalConnecte.Titre;
        }
	}
}