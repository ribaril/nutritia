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
    /// UserControl permettant de faire la gestion des droits d'administration des membres
    /// </summary>
    public partial class FenetreGestionAdmin : UserControl
    {
        private IMembreService serviceMembre = new MySqlMembreService();
        private ObservableCollection<Membre> listMembres;
        private ObservableCollection<Membre> listAdmins;
        //Contiendra les administrateurs actuel du système
        private List<Membre> adminDepart;
        //Contiendra les administrateurs du système et ceux rajouter par nous même
        //Sera utilisé avec adminDepart pour avoir seulement les membres modifiés et réduire le traffique réseau.
        private List<Membre> adminFin;
        //Contiendra uniquement les membres qui ont été modifiés
        private List<Membre> membreModifie;
        //Thread utilisé pour avoir un rafraichissement en temps réel des données de la fenêtre.
        private Thread dbPoolingThread;
        //Le temps précédent de la dernière mise à jour des membres
        private DateTime previousTime;
        //Le temps actuel de la dernière mise à jour des membres
        private DateTime currentTime;

        public FenetreGestionAdmin()
        {
            //Delegate pour exécuter du code personnalisé lors du changement de langue de l'UI.
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreGestionAdmin.Titre;

            //Récupère tout les membres de la BD.
            listMembres = new ObservableCollection<Membre>(serviceMembre.RetrieveAll());

            //Récupère le temps de DerniereMaj (Mise à jour) des membres le plus récent et l'enregistre dans
            //currentTime et previousTime.
            currentTime = previousTime = listMembres.Max(m => m.DerniereMaj);

            //On enlève le membre actuellement connecté de la liste pour qu'il ne puisse pas intéragir sur son propre compte.
            listMembres.Remove(listMembres.FirstOrDefault(x => x.NomUtilisateur == App.MembreCourant.NomUtilisateur));

            //Configure la dataGrid de la searchBox et le predicate pour le filtre.
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(listMembres);
            filterDataGrid.DataGridCollection.Filter = new Predicate<object>(Filter);

            //De listMembres, obtient la liste des administrateurs seulement.
            listAdmins = new ObservableCollection<Membre>(listMembres.Where(m => m.EstAdministrateur).ToList());

            dgAdmin.ItemsSource = listAdmins;
            adminDepart = listAdmins.ToList();

            //Configuration du thread. Met en background pour forcer la terminaison lorsque le logiciel se ferme.
            dbPoolingThread = new Thread(PoolDB);
            dbPoolingThread.IsBackground = true;
            dbPoolingThread.Start();
        }

        /// <summary>
        /// Méthode exécuté continuellement sur un autre thread.
        /// Vérifie si un membre a été modifié dans la base de données
        /// et met à jour les données affichées dans la dataGrid.
        /// </summary>
        private void PoolDB()
        {
            while (true)
            {
                //Récupère le plus récent temps de mise à jour des membres de la base de données
                currentTime = serviceMembre.LastUpdatedTime();
                //Si currentTime est plus récent que previousTime, alors il y a eu une mise à jour.
                if (currentTime > previousTime)
                {
                    //Dois mettre à jour, lance la méthode RefreshDataGrid, met à jour previousTime.
                    Dispatcher.Invoke(RefreshDataGrid);
                    previousTime = currentTime;
                }
                //Fait dormir le Thread pendant un certain temps pour ne pas taxer trop les ressources réseau et processeur.
                Thread.Sleep(App.POOL_TIME);
            }
        }

        /// <summary>
        /// Méthode pour mettre à jour les listes et l'affichage des dataGrids
        /// </summary>
        private void RefreshDataGrid()
        {
            //Déselectionne la ligne de dgAdmin si une est sélectionné (je ne sais trop pourquoi j'ai fais sa).
            dgAdmin.SelectedIndex = -1;
            //Récupère tout les membres de la base de données.
            listMembres = new ObservableCollection<Membre>(serviceMembre.RetrieveAll());
            //Retire le membre actuellement connecté de la liste pour qu'il ne peut pas s'affecter lui-même.
            listMembres.Remove(listMembres.FirstOrDefault(x => x.NomUtilisateur == App.MembreCourant.NomUtilisateur));
            //À partir de listMembres, fait une sous liste avec les administrateurs seulement.
            listAdmins = new ObservableCollection<Membre>(listMembres.Where(m => m.EstAdministrateur).ToList());
            //Configure la dataGrid contenant tout les membres, la liste adminDepart et la dataGrid avec les administrateurs.
            filterDataGrid.DataGridCollection = CollectionViewSource.GetDefaultView(listMembres);
            adminDepart = listAdmins.ToList();
            dgAdmin.ItemsSource = listAdmins;
        }

        /// <summary>
        /// Méthode fournie à la searchBox pour définir le type de l'objet auquel la recherche s'applique et sur quel propriété.
        /// </summary>
        /// <param name="obj">Objet qui sera filtré</param>
        /// <returns>Vrai si un match, faux si non</returns>
        public bool Filter(object obj)
        {
            var data = obj as Membre;
            if (data != null)
            {
                //si obj est un Membre et que FilterString n'est pas vide ou null
                if (!string.IsNullOrEmpty(filterDataGrid.FilterString))
                {
                    //Appel la méthode Filter de SeachBox en envoyant le nom de l'utilisateur comme paramètre pour filtrer.
                    return filterDataGrid.Filter(data.NomUtilisateur);
                }
                //Lorsque FilterString est vide, true pour afficher tout les membres.
                return true;
            }
            //Mauvais type, n'affiche rien.
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Récupère les administrateurs selon les modifications apportés, mais pas encore dans la base de données.
            adminFin = listMembres.Where(x => x.EstAdministrateur == true).ToList();
            //Un peu de mathémathique des ensembles pour récupéré uniquement les membres qui ont été modifiés.
            membreModifie = adminDepart.Union(adminFin).Except(adminDepart.Intersect(adminFin)).ToList();

            //Update dans la base de données de tout les membres qui ont été modifiés.
            foreach (Membre m in membreModifie)
            {
                serviceMembre.Update(m);
            }

            //Si aucun membre a été modifié (utile??)
            if (membreModifie.Count != 0)
                adminDepart = adminFin;
        }


        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            //Change le titre de la mainwindow correctement lorsque la langue d'affichage change.
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreGestionAdmin.Titre;
        }
    }
}
