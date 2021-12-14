;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    scram2.asm
; AUTHOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2021.01.11 (kjc)
;
; DESCRIPTION:
;
;     Set to Clear all RAM (SCRAM) in a specified range. Despite my
;     love of assembler, I become less enamored by Intel 8080's "argh-
;     itecture". For example, subtracting one register pair from 
;     another requires 10 instructions. And decrementing a register
;     pair to zero does NOT set the zero flag... How delightfully
;     droll. It's these little unexpected gems that cost me hours.
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Code segment

	ORG	000o	; Set the location counter to 0 (octal)

	LHLD	FIRST	; Load register pair HL w/ address of first byte
	MOV	A,H	; Negate HL and store in DE (get high order byte)
	CMA		;   "    "   "    "   "  "  (complement high order)
	MOV	D,A	;   "    "   "    "   "  "  (store in D)
	MOV	A,L	;   "    "   "    "   "  "  (get low order byte)
	CMA		;   "    "   "    "   "  "  (complement low order)
	MOV	E,A	;   "    "   "    "   "  "  (store in E)
	INX	D	;   "    "   "    "   "  "  (make 2's complement)
	LHLD	LAST	; Load register pair HL w/ address of last byte
	DAD	D	; Daddy? ;-) byte count in HL = LAST - FIRST
	XCHG		; Store byte count in DE
	INX	D	; Actually, byte count is off by one. Fix it.
	LHLD	FIRST	; First location to fill
LOOP:	MVI	M,FILL	; Move fill pattern into memory location
	INX	H	; Point to next memory location
	DCX	D	; Decrement number of bytes remaining
	MOV	A,D	; Check if bytes remaining (register pair DE) is...
	ORA	E	; ...zero by logically OR'ing D with E
	JNZ	LOOP	; Repeat until bytes remaining = 0
	HLT		; Don't try to execute random crap following code end

; Data segment

	ORG	100o

FILL:	EQU	0AAh	; Fill pattern [Change to 000h to zero RAM.]
FIRST:	DW	0040h	; First memory location to be filled
LAST:	DW	005Fh	; Last  memory location to be filled

	END		; End
