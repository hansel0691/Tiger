using System.Collections.Generic;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;

namespace Tiger.AST_Nodes.Declarations
{
    internal abstract class TypeDeclarationNode : DeclarationNode
    {
        #region CONSTRUCTORS:
        
        protected TypeDeclarationNode(IToken tok) : base(tok){ }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// The identifier of the new type.
        /// </summary>
        public override IdNode Identifier { get { return (IdNode)Children[0]; } }

        #endregion

        protected void CheckIdentifier(Scope scope, List<SemanticError> errors)
        {
            //check identifier type isn't an int or string.
            if (this.Identifier.Text == "int" || this.Identifier.Text == "string")
                errors.Add(SemanticError.WrongAliasDeclaration(this.Identifier.Text, this));

        }
        
    }
}
