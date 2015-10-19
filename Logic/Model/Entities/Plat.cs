using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Plat
    {
        #region Proprietes
        public int? IdPlat { get; set; }
        public string Createur { get; set; }
        public string Nom { get; set; }
        public string TypePlat { get; set; }
        public double? Note { get; set; }
        public string ImageUrl { get; set; }
        public IList<Aliment> ListeIngredients { get; set; }
        public bool EstActif { get; set; }
        #endregion
    }
}
