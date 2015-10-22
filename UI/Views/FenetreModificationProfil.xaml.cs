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
    /// Logique d'interaction pour ModificationProfil.xaml
    /// </summary>
    public partial class ModificationProfil : UserControl
    {
        public ModificationProfil()
        {
            InitializeComponent();

            /*-----------------------------------Intégration des données du membre connecté-----------------------------------*/
            
            Nom.Text = App.MembreCourant.Nom;
            Prenom.Text = App.MembreCourant.Prenom;
            Nom_utilisateur.Text = App.MembreCourant.NomUtilisateur;
            Vieux_mdp.Password = App.MembreCourant.MotPasse;
            Date_naissance.SelectedDate = App.MembreCourant.DateNaissance;
            Taille.SelectedValue = App.MembreCourant.Taille.ToString();
            Masse.SelectedValue = App.MembreCourant.Masse.ToString();

            /*-----------------------------------Intégration des restrictions alimentaires du membre connecté-----------------------------------*/
            
            #region restrictions

            if(App.MembreCourant.ListeRestrictions.Count != 0)
            {
                for(int i = 0; i < App.MembreCourant.ListeRestrictions.Count; i++)
                {
                    if(App.MembreCourant.ListeRestrictions[i].Nom == "Lactose")
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

                    if (App.MembreCourant.ListeObjectifs[i].Nom == "Contrôle de glycémie")
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
    }
}
