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
    /// Interaction logic for Bannissement.xaml
    /// </summary>
    public partial class Bannissement : UserControl
    {
        public ObservableCollection<Membre> LstMembre { get; set; }
        public ObservableCollection<Membre> LstBanni { get; set; }
        public List<Membre> TousLesMembres { get; set; }
        public Bannissement()
        {
            InitializeComponent();
            LstMembre = new ObservableCollection<Membre>();
            LstBanni = new ObservableCollection<Membre>();
            TousLesMembres = new List<Membre>(ServiceFactory.Instance.GetService<IMembreService>().RetrieveAll());

            RemplirListe();

        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {
            List<Membre> lstAvModif = TousLesMembres;
            TousLesMembres = new List<Membre>(LstBanni);
            TousLesMembres.AddRange(LstMembre);

            RemplirListe();

            foreach (var membre in TousLesMembres)
            {
                //Membre ancienMembre = lstAvModif.FirstOrDefault(x => x.IdMembre == membre.IdMembre);
                //if(membre.EstBanni != ancienMembre.EstBanni)
                ServiceFactory.Instance.GetService<IMembreService>().Update(membre);
            }
        }

        void RemplirListe()
        {
            LstMembre.Clear();
            LstBanni.Clear();

            foreach (var membre in TousLesMembres)
            {

                if (membre.EstBanni)
                {
                    membre.EstBanni = true;
                    LstBanni.Add(membre);
                }
                else
                {
                    membre.EstBanni = false;
                    LstMembre.Add(membre);
                }
            }

            dgBanni.ItemsSource = LstBanni;
            dgMembre.ItemsSource = LstMembre;
        }
    }
}
