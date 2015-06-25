using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions.AccessNodes
{
    internal class FieldNestedNode  : NestedNode
    {
        #region CONSTRUCTORS:

        public FieldNestedNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        internal IdNode FieldIdentifier
        {
            get { return (IdNode)Children[0]; }
        }

        #endregion

        #region Overrides of LanguageNode

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            //
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            
            //FieldIdentifier.Generate(generator, symbols);
            generator.Emit(OpCodes.Ldfld, symbols.Variables[FieldIdentifier.ILName]);

            if (NextNested != null)
                NextNested.Generate(generator, symbols);
        }

        #endregion
    }
}
