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
    /// Logique d'interaction pour FenetreGenerateurMenus.xaml
    /// </summary>
    public partial class FenetreGenerateurMenus : UserControl
    {
        private IAlimentService AlimentService { get; set; }
        private IPlatService PlatService { get; set; }
        private IMenuService MenuService { get; set; }
        private ObservableCollection<Plat> ListeDejeuners { get; set; }
        private ObservableCollection<Plat> ListeEntrees { get; set; }
        private ObservableCollection<Plat> ListePlatPrincipaux { get; set; }
        private ObservableCollection<Plat> ListeBreuvages { get; set; }
        private ObservableCollection<Plat> ListeDesserts { get; set; }
        private Menu MenuGenere { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreGenerateurMenus()
        {
            InitializeComponent();

            App.Current.MainWindow.ResizeMode = ResizeMode.CanResize;
            App.Current.MainWindow.MinWidth = 650;
            App.Current.MainWindow.MinHeight = 550;

            AlimentService = ServiceFactory.Instance.GetService<IAlimentService>();
            PlatService = ServiceFactory.Instance.GetService<IPlatService>();
            MenuService = ServiceFactory.Instance.GetService<IMenuService>();

            // Chargement des plats.
            Mouse.OverrideCursor = Cursors.Wait;
            ListeDejeuners = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Déjeuner" }));
            ListeEntrees = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Entrée" }));
            ListePlatPrincipaux = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Plat principal" }));
            ListeBreuvages = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Breuvage" }));
            ListeDesserts = new ObservableCollection<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Déssert" }));
            Mouse.OverrideCursor = null;

            // Header de la fenêtre.
            App.Current.MainWindow.Title = "Nutritia - Génération de menus";

            if (String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
            {
                btnOuvrirMenu.IsEnabled = false;
            }
            else
            {
                AppliquerSpecificites();
            }
        }

        /// <summary>
        /// Méthode permettant de générer dynamiquement les rangées de la grid contenant le menu.
        /// </summary>
        /// <param name="nbRangees">Le nombre de rangées.</param>
        private void GenererRangees(int nbRangees)
        {
            RowDefinition rowDefinition;

            for (int i = 0; i < nbRangees; i++)
            {
                rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(110);

                grdMenus.RowDefinitions.Add(rowDefinition);
            }
        }

        /// <summary>
        /// Méthode permettant d'initialiser la section des menus.
        /// </summary>
        private void InitialiserSectionMenu(int nbPlats)
        {
            svMenus.ScrollToTop();
            // Génération des rangées de la grid.
            grdMenus.RowDefinitions.Clear();
            GenererRangees(nbPlats);
            Grid.SetRowSpan(dgMenus, nbPlats);
            
            // Retrait des séparateurs s'il y a lieu.
            List<Label> separateurs = new List<Label>(grdMenus.Children.OfType<Label>());

            foreach (Label separateurCourant in separateurs)
            {
                grdMenus.Children.Remove(separateurCourant);
            }
        }

        /// <summary>
        /// Méthode permettant d'ajouter un séparateur entre les plats.
        /// </summary>
        /// <param name="contenu">Le contenu du séparateur.</param>
        /// <param name="index">L'index du séparateur dans la Grid.</param>
        private void AjouterSeparateurPlat(string contenu, int index)
        {
            Label lblSeparateur = new Label();
            lblSeparateur.Content = contenu;
            lblSeparateur.Style = FindResource("fontNutritia") as Style;
            lblSeparateur.FontSize = 16;
            lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(lblSeparateur, index);
            grdMenus.Children.Add(lblSeparateur);
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton "Ouvrir un menu".
        /// Permet d'ouvrir un menu précédemment généré par un membre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOuvrirMenu_Click(object sender, RoutedEventArgs e)
        {
            FenetreOuvrirMenu popupOuvertureMenu = new FenetreOuvrirMenu();
            popupOuvertureMenu.ShowDialog();

            if(popupOuvertureMenu.DialogResult == true)
            {
                MenuGenere = popupOuvertureMenu.MenuSelectionne;
                dgMenus.ItemsSource = MenuGenere.ListePlats;
                btnSauvegarder.IsEnabled = true;
                btnListeEpicerie.IsEnabled = true;
                imgInfos.Visibility = Visibility.Hidden;
                gbMenus.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Méthode permettant de conserver ou non les plats qui correspondent ou non au profil du membre connecté.
        /// </summary>
        private void AppliquerSpecificites()
        {
            List<Plat> listePlatsAEnlever = new List<Plat>();

            foreach(Plat platCourant in ListeDejeuners)
            {
                foreach (RestrictionAlimentaire restrictionCourante in App.MembreCourant.ListeRestrictions)
                {
                    if (restrictionCourante.Nom == "Lactose" || restrictionCourante.Nom == "Arachides et noix"
                       || restrictionCourante.Nom == "Gluten" || restrictionCourante.Nom == "Poissons et fruits de mers")
                    {
                        foreach (Aliment alimentCourant in platCourant.ListeIngredients)
                        {
                            if (restrictionCourante.Nom == "Lactose" && alimentCourant.Categorie == "Produits laitiers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Arachides et noix" && alimentCourant.Categorie == "Arachides et noix")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Gluten" && (alimentCourant.Categorie == "Céréales" || alimentCourant.Categorie == "Pâtes"))
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Poissons et fruits de mers" && alimentCourant.Categorie == "Poissons et fruits de mers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }
                        }
                    }
                }
            }

            foreach (Plat platCourant in ListeBreuvages)
            {
                foreach (RestrictionAlimentaire restrictionCourante in App.MembreCourant.ListeRestrictions)
                {
                    if (restrictionCourante.Nom == "Lactose" || restrictionCourante.Nom == "Arachides et noix"
                       || restrictionCourante.Nom == "Gluten" || restrictionCourante.Nom == "Poissons et fruits de mers")
                    {
                        foreach (Aliment alimentCourant in platCourant.ListeIngredients)
                        {
                            if (restrictionCourante.Nom == "Lactose" && alimentCourant.Categorie == "Produits laitiers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Arachides et noix" && alimentCourant.Categorie == "Arachides et noix")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Gluten" && (alimentCourant.Categorie == "Céréales" || alimentCourant.Categorie == "Pâtes"))
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Poissons et fruits de mers" && alimentCourant.Categorie == "Poissons et fruits de mers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }
                        }
                    }
                }
            }

            foreach (Plat platCourant in ListeEntrees)
            {
                foreach (RestrictionAlimentaire restrictionCourante in App.MembreCourant.ListeRestrictions)
                {
                    if (restrictionCourante.Nom == "Lactose" || restrictionCourante.Nom == "Arachides et noix"
                       || restrictionCourante.Nom == "Gluten" || restrictionCourante.Nom == "Poissons et fruits de mers")
                    {
                        foreach (Aliment alimentCourant in platCourant.ListeIngredients)
                        {
                            if (restrictionCourante.Nom == "Lactose" && alimentCourant.Categorie == "Produits laitiers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Arachides et noix" && alimentCourant.Categorie == "Arachides et noix")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Gluten" && (alimentCourant.Categorie == "Céréales" || alimentCourant.Categorie == "Pâtes"))
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Poissons et fruits de mers" && alimentCourant.Categorie == "Poissons et fruits de mers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }
                        }
                    }
                }
            }

            foreach (Plat platCourant in ListePlatPrincipaux)
            {
                foreach (RestrictionAlimentaire restrictionCourante in App.MembreCourant.ListeRestrictions)
                {
                    if (restrictionCourante.Nom == "Lactose" || restrictionCourante.Nom == "Arachides et noix"
                       || restrictionCourante.Nom == "Gluten" || restrictionCourante.Nom == "Poissons et fruits de mers")
                    {
                        foreach (Aliment alimentCourant in platCourant.ListeIngredients)
                        {
                            if (restrictionCourante.Nom == "Lactose" && alimentCourant.Categorie == "Produits laitiers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Arachides et noix" && alimentCourant.Categorie == "Arachides et noix")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Gluten" && (alimentCourant.Categorie == "Céréales" || alimentCourant.Categorie == "Pâtes"))
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Poissons et fruits de mers" && alimentCourant.Categorie == "Poissons et fruits de mers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }
                        }
                    }
                }
            }

            foreach (Plat platCourant in ListeDesserts)
            {
                foreach (RestrictionAlimentaire restrictionCourante in App.MembreCourant.ListeRestrictions)
                {
                    if (restrictionCourante.Nom == "Lactose" || restrictionCourante.Nom == "Arachides et noix"
                       || restrictionCourante.Nom == "Gluten" || restrictionCourante.Nom == "Poissons et fruits de mers")
                    {
                        foreach (Aliment alimentCourant in platCourant.ListeIngredients)
                        {
                            if (restrictionCourante.Nom == "Lactose" && alimentCourant.Categorie == "Produits laitiers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Arachides et noix" && alimentCourant.Categorie == "Arachides et noix")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Gluten" && (alimentCourant.Categorie == "Céréales" || alimentCourant.Categorie == "Pâtes"))
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }

                            if (restrictionCourante.Nom == "Poissons et fruits de mers" && alimentCourant.Categorie == "Poissons et fruits de mers")
                            {
                                listePlatsAEnlever.Add(platCourant);
                            }
                        }
                    }
                }
            }

            // Retirer les plats.
            foreach(Plat platCourant in listePlatsAEnlever)
            {
                if(platCourant.TypePlat == "Déjeuner")
                {
                    ListeDejeuners.Remove(platCourant);
                }
                else if (platCourant.TypePlat == "Breuvage")
                {
                    ListeBreuvages.Remove(platCourant);
                }
                else if (platCourant.TypePlat == "Entrée")
                {
                    ListeEntrees.Remove(platCourant);
                }
                else if (platCourant.TypePlat == "Plat principal")
                {
                    ListePlatPrincipaux.Remove(platCourant);
                }
                else if (platCourant.TypePlat == "Déssert")
                {
                    ListeDesserts.Remove(platCourant);
                }
            }
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Générer.
        /// Permet de générer un menu en fonction des informations préalables.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerer_Click(object sender, RoutedEventArgs e)
        {
            MenuGenere = new Menu();
            int nbJours = 0;
            int nbPlats = 4;
            Random rand = new Random();

            if (rbDejeuner.IsChecked != null && (bool)rbDejeuner.IsChecked) { nbPlats = 2; }
            if (rbDinerSouper.IsChecked != null && (bool)rbDinerSouper.IsChecked) { nbPlats = 4; }
            if (rbMenuJournalier.IsChecked != null && (bool)rbMenuJournalier.IsChecked) { nbJours = 1; nbPlats = 10; }
            if (rbMenuHebdomadaire.IsChecked != null && (bool)rbMenuHebdomadaire.IsChecked) { nbJours = 7; nbPlats = 70; }
            
            MenuGenere.NbPersonnes = Convert.ToInt32(((ComboBoxItem)cboNbPersonnes.SelectedItem).Content);

            InitialiserSectionMenu(nbPlats);

            // Il s'agit d'un déjeuner.
            if(nbPlats == 2)
            {
                MenuGenere.ListePlats.Add(ListeDejeuners[rand.Next(0, ListeDejeuners.Count)]);
                AjouterSeparateurPlat("Déjeuner", 0);

                MenuGenere.ListePlats.Add(ListeBreuvages[rand.Next(0, ListeBreuvages.Count)]);
                AjouterSeparateurPlat("Breuvage", 1);
            }

            // Il s'agit d'un diner/souper.
            if(nbPlats == 4)
            {
                MenuGenere.ListePlats.Add(ListeEntrees[rand.Next(0, ListeEntrees.Count)]);
                AjouterSeparateurPlat("Entrée", 0);

                MenuGenere.ListePlats.Add(ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)]);
                AjouterSeparateurPlat("Plat principal", 1);

                MenuGenere.ListePlats.Add(ListeBreuvages[rand.Next(0, ListeBreuvages.Count)]);
                AjouterSeparateurPlat("Breuvage", 2);

                MenuGenere.ListePlats.Add(ListeDesserts[rand.Next(0, ListeDesserts.Count)]);
                AjouterSeparateurPlat("Déssert", 3);
            }

            // Il s'agit d'une journée ou encore d'une semaine.
            if(nbJours > 0)
            {
                for (int i = 0; i < nbJours; i++)
                {
                    MenuGenere.ListePlats.Add(ListeDejeuners[rand.Next(0, ListeDejeuners.Count)]);
                    AjouterSeparateurPlat("Jour " + (i + 1) + " (Déjeuner)", i * (nbPlats / nbJours));

                    MenuGenere.ListePlats.Add(ListeBreuvages[rand.Next(0, ListeBreuvages.Count)]);
                    AjouterSeparateurPlat("Jour " + (i + 1) + " (Breuvage)", i * (nbPlats / nbJours) + 1);

                    for (int j = 0; j < 2; j++)
                    {
                        MenuGenere.ListePlats.Add(ListeEntrees[rand.Next(0, ListeEntrees.Count)]);
                        AjouterSeparateurPlat("Jour " + (i + 1) + " (Entrée)", (i * (nbPlats / nbJours) + 2 + (j * 4)));

                        MenuGenere.ListePlats.Add(ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)]);
                        AjouterSeparateurPlat("Jour " + (i + 1) + " (Plat principal)", (i * (nbPlats / nbJours) + 3 + (j * 4)));

                        MenuGenere.ListePlats.Add(ListeBreuvages[rand.Next(0, ListeBreuvages.Count)]);
                        AjouterSeparateurPlat("Jour " + (i + 1) + " (Breuvage)", (i * (nbPlats / nbJours) + 4 + (j * 4)));

                        MenuGenere.ListePlats.Add(ListeDesserts[rand.Next(0, ListeDesserts.Count)]);
                        AjouterSeparateurPlat("Jour " + (i + 1) + " (Déssert)", (i * (nbPlats / nbJours) + 5 + (j * 4)));
                    }
                }
            }

            dgMenus.ItemsSource = MenuGenere.ListePlats;
            btnSauvegarder.IsEnabled = true;
            btnListeEpicerie.IsEnabled = true;
            imgInfos.Visibility = Visibility.Hidden;
            gbMenus.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Sauvegarder.
        /// Permet de sauvegarder un menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            FenetreSauvegarderMenu popupSauvegarde = new FenetreSauvegarderMenu();
            popupSauvegarde.ShowDialog();

            if(popupSauvegarde.DialogResult == true)
            {
                MenuGenere.Nom = popupSauvegarde.txtNom.Text;
                MenuService.Insert(MenuGenere);
            }
        }

        /// <summary>
        /// Événement lancé lorsque la roulette de la souris est utilisée dans le "scrollviewer" contenant le menu.
        /// Explicitement, cet événement permet de gérer le "scroll" avec la roulette correctement sur toute la surface du "scrollviewer".
        /// Si on ne le gère pas, il est seulement possible de "scroller" lorsque le pointeur de la souris est situé sur la "scrollbar".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void svMenus_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            svMenus.ScrollToVerticalOffset(svMenus.VerticalOffset - e.Delta);
        }

        /// <summary>
        /// Événement lancé sur un clique d'un bouton Ingrédients.
        /// Permet d'afficher les ingrédients du plat lié à ce bouton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIngredients_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)dgMenus.SelectedItem;

            FenetreIngredients fenetreIngredients = new FenetreIngredients(platSelectionne, MenuGenere.NbPersonnes);
            fenetreIngredients.ShowDialog();
        }

        /// <summary>
        /// Événement lancé sur un clique d'un bouton regénérer.
        /// Permet de régénérer le plat lié à ce bouton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegenerer_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)dgMenus.SelectedItem;
            Plat platRegenere;
            Random rand = new Random();

            switch(platSelectionne.TypePlat)
            {
                case "Déjeuner":
                    if(ListeDejeuners.Count > 1)
                    {
                        platRegenere = ListeDejeuners[rand.Next(0, ListeDejeuners.Count)];
                        while (platRegenere == platSelectionne)
                        {
                            platRegenere = ListeDejeuners[rand.Next(0, ListeDejeuners.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Entrée":
                    if(ListeEntrees.Count > 1)
                    {
                        platRegenere = ListeEntrees[rand.Next(0, ListeEntrees.Count)];
                        while (platRegenere == platSelectionne)
                        {
                            platRegenere = ListeEntrees[rand.Next(0, ListeEntrees.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Plat principal":
                    if(ListePlatPrincipaux.Count > 1)
                    {
                        platRegenere = ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)];
                        while (platRegenere == platSelectionne)
                        {
                            platRegenere = ListePlatPrincipaux[rand.Next(0, ListePlatPrincipaux.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Breuvage":
                    if(ListeBreuvages.Count > 1)
                    {
                        platRegenere = ListeBreuvages[rand.Next(0, ListeBreuvages.Count)];
                        while (platRegenere == platSelectionne)
                        {
                            platRegenere = ListeBreuvages[rand.Next(0, ListeBreuvages.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Déssert":
                    if(ListeDesserts.Count > 1)
                    {
                        platRegenere = ListeDesserts[rand.Next(0, ListeDesserts.Count)];
                        while (platRegenere == platSelectionne)
                        {
                            platRegenere = ListeDesserts[rand.Next(0, ListeDesserts.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
            }
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Liste d'épicerie.
        /// Permet de générer la liste d'épicerie du menu précédemment généré.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListeEpicerie_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue<FenetreListeEpicerie>(new FenetreListeEpicerie(MenuGenere));
        }
    }
}
