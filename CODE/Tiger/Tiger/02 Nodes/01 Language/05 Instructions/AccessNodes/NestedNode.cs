using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instructions.AccessNodes
{
    /// <summary>
    /// Stands for the anidation of access to fields or array items.
    /// </summary>
    internal abstract class NestedNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        internal NestedNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public NestedNode NextNested
        {
            get { return (Children.Count > 1) ? ((NestedNode)Children[1]) : (null); }
        }

        public override ItemInfo ExpressionType { get; set; }

        #endregion

        public void CheckSemantics(string identifier, Scope scope, List<SemanticError> errors)
        {
            NestedNode curreNode = this;
            do
            {
                //check the field access
                var typeInfo = scope.GetType(identifier);
                if (curreNode is FieldNestedNode)
                {
                    //check if the type is record
                    if (typeInfo.Type != TypesEnumeration.Record)
                    {
                        errors.Add(SemanticError.WrongType("Record", typeInfo.Type.ToString(), this));
                        return;
                    }
                    var recordInfo = (RecordInfo)typeInfo;
                    var fieldNested  = (FieldNestedNode) curreNode;
                    
                    var currentField = recordInfo.Parameters.FirstOrDefault(x => x.Identifier == fieldNested.FieldIdentifier.Text);
                    if (currentField == null)
                        errors.Add(SemanticError.InvalidFieldAccess(identifier,
                                                                    fieldNested.FieldIdentifier.Text, this));
                    else
                    {
                        fieldNested.FieldIdentifier.ILName = scope.GetILTypeName(identifier) + "." + fieldNested.FieldIdentifier.Text;
                        identifier = currentField.Type;
                    }
                }
                else
                {
                    //check if the type is an array
                    if (typeInfo.Type != TypesEnumeration.Array)
                    {
                        errors.Add(SemanticError.WrongType("Array", typeInfo.Type.ToString(), this));
                        return;
                    }
                    var arrayInfo = (ArrayInfo) typeInfo;
                    var arrayIndex = (IndexNestedNode) curreNode;
                    arrayIndex.CheckSemantics(scope, errors);
                    if (((IndexNestedNode)curreNode).Index.ExpressionType.Type != TypesEnumeration.Integer)
                        errors.Add(SemanticError.WrongType(((IndexNestedNode)curreNode).Index.ExpressionType.Name,"int",this));
                    else
                    {
                        identifier = arrayInfo.ItemsType;
                        //arrayIndex.Index.ILName = scope.GetILTypeName(identifier);
                    }

                    curreNode.ILName = scope.GetILTypeName(identifier);
                    //curreNode.ILName = scope.GetILTypeName(identifier) + "." + fieldNested.FieldIdentifier.Text;
                }
                curreNode = curreNode.NextNested;
            } while (curreNode != null);
            ExpressionType = scope.GetType(identifier);
            
        }
    }
}
