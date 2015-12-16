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
        /// <summary>
        /// Méthode permettant d'obtenir l'ensemble des aliments sauvegardés dans la base de données.
        /// </summary>
        /// <returns>Une liste contenant les aliments.</returns>
        public IList<Aliment> RetrieveAll()
        {
            IList<Aliment> resultat = new List<Aliment>();
            string requete = "SELECT * FROM Aliments a INNER JOIN CategoriesAlimentaires ca ON ca.idCategorieAlimentaire = a.idCategorieAlimentaire INNER JOIN UnitesMesure um ON um.idUniteMesure = a.idUniteMesure ORDER BY a.nom asc";

            try
            {
                using (MySqlConnexion connexion = new MySqlConnexion())
                using (DataSet dataSetAliments = connexion.Query(requete))
                using (DataTable tableAliments = dataSetAliments.Tables[0])
                {
                    foreach (DataRow rowAliment in tableAliments.Rows)
                    {
                        requete = string.Format("SELECT quantite, valeurNutritionnelle FROM AlimentsValeursNutritionnelles avn INNER JOIN ValeursNutritionnelles vn ON avn.idValeurNutritionnelle = vn.idValeurNutritionnelle WHERE idAliment = {0}", (int)rowAliment["idAliment"]);

                        using (DataSet dataSetAlimentsValeursNut = connexion.Query(requete))
                        using (DataTable tableAlimentsValeursNut = dataSetAlimentsValeursNut.Tables[0])
                        {

                            Dictionary<string, double> valeurNut = new Dictionary<string, double>();

                            foreach (DataRow rowAlimentValeurNut in tableAlimentsValeursNut.Rows)
                            {
                                valeurNut.Add((string)rowAlimentValeurNut["valeurNutritionnelle"], (double)rowAlimentValeurNut["quantite"]);
                            }

                            resultat.Add(ConstruireAliment(rowAliment, valeurNut));
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return resultat;
        }

        /// <summary>
        /// Méthode permettant d'obtenir un aliment sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver l'aliment.</param>
        /// <returns>Un objet Aliment.</returns>
        public Aliment Retrieve(RetrieveAlimentArgs args)
        {
            Aliment aliment;
            string requete = string.Format("SELECT * FROM Aliments a INNER JOIN CategoriesAlimentaires ca ON ca.idCategorieAlimentaire = a.idCategorieAlimentaire INNER JOIN UnitesMesure um ON um.idUniteMesure = a.idUniteMesure WHERE idAliment = {0}", args.IdAliment);

            try
            {
                using (MySqlConnexion connexion = new MySqlConnexion())
                using (DataSet dataSetAliments = connexion.Query(requete))
                using (DataTable tableAliments = dataSetAliments.Tables[0])
                {
                    DataRow rowAliment = tableAliments.Rows[0];

                    requete = string.Format("SELECT quantite, valeurNutritionnelle FROM AlimentsValeursNutritionnelles avn INNER JOIN ValeursNutritionnelles vn ON avn.idValeurNutritionnelle = vn.idValeurNutritionnelle WHERE idAliment = {0}", (int)rowAliment["idAliment"]);

                    using (DataSet dataSetAlimentsValeursNut = connexion.Query(requete))
                    using (DataTable tableAlimentsValeursNut = dataSetAlimentsValeursNut.Tables[0])
                    {
                        Dictionary<string, double> valeurNut = new Dictionary<string, double>();

                        foreach (DataRow rowAlimentValeurNut in tableAlimentsValeursNut.Rows)
                        {
                            valeurNut.Add((string)rowAlimentValeurNut["valeurNutritionnelle"], (double)rowAlimentValeurNut["quantite"]);
                        }

                        aliment = ConstruireAliment(rowAliment, valeurNut);
                    }
                }

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
        private Aliment ConstruireAliment(DataRow aliment, Dictionary<string, double> valeurNut)
        {
            return new Aliment()
            {
                IdAliment = (int)aliment["idAliment"],
                Nom = (string)aliment["nom"],
                Categorie = (string)aliment["categorieAlimentaire"],
                Mesure = (int)aliment["mesure"],
                UniteMesure = (string)aliment["uniteMesure"],
                Symbole = (string)aliment["symbole"],
                Energie = valeurNut["Calories"],
                Glucide = valeurNut["Glucides"],
                Fibre = valeurNut["Fibres"],
                Proteine = valeurNut["Protéines"],
                Lipide = valeurNut["Lipides"],
                Cholesterol = valeurNut["Cholestérol"],
                Sodium = valeurNut["Sodium"],
                ImageURL = (string)aliment["imageUrl"]
            };
        }

        /// <summary>
        /// Méthode d'insertion d'un nouvel aliment dans la base de données.
        /// </summary>
        /// <param name="unAliment">L'aliment à insérer dans la base de données.</param>
        public void Insert(Aliment unAliment)
        {
            string requeteCategoriesAlim = string.Format("SELECT * FROM CategoriesAlimentaires WHERE categorieAlimentaire = '{0}'", unAliment.Categorie);
            try
            {
                using (MySqlConnexion connexion = new MySqlConnexion())
                using (DataSet dataSetCategories = connexion.Query(requeteCategoriesAlim))
                using (DataTable tableCategories = dataSetCategories.Tables[0])
                {
                    int idCategorie = 0;

                    foreach (DataRow rowCategories in tableCategories.Rows)
                    {
                        idCategorie = (int)rowCategories["idCategorieAlimentaire"];
                    }

                    string requeteUnitesMesure = string.Format("SELECT * FROM UnitesMesure WHERE uniteMesure = '{0}'", unAliment.UniteMesure);

                    using (DataSet dataSetUnites = connexion.Query(requeteUnitesMesure))
                    using (DataTable tableUnites = dataSetUnites.Tables[0])
                    {
                        int idUnite = 0;

                        foreach (DataRow rowUnites in tableUnites.Rows)
                        {
                            idUnite = (int)rowUnites["idUniteMesure"];
                        }

                        string requeteInsert = string.Format("INSERT INTO Aliments (idUniteMesure ,idCategorieAlimentaire, nom, mesure, imageURL) VALUES ({0}, {1}, '{2}', {3}, '{4}')", idUnite, idCategorie, unAliment.Nom, unAliment.Mesure, unAliment.ImageURL);
                        connexion.Query(requeteInsert);

                        string requeteAlim = string.Format("SELECT * FROM Aliments WHERE nom = '{0}'", unAliment.Nom);

                        using (DataSet dataSetAlim = connexion.Query(requeteAlim))
                        using (DataTable tableAlim = dataSetAlim.Tables[0])
                        {
                            int idAliment = 0;

                            foreach (DataRow rowAlim in tableAlim.Rows)
                            {
                                idAliment = (int)rowAlim["idAliment"];
                            }

                            int idEnergie = Associer_Valeur_Nutritionnelle("Calories");
                            int idProteine = Associer_Valeur_Nutritionnelle("Protéines");
                            int idGlucide = Associer_Valeur_Nutritionnelle("Glucides");
                            int idFibre = Associer_Valeur_Nutritionnelle("Fibres");
                            int idLipide = Associer_Valeur_Nutritionnelle("Lipides");
                            int idSodium = Associer_Valeur_Nutritionnelle("Sodium");
                            int idCholesterol = Associer_Valeur_Nutritionnelle("Cholestérol");

                            Inserer_Valeur_Nutritionnelle(idEnergie, idAliment, unAliment.Energie);
                            Inserer_Valeur_Nutritionnelle(idProteine, idAliment, unAliment.Proteine);
                            Inserer_Valeur_Nutritionnelle(idGlucide, idAliment, unAliment.Glucide);
                            Inserer_Valeur_Nutritionnelle(idFibre, idAliment, unAliment.Fibre);
                            Inserer_Valeur_Nutritionnelle(idLipide, idAliment, unAliment.Lipide);
                            Inserer_Valeur_Nutritionnelle(idSodium, idAliment, unAliment.Sodium);
                            Inserer_Valeur_Nutritionnelle(idCholesterol, idAliment, unAliment.Cholesterol);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Méthode permettant d'obtenir l'id pour une valeur nutritionnelle désirée.
        /// </summary>
        /// <param name="uneValeur">Nom de la valeur nutritionnelle dont on veut l'id.</param>
        /// <returns>Un int contenant l'id de la valeur nutritionnelle désirée.</returns>
        private int Associer_Valeur_Nutritionnelle(String uneValeur)
        {
            int idValeur = 0;
            try
            {
                using (MySqlConnexion connexion = new MySqlConnexion())
                {

                    string requeteValeur = string.Format("SELECT * FROM ValeursNutritionnelles WHERE valeurNutritionnelle = '{0}'", uneValeur);

                    using (DataSet dataSetValeur = connexion.Query(requeteValeur))
                    using (DataTable tableValeur = dataSetValeur.Tables[0])
                    {

                        foreach (DataRow rowValeur in tableValeur.Rows)
                        {
                            idValeur = (int)rowValeur["idValeurNutritionnelle"];
                        }
                    }
                }
                return idValeur;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insertion d'une valeur nutritionnelle dans la table de correspondance "AlimentsValeursNutritionnelles".
        /// </summary>
        /// <param name="idValeur">l'id de la valeur nutritionnelle.</param>
        /// <param name="idAliment">L'id de l'aliment correspondant.</param>
        /// <param name="valeur">La valeur nutritionnelle.</param>
        private void Inserer_Valeur_Nutritionnelle(int idValeur, int idAliment, double valeur)
        {
            try
            {
                using (MySqlConnexion connexion = new MySqlConnexion())
                {
                    string uneValeur = valeur.ToString();

                    if (uneValeur.Contains(","))
                    {
                        uneValeur = uneValeur.Replace(",", ".");
                    }

                    string requeteInsert = string.Format("INSERT INTO AlimentsValeursNutritionnelles (idAliment ,idValeurNutritionnelle, quantite) VALUES ({0}, {1}, {2})", idAliment, idValeur, uneValeur);
                    connexion.Query(requeteInsert);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Méthode de mise à jour d'un aliment dans la base de données.
        /// </summary>
        /// <param name="unAliment">Aliment qu'il faut mettre à jour dans la base de données.</param>
        public void Update(Aliment unAliment)
        {
            string requeteCategoriesAlim = string.Format("SELECT * FROM CategoriesAlimentaires WHERE categorieAlimentaire = '{0}'", unAliment.Categorie);

            try
            {
                using (MySqlConnexion connexion = new MySqlConnexion())
                using (DataSet dataSetCategories = connexion.Query(requeteCategoriesAlim))
                using (DataTable tableCategories = dataSetCategories.Tables[0])
                {

                    int idCategorie = 0;

                    foreach (DataRow rowCategories in tableCategories.Rows)
                    {
                        idCategorie = (int)rowCategories["idCategorieAlimentaire"];
                    }

                    string requeteUnitesMesure = string.Format("SELECT * FROM UnitesMesure WHERE uniteMesure = '{0}'", unAliment.UniteMesure);

                    using (DataSet dataSetUnites = connexion.Query(requeteUnitesMesure))
                    using (DataTable tableUnites = dataSetUnites.Tables[0])
                    {
                        int idUnite = 0;

                        foreach (DataRow rowUnites in tableUnites.Rows)
                        {
                            idUnite = (int)rowUnites["idUniteMesure"];
                        }

                        string requeteUpdate = string.Format("UPDATE Aliments SET idUniteMesure = {0}, idCategorieAlimentaire = {1}, nom = '{2}', mesure = {3}, imageURL = '{4}' WHERE idAliment = {5}", idUnite, idCategorie, unAliment.Nom, unAliment.Mesure, unAliment.ImageURL, unAliment.IdAliment);
                        connexion.Query(requeteUpdate);

                        int idAliment = (int)unAliment.IdAliment;

                        int idEnergie = Associer_Valeur_Nutritionnelle("Calories");
                        int idProteine = Associer_Valeur_Nutritionnelle("Protéines");
                        int idGlucide = Associer_Valeur_Nutritionnelle("Glucides");
                        int idFibre = Associer_Valeur_Nutritionnelle("Fibres");
                        int idLipide = Associer_Valeur_Nutritionnelle("Lipides");
                        int idSodium = Associer_Valeur_Nutritionnelle("Sodium");
                        int idCholesterol = Associer_Valeur_Nutritionnelle("Cholestérol");

                        string requeteDelete = string.Format("DELETE FROM AlimentsValeursNutritionnelles WHERE idAliment = {0}", idAliment);
                        connexion.Query(requeteDelete);

                        Inserer_Valeur_Nutritionnelle(idEnergie, idAliment, unAliment.Energie);
                        Inserer_Valeur_Nutritionnelle(idProteine, idAliment, unAliment.Proteine);
                        Inserer_Valeur_Nutritionnelle(idGlucide, idAliment, unAliment.Glucide);
                        Inserer_Valeur_Nutritionnelle(idFibre, idAliment, unAliment.Fibre);
                        Inserer_Valeur_Nutritionnelle(idLipide, idAliment, unAliment.Lipide);
                        Inserer_Valeur_Nutritionnelle(idSodium, idAliment, unAliment.Sodium);
                        Inserer_Valeur_Nutritionnelle(idCholesterol, idAliment, unAliment.Cholesterol);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
