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
    /// Logique d'interaction pour FenetreIngredients.xaml
    /// </summary>
    public partial class FenetreIngredients : Window
    {
        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        /// <param name="listeIngredients">La liste des ingrédients du plat.</param>
        public FenetreIngredients(Plat plat, int nbPersonnes)
        {
            InitializeComponent();

            GenererRangees(plat.ListeIngredients.Count + 1);

            Label entete = new Label();
            entete.FontSize = 18;
            entete.FontWeight = FontWeights.Bold;
            entete.Content = plat.Nom;
            grdIngredients.Children.Add(entete);

            AfficherIngredients(new List<Aliment>(plat.ListeIngredients), nbPersonnes);
        }

        /// <summary>
        /// Méthode permettant de générer les rangées de la grid.
        /// </summary>
        /// <param name="nbRangees">Le nombre de rangées.</param>
        private void GenererRangees(int nbRangees)
        {
            RowDefinition rowDefinition;

            for (int i = 0; i < nbRangees; i++)
            {
                rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;

                grdIngredients.RowDefinitions.Add(rowDefinition);
            }
        }

        /// <summary>
        /// Méthode permettant d'afficher des ingrédients.
        /// </summary>
        /// <param name="listeIngredients">La liste des aliments.</param>
        private void AfficherIngredients(List<Aliment> listeIngredients, int nbPersonnes)
        {
            for (int i = 0; i < listeIngredients.Count; i++)
            {
                Aliment alimentCourant = listeIngredients[i];

                StringBuilder sbIngredients = new StringBuilder();
                sbIngredients.Append(alimentCourant.Quantite * nbPersonnes).ToString();
                sbIngredients.Append(alimentCourant.UniteMesure);
                alimentCourant.Nom = alimentCourant.Nom.ToLower();
                if (alimentCourant.Nom[0] == 'a' || alimentCourant.Nom[0] == 'e' || alimentCourant.Nom[0] == 'h' || 
                    alimentCourant.Nom[0] == 'i' || alimentCourant.Nom[0] == 'o' || alimentCourant.Nom[0] == 'u')
                {
                    sbIngredients.Append(" d'");
                }
                else
                {
                    sbIngredients.Append(" de ");
                }
                sbIngredients.Append(alimentCourant.Nom);

                Label lblIngredients = new Label();
                lblIngredients.Content = sbIngredients.ToString();
                lblIngredients.ToolTip = GenererValeursNutritionnelles(alimentCourant, nbPersonnes);
                Grid.SetRow(lblIngredients, i + 1);

                grdIngredients.Children.Add(lblIngredients);
            }
        }

        /// <summary>
        /// Méthode permettant de générer les valeurs nutritionnelles d'un aliment dans un tooltip.
        /// </summary>
        /// <param name="aliment">Un aliment.</param>
        /// <returns>Un tooltip contenant les valeurs nutritionnelles de l'aliment.</returns>
        private ToolTip GenererValeursNutritionnelles(Aliment aliment, int nbPersonnes)
        {
            ToolTip ttValeurNut = new ToolTip();
            StackPanel spValeurNut = new StackPanel();

            Label lblEntete = new Label();
            lblEntete.Content = "Valeurs nutritionnelles";
            spValeurNut.Children.Add(lblEntete);

            double multiplicateurValeur = (aliment.Quantite * nbPersonnes)/aliment.Mesure;

            StringBuilder sbValeurNut = new StringBuilder();
            sbValeurNut.Append("Énergie : ").Append(aliment.Energie * multiplicateurValeur).AppendLine(" cal");
            sbValeurNut.Append("Glucides : ").Append(aliment.Glucide * multiplicateurValeur).AppendLine(" g");
            sbValeurNut.Append("Fibres : ").Append(aliment.Fibre * multiplicateurValeur).AppendLine(" g");
            sbValeurNut.Append("Protéines : ").Append(aliment.Proteine * multiplicateurValeur).AppendLine(" g");
            sbValeurNut.Append("Lipides : ").Append(aliment.Lipide * multiplicateurValeur).AppendLine(" g");
            sbValeurNut.Append("Cholestérol : ").Append(aliment.Cholesterol * multiplicateurValeur).AppendLine(" mg");
            sbValeurNut.Append("Sodium : ").Append(aliment.Sodium * multiplicateurValeur).Append(" mg");
            Label lblValeurNut = new Label();
            lblValeurNut.Content = sbValeurNut.ToString();

            spValeurNut.Children.Add(lblValeurNut);
            ttValeurNut.Content = spValeurNut;
            return ttValeurNut;
        }
    }
}
