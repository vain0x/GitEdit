using System;
using System.Linq;

namespace GitEdit.View
{
    public static class LinqExtensions
    {
        // Generalized ?. operator
        public static Y RefBind<X, Y>(this X self, Func<X, Y> f)
            where X: class
            where Y: class
        {
            return self == null ? null : f(self);
        }
    }
}
