using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Declarations
{
    /// <summary>
    /// Stands for the declaration of a variable.
    /// var id := expr
    /// var id : type-id := expr
    /// </summary>
    internal class VariableDeclarationNode : DeclarationNode
    {
        #region CONSTRUCTORS:

        public VariableDeclarationNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// Variable's identifier
        /// </summary>
        public override IdNode Identifier { get { return (IdNode)Children[0]; } }

        /// <summary>
        /// Variable's type.
        /// </summary>
        public IdNode VariableType { get { return Children.Count == 3 ? (IdNode) Children[1] : null; } }

        /// <summary>
        /// Expression you wanna assign to the variable.
        /// </summary>
        public ValuedExpressionNode Value { get { return VariableType != null ? (ValuedExpressionNode)Children[2] : (ValuedExpressionNode)Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            Value.CheckSemantics(scope, errors);
            
            if (scope.ContainsVarInstance(Identifier.Text, true) || scope.ContainsRoutine(Identifier.Text, true))
                errors.Add(SemanticError.DefinedVariable(Identifier.Text, this));
            if (Scope.standard_functions.Contains(Identifier.Text))
                errors.Add(SemanticError.HidingAnStandardFunc("Function", Identifier.Text, this));

            if (VariableType != null)
            {
                if (!scope.ContainsType(this.VariableType.Text))
                {
                    errors.Add(SemanticError.TypeNotDefined(this.VariableType.Text, this));
                    return;
                }
                //check if the value of the variable has the same type that the defined
                else if (Value.ExpressionType.Type == TypesEnumeration.Nil)
                {
                    if (!scope.GetType(VariableType.Text).Nilable)
                        errors.Add(SemanticError.InvalidNilAssignation(VariableType.Text, this));
                }
                else if (scope.GetType(this.Value.ExpressionType.Name).Name != scope.GetType(this.VariableType.Text).Name)
                    errors.Add(SemanticError.WrongType(this.VariableType.Text, this.Value.ExpressionType.Name, this));
            }
            else
            {
                switch (Value.ExpressionType.Type)
                {
                    case TypesEnumeration.Nil:
                        errors.Add(SemanticError.InvalidNilAssignation(Identifier.Text, this));
                        return;
                    case TypesEnumeration.Void:
                        errors.Add(SemanticError.NonValuedAssignation(this));
                        return;
                }
            }

            Identifier.ExpressionType = VariableType != null ? scope.GetType(VariableType.Text) : Value.ExpressionType;
            Identifier.ILName = string.Format("{0}Scope{1}", Identifier.Text, scope.CurrentScope);
            Value.ExpressionType.ILName = scope.GetILTypeName(Value.ExpressionType.Name);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            Type varType = symbols.GetRealType(Value.ExpressionType.ILName);
            FieldBuilder varVariable = symbols.ProgramType.DefineField(Identifier.ILName, varType,
                                                                       FieldAttributes.Public | FieldAttributes.Static);
            
            Value.Generate(generator, symbols);
            generator.Emit(OpCodes.Stsfld, varVariable);
            symbols.Variables.Add(Identifier.ILName, varVariable);
        }

        #endregion
    }
}
