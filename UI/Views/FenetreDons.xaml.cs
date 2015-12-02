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
    /// </summary>
    public partial class FenetreDons : UserControl
    {
        private IDonService donService;
        private DateTime previousTime;
        private DateTime currentTime;
        private Thread dbPoolingThread;
        List<Transaction> transactions;

        public FenetreDons()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;
            donService = new MySqlDonService();

            InitializeComponent();

            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreDons.Titre;

            previousTime = DateTime.MinValue;

            dbPoolingThread = new Thread(PoolDB);
            dbPoolingThread.IsBackground = true;
            dbPoolingThread.Start();

        }


        private void PoolDB()
        {
            while (true)
            {
                currentTime = donService.LastTimeDon();
                if (currentTime > previousTime)
                {
                    //Dois mettre à jour
                    Dispatcher.Invoke(RefreshDataGrid);
                    previousTime = currentTime;
                }
                Thread.Sleep(App.POOL_TIME);
            }
        }

        private void RefreshDataGrid()
        {
            transactions = donService.RetrieveAll().ToList();
            dgDons.ItemsSource = transactions;
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreDons.Titre;
        }
    }
}
