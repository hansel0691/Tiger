using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;


namespace Tiger.AST_Nodes.Operators.Arithmetical
{
    internal abstract class ArithmeticNode : OperatorNode
    {
        #region CONSTRUCTORS:

        protected ArithmeticNode(IToken token) : base(token) { }

        #endregion
    }

    /// <summary>
    /// Stands for the unary arithmetical operator.
    /// </summary>
    internal abstract class ArithmeticUnaryOperatorNode : ArithmeticNode
    {
        #region CONSTRUCTORS:

        protected ArithmeticUnaryOperatorNode(IToken token) : base(token)
        {
            ExpressionType = TypeInfo.GenerateIntInfo();
        }
        
        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType { get; set; }
        internal ValuedExpressionNode Operando { get { return (ValuedExpressionNode)Children[0]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            this.Operando.CheckSemantics(scope, errors);
            if (this.Operando.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.WrongType("int", this.Operando.ExpressionType.Name,this));
        }

        #endregion
    }

    /// <summary>
    /// Stands for the binary arithmetical operator.
    /// </summary>
    internal abstract class ArithmeticBinaryOperatorNode : ArithmeticNode
    {
        #region CONSTRUCTORS:

        protected ArithmeticBinaryOperatorNode(IToken tok) : base(tok)
        {
            ExpressionType = TypeInfo.GenerateIntInfo();
        }

        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType { get; set; }
        internal ValuedExpressionNode LeftOperand { get { return (ValuedExpressionNode)Children[0]; } }
        internal ValuedExpressionNode RightOperand { get { return (ValuedExpressionNode)Children[1]; } }
        public abstract OpCode OperatorOpCode { get; }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            LeftOperand.CheckSemantics(scope, errors);
            RightOperand.CheckSemantics(scope, errors);
            if (LeftOperand.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.WrongType("int", LeftOperand.ExpressionType.Name, this));
            if (RightOperand.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.WrongType("int", RightOperand.ExpressionType.Name, this));
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            LeftOperand.Generate(generator, symbols);
            RightOperand.Generate(generator, symbols);
            generator.Emit(OperatorOpCode);
        }

        #endregion
    }
}
