using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;

namespace Tiger.AST_Nodes.Declarations.DeclarationBlocks
{
    /// <summary>
    /// ^(FIELDS_DEC type_field+)
    /// type_field -> ^(FIELD_DEC $id $type_id)
    /// id=ID : type_id=ID, id=ID : type_id=ID, ...
    /// </summary>
    class FieldDeclarationBlock : LanguageNode, IEnumerable<FieldDeclarationNode> 
    {
        #region CONSTRUCTORS: 

        public FieldDeclarationBlock(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public FieldDeclarationNode this[int i]
        {
            get
            {
                if (i < 0 || i >= Children.Count)
                    throw new ArgumentException(string.Format("Index out of range tring indexing fields dec."));
                return (FieldDeclarationNode)Children[i];
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
            var declaredFields = new List<string>();
            foreach (var fieldDeclaration in this)
            {
                fieldDeclaration.CheckSemantics(scope, errors);
                if (declaredFields.Contains(fieldDeclaration.Field.Text))
                    errors.Add(SemanticError.DefinedField(fieldDeclaration.Field.Text,this));
                declaredFields.Add(fieldDeclaration.Field.Text);
            }
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            throw new NotImplementedException();
        }

        #region Implementation of IEnumerable

        public IEnumerator<FieldDeclarationNode> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion

        
    }
}
