using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Aliment : ICloneable
    {
        #region Proprietes
        public int? IdAliment { get; set; }
        public string Nom { get; set; }
        public string Categorie { get; set; }
        public int Mesure { get; set; }
        public double Quantite { get; set; }
        public string UniteMesure { get; set; }
        public double Energie { get; set; }
        public double Glucide { get; set; }
        public double Fibre { get; set; }
        public double Proteine { get; set; }
        public double Lipide { get; set; }
        public double Cholesterol { get; set; }
        public double Sodium { get; set; }
        #endregion
    
        /// <summary>
        /// Méthode permettant de comparer deux objets Aliment ensemble.
        /// Contrairement à la méthode de base, elle compare seulement avec l'id.
        /// </summary>
        /// <param name="objet"></param>
        /// <returns></returns>
        public override bool Equals(Object objet)
        {
            if(objet == null || GetType() != objet.GetType())
            {
                return false;
            }

            Aliment aliment = (Aliment)objet;

            return (IdAliment == aliment.IdAliment);
        }

        /// <summary>
        ///  Méthode permettant de créer un clone d'un objet Aliment.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
