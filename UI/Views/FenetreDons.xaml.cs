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

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour FenetreDons.xaml
    /// </summary>
    public partial class FenetreDons : UserControl
    {
        private IDonService donService;

        public FenetreDons()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;
            donService = ServiceFactory.Instance.GetService<IDonService>();

            InitializeComponent();

            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreDons.Titre;

            List<Transaction> transactions = donService.RetrieveAll().ToList();
            dgDons.ItemsSource = transactions;
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreDons.Titre;
        }
    }
}
