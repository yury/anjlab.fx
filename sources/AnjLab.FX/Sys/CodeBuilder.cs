using System;
using System.CodeDom;

namespace AnjLab.FX.Sys
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

        public static CodeSnippetExpression BuildExpression(string expression)
        {
            return new CodeSnippetExpression(expression);
        }

        public static CodeSnippetExpression BuildExpression(string format, params string [] args)
        {
            return BuildExpression(string.Format(format, args));
        }

        public static CodeCastExpression BuildCastExpression(Type targetType, string expression, params string [] args)
        {
            return new CodeCastExpression(targetType, BuildExpression(expression, args));
        }

        public static Type GetGenericType(Type genericType, Type typeArgument)
        {
            return genericType.MakeGenericType(typeArgument);
        }

        public static CodeParameterDeclarationExpression Parameter(Type type, string name)
        {
            return new CodeParameterDeclarationExpression(type, name);
        }
    }
}
