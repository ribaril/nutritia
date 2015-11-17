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
using Nutritia.UI.Views;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace Nutritia
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IApplicationService
	{
		private const int SLEEP_NOTIFICATION_TIME = 10000;

		static public List<Plat> NouveauxPlats = new List<Plat>();

		public MainWindow()
		{
			InitializeComponent();
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Langue);

			Configurer();

			presenteurContenu.Content = new MenuPrincipal();

			Thread tNotif = new Thread(ChargerChangementNotif);

			tNotif.Start();
		}


		private void ChargerChangementNotif()
		{
			List<Plat> lstPlat;
			List<int> lstIdNouveauPlat = new List<int>();
			String MajUsager;
			while (true)
			{
				if (!String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
				{
					MajUsager = ServiceFactory.Instance.GetService<IMembreService>().RetrieveMiseAJOur();
					if (App.MembreCourant.DerniereMaj != "")
					{
						lstPlat = ServiceFactory.Instance.GetService<IPlatService>().RetrieveAll().ToList();
						lstIdNouveauPlat.Clear();

						foreach (var plat in lstPlat)
						{
							if (plat.DateAjout.CompareTo(App.MembreCourant.DerniereMaj) > -1)
							{
								lstIdNouveauPlat.Add((int)plat.IdPlat);
							}
						}

						if (lstIdNouveauPlat.Count > 0)
						{
							NouveauxPlats = lstPlat.FindAll(plat => lstIdNouveauPlat.Contains((int)plat.IdPlat));
						}

					}
				}

				Thread.Sleep(500); // On fait une pause pour ne pas surcharger le CPU

			}
		}

		private void CreerBoiteNotification()
		{

			StringBuilder sbNotifs = new StringBuilder();
			sbNotifs.AppendLine("Nouveaux plats : ");
			foreach (var plat in NouveauxPlats)
			{
				sbNotifs.AppendLine(plat.Nom);
			}

			MessageBox.Show(sbNotifs.ToString());
		}

		private void Configurer()
		{
			// Inscription des différents services de l'application dans le ServiceFactory.
			ServiceFactory.Instance.Register<IRestrictionAlimentaireService, MySqlRestrictionAlimentaireService>(new MySqlRestrictionAlimentaireService());
			ServiceFactory.Instance.Register<IObjectifService, MySqlObjectifService>(new MySqlObjectifService());
			ServiceFactory.Instance.Register<IUniteMesureService, MySqlUniteMesureService>(new MySqlUniteMesureService());
			ServiceFactory.Instance.Register<IPreferenceService, MySqlPreferenceService>(new MySqlPreferenceService());
			ServiceFactory.Instance.Register<IAlimentService, MySqlAlimentService>(new MySqlAlimentService());
			ServiceFactory.Instance.Register<IPlatService, MySqlPlatService>(new MySqlPlatService());
			ServiceFactory.Instance.Register<IMenuService, MySqlMenuService>(new MySqlMenuService());
			ServiceFactory.Instance.Register<IMembreService, MySqlMembreService>(new MySqlMembreService());
			ServiceFactory.Instance.Register<IVersionLogicielService, MySqlVersionLogicielService>(new MySqlVersionLogicielService());
			ServiceFactory.Instance.Register<IApplicationService, MainWindow>(this);
		}

		public void ChangerVue<T>(T vue)
		{
			presenteurContenu.Content = vue as UserControl;
		}

		private void btnParam_Click(object sender, RoutedEventArgs e)
		{
			(new FenetreParametres()).ShowDialog();
		}

		private void btnRetour_Click(object sender, RoutedEventArgs e)
		{
			if (App.MembreCourant.IdMembre == null)
				ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipal());
			else
			{
				if (presenteurContenu.Content is Bannissement || presenteurContenu.Content is GestionAdmin || presenteurContenu.Content is GestionRepertoire)
					ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuAdministrateur());
				else
					ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
			}
		}

		private void btnInfo_Click(object sender, RoutedEventArgs e)
		{
			(new FenetreAPropos()).ShowDialog();
			//ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreAPropos());
		}

		private void btnAide_Click(object sender, RoutedEventArgs e)
		{
			FenetreAide fenetreAide = new FenetreAide(presenteurContenu.Content.GetType().Name);
			fenetreAide.Show();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Thread newThread = new Thread(LastestVersionPopUp);
			newThread.Start();
		}

		private void LastestVersionPopUp()
		{
			bool isOlder = false;
			IVersionLogicielService serviceMembre = ServiceFactory.Instance.GetService<IVersionLogicielService>();

			VersionLogiciel latestVersionLogiciel = serviceMembre.RetrieveLatest();

			if (String.IsNullOrEmpty(latestVersionLogiciel.Version))
			{
				return;
			}

			Version versionBD = new Version(latestVersionLogiciel.Version);
			Version versionLocal = new Version(FileVersionInfo.GetVersionInfo(App.ResourceAssembly.Location).FileVersion);

			if (versionBD.Major > versionLocal.Major || versionBD.Minor > versionLocal.Minor || versionBD.Build > versionLocal.Build || versionBD.Revision > versionLocal.Revision)
			{
				isOlder = true;
			}


			Thread.Sleep(SLEEP_NOTIFICATION_TIME);

			if (isOlder)
			{
				string message = "Version: " + latestVersionLogiciel.Version + " disponible.\nLien de téléchargement: " + latestVersionLogiciel.DownloadLink;
				MessageBox.Show(
					message,
					"Nouvelle version disponible",
					MessageBoxButton.OK,
					MessageBoxImage.Information);
			}
		}

		private void btnNotification_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
			{
				nbrNotif.Text = "";
				App.MembreCourant.DerniereMaj = DateTime.Now.ToString();
				ServiceFactory.Instance.GetService<IMembreService>().Update(App.MembreCourant);
				CreerBoiteNotification();
				NouveauxPlats.Clear();
			}
		}


		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (!String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
			{
				if (NouveauxPlats.Count > 0)
				{
					nbrNotif.Text = NouveauxPlats.Count.ToString();
				}
			}
			else
			{
				nbrNotif.Text = "";
			}
		}

		
	}
}
