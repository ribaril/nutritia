using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Preference
    {
        #region Proprietes
        public int? IdPreference { get; set; }
        public string Nom { get; set; }
        #endregion

        /// <summary>
        /// Méthode permettant de comparer deux objets Preference ensemble.
        /// Contrairement à la méthode de base, elle compare seulement avec le nom.
        /// </summary>
        /// <param name="objet"></param>
        /// <returns></returns>
        public override bool Equals(Object objet)
        {
            if (objet == null || GetType() != objet.GetType())
            {
                return false;
            }

            Preference preference = (Preference)objet;

            return (Nom == preference.Nom);
        }
    }
}
