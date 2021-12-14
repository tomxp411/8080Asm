	PAGE	40		; 40 lines per page
	TITLE	"bye - Copyright (C) Kevin Cole 2020.11.03 (GPL)"
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;
; NAME:    bye.asm
; EDITOR:  Kevin Cole ("The Ubuntourist") <kevin.cole@novawebdevelopment.org>
; LASTMOD: 2020.11.03 (kjc)
;
; DESCRIPTION:
;
;     When the Altair just can't take it any more... ;-)
;
;     This refactored version abstracts the I/O into callable subroutines
;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Code segment

	INCLUDE	stdlib	; Include standard I/O library

	ORG	000h	; Load at memory location 000 (hex)

	CALL	RSTIO	; Initialize serial input / output bus
	LXI	B,WORDS	; Point to WORDS
	CALL	WRITE	; Write WORDS to stdout (terminal)

HCF:	HLT		; Halt and Catch Fire ;-)

; Data segment

	ORG	0200h	; Load at memory locaton 200 (hex)

WORDS:	DB	"Good-bye, cruel world!"
	DB	CR,LF	; Old-school CRLF newline
	DB	NUL	; NULL string terminator

	END
