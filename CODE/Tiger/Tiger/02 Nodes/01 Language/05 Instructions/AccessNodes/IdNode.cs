using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions.AccessNodes
{
    /// <summary>
    /// Represents a variable.
    /// ID
    /// </summary>
    internal class IdNode : LValueNode
    {
        #region CONSTRUCTORS:

        public IdNode(IToken tok)
            : base(tok)
        {
        }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }

        public string VariableILName { get; set; }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            var varInfo = scope.GetVarInstance(Text);
            if (varInfo == null)
                errors.Add(SemanticError.UndefinedVariableUsed(Text, this));
            else
            {
                ExpressionType = scope.GetType(varInfo.VariableType);
                VariableILName = scope.GetILVarNames(Text);
                ILName = VariableILName;
            }
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var fieldBuilder = symbols.Variables[VariableILName];
            generator.Emit(OpCodes.Ldsfld, fieldBuilder);
        }

        #endregion
    }
}
