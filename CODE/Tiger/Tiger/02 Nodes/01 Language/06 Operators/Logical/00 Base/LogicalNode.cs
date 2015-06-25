using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Operators.Logical
{
    internal abstract class LogicalNode : OperatorNode
    {
        #region CONSTRUCTORS:

        protected LogicalNode(IToken token) : base(token)
        {
            ExpressionType = TypeInfo.GenerateIntInfo();
        }

        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType { get; set; }
        public ValuedExpressionNode LeftOperand { get { return (ValuedExpressionNode)Children[0]; } }
        public ValuedExpressionNode RightOperand { get { return (ValuedExpressionNode)Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            LeftOperand.CheckSemantics(scope, errors);
            RightOperand.CheckSemantics(scope, errors);
            if (LeftOperand.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.WrongType(LeftOperand.ExpressionType.Name, "int", this));
            if (RightOperand.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.WrongType(RightOperand.ExpressionType.Name, "int", this));
        }

        #endregion
    }
}
