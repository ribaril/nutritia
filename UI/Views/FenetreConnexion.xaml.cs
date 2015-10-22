using System;
using System.Collections.Generic;
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
    /// Interaction logic for FenetreConnexion.xaml
    /// </summary>
    public partial class FenetreConnexion : UserControl
    {
        public bool Erreur { get; set; }
        Membre membreValidation = new Membre();
        
        public FenetreConnexion()
        {
            InitializeComponent();
            Erreur = false;
        }

        private void btn_connexion(object sender, RoutedEventArgs e)
        {
            Erreur = false;

            Valider_Utilisateur();
            Valider_Mot_Passe();

            if(!Erreur)
            {
                // On sauvegarde les données du membre dans la variable de session seulement s'il n'est pas banni.
                if(membreValidation.EstBanni)
                {
                    MessageBoxResult resultat;
                    resultat = MessageBox.Show("Vous avez été banni(e), veuillez contacter un administrateur."
                                                , "Connexion impossible"
                                                , MessageBoxButton.OK
                                                , MessageBoxImage.Warning
                                                , MessageBoxResult.OK);
                }
                else
                {
                    App.MembreCourant = membreValidation;

                    // Redirection du membre en fonction de son statut d'administrateur.
                    if (App.MembreCourant.EstAdministrateur)
                    {
                        ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuAdministrateur());
                    }
                    else
                    {
                        ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new MenuPrincipalConnecte());
                    }
                }   
            }
        }

        /// <summary>
        /// Bouton permettant d'accéder à l'écran de création d'un compte.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_inscrire(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new CreationProfil());
        }

        /// <summary>
        /// Méthode de validation du champ "Nom d'utilisateur". Celui-ci doit exister dans la base de données.
        /// </summary>
        private void Valider_Utilisateur()
        {
            /*-----------------------------------Vérifier que le nom d'utilisateur n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(Nom_utilisateur.Text))
            {
                lbl_utilisateur.Foreground = Brushes.Red;
                lbl_utilisateur.Content = "Nom d'utilisateur (Champ vide)";

                Erreur = true;
            }
            else
            {
                membreValidation = ServiceFactory.Instance.GetService<IMembreService>().Retrieve(new RetrieveMembreArgs { NomUtilisateur = Nom_utilisateur.Text });

                if (membreValidation.NomUtilisateur == null)
                {
                    lbl_utilisateur.Foreground = Brushes.Red;
                    lbl_utilisateur.Content = "Utilisateur non-existant";
                    Erreur = true;
                }
                else
                {
                    lbl_utilisateur.Foreground = Brushes.DarkGreen;
                    lbl_utilisateur.Content = "Nom d'utilisateur";
                }
            }
        }

        /// <summary>
        /// Méthode de validation du mot de passe en fonction du nom d'utilisateur saisie.
        /// </summary>
        private void Valider_Mot_Passe()
        {
            /*-----------------------------------Vérifier que le mot de passe n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(Mot_passe.Password))
            {
                lbl_mdp.Foreground = Brushes.Red;
                lbl_mdp.Content = "Mot de passe (Champ vide)";
                Erreur = true;
            }
            else
            {
                // Un mot de passe est automatiquement erroné si le nom d'utilisateur n'est validé.
                if(membreValidation.NomUtilisateur == null)
                {
                    lbl_mdp.Foreground = Brushes.Red;
                    lbl_mdp.Content = "Mot de passe éronné";
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le mot de passe est le bon-----------------------------------*/
                    if (membreValidation.MotPasse != Mot_passe.Password)
                    {
                        lbl_mdp.Foreground = Brushes.Red;
                        lbl_mdp.Content = "Mot de passe éronné";
                        Erreur = true;
                    }
                    else
                    {
                        lbl_mdp.Foreground = Brushes.DarkGreen;
                        lbl_mdp.Content = "Mot de passe";
                    }
                }
            }
        }
    }
}
