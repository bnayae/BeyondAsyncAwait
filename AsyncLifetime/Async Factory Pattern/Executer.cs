using System;
using System.Collections.Generic;
using System.Text;

namespace Bnaya.Samples
{
    internal class Executer : IExecuter
    {
        private Config _config;

        public Executer(Config config)
        {
            _config = config;
        }

        public int GetData()
        {
            try
            {
                return 42 * _config.Factor;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
