﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaintCoinach.Ex {
    public interface IDataSheet : ISheet {
        Language Language { get; }
        byte[] GetBuffer();
    }
}
