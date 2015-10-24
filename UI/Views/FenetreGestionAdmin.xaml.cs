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
    /// Interaction logic for GestionAdmin.xaml
    /// </summary>
    public partial class GestionAdmin : UserControl
    {
        private IMembreService serviceMembre = ServiceFactory.Instance.GetService<IMembreService>();

        public GestionAdmin()
        {
            InitializeComponent();
            //filterDataGrid.ItemsSource = s.ConvertAll(x => new { Value = x });
            IList<Membre> listMembres = serviceMembre.RetrieveAll();
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(listMembres);
            filterDataGrid.DataGridCollection.Filter = new Predicate<object>(Filter);
            List<Membre> admins = listMembres.Where(m => m.EstAdministrateur).ToList();
            dgAdmin.ItemsSource = admins;
        }



        public bool Filter(object obj)
        {
            var data = obj as Membre;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(filterDataGrid.FilterString))
                {
                    return filterDataGrid.Filter(data.NomUtilisateur);
                }
                return true;
            }
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("checked");
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("uncheked");
        }
    }
}
