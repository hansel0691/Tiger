using System;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Operators.Relational
{
    /// <summary>
    /// Stands for the >= operator.
    /// </summary>
    internal class GreaterEqualThanNode : OrderComparison
    {
        #region CONSTRUCTORS:

        public GreaterEqualThanNode(IToken tok) : base(tok) { }

        #endregion

        public override void Generate(ILGenerator generator, Symbols s)
        {
            LeftOperand.Generate(generator, s);
            RightOperand.Generate(generator, s);

            if (LeftOperand.ExpressionType.Type == TypesEnumeration.Integer)
            {
                generator.Emit(OpCodes.Clt);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
            }
            else
            {
                generator.Emit(OpCodes.Call, typeof(String).GetMethod("CompareTo", new Type[] { typeof(string) }));
                generator.Emit(OpCodes.Ldc_I4_M1);
                generator.Emit(OpCodes.Ceq);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
            }
        }
    }
}
