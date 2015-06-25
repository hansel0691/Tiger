using Tiger.AST_Nodes.Declarations;
using Tiger.AST_Nodes.Declarations.DeclarationBlocks;
using Tiger.AST_Nodes.FlowControl;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger.AST_Nodes.Operators.Relational;
using Tiger.Tiger_Error;

namespace Tiger.AST_Nodes.AST_Utils
{
    internal class SemanticError : TigerError
    {
        #region CONSTRUCTOR
        
        public SemanticError(LanguageNode node)
        {
            this.Line = node.Line;
            this.Column = node.CharPositionInLine;
        }
        
        #endregion
        
        #region METHODS:

        internal static SemanticError InvalidNumber(string literal, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("'{0}' is not a valid number.", literal),
            };
        }

        internal static SemanticError UndefinedVariableUsed(string name, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Undefined variable '{0}' at line {1}.", name, node.Line),
            };
        }

        public static SemanticError FunctionUsedAsVariable(string name, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Function '{0}' used as variable or constant.", name),
            };
        }

        public static SemanticError FunctionDoesNotExist(string name, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Function '{0}' does not exist", name),
            };
        }

        public static SemanticError VariableOrConstantUsedAsFunction(string name, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Variable or constant '{0}' is being used as a function", name),
            };
        }

        public static SemanticError TypeNotDefined(string name, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Type '{0}' does not exist.", name),
            }; 
        }

        public static SemanticError WrongFieldInit(string recordName,string fieldName, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("{0}' is missing in the init of '{1}'.", fieldName, recordName),
            }; 
        }

        internal static SemanticError WrongType(string actualType, string formalType, LanguageNode node)
        {

            return new SemanticError(node)
                       {
                           Message =
                               string.Format("Cannot implicitly convert type '{0}' to '{1}'", formalType, actualType),
                       };
        }

        internal static SemanticError WrongParameterNumber(string type,string name, int formalCount, int actualCount, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("{0} '{1}' takes {2} arguments, got {3} instead", type, name, formalCount, actualCount),
            };
        }

        internal static SemanticError DontReturnExpression(string statement, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("{0} used with an expression with return value at line {1}.", statement, node.Line),
            };
        }

        
        internal static SemanticError WrongBreak(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("break used out of a while or for statement at line {0}.", node.Line),
            };
        }

        public static SemanticError WrongAliasDeclaration(string definedType, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The type {0} is an invalid alias .", definedType),
            };
        }

        public static SemanticError StandardFunctionDeclaration(string text, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Routin {0} is defined as a standard function.", text),
            };
        }

        public static SemanticError InvalidFieldAccess(string identifier, string fieldIdentifier, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("{0} does not contain a definition for '{1}'.", identifier, fieldIdentifier),
            };
        }

        public static SemanticError DefinedField(string field, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The field '{0}' is already declared in the definition.", field),
            };
        }

        public static SemanticError InvalidNilAssignation(string type, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The type '{0}' does not accept an assignation of a nil value.", type),
            };
        }

        public static SemanticError ProcedureDontReturn(string functionId, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Routin '{0}' is defined as a procedure and return a value.", functionId),
            };
        }

        public static SemanticError DefinedFunction(string functionId, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Routin '{0}' is already defined as a routin or a variable.", functionId),
            };
        }

        public static SemanticError DefinedVariable(string varId, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Variable '{0}' is already defined as a routin or a variable.", varId),
            };
        }

        public static SemanticError DefinedType(string typeId, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("'{0}' is already defined.", typeId),
            };
        }

        public static SemanticError TypeNotVisible(string name, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The type '{0}' is not visible at this point.", name),
            };
        }

        public static SemanticError InvalidWileCondition(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Invalid type of condition of the while statement at line {0}", node.Line),
            };
        }

        public static SemanticError InvalidArrayType(string text, string identifier, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Undefined type '{0}' in the array '{1}' declaration at line {2}.", text, identifier, node.Line),
            };
        }

        public static SemanticError InvalidIfCondition(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The condition of the if-then statement at line {0} does not return an int value.", node.Line),
            };
        }

        public static SemanticError InvalidIfThen(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The then expression of the if-then statement at line {0} should not return a value.", node.Line),
            };
        }

        public static SemanticError InvalidIfReturn(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The return type of the expressions of the if-then-else statement at line {0} is not the same.", node.Line),
            };
        }

        public static SemanticError UndefinedVariableType(string text, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The actual variable type of {0} cann't be determined.", text),
            };
        }

        public static SemanticError InvalidOperandAtLogical(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Comparison operator {0} receive both operands int or both string.", node),
            };
        }

        public static SemanticError NonValuedAssignation(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Initialization expression doesn't return a value at line {0}.", node.Line),
            };
        }

        public static SemanticError InvalidUseOfOperator(LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Invalid use of '{0}' operator with a non-valued expression at line {1}.", node, node.Line),
            };
        }

        public static SemanticError InvalidForExpression(string expr, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("The expression for the {0} bound of the for loop at line {1} does not return an int value.", expr, node.Line),
            };
        }

        public static SemanticError ReadOnlyAssing(string varName, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("Invalid use of assignment to the readonly variable {0} at line {1}.", varName, node.Line),
            };
        }

        public static SemanticError HidingAnStandardFunc(string declaration, string identifier, LanguageNode node)
        {
            return new SemanticError(node)
            {
                Message = string.Format("{0} declaration of {1} at line {2} is hiding a standard function.", declaration, identifier, node.Line),
            };
        }
        #endregion
    }
}
