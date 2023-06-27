grammar TankLite;

program: line*;

line: (fncall) ';';

fncall: IDENT '(' ')';

IDENT: [a-zA-Z_][a-zA-Z0-9_]*;