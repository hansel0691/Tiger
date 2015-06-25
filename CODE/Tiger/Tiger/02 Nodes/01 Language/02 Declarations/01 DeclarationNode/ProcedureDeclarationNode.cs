using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations.DeclarationBlocks;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Declarations
{
    /// <summary>
    /// Represent the declaration of a procedure.
    /// function id ( type-fields?) = expr
    /// </summary>
    internal class ProcedureDeclarationNode : RoutineDeclarationNode
    {
        #region CONSTRUCTORS:

        /* ^(PROC_DEC $id $args? expr)*/
        public ProcedureDeclarationNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// List of arguments separated by coma. id : type_id
        /// </summary>
        public override FieldDeclarationBlock Arguments
        {
            get
            {
                return Children.Count == 3 ? (FieldDeclarationBlock)Children[1] : null;
            }
        }

        /// <summary>
        /// Body of the procedure.
        /// </summary>
        public ExpressionNode Body { get { return this.Arguments != null ? (ExpressionNode)Children[2] : (ExpressionNode)Children[1]; } }

        public override IdNode ReturnType
        {
            get { return null; }
        }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            CheckFunctionId(scope,errors);
            var newScope = CreateFunctionScope(scope, errors);
            this.Body.CheckSemantics(newScope, errors);

            //Verify that doesn't return
            if (Body.ExpressionType.Type != TypesEnumeration.Void)
                errors.Add(SemanticError.ProcedureDontReturn(Identifier.Text, this));

            Identifier.ILName = string.Format("{0}Scope{1}", Identifier, scope.CurrentScope);
        }

        #endregion
    }
}
