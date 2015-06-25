using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;

namespace Tiger.AST_Nodes.Declarations
{
    /// <summary>
    /// this class represents the creation of an alias.
    /// e.g. "type type_id = id"
    /// </summary>
    internal class AliasDeclarationNode : TypeDeclarationNode
    {
        #region CONSTRUCTORS:

        public AliasDeclarationNode(IToken token)
            : base(token) { }
        
        #endregion
        #region PROPERTIES:

        /// <summary>
        /// Identifier of the alias.
        /// </summary>
        public IdNode Alias { get { return (IdNode)Children[1]; }  }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            CheckIdentifier(scope, errors);
            Identifier.ILName = string.Format("{0}Scope{1}", Identifier.Text, scope.CurrentScope);
            Alias.ILName = scope.GetILTypeName(Alias.Text);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            if (symbols.Records.ContainsKey(Alias.ILName))
                symbols.Records.Add(Identifier.ILName, symbols.Records[Alias.ILName]);
            else 
                symbols.ArraysTypes.Add(Identifier.ILName, symbols.ArraysTypes[Alias.ILName]);
        }

        #endregion
    }
}
