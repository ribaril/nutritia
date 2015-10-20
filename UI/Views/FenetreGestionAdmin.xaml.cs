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
        public GestionAdmin()
        {
            InitializeComponent();
            //filterDataGrid.ItemsSource = s.ConvertAll(x => new { Value = x });
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(TestData);
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


        public IEnumerable<Membre> TestData
        {
            get
            {
                yield return new Membre { Nom = "Lecteur", Prenom = "Archibal" };
                yield return new Membre { Nom = "Charron", Prenom = "Yannick" };
                yield return new Membre { Nom = "Benedict", Prenom = "Sophie" };
                yield return new Membre { Nom = "DiBonzo", Prenom = "Alphonzo" };
                yield return new Membre { Nom = "Desjardin", Prenom = "Richard" };
                yield return new Membre { Nom = "Galilo", Prenom = "Constantin" };
                yield return new Membre { Nom = "Hellsing", Prenom = "Alucard" };
                yield return new Membre { Nom = "Qwerty", Prenom = "Vlad" };
                yield return new Membre { Nom = "Dickingson", Prenom = "Jojo" };
                yield return new Membre { Nom = "Pepito", Prenom = "Alfred" };
            }
        }
    }
}
