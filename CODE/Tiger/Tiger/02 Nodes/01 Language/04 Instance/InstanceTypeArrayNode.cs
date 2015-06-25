using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Instance
{
    /// <summary>
    /// Stands for inicialice an array.
    /// id [return_expr] of return_expr
    /// var row := intArray [ N ] of 0
    /// </summary>
    internal class InstanceTypeArrayNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public InstanceTypeArrayNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; }

        /// <summary>
        /// Type of the array
        /// </summary>
        public IdNode ArrayIdentifier { get { return (IdNode)Children[0]; } }

        /// <summary>
        /// Length of the array
        /// </summary>
        public ValuedExpressionNode Length { get { return (ValuedExpressionNode)Children[1]; } }

        /// <summary>
        /// Default value to fill a new array
        /// </summary>
        public ValuedExpressionNode DefaultValue { get { return (ValuedExpressionNode)Children[2]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            this.Length.CheckSemantics(scope, errors);
            this.DefaultValue.CheckSemantics(scope, errors);

            //check if the array identifier is defined 
            var typeInfo = scope.GetType(this.ArrayIdentifier.Text);
            if (typeInfo == null)
            {
                errors.Add(SemanticError.TypeNotDefined(this.ArrayIdentifier.Text, this));
                return;
            }
            /*if (scope.GetType(typeInfo.Name).Name != scope.GetType(ArrayIdentifier.Text).Name)
            {
                errors.Add(SemanticError.WrongType(ExpressionType.Name, typeInfo.Name, this));
                return;
            }*/
            if (typeInfo.Type != TypesEnumeration.Array)
            {
                errors.Add(SemanticError.WrongType("Array", typeInfo.Name, this));
                return;
            }

            var arrayInfo = (ArrayInfo) typeInfo;
            //check if lenght is int
            if (this.Length.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.InvalidNumber("The length", this));
            //default value must be the same type of the defined
            if (DefaultValue.ExpressionType.Type == TypesEnumeration.Nil)
            {
                if (!scope.GetType(arrayInfo.ItemsType).Nilable)
                    errors.Add(SemanticError.InvalidNilAssignation(arrayInfo.ItemsType, this));
            }
            else if (arrayInfo.ItemsType != scope.GetType(DefaultValue.ExpressionType.Name).Name)
                errors.Add(SemanticError.WrongType(arrayInfo.ItemsType, this.DefaultValue.ExpressionType.Name, this));

            this.ExpressionType = scope.GetType(ArrayIdentifier.Text);
            ArrayIdentifier.ILName = string.Format("{0}Scope{1}", ArrayIdentifier, scope.CurrentScope);
            DefaultValue.ILName = scope.GetILTypeName(arrayInfo.ItemsType);
        }


        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var array = generator.DeclareLocal(symbols.ArraysTypes[ArrayIdentifier.ILName]);
            var itemsType = symbols.GetRealType(DefaultValue.ILName);
            var index = generator.DeclareLocal(typeof(int));
            var lenght = generator.DeclareLocal(typeof (int));
            var item = generator.DeclareLocal(itemsType);

            var loop = generator.DefineLabel();
            var end = generator.DefineLabel();

            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Stloc, index);

            Length.Generate(generator, symbols);
            generator.Emit(OpCodes.Stloc, lenght);

            generator.Emit(OpCodes.Ldloc, lenght);
            generator.Emit(OpCodes.Newarr, itemsType);
            generator.Emit(OpCodes.Stloc, array);

            generator.MarkLabel(loop);
            generator.Emit(OpCodes.Ldloc, index);
            generator.Emit(OpCodes.Ldloc, lenght);
            generator.Emit(OpCodes.Bge, end);

            DefaultValue.Generate(generator, symbols);
            generator.Emit(OpCodes.Stloc, item);

            generator.Emit(OpCodes.Ldloc, array);
            generator.Emit(OpCodes.Ldloc, index);
            generator.Emit(OpCodes.Ldloc, item);
            generator.Emit(OpCodes.Stelem, itemsType);

            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Ldloc, index);
            generator.Emit(OpCodes.Add);
            generator.Emit(OpCodes.Stloc, index);
            generator.Emit(OpCodes.Br, loop);
            generator.MarkLabel(end);

            generator.Emit(OpCodes.Ldloc, array);
        }

        #endregion
    }
}
