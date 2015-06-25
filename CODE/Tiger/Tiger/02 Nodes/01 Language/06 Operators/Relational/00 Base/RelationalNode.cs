using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Operators.Relational
{
    /// <summary>
    /// Define the logical operators.
    /// </summary>
    internal abstract class RelationalNode : OperatorNode
    {
        #region CONSTRUCTORS:

        protected RelationalNode(IToken token) : base(token)
        {
            ExpressionType = TypeInfo.GenerateIntInfo();
        }

        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType { get; set; }
        public ValuedExpressionNode LeftOperand { get { return (ValuedExpressionNode)Children[0]; } }
        public ValuedExpressionNode RightOperand { get { return (ValuedExpressionNode)Children[1]; } }

        #endregion
    }

    internal abstract class EqualyComparison : RelationalNode
    { 
        #region CONSTRUCTORS:

        protected EqualyComparison(IToken token) : base(token) { }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            LeftOperand.CheckSemantics(scope, errors);
            RightOperand.CheckSemantics(scope, errors);

            if (LeftOperand.ExpressionType.Type == TypesEnumeration.Void || RightOperand.ExpressionType.Type == TypesEnumeration.Void)
            {
                errors.Add(SemanticError.InvalidUseOfOperator(this));
                return;
            }

            if (!scope.ContainsType(RightOperand.ExpressionType.Name))
                errors.Add(SemanticError.TypeNotDefined(RightOperand.ExpressionType.Name, this));
            if (!scope.ContainsType(LeftOperand.ExpressionType.Name))
                errors.Add(SemanticError.TypeNotDefined(LeftOperand.ExpressionType.Name, this));

            if (LeftOperand.ExpressionType.Type == TypesEnumeration.Nil )
            {
                if (!scope.GetType(RightOperand.ExpressionType.Name).Nilable)
                    errors.Add(SemanticError.InvalidNilAssignation(RightOperand.ExpressionType.Name, this));
            }
            else if (RightOperand.ExpressionType.Type == TypesEnumeration.Nil)
            {
                if (!scope.GetType(LeftOperand.ExpressionType.Name).Nilable)
                    errors.Add(SemanticError.InvalidNilAssignation(LeftOperand.ExpressionType.Name, this));
            }
            else if (LeftOperand.ExpressionType.Name != RightOperand.ExpressionType.Name )
                errors.Add(SemanticError.WrongType(LeftOperand.ExpressionType.Name, RightOperand.ExpressionType.Name, this));
        }

        #endregion
    }
    internal abstract class OrderComparison : RelationalNode
    {
        #region CONSTRUCTORS:

        protected OrderComparison(IToken token) : base(token) { }
                #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            LeftOperand.CheckSemantics(scope, errors);
            RightOperand.CheckSemantics(scope, errors);
            if (LeftOperand.ExpressionType.Type != TypesEnumeration.String || RightOperand.ExpressionType.Type != TypesEnumeration.String)
                if (LeftOperand.ExpressionType.Type != TypesEnumeration.Integer || RightOperand.ExpressionType.Type != TypesEnumeration.Integer)
                    errors.Add(SemanticError.InvalidOperandAtLogical(this));
        }

        #endregion
    }
}
