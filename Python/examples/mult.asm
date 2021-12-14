;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    mult.asm
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.11.22 (kjc)
;
; DESCRIPTION:
;
;     Multiply the contents of registers C and D
;
;     See Intel 8080 Assembly Prograamming Manual, pages 53-56.
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;
; Constants ;
;;;;;;;;;;;;;

; ASCII characters

CR:	EQU	0Dh	; ASCII CR  (Carriage Return, a.k.a. Ctrl-M)
LF:	EQU	0Ah	; ASCII LF  (Line Feed        a.k.a. Ctrl-J)
ESC:	EQU	1Bh	; ASCII ESC (Escape,          a.k.a. Ctrl-[)
NUL:	EQU	00h	; ASCII NUL (Null)

; I/O

SIO1S:	EQU	10h	; Serial I/O communications port 1 STATUS
SIO1D:	EQU	11h	; Serial I/O communications port 1 DATA

MRST:	EQU	03h	; UART Master Reset
RCVD:	EQU	01h	; Character received
SENT:	EQU	002h	; Data sent. Output complete

; Code segment

	ORG	0000h	; Load at memory location 000 (hex)

	MVI	A,MRST
	OUT	SIO1S	; Reset the UART
	MVI	A,15h	; Settings: No RI, No XI, RTS Low, 8N1, /16
	OUT	SIO1S	; Configure the UART with above settings

	LXI	B,WORDS	; Point to WORDS
	LXI	H,WATCH	; Set location to increment
	MVI	M,00h	; Reset WATCH to 0

FETCH:	IN	SIO1S	; Check serial I/O status bit 1 (XMIT status)
	ANI	SENT	; If data not sent (i.e. XMIT not finished)...
	JZ	FETCH	; ...spin wheels: continue checking status. Else...
	LDAX	B	; ...Fetch byte
	CPI	NUL	; ...If byte is ASCII NUL...
	JZ	COUNT	; ......start counting. Else...
	OUT	SIO1D	; ......output byte
	INX	B	; ......point to next byte
	JMP	FETCH	; ......lather, rinse, repeat: Fetch next byte.

COUNT:	IN	SIO1S	; Check serial I/O status bit
	ANI	RCVD	; If no data received...
	JZ	COUNT	; ...spin wheels: continue checking status. Else...
	IN	SIO1D	; ...read the character
	OUT	SIO1D	; ...echo it
	INR	M	; ...increment the watched counter
	JMP	COUNT	; ...wait for next character
	
; Data segment

	ORG	200h	; Load at memory locaton 200 (hex)

WATCH:	DB	000h	; Initialize to zero

WORDS:	DB	ESC,"[2J"
	DB	CR,LF
	DB	CR,LF,"            "
	DB	"                ",ESC,"[31m","SLOW COUNTER",ESC,"[0m",CR,LF
	DB	CR,LF,"            "
	DB	"Press a printable key slowly several times,"
	DB	CR,LF,"            "
	DB	"while keeping track of the number of presses.",CR,LF
	DB	CR,LF,"            "
	DB	"Then, toggle the ",ESC,"[31m","RUN/STOP",ESC,"[0m"
	DB	" switch to ",ESC,"[31m","STOP",ESC,"[0m",".",CR,LF
	DB	CR,LF,"            "
	DB	"Toggle the address switches to 1000 octal"
	DB	CR,LF,"            "
	DB	"(200 hex), then toggle the "
	DB	ESC,"[31m","EXAMINE",ESC,"[0m"," switch.",CR,LF
	DB	CR,LF,"            "
	DB	"D0-D7 will equal to the number of key presses."
	DB	CR,LF
	DB	CR,LF
	DB	CR,LF
	DB	NUL	; NULL string terminator

; ANSI escape sequences


	END		; End

