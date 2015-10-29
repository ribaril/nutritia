using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Logique d'interaction pour FenetreVote.xaml
    /// </summary>
    public partial class FenetreVote : Window
    {
        public Plat PlatSelectionne { get; set; }
        public IPlatService PlatService { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreVote(Plat plat)
        {
            InitializeComponent();

            PlatService = ServiceFactory.Instance.GetService<IPlatService>();
            PlatSelectionne = plat;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Confirmer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Confirmer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirmer_Click(object sender, RoutedEventArgs e)
        {
            double note = Convert.ToDouble(((ComboBoxItem)cboNote.SelectedItem).Content);
            int nbVotesActuel = PlatSelectionne.NbVotes;

            PlatSelectionne.NbVotes++;

            if(PlatSelectionne.Note == null)
            {
                PlatSelectionne.Note = note;
            }
            else
            {
                double sommeNote = (double)PlatSelectionne.Note * nbVotesActuel;
                sommeNote += note;
                PlatSelectionne.Note = sommeNote / PlatSelectionne.NbVotes;
            }
            
            // Mise à jour dans la base de données.
            PlatSelectionne.Note = Math.Round((Double)PlatSelectionne.Note, 2);
            PlatService.Update(PlatSelectionne);

            Close();
        }
    }
}
