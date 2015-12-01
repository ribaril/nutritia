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
    /// Logique d'interaction pour FenetreSuiviRestrictions.xaml
    /// </summary>
    public partial class FenetreSuiviRestrictions : Window
    {
        public List<Plat> ListePlatsNonAdmissibles { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        /// <param name="listePlatsNonAdmissibles">La liste des plats non admissibles à la génération.</param>
        public FenetreSuiviRestrictions(List<Plat> listePlatsNonAdmissibles)
        {
            InitializeComponent();

            if(listePlatsNonAdmissibles.Count == 0)
            {
                lblEntete.Visibility = Visibility.Hidden;
                lblAucunPlat.Visibility = Visibility.Visible;
                btnEnregistrer.IsEnabled = false;
            }

            ListePlatsNonAdmissibles = listePlatsNonAdmissibles;
            dgPlatsNonAdmissibles.ItemsSource = ListePlatsNonAdmissibles;
        }

        /// <summary>
        /// Événement lancé lorsque la roulette de la souris est utilisée dans le "scrollviewer" contenant les plats non admissibles.
        /// Explicitement, cet événement permet de gérer le "scroll" avec la roulette correctement sur toute la surface du "scrollviewer".
        /// Si on ne le gère pas, il est seulement possible de "scroller" lorsque le pointeur de la souris est situé sur la "scrollbar".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void svPlats_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            svPlats.ScrollToVerticalOffset(svPlats.VerticalOffset - e.Delta);
        }

        /// <summary>
        /// Événement lancé lorsqu'un checkbox est coché.
        /// Permet d'ajouter le plat lié à ce checkbox de la liste de plats admissibles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(dgPlatsNonAdmissibles.SelectedItem != null)
            {
                ListePlatsNonAdmissibles.Find(plat => plat.Nom == ((Plat)dgPlatsNonAdmissibles.SelectedItem).Nom).EstTricherie = true;
            }
        }

        /// <summary>
        /// Événement lancé losrqu'un checkbox est décoché.
        /// Permet d'enlever le plat lié à ce checkbox de la liste de plats admissibles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ListePlatsNonAdmissibles.Find(plat => plat.Nom == ((Plat)dgPlatsNonAdmissibles.SelectedItem).Nom).EstTricherie = false;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Enregistrer.
        /// Permet d'enregistrer les changements du membre en ce qui concerne les plats non admissibles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
