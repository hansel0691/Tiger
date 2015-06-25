using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.FlowControl
{
    /// <summary>
    /// Represents the bucle for
    /// for id := expr to expr do expr
    /// </summary>
    internal class  ForNode : StatementNode
    {
        #region CONSTRUCTORS:

        public ForNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// Identifier of the id that will be used to iterate the loop.
        /// </summary>
        public IdNode IteratorName { get { return ((IdNode)Children[0]); } }
        
        /// <summary>
        /// Lower iteration value.
        /// </summary>
        public ValuedExpressionNode LowerValue { get { return ((ValuedExpressionNode)Children[1]); } }

        /// <summary>
        /// Highest iteration value.
        /// </summary>
        public ValuedExpressionNode HighestValue { get { return ((ValuedExpressionNode)Children[2]); } }

        /// <summary>
        /// Expression that represents the body of the loop bucle.
        /// </summary>
        public ExpressionNode Loop { get { return ((ExpressionNode)Children[3]); } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            this.LowerValue.CheckSemantics(scope, errors);
            this.HighestValue.CheckSemantics(scope, errors);

            if (scope.ContainsType(IteratorName.Text, true))
                errors.Add(SemanticError.DefinedVariable(IteratorName.Text, this));

            if (this.LowerValue.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.InvalidForExpression("lower", this));
            if (this.HighestValue.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.InvalidForExpression("upper", this));
            
            var newScope = new Scope(scope);
            newScope.AddVar(this.IteratorName.Text, new VariableInfo(this.IteratorName.Text, "int"){ReadOnly =  true});
            this.Loop.CheckSemantics(newScope, errors);
            //loop may not return a value
            if (Loop.ExpressionType.Type != TypesEnumeration.Void)
                errors.Add(SemanticError.DontReturnExpression("For body", this));

            IteratorName.ILName = newScope.GetILVarNames(IteratorName.Text);
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            var iteratorVar = symbols.ProgramType.DefineField(IteratorName.ILName, typeof(int),
                                                           FieldAttributes.Public | FieldAttributes.Static);

            var highestValueVar = generator.DeclareLocal(typeof(int));
            var loop = generator.DefineLabel();
            var end = generator.DefineLabel();
            symbols.NestedCylesBreakJumps.Push(end);

            symbols.Variables.Add(IteratorName.ILName, null);
            LowerValue.Generate(generator, symbols);
            generator.Emit(OpCodes.Stsfld, iteratorVar);
            HighestValue.Generate(generator, symbols);
            generator.Emit(OpCodes.Stloc, highestValueVar);
            
            generator.MarkLabel(loop);
            symbols.Variables[IteratorName.ILName] = iteratorVar;
            generator.Emit(OpCodes.Ldsfld, iteratorVar);
            generator.Emit(OpCodes.Ldloc, highestValueVar);
            generator.Emit(OpCodes.Bgt, end);

            Loop.Generate(generator, symbols);
            generator.Emit(OpCodes.Ldsfld, iteratorVar);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Add);
            generator.Emit(OpCodes.Stsfld, iteratorVar);

            generator.Emit(OpCodes.Br, loop);
            generator.MarkLabel(end);

            symbols.Variables.Remove(IteratorName.ILName);
            symbols.NestedCylesBreakJumps.Pop();
        }

        #endregion
    }
}
