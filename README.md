# 8080Asm
Assembler for Intel 8080 

This is a work in progress. Currently, this assembles Intel 8080 assembly langugage, according to the opcode list at https://pastraiser.com/cpu/i8080/i8080_opcodes.html

The 8080 assembler currently outputs Intel hex files. The idea is to load the files directly to an Altair 8800 emulator for direct execution (with no operating system) 
or upload the files to a CP/M computer and use the LOAD command to install and execute the code.

## Future work:

Expressions in operands. ie: MVI A, DATA + 5

I'd also like to see Zilog mnemonics, as seen at: https://clrhome.org/table/

Mnemonics should be selectable based on prepressor directives, like this:
`#Z80 for Zilog Z80 mnemonics (most 80s CP/M computers, MSX)`
`#Intel8080 for Intel 8080 mnemonics (Altair 8800, IMSAI 8080
`#Intel8080 for Intel 8085 opcodes (Tandy 100)
`#TEA - an "unusual" assembler for 8085

Feel free to create pull requests for changes, bug fixes, and alternate mnemonics or instruction sets. 

(c) 2021 Tom Wilson wilsontp@gmail.com MIT license.