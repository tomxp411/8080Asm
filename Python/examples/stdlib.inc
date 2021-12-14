	NEWPAGE	0
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    stdlib.inc
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.11.03 (kjc)
;
; DESCRIPTION: 
;
;     A rudimentary "include" for standard input / output, math,
;     and conversion functions
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

	ORG	1000h	; Load at memory location 1000 (hex)

; Reset serial input / output
;
RSTIO:	MVI	A,MRST
	OUT	SIO1S	; Reset the UART
	MVI	A,15h	; Settings: No RI, No XI, RTS Low, 8N1, /16
	OUT	SIO1S	; Configure the UART with above settings
	RET		; Return

; Put a character on to the serial I/O bus (stdout)
;
; Calling sequence:
;
;	LDAX	register
;	CALL	PUTC
;
; where:
;	register contains the ASCII value to be sent
;

PUTC:	PUSH	PSW	; Preserve Program Status Word
WAITO:	IN	SIO1S	; Check serial I/O status bit 1 (XMIT status)
	ANI	SENT	; If data not sent (i.e. XMIT not finished)...
	JZ	WAITO	; ...spin wheels: continue checking status. Else...
	POP	PSW	; ...restore Program Status Word
	OUT	SIO1D	; ...output byte
	RET		; ...return

; Write a null-terminated string out to the serial port (stdout)
;
; Calling sequence:
;
;	LXI	B,string
;	CALL	WRITE
;
; where:
;	string is the address of a null-terminated ASCII string
;

WRITE:	LDAX	B	; Fetch byte
	CPI	NUL	; If byte is ASCII NUL...
	RZ		; ...return. Else...
	CALL	PUTC	; ...output byte
	INX	B	; ...point to next byte
	JMP	WRITE	; ...lather, rinse, repeat: Fetch next byte.

; Get a character off of the serial I/O bus (stdin)
;
GETC:	PUSH	PSW	; Preserve Program Status Word
WAITI:	IN	SIO1S	; Check serial I/O status bit
	ANI	RCVD	; If no data received...
	JZ	WAITI	; ...spin wheels: continue checking status. Else...
	POP	PSW	; ...restore Program Status Word
	IN	SIO1D	; ...read the character
	OUT	SIO1D	; ...echo it
	RET		; ...return

; Read one line (CR- or LF-terminated string) from the serial port (stdin)
;
; Calling sequence:
;
;	LXI	B,buffer
;	CALL	READ
;
; where:
;	buffer is the address of a byte array for storing the string
;

READ:	CALL	GETC	; Fetch byte
	CPI	CR	; If byte is an ASCII CR (Carriage Return)...
	JZ	CRLF	; ...add LF and return. Else...
	CPI	LF	; ...if byte is an ASCII LF (Line Feed)...
	JZ	LFCR	; ...add CR and return. Else...
	STAX	B	; ...store byte in buffer
	INX	B	; ...point to next empty byte
	JMP	READ	; ...lather, rinse, repeat: Fetch next byte.
LFCR:	MVI	A,CR
	CALL	PUTC	; Print a CR
	JMP	TERM	; Terminate the input string
CRLF:	MVI	A,LF
	CALL	PUTC	; Print a LF
TERM:	MVI	A,NUL	; Null terminator
	STAX	B	; Terminate the input string
	RET		; Return

; Multiply multiplicand by multiplier
;
; Calling sequence:
;
;	LXI	C,multiplier
;	LXI	D,multiplicand
;	CALL	MULTI
;
; where:
;	multiplier   is one of the two numbers to be multiplied
;	multiplicand is the other number to be multiplied
;
; returns:
;	product in register pair BC

MULTI:	PUSH	PSW
	MVI	B,0	; Initialize most significant byte of result
	MVI	E,9	; Bit counter
MULT0:	MOV	A,C	; Rotate least significant bit of...
	RAR		; ...multiplier to carry and shift...
	MOV	C,A	; ...low-order byte of result
	DCR	E	; Countdown to zero (decrement bit counter)
	JZ	MULT2	; If zeroed, we're done
	MOV	A,B
	JNC	MULT1	; If carry bit = 1...
	ADD	D	; ...add multiplicand to high-order byte
MULT1:	RAR		; Else... shift high-order byte of result
	MOV	B,A
	JMP	MULT0	; Lather, rinse, repeat
MULT2:	POP	PSW
	RET		; Return

; Divide dividend by divisor
;
; Calling sequence:
;
;	MVI	B,00h
;	MVI	C,dividend
;	MVI	D,00h
;	MVI	E,divisor

;	CALL	DIVI
;
; where:
;	dividend is the number to be divided
;	divisor  is the number to divide by
;
; returns:
;	quotient  in register pair BC
;	remainder in register pair DE

DIVI:	PUSH	PSW
	MOV	A,D	; Negate the divisor
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

	; 4 register left shift
	; Carry->C->B->L->H

	MOV	A,C
	RAL
	MOV	C,A

	MOV	A,B
	RAL
	MOV	B,A

	MOV	A,L
	RAL
	MOV	L,A

	MOV	A,H
	RAL
	MOV	H,A

	POP	PSW	; Restore loop counter (A)
	DCR	A	; Decrement loop counter
	JNZ	DV0	; Lather, rinse, repeat

	; Post-divide clean-up:
	; Shift remainder right and return in DE

	ORA	A

	MOV	A,H
	RAR
	MOV	D,A	; Store shifted H in D

	MOV	A,L
	RAR
	MOV	E,A	; Store shifted L in E

	POP	PSW
	RET

; Convert a NULL-terminated string to an integer
;
; Calling sequence:
;
;	LXI	B,string
;	CALL	INT
;
; where:
;	string is the address of a string of ASCII digits
;

INT:	LXI	H,0000h	; Initialize subtotal
	MVI	D,00h

CHAR:	LDAX	B	; Fetch char and load into accumulator
	SUI	39h	; If ASCII value > "9" then...
	RP		; ...not a digit. Done.
	LDAX	B	; Refetch char and load into accumulator
	SUI	30h	; If ASCII value < "0"...
	RM		; ...not a digit. Done.

	MOV	E,A	; Save "ones' position" integer digit in E
	MVI	A,09h	; counter for decimal magnitude shift
	PUSH	B
	MOV	B,H
	MOV	C,L
SHFTL:	DAD	B	; Add subtotal to subtotal
	DCR	A	; If loop counter > 0
	JNZ	SHFTL	; ...continue repetitive additon
	POP	B
	DAD	D	; Add in "ones' position" digit
	INX	B	; Point to next char
	JMP	CHAR	; Lather, rinse, repeat

; Convert an integer to a NULL-terminated string
;
; Calling sequence:
;
;	LXI	B,buffer
;	CALL	STR
;
; where:
;	buffer is the address of a byte array for storing ASCII representation
;

STR:	NOP
;	STAX	B	; ...store byte in buffer
;	INX	B	; ...point to next empty byte
	RET		; Return
