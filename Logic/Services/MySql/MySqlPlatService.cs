using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class MySqlPlatService : IPlatService
    {
        private MySqlConnexion connexion;

        // TODO : Voir quoi faire avec le créateur et il manque les aliments.
        public Plat Retrieve(RetrievePlatArgs args)
        {

            Plat plat;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Plats WHERE idPlat = {0}", args.IdPlat);

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                DataRow rowPlat = tablePlats.Rows[0];

                // TODO : Pour le type.
                requete = string.Format("SELECT * FROM TypesPlats WHERE idTypePlat = {0}", (int)rowPlat["idTypePlat"]);

                DataSet dataSetTypes = connexion.Query(requete);
                DataTable tableTypes = dataSetTypes.Tables[0];

                DataRow rowType = tableTypes.Rows[0];

                plat = ConstruirePlat(rowPlat, rowType);

            }
            catch (Exception)
            {
                throw;
            }

            return plat;

        }

        private Plat ConstruirePlat(DataRow plat, DataRow type)
        {
            double? note = null;

            if (!(plat["note"] is DBNull))
            {
                note = (double?)plat["note"];
            }

            return new Plat()
            {
                IdPlat = (int)plat["idPlat"],
                // TODO Arranger sa.
                Createur = new Membre(),
                Nom = (string)plat["nom"],
                TypePlat = (string)type["typePlat"],
                Note = note
            };
        }
    }
}
