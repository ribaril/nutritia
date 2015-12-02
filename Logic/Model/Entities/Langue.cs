using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class Langue
    {
        private Langue() { }
        private Langue(string nom, string ietf)
        {
            IETF = ietf;
            Nom = nom;
        }

        public string IETF { private set; get; }
        public string Nom { private set; get; }


        public static readonly Langue FrancaisCanada = new Langue("Français Canada", "fr-CA");
        public static readonly Langue AnglaisUSA = new Langue("Anglais États-Unis", "en-US");

        public static Langue LangueFromIETF(string ietf)
        {
            if (ietf == AnglaisUSA.IETF)
                return AnglaisUSA;
            else
                return FrancaisCanada;
        }
    }
}
