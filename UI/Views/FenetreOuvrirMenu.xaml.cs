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
    /// Logique d'interaction pour FenetreOuvrirMenu.xaml
    /// </summary>
    public partial class FenetreOuvrirMenu : Window
    {
        private IMembreService MembreService { get; set; }
        private IMenuService MenuService { get; set; }
        private ObservableCollection<Menu> ListeMenus { get; set; }
        
        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreOuvrirMenu()
        {
            InitializeComponent();

            MembreService = ServiceFactory.Instance.GetService<IMembreService>();
            MenuService = ServiceFactory.Instance.GetService<IMenuService>();

            // Prévenir une erreur dans le cas où la connexion est établie à partir d'une création de compte.
            // Dans ce cas, quand un membre est créé, aucun id ne lui est associé.
            Membre membreCourant = MembreService.Retrieve(new RetrieveMembreArgs{ NomUtilisateur = App.MembreCourant.NomUtilisateur });
            
            ListeMenus = new ObservableCollection<Menu>(MenuService.RetrieveSome(new RetrieveMenuArgs { IdMembre = (int)membreCourant.IdMembre }));

            dgMenus.ItemsSource = ListeMenus;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Ouvrir.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOuvrir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
