using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tiger._03_Semantics
{
    public enum TypesEnumeration
    {
        String,
        Integer,
        Record,
        Array,
        Function,
        Procedure,
        Variable,
        Nil,
        Alias,
        Void
    }

    public abstract class ItemInfo
    {
        protected ItemInfo(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
        public TypesEnumeration Type { get; set; }
        public string ILName { get; set; }
    }

    public class VariableInfo : ItemInfo
    {
        public VariableInfo (string name, string variableType) : base(name)
        {
            this.VariableType = variableType;
            Type = TypesEnumeration.Variable;
        }

        public bool ReadOnly { get; set; }
        public string VariableType { get; set; }
    }

    public class RoutineInfo : ItemInfo
    {
        public RoutineInfo(string name, string returnType, IEnumerable<ParameterInfo> parameters = null)
            : base(name)
        {
            if (parameters != null)
                this.ParametersType = new List<ParameterInfo>(parameters);
            this.ReturnType = returnType;
            Type = TypesEnumeration.Function;
        }

        public int ParameterCount { get { return ParametersType == null ? 0 : ParametersType.Count; } }
        public List<ParameterInfo> ParametersType { get; set; }
        public string ReturnType { get; set; }

    }

    public class TypeInfo : ItemInfo
    {
        public TypeInfo(string name, TypesEnumeration type) : base(name)
        {
            this.Type = type;
        }
        public bool Nilable { get
        {
            return Type == TypesEnumeration.Array || Type == TypesEnumeration.String || Type == TypesEnumeration.Record
                       ? true
                       : false;
        } }

        public static TypeInfo GenerateVoidInfo()
        {
            return new TypeInfo("void", TypesEnumeration.Void);
        }
        public static TypeInfo GenerateIntInfo()
        {
            return new TypeInfo("int", TypesEnumeration.Integer);
        }
        public static TypeInfo GenerateStringInfo()
        {
            return new TypeInfo("string", TypesEnumeration.String);
        }
        public static TypeInfo GenerateNilInfo()
        {
            return new TypeInfo("nil", TypesEnumeration.Nil);
        }
    }

    public class RecordInfo : TypeInfo
    {
        public RecordInfo(string name)
            : base(name, TypesEnumeration.Record)
        {
        }

        #region PROPERTIES:

        public List<ParameterInfo> Parameters { get; set; }
        public int FieldsCount { get { return this.Parameters.Count; } }
        #endregion
    }

    public class ArrayInfo : TypeInfo
    {
        public ArrayInfo(string name, string itemsType)
            : base(name, TypesEnumeration.Array)
        {
            this.ItemsType = itemsType;
        }

        #region PROPERTIES:

        public string ItemsType { get; set; }

        #endregion
    }

    public class ParameterInfo
    {
        public ParameterInfo(string name, string type)
        {
            this.Identifier = name;
            this.Type = type;
        }

        public string Identifier { get; set; }
        public string Type { get; set; }
    }

}
