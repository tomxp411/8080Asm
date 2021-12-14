;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    divi.asm
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.11.22 (kjc)
;
; DESCRIPTION:
;
;     Divide the contents of register pair BC by D.
;     C will contain the quotient; B will contain the remainder.
;
;     See Intel 8080 Assembly Prograamming Manual, pages 53-56.
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

	ORG	000h

	MVI	B,00h
	MVI	C,63h	; dividend (BC) = 99 decimal
	MVI	D,00h
	MVI	E,09h	; divisor  (DE) =  9 decimal

DIVI:	MOV	A,D	; Negate the divisor
	CMA		;    "    "     "
	MOV	D,A	;    "    "     "
	MOV	A,E	;    "    "     "
	CMA		;    "    "     "
	MOV	E,A	;    "    "     "
	INX	D	; For two's complement
	LXI	H,00h	; Initial value for remainder
	MVI	A,11h	; Initialize loop counter
DV0:	PUSH	H	; Save remainder
	DAD	D	; Subtract divisor (add negative)
	JNC	DV1	; Underflow. Restore register pair HL
	XTHL
DV1:	POP	H
	PUSH	PSW	; Save loop counter
	MOV	A,C
	RAL		; 4 register left shift?
	MOV	C,A	; CY->C->B->L->H?
	MOV	A,B
	RAL
	MOV	B,A
	MOV	A,L
	RAL
	MOV	L,A
	MOV	A,H
	RAL
	MOV	H,A
	POP	PSW
	DCR	A
	JNZ	DV0

; Post-divide clean-up:
; Shift remainder right and return in DE

	ORA	A
	MOV	A,H
	RAR
	MOV	D,A
	MOV	A,L
	RAR
	MOV	E,A
	HLT

	END		; End

