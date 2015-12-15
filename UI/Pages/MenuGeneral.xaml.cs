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
    /// </summary>
    public partial class MenuGeneral : Page
    {
        #region NestedClassLangue
        //Nested class Langue
        public class LangueUI : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private Langue langueSys;
            private bool actif;
            private string nom;

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


            public bool Actif
            {
                get { return actif; }
                set
                {
                    actif = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Actif"));
                }
            }

            public Langue LangueSys
            {
                get { return langueSys; }
                set
                {
                    langueSys = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LangueSys"));
                }
            }

            public LangueUI(string nom, Langue langue, bool actif = false)
            {
                this.Nom = nom;
                this.langueSys = langue;
                this.Actif = actif;
            }


            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, e);
                }
            }

            public override string ToString()
            {
                return langueSys.Nom;
            }
        }
        #endregion

        private ObservableCollection<LangueUI> mapLangue;

        LangueUI francais = new LangueUI(Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueFrancais, Langue.FrancaisCanada);
        LangueUI anglais = new LangueUI(Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueAnglais, Langue.AnglaisUSA);

        public MenuGeneral()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            mapLangue = new ObservableCollection<LangueUI>();
            string codeLangueActif;

            if (App.MembreCourant.IdMembre != null)
            {
                codeLangueActif = App.MembreCourant.LangueMembre.IETF;
            }
            else
            {
                codeLangueActif = App.LangueInstance.IETF;
            }


            mapLangue.Add(francais);
            mapLangue.Add(anglais);
            mapLangue.FirstOrDefault(l => l.LangueSys.IETF == codeLangueActif).Actif = true;

            InitializeComponent();

            dgLangues.ItemsSource = mapLangue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LangueUI langue = mapLangue.FirstOrDefault(l => l.Actif == true);

            if (App.MembreCourant.IdMembre != null)
            {
                App.MembreCourant.LangueMembre = langue.LangueSys;
                new MySqlMembreService().Update(App.MembreCourant);
            }
            else
            {
                App.LangueInstance = langue.LangueSys;
            }
            CultureManager.UICulture = new CultureInfo(langue.LangueSys.IETF);
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            francais.Nom = Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueFrancais;
            anglais.Nom = Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueAnglais;
        }
    }
}
