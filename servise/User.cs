﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace servise
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public OperationContext opCon { get; set; }

    }
}
