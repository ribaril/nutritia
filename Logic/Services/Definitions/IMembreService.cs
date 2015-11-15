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
        IList<Membre> RetrieveAdmins();
		String RetrieveMiseAJOur();
        Membre Retrieve(RetrieveMembreArgs args);
        void Insert(Membre membre);
        void Update(Membre membre);
    }
}
