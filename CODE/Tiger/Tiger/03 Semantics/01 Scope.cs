using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tiger._03_Semantics;
using RoutineInfo = Tiger._03_Semantics.RoutineInfo;
using TypesEnumeration = Tiger._03_Semantics.TypesEnumeration;

namespace Tiger
{

    /// <summary>
    /// Clase que contiene la informacion
    /// necesaria de las declaraciones de tipos,
    /// funciones y variables
    /// </summary>
    public class Scope
    {
        #region FIELDS:

        public Dictionary<string, TypeInfo> Types { get; set; }
        public Dictionary<string, VariableInfo> Variables { get; set; }
        public Dictionary<string, RoutineInfo> Routines { get; set; }

        public Scope Parent { get; set; }
        public List<Scope> ChildenScopes { get; set; }

        // Es el numero de la clase en si que perdura
        public static int NumberScopes { get; set; }
        public int CurrentScope { get; set; }


        public static List<string> standard_functions = new List<string>()
        {
            "print",
            "printi",
            "ord",
            "chr",
            "size",
            "substring",
            "concat",
            "not",
            "exit",
            "getline",
            "printline",
            "printiline"
        };

        #endregion
        #region CONSTRUCTORS:

        public Scope(Scope parent)
        {
            Parent = parent;
            Types = new Dictionary<string, TypeInfo>();
            Variables = new Dictionary<string, VariableInfo>();
            Routines = new Dictionary<string, RoutineInfo>();

            NumberScopes = parent != null ? NumberScopes + 1 : 0;
            CurrentScope = NumberScopes;

            if (parent != null)
            {
                if (parent.ChildenScopes == null)
                    parent.ChildenScopes = new List<Scope>();
                parent.ChildenScopes.Add(this);
            }
        }

        public Scope() :
            this(null)
        { }

        #endregion
        #region METHODS:
        public void SetTypes()
        {
            Types.Add("int", TypeInfo.GenerateIntInfo());
            Types.Add("string", TypeInfo.GenerateStringInfo());
            Types.Add("nil", TypeInfo.GenerateNilInfo());
            Types.Add("void", TypeInfo.GenerateVoidInfo());
        }
        
        public void SetFunctions()
        {
            Routines.Add("print", new RoutineInfo("print", "void", new List<ParameterInfo> { new ParameterInfo("s", "string") }));
            Routines.Add("printi", new RoutineInfo("printi", "void", new List<ParameterInfo> { new ParameterInfo("i", "int") }));
            Routines.Add("ord", new RoutineInfo("ord", "int", new List<ParameterInfo> { new ParameterInfo("s", "string") }));
            Routines.Add("chr", new RoutineInfo("chr", "string", new List<ParameterInfo> { new ParameterInfo("i", "int") }));
            Routines.Add("size", new RoutineInfo("size", "int", new List<ParameterInfo> { new ParameterInfo("s", "string") }));
            Routines.Add("substring", new RoutineInfo("substring", "string", new List<ParameterInfo>
                                                                                                      {
                                                                                                          new ParameterInfo("s", "string"),
                                                                                                          new ParameterInfo("f", "int"),
                                                                                                          new ParameterInfo("n", "int"),
                                                                                                      }));
            Routines.Add("concat", new RoutineInfo("concat", "string", new List<ParameterInfo>
                                                                                                      {
                                                                                                          new ParameterInfo("s1", "string"),
                                                                                                          new ParameterInfo("s2", "string")
                                                                                                      }));
            Routines.Add("not", new RoutineInfo("not", "int", new List<ParameterInfo> { new ParameterInfo("i", "int") }));
            Routines.Add("exit", new RoutineInfo("exit", "void", new List<ParameterInfo> { new ParameterInfo("i", "int") }));
            Routines.Add("getline", new RoutineInfo("getline", "string"));
            Routines.Add("printline", new RoutineInfo("printline", "void", new List<ParameterInfo> { new ParameterInfo("s", "string") }));
            Routines.Add("printiline", new RoutineInfo("printiline", "void", new List<ParameterInfo> { new ParameterInfo("i", "int") }));
        }

        public void AddRoutine(string name, RoutineInfo routin)
        {
            if (!Routines.ContainsKey(name))
                Routines.Add(name, routin);
        }
        public void AddType(string name, TypeInfo type)
        {
            if (!Types.ContainsKey(name))
                Types.Add(name, type);
        }
        public void AddVar(string name, VariableInfo var)
        {
            if (!Variables.ContainsKey(name))
                Variables.Add(name, var);
        }

        public bool ContainsType(string name, bool inScope = false)
        {
            return name != null && ((Types.ContainsKey(name)) || (!inScope && (Parent != null) && Parent.ContainsType(name)));
        }

        public bool ContainsRoutine(string name, bool inScope = false)
        {
            return name != null && ((Routines.ContainsKey(name)) || (!inScope && (Parent != null) && Parent.ContainsRoutine(name)));
        }

        public bool ContainsVarInstance(string name, bool inScope = false)
        {
            return name != null && ((Variables.ContainsKey(name)) || (!inScope && (Parent != null) && Parent.ContainsVarInstance(name)));
        }

        public TypeInfo GetType(string name)
        {
            TypeInfo result;
            return (name != null && Types.TryGetValue(name, out result)) ? result : Parent != null ? Parent.GetType(name) : null;
        }
        public RoutineInfo GetRoutine(string name)
        {
            RoutineInfo result;
            return (name != null && Routines.TryGetValue(name, out result))
                       ? result
                       : Parent != null ? Parent.GetRoutine(name) : null;
        }
        public VariableInfo GetVarInstance(string name)
        {
            VariableInfo result;
            return name != null && Variables.TryGetValue(name, out result)
                       ? result
                       : Parent != null ? Parent.GetVarInstance(name) : null;
        }

        public string GetILTypeName(string typeId)
        {
            return Types.ContainsKey(typeId)
                        ? typeId + string.Format("Scope{0}", CurrentScope)
                        : Parent != null ? Parent.GetILTypeName(typeId) : null;
        }

        public string GetILRoutineName(string routinId)
        {
            return Routines.ContainsKey(routinId)
                        ? routinId + string.Format("Scope{0}", CurrentScope)
                        : Parent != null ? Parent.GetILRoutineName(routinId) : null;
        }

        public string GetILVarNames(string varId)
        {
            return Variables.ContainsKey(varId)
                       ? varId + string.Format("Scope{0}", CurrentScope)
                       : Parent != null ? Parent.GetILVarNames(varId) : null;
        }

        #endregion
    }
}
