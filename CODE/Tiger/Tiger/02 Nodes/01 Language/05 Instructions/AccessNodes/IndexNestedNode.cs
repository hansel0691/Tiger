using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions.AccessNodes
{
    /// <summary>
    /// Define the consecutive indexes of an array
    /// </summary>
    internal class IndexNestedNode : NestedNode
    {
        /*
         ^(FIELD_ACCESS $id ^(DOT $access indexer_expr?))
         */
        #region CONSTRUCTORS:

        public IndexNestedNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }

        public ValuedExpressionNode Index { get { return (ValuedExpressionNode) Children[0]; } }

        #endregion

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            Index.CheckSemantics(scope, errors);
            if (Index.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.WrongType("int", Index.ExpressionType.Name, this));
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            Index.Generate(generator, symbols);
            generator.Emit(OpCodes.Ldelem, symbols.GetRealType(Index.ILName));

            if (NextNested != null)
                NextNested.Generate(generator, symbols);
        }
    }
}
