using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    /// <summary>
    /// Service MySql lié aux Aliments.
    /// </summary>
    public class MySqlAlimentService : IAlimentService
    {
        private MySqlConnexion connexion;
  
        public IList<Aliment> RetrieveAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Méthode permettant d'obtenir un aliment sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver l'aliment.</param>
        /// <returns>Un objet Aliment.</returns>
        public Aliment Retrieve(RetrieveAlimentArgs args)
        {
            Aliment aliment;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Aliments a INNER JOIN groupesAlimentaires ga ON ga.idGroupeAlimentaire = a.idGroupeAlimentaire INNER JOIN UnitesMesure um ON um.idUniteMesure = a.idUniteMesure WHERE idAliment = {0}", args.IdAliment);

                DataSet dataSetAliments = connexion.Query(requete);
                DataTable tableAliments = dataSetAliments.Tables[0];

                DataRow rowAliment = tableAliments.Rows[0];

                requete = string.Format("SELECT quantite, valeurNutritionnelle FROM AlimentsValeursNutritionnelles avn INNER JOIN ValeursNutritionnelles vn ON avn.idValeurNutritionnelle = vn.idValeurNutritionnelle WHERE idAliment = {0}", (int)rowAliment["idAliment"]);

                DataSet dataSetAlimentsValeursNut = connexion.Query(requete);
                DataTable tableAlimentsValeursNut = dataSetAlimentsValeursNut.Tables[0];

                Dictionary<string, double> valeurNut = new Dictionary<string, double>();

                foreach (DataRow rowAlimentValeurNut in tableAlimentsValeursNut.Rows)
                {
                    valeurNut.Add((string)rowAlimentValeurNut["valeurNutritionnelle"], (double)rowAlimentValeurNut["quantite"]);
                }

                aliment = ConstruireAliment(rowAliment, valeurNut);

            }
            catch (Exception)
            {
                throw;
            }

            return aliment;
        }

        /// <summary>
        /// Méthode permettant de construire un objet Aliment.
        /// </summary>
        /// <param name="aliment">Un enregistrement de la table Aliment.</param>
        /// <param name="valeurNut">Un dictionnaire des valeurs nutritionnelles de l'aliment.
        /// (Clé = Nom de la valeur nut. et valeur = Quantité de celle-ci).</param>
        /// <returns>Un objet Aliment.</returns>
        private Aliment ConstruireAliment(DataRow aliment, Dictionary<string,double> valeurNut)
        {
            return new Aliment()
            {
                IdAliment = (int)aliment["idAliment"],
                Nom = (string)aliment["nom"],
                Categorie = (string)aliment["groupeAlimentaire"],
                Mesure = (int)aliment["mesure"],
                UniteMesure = (string)aliment["symbole"],
                Energie = valeurNut["Calories"],
                Glucide = valeurNut["Glucides"],
                Fibre = valeurNut["Fibres"],
                Proteine = valeurNut["Protéines"],
                Lipide = valeurNut["Lipides"],
                Cholesterol = valeurNut["Cholestérol"],
                Sodium = valeurNut["Sodium"]
            };
        }
    }
}
