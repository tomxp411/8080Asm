#!/usr/bin/make -f
# Written by Kevin Cole <ubuntourist@hacdc.org> 2020.10.17 (kjc)
#
# See "pinfo make" -> Complex Makefile example
#
# This takes Altair-8800 assembler source files and produces:
#
#    - an object file       (.p)
#    - an Intel HEX file    (.hex)
#    - a  binary executable (.bin)
#    - a  list report file  (.lst)
#    - an include(?) file   (.inc)
#    - 2 PDFs:
#        + the list report file with octal       op codes (.lst-oct.pdf)
#        + the list report file with hexadecimal op codes (.lst-hex.pdf)
#
# To use:
#
#        make         # to generate for all .asm files
# or     make root    # (where "root" is the root of a specific "root.asm")
# or     make clean   # to remove all generated files
#
############################################################################

SHELL = /bin/bash
SRCDIR = src
BINDIR = bin
DOCDIR = doc
OBJECTS := $(patsubst %.asm,%,$(wildcard *.asm))

.PHONY : all
all : $(OBJECTS)

#.PHONY : clean
#clean :
#	-rm -f	*.hex         \
#		*.bin         \
#		*.lst         \
#		*.inc         \
#		*.lst-hex.pdf \
#		*.lst-oct.pdf \
#		*.ps          \
#		*.p

.PHONY : clean
clean :
	@find . -name "*~"            -printf "Deleted: %f\n" -delete
	@find . -name "*.hex"         -printf "Deleted: %f\n" -delete
	@find . -name "*.bin"         -printf "Deleted: %f\n" -delete
	@find . -name "*.lst"         -printf "Deleted: %f\n" -delete
	@find . -name "*.inc"         -printf "Deleted: %f\n" -delete
	@find . -name "*.lst-hex.pdf" -printf "Deleted: %f\n" -delete
	@find . -name "*.lst-oct.pdf" -printf "Deleted: %f\n" -delete
	@find . -name "*.ps"          -printf "Deleted: %f\n" -delete
	@find . -name "*.p"           -printf "Deleted: %f\n" -delete

% : %.asm
	asl -a -cpu 8080 -L -listradix 8 $<
	enscript -G -j -r -E \
		 -p enscripted.ps \
		 -b "$@.lst (octal)" \
		 -t "$@.lst (octal)" \
		 -J "$@.lst (octal)" \
		 $@.lst
	ps2pdf enscripted.ps
	mv enscripted.pdf $@.lst-oct.pdf
	asl -a -cpu 8080 -L -listradix 16 $<
	enscript -G -j -r -E \
		 -p enscripted.ps \
		 -b "$@.lst (hex)" \
		 -t "$@.lst (hex)" \
		 -J "$@.lst (hex)" \
		 $@.lst
	ps2pdf enscripted.ps
	mv enscripted.pdf $@.lst-hex.pdf
	rm enscripted.ps
	p2hex $@.p
	p2bin $@.p
	mv $@.p $@.bin $@.hex $@.inc $(BINDIR)/
	mv $@.lst $@.lst-oct.pdf $@.lst-hex.pdf $(DOCDIR)/
