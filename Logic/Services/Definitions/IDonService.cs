using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nutritia.Logic.Model.Entities;

namespace Nutritia
{
    public interface IDonService
    {
        IList<Don> RetrieveAll();
        void Insert(Don don);
        void Insert(Membre membre, Don don);
        DateTime LastTimeDon();
    }
}
