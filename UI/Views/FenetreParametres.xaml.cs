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
using System.Windows.Shapes;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for FenetreParametres.xaml
    /// </summary>
    public partial class FenetreParametres : Window
    {
        private ObservableCollection<Langue> mapLangue;


        public class Langue
        {
            public string Nom { get; set; }
            public string CodeISO { get; set; }
            public bool Actif { get; set; }

            public Langue(string nom, string code, bool actif = false)
            {
                this.Nom = nom;
                this.CodeISO = code;
                this.Actif = actif;
            }

            public override string ToString()
            {
                return Nom.ToString();
            }
        }

        public FenetreParametres()
        {
            mapLangue = new ObservableCollection<Langue>();
            Langue francais = new Langue("Français", "fr", true);
            Langue anglais = new Langue("Anglais", "fr");

            mapLangue.Add(francais);
            mapLangue.Add(anglais);

            InitializeComponent();

            dgLangues.ItemsSource = mapLangue;
        }
    }
}
