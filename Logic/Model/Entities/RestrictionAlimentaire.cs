using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class RestrictionAlimentaire
    {
        #region Proprietes
        public int? IdRestrictionAlimentaire { get; set; }
        public string Nom { get; set; }
        #endregion

        /// <summary>
        /// Méthode permettant de comparer deux objets RestrictionAlimentaire ensemble.
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

            RestrictionAlimentaire restriction = (RestrictionAlimentaire)objet;

            return (Nom == restriction.Nom);
        }
    }
}
