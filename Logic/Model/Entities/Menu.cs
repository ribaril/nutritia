using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Menu
    {
        #region Proprietes
        public int? IdMenu { get; set; }
        public DateTime DateCreation { get; set; }
        public IList<Plat> ListePlats { get; set; }
        #endregion
    }
}
