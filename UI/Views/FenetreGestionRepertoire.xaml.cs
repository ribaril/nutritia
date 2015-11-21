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

        public GestionRepertoire()
        {
            InitializeComponent();

            Erreur = false;

            ModifierContexte();

            listeAliments = ServiceFactory.Instance.GetService<IAlimentService>().RetrieveAll();
        }

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

        #region InsertionAliment

        private void Valider_Inserer_Aliment(object sender, RoutedEventArgs e)
        {
            /*-----------------------------------Validation des différents champs de saisie-----------------------------------*/

            // Initialement, il n'y a aucune erreur, alors la valeur est à false.
            Erreur = false;

            Valider_Nom();
            Valider_Si_Groupe();
            Valider_Si_Unite();
            Valider_Mesure();
            Valider_Valeur_Nutritionnelle(lbl_calories, Calories);
            Valider_Valeur_Nutritionnelle(lbl_proteines, Proteines);
            Valider_Valeur_Nutritionnelle(lbl_glucides, Glucides);
            Valider_Valeur_Nutritionnelle(lbl_sodium, Sodium);
            Valider_Valeur_Nutritionnelle(lbl_fibres, Fibres);
            Valider_Valeur_Nutritionnelle(lbl_lipides, Lipides);
            Valider_Valeur_Nutritionnelle(lbl_cholesterol, Cholesterol);

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

                //ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
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
            Valider_Nom();
        }

        /// <summary>
        /// Méthode de validation du champ "Nom de l'aliment".
        /// De plus, la longueur du nom doit être comprise entre 1 et 50 caractères.
        /// </summary>
        private void Valider_Nom()
        {
            /*-----------------------------------Vérifier que le nom n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(Nom_alim.Text))
            {
                lbl_nom.Foreground = Brushes.Red;
                lbl_nom.Content = "Nom de l'aliment (Champ vide)";
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le nom n'est pas trop grand-----------------------------------*/
                if (Nom_alim.Text.Length > 50)
                {
                    lbl_nom.Foreground = Brushes.Red;
                    lbl_nom.Content = "Nom de l'aliment (Entre 1 et 50 caractères)";
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le nom n'inclus que des caractères valides (A-z, 0-9, ', %, espaces et caractères accentués)-----------------------------------*/
                    Regex RgxNom = new Regex("[^A-z0-9ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÇçÌÍÎÏìíîïÙÚÛÜùúûüÿÑñ|'| |%]");
                    bool ContientSeulement = RgxNom.IsMatch(Nom_alim.Text);

                    if (ContientSeulement)
                    {
                        lbl_nom.Foreground = Brushes.Red;
                        lbl_nom.Content = "Nom de l'aliment (charctères invalides)";
                        Erreur = true;
                    }
                    else
                    {
                        Regex RgxApostropheDebut = new Regex("^['| ]");
                        bool ContientApostropheDebut = RgxApostropheDebut.IsMatch(Nom_alim.Text);

                        if (ContientApostropheDebut)
                        {
                            lbl_nom.Foreground = Brushes.Red;
                            lbl_nom.Content = "Nom (Doit débuter avec une lettre)";
                            Erreur = true;
                        }
                        else
                        {
                            /*-----------------------------------Vérifier que le nom de l'aliment est unique par rapport à la BD-----------------------------------*/
                            bool unique = true;

                            for (int i = 0; i < listeAliments.Count; i++)
                            {
                                if (listeAliments[i].Nom == Nom_alim.Text)
                                {
                                    lbl_nom.Foreground = Brushes.Red;
                                    lbl_nom.Content = "Nom de l'aliment (aliment déjà existant)";
                                    Erreur = true;
                                    unique = false;
                                }
                            }

                            if (unique)
                            {
                                lbl_nom.Foreground = Brushes.DarkGreen;
                                lbl_nom.Content = "Nom de l'aliment";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si un groupe alimentaire est sélectionné ou non.
        /// </summary>
        private void Valider_Si_Groupe()
        {
            if (cbo_grp_alim.SelectedIndex == -1)
            {
                lbl_grp_alim.Foreground = Brushes.Red;
                lbl_grp_alim.Content = "Catégorie alimentaire (Aucun de sélectionné)";
                Erreur = true;
            }
            else
            {
                lbl_grp_alim.Foreground = Brushes.DarkGreen;
                lbl_grp_alim.Content = "Catégorie alimentaire";
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si un type d'unité de mesure est sélectionné ou non.
        /// </summary>
        private void Valider_Si_Unite()
        {
            if (cbo_unite_mesure.SelectedIndex == -1)
            {
                lbl_unite_mesure.Foreground = Brushes.Red;
                lbl_unite_mesure.Content = "Unité de mesure (Aucune de sélectionnée)";
                Erreur = true;
            }
            else
            {
                lbl_unite_mesure.Foreground = Brushes.DarkGreen;
                lbl_unite_mesure.Content = "Unité de mesure";
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si une mesure (taille de portion) est un entier strictement numérique compris entre 1 et 500.
        /// </summary>
        private void Valider_Mesure()
        {
            /*-----------------------------------Vérifier que le champ n'est pas vide-----------------------------------*/
            if (String.IsNullOrEmpty(Mesure.Text))
            {
                lbl_mesure.Foreground = Brushes.Red;
                lbl_mesure.Content = "Mesure de la portion (champ vide)";
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le champ est un entier numérique-----------------------------------*/
                Regex RgxChiffre = new Regex("[^0-9]");
                bool ContientSeulement = RgxChiffre.IsMatch(Mesure.Text);

                if (ContientSeulement)
                {
                    lbl_mesure.Foreground = Brushes.Red;
                    lbl_mesure.Content = "Mesure de la portion (entier numérique)";
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le champ est compris entre 1 et 500-----------------------------------*/
                    if (int.Parse(Mesure.Text) < 1 || int.Parse(Mesure.Text) > 500)
                    {
                        lbl_mesure.Foreground = Brushes.Red;
                        lbl_mesure.Content = "Mesure de la portion (1 à 500)";
                        Erreur = true;
                    }
                    else
                    {
                        lbl_mesure.Foreground = Brushes.DarkGreen;
                        lbl_mesure.Content = "Mesure de la portion";
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

                if(!File.Exists(actuel))
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

        #endregion

        /// <summary>
        /// Méthode qui permet de charger à l'écran une image pour l'aliment.
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

                img_alim.Source = new BitmapImage(new Uri(chemin));
            }
        }
    }
}