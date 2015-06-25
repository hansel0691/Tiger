using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Declarations;
using Tiger.AST_Nodes.Declarations.DeclarationBlocks;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.FlowControl
{
    /// <summary>
    /// Represent the le in end statament.
    /// let declaration-list in expr-seq? end
    /// </summary>
    internal class LetInEndNode : ValuedExpressionNode
    {
        #region CONSTRUCTORS:

        public LetInEndNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        public override ItemInfo ExpressionType { get; set; } 

        public DeclarationBlock DeclarationBlock { get { return (DeclarationBlock)Children[0]; } }

        public ExpressionSequence InstructionsBlock { get { return Children.Count == 2 ? (ExpressionSequence)Children[1] : null; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            var newScope = new Scope(scope);
            DeclarationBlock.CheckSemantics(newScope, errors);
            if (InstructionsBlock != null)
                InstructionsBlock.CheckSemantics(newScope, errors);
            
            //check the visibility of the returned type. 
            ExpressionType = InstructionsBlock == null ? TypeInfo.GenerateVoidInfo() : this.InstructionsBlock.ExpressionType;
            if (ExpressionType != null && ExpressionType.Type != TypesEnumeration.Void && !scope.ContainsType(ExpressionType.Name))
                errors.Add(SemanticError.TypeNotVisible(ExpressionType.Name, this));
            ExpressionType.ILName = scope.GetILTypeName(ExpressionType.Name);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            
            //Save the previous variables
            var currentVariables = new List<string>();
            foreach (var item in symbols.Variables)
            {
                currentVariables.Add(item.Key);
                generator.Emit(OpCodes.Ldsfld, symbols.Variables[item.Key]);
            }
            
            DeclarationBlock.Generate(generator, symbols);
            InstructionsBlock.Generate(generator, symbols);

            LocalBuilder returnValue = null;
            if (InstructionsBlock.ExpressionType.Type != TypesEnumeration.Void)
            {
                returnValue = generator.DeclareLocal(symbols.GetRealType(InstructionsBlock.ExpressionType.ILName));
                generator.Emit(OpCodes.Stloc, returnValue);
            }
            
            currentVariables.Reverse();
            //load in the variables it's previous values
            currentVariables.ForEach(x => generator.Emit(OpCodes.Stsfld, symbols.Variables[x]));
            if (InstructionsBlock.ExpressionType.Type != TypesEnumeration.Void)
                generator.Emit(OpCodes.Ldloc, returnValue);
        }

        #endregion
    }
}
