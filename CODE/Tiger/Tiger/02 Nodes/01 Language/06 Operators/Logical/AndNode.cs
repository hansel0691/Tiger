using System.Reflection.Emit;
using Antlr.Runtime;

namespace Tiger.AST_Nodes.Operators.Logical
{
    /// <summary>
    /// Stands for the & operator.
    /// </summary>
    internal class AndNode : LogicalNode
    {
        #region CONSTRUCTORS:

        public AndNode(IToken token) : base(token) { }

        #endregion

        public override void Generate(ILGenerator generator, Symbols s)
        {
            var Is_False = generator.DefineLabel();
            var End = generator.DefineLabel();

            LeftOperand.Generate(generator, s);
            //Pregunto si hay un 0 en el tope d la pila
            generator.Emit(OpCodes.Ldc_I4_0);
            //Si es verdad salto para la etiqueta es falso
            generator.Emit(OpCodes.Beq, Is_False);

            RightOperand.Generate(generator, s);
            //Pregunto si hay un 0 en el tope d la pila
            generator.Emit(OpCodes.Ldc_I4_0);
            //Si es verdad salto para la etiqueta es falso
            generator.Emit(OpCodes.Beq, Is_False);
            //Si hay un 1 en el tope es porque no salte para el label que me dice si hay algun false ,
            //por lo q el primer operando es true y entonces es true el and
            generator.Emit(OpCodes.Ldc_I4_1);

            generator.Emit(OpCodes.Br, End);

            generator.MarkLabel(Is_False);

            generator.Emit(OpCodes.Ldc_I4_0);
            generator.MarkLabel(End);

        }
    }
}
