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
    /// Logique d'interaction pour ListeEpicerie.xaml
    /// </summary>
    public partial class ListeEpicerie : UserControl
    {
        public Menu MenuGenere { get; set; }
        public List<Aliment> ListeAliments { get; set; }
        private IPlatService PlatService { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public ListeEpicerie(Menu menu)
        {
            InitializeComponent();

            MenuGenere = menu;
            PlatService = ServiceFactory.Instance.GetService<IPlatService>();

            GenererListe();

        }

        /// <summary>
        /// Méthode permettant de générer la liste d'épicerie.
        /// </summary>
        private void GenererListe()
        {
            foreach(Plat platCourant in MenuGenere.ListePlats)
            {
                // platCourant.ListeIngredient = PlatService.RetrieveAlimentsPlat(new RetrievePlatArgs { IdPlat = platCourant.IdPlat });
            }

            dgListeEpicerie.ItemsSource = ListeAliments;
        }
    }
}
