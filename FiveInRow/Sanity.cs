using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInRow
{
    static class Sanity
    {
        public static void Requires(bool valid, string message)
        {
            if (!valid)
                throw new FiveInRowException(message);
        }
    }
}
