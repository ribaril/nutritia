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
        // TODO : Fonctionne pas
        private MySqlConnexion connexion;

        public Plat Retrieve(RetrievePlatArgs args)
        {

            Plat plat;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Plats WHERE idPlat = {0}", args.IdPlat);

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                plat = ConstruirePlat(tablePlats.Rows[0]);

            }
            catch (Exception)
            {
                throw;
            }

            return plat;

        }

        private Plat ConstruirePlat(DataRow plat)
        {
            return new Plat()
            {
                // TODO : Améliorer
                IdPlat = (int)plat["idPlat"],
                // TODO : Améliorer
                Createur = new Membre(),
                Nom = (string)plat["nom"],
                Note = (int)plat["note"],
                // TODO : À voir pour le 'EstType'
                ListeIngredients = new List<Aliment>()
            };
        }
    }
}
