using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions.AccessNodes;

namespace Tiger.AST_Nodes.Declarations
{
    internal abstract class DeclarationNode : StatementNode
    {
        #region CONSTRUCTORS:

        protected DeclarationNode(IToken token) : base(token) { }

        #endregion

        public abstract IdNode Identifier { get; }

    }
}
