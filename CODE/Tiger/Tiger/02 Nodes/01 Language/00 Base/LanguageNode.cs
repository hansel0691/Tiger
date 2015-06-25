using System.Reflection.Emit;
using Antlr.Runtime;
using System.Collections.Generic;
using Tiger.AST_Nodes.AST_Utils;

namespace Tiger.AST_Nodes
{
    internal abstract class LanguageNode : TigerNode
    {
        #region CONSTRUCTORS:

        protected LanguageNode() : base() {}

        protected LanguageNode(IToken token) : base(token) {}

        protected LanguageNode(LanguageNode node) : base(node) { }

        #endregion

        public string ILName { get; set; }

        public abstract void CheckSemantics(Scope scope, List<SemanticError> errors);

        public abstract void Generate(ILGenerator generator, Symbols symbols);
    }
}
