;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    hello-8800.asm
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.10.06 (kjc)
;
; DESCRIPTION:
;
;     "Hello world!" suitable for the Macroassembler AS found at:
;
;         http://john.ccac.rwth-aachen.de:8000/as/
;
;     Assemble this source file with the command:
;
;         asl -a -cpu 8080 -L -listradix 8 hello-8800.asm
;
;     to generate a list file (hello-8800.lst) that displays the machine
;     op codes as octal values.
;
;     However, if you have installed "most" -- you know you want it --
;     it is VERY instructive to instead assemble the source file with
;     the command:
;
;         asl -a -cpu 8080 -L -listradix 16 hello-8800.asm
;
;     This produces the same binary file (hello-8800.p) but changes the
;     format of the list file, displaying the machine op codes in
;     hexadecimal, rather than octal, and, together with the command:
;
;         most hello-8800.p
;
;      you can study the actual binary "executable" produced, and
;      "debug" it by finding the machine op codes (in hex) embedded
;      in the executable (by comparing it to hello-8800.lst).
;
;      NOTE: The code this was built from originally used:
;
;          RRC
;          JNC     LOOP
;
;      in place of:
;
;          ANI     RCVD    ; If no data received... (bit is unset)
;          JZ      LOOP    ; ...spin wheels: continue checking status.
;
;      The original is faster and smaller: RRC requires only one
;      machine cycle, and a single byte, whereas ANI requires two
;      cycles and two bytes.
;
;      However, the first form is less intuitive: Instead of
;      checking the low-order bit of the accumulator and looping when
;      it is zero, the original code rotated the value in the
;      accumulator to the right, wrapping bit 0 into the high-order
;      bit, and setting the carry status flag to the value of the
;      pre-rotated low-order bit. It then jumps if no carry took
;      place, i.e. the carry flag bit remains unset (0).
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

SIO1S:	EQU	10h		; Serial I/O communications port 1 STATUS
SIO1D:	EQU	11h		; Serial I/O communications port 1 DATA

MRST:	EQU	03h		; UART Master Reset
RCVD:	EQU	01h		; Character received

; Code segment

	ORG	000h

	MVI	A,MRST
	OUT	SIO1S		; Reset the UART
	MVI	A,15h		; Settings: No RI, No XI, RTS Low, 8N1, /16
	OUT	SIO1S		; Configure the UART with above settings

LOOP:	IN	SIO1S		; Check serial I/O status bit
	ANI	RCVD		; If no data received...
	JZ	LOOP		; ...spin wheels: continue checking status. Else...
	IN	SIO1D		; ...read the character
	OUT	SIO1D		; ...echo it
	JMP	LOOP		; ...wait for next character
	
; Data segment

	ORG	100h

	END			; End
