using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitEdit.Utility
{
    public static class GenericExtensions
    {
        public static Y ApplyTo<X, Y>(this X x, Func<X, Y> f)
        {
            return f(x);
        }
    }
}
