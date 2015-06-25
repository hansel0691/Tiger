using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Antlr.Runtime.Tree;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations;
using Tiger.AST_Nodes.Instructions;
using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instance
{
    /// <summary>
    /// Stands for the instanciation of a record.
    /// type-id { field-list? }
    /// field_list: field-list , id = expr
    /// e.g. in : Person {Name = Hansel, Age = 22}
    /// </summary>
    internal class InstanceRecordNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public InstanceRecordNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }
        public IdNode RecordIdentifier { get { return (IdNode) Children[0]; } }
        public  List<FieldAssignNode> FieldInitialicer
        {
            get
            {
                var list = Children.Count >= 2 ? new List<FieldAssignNode>() : null;
                for (int i = 1; i < Children.Count; i++)
                    list.Add((FieldAssignNode)Children[i]);
                
                return list;
            }
        }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            RecordIdentifier.ILName = scope.GetILTypeName(RecordIdentifier.Text);

            var typeInfo = scope.GetType(this.RecordIdentifier.Text);
            //check if the record type is defined 
            if (typeInfo == null || typeInfo.Type != TypesEnumeration.Record || scope.GetType(typeInfo.Name).Name != scope.GetType(this.RecordIdentifier.Text).Name)
            {
                errors.Add(SemanticError.TypeNotDefined(this.RecordIdentifier.Text, this));
                return;
            }

            var recordInfo = (RecordInfo) typeInfo;
            //the count of parameters used to define and to initialice a record must match.
            if (recordInfo.FieldsCount != this.FieldInitialicer.Count)
            {
                errors.Add(SemanticError.WrongParameterNumber("Record", this.RecordIdentifier.Text, recordInfo.FieldsCount, this.FieldInitialicer.Count, this));
                return;
            }

            for (int i = 0; i < recordInfo.FieldsCount; i++)
            {
                var assignNode = this.FieldInitialicer[i];
                assignNode.CheckSemantics(scope, errors);

                //check the field name and order
                if (assignNode.Field.Text != recordInfo.Parameters[i].Identifier)
                    errors.Add(SemanticError.WrongFieldInit(this.RecordIdentifier.Text, recordInfo.Parameters[i].Identifier, this));
                //check the field type
                if (assignNode.Value.ExpressionType.Type == TypesEnumeration.Nil)
                {
                    if (!scope.GetType(recordInfo.Parameters[i].Type).Nilable)
                        errors.Add(SemanticError.InvalidNilAssignation(recordInfo.Parameters[i].Identifier, this));
                }
                else if (scope.GetType(assignNode.Value.ExpressionType.Name).Name != scope.GetType(recordInfo.Parameters[i].Type).Name)
                    errors.Add(SemanticError.WrongType(assignNode.Value.ExpressionType.Name, recordInfo.Parameters[i].Type, this));

                FieldInitialicer[i].ILName = RecordIdentifier.ILName + string.Format(".{0}", assignNode.Field.Text);
            }
            ExpressionType = recordInfo;
            ExpressionType.ILName = scope.GetILTypeName(ExpressionType.Name);
            
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var type = symbols.Records[ExpressionType.ILName];
            var record = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(System.Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, record);
            generator.Emit(OpCodes.Ldloc, record);

            if (FieldInitialicer != null)
                foreach (var fieldDec in FieldInitialicer)
                {
                    fieldDec.Generate(generator, symbols);
                    generator.Emit(OpCodes.Stfld, type.GetField(fieldDec.ILName));
                    generator.Emit(OpCodes.Ldloc, record);
                }
        }

        #endregion

        
    }
}
