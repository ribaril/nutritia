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

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for MenuAdministrateur.xaml
    /// </summary>
    public partial class MenuAdministrateur : UserControl
    {
        public MenuAdministrateur()
        {
            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = "Nutritia - Menu Administrateur";
        }

        private void btnRepertoire_Click(object sender, RoutedEventArgs e)
        {

            IApplicationService mainWindow = ServiceFactory.Instance.GetService<IApplicationService>();

            if (mainWindow is MainWindow)
            {
                (mainWindow as MainWindow).ChangerVue(new GestionRepertoire());
            }

        }

        private void btngestionAdmin_Click(object sender, RoutedEventArgs e)
        {
            IApplicationService mainWindow = ServiceFactory.Instance.GetService<IApplicationService>();

            if (mainWindow is MainWindow)
            {
                (mainWindow as MainWindow).ChangerVue(new GestionAdmin());
            }
        }

        private void btnBanUtilisateur_Click(object sender, RoutedEventArgs e)
        {
            IApplicationService mainWindow = ServiceFactory.Instance.GetService<IApplicationService>();

            if (mainWindow is MainWindow)
            {
                (mainWindow as MainWindow).ChangerVue(new Bannissement());
            }
        }
    }
}