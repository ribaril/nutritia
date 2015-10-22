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

            PlatService = ServiceFactory.Instance.GetService<IPlatService>();
            ListePlats = new ObservableCollection<Plat>();
            // Plat "hardcodé" en guise de preuve de concept ...
            Plat plat = PlatService.Retrieve(new RetrievePlatArgs { IdPlat = 1 });
            ListePlats.Add(plat);
            plat = PlatService.Retrieve(new RetrievePlatArgs { IdPlat = 2 });
            ListePlats.Add(plat);
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
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Les plus populaires".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlusPopulaires_Click(object sender, RoutedEventArgs e)
        {
            gbContenu.Header = "Les plus populaires";
        }
    }
}
