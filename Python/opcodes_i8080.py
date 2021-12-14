# 
# 8080 opcodes, written as Python routines
# Each opcode outputs one or more bytes representing the mnemonic and operands
# 
# Opcodes in this file use Intel 8080 mnemonics
#

import asyncio

from asyncio.streams import StreamWriter

class assembler_i8080:
    sout : StreamWriter = None

    def __init__(self, output_stream):
        self.sout = output_stream

