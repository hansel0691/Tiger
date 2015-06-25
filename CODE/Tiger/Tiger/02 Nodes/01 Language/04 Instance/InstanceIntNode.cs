using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instance
{
    /// <summary>
    /// Stands for the creation of a constant int.
    /// INT
    /// </summary>
    internal class InstanceIntNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public InstanceIntNode(IToken tok) : base(tok)
        {
            ExpressionType = TypeInfo.GenerateIntInfo();
        }

        #endregion
        #region PROPERTIES:

        public override sealed ItemInfo ExpressionType { get; set; }

        public int? Value 
        { 
            get 
            { 
                int value;
                return (int.TryParse(this.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out value) ? (int?)value : null);
            }
        }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            if (Value == null)
                errors.Add(SemanticError.InvalidNumber(Text, this));
            
        }

        public override void Generate(ILGenerator generator, Symbols s)
        {
            generator.Emit(OpCodes.Ldc_I4, (int)Value);
        }

        #endregion
    }
}
