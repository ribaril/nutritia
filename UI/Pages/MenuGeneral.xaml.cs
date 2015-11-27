﻿using System;
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

namespace Nutritia.UI.Pages
{
    /// <summary>
    /// Interaction logic for MenuGeneral.xaml
    /// </summary>
    public partial class MenuGeneral : Page
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

        public MenuGeneral()
        {
            mapLangue = new ObservableCollection<Langue>();
            Langue francais = new Langue("Français", "fr");
            Langue anglais = new Langue("Anglais", "en");
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
            MessageBox.Show("Veuillez redémarrer l'application\n pour que le changement s'applique.");
        }
    }
}