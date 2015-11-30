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
using Infralution.Localization.Wpf;

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
            CultureManager.UICulture = new CultureInfo(Properties.Settings.Default.Langue);
            InitializeComponent();
            ConfigurerTaille();
            Configurer();

            presenteurContenu.Content = new MenuPrincipal();
        }

        private void ConfigurerTaille()
        {
            Width = App.APP_WIDTH;
            Height = App.APP_HEIGHT;
            MinWidth = App.APP_WIDTH;
            MinHeight = App.APP_HEIGHT;
        }

        public void Configurer()
        {
            // Inscription des différents services de l'application dans le ServiceFactory.
            ServiceFactory.Instance.Register<IRestrictionAlimentaireService, MySqlRestrictionAlimentaireService>(new MySqlRestrictionAlimentaireService());
            ServiceFactory.Instance.Register<IObjectifService, MySqlObjectifService>(new MySqlObjectifService());
            ServiceFactory.Instance.Register<IUniteMesureService, MySqlUniteMesureService>(new MySqlUniteMesureService());
            ServiceFactory.Instance.Register<IPreferenceService, MySqlPreferenceService>(new MySqlPreferenceService());
            ServiceFactory.Instance.Register<IAlimentService, MySqlAlimentService>(new MySqlAlimentService());
            ServiceFactory.Instance.Register<IPlatService, MySqlPlatService>(new MySqlPlatService());
            ServiceFactory.Instance.Register<ISuiviPlatService, MySqlSuiviPlatService>(new MySqlSuiviPlatService());
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
            App.Current.MainWindow.ResizeMode = ResizeMode.CanMinimize;
            App.Current.MainWindow.Width = App.APP_WIDTH;
            App.Current.MainWindow.Height = App.APP_HEIGHT;
            App.Current.MainWindow.WindowState = WindowState.Normal;

            if (presenteurContenu.Content is FenetreListeEpicerie)
                ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreGenerateurMenus(FenetreListeEpicerie.MenuGenere, FenetreListeEpicerie.NbColonnesMenu));
            else if (App.MembreCourant.IdMembre == null)
				ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipal()); 
			else
            {
                if(presenteurContenu.Content is Bannissement || presenteurContenu.Content is GestionAdmin || presenteurContenu.Content is GestionRepertoire)
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

            if (versionBD > versionLocal)
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


    }
}
