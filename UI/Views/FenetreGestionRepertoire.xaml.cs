using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for GestionRepertoire.xaml
    /// </summary>
    public partial class GestionRepertoire : UserControl
    {
        public bool Ajout { get; set; }
        public bool Modification { get; set; }
        public bool Aliment { get; set; }
        public bool Plat { get; set; }

        public bool Erreur { get; set; }

        IList<Aliment> listeAliments = new List<Aliment>();
        IList<Plat> listePlats = new List<Plat>();
        IList<Aliment> listeAlimentsPlateau = new List<Aliment>();
        int idAlimentModif;

        public GestionRepertoire()
        {
            InitializeComponent();

            Erreur = false;

            ModifierContexte();

            listeAliments = ServiceFactory.Instance.GetService<IAlimentService>().RetrieveAll();
            listePlats = ServiceFactory.Instance.GetService<IPlatService>().RetrieveAll();

            Construire_Accordeon();
        }

        /// <summary>
        /// Changements de contexte selon les boutons radio qui sont sélectionnés.
        /// </summary>
        #region ChangementContexte

        private void rdb_ajout_Checked(object sender, RoutedEventArgs e)
        {
            Ajout = true;
            Modification = false;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void rdb_modif_Checked(object sender, RoutedEventArgs e)
        {
            Ajout = false;
            Modification = true;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void rdb_aliment_Checked(object sender, RoutedEventArgs e)
        {
            Aliment = true;
            Plat = false;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void rdb_plat_Checked(object sender, RoutedEventArgs e)
        {
            Aliment = false;
            Plat = true;
            if (grid_ajout_aliment != null)
            {
                ModifierContexte();
            }
        }

        private void ModifierContexte()
        {
            if (Ajout)
            {
                if (Aliment)
                {
                    grid_ajout_aliment.Visibility = Visibility.Visible;
                    grid_modif_aliment.Visibility = Visibility.Hidden;
                    grid_ajout_plat.Visibility = Visibility.Hidden;
                    grid_modification_plat.Visibility = Visibility.Hidden;
                }
                else
                {
                    grid_modification_plat.Visibility = Visibility.Hidden;
                    grid_ajout_aliment.Visibility = Visibility.Hidden;
                    grid_modif_aliment.Visibility = Visibility.Hidden;
                    grid_ajout_plat.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (Aliment)
                {
                    grid_ajout_aliment.Visibility = Visibility.Hidden;
                    grid_modif_aliment.Visibility = Visibility.Visible;
                    grid_ajout_plat.Visibility = Visibility.Hidden;
                    grid_modification_plat.Visibility = Visibility.Hidden;
                }
                else
                {
                    grid_ajout_plat.Visibility = Visibility.Hidden;
                    grid_ajout_aliment.Visibility = Visibility.Hidden;
                    grid_modif_aliment.Visibility = Visibility.Hidden;
                    grid_modification_plat.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion

        /// <summary>
        /// Création et insertion d'un nouvel aliment dans la base de données.
        /// Inclue aussi les méthodes de validations des différents champs de saisie.
        /// </summary>
        #region InsertionModificationAliment

        /// <summary>
        /// Méthode permettant de valider le contenu des différents champs et de créer un nouvel aliment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider_Inserer_Aliment(object sender, RoutedEventArgs e)
        {
            /*-----------------------------------Validation des différents champs de saisie-----------------------------------*/

            // Initialement, il n'y a aucune erreur, alors la valeur est à false.
            Erreur = false;

            Valider_Nom(lbl_nom, Nom_alim);
            Valider_Si_Groupe(lbl_grp_alim, cbo_grp_alim);
            Valider_Si_Unite(lbl_unite_mesure, cbo_unite_mesure);
            Valider_Mesure(lbl_mesure, Mesure);
            Valider_Valeur_Nutritionnelle(lbl_calories, Calories);
            Valider_Valeur_Nutritionnelle(lbl_proteines, Proteines);
            Valider_Valeur_Nutritionnelle(lbl_glucides, Glucides);
            Valider_Valeur_Nutritionnelle(lbl_sodium, Sodium);
            Valider_Valeur_Nutritionnelle(lbl_fibres, Fibres);
            Valider_Valeur_Nutritionnelle(lbl_lipides, Lipides);
            Valider_Valeur_Nutritionnelle(lbl_cholesterol, Cholesterol);
            Valider_Total_Valeurs(lbl_valeurs, cbo_unite_mesure, Mesure, Proteines, Glucides, Sodium, Fibres, Lipides, Cholesterol);

            /* S'il y a une seule erreur suite au validations, alors la variable booléenne sera à true et conséquement,
               il faut signaler à l'utilisateur qu'il y a un problème. */
            if (Erreur)
            {
                Erreur_Champ();
            }
            // Si tout c'est bien passé, alors on insère les données et passe au menu des membres.
            else
            {
                //Inserer_Donnees();
                Inserer_Aliment();

                MessageBoxResult resultat;
                resultat = MessageBox.Show("Le nouvel aliment a été correctement ajouté à la base de données."
                                            , "Ajout d'aliment réussie"
                                            , MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Méthode permettant de valider le contenu des différents champs et de modifier un aliment existant.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider_Modifier_Aliment(object sender, RoutedEventArgs e)
        {
            /*-----------------------------------Validation des différents champs de saisie-----------------------------------*/

            // Initialement, il n'y a aucune erreur, alors la valeur est à false.
            Erreur = false;

            Valider_Nom_Modif(lbl_nom_modif, Nom_alim_modif);
            Valider_Si_Groupe(lbl_grp_alim_modif, cbo_grp_alim_modif);
            Valider_Si_Unite(lbl_unite_mesure_modif, cbo_unite_mesure_modif);
            Valider_Mesure(lbl_mesure_modif, Mesure_modif);
            Valider_Valeur_Nutritionnelle(lbl_calories_modif, Calories_modif);
            Valider_Valeur_Nutritionnelle(lbl_proteines_modif, Proteines_modif);
            Valider_Valeur_Nutritionnelle(lbl_glucides_modif, Glucides_modif);
            Valider_Valeur_Nutritionnelle(lbl_sodium_modif, Sodium_modif);
            Valider_Valeur_Nutritionnelle(lbl_fibres_modif, Fibres_modif);
            Valider_Valeur_Nutritionnelle(lbl_lipides_modif, Lipides_modif);
            Valider_Valeur_Nutritionnelle(lbl_cholesterol_modif, Cholesterol_modif);
            Valider_Total_Valeurs(lbl_valeurs_modif, cbo_unite_mesure_modif, Mesure_modif, Proteines_modif, Glucides_modif, Sodium_modif, Fibres_modif, Lipides_modif, Cholesterol_modif);

            /* S'il y a une seule erreur suite au validations, alors la variable booléenne sera à true et conséquement,
               il faut signaler à l'utilisateur qu'il y a un problème. */
            if (Erreur)
            {
                Erreur_Champ();
            }
            // Si tout c'est bien passé, alors on insère les données et passe au menu des membres.
            else
            {
                Modifier_Aliment();

                MessageBoxResult resultat;
                resultat = MessageBox.Show("L'aliment a été correctement modifié dans la base de données."
                                            , "Modification d'aliment réussie"
                                            , MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Méthode affichant un message d'erreur en cas d'un ou plusieurs champs invalides.
        /// </summary>
        private void Erreur_Champ()
        {
            MessageBoxResult resultat;
            resultat = MessageBox.Show("Il y a un ou plusieurs champs invalides."
                                        , "Erreur"
                                        , MessageBoxButton.OK
                                        , MessageBoxImage.Warning
                                        , MessageBoxResult.OK);
        }

        /// <summary>
        /// Méthode qui appelle la validation du nom de l'aliment à chaque modification du champ de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nom_alim_TextChanged(object sender, TextChangedEventArgs e)
        {
            Valider_Nom(lbl_nom, Nom_alim);
        }

        /// <summary>
        /// Méthode de validation du champ "Nom de l'aliment".
        /// De plus, la longueur du nom doit être comprise entre 1 et 50 caractères.
        /// </summary>
        private void Valider_Nom(Label unLabel, TextBox unTextBox)
        {
            /*-----------------------------------Vérifier que le nom n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(unTextBox.Text))
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = "Nom de l'aliment (Champ vide)";
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le nom n'est pas trop grand-----------------------------------*/
                if (unTextBox.Text.Length > 50)
                {
                    unLabel.Foreground = Brushes.Red;
                    unLabel.Content = "Nom de l'aliment (Entre 1 et 50 caractères)";
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le nom n'inclus que des caractères valides (A-z, 0-9, ', %, espaces et caractères accentués)-----------------------------------*/
                    Regex RgxNom = new Regex("[^A-z0-9ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÇçÌÍÎÏìíîïÙÚÛÜùúûüÿÑñ|'| |%]");
                    bool ContientSeulement = RgxNom.IsMatch(unTextBox.Text);

                    if (ContientSeulement)
                    {
                        unLabel.Foreground = Brushes.Red;
                        unLabel.Content = "Nom de l'aliment (charctères invalides)";
                        Erreur = true;
                    }
                    else
                    {
                        Regex RgxApostropheDebut = new Regex("^['| ]");
                        bool ContientApostropheDebut = RgxApostropheDebut.IsMatch(unTextBox.Text);

                        if (ContientApostropheDebut)
                        {
                            unLabel.Foreground = Brushes.Red;
                            unLabel.Content = "Nom (Doit débuter avec une lettre)";
                            Erreur = true;
                        }
                        else
                        {
                            /*-----------------------------------Vérifier que le nom de l'aliment est unique par rapport à la BD-----------------------------------*/
                            /* Afin de simplifier et tenir compte d'une multitudes de possibilités, les boucles suivantes convertissent les noms d'aliments en lettre majuscules. */
                            bool unique = true;

                            for (int i = 0; i < listeAliments.Count; i++)
                            {
                                string alimentBD = "";
                                string alimentText = "";

                                for (int j = 0; j < listeAliments[i].Nom.Length; j++)
                                {
                                    if (char.IsLower(listeAliments[i].Nom[j]))
                                    {
                                        alimentBD += char.ToUpper(listeAliments[i].Nom[j]);
                                    }
                                    else
                                    {
                                        alimentBD += listeAliments[i].Nom[j];
                                    }
                                }

                                for (int k = 0; k < unTextBox.Text.Length; k++)
                                {
                                    if (char.IsLower(unTextBox.Text[k]))
                                    {
                                        alimentText += char.ToUpper(unTextBox.Text[k]);
                                    }
                                    else
                                    {
                                        alimentText += unTextBox.Text[k];
                                    }
                                }

                                if (alimentBD == alimentText)
                                {
                                    unLabel.Foreground = Brushes.Red;
                                    unLabel.Content = "Nom de l'aliment (aliment déjà existant)";
                                    Erreur = true;
                                    unique = false;
                                }

                            }

                            if (unique)
                            {
                                unLabel.Foreground = Brushes.DarkGreen;
                                unLabel.Content = "Nom de l'aliment";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode de validation du champ "Nom de l'aliment" lors de la modification d'un aliment.
        /// De plus, la longueur du nom doit être comprise entre 1 et 50 caractères.
        /// </summary>
        private void Valider_Nom_Modif(Label unLabel, TextBox unTextBox)
        {
            /*-----------------------------------Vérifier que le nom n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(unTextBox.Text))
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = "Nom de l'aliment (Champ vide)";
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le nom n'est pas trop grand-----------------------------------*/
                if (unTextBox.Text.Length > 50)
                {
                    unLabel.Foreground = Brushes.Red;
                    unLabel.Content = "Nom de l'aliment (Entre 1 et 50 caractères)";
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le nom n'inclus que des caractères valides (A-z, 0-9, ', %, espaces et caractères accentués)-----------------------------------*/
                    Regex RgxNom = new Regex("[^A-z0-9ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÇçÌÍÎÏìíîïÙÚÛÜùúûüÿÑñ|'| |%]");
                    bool ContientSeulement = RgxNom.IsMatch(unTextBox.Text);

                    if (ContientSeulement)
                    {
                        unLabel.Foreground = Brushes.Red;
                        unLabel.Content = "Nom de l'aliment (charctères invalides)";
                        Erreur = true;
                    }
                    else
                    {
                        Regex RgxApostropheDebut = new Regex("^['| ]");
                        bool ContientApostropheDebut = RgxApostropheDebut.IsMatch(unTextBox.Text);

                        if (ContientApostropheDebut)
                        {
                            unLabel.Foreground = Brushes.Red;
                            unLabel.Content = "Nom (Doit débuter avec une lettre)";
                            Erreur = true;
                        }
                        else
                        {
                            /*-----------------------------------Vérifier que le nom de l'aliment est unique par rapport à la BD-----------------------------------*/
                            /* Afin de simplifier et tenir compte d'une multitudes de possibilités, les boucles suivantes convertissent les noms d'aliments en lettre majuscules. */
                            bool unique = true;

                            for (int i = 0; i < listeAliments.Count; i++)
                            {
                                string alimentBD = "";
                                string alimentText = "";
                                string alimentRecherche = "";


                                for (int j = 0; j < listeAliments[i].Nom.Length; j++)
                                {
                                    if (char.IsLower(listeAliments[i].Nom[j]))
                                    {
                                        alimentBD += char.ToUpper(listeAliments[i].Nom[j]);
                                    }
                                    else
                                    {
                                        alimentBD += listeAliments[i].Nom[j];
                                    }
                                }

                                for (int k = 0; k < unTextBox.Text.Length; k++)
                                {
                                    if (char.IsLower(unTextBox.Text[k]))
                                    {
                                        alimentText += char.ToUpper(unTextBox.Text[k]);
                                    }
                                    else
                                    {
                                        alimentText += unTextBox.Text[k];
                                    }
                                }

                                if (Search_alim.Text.Length > 0)
                                {
                                    for (int l = 0; l < Search_alim.Text.Length; l++)
                                    {
                                        if (char.IsLower(Search_alim.Text[l]))
                                        {
                                            alimentRecherche += char.ToUpper(Search_alim.Text[l]);
                                        }
                                        else
                                        {
                                            alimentRecherche += Search_alim.Text[l];
                                        }
                                    }

                                    if (alimentBD == alimentText && alimentText != alimentRecherche)
                                    {
                                        unLabel.Foreground = Brushes.Red;
                                        unLabel.Content = "Nom de l'aliment (aliment déjà existant)";
                                        Erreur = true;
                                        unique = false;
                                    }
                                }
                                else
                                {
                                    if (alimentBD == alimentText)
                                    {
                                        unLabel.Foreground = Brushes.Red;
                                        unLabel.Content = "Nom de l'aliment (aliment déjà existant)";
                                        Erreur = true;
                                        unique = false;
                                    }
                                }

                            }

                            if (unique)
                            {
                                unLabel.Foreground = Brushes.DarkGreen;
                                unLabel.Content = "Nom de l'aliment";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si un groupe alimentaire est sélectionné ou non.
        /// </summary>
        private void Valider_Si_Groupe(Label unLabel, ComboBox unComboBox)
        {
            if (unComboBox.SelectedIndex == -1)
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = "Catégorie alimentaire (Aucun de sélectionné)";
                Erreur = true;
            }
            else
            {
                unLabel.Foreground = Brushes.DarkGreen;
                unLabel.Content = "Catégorie alimentaire";
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si un type d'unité de mesure est sélectionné ou non.
        /// </summary>
        private void Valider_Si_Unite(Label unLabel, ComboBox unComboBox)
        {
            if (unComboBox.SelectedIndex == -1)
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = "Unité de mesure (Aucune de sélectionnée)";
                Erreur = true;
            }
            else
            {
                unLabel.Foreground = Brushes.DarkGreen;
                unLabel.Content = "Unité de mesure";
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si une mesure (taille de portion) est un entier strictement numérique compris entre 1 et 500.
        /// </summary>
        private void Valider_Mesure(Label unLabel, TextBox unTextBox)
        {
            /*-----------------------------------Vérifier que le champ n'est pas vide-----------------------------------*/
            if (String.IsNullOrEmpty(unTextBox.Text))
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = "Mesure de la portion (champ vide)";
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le champ est un entier numérique-----------------------------------*/
                Regex RgxChiffre = new Regex("[^0-9]");
                bool ContientSeulement = RgxChiffre.IsMatch(unTextBox.Text);

                if (ContientSeulement)
                {
                    unLabel.Foreground = Brushes.Red;
                    unLabel.Content = "Mesure de la portion (entier numérique)";
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le champ est compris entre 1 et 500-----------------------------------*/
                    if (int.Parse(unTextBox.Text) < 1 || int.Parse(unTextBox.Text) > 500)
                    {
                        unLabel.Foreground = Brushes.Red;
                        unLabel.Content = "Mesure de la portion (1 à 500)";
                        Erreur = true;
                    }
                    else
                    {
                        unLabel.Foreground = Brushes.DarkGreen;
                        unLabel.Content = "Mesure de la portion";
                    }
                }
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si une valeur nutritionnelle est un numérique double.
        /// </summary>
        private void Valider_Valeur_Nutritionnelle(Label unLabel, TextBox unTextBox)
        {
            /*-----------------------------------Vérifier que le champ n'est pas vide-----------------------------------*/
            if (String.IsNullOrEmpty(unTextBox.Text))
            {
                unLabel.Foreground = Brushes.Red;
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le champ est un double numérique entre 0 et 500-----------------------------------*/
                double d;
                bool estValide;

                estValide = double.TryParse(unTextBox.Text, out d) && d >= 0.00d && d <= 500.00d;

                if (!estValide)
                {
                    unLabel.Foreground = Brushes.Red;
                    Erreur = true;
                }
                else
                {
                    unLabel.Foreground = Brushes.DarkGreen;
                }
            }
        }

        /// <summary>
        /// Méthode permettant de valider le réalisme des valeurs nutritionnelles saisies par rapport à l'unité de mesure de l'aliment.
        /// </summary>
        private void Valider_Total_Valeurs(Label valeur, ComboBox uniteMesure, TextBox mesure, TextBox proteines, TextBox glucides, TextBox sodium, TextBox fibres, TextBox lipides, TextBox cholesterol)
        {
            if (uniteMesure.SelectedIndex != -1)
            {
                string sUniteMesure = uniteMesure.SelectionBoxItem.ToString();

                double somme = 0;

                double iMesure;
                double iProteine;
                double iGlucides;
                double iSodium;
                double iFibres;
                double iLipides;
                double iCholesterol;

                if (mesure.Text != string.Empty)
                {
                    iMesure = double.Parse(mesure.Text);
                }
                else
                {
                    iMesure = 0;
                }

                if (proteines.Text != string.Empty)
                {
                    iProteine = double.Parse(proteines.Text);
                }
                else
                {
                    iProteine = 0;
                }

                if (glucides.Text != string.Empty)
                {
                    iGlucides = double.Parse(glucides.Text);
                }
                else
                {
                    iGlucides = 0;
                }

                if (sodium.Text != string.Empty)
                {
                    iSodium = double.Parse(sodium.Text) / 1000;
                }
                else
                {
                    iSodium = 0;
                }

                if (fibres.Text != string.Empty)
                {
                    iFibres = double.Parse(fibres.Text);
                }
                else
                {
                    iFibres = 0;
                }

                if (lipides.Text != string.Empty)
                {
                    iLipides = double.Parse(lipides.Text);
                }
                else
                {
                    iLipides = 0;
                }

                if (cholesterol.Text != string.Empty)
                {
                    iCholesterol = double.Parse(cholesterol.Text) / 1000;
                }
                else
                {
                    iCholesterol = 0;
                }

                if (sUniteMesure == "Millilitre" || sUniteMesure == "Gramme")
                {
                    somme += iProteine;
                    somme += iGlucides;
                    somme += iSodium;
                    somme += iFibres;
                    somme += iLipides;
                    somme += iCholesterol;

                    if (somme > iMesure)
                    {
                        valeur.Foreground = Brushes.Red;
                        valeur.Content = "Valeurs nutritionnelles (total irréel)";
                        Erreur = true;
                    }
                    else
                    {
                        valeur.Foreground = Brushes.DarkGreen;
                        valeur.Content = "Valeurs nutritionnelles";
                    }
                }
            }

        }

        /// <summary>
        /// Méthode d'insertion dans la base de données d'un nouvel aliment.
        /// </summary>
        private void Inserer_Aliment()
        {
            Aliment nouvelAliment = new Aliment();

            //----------------------------------------Enregistrement des valeurs nutritionnelles----------------------------------------

            // Conversion automatisée de la première lettre du nom de l'aliment.
            if (char.IsLower(Nom_alim.Text[0]))
            {
                Nom_alim.Text = char.ToUpper(Nom_alim.Text[0]) + Nom_alim.Text.Substring(1);
            }

            nouvelAliment.Nom = Nom_alim.Text;
            nouvelAliment.Categorie = cbo_grp_alim.SelectionBoxItem.ToString();
            nouvelAliment.Mesure = int.Parse(Mesure.Text);
            nouvelAliment.UniteMesure = cbo_unite_mesure.SelectionBoxItem.ToString();
            nouvelAliment.Energie = double.Parse(Calories.Text);
            nouvelAliment.Glucide = double.Parse(Glucides.Text);
            nouvelAliment.Fibre = double.Parse(Fibres.Text);
            nouvelAliment.Proteine = double.Parse(Proteines.Text);
            nouvelAliment.Lipide = double.Parse(Lipides.Text);
            nouvelAliment.Cholesterol = double.Parse(Cholesterol.Text);
            nouvelAliment.Sodium = double.Parse(Sodium.Text);

            //----------------------------------------Enregistrement du chemin de l'image----------------------------------------
            string chemin = img_alim.Source.ToString();
            int position = chemin.LastIndexOf('/');
            string image = chemin.Substring(position + 1);
            string actuel;

            int positionPack = chemin.IndexOf("pack");

            if (positionPack == -1)
            {
                chemin = chemin.Substring(8);
                actuel = Directory.GetCurrentDirectory();
                int positionDest = actuel.LastIndexOf('\\');
                actuel = actuel.Substring(0, actuel.Length - (actuel.Length - positionDest));
                positionDest = actuel.LastIndexOf('\\');
                actuel = actuel.Substring(0, actuel.Length - (actuel.Length - positionDest));
                actuel += "\\UI\\Images\\" + image;

                if (!File.Exists(actuel))
                {
                    System.IO.File.Copy(chemin, actuel);
                }

                nouvelAliment.ImageURL = "pack://application:,,,/UI/Images/" + image;
            }
            else
            {
                nouvelAliment.ImageURL = chemin;
            }

            //----------------------------------------Insertion en BD----------------------------------------
            ServiceFactory.Instance.GetService<IAlimentService>().Insert(nouvelAliment);
            listeAliments = ServiceFactory.Instance.GetService<IAlimentService>().RetrieveAll();

            //----------------------------------------Remise à neuf des champs de saisie----------------------------------------
            Nom_alim.Text = "";
            cbo_grp_alim.SelectedIndex = -1;
            cbo_unite_mesure.SelectedIndex = -1;
            Mesure.Text = "";
            Calories.Text = "";
            Glucides.Text = "";
            Fibres.Text = "";
            Proteines.Text = "";
            Lipides.Text = "";
            Cholesterol.Text = "";
            Sodium.Text = "";
            img_alim.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/nonDisponible.png"));
        }

        /// <summary>
        /// Méthode de modification dans la base de données d'un aliment existant.
        /// </summary>
        private void Modifier_Aliment()
        {
            Aliment modifAliment = new Aliment();

            //----------------------------------------Enregistrement des valeurs nutritionnelles----------------------------------------

            // Conversion automatisée de la première lettre du nom de l'aliment.
            if (char.IsLower(Nom_alim_modif.Text[0]))
            {
                Nom_alim_modif.Text = char.ToUpper(Nom_alim_modif.Text[0]) + Nom_alim_modif.Text.Substring(1);
            }

            modifAliment.IdAliment = idAlimentModif;
            modifAliment.Nom = Nom_alim_modif.Text;
            modifAliment.Categorie = cbo_grp_alim_modif.SelectionBoxItem.ToString();
            modifAliment.Mesure = int.Parse(Mesure_modif.Text);
            modifAliment.UniteMesure = cbo_unite_mesure_modif.SelectionBoxItem.ToString();
            modifAliment.Energie = double.Parse(Calories_modif.Text);
            modifAliment.Glucide = double.Parse(Glucides_modif.Text);
            modifAliment.Fibre = double.Parse(Fibres_modif.Text);
            modifAliment.Proteine = double.Parse(Proteines_modif.Text);
            modifAliment.Lipide = double.Parse(Lipides_modif.Text);
            modifAliment.Cholesterol = double.Parse(Cholesterol_modif.Text);
            modifAliment.Sodium = double.Parse(Sodium_modif.Text);

            //----------------------------------------Enregistrement du chemin de l'image----------------------------------------
            string chemin = img_alim_modif.Source.ToString();
            int position = chemin.LastIndexOf('/');
            string image = chemin.Substring(position + 1);
            string actuel;

            int positionPack = chemin.IndexOf("pack");

            if (positionPack == -1)
            {
                chemin = chemin.Substring(8);
                actuel = Directory.GetCurrentDirectory();
                int positionDest = actuel.LastIndexOf('\\');
                actuel = actuel.Substring(0, actuel.Length - (actuel.Length - positionDest));
                positionDest = actuel.LastIndexOf('\\');
                actuel = actuel.Substring(0, actuel.Length - (actuel.Length - positionDest));
                actuel += "\\UI\\Images\\" + image;

                if (!File.Exists(actuel))
                {
                    System.IO.File.Copy(chemin, actuel);
                }

                modifAliment.ImageURL = "pack://application:,,,/UI/Images/" + image;
            }
            else
            {
                modifAliment.ImageURL = chemin;
            }

            //----------------------------------------Update en BD----------------------------------------
            ServiceFactory.Instance.GetService<IAlimentService>().Update(modifAliment);
            listeAliments = ServiceFactory.Instance.GetService<IAlimentService>().RetrieveAll();

            //----------------------------------------Remise à neuf des champs de saisie----------------------------------------
            Search_alim.Text = "";
            Nom_alim_modif.Text = "";
            cbo_grp_alim_modif.SelectedIndex = -1;
            cbo_unite_mesure_modif.SelectedIndex = -1;
            Mesure_modif.Text = "";
            Calories_modif.Text = "";
            Glucides_modif.Text = "";
            Fibres_modif.Text = "";
            Proteines_modif.Text = "";
            Lipides_modif.Text = "";
            Cholesterol_modif.Text = "";
            Sodium_modif.Text = "";
            img_alim_modif.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/nonDisponible.png"));
        }

        /// <summary>
        /// Méthode qui permet de chercher si un aliment existe dans la base de données.
        /// En cas de succès, les données de ce même aliments sont automatiquement chargées dans les différents champs de saisie et boîtes de sélection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recherche_Aliment(object sender, TextChangedEventArgs e)
        {
            bool recherche = false;

            for (int i = 0; i < listeAliments.Count; i++)
            {
                string alimentBD = "";
                string alimentText = "";


                for (int j = 0; j < listeAliments[i].Nom.Length; j++)
                {
                    if (char.IsLower(listeAliments[i].Nom[j]))
                    {
                        alimentBD += char.ToUpper(listeAliments[i].Nom[j]);
                    }
                    else
                    {
                        alimentBD += listeAliments[i].Nom[j];
                    }
                }

                for (int k = 0; k < Search_alim.Text.Length; k++)
                {
                    if (char.IsLower(Search_alim.Text[k]))
                    {
                        alimentText += char.ToUpper(Search_alim.Text[k]);
                    }
                    else
                    {
                        alimentText += Search_alim.Text[k];
                    }
                }

                // Vérification de l'existence de l'aliment par nom.
                if (alimentBD == alimentText)
                {
                    recherche = true;

                    idAlimentModif = (int)listeAliments[i].IdAliment;

                    Nom_alim_modif.Text = listeAliments[i].Nom;

                    // Sélection de la catégorie alimentaire correspondante à l'aliment.
                    switch (listeAliments[i].Categorie)
                    {
                        case "Boissons":
                            cbo_grp_alim_modif.SelectedIndex = 1;
                            break;
                        case "Arachides et noix":
                            cbo_grp_alim_modif.SelectedIndex = 2;
                            break;
                        case "Céréales":
                            cbo_grp_alim_modif.SelectedIndex = 3;
                            break;
                        case "Épices":
                            cbo_grp_alim_modif.SelectedIndex = 4;
                            break;
                        case "Fruits et légumes":
                            cbo_grp_alim_modif.SelectedIndex = 5;
                            break;
                        case "Produits laitiers":
                            cbo_grp_alim_modif.SelectedIndex = 6;
                            break;
                        case "Poissons et fruits de mers":
                            cbo_grp_alim_modif.SelectedIndex = 7;
                            break;
                        case "Pâtes":
                            cbo_grp_alim_modif.SelectedIndex = 8;
                            break;
                        case "Viandes et substituts":
                            cbo_grp_alim_modif.SelectedIndex = 9;
                            break;
                        case "Matières grasses":
                            cbo_grp_alim_modif.SelectedIndex = 10;
                            break;
                        case "Produits sucrés":
                            cbo_grp_alim_modif.SelectedIndex = 11;
                            break;
                        case "Autres":
                            cbo_grp_alim_modif.SelectedIndex = 12;
                            break;
                    }

                    // Sélection de l'unité de mesure correspondante à l'aliment.
                    switch (listeAliments[i].UniteMesure)
                    {
                        case "Millilitre":
                            cbo_unite_mesure_modif.SelectedIndex = 0;
                            break;
                        case "Gramme":
                            cbo_unite_mesure_modif.SelectedIndex = 1;
                            break;
                        case "Unité":
                            cbo_unite_mesure_modif.SelectedIndex = 2;
                            break;
                    }

                    // Remlissage des autres champs de saisie pour l'aliment désiré.
                    Mesure_modif.Text = listeAliments[i].Mesure.ToString();
                    Calories_modif.Text = listeAliments[i].Energie.ToString();
                    Glucides_modif.Text = listeAliments[i].Glucide.ToString();
                    Fibres_modif.Text = listeAliments[i].Fibre.ToString();
                    Proteines_modif.Text = listeAliments[i].Proteine.ToString();
                    Lipides_modif.Text = listeAliments[i].Lipide.ToString();
                    Cholesterol_modif.Text = listeAliments[i].Cholesterol.ToString();
                    Sodium_modif.Text = listeAliments[i].Sodium.ToString();

                    // Chargement et affichage de l'image d'un aliment si cell-ci existe.
                    if (new BitmapImage(new Uri(listeAliments[i].ImageURL)) != null)
                    {
                        img_alim_modif.Source = new BitmapImage(new Uri(listeAliments[i].ImageURL));
                    }
                    else
                    {
                        img_alim_modif.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/nonDisponible.png"));
                    }
                }
            }

            // Si aucun aliment ne correspond à la recherche, il faut s'assurer que les champs restent vides et l'on signale l'erreur à l'usager.
            if (!recherche)
            {
                Recherche_aliment.Foreground = Brushes.Red;
                Recherche_aliment.Content = "Rechercher un aliment (Auncun trouvé)";

                Nom_alim_modif.Text = "";
                cbo_grp_alim_modif.SelectedIndex = -1;
                cbo_unite_mesure_modif.SelectedIndex = -1;
                Mesure_modif.Text = "";
                Calories_modif.Text = "";
                Glucides_modif.Text = "";
                Fibres_modif.Text = "";
                Proteines_modif.Text = "";
                Lipides_modif.Text = "";
                Cholesterol_modif.Text = "";
                Sodium_modif.Text = "";
                img_alim_modif.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/nonDisponible.png"));
            }
            else
            {
                Recherche_aliment.Foreground = Brushes.DarkGreen;
                Recherche_aliment.Content = "Rechercher un aliment";
            }
        }

        #endregion

        /// <summary>
        /// Méthode qui permet de charger à l'écran une image pour un aliment ou un plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ajouter_Image(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string chemin = dlg.FileName;

                if (sender == btn_ajout_img_plat)
                {
                    img_plat.Source = new BitmapImage(new Uri(chemin));
                }

                if (sender == btn_ajout_img_alim)
                {
                    img_alim.Source = new BitmapImage(new Uri(chemin));
                }

                if (sender == btn_modif_img_alim)
                {
                    img_alim_modif.Source = new BitmapImage(new Uri(chemin));
                }
            }
        }


        #region InsertionModificationPlat

        /// <summary>
        /// Méthode de construction automatisé de l'accordéon des aliments pour la création et modification de plats.
        /// </summary>
        private void Construire_Accordeon()
        {
            Construire_Categorie_Accordeon("Arachides et noix");
            Construire_Categorie_Accordeon("Autres");
            Construire_Categorie_Accordeon("Boissons");
            Construire_Categorie_Accordeon("Céréales");
            Construire_Categorie_Accordeon("Épices");
            Construire_Categorie_Accordeon("Fruits et légumes");
            Construire_Categorie_Accordeon("Matières grasses");
            Construire_Categorie_Accordeon("Pâtes");
            Construire_Categorie_Accordeon("Poissons et fruits de mers");
            Construire_Categorie_Accordeon("Produits laitiers");
            Construire_Categorie_Accordeon("Produits sucrés");
            Construire_Categorie_Accordeon("Viandes et substituts");
        }

        /// <summary>
        /// Méthode d'ajout d'une catégorie d'aliments à l'accordéon et qui peuple cell-ci avec les bons aliments.
        /// </summary>
        /// <param name="categorie"></param>
        private void Construire_Categorie_Accordeon(string categorie)
        {
            AccordionItem categorieAccordeon = new AccordionItem();
            categorieAccordeon.Header = categorie;
            StackPanel stackCategorie = new StackPanel();
            stackCategorie.Background = Brushes.White;
            stackCategorie.Width = 284;

            foreach (Aliment unAliment in listeAliments)
            {
                if (unAliment.Categorie == categorie)
                {
                    Button btnAliment = Creer_Bouton_Aliment(true, unAliment);
                    stackCategorie.Children.Add(btnAliment);
                }
            }

            categorieAccordeon.Content = stackCategorie;
            accordeon_aliments.Items.Add(categorieAccordeon);
        }

        /// <summary>
        /// Méthode qui permet de créer le bouton d'ajout d'un aliment au plateau d'un plat.
        /// </summary>
        /// <param name="plus"></param>
        /// <param name="unAliment"></param>
        /// <returns></returns>
        Button Creer_Bouton_Aliment(bool plus, Aliment unAliment)
        {
            Button btnControl = new Button();

            // Création du bouton pour supprimer ou ajouter un Plat/Aliment
            btnControl.HorizontalContentAlignment = HorizontalAlignment.Left;
            Thickness margin = btnControl.Margin;
            margin.Left = 0;
            btnControl.Margin = margin;
            btnControl.Height = 32;
            btnControl.Name = "btn" + ((int)unAliment.IdAliment).ToString();

            if (plus)
            {
                //listeAlimentsPlateau.Add(unAliment);
                btnControl.Click += Ajout_Aliment_Plateau;

            }


            StackPanel stackLigne = new StackPanel();
            stackLigne.Orientation = Orientation.Horizontal;
            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
            stackLigne.Width = 275;

            Image imgBouton = new Image();
            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/" + (plus ? "plusIcon" : "minusIcon") + ".png"));
            imgBouton.Width = 15;
            imgBouton.Height = 15;
            stackLigne.Children.Add(imgBouton);

            Label lblNom = new Label();
            lblNom.Content = unAliment.Nom;
            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
            lblNom.FontSize = 12;
            lblNom.Width = 230;
            stackLigne.Children.Add(lblNom);

            btnControl.Content = stackLigne;

            return btnControl;
        }

        /// <summary>
        /// Méthode permettant de créer le bouton de retrait d'un aliment du plateau d'un plat.
        /// </summary>
        /// <param name="unBouton"></param>
        /// <param name="unAliment"></param>
        void Creer_Bouton_Retrait(Button unBouton, Aliment unAliment)
        {
            Button btnControl = new Button();

            // Création du bouton pour supprimer ou ajouter un Plat/Aliment
            btnControl.HorizontalContentAlignment = HorizontalAlignment.Left;
            Thickness margin = btnControl.Margin;
            margin.Left = 0;
            btnControl.Margin = margin;
            btnControl.Height = 32;
            btnControl.Name = unBouton.Name;
            btnControl.Click += Retirer_Aliment_Plateau;

            StackPanel stackLigne = new StackPanel();
            stackLigne.Orientation = Orientation.Horizontal;
            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
            stackLigne.Width = 275;

            Image imgBouton = new Image();
            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/minusIcon.png"));
            imgBouton.Width = 15;
            imgBouton.Height = 15;
            stackLigne.Children.Add(imgBouton);

            Label lblNom = new Label();
            lblNom.Content = unAliment.Nom;
            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
            lblNom.FontSize = 12;
            lblNom.Width = 230;
            stackLigne.Children.Add(lblNom);

            btnControl.Content = stackLigne;

            composition_plat.Children.Add(btnControl);
        }

        /// <summary>
        /// Méthode d'ajout d'un aliment au plateau d'un plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ajout_Aliment_Plateau(object sender, RoutedEventArgs e)
        {
            Button btnSender = sender as Button;
            int idButton = int.Parse(btnSender.Name.Substring(3));

            bool test = false;

            foreach (Aliment alimentVerif in listeAliments)
            {
                // Il faut trouver l'aliment dont l'id correspond à celle du bouton.
                if (idButton == alimentVerif.IdAliment)
                {
                    // Il faut ensuite vérifier si cet aliment existe déjà dans la liste d'aliments du plat.
                    foreach (Aliment alimVerif in listeAlimentsPlateau)
                    {
                        if (alimentVerif.IdAliment == alimVerif.IdAliment)
                        {
                            test = true;
                        }
                    }

                    // S'il n'est pas dans la composition de l'aliment, alors on l'ajoute.
                    if (!test)
                    {
                        listeAlimentsPlateau.Add(alimentVerif);
                        Creer_Bouton_Retrait(btnSender, alimentVerif);
                        alimentVerif.Quantite += 1;
                    }
                    // Sinon, il existe auquel cas il faut changer la quantité et modifier l'affichage du bouton.
                    else
                    {
                        alimentVerif.Quantite += 1;

                        foreach (Button btnPlateau in composition_plat.Children)
                        {
                            if (btnPlateau.Name == btnSender.Name)
                            {
                                StackPanel stackLigne = new StackPanel();
                                stackLigne.Orientation = Orientation.Horizontal;
                                stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
                                stackLigne.Width = 275;

                                Image imgBouton = new Image();
                                imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/minusIcon.png"));
                                imgBouton.Width = 15;
                                imgBouton.Height = 15;
                                stackLigne.Children.Add(imgBouton);

                                Label lblQuantite = new Label();
                                lblQuantite.Content = alimentVerif.Quantite.ToString() + "x";
                                lblQuantite.Style = (Style)(this.Resources["fontNutitia"]);
                                lblQuantite.FontSize = 12;
                                lblQuantite.Width = 40;
                                stackLigne.Children.Add(lblQuantite);

                                Label lblNom = new Label();
                                lblNom.Content = alimentVerif.Nom;
                                lblNom.Style = (Style)(this.Resources["fontNutitia"]);
                                lblNom.FontSize = 12;
                                lblNom.Width = 230;
                                stackLigne.Children.Add(lblNom);

                                btnPlateau.Content = stackLigne;
                                //btnPlateau.Click += Retirer_Aliment_Plateau;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode de retrait d'un aliment du plateau d'un plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Retirer_Aliment_Plateau(object sender, RoutedEventArgs e)
        {
            Button btnSender = sender as Button;
            int idButton = int.Parse(btnSender.Name.Substring(3));

            for (int i = 0; i < listeAlimentsPlateau.Count; i++)
            {
                if (listeAlimentsPlateau[i].IdAliment == idButton)
                {
                    listeAlimentsPlateau[i].Quantite -= 1;

                    if (listeAlimentsPlateau[i].Quantite == 0)
                    {
                        composition_plat.Children.Remove(btnSender);
                        listeAlimentsPlateau.Remove(listeAlimentsPlateau[i]);
                    }
                    else
                    {
                        if (listeAlimentsPlateau[i].Quantite == 1)
                        {
                            StackPanel stackLigne = new StackPanel();
                            stackLigne.Orientation = Orientation.Horizontal;
                            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
                            stackLigne.Width = 275;

                            Image imgBouton = new Image();
                            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/minusIcon.png"));
                            imgBouton.Width = 15;
                            imgBouton.Height = 15;
                            stackLigne.Children.Add(imgBouton);

                            Label lblNom = new Label();
                            lblNom.Content = listeAlimentsPlateau[i].Nom;
                            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
                            lblNom.FontSize = 12;
                            lblNom.Width = 230;
                            stackLigne.Children.Add(lblNom);

                            btnSender.Content = stackLigne;
                        }
                        else
                        {
                            StackPanel stackLigne = new StackPanel();
                            stackLigne.Orientation = Orientation.Horizontal;
                            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
                            stackLigne.Width = 275;

                            Image imgBouton = new Image();
                            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/minusIcon.png"));
                            imgBouton.Width = 15;
                            imgBouton.Height = 15;
                            stackLigne.Children.Add(imgBouton);

                            Label lblQuantite = new Label();
                            lblQuantite.Content = listeAlimentsPlateau[i].Quantite.ToString() + "x";
                            lblQuantite.Style = (Style)(this.Resources["fontNutitia"]);
                            lblQuantite.FontSize = 12;
                            lblQuantite.Width = 40;
                            stackLigne.Children.Add(lblQuantite);

                            Label lblNom = new Label();
                            lblNom.Content = listeAlimentsPlateau[i].Nom;
                            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
                            lblNom.FontSize = 12;
                            lblNom.Width = 230;
                            stackLigne.Children.Add(lblNom);

                            btnSender.Content = stackLigne;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Méthode permettant de valider le contenu des différents champs et de créer un nouveau plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider_Inserer_Plat(object sender, RoutedEventArgs e)
        {
            /*-----------------------------------Validation des différents champs de saisie-----------------------------------*/

            // Initialement, il n'y a aucune erreur, alors la valeur est à false.
            Erreur = false;

            Valider_Nom_plat(lbl_nom_plat, Nom_plat);
            Valider_Si_Type(lbl_type, cbo_type);
            Valider_Composition_Plat(compo_plat, listeAlimentsPlateau);

            /* S'il y a une seule erreur suite au validations, alors la variable booléenne sera à true et conséquement,
               il faut signaler à l'utilisateur qu'il y a un problème. */
            if (Erreur)
            {
                Erreur_Champ();
            }
            // Si tout c'est bien passé, alors on insère .
            else
            {
                Inserer_Plat();

                MessageBoxResult resultat;
                resultat = MessageBox.Show("Le nouveau plat a été correctement ajouté à la base de données."
                                            , "Ajout de plat réussie"
                                            , MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Méthode de validation du champ "Nom du plat".
        /// De plus, la longueur du nom doit être comprise entre 2 et 50 caractères.
        /// </summary>
        private void Valider_Nom_plat(Label unLabel, TextBox unTextBox)
        {
            /*-----------------------------------Vérifier que le nom n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(unTextBox.Text))
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = "Nom du plat (Champ vide)";
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le nom n'est pas trop grand-----------------------------------*/
                if (unTextBox.Text.Length > 50 || unTextBox.Text.Length < 2)
                {
                    unLabel.Foreground = Brushes.Red;
                    unLabel.Content = "Nom du plat (Entre 2 et 50 caractères)";
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le nom n'inclus que des caractères valides (A-z, 0-9, ', %, espaces et caractères accentués)-----------------------------------*/
                    Regex RgxNom = new Regex("[^A-z0-9ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÇçÌÍÎÏìíîïÙÚÛÜùúûüÿÑñ|'| |%]");
                    bool ContientSeulement = RgxNom.IsMatch(unTextBox.Text);

                    if (ContientSeulement)
                    {
                        unLabel.Foreground = Brushes.Red;
                        unLabel.Content = "Nom du plat (charctères invalides)";
                        Erreur = true;
                    }
                    else
                    {
                        Regex RgxApostropheDebut = new Regex("^['| ]");
                        bool ContientApostropheDebut = RgxApostropheDebut.IsMatch(unTextBox.Text);

                        if (ContientApostropheDebut)
                        {
                            unLabel.Foreground = Brushes.Red;
                            unLabel.Content = "Nom (Doit débuter avec une lettre)";
                            Erreur = true;
                        }
                        else
                        {
                            /*-----------------------------------Vérifier que le nom du plat est unique par rapport à la BD-----------------------------------*/
                            /* Afin de simplifier et tenir compte d'une multitudes de possibilités, les boucles suivantes convertissent les noms de plats en lettre majuscules. */
                            bool unique = true;

                            for (int i = 0; i < listePlats.Count; i++)
                            {
                                string platBD = "";
                                string platText = "";

                                for (int j = 0; j < listePlats[i].Nom.Length; j++)
                                {
                                    if (char.IsLower(listePlats[i].Nom[j]))
                                    {
                                        platBD += char.ToUpper(listePlats[i].Nom[j]);
                                    }
                                    else
                                    {
                                        platBD += listePlats[i].Nom[j];
                                    }
                                }

                                for (int k = 0; k < unTextBox.Text.Length; k++)
                                {
                                    if (char.IsLower(unTextBox.Text[k]))
                                    {
                                        platText += char.ToUpper(unTextBox.Text[k]);
                                    }
                                    else
                                    {
                                        platText += unTextBox.Text[k];
                                    }
                                }

                                if (platBD == platText)
                                {
                                    unLabel.Foreground = Brushes.Red;
                                    unLabel.Content = "Nom du plat (plat déjà existant)";
                                    Erreur = true;
                                    unique = false;
                                }

                            }

                            if (unique)
                            {
                                unLabel.Foreground = Brushes.DarkGreen;
                                unLabel.Content = "Nom du plat";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si un groupe alimentaire est sélectionné ou non.
        /// </summary>
        private void Valider_Si_Type(Label unLabel, ComboBox unComboBox)
        {
            if (unComboBox.SelectedIndex == -1)
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = "Type de plat (Aucun de sélectionné)";
                Erreur = true;
            }
            else
            {
                unLabel.Foreground = Brushes.DarkGreen;
                unLabel.Content = "Type de plat";
            }
        }

        /// <summary>
        /// Méthode permettant de valider si un plat est composé d'au moins un aliment.
        /// </summary>
        /// <param name="unGroupBox"></param>
        /// <param name="uneListeAliments"></param>
        private void Valider_Composition_Plat(GroupBox unGroupBox, IList<Aliment> uneListeAliments)
        {
            string text = unGroupBox.Header.ToString();

            if (uneListeAliments.Count == 0)
            {
                unGroupBox.Foreground = Brushes.Red;
                unGroupBox.Header = "Composition du plat (Auncun aliment)";
                Erreur = true;
            }
            else
            {
                unGroupBox.Foreground = Brushes.DarkGreen;
                unGroupBox.Header = "Composition du plat";
            }
        }

        /// <summary>
        /// Méthode appelant la validation du nom à chaque changement du champ de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nom_plat_TextChanged(object sender, TextChangedEventArgs e)
        {
            Valider_Nom_plat(lbl_nom_plat, Nom_plat);
        }

        /// <summary>
        /// Méthode d'insertion dans la base de données d'un nouvel aliment.
        /// </summary>
        private void Inserer_Plat()
        {
            Plat nouveauPlat = new Plat();

            //----------------------------------------Enregistrement des valeurs nutritionnelles----------------------------------------

            // Conversion automatisée de la première lettre du nom de l'aliment.
            if (char.IsLower(Nom_plat.Text[0]))
            {
                Nom_plat.Text = char.ToUpper(Nom_plat.Text[0]) + Nom_plat.Text.Substring(1);
            }

            nouveauPlat.Nom = Nom_plat.Text;
            nouveauPlat.Createur = App.MembreCourant.NomUtilisateur;
            nouveauPlat.TypePlat = cbo_type.SelectionBoxItem.ToString();

            if (Description.Text.Length == 0 || Description.Text == null)
            {
                nouveauPlat.Description = "Aucune description.";
            }
            else
            {
                nouveauPlat.Description = Description.Text;
            }

            nouveauPlat.ListeIngredients = listeAlimentsPlateau;

            //----------------------------------------Enregistrement du chemin de l'image----------------------------------------
            string chemin = img_plat.Source.ToString();
            int position = chemin.LastIndexOf('/');
            string image = chemin.Substring(position + 1);
            string actuel;

            int positionPack = chemin.IndexOf("pack");

            if (positionPack == -1)
            {
                chemin = chemin.Substring(8);
                actuel = Directory.GetCurrentDirectory();
                int positionDest = actuel.LastIndexOf('\\');
                actuel = actuel.Substring(0, actuel.Length - (actuel.Length - positionDest));
                positionDest = actuel.LastIndexOf('\\');
                actuel = actuel.Substring(0, actuel.Length - (actuel.Length - positionDest));
                actuel += "\\UI\\Images\\" + image;

                if (!File.Exists(actuel))
                {
                    System.IO.File.Copy(chemin, actuel);
                }

                nouveauPlat.ImageUrl = "pack://application:,,,/UI/Images/" + image;
            }
            else
            {
                nouveauPlat.ImageUrl = chemin;
            }

            //----------------------------------------Insertion en BD----------------------------------------
            ServiceFactory.Instance.GetService<IPlatService>().Insert(nouveauPlat);
            listePlats = ServiceFactory.Instance.GetService<IPlatService>().RetrieveAll();

            //----------------------------------------Remise à neuf des champs de saisie----------------------------------------
            Nom_plat.Text = "";
            cbo_type.SelectedIndex = -1;
            Description.Text = "";
            img_plat.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/nonDisponible.png"));
            listeAlimentsPlateau = new List<Aliment>();
            composition_plat.Children.Clear();
        }

        #endregion
    }
}