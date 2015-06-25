using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions;

namespace Tiger.AST_Nodes.Operators
{
    /// <summary>
    /// Stands for any operator of the language.
    /// </summary>
    internal abstract class OperatorNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        protected OperatorNode(IToken token) : base(token) { }

        #endregion
    }
}
