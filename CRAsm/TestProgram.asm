	ORG 0

	X =1
	Y = 1 + 1
	Z =1+X
	W=X+1
	V=X+Y+Z

START:	
	JMP $+4
	NOP
JD:	MVI A,$AC
	CALL PRTHEX
	HLT
	
PRTHEX:	
	PUSH B
	PUSH H
	MOV C,A		;back up A for later

	RRC		;move the top nibble into the bottom half so we can print it
	RRC
	RRC
	RRC
	CALL PDIGIT
	MOV A,C
	CALL PDIGIT
	MOV C,A
	POP H
	POP B
	RET
	
PDIGIT:	MVI B,$0F
	ANA B
	LXI H,DIGITS
	ADD L
	MOV L,A
	MOV A,M
	OUT $01
	RET

DIGITS:	DB '0123456789ABCDEF ' 

; Test data loads and space allocation.
DATA:	ORG 40h	
		DB 01, 0x02, Fh, F0h, ffh, 10q			;values should be 01h, 02h, 0fh, f0h, ffh, 08h (10 octal)
		ORG 50h									;should now be at address 50h
		DB "STRING TEST. ", '~', 0x7E			;Data at the end of the line should read 20 00 7E 7E (Space, null string termination, then two ~ symbols.)
		DB 'CHARACTER ARRAY', F0h				;should be CHARACTER ARRAY followed immediately by F0h 
		DW $F0E3
OUTPUT: DS 64
