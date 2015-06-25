using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instance
{
    /// <summary>
    /// Stands for initialice of a string
    /// STRING
    /// </summary>
    internal class InstanceStringNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public InstanceStringNode(IToken tok) : base(tok)
        {
            ExpressionType = TypeInfo.GenerateStringInfo();
        }

        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType
        {
            get; set; }

        public string Value { get { return Text; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            //
        }

        public override void Generate(ILGenerator generator, Symbols s)
        {
            generator.Emit(OpCodes.Ldstr, Text);
        }

        #endregion
    }
}
