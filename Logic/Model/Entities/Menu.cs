using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Nutritia
{
    public class Menu
    {
        #region Proprietes
        public int? IdMenu { get; set; }
        public string Nom { get; set; }
        public int NbPersonnes { get; set; }
        public DateTime DateCreation { get; set; }
        public IList<Plat> ListePlats { get; set; }
        #endregion

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public Menu()
        {
            DateCreation = DateTime.Now;
            ListePlats = new ObservableCollection<Plat>();
        }
    }
}
