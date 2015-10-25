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

            //SearchBox
            dgRecherche.DataGridCollection = CollectionViewSource.GetDefaultView(TousLesMembres);
            dgRecherche.DataGridCollection.Filter = new Predicate<object>(Filter);

        }

        private bool Filter(object obj)
        {
            var data = obj as Membre;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(dgRecherche.FilterString))
                {
                    return dgRecherche.Filter(data.NomUtilisateur);
                }
                return true;
            }
            return false;
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
		{
			List<Membre> lstAncienMembre = new List<Membre>(TousLesMembres);
			TousLesMembres = new List<Membre>(LstBanni);
			TousLesMembres.AddRange(LstMembre);

			RemplirListe();
			

			foreach (var membre in TousLesMembres)
			{
				foreach (var mAncien in lstAncienMembre)
				{
					if (mAncien.IdMembre == membre.IdMembre)
					{
						if (mAncien.EstBanni != membre.EstBanni)
							ServiceFactory.Instance.GetService<IMembreService>().Update(membre);
						break;
					}
						
				}

			}
			

		}

		void RemplirListe()
		{
			LstMembre.Clear();
			LstBanni.Clear();
			
			foreach (var membre in TousLesMembres)
				(membre.EstBanni ? LstBanni : LstMembre).Add(membre.Cloner()); // Appel une copie indépendante


			dgBanni.ItemsSource = LstBanni;
			//dgMembre.ItemsSource = LstMembre;
		}

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
