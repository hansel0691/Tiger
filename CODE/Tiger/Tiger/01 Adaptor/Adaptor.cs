using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Tiger.AST_Nodes;
using Tiger.AST_Nodes.Declarations;
using Tiger.AST_Nodes.Declarations.DeclarationBlocks;
using Tiger.AST_Nodes.FlowControl;
using Tiger.AST_Nodes.Instance;
using Tiger.AST_Nodes.Instructions;
using Tiger.AST_Nodes.Instructions.AccessNodes;
using Tiger.AST_Nodes.Operators.Arithmetical;
using Tiger.AST_Nodes.Operators.Logical;
using Tiger.AST_Nodes.Operators.Relational;


namespace Tiger
{
    class Adaptor : CommonTreeAdaptor
    {
        public Adaptor() : base() { }

        public override object Create(Antlr.Runtime.IToken payload)
        {
            if (payload == null)
                return base.Create(payload);
            switch (payload.Type)
            {
                case tigerLexer.ALIAS_DEC:
                    return new AliasDeclarationNode(payload);
                case tigerLexer.ARRAY_DEC:
                    return new ArrayDeclarationNode(payload);
                case tigerLexer.ARRAY_FF_INDEX:
                    return new IndexNestedNode(payload);
                case tigerLexer.ARRAY_INDEX:
                    return new ArrayItemNode(payload); 
                case tigerLexer.ARRAY_INIT:
                    return new InstanceTypeArrayNode(payload);
                case tigerLexer.ARGUMENT:       //return_expr, return_expr, ...
                    return new ArgumentListNode(payload); 
                case tigerLexer.DEC_LIST:   //declaration declaration ...
                    return new DeclarationBlock(payload); 
                case tigerLexer.EXPR_SEQ:       //expr; expr ...
                    return new ExpressionSequence(payload);
                case tigerLexer.FIELD_ACCESS:
                    return new AccessFieldNode(payload);
                case tigerLexer.FIELD_ASSIGN:
                    return new FieldAssignNode(payload);
                case tigerLexer.FIELD_DEC:
                    return new FieldDeclarationNode(payload);      //falta revisar.
                case tigerLexer.FIELDS_DEC:
                    return new FieldDeclarationBlock(payload);
                case tigerLexer.FUNCTION_CALL:
                    return new CallRoutineNode(payload);    
                case tigerLexer.FUNCTION_DEC:
                    return new FunctionDeclarationNode(payload); 
                /*case tigerLexer.ID_ACCESS:
                    return new AccessVariableNode(payload);    */    
                case tigerLexer.PROC_DEC:
                    return new ProcedureDeclarationNode(payload); 
                case tigerLexer.PROGRAM:
                    return new ProgramNode(payload);            
                case tigerLexer.RECORD_DEC:
                    return new RecordDeclarationNode(payload);
                case tigerLexer.RECORD_INIT:
                    return new InstanceRecordNode(payload);
                case tigerLexer.UNARY_MINUS:
                    return new MinusUnaryNode(payload);
                case tigerLexer.VAR_DEC:
                    return new VariableDeclarationNode(payload);
                
                case tigerLexer.BREAK:
                    return new BreakNode(payload);
                case tigerLexer.FOR:
                    return new ForNode(payload);
                case tigerLexer.IF:
                    return new IfThenElseNode(payload);
                case tigerLexer.LET:
                    return new LetInEndNode(payload);
                case tigerLexer.NIL:
                    return new NilNode(payload);
                case tigerLexer.WHILE:
                    return new WhileNode(payload);
                case tigerLexer.DOT:
                    return new FieldNestedNode(payload);
                case tigerLexer.PLUS:
                    return new PlusNode(payload);
                case tigerLexer.MINUS:
                    return new MinusBinaryNode(payload);
                case tigerLexer.MULT:
                    return new MultNode(payload);
                case tigerLexer.DIV:
                    return new DivNode(payload);
                case tigerLexer.EQUAL:
                    return new EqualNode(payload);
                case tigerLexer.DIF:
                    return new NotEqualNode(payload);
                case tigerLexer.LT:
                    return new LowerThanNode(payload);
                case tigerLexer.LTE:
                    return new LowerEqualThanNode(payload);
                case tigerLexer.AND:
                    return new AndNode(payload);
                case tigerLexer.OR:
                    return new OrNode(payload);
                case tigerLexer.GT:
                    return new GreaterThanNode(payload);
                case tigerLexer.GTE:
                    return new GreaterEqualThanNode(payload);
                case tigerLexer.ASSIGN:
                    return new AssignNode(payload);

                case tigerLexer.ID:
                    return new IdNode(payload);
                case tigerLexer.STRING:
                    return new InstanceStringNode(payload);
                case tigerLexer.INT:
                    return new InstanceIntNode(payload);
                
                default:
                    //todo: lanzar un error definido por nosotros con igual texto
                    throw new Exception(string.Format("There has been an error in the Adaptor class.Unrecognizable token {0}.", payload.Type));

            }

        }
    }
}
