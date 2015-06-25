using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations.DeclarationBlocks;
using Tiger.AST_Nodes.Instructions;
using Antlr.Runtime;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Declarations
{
    /// <summary>
    /// Stands for the declaration of a routine.
    /// </summary>
    internal abstract class RoutineDeclarationNode : DeclarationNode
    {
        #region CONSTRUCTORS:

        protected RoutineDeclarationNode(IToken tok)
            : base(tok)
        {

        }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// Identifier of the routine.
        /// </summary>
        public override IdNode Identifier { get { return (IdNode)Children[0]; } }

        public abstract FieldDeclarationBlock Arguments { get; }

        public abstract IdNode ReturnType { get; }

        #endregion
        #region METHODS:

        protected void CheckFunctionId(Scope scope, List<SemanticError> errors)
        {
            //check if the functionId is defined as a standard function 
            if (Scope.standard_functions.Contains(Identifier.Text))
                errors.Add(SemanticError.StandardFunctionDeclaration(Identifier.Text, this));
          
        }

        protected Scope CreateFunctionScope(Scope scope, List<SemanticError> errors)
        {
            var newScope = new Scope(scope);
            if (Arguments != null)
            {
                Arguments.CheckSemantics(scope, errors);
                for (int i = 0; Arguments != null && i < Arguments.Count; i++)
                {
                    newScope.AddVar(Arguments[i].Field.Text, new VariableInfo(Arguments[i].Field.Text, Arguments[i].TypeName.Text));
                    Arguments[i].ILName = newScope.GetILVarNames(Arguments[i].Field.Text);
                }
            }
            return newScope;
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var paramsTypes = Arguments != null ? new Type[Arguments.Count] : System.Type.EmptyTypes;

            for (int i = 0; i < paramsTypes.Length; i++)
            {
                var paramType = symbols.GetRealType(Arguments[i].TypeName.ILName);
                var parameter = symbols.ProgramType.DefineField(
                                            Arguments[i].ILName, paramType,
                                            FieldAttributes.Public |
                                            FieldAttributes.Static);

                symbols.Variables.Add(Arguments[i].ILName, parameter);
                paramsTypes[i] = paramType;
            }

            var returnType = symbols.GetRealType(ReturnType != null ? ReturnType.ILName : "voidScope0");
            var routin = symbols.ProgramType.DefineMethod(Identifier.ILName,
                                                          MethodAttributes.Public | MethodAttributes.Static, returnType,
                                                          paramsTypes);
            var routinIL = routin.GetILGenerator();
            symbols.Routines.Add(Identifier.ILName, routin);

            for (int i = 0; i < paramsTypes.Length; i++) //l_0000
                routinIL.Emit(OpCodes.Ldsfld, symbols.Variables[Arguments[i].ILName]);
            
            //var currentVariables = new List<string>();
            //foreach (var item in symbols.Variables)
            //{
            //    currentVariables.Add(item.Key);
            //    routinIL.Emit(OpCodes.Ldsfld, symbols.Variables[item.Key]);
            //}

            for (int i = 0; i < paramsTypes.Length; i++)
            {
                routinIL.Emit(OpCodes.Ldarg, i); //ldarg A_0
                routinIL.Emit(OpCodes.Stsfld, symbols.Variables[Arguments[i].ILName]); //l_000b
            }

            if (this is FunctionDeclarationNode)
                ((FunctionDeclarationNode)this).Body.Generate(routinIL, symbols);
            else
                ((ProcedureDeclarationNode)this).Body.Generate(routinIL, symbols);  

            LocalBuilder returnVariable = null;
            if (ReturnType != null)
            {
                returnVariable = routinIL.DeclareLocal(returnType);
                routinIL.Emit(OpCodes.Stloc, returnVariable);
            }

            for (int i = paramsTypes.Length - 1; i >= 0; i--)
                routinIL.Emit(OpCodes.Stsfld, symbols.Variables[Arguments[i].ILName]);

            //currentVariables.Reverse();
            //currentVariables.ForEach(x => routinIL.Emit(OpCodes.Stsfld, symbols.Variables[x]));

            if (ReturnType != null)
                routinIL.Emit(OpCodes.Ldloc, returnVariable);
            
            routinIL.Emit(OpCodes.Ret);
            
            if (Arguments != null)
                foreach (var argument in Arguments)
                    symbols.Variables.Remove(argument.Field.ILName);
        }

        #endregion
    }
}
