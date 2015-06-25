using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions.AccessNodes
{
    /// <summary>
    /// Represent the  consecutive access to fields from a Record
    /// id dot id index_expr
    /// ^(ACCESS_FIELD $id ^(DOT $access indexer_expr?))
    /// </summary>
    internal class AccessFieldNode : LValueNode
    {
        #region CONSTRUCTORS:

        public AccessFieldNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }

        /// <summary>
        /// Identifier of the record you want to access
        /// </summary>
        public IdNode Record { get { return ((IdNode)Children[0]); } }

        /// <summary>
        /// Name of the element to acceder
        /// </summary>
        public FieldNestedNode NextNested { get { return (FieldNestedNode)Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            var varInfo = scope.GetVarInstance(Record.Text);
            //Verify if the variable exist
            if (varInfo == null)
            {
                errors.Add(SemanticError.UndefinedVariableUsed(this.Record.Text, this));
                return;
            }
            
            NextNested.CheckSemantics(varInfo.VariableType, scope, errors);
            ExpressionType = NextNested.ExpressionType;
            Record.ILName = scope.GetILVarNames(Record.Text);
            ILName = scope.GetILVarNames(Record.Text);
            ExpressionType.ILName = scope.GetILTypeName(ExpressionType.Name);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var typeFeild = symbols.Variables[Record.ILName];
            generator.Emit(OpCodes.Ldsfld, typeFeild);
            //NextNested.FieldIdentifier.Generate(generator, symbols);
            generator.Emit(OpCodes.Ldfld, symbols.Variables[NextNested.FieldIdentifier.ILName]);

            if (NextNested.NextNested != null)
                NextNested.NextNested.Generate(generator, symbols);
        }

        #endregion
    }
}
