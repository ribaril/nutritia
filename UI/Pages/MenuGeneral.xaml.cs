using Infralution.Localization.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

namespace Nutritia.UI.Pages
{
    /// <summary>
    /// Interaction logic for MenuGeneral.xaml
    /// Page avec les options générals du logiciel
    /// </summary>
    public partial class MenuGeneral : Page
    {
        #region NestedClassLangueUI
        /// <summary>
        /// Wrapper avec la classe métier Langue, mais adapté pour l'affichage dans une dataGrid et la gestion de paramètres
        /// </summary>
        public class LangueUI : INotifyPropertyChanged
        {
            //Évènement lancé lorsqu'une propriété change
            public event PropertyChangedEventHandler PropertyChanged;

            private Langue langueSys;
            //Défini si la langue est active ou non (si c'est la langue d'affichage).
            private bool actif;
            //Techniquement, pourrait utiliser le nom de Langue, mais pas la structure rend plus difficile le changement de nom selon la langue d'affichage.
            private string nom;


            /// <summary>
            /// Nom de la LangueUI
            /// </summary>
            public string Nom
            {
                get
                { return nom; }
                set
                {
                    nom = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Nom"));
                }
            }

            /// <summary>
            /// Défini si la LangueUI est la langue d'affichage active.
            /// </summary>
            public bool Actif
            {
                get { return actif; }
                set
                {
                    actif = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Actif"));
                }
            }

            /// <summary>
            /// Langue du système supporté
            /// </summary>
            public Langue LangueSys
            {
                get { return langueSys; }
                set
                {
                    langueSys = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LangueSys"));
                }
            }

            /// <summary>
            /// Constructeur paramétrés
            /// </summary>
            /// <param name="nom">Nom affiché</param>
            /// <param name="langue">Langue à laquel LangueUI couvre</param>
            /// <param name="actif">Défini si LangueUI est actuellement utilisé comme langue d'affichage.</param>
            public LangueUI(string nom, Langue langue, bool actif = false)
            {
                this.Nom = nom;
                this.langueSys = langue;
                this.Actif = actif;
            }

            /// <summary>
            /// Lance l'événement PropertyChanged
            /// </summary>
            /// <param name="e">La propriété changé</param>
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, e);
                }
            }

            public override string ToString()
            {
                return Nom;
            }
        }
        #endregion

        //ObservableCollection pour l'affichage dans une dataGrid
        private ObservableCollection<LangueUI> mapLangue;

        //Langue française, utilise le Nom défini dans nos ressources pour supporté le changement de Nom dynamique selon la langue d'affichage et 
        // notre classe singleton Langue FrancaisCanada.
        LangueUI francais = new LangueUI(Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueFrancais, Langue.FrancaisCanada);

        //Langue anglaise, utilise le Nom défini dans nos ressources pour supporté le changement de Nom dynamique selon la langue d'affichage et 
        // notre classe singleton Langue AnglaisUSA.
        LangueUI anglais = new LangueUI(Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueAnglais, Langue.AnglaisUSA);

        public MenuGeneral()
        {
            //Delegate pour exécuter du code personnalisé lors du changement de langue de l'UI.
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            mapLangue = new ObservableCollection<LangueUI>();
            //Le tag de langue IETF (ie: fr-CA) utilisé directement pour changer la langue d'affichage.
            //Récupéré selon la Langue active.
            string codeLangueActif;

            //Si l'utilisateur est connecté, utilise la langue sauvegarder sous son profil.
            if (App.MembreCourant.IdMembre != null)
            {
                codeLangueActif = App.MembreCourant.LangueMembre.IETF;
            }
            else
            {
                //Sinon, utilise la valeur par défault tel que défini sous App.
                codeLangueActif = App.LangueInstance.IETF;
            }


            mapLangue.Add(francais);
            mapLangue.Add(anglais);
            //Dans le dictionnaire, trouve l'élément qui a le tag IETF équivalent à selui enregistré sous codeLangueActif et le met à actif.
            mapLangue.FirstOrDefault(l => l.LangueSys.IETF == codeLangueActif).Actif = true;

            InitializeComponent();
            //Configure la source de notre dataGrid.
            dgLangues.ItemsSource = mapLangue;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Dans la dictionnaire, trouve le premier objet LangueUI qui est en mode actif (il n'y a qu'un seul plus que dans la dataGrid c'est défini comme radioButton).
            LangueUI langue = mapLangue.FirstOrDefault(l => l.Actif == true);

            //Si l'utilisateur est connecté à son compte
            if (App.MembreCourant.IdMembre != null)
            {
                //Enregistre la nouvelle langue sous son compte utilisateur et fait la mise à jour dans la base de données.
                App.MembreCourant.LangueMembre = langue.LangueSys;
                new MySqlMembreService().Update(App.MembreCourant);
            }
            else
            {
                //Si l'utilisateur n'est pas connecter, ne fait que changer l'attribut LangueInstance de App.
                App.LangueInstance = langue.LangueSys;
            }
            //Change la langue d'affichage selon le tag IETF.
            CultureManager.UICulture = new CultureInfo(langue.LangueSys.IETF);
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            //Lorsque la langue d'affichage est changé, remet à jour la valeur des propriétés Nom des objets LangueUI affichés dans la dataGrid.
            francais.Nom = Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueFrancais;
            anglais.Nom = Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueAnglais;
        }
    }
}
