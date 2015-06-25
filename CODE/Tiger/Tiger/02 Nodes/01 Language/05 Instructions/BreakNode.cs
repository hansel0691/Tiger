using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations;
using Tiger.AST_Nodes.FlowControl;

namespace Tiger.AST_Nodes.Instructions
{
    /// <summary>
    /// break
    /// </summary>
    internal class BreakNode : StatementNode
    {
        #region CONSTRUCTORS:

        public BreakNode(IToken tok) : base(tok) { }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            if (!InsideLoop((LanguageNode)this.Parent, 0))
                errors.Add(SemanticError.WrongBreak(this));
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            generator.Emit(OpCodes.Br, symbols.NestedCylesBreakJumps.Peek());
        }

        public bool InsideLoop(LanguageNode node, int loopsCount)
        {
            if (node == null || node is RoutineDeclarationNode || node is LetInEndNode)
                return false;

            return ((node is WhileNode || node is ForNode) && loopsCount == 0) || InsideLoop((LanguageNode)node.Parent, node is BreakNode
                                                                                                    ? loopsCount--
                                                                                                    : node is WhileNode || node is ForNode ? loopsCount++ : loopsCount);
        }

        #endregion
    }
}
