using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IPlatService
    {
        IList<Plat> RetrieveAll();
		IList<Plat> RetrieveSome(RetrievePlatArgs args);
        Plat Retrieve(RetrievePlatArgs args);
        IList<Aliment> RetrieveAlimentsPlat(RetrievePlatArgs args);
        void Update(Plat plat);
    }
}
