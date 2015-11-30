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
        public class Langue : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string nom;
            private string codeISO;
            private bool actif;

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

            public string CodeISO
            {
                get { return codeISO; }
                set
                {
                    codeISO = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CodeISO"));
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

            public Langue(string nom, string code, bool actif = false)
            {
                this.Nom = nom;
                this.CodeISO = code;
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
                return Nom.ToString();
            }
        }
        #endregion

        private ObservableCollection<Langue> mapLangue;

        Langue francais = new Langue(Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueFrancais, "fr");
        Langue anglais = new Langue(Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueAnglais, "en");

        public MenuGeneral()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            mapLangue = new ObservableCollection<Langue>();
            string codeLangueActif = Properties.Settings.Default.Langue;

            mapLangue.Add(francais);
            mapLangue.Add(anglais);
            mapLangue.FirstOrDefault(l => l.CodeISO == codeLangueActif).Actif = true;

            InitializeComponent();

            dgLangues.ItemsSource = mapLangue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Langue langue = mapLangue.FirstOrDefault(l => l.Actif == true);
            Properties.Settings.Default.Langue = langue.CodeISO;
            Properties.Settings.Default.Save();

            CultureManager.UICulture = new CultureInfo(Properties.Settings.Default.Langue);
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            francais.Nom = Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueFrancais;
            anglais.Nom = Nutritia.UI.Ressources.Localisation.Pages.MenuGeneral.LangueAnglais;
        }
    }
}
