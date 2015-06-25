using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Tiger.AST_Nodes.Instance;
using Tiger.AST_Nodes.Instructions;


namespace Tiger.AST_Nodes
{
    internal abstract class TigerNode : CommonTree
    {

        #region CONSTRUCTORS:

        protected TigerNode() : base() { }

        protected TigerNode(TigerNode node) : base(node) { }

        protected TigerNode(IToken token) : base(token) { }

        #endregion

        #region PROPERTIES:

        public override bool IsNil
        {
            get
            {
                return (base.IsNil && (this is NilNode));
            }
        }

        #endregion

        //todo: ask: simple example has a dupNode why? and why i don't have it?
    }
}
