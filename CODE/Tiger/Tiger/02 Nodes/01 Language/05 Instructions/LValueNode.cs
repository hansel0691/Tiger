using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace Tiger.AST_Nodes.Instructions
{
    /// <summary>
    /// represents a storage location that can be assigned
    /// a value: variables, parameters, fields of records, and elements
    /// of arrays.
    /// </summary>
    internal abstract class LValueNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        protected LValueNode(IToken tok) : base(tok) { }

        #endregion

    }
}
