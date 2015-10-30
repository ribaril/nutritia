using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IMenuService
    {
        IList<Menu> RetrieveSome(RetrieveMenuArgs args);
        Menu Retrieve(RetrieveMenuArgs args);
        void Insert(Menu menu);
    }
}
