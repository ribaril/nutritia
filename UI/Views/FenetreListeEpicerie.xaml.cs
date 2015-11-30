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
using Infralution.Localization.Wpf;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour FenetreListeEpicerie.xaml
    /// </summary>
    public partial class FenetreListeEpicerie : UserControl
    {
        public static Menu MenuGenere { get; set; }
        public static int NbColonnesMenu { get; set; }
        public ObservableCollection<Aliment> ListeAliments { get; set; }

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreListeEpicerie(Menu menu, int nbColonnes)
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreListeEpicerie.Titre;

            MenuGenere = menu;
            NbColonnesMenu = nbColonnes;

            ListeAliments = new ObservableCollection<Aliment>();

            GenererListe();
            dgListeEpicerie.ItemsSource = ListeAliments;
            GenererListeConviviale();
        }

        /// <summary>
        /// Méthode permettant de générer la liste d'épicerie.
        /// </summary>
        private void GenererListe()
        {
            foreach (Plat platCourant in MenuGenere.ListePlats)
            {
                foreach (Aliment ingredientCourant in platCourant.ListeIngredients)
                {
                    // L'aliment doit être clôné puisque chaque plat qui contient cet aliment pointe sur le même.
                    Aliment ingredientCourantClone = (Aliment)ingredientCourant.Clone();
                    ingredientCourantClone.Quantite *= MenuGenere.NbPersonnes;

                    if(!ListeAliments.Contains(ingredientCourant))
                    {
                        ListeAliments.Add(ingredientCourantClone);
                    }
                    else
                    {
                        ListeAliments[ListeAliments.IndexOf(ingredientCourant)].Quantite += ingredientCourantClone.Quantite;
                    }
                }
            }
        }

        /// <summary>
        /// Méthode permettant de générer les rangées de la grid de la liste conviviale.
        /// </summary>
        private void GenererRangees()
        {
            RowDefinition rowDefinition;

            for (int i = 0; i < ListeAliments.Count; i++)
            {
                rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(30);

                grdListeConviviale.RowDefinitions.Add(rowDefinition);
            }
        }

        /// <summary>
        /// Méthode permettant de générer la liste d'épicerie conviviale.
        /// </summary>
        private void GenererListeConviviale()
        {
            grdListeConviviale.RowDefinitions.Clear();
            
            if(grdListeConviviale.Children.Count != 0)
            {
                grdListeConviviale.Children.RemoveRange(0, grdListeConviviale.Children.Count);
            }

            GenererRangees();

            Label lblArticle;

            for(int i = 0; i < ListeAliments.Count; i++)
            {
                StringBuilder sbArticle = new StringBuilder();
                sbArticle.Append(ListeAliments[i].Quantite);
                sbArticle.Append(ListeAliments[i].UniteMesure);
                ListeAliments[i].Nom = ListeAliments[i].Nom.ToLower();
                if (ListeAliments[i].Nom[0] == 'a' || ListeAliments[i].Nom[0] == 'e' || ListeAliments[i].Nom[0] == 'h' ||
                    ListeAliments[i].Nom[0] == 'i' || ListeAliments[i].Nom[0] == 'o' || ListeAliments[i].Nom[0] == 'u')
                {
                    sbArticle.Append(" d'");
                }
                else
                {
                    sbArticle.Append(" de ");
                }
                sbArticle.AppendLine(ListeAliments[i].Nom);

                lblArticle = new Label();
                lblArticle.Style = FindResource("fontNutritia") as Style;
                lblArticle.FontSize = 14;
                lblArticle.Content = sbArticle.ToString();
                Grid.SetRow(lblArticle, i);
                grdListeConviviale.Children.Add(lblArticle);
            }
        }

        /// <summary>
        /// Méthode permettant de supprimer un article de la liste d'épicerie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            Aliment alimentSelectionne = (Aliment)dgListeEpicerie.SelectedItem;

            ListeAliments.Remove(alimentSelectionne);
            GenererListeConviviale();
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreListeEpicerie.Titre;
        }
    }
}
