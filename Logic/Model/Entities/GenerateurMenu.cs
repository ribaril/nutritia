using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class GenerateurMenu
    {
        #region Proprietes
        public Membre Utilisateur { get; set; }
        public Menu MenuCourant { get; set; }
        public int NbrRepas { get; set; }
        public int NbrPersonnes { get; set; }
        #endregion
    }
}
