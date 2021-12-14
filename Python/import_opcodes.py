import mnemonic

#
# gets text up to the next tab character
#
parts = []
ix = 0
mnemonics = {}

def get_start(line:str):
    global parts, ix 
    
    parts = line.split('\t')
    ix = 0

def get_next():
    global parts, ix 
    
    ret = ""
    if ix < len(parts):
        ret = parts[ix]
    ix += 1
    return ret.strip()

def get_mnemonic(mnemonic):
    mnemonic = mnemonic.replace(", ", ",")
    mparts = mnemonic.split(" ")
    oparts = [] 

    if len(mparts) > 1:
        oparts = mparts[1].split(",")
    ret = [mparts[0]]
    if len(oparts) > 0:
        ret = ret + oparts
    return ret 

# Parse and convert to python def statements    
with open("opcodes.txt") as f:
    lines = f.readlines()

count = 0
for line in lines:
    mm = mnemonic.Mnemonic()
    mm.text = line.strip()

    get_start(line)
    opcode = get_next()
    mtext = get_next()
    mparts = get_mnemonic(mtext)
    mm.mnemonic = mparts[0]
    if len(mparts) > 1:
        mm.operand1 = mparts[1]
    if len(mparts) > 2:
        mm.operand2 = mparts[2]
    mm.length = get_next()
    mm.registers = get_next()
    mm.description = get_next()

    if mm.mnemonic != '-':
        mnemonics[mm.mnemonic] = mm 
        mm.print()

    #print("#",line[0:len(line)-1])
    # print("# opcode:      ", opcode)
    # print("# mnemonic:    ", mnemonic)
    # print("# length:      ", length)
    # print("# regsiters:   ", regsiters)
    # print("# description: ", desc)
    # print()
    
    count += 1
    if count > 32:
        break