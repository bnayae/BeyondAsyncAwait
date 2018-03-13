﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Setting : ISetting
    {
        public async Task<Config> GetSetting()
        {
            await Task.Delay(1000);
            return new Config { Factor = 10 };
        }
    }
}
