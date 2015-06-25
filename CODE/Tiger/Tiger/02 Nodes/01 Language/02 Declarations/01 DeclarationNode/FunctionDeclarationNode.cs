using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations.DeclarationBlocks;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Declarations
{
    /// <summary>
    /// Represents the declaration of a function.
    /// function id ( type-fields?) : type-id = expr
    /// </summary>
    internal class FunctionDeclarationNode : RoutineDeclarationNode
    {
        #region CONSTRUCTORS:

        /*^(FUNCTION_DEC $id $args? $type_id return_expr)*/
        public FunctionDeclarationNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// List of arguments separated by coma. id : type_id
        /// </summary>
        public override FieldDeclarationBlock Arguments
        {
            get
            {
                return Children.Count == 4 ? (FieldDeclarationBlock)Children[1] : null;
            }
        }

        /// <summary>
        /// Type that return the function.
        /// </summary>
        public override IdNode ReturnType { get { return Arguments != null? (IdNode) Children[2] : (IdNode)Children[1];  } }

        /// <summary>
        /// Body of the function.
        /// </summary>
        public ValuedExpressionNode Body { get { return Arguments != null ? (ValuedExpressionNode)Children[3] : (ValuedExpressionNode)Children[2]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            CheckFunctionId(scope,errors);
            
            //check the return type is defined already
            if (!scope.ContainsType(ReturnType.Text))
            {
                errors.Add(SemanticError.TypeNotDefined(ReturnType.Text, this));
                return;
            }
            var newScope = CreateFunctionScope(scope, errors);
            Body.CheckSemantics(newScope, errors);

            if (Body.ExpressionType.Type == TypesEnumeration.Nil)
            {
                if (!scope.GetType(ReturnType.Text).Nilable)
                    errors.Add(SemanticError.InvalidNilAssignation(ReturnType.Text,this));
            }
            else if (Body.ExpressionType.Name != scope.GetType(ReturnType.Text).Name)
                errors.Add(SemanticError.WrongType(ReturnType.Text, Body.ExpressionType.Name, this));
            
            ReturnType.ILName = ReturnType != null ? scope.GetILTypeName(ReturnType.Text) : "voidScope0";
            Identifier.ILName = string.Format("{0}Scope{1}", Identifier, scope.CurrentScope);
        }

        #endregion
    }
}
