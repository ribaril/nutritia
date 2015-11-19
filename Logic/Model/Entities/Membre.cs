using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Membre
    {
        #region Proprietes
        public int? IdMembre { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public double Taille { get; set; }
        public double Masse { get; set; }
        public DateTime DateNaissance { get; set; }
        public string NomUtilisateur { get; set; }
        public string MotPasse { get; set; }
        public IList<RestrictionAlimentaire> ListeRestrictions { get; set; }
        public IList<Objectif> ListeObjectifs { get; set; }
        public IList<Preference> ListePreferences { get; set; }
        public IList<Menu> ListeMenus { get; set; }
        public bool EstConnecte { get; set; }
        public bool EstAdministrateur { get; set; }
        public bool EstBanni { get; set; }
        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - DateNaissance.Year;
                if (DateTime.Now.Month < DateNaissance.Month || (DateTime.Now.Month == DateNaissance.Month && DateTime.Now.Day < DateNaissance.Day))
                    age--;
                if (age < 0)
                    age = 0;
                return age;
            }
        }
        #endregion

		/// <summary>
		/// Méthode qui retourne une copie indépendente de cette objet
		/// </summary>
		/// <returns></returns>
		public Membre Cloner()
		{
			return (Membre)this.MemberwiseClone();
		}
    }
}
