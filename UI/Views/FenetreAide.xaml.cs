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
    /// Interaction logic for FenetreAide.xaml
    /// </summary>
    public partial class FenetreAide : Window
    {
        public FenetreAide(String nomFenetre)
        {
            InitializeComponent();
			switch (nomFenetre)
			{
				case "AjoutPlatMembre":
					tcAide.SelectedItem = tiAjoutPlat;
					break;
				case "APropos":
					tcAide.SelectedItem = tiLangue;
					break;
				case "CreationProfil":
					tcAide.SelectedItem = tiProfil;
					break;
				case "Don":
					tcAide.SelectedItem = tiDon;
					break;
				case "FenetreCalculatriceNutritionelle":
					tcAide.SelectedItem = tiCalculatrice;
					break;
				case "FenetreConnexion":
					tcAide.SelectedItem = tiConnexion;
					break;
				case "FenetreParametre":
					tcAide.SelectedItem = tiParametre;
					break;
				case "GenerateurMenus":
					tcAide.SelectedItem = tiGenerationMenu;
					break;
				case "ListeEpicerie":
					tcAide.SelectedItem = tiListeEpicerie;
					break;
				case "MenuPrincipal":
					tcAide.SelectedItem = tiMenuPrincipal;
					break;
				case "MenuPrincipalConnecte":
					tcAide.SelectedItem = tiMenuConnecte;
					break;
				case "ModificationProfil":
					tcAide.SelectedItem = tiProfil;
					break;
				default:
					tcAide.SelectedItem = tiMenuPrincipal;
					break;
			}
		}
    }
}
