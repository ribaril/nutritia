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
        IList<Transaction> RetrieveAll();
        void Insert(Transaction don);
    }
}
