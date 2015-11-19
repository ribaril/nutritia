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
using System.ComponentModel;

namespace Nutritia
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IApplicationService
    {
        private const int SLEEP_NOTIFICATION_TIME = 10000;

        // Thread asincrone qui regarde les modifications dans la BD
        private Thread tNotif { get; set; }
        private Thread ThreadNotif { get; set; }

        // Permet d'interargir avec le threas asyncrone pour sauvegardé les nouveau plats dan la BD
        static public List<Plat> NouveauxPlats { get; set; }


        public MainWindow()
        {

            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Langue);

            Configurer();

            NouveauxPlats = new List<Plat>();

            presenteurContenu.Content = new MenuPrincipal();

            // On lance le thread
            tNotif = new Thread(VerifierChangementBD);
            tNotif.Start();
        }


        /// <summary>
        /// Méthode appellé en asyncrone qui boucle toute les demi-secondes pour verifier s'il y a des changements dans la BD
        /// </summary>
        private void VerifierChangementBD()
        {
            List<Plat> lstPlat;
            List<int> lstIdNouveauPlat = new List<int>();
            while (true)
            {
                if (!String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
                {
                    bool ancienStatutAdmin = App.MembreCourant.EstAdministrateur;
                    bool ancienStatutBanni = App.MembreCourant.EstBanni;
                    
                    App.MembreCourant = ServiceFactory.Instance.GetService<IMembreService>().Retrieve(new RetrieveMembreArgs { IdMembre = App.MembreCourant.IdMembre });

                    if (App.MembreCourant.EstAdministrateur != ancienStatutAdmin 
                        || App.MembreCourant.EstBanni != ancienStatutBanni)
                    {
                        Dispatcher.Invoke(AppliquerNouveauChangementStatut);
                    }

                    if (App.MembreCourant.DerniereMaj != "")
                    {
                        lstPlat = ServiceFactory.Instance.GetService<IPlatService>().RetrieveAll().ToList();
                        lstIdNouveauPlat.Clear();

                        foreach (var plat in lstPlat)
                        {
                            if (plat.DateAjout.CompareTo(App.MembreCourant.DerniereMaj) > -1)
                            {
                                if (!lstIdNouveauPlat.Exists(id => id == plat.IdPlat))
                                    lstIdNouveauPlat.Add((int)plat.IdPlat);
                            }
                        }

                        if (lstIdNouveauPlat.Count > 0)
                        {
                            NouveauxPlats = lstPlat.FindAll(plat => lstIdNouveauPlat.Contains((int)plat.IdPlat));
                            Dispatcher.Invoke(DessinerNotification);
                        }

                    }

                }
                else
                {
                    NouveauxPlats.Clear(); // On vide la liste si l'utilisateur n'est pas connécté
                }

                Thread.Sleep(500); // On fait une pause pour ne pas surcharger le CPU

            }
        }

        /// <summary>
        /// Charge toutes les ressources du service factory
        /// </summary>
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

        /// <summary>
        /// Méthode qui permet de changer de userControl
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vue"></param>
        public void ChangerVue<T>(T vue)
        {
            presenteurContenu.Content = vue as UserControl;
        }

        /// <summary>
        /// Ouvre la fenêtre des paramètres en modal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnParam_Click(object sender, RoutedEventArgs e)
        {
            (new FenetreParametres()).ShowDialog();
        }

        /// <summary>
        /// Permet, suivant le contexte, de revenir au menu précédent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Ouvre la fenêtre des infos sur l'application en modal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAPropos_Click(object sender, RoutedEventArgs e)
        {
            (new FenetreAPropos()).ShowDialog();
            //ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreAPropos());
        }

        /// <summary>
        /// Ouvre la fenêtre d'aide de l'application en modeless
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAide_Click(object sender, RoutedEventArgs e)
        {
            FenetreAide fenetreAide = new FenetreAide(presenteurContenu.Content.GetType().Name);
            fenetreAide.Show();
        }

        /// <summary>
        /// Quand la fenêtre se charge, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadNotif = new Thread(LastestVersionPopUp);
            ThreadNotif.Start();
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


            //Thread.Sleep(SLEEP_NOTIFICATION_TIME);

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
            if (!String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
            {
                if (NouveauxPlats.Count > 0)
                {
                    NouveauxPlats.Clear();
                    nbrNotif.Text = "";
                    App.MembreCourant.DerniereMaj = DateTime.Now.ToString();
                    ServiceFactory.Instance.GetService<IMembreService>().Update(App.MembreCourant);
                    CreerBoiteNotification();
                }
            }
        }

        /// <summary>
        /// Formate les nouveaux plats pour les affichers lors du clique sur le bouton de notif
        /// </summary>
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

        /// <summary>
        /// Génere le nbr de notification pour le dessiner à côté de l'image de notif
        /// </summary>
        private void DessinerNotification()
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

        public void AppliquerNouveauChangementStatut()
        {
            if (!String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
            {
                if (App.MembreCourant.EstBanni)
                {
                    App.MembreCourant = new Membre();
                    MainWindow.NouveauxPlats.Clear();
                    ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipal());
                }
                else
                {
                    if (presenteurContenu.Content is MenuPrincipalConnecte)
                        ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
                }

                if (App.MembreCourant.EstAdministrateur)
                {
                    if (presenteurContenu.Content is MenuPrincipalConnecte)
                        ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
                }
                else
                {
                    if (presenteurContenu.Content is Bannissement
                    || presenteurContenu.Content is GestionAdmin
                    || presenteurContenu.Content is GestionRepertoire
                    || presenteurContenu.Content is MenuAdministrateur)
                        ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
                }
                    
            }
            
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            tNotif.Abort();
            ThreadNotif.Abort();
        }



    }
}
