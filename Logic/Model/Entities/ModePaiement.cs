﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia.Logic.Model.Entities
{
    public class ModePaiement
    {
        private ModePaiement() { }

        public static readonly ModePaiement Inconnue = new ModePaiement();
        public static readonly ModePaiement MasterCard = new ModePaiement();
        public static readonly ModePaiement Visa = new ModePaiement();
        public static readonly ModePaiement Amex = new ModePaiement();

        public override string ToString()
        {
            if (this == ModePaiement.Amex)
                return "Amex";
            if (this == ModePaiement.MasterCard)
                return "MasterCard";
            if (this == ModePaiement.Visa)
                return "Visa";
            return "Mode de Paiement Inconnue";
        }

        public static ModePaiement StringToValue(string s)
        {
            if (s == MasterCard.ToString())
                return MasterCard;

            if (s == Visa.ToString())
                return Visa;

            if (s == Amex.ToString())
                return Amex;

            return Inconnue;
        }
    }
}
