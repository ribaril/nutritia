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
        private List<Plat> ListeDejeuners { get; set; }
        private List<Plat> ListeEntrees { get; set; }
        private List<Plat> ListePlatPrincipaux { get; set; }
        private List<Plat> ListeBreuvages { get; set; }
        private List<Plat> ListeDesserts { get; set; }
        private List<Plat> ListePlatsRetires { get; set; }
        private Menu MenuGenere { get; set; }
        private int NbColonnes { get; set; }
        private Random Rand { get; set; }
        private bool EstNouveauMenu { get; set; }
        private const int NB_PLATS_DEJEUNER = 2;
        private const int NB_PLATS_DINER_SOUPER = 4;
        private const int NB_PLATS_JOURNEE = 10;
        private const int NB_PLATS_SEMAINE = 70;
        private const int NB_COLONNES_AVEC_IMAGES = 4;
        private const int NB_COLONNES_SANS_IMAGES = 3;

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreGenerateurMenus()
        {
            InitializeComponent();

            Rand = new Random();

            App.Current.MainWindow.ResizeMode = ResizeMode.CanResize;

            AlimentService = ServiceFactory.Instance.GetService<IAlimentService>();
            PlatService = ServiceFactory.Instance.GetService<IPlatService>();
            MenuService = ServiceFactory.Instance.GetService<IMenuService>();

            // Chargement des plats.
            Mouse.OverrideCursor = Cursors.Wait;
            ListeDejeuners = new List<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Déjeuner" }));
            ListeEntrees = new List<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Entrée" }));
            ListePlatPrincipaux = new List<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Plat principal" }));
            ListeBreuvages = new List<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Breuvage" }));
            ListeDesserts = new List<Plat>(PlatService.RetrieveSome(new RetrievePlatArgs { Categorie = "Déssert" }));
            ListePlatsRetires = new List<Plat>();
            Mouse.OverrideCursor = null;

            // Header de la fenêtre.
            App.Current.MainWindow.Title = "Nutritia - Génération de menus";

            if (!String.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
            {
                btnSuiviPlatsNonAdmissibles.IsEnabled = true;
                btnOuvrirMenu.IsEnabled = true;
                RestreindrePossibilites();
            }

            EstNouveauMenu = true;
            NbColonnes = NB_COLONNES_AVEC_IMAGES;
        }

        /// <summary>
        /// Méthode permettant de conserver ou non les plats qui correspondent ou non au profil du membre connecté.
        /// </summary>
        private void RestreindrePossibilites()
        {
            /************************** Allergie au lactose **************************/
            if (App.MembreCourant.ListeRestrictions.Contains(new RestrictionAlimentaire { Nom = "Lactose" }))
            {
                ListeDejeuners.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListeEntrees.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListePlatPrincipaux.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListeBreuvages.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListeDesserts.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
            }

            /************************** Allergie aux arachides **************************/
            if (App.MembreCourant.ListeRestrictions.Contains(new RestrictionAlimentaire { Nom = "Arachides et noix" }))
            {
                ListeDejeuners.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Arachides et noix"));
                ListeEntrees.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Arachides et noix"));
                ListePlatPrincipaux.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Arachides et noix"));
                ListeBreuvages.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Arachides et noix"));
                ListeDesserts.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Arachides et noix"));
            }

            /************************** Allergie au gluten **************************/
            if (App.MembreCourant.ListeRestrictions.Contains(new RestrictionAlimentaire { Nom = "Gluten" }))
            {
                ListeDejeuners.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Céréales") 
                                             || plat.ObtenirCategoriesIngredients().Contains("Pâtes"));
                ListeEntrees.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Céréales")
                                             || plat.ObtenirCategoriesIngredients().Contains("Pâtes"));
                ListePlatPrincipaux.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Céréales")
                                             || plat.ObtenirCategoriesIngredients().Contains("Pâtes"));
                ListeBreuvages.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Céréales")
                                             || plat.ObtenirCategoriesIngredients().Contains("Pâtes"));
                ListeDesserts.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Céréales")
                                             || plat.ObtenirCategoriesIngredients().Contains("Pâtes"));
            }

            /************************** Allergie aux poissons et fruits de mer **************************/
            if (App.MembreCourant.ListeRestrictions.Contains(new RestrictionAlimentaire { Nom = "Poissons et fruits de mers" }))
            {
                ListeDejeuners.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListeEntrees.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListePlatPrincipaux.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListeBreuvages.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListeDesserts.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
            }

            /************************** Végétarien **************************/
            if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Végétarien" }))
            {
                ListeDejeuners.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListeEntrees.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListePlatPrincipaux.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListeBreuvages.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
                ListeDesserts.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"));
            }

            /************************** Végétalien **************************/
            if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Végétalien" }))
            {
                ListeDejeuners.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers")
                                             || plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListeEntrees.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers")
                                             || plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListePlatPrincipaux.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers")
                                             || plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListeBreuvages.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers")
                                             || plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
                ListeDesserts.RemoveAll(plat => plat.ObtenirCategoriesIngredients().Contains("Viandes et substituts")
                                             || plat.ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers")
                                             || plat.ObtenirCategoriesIngredients().Contains("Produits laitiers"));
            }

            /************************** Diabétique **************************/
            if (App.MembreCourant.ListeRestrictions.Contains(new RestrictionAlimentaire { Nom = "Diabète" })
                || App.MembreCourant.ListeObjectifs.Contains(new Objectif { Nom = "Contrôle glycémique"}))
            {
                double maxGlucides;
                Plat platPlusGlucides;

                for (int i = 0; i < ListeDejeuners.Count / 2; i++)
                {
                    maxGlucides = ListeDejeuners.Max(plat => plat.CalculerValeursNutritionnelles()["Glucide"]);
                    platPlusGlucides = ListeDejeuners.Find(plat => plat.CalculerValeursNutritionnelles()["Glucide"] == maxGlucides);
                    ListePlatsRetires.Add(platPlusGlucides);
                    ListeDejeuners.Remove(platPlusGlucides);
                }

                for (int i = 0; i < ListeBreuvages.Count / 2; i++)
                {
                    maxGlucides = ListeBreuvages.Max(plat => plat.CalculerValeursNutritionnelles()["Glucide"]);
                    platPlusGlucides = ListeBreuvages.Find(plat => plat.CalculerValeursNutritionnelles()["Glucide"] == maxGlucides);
                    ListePlatsRetires.Add(platPlusGlucides);
                    ListeBreuvages.Remove(platPlusGlucides);
                }

                for (int i = 0; i < ListeEntrees.Count / 2; i++)
                {
                    maxGlucides = ListeEntrees.Max(plat => plat.CalculerValeursNutritionnelles()["Glucide"]);
                    platPlusGlucides = ListeEntrees.Find(plat => plat.CalculerValeursNutritionnelles()["Glucide"] == maxGlucides);
                    ListePlatsRetires.Add(platPlusGlucides);
                    ListeEntrees.Remove(platPlusGlucides);
                }

                for (int i = 0; i < ListePlatPrincipaux.Count / 2; i++)
                {
                    maxGlucides = ListePlatPrincipaux.Max(plat => plat.CalculerValeursNutritionnelles()["Glucide"]);
                    platPlusGlucides = ListePlatPrincipaux.Find(plat => plat.CalculerValeursNutritionnelles()["Glucide"] == maxGlucides);
                    ListePlatsRetires.Add(platPlusGlucides);
                    ListePlatPrincipaux.Remove(platPlusGlucides);
                }

                for (int i = 0; i < ListeDesserts.Count / 2; i++)
                {
                    maxGlucides = ListeDesserts.Max(plat => plat.CalculerValeursNutritionnelles()["Glucide"]);
                    platPlusGlucides = ListeDesserts.Find(plat => plat.CalculerValeursNutritionnelles()["Glucide"] == maxGlucides);
                    ListePlatsRetires.Add(platPlusGlucides);
                    ListeDesserts.Remove(platPlusGlucides);
                }
            }

            /************************** Cholestérol **************************/
            if (App.MembreCourant.ListeRestrictions.Contains(new RestrictionAlimentaire { Nom = "Cholestérol" })
                || App.MembreCourant.ListeObjectifs.Contains(new Objectif { Nom = "Contrôle du cholestérol" }))
            {
                double maxCholesterol;
                Plat platPlusCholesterol;

                for (int i = 0; i < ListeDejeuners.Count / 2; i++)
                {
                    maxCholesterol = ListeDejeuners.Max(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"]);
                    platPlusCholesterol = ListeDejeuners.Find(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"] == maxCholesterol);
                    ListePlatsRetires.Add(platPlusCholesterol);
                    ListeDejeuners.Remove(platPlusCholesterol);
                }

                for (int i = 0; i < ListeBreuvages.Count / 2; i++)
                {
                    maxCholesterol = ListeBreuvages.Max(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"]);
                    platPlusCholesterol = ListeBreuvages.Find(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"] == maxCholesterol);
                    ListePlatsRetires.Add(platPlusCholesterol);
                    ListeBreuvages.Remove(platPlusCholesterol);
                }

                for (int i = 0; i < ListeEntrees.Count / 2; i++)
                {
                    maxCholesterol = ListeEntrees.Max(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"]);
                    platPlusCholesterol = ListeEntrees.Find(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"] == maxCholesterol);
                    ListePlatsRetires.Add(platPlusCholesterol);
                    ListeEntrees.Remove(platPlusCholesterol);
                }

                for (int i = 0; i < ListePlatPrincipaux.Count / 2; i++)
                {
                    maxCholesterol = ListePlatPrincipaux.Max(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"]);
                    platPlusCholesterol = ListePlatPrincipaux.Find(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"] == maxCholesterol);
                    ListePlatsRetires.Add(platPlusCholesterol);
                    ListePlatPrincipaux.Remove(platPlusCholesterol);
                }

                for (int i = 0; i < ListeDesserts.Count / 2; i++)
                {
                    maxCholesterol = ListeDesserts.Max(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"]);
                    platPlusCholesterol = ListeDesserts.Find(plat => plat.CalculerValeursNutritionnelles()["Cholesterol"] == maxCholesterol);
                    ListePlatsRetires.Add(platPlusCholesterol);
                    ListeDesserts.Remove(platPlusCholesterol);
                }
            }

            /************************** Haute/basse pression **************************/
            if (App.MembreCourant.ListeRestrictions.Contains(new RestrictionAlimentaire { Nom = "Haute/Basse pression" }))
            {
                double maxSodium;
                Plat platPlusSodium;

                for (int i = 0; i < ListeDejeuners.Count / 2; i++)
                {
                    maxSodium = ListeDejeuners.Max(plat => plat.CalculerValeursNutritionnelles()["Sodium"]);
                    platPlusSodium = ListeDejeuners.Find(plat => plat.CalculerValeursNutritionnelles()["Sodium"] == maxSodium);
                    ListePlatsRetires.Add(platPlusSodium);
                    ListeDejeuners.Remove(platPlusSodium);
                }

                for (int i = 0; i < ListeBreuvages.Count / 2; i++)
                {
                    maxSodium = ListeBreuvages.Max(plat => plat.CalculerValeursNutritionnelles()["Sodium"]);
                    platPlusSodium = ListeBreuvages.Find(plat => plat.CalculerValeursNutritionnelles()["Sodium"] == maxSodium);
                    ListePlatsRetires.Add(platPlusSodium);
                    ListeBreuvages.Remove(platPlusSodium);
                }

                for (int i = 0; i < ListeEntrees.Count / 2; i++)
                {
                    maxSodium = ListeEntrees.Max(plat => plat.CalculerValeursNutritionnelles()["Sodium"]);
                    platPlusSodium = ListeEntrees.Find(plat => plat.CalculerValeursNutritionnelles()["Sodium"] == maxSodium);
                    ListePlatsRetires.Add(platPlusSodium);
                    ListeEntrees.Remove(platPlusSodium);
                }

                for (int i = 0; i < ListePlatPrincipaux.Count / 2; i++)
                {
                    maxSodium = ListePlatPrincipaux.Max(plat => plat.CalculerValeursNutritionnelles()["Sodium"]);
                    platPlusSodium = ListePlatPrincipaux.Find(plat => plat.CalculerValeursNutritionnelles()["Sodium"] == maxSodium);
                    ListePlatsRetires.Add(platPlusSodium);
                    ListePlatPrincipaux.Remove(platPlusSodium);
                }

                for (int i = 0; i < ListeDesserts.Count / 2; i++)
                {
                    maxSodium = ListeDesserts.Max(plat => plat.CalculerValeursNutritionnelles()["Sodium"]);
                    platPlusSodium = ListeDesserts.Find(plat => plat.CalculerValeursNutritionnelles()["Sodium"] == maxSodium);
                    ListePlatsRetires.Add(platPlusSodium);
                    ListeDesserts.Remove(platPlusSodium);
                }
            }

            /************************** Perte de poids **************************/
            if (App.MembreCourant.ListeObjectifs.Contains(new Objectif { Nom = "Perte de poids" }))
            {
                double maxCalories;
                Plat platPlusCalorique;

                for (int i = 0; i < ListeDejeuners.Count / 2; i++)
                {
                    maxCalories = ListeDejeuners.Max(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platPlusCalorique = ListeDejeuners.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == maxCalories);
                    ListePlatsRetires.Add(platPlusCalorique);
                    ListeDejeuners.Remove(platPlusCalorique);
                }

                for (int i = 0; i < ListeBreuvages.Count / 2; i++)
                {
                    maxCalories = ListeBreuvages.Max(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platPlusCalorique = ListeBreuvages.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == maxCalories);
                    ListePlatsRetires.Add(platPlusCalorique);
                    ListeBreuvages.Remove(platPlusCalorique);
                }

                for (int i = 0; i < ListeEntrees.Count / 2; i++)
                {
                    maxCalories = ListeEntrees.Max(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platPlusCalorique = ListeEntrees.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == maxCalories);
                    ListePlatsRetires.Add(platPlusCalorique);
                    ListeEntrees.Remove(platPlusCalorique);
                }

                for (int i = 0; i < ListePlatPrincipaux.Count / 2; i++)
                {
                    maxCalories = ListePlatPrincipaux.Max(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platPlusCalorique = ListePlatPrincipaux.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == maxCalories);
                    ListePlatsRetires.Add(platPlusCalorique);
                    ListePlatPrincipaux.Remove(platPlusCalorique);
                }

                for (int i = 0; i < ListeDesserts.Count / 2; i++)
                {
                    maxCalories = ListeDesserts.Max(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platPlusCalorique = ListeDesserts.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == maxCalories);
                    ListePlatsRetires.Add(platPlusCalorique);
                    ListeDesserts.Remove(platPlusCalorique);
                }
            }

            /************************** Gain de poids **************************/
            if (App.MembreCourant.ListeObjectifs.Contains(new Objectif { Nom = "Gain de poids" }))
            {
                double minCalories;
                Plat platMoinsCalorique;

                for (int i = 0; i < ListeDejeuners.Count / 2; i++)
                {
                    minCalories = ListeDejeuners.Min(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platMoinsCalorique = ListeDejeuners.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == minCalories);
                    ListePlatsRetires.Add(platMoinsCalorique);
                    ListeDejeuners.Remove(platMoinsCalorique);
                }

                for (int i = 0; i < ListeBreuvages.Count / 2; i++)
                {
                    minCalories = ListeBreuvages.Min(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platMoinsCalorique = ListeBreuvages.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == minCalories);
                    ListePlatsRetires.Add(platMoinsCalorique);
                    ListeBreuvages.Remove(platMoinsCalorique);
                }

                for (int i = 0; i < ListeEntrees.Count / 2; i++)
                {
                    minCalories = ListeEntrees.Min(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platMoinsCalorique = ListeEntrees.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == minCalories);
                    ListePlatsRetires.Add(platMoinsCalorique);
                    ListeEntrees.Remove(platMoinsCalorique);
                }

                for (int i = 0; i < ListePlatPrincipaux.Count / 2; i++)
                {
                    minCalories = ListePlatPrincipaux.Min(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platMoinsCalorique = ListePlatPrincipaux.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == minCalories);
                    ListePlatsRetires.Add(platMoinsCalorique);
                    ListePlatPrincipaux.Remove(platMoinsCalorique);
                }

                for (int i = 0; i < ListeDesserts.Count / 2; i++)
                {
                    minCalories = ListeDesserts.Min(plat => plat.CalculerValeursNutritionnelles()["Energie"]);
                    platMoinsCalorique = ListeDesserts.Find(plat => plat.CalculerValeursNutritionnelles()["Energie"] == minCalories);
                    ListePlatsRetires.Add(platMoinsCalorique);
                    ListeDesserts.Remove(platMoinsCalorique);
                }
            }

            /************************** Gain musculaire **************************/
            if (App.MembreCourant.ListeObjectifs.Contains(new Objectif { Nom = "Gain musculaire" }))
            {
                double minProteines;
                Plat platMoinsProteines;

                for (int i = 0; i < ListeDejeuners.Count / 2; i++)
                {
                    minProteines = ListeDejeuners.Min(plat => plat.CalculerValeursNutritionnelles()["Proteine"]);
                    platMoinsProteines = ListeDejeuners.Find(plat => plat.CalculerValeursNutritionnelles()["Proteine"] == minProteines);
                    ListePlatsRetires.Add(platMoinsProteines);
                    ListeDejeuners.Remove(platMoinsProteines);
                }

                for (int i = 0; i < ListeBreuvages.Count / 2; i++)
                {
                    minProteines = ListeBreuvages.Min(plat => plat.CalculerValeursNutritionnelles()["Proteine"]);
                    platMoinsProteines = ListeBreuvages.Find(plat => plat.CalculerValeursNutritionnelles()["Proteine"] == minProteines);
                    ListePlatsRetires.Add(platMoinsProteines);
                    ListeBreuvages.Remove(platMoinsProteines);
                }

                for (int i = 0; i < ListeEntrees.Count / 2; i++)
                {
                    minProteines = ListeEntrees.Min(plat => plat.CalculerValeursNutritionnelles()["Proteine"]);
                    platMoinsProteines = ListeEntrees.Find(plat => plat.CalculerValeursNutritionnelles()["Proteine"] == minProteines);
                    ListePlatsRetires.Add(platMoinsProteines);
                    ListeEntrees.Remove(platMoinsProteines);
                }

                for (int i = 0; i < ListePlatPrincipaux.Count / 2; i++)
                {
                    minProteines = ListePlatPrincipaux.Min(plat => plat.CalculerValeursNutritionnelles()["Proteine"]);
                    platMoinsProteines = ListePlatPrincipaux.Find(plat => plat.CalculerValeursNutritionnelles()["Proteine"] == minProteines);
                    ListePlatsRetires.Add(platMoinsProteines);
                    ListePlatPrincipaux.Remove(platMoinsProteines);
                }

                for (int i = 0; i < ListeDesserts.Count / 2; i++)
                {
                    minProteines = ListeDesserts.Min(plat => plat.CalculerValeursNutritionnelles()["Proteine"]);
                    platMoinsProteines = ListeDesserts.Find(plat => plat.CalculerValeursNutritionnelles()["Proteine"] == minProteines);
                    ListePlatsRetires.Add(platMoinsProteines);
                    ListeDesserts.Remove(platMoinsProteines);
                }
            }
        }

        /// <summary>
        /// Méthode permettant de générer dynamiquement les rangées de la grid contenant le menu.
        /// </summary>
        /// <param name="nbRangees">Le nombre de rangées.</param>
        private void GenererRangees(int nbRangees, int hauteur)
        {
            RowDefinition rowDefinition;

            for (int i = 0; i < nbRangees; i++)
            {
                rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(hauteur);

                grdMenus.RowDefinitions.Add(rowDefinition);
            }
        }

        /// <summary>
        /// Méthode permettant de générer dynamiquement les colonnes de la grid contenant le menu.
        /// </summary>
        private void GenererColonnes()
        {
            ColumnDefinition columnDefinition;

            if(NbColonnes == NB_COLONNES_SANS_IMAGES)
            {
                for (int i = 0; i < NbColonnes; i++)
                {
                    columnDefinition = new ColumnDefinition();
                    if(i == 0)
                    {
                        columnDefinition.Width = new GridLength(2, GridUnitType.Star);
                    }
                    else
                    {
                        columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                    }
                    grdMenus.ColumnDefinitions.Add(columnDefinition);
                }
            }
            else if(NbColonnes == NB_COLONNES_AVEC_IMAGES)
            {
                for (int i = 0; i < NbColonnes; i++)
                {
                    columnDefinition = new ColumnDefinition();
                    if(i < 2)
                    {
                        columnDefinition.Width = new GridLength(3, GridUnitType.Star);
                    }
                    else
                    {
                        columnDefinition.Width = new GridLength(2, GridUnitType.Star);
                    }
                    grdMenus.ColumnDefinitions.Add(columnDefinition);
                }
            }
        }

        /// <summary>
        /// Méthode permettant d'initialiser la section des menus.
        /// </summary>
        private void InitialiserSectionMenu(int nbPlats, int hauteurRangees)
        {
            svMenus.ScrollToTop();
            // Génération des rangées de la grid.
            grdMenus.RowDefinitions.Clear();
            GenererRangees(nbPlats, hauteurRangees);
            grdMenus.ColumnDefinitions.Clear();
            GenererColonnes();
            Grid.SetRowSpan(dgMenus, nbPlats);
            Grid.SetColumnSpan(dgMenus, grdMenus.ColumnDefinitions.Count);
            
            // Retrait des séparateurs s'il y a lieu.
            List<Label> separateurs = new List<Label>(grdMenus.Children.OfType<Label>());

            foreach (Label separateurCourant in separateurs)
            {
                grdMenus.Children.Remove(separateurCourant);
            }
        }

        /// <summary>
        /// Méthode permettant de générer un séparateur de plats.
        /// </summary>
        /// <param name="contenu">Le contenu du séparateur.</param>
        /// <param name="index">L'index du séparateur dans la Grid.</param>
        private void GenererSeparateurPlat(string contenu, int index)
        {
            Label lblSeparateur = new Label();
            lblSeparateur.Content = contenu;
            lblSeparateur.Style = FindResource("fontNutritia") as Style;
            lblSeparateur.Foreground = Brushes.Red;
            lblSeparateur.FontSize = 16;
            lblSeparateur.HorizontalAlignment = HorizontalAlignment.Center;
            Thickness margin = lblSeparateur.Margin;
            margin.Left = 10;
            margin.Top = 10;
            lblSeparateur.Margin = margin;
            Grid.SetRow(lblSeparateur, index);
            grdMenus.Children.Add(lblSeparateur);
        }

        /// <summary>
        /// Méthode permettant d'ajouter des séparateurs entre les plats d'un menu.
        /// </summary>
        private void AjouterSeparateursPlats()
        {
            int nbPlats = MenuGenere.ListePlats.Count;
            int nbJours = nbPlats / 10;

            // Il s'agit d'un déjeuner.
            if (nbPlats == NB_PLATS_DEJEUNER)
            {
                GenererSeparateurPlat("Déjeuner", 0);
                GenererSeparateurPlat("Breuvage", 1);
            }

            // Il s'agit d'un diner/souper.
            if (nbPlats == NB_PLATS_DINER_SOUPER)
            {
                GenererSeparateurPlat("Entrée", 0);
                GenererSeparateurPlat("Plat principal", 1);
                GenererSeparateurPlat("Breuvage", 2);
                GenererSeparateurPlat("Déssert", 3);
            }

            // Il s'agit d'une journée ou encore d'une semaine.
            if (nbJours > 0)
            {
                for (int i = 0; i < nbJours; i++)
                {
                    GenererSeparateurPlat("Jour " + (i + 1) + " (Déjeuner)", i * (nbPlats / nbJours));
                    GenererSeparateurPlat("Jour " + (i + 1) + " (Breuvage)", i * (nbPlats / nbJours) + 1);

                    for (int j = 0; j < 2; j++)
                    {
                        GenererSeparateurPlat("Jour " + (i + 1) + " (Entrée)", (i * (nbPlats / nbJours) + 2 + (j * 4)));
                        GenererSeparateurPlat("Jour " + (i + 1) + " (Plat principal)", (i * (nbPlats / nbJours) + 3 + (j * 4)));
                        GenererSeparateurPlat("Jour " + (i + 1) + " (Breuvage)", (i * (nbPlats / nbJours) + 4 + (j * 4)));
                        GenererSeparateurPlat("Jour " + (i + 1) + " (Déssert)", (i * (nbPlats / nbJours) + 5 + (j * 4)));
                    }
                }
            }
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
                if (chbAfficherImages.IsChecked == null || chbAfficherImages.IsChecked == false)
                {
                    dgMenus.RowHeight = 60;
                    dgtcNom.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                    dgtcImage.Visibility = Visibility.Hidden;
                    dgtcRegenerer.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    dgtcIngredient.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    NbColonnes = 3;
                }

                MenuGenere = popupOuvertureMenu.MenuSelectionne;
                InitialiserSectionMenu(MenuGenere.ListePlats.Count, (Convert.ToInt32(dgMenus.RowHeight)));
                gbMenus.Header = MenuGenere.Nom;
                AjouterSeparateursPlats();
                dgMenus.ItemsSource = MenuGenere.ListePlats;
                btnSauvegarder.IsEnabled = true;
                btnListeEpicerie.IsEnabled = true;
                spInfosSup.Visibility = Visibility.Hidden;
                gbMenus.Visibility = Visibility.Visible;
                EstNouveauMenu = false;
            }
        }

        /// <summary>
        /// Méthode permettant de calculer des statistiques à propos du menu généré.
        /// </summary>
        /// <returns>Un dictionnaire contenant les statistiques de certaines préférences.</returns>
        private Dictionary<string, int> CalculerStatistiques()
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();

            stats["viande"] = 0;
            stats["pate"] = 0;
            stats["poisson"] = 0;

            foreach(Plat platCourant in MenuGenere.ListePlats)
            {
                bool contientDejaViande = false;
                bool contientDejaPate = false;
                bool contientDejaPoisson = false;

                foreach(Aliment alimentCourant in platCourant.ListeIngredients)
                {
                    if(alimentCourant.Categorie == "Viandes et substituts" && !contientDejaViande)
                    {
                        stats["viande"]++;
                        contientDejaViande = true;
                    }

                    if(alimentCourant.Categorie == "Pâtes" && !contientDejaPate)
                    {
                        stats["pate"]++;
                        contientDejaPate = true;
                    }

                    if (alimentCourant.Categorie == "Poissons et fruits de mers" && !contientDejaPoisson)
                    {
                        stats["poisson"]++;
                        contientDejaPoisson = true;

                    }
                }
            }

            return stats;
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
            int nbPlats = NB_PLATS_DINER_SOUPER;

            if (rbDejeuner.IsChecked != null && (bool)rbDejeuner.IsChecked) { nbPlats = NB_PLATS_DEJEUNER; }
            if (rbDinerSouper.IsChecked != null && (bool)rbDinerSouper.IsChecked) { nbPlats = NB_PLATS_DINER_SOUPER; }
            if (rbMenuJournalier.IsChecked != null && (bool)rbMenuJournalier.IsChecked) { nbJours = 1; nbPlats = NB_PLATS_JOURNEE; }
            if (rbMenuHebdomadaire.IsChecked != null && (bool)rbMenuHebdomadaire.IsChecked) { nbJours = 7; nbPlats = NB_PLATS_SEMAINE; }
            
            MenuGenere.NbPersonnes = Convert.ToInt32(((ComboBoxItem)cboNbPersonnes.SelectedItem).Content);

            if(chbAfficherImages.IsChecked == null || chbAfficherImages.IsChecked == false)
            {
                dgMenus.RowHeight = 60;
                dgtcNom.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                dgtcImage.Visibility = Visibility.Hidden;
                dgtcRegenerer.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                dgtcIngredient.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                NbColonnes = 3;
            }

            InitialiserSectionMenu(nbPlats, (Convert.ToInt32(dgMenus.RowHeight)));
            gbMenus.Header = "Menus";

            // Il s'agit d'un déjeuner.
            if(nbPlats == NB_PLATS_DEJEUNER)
            {
                MenuGenere.ListePlats.Add(ListeDejeuners.Count > 0 ? ListeDejeuners[Rand.Next(0, ListeDejeuners.Count)] : new Plat());
                MenuGenere.ListePlats.Add(ListeBreuvages.Count > 0 ? ListeBreuvages[Rand.Next(0, ListeBreuvages.Count)] : new Plat());
            }

            // Il s'agit d'un diner/souper.
            if(nbPlats == NB_PLATS_DINER_SOUPER)
            {
                MenuGenere.ListePlats.Add(ListeEntrees.Count > 0 ? ListeEntrees[Rand.Next(0, ListeEntrees.Count)] : new Plat());
                MenuGenere.ListePlats.Add(ListePlatPrincipaux.Count > 0 ? ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)] : new Plat());
                MenuGenere.ListePlats.Add(ListeBreuvages.Count > 0 ? ListeBreuvages[Rand.Next(0, ListeBreuvages.Count)] : new Plat());
                MenuGenere.ListePlats.Add(ListeDesserts.Count > 0 ? ListeDesserts[Rand.Next(0, ListeDesserts.Count)] : new Plat());
            }

            // Il s'agit d'une journée ou encore d'une semaine.
            if(nbJours > 0)
            {
                for (int i = 0; i < nbJours; i++)
                {
                    MenuGenere.ListePlats.Add(ListeDejeuners.Count > 0 ? ListeDejeuners[Rand.Next(0, ListeDejeuners.Count)] : new Plat());
                    MenuGenere.ListePlats.Add(ListeBreuvages.Count > 0 ? ListeBreuvages[Rand.Next(0, ListeBreuvages.Count)] : new Plat());

                    for (int j = 0; j < 2; j++)
                    {
                        MenuGenere.ListePlats.Add(ListeEntrees.Count > 0 ? ListeEntrees[Rand.Next(0, ListeEntrees.Count)] : new Plat());
                        MenuGenere.ListePlats.Add(ListePlatPrincipaux.Count > 0 ? ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)] : new Plat());
                        MenuGenere.ListePlats.Add(ListeBreuvages.Count > 0 ? ListeBreuvages[Rand.Next(0, ListeBreuvages.Count)] : new Plat());
                        MenuGenere.ListePlats.Add(ListeDesserts.Count > 0 ? ListeDesserts[Rand.Next(0, ListeDesserts.Count)] : new Plat());
                    }
                }
            }

            if(!string.IsNullOrEmpty(App.MembreCourant.NomUtilisateur))
            {
                btnSauvegarder.IsEnabled = true;
                AffecterPreferences();
            }

            dgMenus.ItemsSource = MenuGenere.ListePlats;
            AjouterSeparateursPlats();
            btnListeEpicerie.IsEnabled = true;
            spInfosSup.Visibility = Visibility.Hidden;
            gbMenus.Visibility = Visibility.Visible;
            EstNouveauMenu = true;
        }

        /// <summary>
        /// Méthode permettant d'affecter les préférences d'un membre a un menu généré.
        /// </summary>
        private void AffecterPreferences()
        {
            if(MenuGenere.ListePlats.Count == NB_PLATS_DEJEUNER)
            {
                if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Viandes" }))
                {
                    while(!MenuGenere.ListePlats[0].ObtenirCategoriesIngredients().Contains("Viandes et substituts"))
                    {
                        MenuGenere.ListePlats[0] = ListeDejeuners[Rand.Next(0, ListeDejeuners.Count)];
                    }
                }
            }
            else if (MenuGenere.ListePlats.Count == NB_PLATS_DINER_SOUPER)
            {
                if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Viandes" }))
                {
                    while (!MenuGenere.ListePlats[1].ObtenirCategoriesIngredients().Contains("Viandes et substituts"))
                    {
                        MenuGenere.ListePlats[1] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                    }
                }
                else if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Pâtes" }))
                {
                    while (!MenuGenere.ListePlats[1].ObtenirCategoriesIngredients().Contains("Pâtes"))
                    {
                        MenuGenere.ListePlats[1] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                    }
                }
                else if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Poissons et fruits de mers" }))
                {
                    while (!MenuGenere.ListePlats[1].ObtenirCategoriesIngredients().Contains("Poissons et fruits de mers"))
                    {
                        MenuGenere.ListePlats[1] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                    }
                }
            }
            else if (MenuGenere.ListePlats.Count == NB_PLATS_JOURNEE)
            {
                int nbPlatsCorrespondant;

                if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Viandes" }))
                {
                    nbPlatsCorrespondant = CalculerStatistiques()["viande"];

                    while (nbPlatsCorrespondant < 1)
                    {
                        int positionAleatoire = Rand.Next(0, MenuGenere.ListePlats.Count);

                        switch (MenuGenere.ListePlats[positionAleatoire].TypePlat)
                        {
                            case "Entrée":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                                }
                                break;
                            case "Plat principal":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                                }
                                break;
                        }

                        nbPlatsCorrespondant = CalculerStatistiques()["viande"];

                    }
                }
                else if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Pâtes" }))
                {
                    nbPlatsCorrespondant = CalculerStatistiques()["pate"];

                    while (nbPlatsCorrespondant < 1)
                    {
                        int positionAleatoire = Rand.Next(0, MenuGenere.ListePlats.Count);

                        switch (MenuGenere.ListePlats[positionAleatoire].TypePlat)
                        {
                            case "Entrée":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                                }
                                break;
                            case "Plat principal":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                                }
                                break;
                        }

                        nbPlatsCorrespondant = CalculerStatistiques()["pate"];

                    }
                }
                else if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Poissons et fruits de mers" }))
                {
                    nbPlatsCorrespondant = CalculerStatistiques()["poisson"];

                    while (nbPlatsCorrespondant < 1)
                    {
                        int positionAleatoire = Rand.Next(0, MenuGenere.ListePlats.Count);

                        switch (MenuGenere.ListePlats[positionAleatoire].TypePlat)
                        {
                            case "Entrée":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                                }
                                break;
                            case "Plat principal":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                                }
                                break;
                        }

                        nbPlatsCorrespondant = CalculerStatistiques()["poisson"];

                    }
                }
            }
            else if (MenuGenere.ListePlats.Count == NB_PLATS_SEMAINE)
            {
                int nbPlatsCorrespondant;

                if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Viandes" }))
                {
                    nbPlatsCorrespondant = CalculerStatistiques()["viande"];

                    while (nbPlatsCorrespondant < 7)
                    {
                        int positionAleatoire = Rand.Next(0, MenuGenere.ListePlats.Count);

                        switch (MenuGenere.ListePlats[positionAleatoire].TypePlat)
                        {
                            case "Entrée":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                                }
                                break;
                            case "Plat principal":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                                }
                                break;
                        }

                        nbPlatsCorrespondant = CalculerStatistiques()["viande"];

                    }
                }
                else if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Pâtes" }))
                {
                    nbPlatsCorrespondant = CalculerStatistiques()["pate"];

                    while (nbPlatsCorrespondant < 7)
                    {
                        int positionAleatoire = Rand.Next(0, MenuGenere.ListePlats.Count);

                        switch (MenuGenere.ListePlats[positionAleatoire].TypePlat)
                        {
                            case "Entrée":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                                }
                                break;
                            case "Plat principal":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                                }
                                break;
                        }

                        nbPlatsCorrespondant = CalculerStatistiques()["pate"];

                    }
                }
                else if (App.MembreCourant.ListePreferences.Contains(new Preference { Nom = "Poissons et fruits de mers" }))
                {
                    nbPlatsCorrespondant = CalculerStatistiques()["poisson"];

                    while (nbPlatsCorrespondant < 7)
                    {
                        int positionAleatoire = Rand.Next(0, MenuGenere.ListePlats.Count);

                        switch (MenuGenere.ListePlats[positionAleatoire].TypePlat)
                        {
                            case "Entrée":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                                }
                                break;
                            case "Plat principal":
                                if (ListeEntrees.Count > 1)
                                {
                                    MenuGenere.ListePlats[positionAleatoire] = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                                }
                                break;
                        }

                        nbPlatsCorrespondant = CalculerStatistiques()["poisson"];

                    }
                }
            }
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Sauvegarder.
        /// Permet de sauvegarder un menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            if(EstNouveauMenu)
            {
                FenetreSauvegarderMenu popupSauvegarde = new FenetreSauvegarderMenu();
                popupSauvegarde.ShowDialog();

                if (popupSauvegarde.DialogResult == true)
                {
                    MenuGenere.Nom = popupSauvegarde.txtNom.Text;
                    MenuService.Insert(MenuGenere);
                }
            }
            else
            {
                MenuService.Update(MenuGenere);
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
            fenetreIngredients.Show();
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

            switch(platSelectionne.TypePlat)
            {
                case "Déjeuner":
                    if(ListeDejeuners.Count > 1)
                    {
                        platRegenere = ListeDejeuners[Rand.Next(0, ListeDejeuners.Count)];
                        while (platRegenere.Nom == platSelectionne.Nom)
                        {
                            platRegenere = ListeDejeuners[Rand.Next(0, ListeDejeuners.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Entrée":
                    if(ListeEntrees.Count > 1)
                    {
                        platRegenere = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                        while (platRegenere.Nom == platSelectionne.Nom)
                        {
                            platRegenere = ListeEntrees[Rand.Next(0, ListeEntrees.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Plat principal":
                    if(ListePlatPrincipaux.Count > 1)
                    {
                        platRegenere = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                        while (platRegenere.Nom == platSelectionne.Nom)
                        {
                            platRegenere = ListePlatPrincipaux[Rand.Next(0, ListePlatPrincipaux.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Breuvage":
                    if(ListeBreuvages.Count > 1)
                    {
                        platRegenere = ListeBreuvages[Rand.Next(0, ListeBreuvages.Count)];
                        while (platRegenere.Nom == platSelectionne.Nom)
                        {
                            platRegenere = ListeBreuvages[Rand.Next(0, ListeBreuvages.Count)];
                        }
                        MenuGenere.ListePlats[dgMenus.SelectedIndex] = platRegenere;
                    }
                break;
                case "Déssert":
                    if(ListeDesserts.Count > 1)
                    {
                        platRegenere = ListeDesserts[Rand.Next(0, ListeDesserts.Count)];
                        while (platRegenere.Nom == platSelectionne.Nom)
                        {
                            platRegenere = ListeDesserts[Rand.Next(0, ListeDesserts.Count)];
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
            App.Current.MainWindow.ResizeMode = ResizeMode.CanMinimize;
            App.Current.MainWindow.Width = App.APP_WIDTH;
            App.Current.MainWindow.Height = App.APP_HEIGHT;
            App.Current.MainWindow.WindowState = WindowState.Normal;

            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue<FenetreListeEpicerie>(new FenetreListeEpicerie(MenuGenere));
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton de suivi des plats non admissibles.
        /// Permet d'afficher l'ensemble des plats non admissibles à la génération.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSuiviPlatsNonAdmissibles_Click(object sender, RoutedEventArgs e)
        {
            FenetreSuiviRestrictions fenetreSuvi = new FenetreSuiviRestrictions(ListePlatsRetires);
            fenetreSuvi.ShowDialog();

            if(fenetreSuvi.DialogResult == true)
            {
                foreach(Plat platCourant in fenetreSuvi.ListePlatsAdmissibles)
                {
                    switch (platCourant.TypePlat)
                    {
                        case "Déjeuner":
                            ListeDejeuners.Add(platCourant);
                            break;
                        case "Entrée":
                            ListeEntrees.Add(platCourant);
                            break;
                        case "Plat principal":
                            ListePlatPrincipaux.Add(platCourant);
                            break;
                        case "Breuvage":
                            ListeBreuvages.Add(platCourant);
                            break;
                        case "Déssert":
                            ListeDesserts.Add(platCourant);
                            break;
                    }
                }
            }
        }
    }
}