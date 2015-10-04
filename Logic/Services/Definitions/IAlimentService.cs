using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IAlimentService
    {
        IList<Aliment> RetrieveAll();
        Aliment Retrieve(RetrieveAlimentArgs args);
    }
}
