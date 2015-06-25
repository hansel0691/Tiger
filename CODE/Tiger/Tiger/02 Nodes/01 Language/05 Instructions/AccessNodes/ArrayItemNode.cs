using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions.AccessNodes
{
    /// <summary>
    /// Stands for the indexation of an array.
    /// id [return_expr] indexer_expr?
    /// ^(ARRAY_INDEX $id ^(ARRAY_FF_INDEX $length_index indexer_expr?))
    /// </summary>
    
    internal class ArrayItemNode : LValueNode
    {
        #region CONSTRUCTORS:

        public ArrayItemNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }

        public IdNode ArrayIdentifier { get { return (IdNode) Children[0]; } }

        public IndexNestedNode NextNested { get { return (IndexNestedNode) Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            //Verify if the array exist
            var varInfo = scope.GetVarInstance(this.ArrayIdentifier.Text);
            if (varInfo == null)
            {
                errors.Add(SemanticError.UndefinedVariableUsed(this.ArrayIdentifier.Text, this));
                return;
            }
            this.NextNested.CheckSemantics(varInfo.VariableType, scope, errors);
            ExpressionType = this.NextNested.ExpressionType;
            ArrayIdentifier.ILName = scope.GetILVarNames(ArrayIdentifier.Text);
            var arrayInfo = (ArrayInfo) scope.GetType(varInfo.VariableType);
            
            NextNested.Index.ILName = scope.GetILTypeName(arrayInfo.ItemsType);
            ILName = scope.GetILVarNames(ArrayIdentifier.Text);
            ExpressionType.ILName = scope.GetILTypeName(ExpressionType.Name);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var varVariable = symbols.Variables[ArrayIdentifier.ILName];
            generator.Emit(OpCodes.Ldsfld, varVariable);
            NextNested.Index.Generate(generator, symbols);
            generator.Emit(OpCodes.Ldelem, symbols.GetRealType(NextNested.Index.ILName));

            if (NextNested.NextNested != null) 
                NextNested.NextNested.Generate(generator, symbols);
        }

        #endregion
    }
}
