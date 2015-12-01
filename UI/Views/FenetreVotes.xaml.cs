using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logique d'interaction pour FenetreVotes.xaml
    /// </summary>
    public partial class FenetreVotes : UserControl
    {
        private IPlatService PlatService { get; set; }
        private ObservableCollection<Plat> ListePlats { get; set; }
        private int NbResultatsAffiches { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreVotes()
        {
            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = "Nutritia - Vote";

            PlatService = ServiceFactory.Instance.GetService<IPlatService>();

            NbResultatsAffiches = 10;

            // Par défaut, tous les plats sont affichés.
            btnSelectionComplete_Click(null, null);
        }

        /// <summary>
        /// Méthode permettant de déterminer la note conviviale de chacun des plats de la liste.
        /// </summary>
        private void DeterminerNoteConviviale()
        {
            foreach(Plat platCourant in ListePlats)
            {
                platCourant.DeterminerNoteConviviale();
            }
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Tous les plats".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectionComplete_Click(object sender, RoutedEventArgs e)
        {
            gbContenu.Header = Nutritia.UI.Ressources.Localisation.FenetreVotes.TousLesPlats;
            ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveAll().OrderBy(plat => plat.Nom));
            DeterminerNoteConviviale();
            dgPlats.ItemsSource = ListePlats;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Nouveautés".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNouveautes_Click(object sender, RoutedEventArgs e)
        {
            gbContenu.Header = Nutritia.UI.Ressources.Localisation.FenetreVotes.Nouveaute;
            NbResultatsAffiches = 10;
            if (Regex.IsMatch(txtNbResultats.Text, @"^\d+$")) { NbResultatsAffiches = Convert.ToInt32(txtNbResultats.Text); }
            ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = NbResultatsAffiches, Depart = "Fin" }));
            DeterminerNoteConviviale();
            dgPlats.ItemsSource = ListePlats;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Les plus populaires".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlusPopulaires_Click(object sender, RoutedEventArgs e)
        {
            gbContenu.Header = Nutritia.UI.Ressources.Localisation.FenetreVotes.LesPlusPopulaires;
            NbResultatsAffiches = 10;
            if (Regex.IsMatch(txtNbResultats.Text, @"^\d+$")) { NbResultatsAffiches = Convert.ToInt32(txtNbResultats.Text); }
            ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = NbResultatsAffiches, PlusPopulaires = true }));
            DeterminerNoteConviviale();
            dgPlats.ItemsSource = ListePlats;
        }

        /// <summary>
        /// Événement lancé lorsque la roulette de la souris est utilisée dans le "scrollviewer" contenant les plats.
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
        /// Événement lancé sur un clique du bouton "Informations".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInformations_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)dgPlats.SelectedItem;

            FenetreIngredients fenetreIngredients = new FenetreIngredients(platSelectionne, 1);
            fenetreIngredients.Show();
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Voter".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVoter_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)dgPlats.SelectedItem;

            FenetreVote popupVote = new FenetreVote(platSelectionne);
            popupVote.ShowDialog();

            switch(gbContenu.Header.ToString())
            {
                case "Tous les plats" :
                    btnSelectionComplete_Click(null, null);
                break;
                case "Nouveautés" :
                    btnNouveautes_Click(null, null);
                break;
                case "Les plus populaires" :
                    btnPlusPopulaires_Click(null, null);
                break;
            }

        }

        /// <summary>
        /// Événement lancé lorsque l'utilisateur tape quelque chose dans le champ de recherche.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRecherche_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            List<Plat> listePlatsTemp = new List<Plat>();

            switch (gbContenu.Header.ToString())
            {
                case "Tous les plats":
                    listePlatsTemp = new List<Plat>(PlatService.RetrieveAll());
                    break;
                case "Nouveautés":
                    listePlatsTemp = new List<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = NbResultatsAffiches, Depart = "Fin" }));
                    break;
                case "Les plus populaires":
                    listePlatsTemp = new List<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = NbResultatsAffiches, PlusPopulaires = true }));
                    break;
            }

            string recherche = ((TextBox)sender).Text;
            ListePlats = new ObservableCollection<Plat>(listePlatsTemp.FindAll(plat => plat.Nom.ToLower().Contains(recherche.ToLower())).ToList());
            DeterminerNoteConviviale();
            dgPlats.ItemsSource = ListePlats;
        }

		public void Rafraichir()
		{
			ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveAll());
            DeterminerNoteConviviale();

            switch (gbContenu.Header.ToString())
            {
                case "Tous les plats":
                    btnSelectionComplete_Click(null, null);
                    break;
                case "Nouveautés":
                    btnNouveautes_Click(null, null);
                    break;
                case "Les plus populaires":
                    btnPlusPopulaires_Click(null, null);
                    break;
            }
		}
    }
}
