grammar TankLite;

program: line*;

line: (fncall | assignment | reassignment) ';';

fncall: deepIdent '(' expr? (',' expr)* ')';
assignment: KEYW_LET IDENT '=' expr;
reassignment: deepIdent '=' expr;

expr
	: constant								#constantExpression
	| deepIdent								#identifierExpression
	| '(' expr ')'							#parenthesizedExpression
	| 'new' IDENT '(' expr? (',' expr)* ')'	#constructorExpression
	| expr multOp expr						#multiplicationExpression
	| expr addOp expr						#additiveExpression
	;

constant: BOOL | FLOAT | INT | STRING;

deepIdent: IDENT ('.' IDENT)*;

addOp: '+' | '-';
multOp: '*' | '/';

KEYW_LET: 'let';

INT: [0-9]+;
FLOAT: [0-9]+ '.' [0-9]+;
STRING: '"' ~'"'* '"';
BOOL: 'true' | 'false';

IDENT: [a-zA-Z_][a-zA-Z0-9_]*;

WS: [ \t\r\n] -> skip;