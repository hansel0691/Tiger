using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Antlr.Runtime;
using Tiger.AST_Nodes.AST_Utils;
using Tiger._03_Semantics;

namespace Tiger.AST_Nodes.Declarations.DeclarationBlocks
{
    /// <summary>
    /// Stands for a list of declarations (Arrays, alias and records).
    /// declaration+ -> ^(DEC_LIST declaration+)
    /// </summary>
    internal class DeclarationBlock : LanguageNode, IEnumerable<DeclarationNode>
    {
        private RecordDeclarationNode _recordDeclaration = null;
        #region CONSTRUCTORS:

        public DeclarationBlock(IToken token)
            : base(token)
        {
        }

        #endregion
        #region PROPERTIES:

        public DeclarationNode this[int i]
        {
            get
            {
                if (i < 0 || i >= Children.Count)
                    throw new ArgumentException(string.Format("Index out of range tring indexing declarations list."));
                return (DeclarationNode)Children[i];
            }
        }

        public int Count
        {
            get
            {
                if (Children == null)
                    return 0;
                return Children.Count;
            }
        }

        #endregion
        #region METHODS:

        public override void CheckSemantics(Scope scope, List<SemanticError> errors)
        {
            //defin ethe block of declarations
            var lastDeclaration = TypesEnumeration.Void;
            var unChecked = new List<DeclarationNode>();
            var cascadeCheck = new Dictionary<string,bool>();
            foreach (var declaration in this)
            {
                //chain declarations
                if (((declaration is FunctionDeclarationNode || declaration is ProcedureDeclarationNode) &&
                     lastDeclaration != TypesEnumeration.Function)
                    || declaration is VariableDeclarationNode 
                    || ((declaration is  ArrayDeclarationNode || declaration is RecordDeclarationNode || declaration is AliasDeclarationNode) && lastDeclaration != TypesEnumeration.Alias))
                {
                    foreach (var dec in unChecked.Where(dec => !cascadeCheck[dec.Identifier.Text]))
                        if (AddDeclaration(dec, unChecked, cascadeCheck, scope, errors))
                            AddFields(_recordDeclaration, unChecked, cascadeCheck, scope, errors);
                    if (lastDeclaration == TypesEnumeration.Function) unChecked.ForEach(x => x.CheckSemantics(scope, errors));
                    unChecked = new List<DeclarationNode>();
                    cascadeCheck = new Dictionary<string, bool>();
                }
                lastDeclaration = declaration is FunctionDeclarationNode || declaration is ProcedureDeclarationNode
                                          ? TypesEnumeration.Function
                                          : declaration is VariableDeclarationNode
                                                ? TypesEnumeration.Variable
                                                : TypesEnumeration.Alias;
                
                if (cascadeCheck.Keys.Contains(declaration.Identifier.Text))
                    errors.Add(SemanticError.DefinedType(declaration.Identifier.Text,this));
                else
                {
                    unChecked.Add(declaration);
                    cascadeCheck.Add(declaration.Identifier.Text, false);
                }
            }
            foreach (var dec in unChecked.Where(dec => !cascadeCheck[dec.Identifier.Text]))
                if (AddDeclaration(dec, unChecked, cascadeCheck, scope, errors))
                    AddFields(_recordDeclaration, unChecked, cascadeCheck, scope, errors);
            if (lastDeclaration == TypesEnumeration.Function) unChecked.ForEach(x => x.CheckSemantics(scope, errors));
        }

       

        /// <summary>
        /// Add a declaration of type, variable or routin to the scope.
        /// </summary>
        /// <param name="declaration">declaration to add.</param>
        /// <param name="unChecked">declaration block where the scope of the declaration begin.</param>
        /// <param name="booleanUnChecked">declarations name mapped to if this declration is checked or not.</param>
        /// <param name="scope"></param>
        /// <param name="errors"></param>
        /// <returns>returns true if the cicle of declaration check a record declaration in it's way.</returns>
        public bool AddDeclaration(DeclarationNode declaration, List<DeclarationNode> unChecked, Dictionary<string, bool> booleanUnChecked ,Scope scope, List<SemanticError> errors)
        {
            if (declaration is ArrayDeclarationNode)
                return AddArrayToScope((ArrayDeclarationNode)declaration, unChecked, booleanUnChecked, scope, errors);
            if (declaration is RecordDeclarationNode)
                return AddRecordToScope((RecordDeclarationNode)declaration, unChecked, booleanUnChecked, scope, errors);
            if (declaration is AliasDeclarationNode)
                return AddAliasToScope((AliasDeclarationNode)declaration, unChecked, booleanUnChecked, scope, errors);
            
            if (declaration is VariableDeclarationNode)
                AddVarToScope((VariableDeclarationNode)declaration, unChecked, booleanUnChecked, scope, errors);
            else
                AddRoutin((RoutineDeclarationNode)declaration, unChecked, booleanUnChecked, scope, errors);
            return false;
        }
        
        public bool AddArrayToScope(ArrayDeclarationNode declaration, List<DeclarationNode> decBlock, Dictionary<string, bool> unChecked, Scope scope, List<SemanticError> errors)
        {
            var identifier = declaration.Identifier.Text;
            unChecked[identifier] = true;
            var result = false;

            //check if there is a declaration with th same type name.
            if (scope.ContainsType(identifier, true))
                errors.Add(SemanticError.DefinedType(identifier, this));

            //type type of the items in the array.
            if (!scope.ContainsType(declaration.DefinedType.Text, true))
                if (!decBlock.Exists(x => !unChecked[x.Identifier.Text] && x.Identifier.Text == declaration.DefinedType.Text))
                {
                    if (!scope.Parent.ContainsType(declaration.DefinedType.Text))
                    {
                        errors.Add(SemanticError.InvalidArrayType(declaration.DefinedType.Text, identifier, this));
                        return false;
                    }
                }
                else
                    result = AddDeclaration(decBlock.FirstOrDefault(x => x.Identifier.Text == declaration.DefinedType.Text), decBlock, unChecked, scope, errors);

            if (scope.GetType(declaration.DefinedType.Text) != null)
            {
                scope.AddType(identifier, new ArrayInfo(identifier, scope.GetType(declaration.DefinedType.Text).Name));
                declaration.CheckSemantics(scope, errors);
            }
            return result;
        }
        
        public bool AddRecordToScope(RecordDeclarationNode declaration, List<DeclarationNode> decBlock, Dictionary<string, bool> unChecked, Scope scope, List<SemanticError> errors)
        {
            var identifier = declaration.Identifier.Text;
            unChecked[identifier] = true;

            if (scope.ContainsType(identifier, true))
                errors.Add(SemanticError.DefinedType(identifier, this));
            
            scope.AddType(identifier, new RecordInfo(identifier));
            _recordDeclaration = declaration;
            return true;
        }

        private void AddFields(RecordDeclarationNode declaration, List<DeclarationNode> decBlock, Dictionary<string, bool> unChecked, Scope scope, List<SemanticError> errors)
        {
            var paramInfo = new List<ParameterInfo>();
            foreach (var fieldDeclaration in declaration.FieldDeclarationList)
            {
                if (!scope.ContainsType(fieldDeclaration.TypeName.Text, true))
                    if (!decBlock.Exists(x => !unChecked[x.Identifier.Text] && x.Identifier.Text == fieldDeclaration.TypeName.Text))
                    {
                        if (!scope.Parent.ContainsType(fieldDeclaration.TypeName.Text))
                        {
                            errors.Add(SemanticError.TypeNotDefined(fieldDeclaration.TypeName.Text, this));
                            continue;
                        }
                    }
                    else
                        AddDeclaration(decBlock.First(x => x.Identifier.Text == fieldDeclaration.TypeName.Text), decBlock, unChecked, scope, errors);
                fieldDeclaration.CheckSemantics(scope, errors);
                paramInfo.Add(new ParameterInfo(fieldDeclaration.Field.Text, scope.GetType(fieldDeclaration.TypeName.Text).Name));
            }
            ((RecordInfo)scope.Types[declaration.Identifier.Text]).Parameters = paramInfo;
            declaration.CheckSemantics(scope, errors);
        }

        public bool AddAliasToScope(AliasDeclarationNode declaration, List<DeclarationNode> decBlock, Dictionary<string, bool> unChecked, Scope scope, List<SemanticError> errors)
        {
            var identifier = declaration.Identifier.Text;
            unChecked[identifier] = true;
            var result = false;

            if (scope.ContainsType(identifier, true))
                errors.Add(SemanticError.DefinedType(identifier, this));

            //check if the alias exist or is declareed in the block.
            if (!scope.ContainsType(declaration.Alias.Text, true))
                if (!decBlock.Exists(x => !unChecked[x.Identifier.Text] && x.Identifier.Text == declaration.Alias.Text))
                {
                    if (!scope.Parent.ContainsType(declaration.Alias.Text))
                    {
                        errors.Add(SemanticError.TypeNotDefined(declaration.Alias.Text, this));
                        return false;
                    }
                }
                else 
                    result = AddDeclaration(decBlock.First(x => x.Identifier.Text == declaration.Alias.Text), decBlock, unChecked, scope, errors);

            scope.AddType(identifier, scope.GetType(declaration.Alias.Text));
            declaration.CheckSemantics(scope, errors);
            return result;
        }
        
        public void AddVarToScope(VariableDeclarationNode declaration, List<DeclarationNode> decBlock, Dictionary<string, bool> unChecked, Scope scope, List<SemanticError> errors)
        {
            var identifier = declaration.Identifier.Text;
            declaration.CheckSemantics(scope, errors);
            scope.AddVar(identifier, new VariableInfo(identifier, scope.GetType(declaration.Identifier.ExpressionType.Name).Name));
        }

        private void AddRoutin(RoutineDeclarationNode declaration, List<DeclarationNode> decBlock, Dictionary<string, bool> unChecked, Scope scope, List<SemanticError> errors)
        {
            var returnType = declaration is FunctionDeclarationNode ? (((FunctionDeclarationNode)declaration)).ReturnType.Text : "void";
            
           
            var identifier = declaration.Identifier.Text;
            unChecked[identifier] = true;

            if (scope.ContainsRoutine(identifier, true) || scope.ContainsVarInstance(identifier, true))
                errors.Add(SemanticError.DefinedFunction(identifier, this));

            var arguments = new List<ParameterInfo>();
            if (declaration.Arguments != null)
                foreach (var argument in declaration.Arguments)
                    if (!scope.ContainsType(argument.TypeName.Text))
                        errors.Add(SemanticError.TypeNotDefined(argument.Field.Text, this));
                    else if (arguments.Exists(x => x.Identifier == argument.Field.Text))
                        errors.Add(SemanticError.DefinedField(argument.Field.Text,this));
                    else
                        arguments.Add(new ParameterInfo(argument.Field.Text, scope.GetType(argument.TypeName.Text).Name));

            if (scope.ContainsType(returnType) || returnType == "void")
                scope.AddRoutine(identifier, new RoutineInfo(identifier, returnType != "void" ? scope.GetType(returnType).Name : "void", arguments));
        }

        public override void Generate(ILGenerator generator, Symbols symbols)
        {
            //check the thisng with the declaration block?????
            foreach (var dec in this)
                dec.Generate(generator, symbols);
        }

        #region Implementation of IEnumerable

        public IEnumerator<DeclarationNode> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion
   
    }
}
