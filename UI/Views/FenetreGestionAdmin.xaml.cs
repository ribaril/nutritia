using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for GestionAdmin.xaml
    /// </summary>
    public partial class GestionAdmin : UserControl
    {
        private IMembreService serviceMembre = new MySqlMembreService();
        private ObservableCollection<Membre> listMembres;
        private ObservableCollection<Membre> listAdmins;
        private List<Membre> adminDepart;
        private List<Membre> adminFin;
        private List<Membre> membreModifie;
        private Thread dbPoolingThread;
        private DateTime previousTime;
        private DateTime currentTime;

        public GestionAdmin()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreGestionAdmin.Titre;

            //filterDataGrid.ItemsSource = s.ConvertAll(x => new { Value = x });
            listMembres = new ObservableCollection<Membre>(serviceMembre.RetrieveAll());
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(listMembres);
            filterDataGrid.DataGridCollection.Filter = new Predicate<object>(Filter);
            listAdmins = new ObservableCollection<Membre>(listMembres.Where(m => m.EstAdministrateur).ToList());
            //adminsSysteme = new ObservableCollection<Membre>(serviceMembre.RetrieveAdmins());
            dgAdmin.ItemsSource = listAdmins;
            adminDepart = listAdmins.ToList();

            currentTime = previousTime = listMembres.Max(m => m.DerniereMaj);

            dbPoolingThread = new Thread(PoolDB);
            dbPoolingThread.Start();
        }

        private void PoolDB()
        {
            while (true)
            {
                currentTime = serviceMembre.LastUpdatedTime();
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
            dgAdmin.SelectedIndex = -1;
            listMembres = new ObservableCollection<Membre>(serviceMembre.RetrieveAll());
            listAdmins = new ObservableCollection<Membre>(listMembres.Where(m => m.EstAdministrateur).ToList());
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(listMembres);
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
            adminFin = listAdmins.ToList();
            membreModifie = adminDepart.Union(adminFin).Except(adminDepart.Intersect(adminFin)).ToList();
            //membreModifie = adminDepart.Except(adminFin).ToList();

            foreach (Membre m in membreModifie)
            {
                serviceMembre.Update(m);
            }
            if (membreModifie.Count != 0)
                adminDepart = listAdmins.ToList();
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

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreGestionAdmin.Titre;
        }
    }
}
