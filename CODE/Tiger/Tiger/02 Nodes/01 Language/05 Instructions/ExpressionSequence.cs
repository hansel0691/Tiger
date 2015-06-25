using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.FlowControl;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions
{
    /* expr-seq ; expr ->  expr+ */
    internal class ExpressionSequence: ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public ExpressionSequence(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }

        public ExpressionNode this[int i]
        {
            get
            {
                return (ExpressionNode)Children[i];
            }
        }

        public int Count
        {
            get
            {
                return Children != null ? Children.Count : 0;
            }
        }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            if (Count == 0) this.ExpressionType = TypeInfo.GenerateVoidInfo();
            var breakable = Breakable(this, 0);
            for (int i = 0; i < this.Count; i++)
            {
                this[i].CheckSemantics(scope, errors);
                if (i == Count - 1 && !breakable) ExpressionType = this[i].ExpressionType;
            }
            if (breakable) ExpressionType = TypeInfo.GenerateVoidInfo();
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].Generate(generator, symbols);
                if (i < Count - 1 && this[i].ExpressionType.Type != TypesEnumeration.Void) generator.Emit(OpCodes.Pop);
            }
        }

        private bool Breakable(LanguageNode root, int bucleCount)
        {
            if (root.ChildCount == 0) return false;
            foreach (var child in root.Children)
            {
                if (child is WhileNode || child is ForNode)
                    bucleCount++;
                if (child is BreakNode || Breakable((LanguageNode)child, bucleCount))
                    if (bucleCount == 0)
                        return true;
                    else bucleCount--;
            }
            return false;
        }

        #endregion


    }
}
