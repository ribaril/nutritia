using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Nutritia
{
    public class MySqlMembreService : IMembreService
    {
        private MySqlConnexion connexion;
        private readonly IRestrictionAlimentaireService restrictionAlimentaireService;
        private readonly IObjectifService objectifService;
        private readonly IPreferenceService preferenceService;
        private readonly IMenuService menuService;

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public MySqlMembreService()
        {
            restrictionAlimentaireService = ServiceFactory.Instance.GetService<IRestrictionAlimentaireService>();
            objectifService = ServiceFactory.Instance.GetService<IObjectifService>();
            preferenceService = ServiceFactory.Instance.GetService<IPreferenceService>();
            menuService = ServiceFactory.Instance.GetService<IMenuService>();
        }

        /// <summary>
        /// Méthode permettant d'obtenir l'ensemble des membres sauvegardés dans la base de données.
        /// </summary>
        /// <returns>Une liste contenant les membres.</returns>
        public IList<Membre> RetrieveAll()
        {
            IList<Membre> resultat = new List<Membre>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Membres";

                DataSet dataSetMembres = connexion.Query(requete);
                DataTable tableMembres = dataSetMembres.Tables[0];

                // Construction de chaque objet Membre.
                foreach (DataRow rowMembre in tableMembres.Rows)
                {
                    Membre membre = ConstruireMembre(rowMembre);

                    // Ajout des restrictions alimentaires du membre.
                    requete = string.Format("SELECT idRestrictionAlimentaire FROM RestrictionsAlimentairesMembres WHERE idMembre = {0}", membre.IdMembre);

                    DataSet dataSetRestrictions = connexion.Query(requete);
                    DataTable tableRestrictions = dataSetRestrictions.Tables[0];

                    foreach (DataRow rowRestriction in tableRestrictions.Rows)
                    {
                        membre.ListeRestrictions.Add(restrictionAlimentaireService.Retrieve(new RetrieveRestrictionAlimentaireArgs { IdRestrictionAlimentaire = (int)rowRestriction["idRestrictionAlimentaire"] }));
                    }

                    // Ajout des objectifs du membre.
                    requete = string.Format("SELECT idObjectif FROM ObjectifsMembres WHERE idMembre = {0}", membre.IdMembre);

                    DataSet dataSetObjectifs = connexion.Query(requete);
                    DataTable tableObjectifs = dataSetObjectifs.Tables[0];

                    foreach (DataRow rowObjectif in tableObjectifs.Rows)
                    {
                        membre.ListeObjectifs.Add(objectifService.Retrieve(new RetrieveObjectifArgs { IdObjectif = (int)rowObjectif["idObjectif"] }));
                    }

                    // Ajout des préférences du membre.
                    requete = string.Format("SELECT idPreference FROM PreferencesMembres WHERE idMembre = {0}", membre.IdMembre);

                    DataSet dataSetPreferences = connexion.Query(requete);
                    DataTable tablePreferences = dataSetPreferences.Tables[0];

                    foreach (DataRow rowPreference in tablePreferences.Rows)
                    {
                        membre.ListePreferences.Add(preferenceService.Retrieve(new RetrievePreferenceArgs { IdPreference = (int)rowPreference["idPreference"] }));
                    }

                    membre.ListeMenus = menuService.RetrieveAll(new RetrieveMenuArgs { IdMembre = (int)membre.IdMembre });

                    resultat.Add(membre);

                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;
        }

        /// <summary>
        /// Méthode permettant d'obtenir un membre sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver le membre.</param>
        /// <returns>Un objet Membre.</returns>
        public Membre Retrieve(RetrieveMembreArgs args)
        {

            Membre membre;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Membres WHERE idMembre = {0}", args.IdMembre);

                DataSet dataSetMembres = connexion.Query(requete);
                DataTable tableMembres = dataSetMembres.Tables[0];

                // Construction de l'objet Membre.
                membre = ConstruireMembre(tableMembres.Rows[0]);

                // Ajout des restrictions alimentaires du membre.
                requete = string.Format("SELECT idRestrictionAlimentaire FROM RestrictionsAlimentairesMembres WHERE idMembre = {0}", membre.IdMembre);

                DataSet dataSetRestrictions = connexion.Query(requete);
                DataTable tableRestrictions = dataSetRestrictions.Tables[0];

                foreach (DataRow rowRestriction in tableRestrictions.Rows)
                {
                    membre.ListeRestrictions.Add(restrictionAlimentaireService.Retrieve(new RetrieveRestrictionAlimentaireArgs { IdRestrictionAlimentaire = (int)rowRestriction["idRestrictionAlimentaire"] }));
                }

                // Ajout des objectifs du membre.
                requete = string.Format("SELECT idObjectif FROM ObjectifsMembres WHERE idMembre = {0}", membre.IdMembre);

                DataSet dataSetObjectifs = connexion.Query(requete);
                DataTable tableObjectifs = dataSetObjectifs.Tables[0];

                foreach (DataRow rowObjectif in tableObjectifs.Rows)
                {
                    membre.ListeObjectifs.Add(objectifService.Retrieve(new RetrieveObjectifArgs { IdObjectif = (int)rowObjectif["idObjectif"] }));
                }

                // Ajout des préférences du membre.
                requete = string.Format("SELECT idPreference FROM PreferencesMembres WHERE idMembre = {0}", membre.IdMembre);

                DataSet dataSetPreferences = connexion.Query(requete);
                DataTable tablePreferences = dataSetPreferences.Tables[0];

                foreach (DataRow rowPreference in tablePreferences.Rows)
                {
                    membre.ListePreferences.Add(preferenceService.Retrieve(new RetrievePreferenceArgs { IdPreference = (int)rowPreference["idPreference"] }));
                }

                membre.ListeMenus = menuService.RetrieveAll(new RetrieveMenuArgs { IdMembre = (int)membre.IdMembre });

            }
            catch (MySqlException)
            {
                throw;
            }

            return membre;
        }
       
        /// <summary>
        /// Méthode permettant de construire un objet Membre.
        /// </summary>
        /// <param name="membre">Un enregistrement de la table Membres.</param>
        /// <returns>Un objet Membre.</returns>
        private Membre ConstruireMembre(DataRow membre)
        {
            return new Membre()
            {
                IdMembre = (int)membre["idMembre"],
                Nom = (string)membre["nom"],
                Prenom = (string)membre["prenom"],
                Taille = (double)membre["taille"],
                Masse = (double)membre["masse"],
                DateNaissance = (DateTime)membre["dateNaissance"],
                MotPasse = (string)membre["motPasse"],
                ListeRestrictions = new List<RestrictionAlimentaire>(),
                ListeObjectifs = new List<Objectif>(),
                ListePreferences = new List<Preference>(),
                ListeMenus = new List<Menu>(),
                EstAdministrateur = (bool)membre["estAdmin"],
                EstBanni = (bool)membre["estBanni"]
            };
        }
    }
}