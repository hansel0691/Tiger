using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.FlowControl
{
     /// <summary>
    /// Represent the declaration of an if then else statament
    /// if expr then expr
    /// if expr then expr else expr
    /// </summary>
    internal class IfThenElseNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public IfThenElseNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }
    

        /// <summary>
        /// Condition of the statament
        /// </summary>
        public ValuedExpressionNode Condition { get { return (ValuedExpressionNode)Children[0]; } }

        /// <summary>
        /// Body for the statament then. This one is executed if the condition is true
        /// </summary>
        public ExpressionNode Then { get { return (ExpressionNode)Children[1]; } }

        /// <summary>
        /// Body for the statament else. This one is executed if the condition is false
        /// </summary>
        public ExpressionNode Else
        {
            get { return Children.Count > 2 ? (ExpressionNode)Children[2] : null; }
        }

        #endregion
        #region METHODS:

         public override void CheckSemantics(Scope scope, List<SemanticError> errors)
         {
             this.Condition.CheckSemantics(scope, errors);
             this.Then.CheckSemantics(scope, errors);
             if (Else != null)
                this.Else.CheckSemantics(scope, errors);
             
             //condition must be int
             if (this.Condition.ExpressionType.Type != TypesEnumeration.Integer)
                 errors.Add(SemanticError.InvalidIfCondition(this));
             //if else expression doesn't exist then condition shouldn't return a value
             if (Else == null && Then.ExpressionType.Type != TypesEnumeration.Void)
                 errors.Add(SemanticError.InvalidIfThen(this));
             else
             {
                 //if any, the return value of then and else expression must be the same.
                 if (Else != null && Then.ExpressionType.Name != Else.ExpressionType.Name)
                     errors.Add(SemanticError.InvalidIfReturn(this));
             }
             ExpressionType = Then.ExpressionType;
         }

         public override void Generate(ILGenerator generator, Symbols symbols)
         {
             var end = generator.DefineLabel();
             var elseLabel = generator.DefineLabel();


             Condition.Generate(generator, symbols);
             generator.Emit(OpCodes.Ldc_I4_0);
             generator.Emit(OpCodes.Beq, elseLabel);

             Then.Generate(generator, symbols);
             generator.Emit(OpCodes.Br, end);

             generator.MarkLabel(elseLabel);
             if (Else != null)
                 Else.Generate(generator, symbols);

             generator.MarkLabel(end);

         }

         #endregion
    }
}
