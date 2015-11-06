﻿using System;
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

        /// <summary>
        /// Méthode permettant d'obtenir l'ensemble des aliments sauvegardés dans la base de données.
        /// </summary>
        /// <returns>Une liste contenant les aliments.</returns>
        public IList<Aliment> RetrieveAll()
        {
            IList<Aliment> resultat = new List<Aliment>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Aliments a INNER JOIN CategoriesAlimentaires ca ON ca.idCategorieAlimentaire = a.idCategorieAlimentaire INNER JOIN UnitesMesure um ON um.idUniteMesure = a.idUniteMesure";

                DataSet dataSetAliments = connexion.Query(requete);
                DataTable tableAliments = dataSetAliments.Tables[0];

                foreach(DataRow rowAliment in tableAliments.Rows)
                {
                    requete = string.Format("SELECT quantite, valeurNutritionnelle FROM AlimentsValeursNutritionnelles avn INNER JOIN ValeursNutritionnelles vn ON avn.idValeurNutritionnelle = vn.idValeurNutritionnelle WHERE idAliment = {0}", (int)rowAliment["idAliment"]);

                    DataSet dataSetAlimentsValeursNut = connexion.Query(requete);
                    DataTable tableAlimentsValeursNut = dataSetAlimentsValeursNut.Tables[0];

                    Dictionary<string, double> valeurNut = new Dictionary<string, double>();

                    foreach (DataRow rowAlimentValeurNut in tableAlimentsValeursNut.Rows)
                    {
                        valeurNut.Add((string)rowAlimentValeurNut["valeurNutritionnelle"], (double)rowAlimentValeurNut["quantite"]);
                    }

                    resultat.Add(ConstruireAliment(rowAliment, valeurNut));
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

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Aliments a INNER JOIN CategoriesAlimentaires ca ON ca.idCategorieAlimentaire = a.idCategorieAlimentaire INNER JOIN UnitesMesure um ON um.idUniteMesure = a.idUniteMesure WHERE idAliment = {0}", args.IdAliment);

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
                Categorie = (string)aliment["categorieAlimentaire"],
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

        /// <summary>
        /// Méthode d'insertion d'un nouvel aliment dans la base de données.
        /// </summary>
        /// <param name="unAliment"></param>
        public void Insert(Aliment unAliment)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requeteCategoriesAlim = string.Format("SELECT * FROM CategoriesAlimentaires WHERE categorieAlimentaire = '{0}'", unAliment.Categorie);

                DataSet dataSetCategories = connexion.Query(requeteCategoriesAlim);
                DataTable tableCategories = dataSetCategories.Tables[0];

                int idCategorie = 0;

                foreach (DataRow rowCategories in tableCategories.Rows)
                {
                    idCategorie = (int)rowCategories["idCategorieAlimentaire"];
                }

                string requeteUnitesMesure = string.Format("SELECT * FROM UnitesMesure WHERE uniteMesure = '{0}'", unAliment.UniteMesure);

                DataSet dataSetUnites = connexion.Query(requeteUnitesMesure);
                DataTable tableUnites = dataSetUnites.Tables[0];

                int idUnite = 0;

                foreach (DataRow rowUnites in tableUnites.Rows)
                {
                    idUnite = (int)rowUnites["idUniteMesure"];
                }

                string requeteInsert = string.Format("INSERT INTO Aliments (idUniteMesure ,idCategorieAlimentaire, nom, mesure) VALUES ({0}, {1}, '{2}', {3})", idUnite, idCategorie, unAliment.Nom, unAliment.Mesure);
                connexion.Query(requeteInsert);

                string requeteAlim = string.Format("SELECT * FROM Aliments WHERE nom = '{0}'", unAliment.Nom);

                DataSet dataSetAlim = connexion.Query(requeteAlim);
                DataTable tableAlim = dataSetAlim.Tables[0];

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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Méthode permettant d'obtenir l'id pour une valeur nutritionnelle désirée.
        /// </summary>
        /// <param name="uneValeur"></param>
        /// <returns></returns>
        private int Associer_Valeur_Nutritionnelle(String uneValeur)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requeteValeur = string.Format("SELECT * FROM ValeursNutritionnelles WHERE valeurNutritionnelle = '{0}'", uneValeur);

                DataSet dataSetValeur = connexion.Query(requeteValeur);
                DataTable tableValeur = dataSetValeur.Tables[0];

                int idValeur = 0;

                foreach (DataRow rowValeur in tableValeur.Rows)
                {
                    idValeur = (int)rowValeur["idValeurNutritionnelle"];
                }

                return idValeur;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insertion d'une valeur nutritionnelle dans la table de correspondance "AlimentsValeursNutritionnelles".
        /// </summary>
        /// <param name="idValeur"></param>
        /// <param name="idAliment"></param>
        /// <param name="valeur"></param>
        private void Inserer_Valeur_Nutritionnelle(int idValeur, int idAliment, double valeur)
        {
            try
            {
                connexion = new MySqlConnexion();

                string uneValeur = valeur.ToString();

                if (uneValeur.Contains(","))
                {
                    uneValeur = uneValeur.Replace(",", ".");
                }

                string requeteInsert = string.Format("INSERT INTO AlimentsValeursNutritionnelles (idAliment ,idValeurNutritionnelle, quantite) VALUES ({0}, {1}, {2})", idAliment, idValeur, uneValeur);
                connexion.Query(requeteInsert);
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Méthode de mise à jour d'un aliment dans la base de données.
        /// </summary>
        /// <param name="unAliment"></param>
        public void Update(Aliment unAliment)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requeteCategoriesAlim = string.Format("SELECT * FROM CategoriesAlimentaires WHERE categorieAlimentaire = '{0}'", unAliment.Categorie);

                DataSet dataSetCategories = connexion.Query(requeteCategoriesAlim);
                DataTable tableCategories = dataSetCategories.Tables[0];

                int idCategorie = 0;

                foreach (DataRow rowCategories in tableCategories.Rows)
                {
                    idCategorie = (int)rowCategories["idCategorieAlimentaire"];
                }

                string requeteUnitesMesure = string.Format("SELECT * FROM UnitesMesure WHERE uniteMesure = '{0}'", unAliment.UniteMesure);

                DataSet dataSetUnites = connexion.Query(requeteUnitesMesure);
                DataTable tableUnites = dataSetUnites.Tables[0];

                int idUnite = 0;

                foreach (DataRow rowUnites in tableUnites.Rows)
                {
                    idUnite = (int)rowUnites["idUniteMesure"];
                }

                int idAliment = (int)unAliment.IdAliment;

                string requeteUpdate = string.Format("UPDATE Aliments SET idUniteMesure = {0}, idCategorieAlimentaire = {1}, nom = '{2}', mesure = {3} WHERE idAliment = {4}", idUnite, idCategorie, unAliment.Nom, unAliment.Mesure, unAliment.IdAliment);
                connexion.Query(requeteUpdate);

                int idEnergie = Associer_Valeur_Nutritionnelle("Calories");
                int idProteine = Associer_Valeur_Nutritionnelle("Protéines");
                int idGlucide = Associer_Valeur_Nutritionnelle("Glucides");
                int idFibre = Associer_Valeur_Nutritionnelle("Fibres");
                int idLipide = Associer_Valeur_Nutritionnelle("Lipides");
                int idSodium = Associer_Valeur_Nutritionnelle("Sodium");
                int idCholesterol = Associer_Valeur_Nutritionnelle("Cholestérol");

                Modifier_Valeur_Nutritionnelle(idEnergie, idAliment, unAliment.Energie);
                Modifier_Valeur_Nutritionnelle(idProteine, idAliment, unAliment.Proteine);
                Modifier_Valeur_Nutritionnelle(idGlucide, idAliment, unAliment.Glucide);
                Modifier_Valeur_Nutritionnelle(idFibre, idAliment, unAliment.Fibre);
                Modifier_Valeur_Nutritionnelle(idLipide, idAliment, unAliment.Lipide);
                Modifier_Valeur_Nutritionnelle(idSodium, idAliment, unAliment.Sodium);
                Modifier_Valeur_Nutritionnelle(idCholesterol, idAliment, unAliment.Cholesterol);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Méthode permettant de modifier une valeur nutritionnelle.
        /// </summary>
        /// <param name="idValeur"></param>
        /// <param name="idAliment"></param>
        /// <param name="valeur"></param>
        private void Modifier_Valeur_Nutritionnelle(int idValeur, int idAliment, double valeur)
        {
            try
            {
                connexion = new MySqlConnexion();

                string uneValeur = valeur.ToString();

                if (uneValeur.Contains(","))
                {
                    uneValeur = uneValeur.Replace(",", ".");
                }

                string requeteUpdate = string.Format("UPDATE AlimentsValeursNutritionnelles SET idValeurNutritionnelle = {0}, quantite = {1} WHERE idAliment = {2}", idValeur, uneValeur, idAliment);
                connexion.Query(requeteUpdate);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
