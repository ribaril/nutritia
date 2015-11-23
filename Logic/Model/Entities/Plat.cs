using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Plat
    {
        #region Proprietes
        public int? IdPlat { get; set; }
        public string Createur { get; set; }
        public string Nom { get; set; }
        public string TypePlat { get; set; }
        public double? Note { get; set; }
        public string NoteConviviale { get; set; }
        public int NbVotes { get; set; }
        public string ImageUrl { get; set; }
        public IList<Aliment> ListeIngredients { get; set; }
        public bool EstActif { get; set; }
        #endregion

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public Plat()
        {
            ListeIngredients = new List<Aliment>();
        }

        /// <summary>
        /// Méthode permettant de déterminer la note du plat de façon plus conviviale.
        /// Elle se base sur la note en chiffre du plat.
        /// </summary>
        public void DeterminerNoteConviviale()
        {
            if (Note == null) { NoteConviviale = "Aucune"; }
            else if (Note == 1) { NoteConviviale = "Mauvais"; }
            else if (Note > 1 && Note < 1.5) { NoteConviviale = "Mauvais +"; }
            else if (Note >= 1.5 && Note < 2) { NoteConviviale = "Passable -"; }
            else if (Note == 2) { NoteConviviale = "Passable"; }
            else if (Note > 2 && Note < 2.5) { NoteConviviale = "Passable +"; }
            else if (Note >= 2.5 && Note < 3) { NoteConviviale = "Moyen -"; }
            else if (Note == 3) { NoteConviviale = "Moyen"; }
            else if (Note > 3 && Note < 3.5) { NoteConviviale = "Moyen +"; }
            else if (Note >= 3.5 && Note < 4) { NoteConviviale = "Succulent -"; }
            else if (Note == 4) { NoteConviviale = "Succulent"; }
            else if (Note > 4 && Note < 4.5) { NoteConviviale = "Succulent +"; }
            else if (Note >= 4.5 && Note < 5) { NoteConviviale = "Divin -"; }
            else if (Note == 5) { NoteConviviale = "Divin"; }
        }

        /// <summary>
        /// Méthode permettant de calculer les valeurs nutritionnelles d'un plat.
        /// </summary>
        /// <param name="plat">Le plat à analyser.</param>
        /// <returns>Un dictionnaire contenant les valeurs nutritionnelles du plat.</returns>
        public Dictionary<string, double> CalculerValeursNutritionnelles()
        {
            // Initialisation du dictionnaire.
            Dictionary<string, double> dictionnaireValeursNut = new Dictionary<string, double>();

            dictionnaireValeursNut.Add("Energie", 0);
            dictionnaireValeursNut.Add("Glucide", 0);
            dictionnaireValeursNut.Add("Fibre", 0);
            dictionnaireValeursNut.Add("Proteine", 0);
            dictionnaireValeursNut.Add("Lipide", 0);
            dictionnaireValeursNut.Add("Cholesterol", 0);
            dictionnaireValeursNut.Add("Sodium", 0);

            foreach (Aliment alimentCourant in ListeIngredients)
            {
                dictionnaireValeursNut["Energie"] += alimentCourant.Energie * (alimentCourant.Quantite/alimentCourant.Mesure);
                dictionnaireValeursNut["Glucide"] += alimentCourant.Glucide * (alimentCourant.Quantite / alimentCourant.Mesure);
                dictionnaireValeursNut["Fibre"] += alimentCourant.Fibre * (alimentCourant.Quantite / alimentCourant.Mesure);
                dictionnaireValeursNut["Proteine"] += alimentCourant.Proteine * (alimentCourant.Quantite / alimentCourant.Mesure);
                dictionnaireValeursNut["Lipide"] += alimentCourant.Lipide * (alimentCourant.Quantite / alimentCourant.Mesure);
                dictionnaireValeursNut["Cholesterol"] += alimentCourant.Cholesterol * (alimentCourant.Quantite / alimentCourant.Mesure);
                dictionnaireValeursNut["Sodium"] += alimentCourant.Sodium * (alimentCourant.Quantite / alimentCourant.Mesure);
            }

            return dictionnaireValeursNut;
        }

        /// <summary>
        /// Méthode permettant d'obtenir une liste contenant les catégories de ses ingrédients.
        /// </summary>
        /// <returns>Une liste contenant les catégories.</returns>
        public List<string> ObtenirCategoriesIngredients()
        {
            List<string> categories = new List<string>();

            foreach (Aliment alimentCourant in ListeIngredients)
            {
                categories.Add(alimentCourant.Categorie);
            }

            return categories.Distinct().ToList<string>();
        }
    }
}
