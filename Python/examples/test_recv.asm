	PAGE	40		; 40 lines per page
	TITLE	"test_recv - Copyright (C) Kevin Cole 2021.11.04 (GPL)"
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    test_recv.asm
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2021.11.04 (kjc)
;
; DESCRIPTION:
;
;     Store the ASCII value of a keypress in memory location 200 (hex)
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;
; Constants ;
;;;;;;;;;;;;;

; I/O

SIO1S:	EQU	10h	; Serial I/O communications port 1 STATUS
SIO1D:	EQU	11h	; Serial I/O communications port 1 DATA

MRST:	EQU	03h	; UART Master Reset
RCVD:	EQU	01h	; Character received
SENT:	EQU	002h	; Data sent. Output complete

; Code segment

	ORG	000h	; Load at memory location 000 (hex)

	MVI	A,MRST	; Initialize serialize input / output device
	OUT	SIO1S	; Reset the UART
	MVI	A,15h	; Settings: No RI, No XI, RTS Low, 8N1, /16
	OUT	SIO1S	; Configure the UART with above settings

; Get a character off of the serial I/O bus (stdin)
;
	LXI	B,WATCH	; Point to buffer
	PUSH	PSW	; Preserve Program Status Word
WAITI:	IN	SIO1S	; Check serial I/O status bit
	ANI	RCVD	; If no data received...
	JZ	WAITI	; ...spin wheels: continue checking status. Else...
	POP	PSW	; ...restore Program Status Word
	IN	SIO1D	; ...read the character
	OUT	SIO1D	; ...echo it
	STAX	B	; ...store it in buffer
	HLT		; ...halt and catch fire

; Data segment

	ORG	200h	; Load at memory locaton 128 (decimal)

WATCH:	DB	000h	; Initialize to zero

	END		; End
