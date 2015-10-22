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
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(serviceMembre.RetrieveAll());
            filterDataGrid.DataGridCollection.Filter = new Predicate<object>(Filter);
        }



        public bool Filter(object obj)
        {
            var data = obj as Membre;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(filterDataGrid.FilterString))
                {
                    return filterDataGrid.Filter(data.Nom);
                }
                return true;
            }
            return false;
        }

    }
}
