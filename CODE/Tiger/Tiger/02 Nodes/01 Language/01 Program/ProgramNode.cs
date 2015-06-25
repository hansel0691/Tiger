using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;

namespace Tiger.AST_Nodes
{
    /// <summary>
    /// This class define the main node of the AST.
    /// </summary>
    internal class ProgramNode : LanguageNode
    {
        #region CONSTRUCTORS:

        public ProgramNode()
            : base()
        {
        }

        public ProgramNode(IToken token)
            : base(token)
        {
        }

        public ProgramNode(ProgramNode node)
            : base(node)
        {
        }

        #endregion

        #region PROPERTIES:

        #region STRUCTURE:

        public ExpressionNode Expression
        {
            get { return (ExpressionNode) Children[0]; }
        }

        #endregion

        #endregion

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            Expression.CheckSemantics(scope, errors);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            Expression.Generate(generator, symbols);
        }
    }
}
