grammar tiger;

options
{
    language=CSharp3;
    output=AST;
    //language=Java;
}

tokens
{
    //Imaginary tokens
    ALIAS_DEC;
    ARRAY_DEC;
    ARRAY_FF_INDEX;
    ARRAY_INDEX;
    ARRAY_INIT;  
    ARGUMENT;
    DEC_LIST;
    EXPR_SEQ;
    FIELD_ACCESS;
    FIELD_ASSIGN;
    FIELD_DEC;
    FIELDS_DEC;
    FUNCTION_CALL;
    FUNCTION_DEC;
    ID_ACCESS;
    PROC_DEC;
    PROGRAM;			
    RECORD_DEC;
    RECORD_INIT;
    UNARY_MINUS;
    VAR_DEC;
            
    //operator tokens
    COMMA = ',';
    COLON = ':';
    SEMI = ';';
    OP = '(';
    CP = ')';
    OB = '[';
    CB = ']';
    OBRACE  = '{';
    CBRACE  = '}';
    DOT = '.';
    PLUS = '+';
    MINUS = '-';
    MULT = '*';
    DIV ='/';
    EQUAL ='=';
    DIF ='<>';
    LT ='<';
    LTE ='<=';
    GT = '>';
    GTE = '>=';
    AND = '&';
    OR = '|';
    ASSIGN = ':=';
    
    DQUOTE = '\"';
    OC = '/*';
    CC = '*/';
    
    //reserved words tokens
    ARRAY = 'array';
    BREAK = 'break';
    DO = 'do';
    ELSE = 'else';
    END = 'end';
    FOR = 'for';
    FUNCTION = 'function';
    IF = 'if';
    IN = 'in';
    LET = 'let';
    NIL = 'nil';
    OF = 'of';
    THEN = 'then';
    TO = 'to';
    TYPE = 'type';
    VAR = 'var';
    WHILE = 'while';
}

fragment
DEC_DIGIT 
    :   ('0'..'9')
    ;

fragment
LETTER
    :   ('a'..'z'|'A'..'Z')
    ;

fragment
ESC_SEQ
    :   '\\'
    (
     't'  {$text = "\t";}
    |'n'  {$text = "\n";}
    |'r'  {$text = "\r";}
    |'\"' {$text = "\"";}
    |'\\' {$text = "\\";}
    | UNICODE_ESC
    | WS+ '\\' {$text = "";}
    )
    ;


fragment
UNICODE_ESC
    : 
    '0' d1 =DEC_DIGIT d2 =DEC_DIGIT { $text = ((char)int.Parse(string.Format("0{0}{1}",$d1.text, $d2.text))).ToString(); }
    | '1' 
    (
    d3=('0'|'1') d4=DEC_DIGIT { $text = ((char)int.Parse(string.Format("1{0}{1}",(char)d3,$d4.text))).ToString(); }
    | '2' d5='1'..'7' { $text = ((char)int.Parse(string.Format("12{0}",(char)d5))).ToString(); }
    )
    ;



ID  :   LETTER (LETTER|DEC_DIGIT|'_')*
    ;


WS  
    :   ( ' '
        | '\t'
        | '\r'
        | '\n'
        )+   {$channel=Hidden;} 
    ;

COMMENT 
    : OC ( options {greedy=false;} : COMMENT | ~'/'| '/' ~('*'|'/') )* CC { $channel = Hidden; } 
    ;

INT 
    :   DEC_DIGIT+
    ;

STRING
@init
{
	var inString = "";
}
    :  DQUOTE 
    ( ESC_SEQ {inString += $text;} 
    |' ' {inString += " ";} 
    |'!' {inString += "!";}
    | val = '#'..'[' {inString += ((char)val).ToString();}
    | val = ']'..'~' {inString += ((char)val).ToString();})* DQUOTE { $text = inString; }
    ;


public program 
    :   expr EOF -> ^(PROGRAM expr)
    ;

expr
    :   return_expr
    |   non_return_expr
    ;

return_expr
    :   binary_expression
    ;
     
//el llamado a procedimientos no retorna valor
non_return_expr   
    :   WHILE cond=return_expr DO loop=expr -> ^(WHILE $cond $loop) 
    |   FOR ID ASSIGN init=return_expr TO end=return_expr DO loop=expr -> ^(FOR  ID $init $end $loop) 
    |   BREAK
    ;     

binary_expression
    :   and_expression (OR^ and_expression)*
        ;

and_expression
    :   comparison_expression (AND^ comparison_expression)*
    ;

comparison_expression
    :   addition_expression ((EQUAL^|DIF^|LT^ |LTE^ |GT^ |GTE^) addition_expression)?
    ;

addition_expression
    :   term ((PLUS^|MINUS^) term)*
    ;

term
    :   factor ((MULT^|DIV^) factor)*
    ;

factor
    :   INT 
    |   MINUS factor -> ^(UNARY_MINUS factor)
    | 	IF cond = return_expr THEN then=expr (ELSE else_do=expr)? -> ^(IF $cond $then $else_do?)
    |   LET declaration_list IN expr_seq?  END -> ^(LET declaration_list ^(EXPR_SEQ expr_seq? ))
    |   STRING
    |   NIL
    |   id_expr
    |   OP expr_seq? CP -> ^(EXPR_SEQ expr_seq?) //beacause the empty parenthesis are allowed
    ;

indexer_expr
    :   DOT ID indexer_expr? -> ^(DOT ID indexer_expr?)
    |   OB return_expr CB indexer_expr? -> ^(ARRAY_FF_INDEX return_expr indexer_expr?)
    ;

id_expr
    :   id=ID 
    (
        //record istanciation
        OBRACE field_list? CBRACE  -> ^(RECORD_INIT $id field_list?)
                        //array instanciation   
        | OB length_index=return_expr CB
        		(OF value=return_expr -> ^(ARRAY_INIT $id $length_index $value)
                        | indexer_expr?
                            //for when i wanna assign a value to an item of the array
                            ((ASSIGN value=return_expr) -> ^(ASSIGN ^(ARRAY_INDEX $id ^(ARRAY_FF_INDEX $length_index indexer_expr?))  $value)
                            //for get a value from the array
                             |-> ^(ARRAY_INDEX $id ^(ARRAY_FF_INDEX $length_index indexer_expr?)))
                         ) 
        
        | DOT access=ID indexer_expr? 
        //for when i wanna asign a value to a field of an object
         (ASSIGN asignation_value=return_expr -> ^(ASSIGN ^(FIELD_ACCESS $id ^(DOT $access indexer_expr?)) $asignation_value)
          //acces to the fields of a record
          |-> ^(FIELD_ACCESS $id ^(DOT $access indexer_expr?))
         )
        //function calls
        | OP expr_list? CP ->^(FUNCTION_CALL $id  expr_list?)
        //asignation of varoables
        | ASSIGN rvalue=return_expr -> ^(ASSIGN $id $rvalue)
        | -> $id
    )
    ;

expr_seq
    :   expr ( SEMI! expr)* 
    ;
    
expr_list
    :   return_expr (COMMA return_expr)* -> ^(ARGUMENT return_expr+)
    ;
     
field_list
    :   ID EQUAL return_expr ( COMMA ID EQUAL return_expr )* ->  ^(FIELD_ASSIGN ID return_expr)+
    ;

declaration_list
    :   declaration+ -> ^(DEC_LIST declaration+)
    ;

declaration
    :   type_declaration
    |   variable_declaration
    |   function_declaration
    ;

type_declaration
    :   TYPE type_id=ID EQUAL 
    (
            id=ID -> ^(ALIAS_DEC $type_id $id)
            |OBRACE type_fields? CBRACE -> ^(RECORD_DEC $type_id type_fields?)
            |ARRAY OF type=ID -> ^(ARRAY_DEC $type_id $type)
    )
    ;

type_fields
    :   type_field (COMMA type_field)* -> ^(FIELDS_DEC type_field+)
    ;
    
type_field
    :   id=ID COLON type_id=ID -> ^(FIELD_DEC $id $type_id)
    ;
    
variable_declaration
    :   VAR id=ID (COLON type_id=ID)? ASSIGN return_expr -> ^(VAR_DEC $id $type_id? return_expr)
    ;

function_declaration
    :   FUNCTION id=ID OP args=type_fields? CP 
    (
        EQUAL expr             -> ^(PROC_DEC $id $args? expr)
            |COLON type_id = ID EQUAL return_expr -> ^(FUNCTION_DEC $id $args? $type_id return_expr)
    )
    ;
    