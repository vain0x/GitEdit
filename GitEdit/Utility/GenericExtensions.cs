using System;

namespace GitEdit
{
    public static class GenericExtensions
    {
        public static Y ApplyTo<X, Y>(this X x, Func<X, Y> f)
        {
            return f(x);
        }
    }
}
