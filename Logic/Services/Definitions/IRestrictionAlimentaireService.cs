using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IRestrictionAlimentaireService
    {
        IList<RestrictionAlimentaire> RetrieveAll();
        RestrictionAlimentaire Retrieve(RetrieveRestrictionAlimentaireArgs args);
    }
}
