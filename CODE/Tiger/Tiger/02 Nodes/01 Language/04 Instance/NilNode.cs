using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instance
{
    /// <summary>
    /// Represent the nill value.
    /// NILL
    /// </summary>
    internal class NilNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public NilNode(IToken tok) : base(tok)
        {
            ExpressionType = TypeInfo.GenerateNilInfo();
        }

        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType
        {
            get; set; }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            //
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            generator.Emit(OpCodes.Ldnull);
        }

        #endregion
    }
}
