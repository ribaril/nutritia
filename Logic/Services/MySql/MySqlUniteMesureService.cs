using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class MySqlUniteMesureService : IUniteMesureService
    {
        private MySqlConnexion connexion;

        public IList<UniteMesure> RetrieveAll()
        {
            IList<UniteMesure> resultat = new List<UniteMesure>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM UnitesMesure";

                DataSet dataSetUnitesMesure = connexion.Query(requete);
                DataTable tableUnitesMesure = dataSetUnitesMesure.Tables[0];

                foreach (DataRow uniteMesure in tableUnitesMesure.Rows)
                {
                    resultat.Add(ConstruireUniteMesure(uniteMesure));
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;
        }

        public UniteMesure Retrieve(RetrieveUniteMesureArgs args)
        {
            throw new NotImplementedException();
        }

        private UniteMesure ConstruireUniteMesure(DataRow uniteMesure)
        {
            return new UniteMesure()
            {
                IdUniteMesure = (int)uniteMesure["idUniteMesure"],
                Symbole = (string)uniteMesure["symbole"]
            };
        }
    }
}
