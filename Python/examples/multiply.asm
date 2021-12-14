;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    multiply.asm
; AUTHOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.10.06 (kjc)
;
; DESCRIPTION:
;
;     This is the assembly source code that produces the same machine
;     op codes as shown by the second example (multiply two numbers) in
;     the Altair 8800 Operator's Manual.
;
;     On a modern (as of 2020) Linux machine, install the
;     Macroassembler AS found at:
;
;         http://john.ccac.rwth-aachen.de:8000/as/
;
;     Then assemble this source file with the command:
;
;         asl -a -cpu 8080 -L -listradix 8 multiply.asm
;
;     to compare the generated list file (multiply.lst) to the octal
;     machine op codes provided in the manual.
;
;     However, if you have installed "most" -- you know you want it --
;     it is VERY instructive to instead assemble the source file with
;     the command:
;
;         asl -a -cpu 8080 -L -listradix 16 multiply.asm
;
;     This produces the same binary file (multiply.p) but changes the
;     format of the list file, displaying the machine op codes in
;     hexadecimal, rather than octal, and, together with the command:
;
;         most multiply.p
;
;      you can study the actual binary "executable" produced, and
;      "debug" it by finding the machine op codes (in hex) embedded
;      in the executable (by comparing it to multiply.lst).
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Code segment:

	ORG	0000h	; Set Program Counter to address 0
START:	MVI	A,0FFh	; Multiplier    (-1) to A   Register
	MVI	D,07Fh	; Multiplicand (127) to D,E Registers
	MVI	E,000h
	LXI	H,000h	; Clear H,L Registers to initialize Partial Product
	MVI	B,008h	; Iteration Count (8) to B Register
LOOP:	DAD	H	; Shift Partial Product left into Carry (H&L)
	RAL		; Rotate Multiplier Bit to Carry
	JNC	NEXT	; Test Multiplier at Carry
	DAD	D	; Add Multiplicand to Partial Product (D&E)
			;   if Carry =1
	ACI	000h	; (Add Carry Bit)
NEXT:	DCR	B	; Decrement Iteration Counter
	JNZ	LOOP	; Check Iterations
	SHLD	TOTAL	; Store Answer in Locations 100,101
;	JMP	START	; Restart
	HLT		; Halt and Catch Fire (HCF)

; Data segment (210 = 0xD2):

	ORG	040h	; Set Program Counter to address 100 (octal)
TOTAL:	DS	002h	; Reserve 2 bytes (1 word) of unintialized storage

	END		; End
