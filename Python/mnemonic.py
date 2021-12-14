#
# mnemonic.py
# Represnts a single 8080 mnemonic
#

class Mnemonic:
    text : str = ""
    mnemonic : str = "" # NOP, LXI, STAX, etc
    operand1 : str = None # A, B, D16, etc
    operand2 : str = None # A, B, D16, etc
    length : int = 0
    registers : str = "" 
    description : str = ""

    def print(self):
        
        print("#",self.text)
        print("def",self.mnemonic,end="(")
        if self.operand1:
            print(self.operand1,end="")
        if self.operand2:
            print(",",end=self.operand2)
        print("):")
        print("    pass")
        print()