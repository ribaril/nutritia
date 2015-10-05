using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Aliment
    {
        #region Proprietes
        public int? IdAliment { get; set; }
        public string Nom { get; set; }
        public string Categorie { get; set; }
        public int Mesure { get; set; }
        public UniteMesure Symbole { get; set; }
        public double Energie { get; set; }
        public double Glucide { get; set; }
        public double Fibre { get; set; }
        public double Proteine { get; set; }
        public double Lipide { get; set; }
        public double Cholesterol { get; set; }
        public double Sodium { get; set; }
        #endregion
    }
}
