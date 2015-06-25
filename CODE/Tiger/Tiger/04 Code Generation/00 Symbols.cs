using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

namespace Tiger
{   // Tabla de simbolos de las variables, funciones y tipos
    public class Symbols
    {
        #region Properties

        public AssemblyBuilder Assembly { get; set; }
        public AssemblyName AssemblyName { get; set; }
        public ModuleBuilder ModuleBuilder { get; set; }
        public TypeBuilder ProgramType { get; set; }
        public MethodBuilder MainMethod { get; set; }
        public Stack<Label> NestedCylesBreakJumps { get; set; }

        public Dictionary<string, TypeBuilder> Records { get; set; }
        public Dictionary<string, FieldBuilder> Variables { get; set; }
        public Dictionary<string, Type> ArraysTypes { get; set; }
        public Dictionary<string, MethodBuilder> Routines { get; set; }

        #endregion

        public Symbols(string outputPath) 
        {
            string name = Path.GetFileNameWithoutExtension(outputPath);
            AssemblyName = new AssemblyName(name);

            name = "";
            var dir = Path.GetFullPath(outputPath).Split('\\');
            for (int i = 0; i < dir.Length - 1; i++)
                name += dir[i] + "\\";
            
            Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndSave, name);
            NestedCylesBreakJumps = new Stack<Label>();
            ModuleBuilder = Assembly.DefineDynamicModule(AssemblyName.Name, AssemblyName.Name + ".exe");
            ProgramType = ModuleBuilder.DefineType("Program", TypeAttributes.Public);
            MainMethod = ProgramType.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static, typeof(void), Type.EmptyTypes);


            ArraysTypes = new Dictionary<string, Type>();
            Records = new Dictionary<string, TypeBuilder>();
            Routines = new Dictionary<string, MethodBuilder>();
            Variables = new Dictionary<string, FieldBuilder>();

            ArraysTypes.Add("intScope0", Type.GetType("System.Int32"));
            ArraysTypes.Add("stringScope0", Type.GetType("System.String"));
            StandardsRoutines();
        }

        /// <summary>
        /// Este metodo obtiene el nombre de un tipo tiger y devuelve un tipo C#
        /// </summary>
        /// <param name="typeName">Nombre IL del tipo</param>
        /// <returns>Retorna el tipo real en C#</returns>
        public Type GetRealType(string typeName)
        {
            switch (typeName)
            {
                case "intScope0":
                    return Type.GetType("System.Int32");
                case "stringScope0":
                    return Type.GetType("System.String");
                case "nilScope0":
                    return Type.GetType("System.Nullable");
                case "voidScope0":
                    return Type.GetType("System.Void");
                default:
                    {
                        if (Records.ContainsKey(typeName))
                            return Records[typeName];
                        return ArraysTypes[typeName];
                    }
            }
        }

        public void StandardsRoutines()
        {
            Routines.Add("printScope0", Print());
            Routines.Add("printiScope0", PrintI());
            Routines.Add("substringScope0", Substring());
            Routines.Add("ordScope0", Ord());
            Routines.Add("chrScope0", Chr());
            Routines.Add("sizeScope0", Size());
            Routines.Add("concatScope0", Concat());
            Routines.Add("notScope0", Not());
            Routines.Add("exitScope0", Exit());
            Routines.Add("getlineScope0", GetLine());
            Routines.Add("printlineScope0", PrintLine("printline", typeof(string)));
            Routines.Add("printilineScope0", PrintLine("printiline", typeof(int)));
        }

        private MethodBuilder Print()
        {
            var paramTypes = new [] { typeof(string) };

            MethodBuilder method = ProgramType.DefineMethod("print", MethodAttributes.Static | MethodAttributes.Public, typeof(void), paramTypes);
            var printIL = method.GetILGenerator();

            printIL.Emit(OpCodes.Ldarg_0);
            printIL.EmitCall(OpCodes.Call, typeof(Console).GetMethod("Write", paramTypes), paramTypes);
            printIL.Emit(OpCodes.Ret);

            return method;
        }

        private MethodBuilder PrintI()
        {
            var parameters = new [] { typeof(int) };

            MethodBuilder methodFunc = ProgramType.DefineMethod("printi", MethodAttributes.Static | MethodAttributes.Public, typeof(void), parameters);
            var printiIL = methodFunc.GetILGenerator();

            printiIL.Emit(OpCodes.Ldarg_0);
            printiIL.EmitCall(OpCodes.Call, typeof(Console).GetMethod("Write", parameters), parameters);
            printiIL.Emit(OpCodes.Ret);

            return methodFunc;
        }

        private MethodBuilder Substring()
        {
            var parameters = new [] { typeof(string), typeof(int), typeof(int) };

            MethodBuilder methodFunc = ProgramType.DefineMethod("substring", MethodAttributes.Static | MethodAttributes.Public, typeof(string), parameters);
            var substringiL = methodFunc.GetILGenerator();


            //PONE EL ARGUMENTO DEL INDICE 0 EN LA PILA DE EVALUACION
            substringiL.Emit(OpCodes.Ldarg_0);
            substringiL.Emit(OpCodes.Ldarg_1);
            substringiL.Emit(OpCodes.Ldarg_2);

            var callParameterTypes = new [] { typeof(int), typeof(int) };
            substringiL.Emit(OpCodes.Call, typeof(string).GetMethod("Substring", callParameterTypes));
            substringiL.Emit(OpCodes.Ret);

            return methodFunc;
        }

        private MethodBuilder Ord()
        {
            var parameters = new [] {typeof (string)};
            MethodBuilder methodFunc = ProgramType.DefineMethod("ord", MethodAttributes.Static | MethodAttributes.Public,
                                                                typeof (int), parameters);

            var ordIL = methodFunc.GetILGenerator();
            var input = ordIL.DeclareLocal(typeof (string));
            var end = ordIL.DefineLabel();
            var nullEntry = ordIL.DefineLabel();
            
            ordIL.Emit(OpCodes.Ldarg_0);
            ordIL.Emit(OpCodes.Stloc, input);
            ordIL.Emit(OpCodes.Ldloc, input);
            ordIL.Emit(OpCodes.Ldstr, "");
            ordIL.Emit(OpCodes.Beq, nullEntry);

            ordIL.Emit(OpCodes.Ldloc, input);
            ordIL.Emit(OpCodes.Ldc_I4_0);
            ordIL.Emit(OpCodes.Call, typeof(char).GetMethod("ConvertToUtf32", new []{typeof(string), typeof(int)}));
            ordIL.Emit(OpCodes.Br, end);
            
            ordIL.MarkLabel(nullEntry);
            ordIL.Emit(OpCodes.Ldc_I4_0);

            ordIL.MarkLabel(end);
            ordIL.Emit(OpCodes.Ret);
            return methodFunc;
        }

        private MethodBuilder Chr()
        {
            var parameters = new[] {typeof (int)};
            MethodBuilder methodFunc = ProgramType.DefineMethod("chr", MethodAttributes.Static | MethodAttributes.Public,
                                                                typeof (string), parameters);
            var chrIL = methodFunc.GetILGenerator();
            var end = chrIL.DefineLabel();
            var outOfRange = chrIL.DefineLabel();
            var input = chrIL.DeclareLocal(typeof(int));

            chrIL.Emit(OpCodes.Ldarg_0);
            chrIL.Emit(OpCodes.Stloc, input);
            chrIL.Emit(OpCodes.Ldloc, input);
            chrIL.Emit(OpCodes.Ldc_I4, 255);
            chrIL.Emit(OpCodes.Bgt, outOfRange);
            chrIL.Emit(OpCodes.Ldloc, input);
            chrIL.Emit(OpCodes.Ldc_I4_0);
            chrIL.Emit(OpCodes.Ble, outOfRange);

            chrIL.Emit(OpCodes.Ldloc, input);
            chrIL.Emit(OpCodes.Call, typeof(char).GetMethod("ConvertFromUtf32", parameters));
            
            chrIL.Emit(OpCodes.Br, end);
            
            chrIL.MarkLabel(outOfRange);
            chrIL.Emit(OpCodes.Ldc_I4_1);
            chrIL.Emit(OpCodes.Call, typeof(Environment).GetMethod("Exit", new[] { typeof(int) }));
            
            chrIL.MarkLabel(end);

            chrIL.Emit(OpCodes.Ret);
            return methodFunc;
        }

        private MethodBuilder Size()
        {
            var parameters = new[] {typeof (string)};
            MethodBuilder methodFunc = ProgramType.DefineMethod("size", MethodAttributes.Static | MethodAttributes.Public,
                                                            typeof (int), parameters);
            var sizeIL = methodFunc.GetILGenerator();
            sizeIL.Emit(OpCodes.Ldarg_0);
            sizeIL.Emit(OpCodes.Call, typeof(String).GetProperty("Length").GetGetMethod());
            sizeIL.Emit(OpCodes.Ret);
            return methodFunc;
        }

        private  MethodBuilder Concat()
        {
            var parameters = new[] { typeof(string), typeof(string) };
            MethodBuilder methodFunc = ProgramType.DefineMethod("concat", MethodAttributes.Static | MethodAttributes.Public,
                                                            typeof(string), parameters);
            var concatIL = methodFunc.GetILGenerator();
            concatIL.Emit(OpCodes.Ldarg_0);
            concatIL.Emit(OpCodes.Ldarg_1);
            concatIL.Emit(OpCodes.Call, typeof(String).GetMethod("Concat", parameters));
            concatIL.Emit(OpCodes.Ret);
            return methodFunc;
        }

        private MethodBuilder Not()
        {
            var parameters = new[] { typeof(int) };
            MethodBuilder methodFunc = ProgramType.DefineMethod("not", MethodAttributes.Static | MethodAttributes.Public,
                                                            typeof(int), parameters);
            var notIL = methodFunc.GetILGenerator();
            var isTrue = notIL.DefineLabel();
            var end = notIL.DefineLabel();
            
            notIL.Emit(OpCodes.Ldarg_0);
            notIL.Emit(OpCodes.Ldc_I4_0);
            notIL.Emit(OpCodes.Bne_Un, isTrue);
            notIL.Emit(OpCodes.Ldc_I4_1);
            notIL.Emit(OpCodes.Br, end);
            notIL.MarkLabel(isTrue);
            notIL.Emit(OpCodes.Ldc_I4_0);
            notIL.MarkLabel(end);
            notIL.Emit(OpCodes.Ret);
            return methodFunc;
        }

        private MethodBuilder Exit()
        {
            var parameters = new[] { typeof(int) };
            MethodBuilder methodFunc = ProgramType.DefineMethod("exit", MethodAttributes.Static | MethodAttributes.Public,
                                                            typeof(void), parameters);
            var exitIL = methodFunc.GetILGenerator();
            exitIL.Emit(OpCodes.Ldarg_0);
            exitIL.Emit(OpCodes.Call, typeof(Environment).GetMethod("Exit", parameters));
            exitIL.Emit(OpCodes.Ret);
            return methodFunc;
        }

        private MethodBuilder GetLine()
        {
            var parameter = Type.EmptyTypes;
            MethodBuilder methodFunc = ProgramType.DefineMethod("getline", MethodAttributes.Static | MethodAttributes.Public,
                                                            typeof(string), parameter);
            var getLineIL = methodFunc.GetILGenerator();
            getLineIL.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine", parameter));
            getLineIL.Emit(OpCodes.Ret);
            return methodFunc;
        }

        private MethodBuilder PrintLine(string funcName, params Type[] parameter)
        {
            MethodBuilder methodFunc = ProgramType.DefineMethod(funcName, MethodAttributes.Static | MethodAttributes.Public,
                                                            typeof(void), parameter);
            var printiIL = methodFunc.GetILGenerator();
            printiIL.Emit(OpCodes.Ldarg_0);
            printiIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", parameter));
            printiIL.Emit(OpCodes.Ret);
            return methodFunc;
        }
    }
    
}
