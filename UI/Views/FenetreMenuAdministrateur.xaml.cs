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

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for MenuAdministrateur.xaml
    /// Fenêtre principal avec les options des administrateurs
    /// </summary>
    public partial class FenetreMenuAdministrateur : UserControl
    {
        public FenetreMenuAdministrateur()
        {
            //Enregistre un delegate exécuté lorsque la langue d'affichage change.
            //Utilisé pour avoir du code plus complexe dans les cas où l'UI ne peut pas changer
            //automatiquement. Par example, le header des fenêtres.
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreMenuAdministrateur.Titre;
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
                (mainWindow as MainWindow).ChangerVue(new FenetreGestionAdmin());
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

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            //Change le titre de la mainwindow correctement lorsque la langue d'affichage change.
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreMenuAdministrateur.Titre;
        }

        private void btnDons_Click(object sender, RoutedEventArgs e)
        {
            IApplicationService mainWindow = ServiceFactory.Instance.GetService<IApplicationService>();

            if (mainWindow is MainWindow)
            {
                (mainWindow as MainWindow).ChangerVue(new FenetreDons());
            }
        }
    }
}