using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface ISuiviPlatService
    {
        IList<Plat> RetrieveSome(RetrieveSuiviPlatArgs args);
        void Insert(IList<Plat> listePlats, Membre membre);
        void Update(IList<Plat> listePlats, Membre membre);
    }
}
