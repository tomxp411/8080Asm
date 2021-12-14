	PAGE	40		; 40 lines per page
	TITLE	"Black Adder - Copyright (C) Kevin Cole 2020.11.04 (GPL)"
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    black-adder.asm
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.11.03 (kjc)
;
; DESCRIPTION:
;
;     Increment a memory location each time a key is pressed.
;
;     To take full advantage of the included ANSI escape sequences,
;     start minicom with the `-c on` option:
;
;         $ minicom -c on altair
;
;     or include it in the MINICOM environment variable:
;
;         $ export MINICOM="-m -c on"
;         $ minicom altair
;
;     NOTES: For help with ANSI escape sequences see:
;
;                https://en.wikipedia.org/wiki/ANSI_escape_code
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Code segment

	INCLUDE	stdlib	; Include standard library

	NEWPAGE	0

	ORG	000h	; Load at memory location 000 (hex)

	CALL	RSTIO	; Initialize serialize input / output device

	LXI	B,WORDS	; Point to instructions (WORDS)
	CALL	WRITE	; Write WORDS to stdout (terminal)

	LXI	B,ASK1	; Point to first prompt (ASK1)
	CALL	WRITE	; Write ASK1 to stdout (terminal)

	LXI	B,BUFFR	; Point to input buffer (BUFFR)
	CALL	READ	; Read a line from stdin to BUFFR

	LXI	B,ASK2	; Point to first prompt (ASK2)
	CALL	WRITE	; Write ASK2 to stdout (terminal)

	LXI	B,BUFFR	; Point to input buffer (BUFFR)
	CALL	READ	; Read a line from stdin to BUFFR

	HLT		; DEBUG

	LXI	H,WATCH	; Set location to increment

COUNT:	IN	SIO1S	; Check serial I/O status bit
	ANI	RCVD	; If no data received...
	JZ	COUNT	; ...spin wheels: continue checking status. Else...
	IN	SIO1D	; ...read the character
	OUT	SIO1D	; ...echo it
	INR	M	; ...increment the watched counter
	JMP	COUNT	; ...wait for next character
	
; Data segment

	ORG	2000h	; Load at memory locaton 8192 (decimal)

WATCH:	DB	000h	; Initialize to zero
VAL1:	DB	000h	; Data Byte at address 2000 (hex) = 0
VAL2:	DB	000h	; Data Byte at address 2001 (hex) = 0
SUM:	DB	000h	; Data Byte at address 2002 (hex) = 0

BUFFR:	DS	800h	; ~ one 80x25 screens-worth of bytes at 2003 (hex)

WORDS:	DB	ESC,"[2J"
	DB	CR,LF
	DB	CR,LF,"            "
	DB	"                ",ESC,"[31m","BLACK ADDER",ESC,"[0m",CR,LF
	DB	CR,LF,"            "
	DB	"This program adds two numbers in the range"
	DB	CR,LF,"            "
	DB	"-127 to +127. Enter values X and Y at the"
	DB	CR,LF,"            "
	DB	"prompts.",CR,LF
	DB	CR,LF
	DB	CR,LF
	DB	NUL	; NULL string terminator

ASK1:	DB	"X: ",NUL

ASK2:	DB	"Y: ",NUL

	END		; End

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

	ORG	0000h	; Set Program Counter to address 0
START:	LDA	VAL1	; Load value (5) at VAL1 (200) into Accumulator
	MOV	B,A	; Move value in Accumulator to Register B
	LDA	VAL2	; Load value (8) at VAL2 (201) into Accumulator
	ADD	B	; Add value in Register B to value in Accumulator
	STA	SUM	; Store accumulator at SUM (202)
	JMP	START	; Jump to start of code (infinite loop)
