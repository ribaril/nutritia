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

        public MainWindow()
		{
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Langue);
            InitializeComponent();
            Configurer();

            presenteurContenu.Content = new MenuPrincipal();
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
			if(App.MembreCourant.IdMembre == null)
				ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipal());
			else if(App.MembreCourant.EstAdministrateur)
				ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuAdministrateur());
			else
				ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
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
            IVersionLogicielService serviceMembre = ServiceFactory.Instance.GetService<IVersionLogicielService>();

            VersionLogiciel latestVersionLogiciel = serviceMembre.RetrieveLatest();

            Thread.Sleep(SLEEP_NOTIFICATION_TIME);
    
            if (latestVersionLogiciel.Version.Equals(FileVersionInfo.GetVersionInfo(App.ResourceAssembly.Location).FileVersion) == false )
            {
                string message = "Version: " + latestVersionLogiciel.Version + " disponible.\nLien de téléchargement: " + latestVersionLogiciel.DownloadLink;
                MessageBox.Show(
                    message,
                    "Nouvelle version disponible",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
