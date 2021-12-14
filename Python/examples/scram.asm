;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    scram.asm
; AUTHOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2021.01.11 (kjc)
;
; DESCRIPTION:
;
;     Set to Clear all RAM (SCRAM)
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Code segment

	ORG	0000h	; Set the location counter to 0 (hex)

	LXI	H,AFTER	; Point to first byte after the end of the program
LOOP:	MVI	M,00h	; Move zero into memory location
	INX	H	; Point to next memory location
	JMP	LOOP	; You go back, Jack, and do it again, wheels turnin'...

AFTER:	EQU	$	; Define constant AFTER as current location counter

	END		; End

