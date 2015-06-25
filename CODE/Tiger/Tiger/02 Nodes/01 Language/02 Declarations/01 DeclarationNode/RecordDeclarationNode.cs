using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Antlr.Runtime;
using Tiger;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations;
using Tiger.AST_Nodes.Declarations.DeclarationBlocks;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;

namespace Tiger.AST_Nodes.Declarations
{
    /// <summary>
    /// Represents the declaration of a record
    /// type type-id = { type-fields? }
    /// /*^(RECORD_DEC $type_id type_fields)*/
    /// </summary>
    internal class RecordDeclarationNode : TypeDeclarationNode
    {
        #region CONSTRUCTORS:

        public RecordDeclarationNode(IToken tok)
            : base(tok) { }

        #endregion
        #region PROPERTIES:

        public FieldDeclarationBlock FieldDeclarationList { get { return Children.Count == 2 ? (FieldDeclarationBlock)Children[1] : null; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            CheckIdentifier(scope, errors);
            Identifier.ILName = scope.GetILTypeName(Identifier.Text);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var recordBuilder = symbols.ModuleBuilder.DefineType(Identifier.ILName, TypeAttributes.Public);
            symbols.Records.Add(Identifier.ILName, recordBuilder);

            foreach (var item in FieldDeclarationList)
            {
                item.ILName = Identifier.ILName + item.Field.ILName;
                Type _type = symbols.GetRealType(item.TypeName.ILName);
                FieldBuilder fieldBuilder = recordBuilder.DefineField(item.ILName,
                    _type,
                    FieldAttributes.Public);
                symbols.Variables.Add(item.ILName, fieldBuilder);
            }
            recordBuilder.CreateType();
        }

        #endregion
    }
}
