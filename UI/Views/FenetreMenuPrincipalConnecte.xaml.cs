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
using Nutritia.UI.Views;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour MenuPrincipalConnecte.xaml
    /// </summary>
    public partial class MenuPrincipalConnecte : UserControl
    {
        public MenuPrincipalConnecte()
        {
            InitializeComponent();

            if(App.MembreCourant.Nom != "")
            {
                lbl_nom_membre.Content = "Bienvenue " + App.MembreCourant.NomUtilisateur;
            }
        }

        private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            App.MembreCourant = new Membre();
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
			ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new GenerateurMenus());
		}

		private void btnAjoutPlat_Click(object sender, RoutedEventArgs e)
		{
			ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new AjoutPlat());
		}

		private void btnVoterPlat_Click(object sender, RoutedEventArgs e)
		{
			
		}
	}
}