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
using Nutritia.UI.Views;

namespace Nutritia
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IApplicationService
    {
        public MainWindow()
        {
            InitializeComponent();
            Configurer();

            presenteurContenu.Content = new MenuPrincipal();

        }

        private void Configurer()
        {
            //Déclaration du ApplicationService
            /*
            ServiceFactory.Instance.Register<IApplicationService, MainWindow>(this);
            ServiceFactory.Instance.Register<IClientService, SimpleClientService>(new SimpleClientService());
            ServiceFactory.Instance.Register<IProprietaireService, SimpleProprietaireService>(new SimpleProprietaireService());
            ServiceFactory.Instance.Register<IProprieteService, SimpleProprieteService>(new SimpleProprieteService());
            */
        }

        public void ChangerVue<T>(T vue)
        {
            presenteurContenu.Content = vue as UserControl;
        }
    }
}
