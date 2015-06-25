using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Operators.Relational
{
    /// <summary>
    /// Stands for the <> operator.
    /// </summary>
    internal class NotEqualNode : EqualyComparison
    {
        #region CONSTRUCTORS:

        public NotEqualNode(IToken token) : base(token) { }

        #endregion

        public override void Generate(ILGenerator generator, Symbols s)
        {
            LeftOperand.Generate(generator, s);
            RightOperand.Generate(generator, s);
            if (LeftOperand.ExpressionType.Type == TypesEnumeration.String)
            {
                generator.Emit(OpCodes.Call, typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) }));
                var peek = generator.DeclareLocal(typeof (int));
                generator.Emit(OpCodes.Stloc, peek);
                generator.Emit(OpCodes.Ldloc,peek);
                generator.Emit(OpCodes.Ldloc, peek);
                generator.Emit(OpCodes.Mul);
            }
            else
            {
                generator.Emit(OpCodes.Ceq);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
            }
        }
    }
}
