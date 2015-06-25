using System.Reflection.Emit;
using Antlr.Runtime;

namespace Tiger.AST_Nodes.Operators.Logical
{
    /// <summary>
    /// Stands for the | operator.
    /// </summary>
    internal class OrNode : LogicalNode
    {
        #region CONSTRUCTORS:

        public OrNode(IToken token) : base(token) { }

        #endregion

        public override void Generate(ILGenerator generator, Symbols s)
        {
            var Is_True = generator.DefineLabel();
            var End = generator.DefineLabel();

            LeftOperand.Generate(generator, s);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Bne_Un, Is_True);

            RightOperand.Generate(generator, s);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Bne_Un, Is_True);

            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Br, End);

            generator.MarkLabel(Is_True);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.MarkLabel(End);
        }
    }
}
