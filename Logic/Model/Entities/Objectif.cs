using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Objectif
    {
        #region Proprietes
        public int? IdObjectif { get; set; }
        public string Nom { get; set; }
        #endregion

        /// <summary>
        /// Méthode permettant de comparer deux objets Objectif ensemble.
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

            Objectif objectif = (Objectif)objet;

            return (Nom == objectif.Nom);
        }
    }
}
