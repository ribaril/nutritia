﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IUniteMesureService
    {
        IList<UniteMesure> RetrieveAll();
        UniteMesure Retrieve(RetrieveUniteMesureArgs args);
    }
}
