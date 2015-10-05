using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class MySqlAlimentService : IAlimentService
    {
        private MySqlConnexion connexion;
  
        public IList<Aliment> RetrieveAll()
        {
            throw new NotImplementedException();
        }

        public Aliment Retrieve(RetrieveAlimentArgs args)
        {
            Aliment aliment;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Aliments WHERE idAliment = {0}", args.IdAliment);

                DataSet dataSetAliments = connexion.Query(requete);
                DataTable tableAliments = dataSetAliments.Tables[0];

                DataRow rowAliment = tableAliments.Rows[0];

                // TODO : Groupe alimentaire.
                requete = string.Format("SELECT * FROM GroupesAlimentaires WHERE idGroupeAlimentaire = {0}", (int)rowAliment["idGroupeAlimentaire"]);

                DataSet dataSetGroupesAlimentaires = connexion.Query(requete);
                DataTable tableGroupesAlimentaires = dataSetGroupesAlimentaires.Tables[0];

                DataRow rowGroupeAlimentaire = tableGroupesAlimentaires.Rows[0];

                // TODO : Voir pour services.
                requete = string.Format("SELECT * FROM UnitesMesure WHERE idUniteMesure = {0}", (int)rowAliment["idUniteMesure"]);

                DataSet dataSetUnitesMesure = connexion.Query(requete);
                DataTable tableUnitesMesure = dataSetUnitesMesure.Tables[0];

                DataRow rowUniteMesure = tableUnitesMesure.Rows[0];

                // TODO : VOIR pour les valeurs nut.
                requete = string.Format("SELECT quantite, valeurNutritionnelle FROM AlimentsValeursNutritionnelles avn INNER JOIN ValeursNutritionnelles vn ON avn.idValeurNutritionnelle = vn.idValeurNutritionnelle WHERE idAliment = {0}", (int)rowAliment["idAliment"]);

                DataSet dataSetAlimentsValeursNut = connexion.Query(requete);
                DataTable tableAlimentsValeursNut = dataSetAlimentsValeursNut.Tables[0];

                Dictionary<string, double> valeurNut = new Dictionary<string, double>();

                foreach (DataRow rowAlimentValeurNut in tableAlimentsValeursNut.Rows)
                {
                    valeurNut.Add((string)rowAlimentValeurNut["valeurNutritionnelle"], (double)rowAlimentValeurNut["quantite"]);
                }

                aliment = ConstruireAliment(rowAliment, rowGroupeAlimentaire, valeurNut);

            }
            catch (Exception)
            {
                throw;
            }

            return aliment;
        }


        private Aliment ConstruireAliment(DataRow aliment, DataRow groupeAlimentaire, Dictionary<string,double> valeurNut)
        {
            return new Aliment()
            {
                // TODO : améliorer.
                IdAliment = (int)aliment["idAliment"],
                Nom = (string)aliment["nom"],
                Categorie = (string)groupeAlimentaire["groupeAlimentaire"],
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
