using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations;
using Tiger.AST_Nodes.Instructions.AccessNodes;

namespace Tiger.AST_Nodes.Instructions
{
    /*
     *   expr-list , return_expr
         ^(ARGUMENT return_expr+)
    */
    class ArgumentListNode : LanguageNode, IEnumerable<ValuedExpressionNode>
    {

        #region CONSTRUCTORS:

        public ArgumentListNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public ValuedExpressionNode this[int i] { get { return (ValuedExpressionNode)Children[i]; } set { throw new AccessViolationException(); } }

        public int Count { get { return Children.Count; } set{throw new AccessViolationException();} }

        #endregion
        #region METHODS:


        #region Implementation of IEnumerable

        public IEnumerator<ValuedExpressionNode> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            foreach (var argument in this)   
                argument.CheckSemantics(scope, errors);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            throw new NotImplementedException();
        }

        #endregion

        
    }
}
