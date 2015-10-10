using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IPlatService
    {
        IList<Plat> RetrieveAll(RetrievePlatArgs args);
        Plat Retrieve(RetrievePlatArgs args);
    }
}
