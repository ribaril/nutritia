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
using Infralution.Localization.Wpf;
using Nutritia.Logic.Model.Entities;
using System.Threading;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour FenetreDons.xaml
    /// UserControl affichant tout les dons de la base de données
    /// </summary>
    public partial class FenetreDons : UserControl
    {
        private IDonService donService;
        //Le temps précédent du dernier don
        private DateTime previousTime;
        //Le temps actuel du dernier don
        private DateTime currentTime;
        //Thread pour pouvoir actualiser en temps réel les données.
        private Thread dbPoolingThread;
        List<Don> dons;

        public FenetreDons()
        {
            //Delegate pour exécuter du code personnalisé lors du changement de langue de l'UI.
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;
            donService = new MySqlDonService();

            InitializeComponent();

            //Header de la fenêtre
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreDons.Titre;

            //Puisqu'au départ il n'y a pas de don (selon les scripts de bd), pour s'assurer le bon fonctionnement,
            //on met previousTime à la plus petite valeur possible.
            previousTime = DateTime.MinValue;

            //Configuratio du thread. Met en background pour permettre l'arrêt automatique lorsque le logiciel se ferme.
            dbPoolingThread = new Thread(PoolDB);
            dbPoolingThread.IsBackground = true;
            dbPoolingThread.Start();

        }

        /// <summary>
        /// Méthode exécuté continuellement sur un autre thread pour vérifier les mise à jours des dons dans la base de données
        /// </summary>
        private void PoolDB()
        {
            while (true)
            {
                //Récupère le temps du dernier don de la base de données.
                currentTime = donService.LastTimeDon();
                //Si le temps est plus récent que previousTime, il y a donc eu une mise à jour.
                if (currentTime > previousTime)
                {
                    //Dois mettre à jour
                    Dispatcher.Invoke(RefreshDataGrid);
                    previousTime = currentTime;
                }
                //Met le thread en veille pendant un certain temps pour ne pas taxer les ressources réseau et processeur.
                Thread.Sleep(App.POOL_TIME);
            }
        }

        /// <summary>
        /// Méthode permettant de mettre à jour les dons affichés dans la dataGrid.
        /// </summary>
        private void RefreshDataGrid()
        {
            //Récupère tout les dons et remet ItemsSource de la dataGrid.
            dons = donService.RetrieveAll().ToList();
            dgDons.ItemsSource = dons;
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            //Change le titre de la mainwindow correctement lorsque la langue d'affichage change.
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreDons.Titre;
        }
    }
}
