;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    subtract.asm
; AUTHOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.11.13 (kjc)
;
; DESCRIPTION:
;
;     A demonstration of subtracting and conditional branching. And
;     an opportunity to investigate 2's-complement arithmetic.
;
;     Completely arbitrary values that should be easy to see in LEDs
;     or memory dumps:
;
;        AA (10101010) indicates the sign of the result is unknown
;        00 (00000000) indicates the sign of the result is 0 (no sign)
;        F0 (11110000) indicates the sign of the result is - (negative)
;        0F (00001111) indicates the sign of the result is + (positive)
;
;     The HLT (halt instruction) should never be reached. If it is,
;     something has gone terribly wrong.
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Code segment:

	ORG	000h	; Set Program Counter to address 0
	LXI	H,SIGN	; Point to sign indicator
	MVI	M,0AAh	; Set sign indicator to 10101010 (unknown)
LOOP:	LDA	LESS	; Load value at LESS into Accumulator
	MOV	B,A	; Move value in Accumulator to Register B
	LDA	TOTAL	; Load value at TOTAL into Accumulator
	SUB	B	; Accumulator (TOTAL) minus Reg. B (LESS)
	STA	DIFF	; Store accumulator at DIFF (DIFF = TOTAL - LESS)
	JZ	ZERO	; If the difference equals       zero (no sign)...
	JM	MINUS	; If the difference is less than zero (negative)...
	JP	PLUS	; If the difference is more than zero (positive)...
	HLT		; WARNING! WARNING! DANGER, WILL ROBINSON!

ZERO:	MVI	M,000h	; Set sign indicator to 00000000
	JMP	LOOP	; Jump to start of code (infinite loop)

MINUS:	MVI	M,0F0h	; Set sign indicator to 11110000
	JMP	LOOP	; Jump to start of code (infinite loop)

PLUS:	MVI	M,00Fh	; Set sign indicator to 00001111
	JMP	LOOP	; Jump to start of code (infinite loop)

; Data segment:

	ORG	100h	; Set Program Counter to address 100 (hex)

SIGN:	DB	0AAh	; Sign of the result (address 100) = unknown (AA hex)
TOTAL:	DB	005h	; Data Byte at address 101 = 5
LESS:	DB	010h	; Data Byte at address 102 = 16 (10 octal)
DIFF:	DB	000h	; Data Byte at address 103 = 0

	END		; End
