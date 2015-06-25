using Antlr.Runtime;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes
{
    internal abstract class ExpressionNode : LanguageNode
    {
        #region CONSTRUCTORS:

        protected ExpressionNode()
            : base()
        {
        }

        protected ExpressionNode(IToken token)
            : base(token)
        {
        }

        protected ExpressionNode(ExpressionNode node)
            : base(node)
        {
        }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// returns the type of the expression.
        /// </summary>
        public abstract ItemInfo ExpressionType { get; set; }

        #endregion
    }
}
