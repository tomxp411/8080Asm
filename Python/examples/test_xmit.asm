	PAGE	40		; 40 lines per page
	TITLE	"test_xmit - Copyright (C) Kevin Cole 2021.11.03 (GPL)"
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    test_xmit.asm
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2021.11.03 (kjc)
;
; DESCRIPTION:
;
;     Sends a single character over the serial port for printing
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

	ORG	000h	; Load at memory location 1000 (hex)

; Reset serial input / output
;
	MVI	A,MRST
	OUT	SIO1S	; Reset the UART
	MVI	A,15h	; Settings: No RI, No XI, RTS Low, 8N1, /16
	OUT	SIO1S	; Configure the UART with above settings

; Put a character on to the serial I/O bus (stdout)
;
	LXI	B,CHAR	; Point to test character
	LDAX	B	; Fetch byte
	PUSH	PSW	; Preserve Program Status Word
WAITO:	IN	SIO1S	; ...Check serial I/O status bit 1 (XMIT status)
	ANI	SENT	; ...If data not sent (i.e. XMIT not finished)...
	JZ	WAITO	; ......spin wheels: continue checking status. Else...
	POP	PSW	; ......restore Program Status Word
	OUT	SIO1D	; ......output byte

HCF:	HLT		; Halt and Catch Fire ;-)

; Data segment

	ORG	0200h	; Load at memory locaton 200 (hex)

CHAR:	DB	"*"	; Test character

	END
