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
		public ObservableCollection<Membre> lstMembre { get; set; }
		public ObservableCollection<Membre> lstBanni { get; set; }
		public Bannissement()
        {
            InitializeComponent();
			lstMembre = new ObservableCollection<Membre>();
			lstBanni = new ObservableCollection<Membre>();
			List<Membre> lstTemp = new List<Membre>(ServiceFactory.Instance.GetService<IMembreService>().RetrieveAll());

			Membre membre = new Membre();
			membre.Prenom = "Jacque";
			membre.Nom = "Roll";
			membre.NomUtilisateur = "ElJackoLantern";

			Membre membre2 = new Membre();
			membre2.Prenom = "Guillaume";
			membre2.Nom = "Leblond";
			membre2.NomUtilisateur = "ElSceauxDeau";
			membre2.EstBanni = true;

			Membre membre3 = new Membre();
			membre3.Prenom = "Coco";
			membre3.Nom = "Nolo";
			membre3.NomUtilisateur = "Lapoire";
			membre3.EstBanni = true;

			lstTemp.Add(membre);
			lstTemp.Add(membre2);
			lstTemp.Add(membre3);

			RemplirListe(lstTemp);

			

			
		}

		private void btnAppliquer_Click(object sender, RoutedEventArgs e)
		{
			List<Membre> lstTemp = new List<Membre>(lstBanni);
			lstTemp.AddRange(lstMembre);

			RemplirListe(lstTemp);
		}

		void RemplirListe(List<Membre> lstTemp)
		{
			lstMembre.Clear();
			lstBanni.Clear();

			foreach (var membre in lstTemp)
			{

				if (membre.EstBanni)
					lstBanni.Add(membre);
				else
					lstMembre.Add(membre);
			}

			dgBanni.ItemsSource = lstBanni;
			dgMembre.ItemsSource = lstMembre;
		}
	}
}
