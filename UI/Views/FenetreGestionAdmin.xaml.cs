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

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for GestionAdmin.xaml
    /// </summary>
    public partial class GestionAdmin : UserControl
    {
        private IMembreService serviceMembre = ServiceFactory.Instance.GetService<IMembreService>();
        private ObservableCollection<Membre> listMembres;
        private ObservableCollection<Membre> listAdmins;

        public GestionAdmin()
        {
            InitializeComponent();
            //filterDataGrid.ItemsSource = s.ConvertAll(x => new { Value = x });
            listMembres =  new ObservableCollection<Membre>(serviceMembre.RetrieveAll());
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(listMembres);
            filterDataGrid.DataGridCollection.Filter = new Predicate<object>(Filter);
            listAdmins = new ObservableCollection<Membre>(listMembres.Where(m => m.EstAdministrateur).ToList());
            //adminsSysteme = new ObservableCollection<Membre>(serviceMembre.RetrieveAdmins());
            dgAdmin.ItemsSource = listAdmins;
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

        private Membre MemberFromDataContext(CheckBox box)
        {
            var context = box.DataContext;
            if (context is Membre)
            {
                return context as Membre;
            }
            return null;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                CheckBox box = sender as CheckBox;
                Membre membre = MemberFromDataContext(box);
                if (membre != null)
                {
                    if (!listAdmins.Any(x => x == membre))
                    {
                        membre.EstAdministrateur = true;
                        listAdmins.Add(membre);
                    }
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                CheckBox box = sender as CheckBox;
                var context = box.DataContext;
                Membre membre = MemberFromDataContext(box);
                if (membre != null)
                {
                    membre.EstAdministrateur = false;
                    listAdmins.Remove(membre);
                    filterDataGrid.DataGridCollection.Refresh();
                }
            }
        }
    }
}
