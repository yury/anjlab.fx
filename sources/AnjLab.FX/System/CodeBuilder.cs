using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace AnjLab.FX.System
{
    public class CodeBuilder
    {
        public static CodeSnippetStatement BuildStatement(string statement)
        {
            return new CodeSnippetStatement(statement);
        }

        public static CodeSnippetStatement BuildStatement(string format, params string[] args)
        {
            return BuildStatement(string.Format(format, args));
        }

        public static Type GetGenericType(Type genericType, Type typeArgument)
        {
            return genericType.MakeGenericType(typeArgument);
        }
    }
}
