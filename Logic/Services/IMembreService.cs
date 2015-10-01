using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IMembreService
    {
        IList<Membre> RetrieveAll();
        Membre Retrieve(RetrieveMembresArgs args);
    }
}
