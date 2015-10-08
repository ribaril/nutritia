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
    /// Logique d'interaction pour GenerateurMenus.xaml
    /// </summary>
    public partial class GenerateurMenus : UserControl
    {
        private int NbRepas { get; set; }
        private int NbPersonnes { get; set; }

        public GenerateurMenus()
        {
            InitializeComponent();
        }
  
        /// <summary>
        /// Événement lancé sur un clique du bouton Générer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerer_Click(object sender, RoutedEventArgs e)
        {
            if (rbMenuUnique.IsChecked != null && (bool)rbMenuUnique.IsChecked) { NbRepas = 1; }
            if (rbMenuJournalier.IsChecked != null && (bool)rbMenuJournalier.IsChecked) { NbRepas = 3; }
            if (rbMenuHebdomadaire.IsChecked != null && (bool)rbMenuHebdomadaire.IsChecked) { NbRepas = 21; }
            
            NbPersonnes = Convert.ToInt32(((ComboBoxItem)cboNbPersonnes.SelectedItem).Content);
        }

        /// <summary>
        /// Méthode permettant de générer les rows dans la grid contenant les menus.
        /// </summary>
        /// <param name="nbRows">Le nombre de rows a générées.</param>
        private void GenererRows(int nbRows)
        {
            
        }
    }
}
