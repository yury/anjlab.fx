using System;

namespace AnjLab.FX.Sys
{
    public class Operation
    {
        public static void Execute<TIgnoreException>(VoidAction operation)
            where TIgnoreException : Exception
        {
            try
            {
                operation();
            }
            catch (TIgnoreException)
            {
            }
        }

        public static TProduct Execute<TProduct, TIgnoreException>(FactoryMethod<TProduct> operation)
            where TIgnoreException : Exception
           where TProduct : class
        {
            try
            {
                return operation();
            }
            catch (TIgnoreException)
            {
            }
            return null;
        }

        public static TProduct Execute<TProduct, TSource, TIgnoreException>(FactoryMethod<TProduct, TSource> operation, TSource source)
            where TIgnoreException : Exception
            where TProduct : class
        {
            try
            {
                return operation(source);
            }
            catch (TIgnoreException)
            {
            }
            return null;
        }
    }
}
