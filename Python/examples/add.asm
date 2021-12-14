;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    add.asm
; AUTHOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.10.06 (kjc)
;
; DESCRIPTION:
;
;     This is the assembly source code that produces the same machine
;     op codes as shown by the first example (add two numbers) in the
;     Altair 8800 Operator's Manual.
;
;     On a modern (as of 2020) Linux machine, install the
;     Macroassembler AS found at:
;
;         http://john.ccac.rwth-aachen.de:8000/as/
;
;     Then assemble this source file with the command:
;
;         asl -a -cpu 8080 -L -listradix 8 add.asm
;
;     to compare the generated list file (add.lst) to the octal machine
;     op codes provided in the manual.
;
;     However, if you have installed "most" -- you know you want it --
;     it is VERY instructive to instead assemble the source file with
;     the command:
;
;         asl -a -cpu 8080 -L -listradix 16 add.asm
;
;     This produces the same binary file (add.p) but changes the format
;     of the list file, displaying the machine op codes in hexadecimal,
;     rather than octal, and, together with the command:
;
;         most add.p
;
;      you can study the actual binary "executable" produced, and
;      "debug" it by finding the machine op codes (in hex) embedded
;      in the executable (by comparing it to add.lst).
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Code segment:

	ORG	0o	; Set Program Counter to address 0
START:	LDA	VAL1	; Load value (5) at VAL1 (200) into Accumulator
	MOV	B,A	; Move value in Accumulator to Register B
	LDA	VAL2	; Load value (8) at VAL2 (201) into Accumulator
	ADD	B	; Add value in Register B to value in Accumulator
	STA	SUM	; Store accumulator at SUM (202)
	JMP	START	; Jump to start of code (infinite loop)

; Data segment:

	ORG	200o	; Set Program Counter to address 200
VAL1:	DB	5o	; Data Byte at address 200 = 5
VAL2:	DB	10o	; Data Byte at address 201 = 8 (10 octal)
SUM:	DB	0o	; Data Byte at address 202 = 0

	END		; End
