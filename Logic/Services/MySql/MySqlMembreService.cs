using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;

namespace Nutritia
{
    /// <summary>
    /// Service MySql lié aux Membres.
    /// </summary>
    public class MySqlMembreService : IMembreService
    {
        private readonly MySqlConnexion connexion;
        private readonly IRestrictionAlimentaireService restrictionAlimentaireService;
        private readonly IObjectifService objectifService;
        private readonly IPreferenceService preferenceService;
        private readonly IMenuService menuService;

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public MySqlMembreService()
            : this(new MySqlConnexion())
        {

        }

        public MySqlMembreService(MySqlConnexion mysqlConnexion)
        {
            connexion = mysqlConnexion;

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

                    membre.ListeMenus = menuService.RetrieveSome(new RetrieveMenuArgs { IdMembre = (int)membre.IdMembre });

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

            Membre membre = new Membre();

            try
            {
                string requete = string.Format("SELECT * FROM Membres WHERE idMembre = {0}", args.IdMembre);

                if (args.NomUtilisateur != null && args.NomUtilisateur != string.Empty)
                {
                    requete = string.Format("SELECT * FROM Membres WHERE nomUtilisateur = '{0}'", args.NomUtilisateur);
                }

                DataSet dataSetMembres = connexion.Query(requete);
                DataTable tableMembres = dataSetMembres.Tables[0];

                // Construction de l'objet Membre.
                if (tableMembres.Rows.Count != 0)
                {
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

                    membre.ListeMenus = menuService.RetrieveSome(new RetrieveMenuArgs { IdMembre = (int)membre.IdMembre });
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return membre;
        }

        /// <summary>
        /// Méthode permettant d'insérer un membre dans la base de données.
        /// </summary>
        /// <param name="membre">L'objet Membre a insérer.</param>
        public void Insert(Membre membre)
        {
            try
            {
                string requete = string.Format("INSERT INTO Membres (nom ,prenom, taille, masse, dateNaissance, nomUtilisateur, motPasse, estAdmin, estBanni, derniereMaj) VALUES ('{0}', '{1}', {2}, {3}, '{4}', '{5}', '{6}', {7}, {8}, '{9}')", membre.Nom, membre.Prenom, membre.Taille, membre.Masse, membre.DateNaissance.ToString("yyyy-MM-dd"), membre.NomUtilisateur, membre.MotPasse, membre.EstAdministrateur, membre.EstBanni, membre.DerniereMaj.ToString("yyyy-MM-dd HH:mm:ss"));
                connexion.Query(requete);

                int idMembre = (int)Retrieve(new RetrieveMembreArgs { NomUtilisateur = membre.NomUtilisateur }).IdMembre;

                // Ajout des restrictions alimentaires du membre.
                foreach (RestrictionAlimentaire restriction in membre.ListeRestrictions)
                {
                    requete = string.Format("INSERT INTO RestrictionsAlimentairesMembres (idRestrictionAlimentaire, idMembre) VALUES ({0}, {1})", restriction.IdRestrictionAlimentaire, idMembre);
                    connexion.Query(requete);
                }

                // Ajout des objectifs du membre.
                foreach (Objectif objectif in membre.ListeObjectifs)
                {
                    requete = string.Format("INSERT INTO ObjectifsMembres (idObjectif, idMembre) VALUES ({0}, {1})", objectif.IdObjectif, idMembre);
                    connexion.Query(requete);
                }

                // Ajout des préférences du membre.
                foreach (Preference preference in membre.ListePreferences)
                {
                    requete = string.Format("INSERT INTO PreferencesMembres (idPreference, idMembre) VALUES ({0}, {1})", preference.IdPreference, idMembre);
                    connexion.Query(requete);
                }
            }
            catch (MySqlException)
            {
                throw;
            }
        }


        /// <summary>
        /// Méthode permettant de mettre à jour un membre dans la base de données.
        /// </summary>
        /// <param name="membre">L'objet Membre à mettre à jour.</param>
        public void Update(Membre membre)
        {
            try
            {
                string requete = string.Format("UPDATE Membres SET nom = '{0}' ,prenom = '{1}', taille = {2}, masse = {3}, dateNaissance = '{4}', nomUtilisateur = '{5}', motPasse = '{6}', estAdmin = {7}, estBanni = {8}, derniereMaj = '{9}' WHERE idMembre = {10}", membre.Nom, membre.Prenom, membre.Taille, membre.Masse, membre.DateNaissance.ToString("yyyy-MM-dd"), membre.NomUtilisateur, membre.MotPasse, membre.EstAdministrateur, membre.EstBanni, membre.DerniereMaj.ToString("yyyy-MM-dd HH:mm:ss"), membre.IdMembre);

                connexion.Query(requete);

                string requeteEffacerRestrictions = string.Format("DELETE FROM RestrictionsAlimentairesMembres WHERE idMembre = {0}", membre.IdMembre);
                string requeteEffacerObjectifs = string.Format("DELETE FROM ObjectifsMembres WHERE idMembre = {0}", membre.IdMembre);
                string requeteEffacerPreferences = string.Format("DELETE FROM PreferencesMembres WHERE idMembre = {0}", membre.IdMembre);

                connexion.Query(requeteEffacerRestrictions);
                connexion.Query(requeteEffacerObjectifs);
                connexion.Query(requeteEffacerPreferences);

                // Ajout des restrictions alimentaires du membre.
                foreach (RestrictionAlimentaire restriction in membre.ListeRestrictions)
                {
                    requete = string.Format("INSERT INTO RestrictionsAlimentairesMembres (idRestrictionAlimentaire, idMembre) VALUES ({0}, {1})", restriction.IdRestrictionAlimentaire, membre.IdMembre);
                    connexion.Query(requete);
                }

                // Ajout des objectifs du membre.
                foreach (Objectif objectif in membre.ListeObjectifs)
                {
                    requete = string.Format("INSERT INTO ObjectifsMembres (idObjectif, idMembre) VALUES ({0}, {1})", objectif.IdObjectif, membre.IdMembre);
                    connexion.Query(requete);
                }

                // Ajout des préférences du membre.
                foreach (Preference preference in membre.ListePreferences)
                {
                    requete = string.Format("INSERT INTO PreferencesMembres (idPreference, idMembre) VALUES ({0}, {1})", preference.IdPreference, membre.IdMembre);
                    connexion.Query(requete);
                }
            }
            catch (MySqlException)
            {
                throw;
            }
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
				NomUtilisateur = (string)membre["nomUtilisateur"],
				MotPasse = (string)membre["motPasse"],
				ListeRestrictions = new List<RestrictionAlimentaire>(),
				ListeObjectifs = new List<Objectif>(),
				ListePreferences = new List<Preference>(),
				ListeMenus = new List<Menu>(),
				EstAdministrateur = (bool)membre["estAdmin"],
				EstBanni = (bool)membre["estBanni"],
                DerniereMaj = (DateTime)membre["derniereMaj"]
		    };
        }

        public IList<Membre> RetrieveAdmins()
        {
            IList<Membre> resultat = new List<Membre>();

            try
            {
                string requete = "SELECT * FROM Membres WHERE estAdmin = True";

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

                    membre.ListeMenus = menuService.RetrieveSome(new RetrieveMenuArgs { IdMembre = (int)membre.IdMembre });

                    resultat.Add(membre);

                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;
        }
    }
}