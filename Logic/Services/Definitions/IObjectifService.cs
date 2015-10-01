using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IObjectifService
    {
        IList<Objectif> RetrieveAll();
        Objectif Retrieve(RetrieveObjectifArgs args);
    }
}
