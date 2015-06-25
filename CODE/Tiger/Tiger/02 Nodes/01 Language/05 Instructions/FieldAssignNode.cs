using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.AST_Nodes;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions.AccessNodes;

namespace Tiger
{
    /// <summary>
    /// Used in the instanciation of a record
    /// e.g. in : record {Name = Hansel, Age = 22}
    /// field assign is : Name = Hansel and Age = 22
    /// </summary>
    class FieldAssignNode : LanguageNode
    {
        #region CONSTRUCTORS:

        public FieldAssignNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public IdNode Field { get { return (IdNode) Children[0]; } }

        public ValuedExpressionNode Value { get { return (ValuedExpressionNode) Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            //check the nil assignation
            Value.CheckSemantics(scope, errors);
            Field.ExpressionType = Value.ExpressionType;
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            Value.Generate(generator, symbols);
        }

        #endregion
    }
}
