using System;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Operators.Relational
{
    /// <summary>
    /// Stands for the > operator.
    /// </summary>
    internal class GreaterThanNode : OrderComparison
    {
        #region CONSTRUCTORS:

        public GreaterThanNode(IToken token) : base(token) { }

        #endregion

        public override void Generate(ILGenerator generator, Symbols s)
        {
            LeftOperand.Generate(generator, s);
            RightOperand.Generate(generator, s);

            if (LeftOperand.ExpressionType.Type == TypesEnumeration.Integer)
                generator.Emit(OpCodes.Cgt);
            else
            {
                generator.Emit(OpCodes.Call, typeof(String).GetMethod("CompareTo", new Type[] { typeof(string) }));
                generator.Emit(OpCodes.Ldc_I4_1);
                generator.Emit(OpCodes.Ceq);
            }
        }
    }
}
