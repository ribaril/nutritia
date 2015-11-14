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
                case "FenetreGenerateurMenus":
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

		/// <summary>
		/// Code de Guillaume (légerement modifié pour qu'il soit compatible avec un apel de tous les SV):
		/// Événement lancé lorsque la roulette de la souris est utilisée dans le "scrollviewer" contenant le menu.
		/// Explicitement, cet événement permet de gérer le "scroll" avec la roulette correctement sur toute la surface du "scrollviewer".
		/// Si on ne le gère pas, il est seulement possible de "scroller" lorsque le pointeur de la souris est situé sur la "scrollbar".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScrollFocus(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scrollViewer = (ScrollViewer)sender;
			scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
		}
	}
}
