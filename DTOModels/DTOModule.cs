﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModels
{
    public class DTOModule
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; }
    }
}
