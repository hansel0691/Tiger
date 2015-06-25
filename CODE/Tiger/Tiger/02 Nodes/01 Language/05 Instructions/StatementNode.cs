using System;
using Antlr.Runtime;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes
{
    /// <summary>
    /// Represent the expression that do not return a value.
    /// </summary>
    internal abstract class StatementNode : ExpressionNode
    {
        #region CONSTRUCTORS:

        protected StatementNode(IToken token) : base(token)
        {
            this.ExpressionType = TypeInfo.GenerateVoidInfo();
        }

        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType { get;  set;  }

        #endregion
    }
}
