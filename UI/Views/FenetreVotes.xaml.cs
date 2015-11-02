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
    /// Logique d'interaction pour FenetreVotes.xaml
    /// </summary>
    public partial class FenetreVotes : UserControl
    {
        private IPlatService PlatService { get; set; }
        private ObservableCollection<Plat> ListePlats;

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreVotes()
        {
            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = "Nutritia - Vote";

            PlatService = ServiceFactory.Instance.GetService<IPlatService>();

            ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveAll());
            dgPlats.ItemsSource = ListePlats;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Tous les plats".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectionComplete_Click(object sender, RoutedEventArgs e)
        {
            gbContenu.Header = "Tous les plats";
            ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveAll());
            dgPlats.ItemsSource = ListePlats;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Nouveautés".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNouveautes_Click(object sender, RoutedEventArgs e)
        {
            gbContenu.Header = "Nouveautés";
            ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = 10, Depart = "Fin" }));
            dgPlats.ItemsSource = ListePlats;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Les plus populaires".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlusPopulaires_Click(object sender, RoutedEventArgs e)
        {
            gbContenu.Header = "Les plus populaires";
            ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = 10, PlusPopulaires = true }));
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
            fenetreIngredients.ShowDialog();
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
                    ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveAll());
                break;
                case "Nouveautés" : 
                    ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = 10, Depart = "Fin" }));
                break;
                case "Les plus populaires" :
                    ListePlats = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { NbResultats = 10, PlusPopulaires = true }));
                break;
            }

            dgPlats.ItemsSource = ListePlats;

        }
    }
}
