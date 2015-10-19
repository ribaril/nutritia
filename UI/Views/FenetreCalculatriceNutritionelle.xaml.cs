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
	/// Interaction logic for FenetreCalculatriceNutritionelle.xaml
	/// </summary>
	public partial class FenetreCalculatriceNutritionelle : UserControl
	{

		public List<Plat> PlateauPlat { get; set; }
		// Cet Aliment sert de totaux de tous les Aliments/Ingrédients du plateau
		public Aliment ValeurNutritionellePlateau { get; set; }
		public List<Aliment> PlateauAliment { get; set; }
		public List<Plat> LstPlat { get; set; }
		public SousEcran Plateau { get; set; }
		public SousEcran2 TabValeurNutritionelle { get; set; }

		public FenetreCalculatriceNutritionelle()
		{
			InitializeComponent();
			Plateau = new SousEcran();
			presenteurContenu2.Content = Plateau;
			TabValeurNutritionelle = new SousEcran2();
            presenteurContenu3.Content = TabValeurNutritionelle;
			LstPlat = new List<Plat>();
			PlateauPlat = new List<Plat>();
			PlateauAliment = new List<Aliment>();
			ValeurNutritionellePlateau = new Aliment();

			// Initialisation des valeurs nutritionelles du plateau
			ValeurNutritionellePlateau.Energie = 0;
			ValeurNutritionellePlateau.Glucide = 0;
			ValeurNutritionellePlateau.Fibre = 0;
			ValeurNutritionellePlateau.Proteine = 0;
			ValeurNutritionellePlateau.Lipide = 0;
			ValeurNutritionellePlateau.Cholesterol = 0;
			ValeurNutritionellePlateau.Sodium = 0;

			// TODO : A modifier quand les services mysql
			for (int i = 2; i < 23; i++)
				LstPlat.Add(ServiceFactory.Instance.GetService<IPlatService>().Retrieve(new RetrievePlatArgs { IdPlat = i }));

			// TODO : A modifier quand les services mysql
			foreach (var item in LstPlat)
			{
				item.ListeIngredients = new List<Aliment>();
				for (int i = 1; i < 10; i++)
					item.ListeIngredients.Add(ServiceFactory.Instance.GetService<IAlimentService>().Retrieve(new RetrieveAlimentArgs { IdAliment = i }));

			}

			// --------- Entrée -------------
			AccordionItem itemEntree = new AccordionItem();
			itemEntree.Header = "Entrée";
			StackPanel stackEntree = new StackPanel();
			stackEntree.Background = Brushes.White;
			stackEntree.Width = 284;

			// --------- Breuvage -------------
			AccordionItem itemBreuvage = new AccordionItem();
			itemBreuvage.Header = "Breuvage";
			StackPanel stackBreuvage = new StackPanel();
			stackBreuvage.Background = Brushes.White;
			stackBreuvage.Width = 284;

			// --------- Plat principal -------------
			AccordionItem itemPlatPrincipal = new AccordionItem();
			itemPlatPrincipal.Header = "Plat principaux";
			StackPanel stackPlatPrincipal = new StackPanel();
			stackPlatPrincipal.Background = Brushes.White;
			stackPlatPrincipal.Width = 284;

			// --------- Déssert -------------
			AccordionItem itemDessert = new AccordionItem();
			itemDessert.Header = "Déssert";
			StackPanel stackDessert = new StackPanel();
			stackDessert.Background = Brushes.White;
			stackDessert.Width = 284;

			// --------- Déjeuner -------------
			AccordionItem itemDejeuner = new AccordionItem();
			itemDejeuner.Header = "Déjeuner";
			StackPanel stackDejeuner = new StackPanel();
			stackDejeuner.Background = Brushes.White;
			stackDejeuner.Width = 284;

			// Dispatch des aliment dans les différentes parties de l'accordéon

			foreach (var plat in LstPlat)
			{

				StackPanel stackPlat = FormerListeBoutonLabel(true, plat);

				if (plat.TypePlat == "Entrée")
					stackEntree.Children.Add(stackPlat);
				else if (plat.TypePlat == "Breuvage")
					stackBreuvage.Children.Add(stackPlat);
				else if (plat.TypePlat == "Plat principal")
					stackPlatPrincipal.Children.Add(stackPlat);
				else if (plat.TypePlat == "Déssert")
					stackDessert.Children.Add(stackPlat);
				else if (plat.TypePlat == "Déjeuner")
					stackDejeuner.Children.Add(stackPlat);
			}

			itemDejeuner.Content = stackDejeuner;
			accPlat.Items.Add(itemDejeuner);

			itemBreuvage.Content = stackBreuvage;
			accPlat.Items.Add(itemBreuvage);

			itemEntree.Content = stackEntree;
			accPlat.Items.Add(itemEntree);

			itemPlatPrincipal.Content = stackPlatPrincipal;
			accPlat.Items.Add(itemPlatPrincipal);

			itemDessert.Content = stackDessert;
			accPlat.Items.Add(itemDessert);


		}

		/// <summary>
		/// Méthode qui génere deux StackPanel pour les plat et aliment dans le plateau
		/// </summary>
		private void DessinerPlateau()
		{
			Plateau = new SousEcran();
			presenteurContenu2.Content = Plateau;

			foreach (var plat in PlateauPlat)
			{
				Plateau.stackEcran.Children.Add(FormerListeBoutonLabel(false, plat));
			}

			foreach (var aliment in PlateauAliment)
			{
				Plateau.stackEcran.Children.Add(FormerListeBoutonLabel(false, aliment));
			}

			// On met à jour le tableau de valeur nutritionel
			CalculerValeurNutritionelle();

		}

		/// <summary>
		/// Supprime la ligne de Plat/Aliment si l'utilisateur clique sur le bouton
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnControlSupprimer_Click(object sender, RoutedEventArgs e)
		{
			Button btn = (Button)sender;

			foreach (var plat in LstPlat)
			{
				if (plat.IdPlat == Convert.ToInt32(btn.Uid))
					PlateauPlat.Remove(plat);
			}

			DessinerPlateau();
		}

		/// <summary>
		/// Ajoute la ligne de Plat/Aliment si l'utilisateur clique sur le bouton
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnControlAjout_Click(object sender, RoutedEventArgs e)
		{
			Button btn = (Button)sender;

			foreach (var plat in LstPlat)
			{
				if (plat.IdPlat == Convert.ToInt32(btn.Uid))
					PlateauPlat.Add(plat);
			}

			DessinerPlateau();
		}

		/// <summary>
		/// Génere une ligne de plat ou aliment 
		/// comportant son nom et son bouton pouvant l'Ajouter ou le supprimer
		/// </summary>
		/// <param name="plus">Parametre qui permet de determiner si la méthode
		/// est appelé pour faire une ligne avec un bouton plus ou un bouton moins</param>
		/// <param name="obj">Plat ou Aliment</param>
		/// <returns></returns>
		StackPanel FormerListeBoutonLabel(bool plus, Object obj)
		{
			StackPanel stackLigne = new StackPanel();
			Plat plat;
			Aliment aliment;
			stackLigne.Orientation = Orientation.Horizontal;
			bool EstPlat = false;


			if (obj.GetType().ToString() == "Nutritia.Plat")
                EstPlat = true;

            plat = (EstPlat ? (Plat)obj : null);
            aliment = (!EstPlat ? (Aliment)obj : null);

			// Création du bouton pour supprimer ou ajouter un Plat/Aliment
			Button btnControl = new Button();
			btnControl.Width = 20;
			btnControl.Height = 20;
			Thickness margin = btnControl.Margin;
			margin.Left = 5;
			btnControl.Margin = margin;
			Image imgBouton = new Image();
			imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/" + (plus ? "plusIcon" : "minusIcon") + ".png"));
			btnControl.Content = imgBouton;
			if (plus)
				btnControl.Click += BtnControlAjout_Click;
			else
				btnControl.Click += BtnControlSupprimer_Click;
            btnControl.Uid = (EstPlat ? plat.IdPlat : aliment.IdAliment).ToString();
			btnControl.Cursor = Cursors.Hand;
			stackLigne.Children.Add(btnControl);

			// Génération du Label comportant le nom du Plat/Aliment
			Label lblNom = new Label();
			lblNom.Style = (Style)(this.Resources["fontNutitia"]);
			lblNom.FontSize = 15;
			lblNom.Content = plat.Nom;
			stackLigne.Children.Add(lblNom);

			return stackLigne;
		}


        /// <summary>
        /// Méthode permettant de générer les valeurs nutritionnelles d'un aliment dans un tooltip. (Prise de Guillaume)
        /// </summary>
        /// <param name="item">Un aliment ou un plat dépendant du contexte de l'apelle</param>
        /// <returns>Un tooltip contenant les valeurs nutritionnelles de l'aliment.</returns>
        private ToolTip GenererValeursNutritionnelles(Object item)
        {
            ToolTip ttValeurNut = new ToolTip();
            StackPanel spValeurNut = new StackPanel();
            bool EstPlat = false;
            if (item.GetType().ToString() == "Nutritia.Plat")
            {
                item = (Plat)item;
                EstPlat = true;
            }
            
            else
            {
                item = (Aliment)item;
            }


            Label lblEntete = new Label();
            lblEntete.Content = "Valeurs nutritionnelles";
            spValeurNut.Children.Add(lblEntete);

            StringBuilder sbValeurNut = new StringBuilder();
            sbValeurNut.Append("Énergie : ").Append(aliment.Energie * aliment.Quantite).Append(" cal").AppendLine();
            sbValeurNut.Append("Glucides : ").Append(aliment.Glucide * aliment.Quantite).Append(" g").AppendLine();
            sbValeurNut.Append("Fibres : ").Append(aliment.Fibre * aliment.Quantite).Append(" g").AppendLine();
            sbValeurNut.Append("Protéines : ").Append(aliment.Proteine * aliment.Quantite).Append(" g").AppendLine();
            sbValeurNut.Append("Lipides : ").Append(aliment.Lipide * aliment.Quantite).Append(" g").AppendLine();
            sbValeurNut.Append("Cholestérol : ").Append(aliment.Cholesterol * aliment.Quantite).Append(" mg").AppendLine();
            sbValeurNut.Append("Sodium : ").Append(aliment.Sodium * aliment.Quantite).Append(" mg");
            Label lblValeurNut = new Label();
            lblValeurNut.Content = sbValeurNut.ToString();

            spValeurNut.Children.Add(lblValeurNut);
            ttValeurNut.Content = spValeurNut;
            return ttValeurNut;
        }


		void CalculerValeurNutritionelle()
		{
			ValeurNutritionellePlateau.Energie = 0;
			ValeurNutritionellePlateau.Glucide = 0;
			ValeurNutritionellePlateau.Fibre = 0;
			ValeurNutritionellePlateau.Proteine = 0;
			ValeurNutritionellePlateau.Lipide = 0;
			ValeurNutritionellePlateau.Cholesterol = 0;
			ValeurNutritionellePlateau.Sodium = 0;

			


			foreach (var Plat in PlateauPlat)
			{
				if (Plat.ListeIngredients != null)
					foreach (var Aliment in Plat.ListeIngredients)
					{
						// TODO : Convertir / 100g
						ValeurNutritionellePlateau.Energie += Aliment.Energie;
						ValeurNutritionellePlateau.Glucide += Aliment.Glucide;
						ValeurNutritionellePlateau.Fibre += Aliment.Fibre;
						ValeurNutritionellePlateau.Proteine += Aliment.Proteine;
						ValeurNutritionellePlateau.Lipide += Aliment.Lipide;
						ValeurNutritionellePlateau.Cholesterol += Aliment.Cholesterol;
						ValeurNutritionellePlateau.Sodium += Aliment.Sodium;
					}
			}

			foreach (var Aliment in PlateauAliment)
			{
				// TODO : Convertir / 100g
				ValeurNutritionellePlateau.Energie += Aliment.Energie;
				ValeurNutritionellePlateau.Glucide += Aliment.Glucide;
				ValeurNutritionellePlateau.Fibre += Aliment.Fibre;
				ValeurNutritionellePlateau.Proteine += Aliment.Proteine;
				ValeurNutritionellePlateau.Lipide += Aliment.Lipide;
				ValeurNutritionellePlateau.Cholesterol += Aliment.Cholesterol;
				ValeurNutritionellePlateau.Sodium += Aliment.Sodium;
			}

			

			DessinerTabValeurNutritionelle();

		}

		void DessinerTabValeurNutritionelle()
		{
			TabValeurNutritionelle = new SousEcran2();
			presenteurContenu3.Content = TabValeurNutritionelle;

			TabValeurNutritionelle.vEnegie.Inlines.Add(Math.Round(ValeurNutritionellePlateau.Energie,2).ToString("#########0.00"));
			TabValeurNutritionelle.vGlucide.Inlines.Add(Math.Round(ValeurNutritionellePlateau.Glucide, 2).ToString("##########0.00"));
            TabValeurNutritionelle.vFibre.Inlines.Add(Math.Round(ValeurNutritionellePlateau.Fibre, 2).ToString("##########0.00"));
			TabValeurNutritionelle.vProtein.Inlines.Add(Math.Round(ValeurNutritionellePlateau.Proteine, 2).ToString("##########0.00"));
			TabValeurNutritionelle.vLipide.Inlines.Add(Math.Round(ValeurNutritionellePlateau.Lipide, 2).ToString("##########0.00"));
			TabValeurNutritionelle.vCholesterol.Inlines.Add(Math.Round(ValeurNutritionellePlateau.Cholesterol, 2).ToString("##########0.00"));
			TabValeurNutritionelle.vSodium.Inlines.Add(Math.Round(ValeurNutritionellePlateau.Sodium, 2).ToString("##########0.00"));
			
		}


	}
}
