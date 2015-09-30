using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IMenuService
    {
        IList<Menu> RetrieveAll(RetrieveMenuArgs args);
        Menu Retrieve(RetrieveMenuArgs args);
    }
}
