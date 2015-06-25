using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.FlowControl
{
    /// <summary>
    /// represents the bucle while.
    /// while expr do expr
    /// </summary>
    internal class WhileNode : StatementNode
    {
        #region CONSTRUCTORS:

        public WhileNode(IToken tok) : base(tok) { }

        #endregion
        #region PROPERTIES:

        /// <summary>
        /// condition that has to be true for one execution of the loop.
        /// </summary>
        public ValuedExpressionNode Condition { get { return (ValuedExpressionNode)Children[0]; } }

        /// <summary>
        /// Body of the while that will be executed every time the condition comes true.
        /// </summary>
        public ExpressionNode Loop { get { return (ExpressionNode)Children[1]; } }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            this.Condition.CheckSemantics(scope,errors);
            this.Loop.CheckSemantics(scope, errors);
            //condition must be int
            if (this.Condition.ExpressionType.Type != TypesEnumeration.Integer)
                errors.Add(SemanticError.InvalidWileCondition(this));
            //loop must not return any value
            if (Loop.ExpressionType.Type != TypesEnumeration.Void)
                errors.Add(SemanticError.DontReturnExpression("While-Do statement", this));
        }


        public override void Generate(ILGenerator generator, Symbols s)
        {
            // Definir los lables necesarios
            var loopLabel = generator.DefineLabel();
            var endLabel = generator.DefineLabel();

            // Guardar el posible break que podria existir en el codigo
            s.NestedCylesBreakJumps.Push(endLabel);

            // Salto a la evaluacion de la condicion
            generator.MarkLabel(loopLabel);
            Condition.Generate(generator, s);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Beq, endLabel);
            // Codigo del cuerpo
            Loop.Generate(generator, s);
            // looping
            generator.Emit(OpCodes.Br, loopLabel);
            generator.MarkLabel(endLabel);
            // Verificar que en el tope de la pila quede el resultado apropiado
            s.NestedCylesBreakJumps.Pop();
        }

        #endregion

        
    }
}
