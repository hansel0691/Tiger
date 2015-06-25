using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Declarations
{
    /// <summary>
    /// This class represents a declaration of a type of array.
    /// e.g. "type a = array of b"
    /// </summary>
    internal class ArrayDeclarationNode : TypeDeclarationNode
    {
        #region CONSTRUCTORS:

        public ArrayDeclarationNode(IToken tok)
            : base(tok) { }

        #endregion
        #region PROPERTIES:
        
        /// <summary>
        /// Type text of the defined type.
        /// </summary>
        public IdNode DefinedType { get { return (IdNode)Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            CheckIdentifier(scope, errors);
            if (scope.GetType(Identifier.Text).Type != TypesEnumeration.Array)
                errors.Add(SemanticError.WrongType(Identifier.Text, "Array", this));

            Identifier.ILName = string.Format("{0}Scope{1}", Identifier.Text, scope.CurrentScope);
            DefinedType.ILName = scope.GetILTypeName(DefinedType.Text);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var arrayType = symbols.GetRealType(DefinedType.ILName);
            symbols.ArraysTypes.Add(Identifier.ILName, arrayType);
        }

        #endregion
    }
}
