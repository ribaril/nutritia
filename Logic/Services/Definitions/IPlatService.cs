using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IPlatService
    {
        // TODO : Implementer
        //IList<Plat> RetrieveAll();
        Plat Retrieve(RetrievePlatArgs args);
    }
}
