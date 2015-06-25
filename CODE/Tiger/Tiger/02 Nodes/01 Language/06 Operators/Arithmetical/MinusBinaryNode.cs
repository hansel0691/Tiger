using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions;

namespace Tiger.AST_Nodes.Operators.Arithmetical
{
    /// <summary>
    /// Stands for the - operator.
    /// </summary>
    internal class MinusBinaryNode : ArithmeticBinaryOperatorNode
    {
        #region CONSTRUCTORS:

        public MinusBinaryNode(IToken token) : base(token) { }

        #endregion

        public override OpCode OperatorOpCode
        {
            get { return OpCodes.Sub; }
        }

    }
}
