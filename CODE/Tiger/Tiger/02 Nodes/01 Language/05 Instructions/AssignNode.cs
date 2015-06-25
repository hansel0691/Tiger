using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions
{
    /// <summary>
    /// Stands for any asignation statement.
    /// ^(ASSIGN ^(ARRAY_INDEX $id ^(ARRAY_FF_INDEX $length_index indexer_expr?))  $value)
    /// ^(ASSIGN ^(FIELD_ACCESS $id ^(DOT $access indexer_expr?)) $asignation_value)
    /// ^(ASSIGN $id $rvalue)
    /// </summary>
    internal class AssignNode : StatementNode
    {
        #region CONSTRUCTORS:

        public AssignNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public LValueNode Identifier { get { return (LValueNode)Children[0]; } }

        public ValuedExpressionNode Expr { get { return (ValuedExpressionNode)Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            //check the existence and the semantic of the left value.
            Identifier.CheckSemantics(scope, errors);
            //check the semantick of the expression.
            this.Expr.CheckSemantics(scope, errors);
            if (Identifier is IdNode)
                if (scope.ContainsVarInstance(Identifier.Text) && scope.GetVarInstance(Identifier.Text).ReadOnly)
                    errors.Add(SemanticError.ReadOnlyAssing(Identifier.Text,this));
            
            if (Expr.ExpressionType.Type == TypesEnumeration.Nil)
            {
                if (!scope.GetType(Identifier.ExpressionType.Name).Nilable)
                    errors.Add(SemanticError.InvalidNilAssignation(Identifier.ExpressionType.Name, this));
            }
            else if (Expr.ExpressionType.Type == TypesEnumeration.Void || scope.GetType(Identifier.ExpressionType.Name).Name != scope.GetType(Expr.ExpressionType.Name).Name)
                errors.Add(SemanticError.WrongType(Identifier.ExpressionType.Name, Expr.ExpressionType.Name, this));
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            if (Identifier is IdNode)
            {
                var lvalue = symbols.Variables[Identifier.ILName];
                Expr.Generate(generator, symbols);
                generator.Emit(OpCodes.Stsfld, lvalue);
            }
            else
            {
                var variable = symbols.Variables[Identifier.ILName];
                generator.Emit(OpCodes.Ldsfld, variable);

                if (Identifier is AccessFieldNode)
                    GenerateFieldAccess(((AccessFieldNode)Identifier).NextNested, generator, symbols);
                else
                    GenerateIndexAccess(((ArrayItemNode)Identifier).NextNested, generator, symbols);
            }
        }

        private void GenerateFieldAccess(FieldNestedNode current, ILGenerator generator, Symbols symbols)
        {
            if (current.NextNested != null)
            {
                generator.Emit(OpCodes.Ldfld, symbols.Variables[current.FieldIdentifier.ILName]);
                if (current.NextNested is FieldNestedNode)
                    GenerateFieldAccess((FieldNestedNode)current.NextNested, generator, symbols);
                else
                    GenerateIndexAccess((IndexNestedNode)current.NextNested, generator, symbols);
            }
            else
            {
                Expr.Generate(generator, symbols);
                generator.Emit(OpCodes.Stfld, symbols.Variables[current.FieldIdentifier.ILName]);
            }
        }

        private void GenerateIndexAccess(IndexNestedNode current, ILGenerator generator, Symbols symbols)
        {
            if (current.NextNested != null)
            {
                current.Index.Generate(generator, symbols);
                generator.Emit(OpCodes.Ldelem, symbols.GetRealType(current.ILName));
                current = (IndexNestedNode)current.NextNested;
                if (current.NextNested is FieldNestedNode)
                    GenerateFieldAccess((FieldNestedNode)current.NextNested, generator, symbols);
                else
                    GenerateIndexAccess((IndexNestedNode)current.NextNested, generator, symbols);
            }
            else
            {
                current.Index.Generate(generator, symbols);
                Expr.Generate(generator, symbols);
                generator.Emit(OpCodes.Stelem, symbols.GetRealType(Identifier.ExpressionType.ILName));
            }
        }

        #endregion
    }
}
