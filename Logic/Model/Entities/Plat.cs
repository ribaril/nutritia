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
        public Membre Createur { get; set; }
        public string Nom { get; set; }
        public int Note { get; set; }
        public bool EstDejeuner { get; set; }
        public bool EstEntree { get; set; }
        public bool EstPrincipal { get; set; }
        public bool EstBreuvage { get; set; }
        public bool EstDessert { get; set; }
        public IList<Aliments> ListeIngredients { get; set; }
        public bool EstActif { get; set; }
        #endregion
    }
}
