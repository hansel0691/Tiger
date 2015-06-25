using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes
{
    /// <summary>
    /// Represent the the sentences that return a value.
    /// </summary>
    internal abstract class ValuedExpressionNode : ExpressionNode
    {
        #region CONSTRUCTORS:

        protected ValuedExpressionNode(IToken token) : base(token) { }

        #endregion
    }
}
