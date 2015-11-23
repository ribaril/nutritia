using System;
using System.Collections.Generic;
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
using Nutritia.UI.Ressources.Localisation;
using Infralution.Localization.Wpf;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour ModificationProfil.xaml
    /// </summary>
    public partial class ModificationProfil : UserControl
    {
        public bool Erreur { get; set; }

        public ModificationProfil()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = FenetreProfil.Titre;

            Erreur = false;

            /*-----------------------------------Intégration des données du membre connecté-----------------------------------*/

            Nom.Text = App.MembreCourant.Nom;
            Prenom.Text = App.MembreCourant.Prenom;
            Nom_utilisateur.Text = App.MembreCourant.NomUtilisateur;
            Mot_passe.Password = App.MembreCourant.MotPasse;
            Date_naissance.SelectedDate = App.MembreCourant.DateNaissance;
            Taille.SelectedValue = App.MembreCourant.Taille.ToString();
            Masse.SelectedValue = App.MembreCourant.Masse.ToString();

            /*-----------------------------------Intégration des restrictions alimentaires du membre connecté-----------------------------------*/

            #region restrictions

            if (App.MembreCourant.ListeRestrictions.Count != 0)
            {
                for (int i = 0; i < App.MembreCourant.ListeRestrictions.Count; i++)
                {
                    if (App.MembreCourant.ListeRestrictions[i].Nom == "Lactose")
                    {
                        Lactose.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeRestrictions[i].Nom == "Arachides et noix")
                    {
                        Noix.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeRestrictions[i].Nom == "Gluten")
                    {
                        Gluten.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeRestrictions[i].Nom == "Poissons et fruits de mers")
                    {
                        Rest_poissons_mer.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeRestrictions[i].Nom == "Diabète")
                    {
                        Diabete.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeRestrictions[i].Nom == "Cholestérol")
                    {
                        Cholesterol.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeRestrictions[i].Nom == "Haute/Basse pression")
                    {
                        Pression.IsChecked = true;
                    }
                }
            }

            #endregion

            /*-----------------------------------Intégration des objectifs du membre connecté-----------------------------------*/

            #region objectifs

            if (App.MembreCourant.ListeObjectifs.Count != 0)
            {
                for (int i = 0; i < App.MembreCourant.ListeObjectifs.Count; i++)
                {
                    if (App.MembreCourant.ListeObjectifs[i].Nom == "Perte de poids")
                    {
                        Perte_poids.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeObjectifs[i].Nom == "Gain de poids")
                    {
                        Gain_poids.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeObjectifs[i].Nom == "Gain musculaire")
                    {
                        Muscles.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeObjectifs[i].Nom == "Contrôle glycémique")
                    {
                        Glycemie.IsChecked = true;
                    }

                    if (App.MembreCourant.ListeObjectifs[i].Nom == "Contrôle du cholestérol")
                    {
                        Ctrl_cholesterol.IsChecked = true;
                    }
                }
            }

            #endregion

            /*-----------------------------------Intégration des préférences du membre connecté-----------------------------------*/

            #region preferences

            if (App.MembreCourant.ListePreferences.Count != 0)
            {
                for (int i = 0; i < App.MembreCourant.ListePreferences.Count; i++)
                {
                    if (App.MembreCourant.ListePreferences[i].Nom == "Végétarien")
                    {
                        Vegetarien.IsChecked = true;
                    }

                    if (App.MembreCourant.ListePreferences[i].Nom == "Végétalien")
                    {
                        Vegetalien.IsChecked = true;
                    }

                    if (App.MembreCourant.ListePreferences[i].Nom == "Viandes")
                    {
                        Viandes.IsChecked = true;
                    }

                    if (App.MembreCourant.ListePreferences[i].Nom == "Pâtes")
                    {
                        Pates.IsChecked = true;
                    }

                    if (App.MembreCourant.ListePreferences[i].Nom == "Poissons et fruits de mers")
                    {
                        Pref_poissons_mer.IsChecked = true;
                    }
                }
            }

            #endregion
        }

        private void Valider_Modifier(object sender, RoutedEventArgs e)
        {
            /*-----------------------------------Validation des différents champs de saisie-----------------------------------*/

            // Initialement, il n'y a aucune erreur, alors la valeur est à false.
            Erreur = false;

            Valider_Nom();
            Valider_Prenom();
            Valider_Utilisateur();
            Valider_Mot_Passe();

            //On ne valide la confirmation que si la mot de passe est différent de l'original.
            if (Mot_passe.Password != App.MembreCourant.MotPasse)
            {
                Valider_Confirmation_Mot_Passe();
            }

            Valider_Si_Taille();
            Valider_Si_Masse();
            Valider_Si_Date();

            /* S'il y a une seule erreur suite au validations, alors la variable booléenne sera à true et conséquement,
               il faut signaler à l'utilisateur qu'il y a un problème. */
            if (Erreur)
            {
                Erreur_Champ();
            }
            // Si tout c'est bien passé, alors on insère les données et passe au menu des membres.
            else
            {
                Modifier_Donnees();

                MessageBoxResult resultat;
                resultat = MessageBox.Show(FenetreProfil.MessageModificationSucces
                                            , FenetreProfil.MessageModificationSuccesTitre
                                            , MessageBoxButton.OK);

                ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
            }
        }

        /// <summary>
        /// Méthode affichant un message d'erreur en cas d'un ou plusieurs champs invalides.
        /// </summary>
        private void Erreur_Champ()
        {
            MessageBoxResult resultat;
            resultat = MessageBox.Show(FenetreProfil.MessageModificationErreur
                                        , FenetreProfil.MessageModificationErreurTitre
                                        , MessageBoxButton.OK
                                        , MessageBoxImage.Warning
                                        , MessageBoxResult.OK);
        }

        /// <summary>
        /// Méthode de validation du champ "Nom". Celui-ci ne peut contenir que des lettres et les caractères ' et -.
        /// De plus, la longueur du nom doit être comprise entre 1 et 25 caractères.
        /// </summary>
        private void Valider_Nom()
        {
            /*-----------------------------------Vérifier que le nom n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(Nom.Text))
            {
                lbl_nom.Foreground = Brushes.Red;
                lbl_nom.Content = FenetreProfil.NomVide;
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le nom n'est pas trop grand-----------------------------------*/
                if (Nom.Text.Length > 25)
                {
                    lbl_nom.Foreground = Brushes.Red;
                    lbl_nom.Content = FenetreProfil.NomLongueur;
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le nom n'inclus que des caractères valides (A-z - ')-----------------------------------*/
                    Regex RgxNom = new Regex("[^A-z|'|-]");
                    bool ContientSeulement = RgxNom.IsMatch(Nom.Text);

                    if (ContientSeulement)
                    {
                        lbl_nom.Foreground = Brushes.Red;
                        lbl_nom.Content = FenetreProfil.NomCaractereInvalide;
                        Erreur = true;
                    }
                    else
                    {
                        Regex RgxApostropheDebut = new Regex("^[']");
                        bool ContientApostropheDebut = RgxApostropheDebut.IsMatch(Nom.Text);

                        if (ContientApostropheDebut)
                        {
                            lbl_nom.Foreground = Brushes.Red;
                            lbl_nom.Content = FenetreProfil.NomDebutInvalide;
                            Erreur = true;
                        }
                        else
                        {
                            lbl_nom.Foreground = Brushes.DarkGreen;
                            lbl_nom.Content = FenetreProfil.NomGeneral;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode de validation du champ "Prenom". Celui-ci ne peut contenir que des lettres et les caractères ' et -.
        /// De plus, la longueur du prénom doit être comprise entre 1 et 25 charactères.
        /// </summary>
        private void Valider_Prenom()
        {
            /*-----------------------------------Vérifier que le prénom n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(Prenom.Text))
            {
                lbl_prenom.Foreground = Brushes.Red;
                lbl_prenom.Content = FenetreProfil.PrenomVide;
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le prénom n'est pas trop grand-----------------------------------*/
                if (Prenom.Text.Length > 25)
                {
                    lbl_prenom.Foreground = Brushes.Red;
                    lbl_prenom.Content = FenetreProfil.PrenomLongueur;
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le prénom n'inclus que des caractères valides (A-z - ')-----------------------------------*/
                    Regex RgxPrenom = new Regex("[^A-z|'|-]");
                    bool ContientSeulement = RgxPrenom.IsMatch(Prenom.Text);

                    if (ContientSeulement)
                    {
                        lbl_prenom.Foreground = Brushes.Red;
                        lbl_prenom.Content = FenetreProfil.PrenomCaractereInvalide;
                        Erreur = true;
                    }
                    else
                    {
                        Regex RgxApostropheDebut = new Regex("^[']");
                        bool ContientApostropheDebut = RgxApostropheDebut.IsMatch(Prenom.Text);

                        if (ContientApostropheDebut)
                        {
                            lbl_prenom.Foreground = Brushes.Red;
                            lbl_prenom.Content = FenetreProfil.PrenomDebutInvalide;
                            Erreur = true;
                        }
                        else
                        {
                            lbl_prenom.Foreground = Brushes.DarkGreen;
                            lbl_prenom.Content = FenetreProfil.PrenomGeneral;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode de validation du champ "Nom d'utilisateur". Celui-ci est un champ alphanumérique (lettres et chiffres uniquement).
        /// De plus, la longueur du nom d'utilisateur doit être comprise entre 2 et 15 charactères.
        /// </summary>
        private void Valider_Utilisateur()
        {
            /*-----------------------------------Vérifier que le nom d'utilisateur n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(Nom_utilisateur.Text))
            {
                lbl_utilisateur.Foreground = Brushes.Red;
                lbl_utilisateur.Content = FenetreProfil.UsernameVide;
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le nom d'utilisateur n'est pas trop petit ou grand-----------------------------------*/
                if (Nom_utilisateur.Text.Length < 2 || Nom_utilisateur.Text.Length > 15)
                {
                    lbl_utilisateur.Foreground = Brushes.Red;
                    lbl_utilisateur.Content = FenetreProfil.UsernameLongueur;
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le nom d'utilisateur n'inclus que des caractères valides (A-z 0-9)-----------------------------------*/
                    Regex RgxUtilisateur = new Regex("[^A-z0-9]");
                    bool ContientSeulement = RgxUtilisateur.IsMatch(Nom_utilisateur.Text);

                    if (ContientSeulement)
                    {
                        lbl_utilisateur.Foreground = Brushes.Red;
                        lbl_utilisateur.Content = FenetreProfil.UsernameCaractereInvalide;
                        Erreur = true;
                    }
                    else
                    {
                        /*-----------------------------------Vérifier que le nom d'utilisateur n'existe pas dans la BD seuelement s'il est différent de l'original-----------------------------------*/
                        Membre membreValidation = new Membre();
                        membreValidation = ServiceFactory.Instance.GetService<IMembreService>().Retrieve(new RetrieveMembreArgs { NomUtilisateur = Nom_utilisateur.Text });

                        if (membreValidation.NomUtilisateur != null && Nom_utilisateur.Text != App.MembreCourant.NomUtilisateur)
                        {
                            lbl_utilisateur.Foreground = Brushes.Red;
                            lbl_utilisateur.Content = FenetreProfil.UsernameUnique;
                            Erreur = true;
                        }
                        else
                        {
                            lbl_utilisateur.Foreground = Brushes.DarkGreen;
                            lbl_utilisateur.Content = FenetreProfil.UsernameGeneral;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode de validation du champ "Mot de passe". Celui-ci est un champ alphanumérique (lettres et chiffres uniquement).
        /// De plus, la longueur du mot de passe doit être comprise entre 2 et 10 charactères.
        /// </summary>
        private void Valider_Mot_Passe()
        {
            /*-----------------------------------Vérifier que le mot de passe n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(Mot_passe.Password))
            {
                lbl_mdp.Foreground = Brushes.Red;
                lbl_mdp.Content = FenetreProfil.MotPasseVide;
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le mot de passe n'est pas trop petit ou grand-----------------------------------*/
                if (Mot_passe.Password.Length < 2 || Mot_passe.Password.Length > 10)
                {
                    lbl_mdp.Foreground = Brushes.Red;
                    lbl_mdp.Content = FenetreProfil.MotpasseLongueur;
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le mot de passe n'inclus que des caractères valides (A-z 0-9)-----------------------------------*/
                    Regex RgxMotPasse = new Regex("[^A-z0-9]");
                    bool ContientSeulement = RgxMotPasse.IsMatch(Mot_passe.Password);

                    if (ContientSeulement)
                    {
                        lbl_mdp.Foreground = Brushes.Red;
                        lbl_mdp.Content = FenetreProfil.NomCaractereInvalide;
                        Erreur = true;
                    }
                    else
                    {
                        lbl_mdp.Foreground = Brushes.DarkGreen;
                        lbl_mdp.Content = FenetreProfil.MotPasseGeneral;
                    }
                }
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si le champ "Confirmation du mot de passe" est identique ou non au contenu du champ "Mot de passe".
        /// </summary>
        private void Valider_Confirmation_Mot_Passe()
        {
            if (Conf_mot_passe.Password != Mot_passe.Password)
            {
                lbl_conf_mdp.Foreground = Brushes.Red;
                lbl_conf_mdp.Content = FenetreProfil.MotPasseConfirmationDifferent;
                Erreur = true;
            }
            else
            {
                lbl_conf_mdp.Foreground = Brushes.DarkGreen;
                lbl_conf_mdp.Content = FenetreProfil.MotPasseConfirmationGeneral;
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si une taille est sélectionnée ou non.
        /// </summary>
        private void Valider_Si_Taille()
        {
            if (Taille.SelectedIndex == -1)
            {
                lbl_taille.Foreground = Brushes.Red;
                lbl_taille.Content = FenetreProfil.TailleVide;
                Erreur = true;
            }
            else
            {
                lbl_taille.Foreground = Brushes.DarkGreen;
                lbl_taille.Content = FenetreProfil.TailleGeneral;
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si une masse est sélectionnée ou non.
        /// </summary>
        private void Valider_Si_Masse()
        {
            if (Masse.SelectedIndex == -1)
            {
                lbl_masse.Foreground = Brushes.Red;
                lbl_masse.Content = FenetreProfil.MasseVide;
                Erreur = true;
            }
            else
            {
                lbl_masse.Foreground = Brushes.DarkGreen;
                lbl_masse.Content = FenetreProfil.MasseGeneral;
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si une date de naissance est sélectionnée ou non.
        /// </summary>
        private void Valider_Si_Date()
        {
            if (Date_naissance.SelectedDate == null)
            {
                lbl_date.Foreground = Brushes.Red;
                lbl_date.Content = FenetreProfil.DateNaissanceVide;
                Erreur = true;
            }
            else
            {
                lbl_date.Foreground = Brushes.DarkGreen;
                lbl_date.Content = FenetreProfil.DateNaissanceGeneral;
            }
        }

        /// <summary>
        /// Méthode qui gère l'insertion de données dans la variable de login MembreCourant ainsi que l'insertio en BD.
        /// </summary>
        private void Modifier_Donnees()
        {
            // Conversion automatisée de la première lettre du nom et du prénom en majuscule si nécessaire.
            if (char.IsLower(Nom.Text[0]))
            {
                Nom.Text = char.ToUpper(Nom.Text[0]) + Nom.Text.Substring(1);
            }

            if (char.IsLower(Prenom.Text[0]))
            {
                Prenom.Text = char.ToUpper(Prenom.Text[0]) + Prenom.Text.Substring(1);
            }

            App.MembreCourant.Nom = Nom.Text;
            App.MembreCourant.Prenom = Prenom.Text;
            App.MembreCourant.NomUtilisateur = Nom_utilisateur.Text;
            App.MembreCourant.Taille = double.Parse(Taille.SelectionBoxItem.ToString());
            App.MembreCourant.Masse = double.Parse(Masse.SelectionBoxItem.ToString());
            App.MembreCourant.MotPasse = Mot_passe.Password;
            App.MembreCourant.DateNaissance = (DateTime)Date_naissance.SelectedDate;

            /*-----------------------------------Restrictions alimentaires-----------------------------------*/

            #region restrictions

            IList<RestrictionAlimentaire> restrictions = new List<RestrictionAlimentaire>();
            IList<RestrictionAlimentaire> restrictionsMembre = new List<RestrictionAlimentaire>();
            restrictions = ServiceFactory.Instance.GetService<IRestrictionAlimentaireService>().RetrieveAll();

            if (Lactose.IsChecked == true)
            {
                for (int i = 0; i < restrictions.Count; i++)
                {
                    if (restrictions[i].Nom == "Lactose")
                    {
                        restrictionsMembre.Add(restrictions[i]);
                    }
                }
            }

            if (Noix.IsChecked == true)
            {
                for (int i = 0; i < restrictions.Count; i++)
                {
                    if (restrictions[i].Nom == "Arachides et noix")
                    {
                        restrictionsMembre.Add(restrictions[i]);
                    }
                }
            }

            if (Gluten.IsChecked == true)
            {
                for (int i = 0; i < restrictions.Count; i++)
                {
                    if (restrictions[i].Nom == "Gluten")
                    {
                        restrictionsMembre.Add(restrictions[i]);
                    }
                }
            }

            if (Rest_poissons_mer.IsChecked == true)
            {
                for (int i = 0; i < restrictions.Count; i++)
                {
                    if (restrictions[i].Nom == "Poissons et fruits de mers")
                    {
                        restrictionsMembre.Add(restrictions[i]);
                    }
                }
            }

            if (Diabete.IsChecked == true)
            {
                for (int i = 0; i < restrictions.Count; i++)
                {
                    if (restrictions[i].Nom == "Diabète")
                    {
                        restrictionsMembre.Add(restrictions[i]);
                    }
                }
            }

            if (Cholesterol.IsChecked == true)
            {
                for (int i = 0; i < restrictions.Count; i++)
                {
                    if (restrictions[i].Nom == "Cholestérol")
                    {
                        restrictionsMembre.Add(restrictions[i]);
                    }
                }
            }

            if (Pression.IsChecked == true)
            {
                for (int i = 0; i < restrictions.Count; i++)
                {
                    if (restrictions[i].Nom == "Haute/Basse pression")
                    {
                        restrictionsMembre.Add(restrictions[i]);
                    }
                }
            }

            App.MembreCourant.ListeRestrictions = restrictionsMembre;

            #endregion

            /*-----------------------------------Objectifs-----------------------------------*/

            #region objectifs

            IList<Objectif> objectifs = new List<Objectif>();
            IList<Objectif> objectifsMembre = new List<Objectif>();
            objectifs = ServiceFactory.Instance.GetService<IObjectifService>().RetrieveAll();

            if (Perte_poids.IsChecked == true)
            {
                for (int i = 0; i < objectifs.Count; i++)
                {
                    if (objectifs[i].Nom == "Perte de poids")
                    {
                        objectifsMembre.Add(objectifs[i]);
                    }
                }
            }

            if (Gain_poids.IsChecked == true)
            {
                for (int i = 0; i < objectifs.Count; i++)
                {
                    if (objectifs[i].Nom == "Gain de poids")
                    {
                        objectifsMembre.Add(objectifs[i]);
                    }
                }
            }

            if (Muscles.IsChecked == true)
            {
                for (int i = 0; i < objectifs.Count; i++)
                {
                    if (objectifs[i].Nom == "Gain musculaire")
                    {
                        objectifsMembre.Add(objectifs[i]);
                    }
                }
            }

            if (Glycemie.IsChecked == true)
            {
                for (int i = 0; i < objectifs.Count; i++)
                {
                    if (objectifs[i].Nom == "Contrôle glycémique")
                    {
                        objectifsMembre.Add(objectifs[i]);
                    }
                }
            }

            if (Ctrl_cholesterol.IsChecked == true)
            {
                for (int i = 0; i < objectifs.Count; i++)
                {
                    if (objectifs[i].Nom == "Contrôle du cholestérol")
                    {
                        objectifsMembre.Add(objectifs[i]);
                    }
                }
            }

            App.MembreCourant.ListeObjectifs = objectifsMembre;

            #endregion

            /*-----------------------------------Préférences-----------------------------------*/

            #region preferences

            IList<Preference> preferences = new List<Preference>();
            IList<Preference> preferencesMembre = new List<Preference>();
            preferences = ServiceFactory.Instance.GetService<IPreferenceService>().RetrieveAll();

            if (Vegetarien.IsChecked == true)
            {
                for (int i = 0; i < preferences.Count; i++)
                {
                    if (preferences[i].Nom == "Végétarien")
                    {
                        preferencesMembre.Add(preferences[i]);
                    }
                }
            }

            if (Vegetalien.IsChecked == true)
            {
                for (int i = 0; i < preferences.Count; i++)
                {
                    if (preferences[i].Nom == "Végétalien")
                    {
                        preferencesMembre.Add(preferences[i]);
                    }
                }
            }

            if (Viandes.IsChecked == true)
            {
                for (int i = 0; i < preferences.Count; i++)
                {
                    if (preferences[i].Nom == "Viandes")
                    {
                        preferencesMembre.Add(preferences[i]);
                    }
                }
            }

            if (Pates.IsChecked == true)
            {
                for (int i = 0; i < preferences.Count; i++)
                {
                    if (preferences[i].Nom == "Pâtes")
                    {
                        preferencesMembre.Add(preferences[i]);
                    }
                }
            }

            if (Pref_poissons_mer.IsChecked == true)
            {
                for (int i = 0; i < preferences.Count; i++)
                {
                    if (preferences[i].Nom == "Poissons et fruits de mers")
                    {
                        preferencesMembre.Add(preferences[i]);
                    }
                }
            }

            App.MembreCourant.ListePreferences = preferencesMembre;

            #endregion

            /*-----------------------------------Modifications dans la base de données-----------------------------------*/
            ServiceFactory.Instance.GetService<IMembreService>().Update(App.MembreCourant);
        }

        #region ObjectifsPreferences

        /// <summary>
        /// Méthode permettant de décocher l'option "Végétalien" si l'option "Végétarien" est cochée.
        /// Désactive les options "Viandes" et "Poissons et fruits de mer".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vegetarien_Checked(object sender, RoutedEventArgs e)
        {
            if (Vegetarien.IsChecked == true)
            {
                Vegetalien.IsChecked = false;

                Viandes.IsEnabled = false;
                Pref_poissons_mer.IsEnabled = false;
            }
        }

        /// <summary>
        /// Méthode permettant de décocher l'option "Végétarien" si l'option "Végétalien" est cochée.
        /// /// Désactive les options "Viandes" et "Poissons et fruits de mer".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vegetalien_Checked(object sender, RoutedEventArgs e)
        {
            if (Vegetalien.IsChecked == true)
            {
                Vegetarien.IsChecked = false;

                Viandes.IsEnabled = false;
                Pref_poissons_mer.IsEnabled = false;
            }
        }

        /// <summary>
        /// Méthode permettant de décocher les autres préférences si "Viandes" est cochée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Viandes_Checked(object sender, RoutedEventArgs e)
        {
            if (Viandes.IsChecked == true)
            {
                Pates.IsChecked = false;
                Pref_poissons_mer.IsChecked = false;
            }
        }

        /// <summary>
        /// Méthode permettant de décocher les autres préférences si "Pâtes" est cochée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pates_Checked(object sender, RoutedEventArgs e)
        {
            if (Pates.IsChecked == true)
            {
                Viandes.IsChecked = false;
                Pref_poissons_mer.IsChecked = false;
            }
        }

        /// <summary>
        /// Méthode permettant de décocher les autres préférences si "Poissons et fruits de mer" est cochée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pref_poissons_mer_Checked(object sender, RoutedEventArgs e)
        {
            if (Pref_poissons_mer.IsChecked == true)
            {
                Pates.IsChecked = false;
                Viandes.IsChecked = false;
            }
        }

        /// <summary>
        /// Méthode de réactivation des préférences "Viandes" et "Poissons et fruits de mer" 
        /// si le membre n'est pas végétarien ou végétalien.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vegetarien_Vegetalien_Dechoche(object sender, RoutedEventArgs e)
        {
            if (Vegetarien.IsChecked == false && Vegetalien.IsChecked == false)
            {
                Viandes.IsEnabled = true;
                Pref_poissons_mer.IsEnabled = true;
            }
        }

        /// <summary>
        /// Méthode permettant de décocher l'option "Gain de poids" si "Perte de poids" est cochée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Perte_poids_Checked(object sender, RoutedEventArgs e)
        {
            if (Perte_poids.IsChecked == true)
            {
                Gain_poids.IsChecked = false;
            }
        }

        /// <summary>
        /// Méthode permettant de décocher l'option "Perte de poids" si "Gain de poids" est cochée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gain_poids_Checked(object sender, RoutedEventArgs e)
        {
            if (Gain_poids.IsChecked == true)
            {
                Perte_poids.IsChecked = false;
            }
        }

        #endregion

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = FenetreProfil.Titre;
        }
    }
}
