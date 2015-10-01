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
        public int Energie { get; set; }
        public int Glucide { get; set; }
        public int Fibre { get; set; }
        public int Proteine { get; set; }
        public int Lipide { get; set; }
        public int Cholesterol { get; set; }
        public int Sodium { get; set; }
        #endregion
    }
}
