using System.Collections.Generic;
using System.Reflection.Emit;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Declarations
{
    /* id=ID COLON type_id=ID -> ^(FIELD_DEC $id $type_id)*/
    internal class FieldDeclarationNode : LanguageNode
    {
        #region CONSTRUCTORS:

        public FieldDeclarationNode(IToken tok): base(tok) { }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// the name of the field.
        /// </summary>
        public IdNode Field { get { return Children[0] as IdNode; } }

        /// <summary>
        /// The the type of the field.
        /// </summary>
        public IdNode TypeName { get { return Children[1] as IdNode; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            if (!scope.ContainsType(TypeName.Text))
                errors.Add(SemanticError.TypeNotDefined(TypeName.Text, this));

            TypeName.ILName = scope.GetILTypeName(TypeName.Text);
            Field.ILName = string.Format(".{0}", Field.Text);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            //
        }

        #endregion
    }
}
