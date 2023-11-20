grammar TankLite;

program: line*;

line: (fncall | assignment | reassignment) ';';

fncall: deepIdent '(' expr? (',' expr)* ')';
assignment: KEYW_VAR IDENT '=' expr;
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

KEYW_VAR: 'var';

INT: '-'? [0-9]+;
FLOAT: '-'? [0-9]+ '.' [0-9]+;
STRING: '"' ~'"'* '"';
BOOL: 'true' | 'false';

IDENT: [a-zA-ZäöüÄÖÜ_][a-zA-Z0-9äöüÄÖÜ_]*;

WS: [ \t\r\n] -> skip;