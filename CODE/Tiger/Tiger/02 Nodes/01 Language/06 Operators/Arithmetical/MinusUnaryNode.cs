using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions;

namespace Tiger.AST_Nodes.Operators.Arithmetical
{
    /// <summary>
    /// Represents the expretion with a minus and a value
    /// </summary>
    internal class MinusUnaryNode : ArithmeticUnaryOperatorNode
    {
        #region CONSTRUCTORS:

        public MinusUnaryNode(IToken token) : base(token) { }

        #endregion

        public override void Generate(ILGenerator generator, Symbols s)
        {
            Operando.Generate(generator, s);
            generator.Emit(OpCodes.Neg);
        }
    }
}
