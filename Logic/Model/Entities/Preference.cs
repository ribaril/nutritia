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
        public bool EstSelectionne { get; set; }
        #endregion
    }
}
