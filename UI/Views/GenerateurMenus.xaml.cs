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
        private ObservableCollection<Plat> ListePlats;

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

            ListePlats = new ObservableCollection<Plat>(ServiceFactory.Instance.GetService<IPlatService>().RetrieveAll(new RetrievePlatArgs {Categorie = "Plat principal"}));

            dgMenus.ItemsSource = ListePlats;
            
            // GenererRangees(NbRepas);

        }

        private void btnIngredients_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)dgMenus.SelectedItem;
            
            StringBuilder sb = new StringBuilder();
            foreach (Aliment aliment in platSelectionne.ListeIngredients)
            {
                sb.AppendLine(aliment.Nom);
            }

            MessageBox.Show(sb.ToString());
        }

        /*
        /// <summary>
        /// Méthode permettant de générer les rangées de la grid contenant les menus.
        /// </summary>
        /// <param name="nbRangees">Le nombre de rangées a généré.</param>
        private void GenererRangees(int nbRangees)
        {
            RowDefinition rowDefinition;

            for (int i = 0; i < nbRangees; i++)
            {
                rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(125);
                grdMenu.RowDefinitions.Add(rowDefinition);
            }
        }
        */
    }
}
