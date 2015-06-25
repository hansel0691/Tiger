using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;
using TypeInfo = Tiger._03_Semantics.TypeInfo;

namespace Tiger.AST_Nodes.Instructions
{
    /// <summary>
    /// Represent the call to functions already defined
    /// e.g id(return_expr, return_expr) 
    /// ^(FUNCTION_CALL $id  expr_list?)
    /// </summary>
    internal class CallRoutineNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public CallRoutineNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        //si es un procedimiento retornar void, en otro caso el tipo de retorno definido
        public override ItemInfo ExpressionType { get; set; }

        //public MethodInfo SymbolInfo { get; private set; }

        /// <summary>
        /// Identifier of the function you wanna call
        /// </summary>
        public IdNode FunctionId { get { return (IdNode)Children[0]; } }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <returns>returns the parameter in the index "index"</returns>
        public ArgumentListNode Arguments
        {
            get
            {
                return Children.Count == 2 ? (ArgumentListNode)Children[1] : null;
            }
        }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            if (Arguments != null)
                Arguments.CheckSemantics(scope, errors);
            var functionInfo = scope.GetRoutine(FunctionId.Text);
            //check if the function exist
            if (functionInfo == null)
            {
                errors.Add(SemanticError.FunctionDoesNotExist(FunctionId.Text, this));
                return;
            }

            if (functionInfo.ParameterCount != 0 && Arguments == null)
            {
                errors.Add(SemanticError.WrongParameterNumber("Function", FunctionId.Text, functionInfo.ParameterCount, 0, this));
                return;
            }
            if (Arguments != null && Arguments.Count != functionInfo.ParameterCount)
            {
                errors.Add(SemanticError.WrongParameterNumber("Function", FunctionId.Text, functionInfo.ParameterCount, this.Arguments.Count, this));
                return;
            }

            //var newScope = new Scope(scope);
            for (int i = 0; Arguments != null && i < Arguments.Count; i++)
            {
                //check the type of the parameters passed
                var expressionType = Arguments[i].ExpressionType;

                if (expressionType.Type == TypesEnumeration.Nil)
                {
                    if (!scope.GetType(functionInfo.ParametersType[i].Type).Nilable)
                        errors.Add(SemanticError.InvalidNilAssignation(functionInfo.ParametersType[i].Type, this));
                }
                else if (expressionType.Type == TypesEnumeration.Void ||  scope.GetType(expressionType.Name).Name != functionInfo.ParametersType[i].Type)
                    errors.Add(SemanticError.WrongType(functionInfo.ParametersType[i].Type, expressionType.Name, this));
            }
            ExpressionType = functionInfo.ReturnType != "void" ? scope.GetType(functionInfo.ReturnType) : TypeInfo.GenerateVoidInfo();
            ILName = scope.GetILRoutineName(FunctionId.Text);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            if (Arguments != null)
                foreach (var arg in Arguments)
                    arg.Generate(generator, symbols);
            generator.Emit(OpCodes.Call, symbols.Routines[this.ILName]);
        }

        #endregion
    }
}
