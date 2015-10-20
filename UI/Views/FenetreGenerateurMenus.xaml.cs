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
    /// Logique d'interaction pour GenerateurMenus.xaml
    /// </summary>
    public partial class GenerateurMenus : UserControl
    {
        private int NbJours { get; set; }
        private int NbPlats { get; set; }
        private int NbPersonnes { get; set; }
        private IPlatService PlatService { get; set; }
        private ObservableCollection<Plat> ListeDejeuners { get; set; }
        private ObservableCollection<Plat> ListeEntrees { get; set; }
        private ObservableCollection<Plat> ListePlatPrincipaux { get; set; }
        private ObservableCollection<Plat> ListeBreuvages { get; set; }
        private ObservableCollection<Plat> ListeDesserts { get; set; }
        private Menu MenuGenere { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public GenerateurMenus()
        {
            InitializeComponent();

            NbPlats = 4;
            btnOuvrirMenu.IsEnabled = false;
            PlatService = ServiceFactory.Instance.GetService<IPlatService>();
        }
        
        /// <summary>
        /// Événement lancé sur un clique du bouton Générer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerer_Click(object sender, RoutedEventArgs e)
        {
            MenuGenere = new Menu();
            Random rand = new Random();

            if (rbMenuUnique.IsChecked != null && (bool)rbMenuUnique.IsChecked) { NbJours = 0; NbPlats = 4; }
            if (rbMenuJournalier.IsChecked != null && (bool)rbMenuJournalier.IsChecked) { NbJours = 1; NbPlats = 10; }
            if (rbMenuHebdomadaire.IsChecked != null && (bool)rbMenuHebdomadaire.IsChecked) { NbJours = 7; NbPlats = 70; }
            
            NbPersonnes = Convert.ToInt32(((ComboBoxItem)cboNbPersonnes.SelectedItem).Content);

            grdMenus.RowDefinitions.Clear();
            GenererRangees(NbPlats);
            Grid.SetRowSpan(dgMenus, NbPlats);

            Cursor = Cursors.Wait;

            ListeDejeuners = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Déjeuner" }));
            ListeEntrees = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Entrée" }));
            ListePlatPrincipaux = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Plat principal" }));
            ListeBreuvages = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Breuvage" }));
            ListeDesserts = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Déssert" }));

            if(NbJours == 0)
            {
                Label lblSeparateur = new Label();
                
                MenuGenere.ListePlats.Add(ListeEntrees[rand.Next(0, ListeEntrees.Count)]);
                lblSeparateur.Content = "Entrée";
                lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                grdMenus.Children.Add(lblSeparateur);

                MenuGenere.ListePlats.Add(ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)]);
                lblSeparateur = new Label();
                lblSeparateur.Content = "Plat principal";
                lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(lblSeparateur, 1);
                grdMenus.Children.Add(lblSeparateur);

                MenuGenere.ListePlats.Add(ListeBreuvages[rand.Next(0, ListeBreuvages.Count)]);
                lblSeparateur = new Label();
                lblSeparateur.Content = "Breuvage";
                lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(lblSeparateur, 2);
                grdMenus.Children.Add(lblSeparateur);

                MenuGenere.ListePlats.Add(ListeDesserts[rand.Next(0, ListeDesserts.Count)]);
                lblSeparateur = new Label();
                lblSeparateur.Content = "Déssert";
                lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(lblSeparateur, 3);
                grdMenus.Children.Add(lblSeparateur);
            }
            else
            {
                for (int i = 0; i < NbJours; i++)
                {
                    Label lblSeparateur = new Label();

                    MenuGenere.ListePlats.Add(ListeDejeuners[rand.Next(0, ListeDejeuners.Count)]);
                    lblSeparateur.Content = "Jour " + (i + 1) + " (Déjeuner)";
                    lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(lblSeparateur, i * (NbPlats / NbJours));
                    grdMenus.Children.Add(lblSeparateur);

                    MenuGenere.ListePlats.Add(ListeBreuvages[rand.Next(0, ListeBreuvages.Count)]);
                    lblSeparateur = new Label();
                    lblSeparateur.Content = "Jour " + (i + 1) + " (Breuvage)";
                    lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(lblSeparateur, i * (NbPlats / NbJours) + 1);
                    grdMenus.Children.Add(lblSeparateur);

                    for (int j = 0; j < 2; j++)
                    {
                        lblSeparateur = new Label();

                        MenuGenere.ListePlats.Add(ListeEntrees[rand.Next(0, ListeEntrees.Count)]);
                        lblSeparateur.Content = "Jour " + (i + 1) + " (Entrée)";
                        lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetRow(lblSeparateur, (i * (NbPlats / NbJours) + 2 + (j * 4)));
                        grdMenus.Children.Add(lblSeparateur);

                        MenuGenere.ListePlats.Add(ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)]);
                        lblSeparateur = new Label();
                        lblSeparateur.Content = "Jour " + (i + 1) + " (Plat principal)";
                        lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetRow(lblSeparateur, (i * (NbPlats / NbJours) + 3 + (j * 4)));
                        grdMenus.Children.Add(lblSeparateur);

                        MenuGenere.ListePlats.Add(ListeBreuvages[rand.Next(0, ListeBreuvages.Count)]);
                        lblSeparateur = new Label();
                        lblSeparateur.Content = "Jour " + (i + 1) + " (Breuvage)";
                        lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetRow(lblSeparateur, (i * (NbPlats / NbJours) + 4 + (j * 4)));
                        grdMenus.Children.Add(lblSeparateur);

                        MenuGenere.ListePlats.Add(ListeDesserts[rand.Next(0, ListeDesserts.Count)]);
                        lblSeparateur = new Label();
                        lblSeparateur.Content = "Jour " + (i + 1) + " (Déssert)";
                        lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetRow(lblSeparateur, (i * (NbPlats / NbJours) + 5 + (j * 4)));
                        grdMenus.Children.Add(lblSeparateur);
                    }
                }
            }

            Cursor = Cursors.Arrow;

            dgMenus.ItemsSource = MenuGenere.ListePlats;
        }

        /// <summary>
        /// Événement lancé sur un clique d'un bouton Ingrédients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIngredients_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)dgMenus.SelectedItem;

            FenetreIngredients fenetreIngredients = new FenetreIngredients(platSelectionne);
            fenetreIngredients.ShowDialog();
        }

        /// <summary>
        /// Événement lancé sur un clique d'un bouton regénérer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegenerer_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)dgMenus.SelectedItem;
            Plat platRegenere;
            Random rand = new Random();

            int index = MenuGenere.ListePlats.IndexOf(platSelectionne);

            switch(platSelectionne.TypePlat)
            {
                case "Déjeuner":
                    platRegenere = ListeDejeuners[rand.Next(0, ListeDejeuners.Count)];
                    while (platRegenere == platSelectionne)
                    {
                        platRegenere = ListeDejeuners[rand.Next(0, ListeDejeuners.Count)];
                    }
                    MenuGenere.ListePlats[index] = platRegenere;
                break;
                case "Entrée":
                    platRegenere = ListeEntrees[rand.Next(0, ListeEntrees.Count)];
                    while (platRegenere == platSelectionne)
                    {
                        platRegenere = ListeEntrees[rand.Next(0, ListeEntrees.Count)];
                    }
                    MenuGenere.ListePlats[index] = platRegenere;
                break;
                case "Plat principal":
                    platRegenere = ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)];
                    while (platRegenere == platSelectionne)
                    {
                        platRegenere = ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)];
                    }
                    MenuGenere.ListePlats[index] = platRegenere;
                break;
                case "Breuvage":
                    platRegenere = ListeBreuvages[rand.Next(0, ListeBreuvages.Count)];
                    while (platRegenere == platSelectionne)
                    {
                        platRegenere = ListeBreuvages[rand.Next(0, ListeBreuvages.Count)];
                    }
                    MenuGenere.ListePlats[index] = platRegenere;
                break;
                case "Déssert":
                    platRegenere = ListeDesserts[rand.Next(0, ListeDesserts.Count)];
                    while (platRegenere == platSelectionne)
                    {
                        platRegenere = ListeDesserts[rand.Next(0, ListeDesserts.Count)];
                    }
                    MenuGenere.ListePlats[index] = platRegenere;
                break;
            }
        }

        /// <summary>
        /// Méthode permettant de générer les rangées de la grid contenant le menu.
        /// </summary>
        /// <param name="nbRangees">Le nombre de rangées.</param>
        private void GenererRangees(int nbRangees)
        {
            RowDefinition rowDefinition;

            for (int i = 0; i < nbRangees; i++)
            {
                rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(150);

                grdMenus.RowDefinitions.Add(rowDefinition);
            }
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Liste d'épicerie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListeEpicerie_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue<ListeEpicerie>(new ListeEpicerie(MenuGenere));
        }
    }
}
