# Digging in to slow-count.asm

This is mostly for Peter, as he was previously pondering
[slow-count.asm](slow-count.asm). 

The rest of you may want to come back to this later, after you're
comfy with toggling in a program on the front panel, and then
examining a memory location via the toggle switches to see what's
changed.

I am going to be a bit pedantic and belabor the explanation.

## Symbols and symbol tables

Assemblers, interpreters and compilers refer to variable names,
function names, attribute names, and other non-keyword, non-value text
as **symbols** and keep track of them internally in what is referred
to as a **symbol table**. I mention it just in case you see the term
pop up in your explorations.

### Variables vs. constants

First, there is a difference between variables,which vary, and
constants, which do not. Some languages come equipped with certain
constants built in. For example, you often don't need to write out
3.14159... The language might provide you with a symbolic constant
named `pi`. If it does, you can then type some variation of "`pi`" in
your code instead of specifying the numeric value of Pi as a series of
digits.

Another example: colors. Colors are often expressed as a mix of the
three primary colors of light: red, green and blue. Those can be
expressed as three decimal numbers, three hexadecimal numbers, or
three percentages. Or, they can often be expressed via a name.
"yellow" for example indicates 100% red mixed with 100% green, and 0%
blue. You don't need to type `#FFFF00` or `RGB(255,255,0)` or `(1.00,
1.00, 0)`.  If you're somewhat sadistic, some languages might enable
you to torture people by using `16776960` to represent yellow. That is
FFFF00 as a decimal value. But, if you're a helpful sort, you can
often simply type "yellow" or color.YELLOW, or some other helpful
representation into your code.

When you are using a modern programming language, C or Python or even
old languages like FORTRAN and BASIC, you will see "assignment
statements" like:

```
    x = 5
    prompt = "Press any key to continue..."
```

`x` and `prompt` are variables, and if you wanted to, you could vary
their content later in the program by assigning other values to them.

### Machine code has no named variables or constants

By this point, you may be bored and saying "Obviously. Why is he
droning on and boring me to tears?"...

But with machine language... Say it with us, "It's all just... what
now?" "BITS!"

There are no constants. There are no variables. At least not as you
have known them. There are only memory locations, referenced by their
address -- which is like a list index for a very, very large list --
and the contents of the location. The contents of memory at a specific
address is a group of eight bits. Those bits can take on different
meanings depending upon how they are used.

But, as you've already learned, toggling bits into memory is both
tedious and error-prone. People quickly begin making mistakes after 15
minutes of 1011 0111 1001 0000 1010 1010 1101 0111 0111 1111... Even
making the mental jump to seeing it as hexadecimal B7 90 AA D7 7F, it
doesn't have a meaning that human beings can quickly latch onto.

Keywords like "`for`" and "`if`", operators and punctuation like
"`+`", "`=`", "`[`", assembly language mnemonics like "`MVI`",
variable names, constant names, functions, are conveniences. I could
write code in Python like:

```
    import math
    cole = input("Enter a value between 0 and 360: ")
    elkner = cole * (math.pi / 180)
    print(elkner)
```

And it would happily convert degrees to radians, even though I chose
ridiculous, meaningless variable names.

### Assembly language syntax

So... finally, trying to get to the point... When you were looking at
my assembler code, the first thing to notice is the general syntax of
a line of code:

* anything following a semicolon ("`;`") is a comment and will be
  ignored by the assembler.

* the first six columns are either blank or a "label" followed by a
  colon ("`:`"). A label is like a variable name or a function name or
  a class name. But you should really learn to think of it as what it
  truly is: It does not identify the contents or value of anything. It
  identifies the memory address of the line. You use the label as a
  shorthand for a memory address of a byte -- possibly the first of
  several consecutive bytes -- that you want to load, store, change,
  or... jump to and execute.

* The next columns are the mnemonic for an machine op code, taken from
  the manual you have been studying. Or... Well, wait, a sec. I'll get
  to the "or" in a minute.

* The next columns are either blank, or contain information needed to
  "flesh out" the instruction. As you read through Part 4 of the
  manual you will notice that many instructions either require more
  than a single byte, or that the instruction itself has a few bits
  embedded in it that change depending upon which register you want
  the instruction to operate on.  For example, the "Move Data" (`MOV`)
  instruction requires a source and a destination. Where are you
  moving data from? Where are you moving data to? So, it requires two
  arguments following the mnemonic.

```
    ; An example... Here's a comment.

    LOOP:  MOV   B,A   ; Move the contents of register A to regsister B
           JMP   LOOP  ; Go to loop. Repeat the move instuction ad nauseum
```

### Pseudo-instructions

Now it's time for that "or..." that I said I was going to get to in a
minute.  There are some "pseudo-instructions" which don't tell the
computer what to do, but rather, provide a convenience to the coder --
and later, unless you chose ridiculous, meaningless variable names,
convenience to the person debugging the code (who might be the
original coder). 

### EQU

Sometimes, the pseudo-instructions are just defining a constant you
can use. Those are the EQU lines in the code.

When you use the immediate instructions like "Load Register Pair
Immediate", you use the mnemonic, "LXI", a reference to the register
pair being loaded, "B" for "B and C", "D" for "D and E" or "H" for "H
and L", and a single byte value 0-255 (decimal), which is the same as 
000-377 octal or 00-FF hexadecimal. That single byte value is "constant".
It is not a memory address. It is a value that gets used directly and
immediately. But, it would be nice to give a meaningful context to the
number. This is where EQU comes in. At the top of the program you were
looking at, there were several lines:

```
    ;;;;;;;;;;;;;
    ; Constants ;
    ;;;;;;;;;;;;;

    ; ASCII characters

    CR:     EQU    0DH    ; ASCII CR  (Carriage Return, a.k.a. Ctrl-M)
    LF:     EQU    0AH    ; ASCII LF  (Line Feed        a.k.a. Ctrl-J)
    ESC:    EQU    1BH    ; ASCII ESC (Escape,          a.k.a. Ctrl-[)
    NUL:    EQU    00H    ; ASCII NUL (Null)
```

These are, in effect, constants. They are all values between 0 and FF
hexadecimal, and, therefore will fit just fine in any of the machine
instructions that require a byte after the instruction. In other words,
with the lines above included in the code, I can write either:

```
            LXI    B,1BH  ; Load the hex value 1B into register pair B,C
```

or:

```
            LXI    B,ESC  ; Load an ASCII ESC into register pair B,C
```

They will do exactly the same thing, but the second form is a lot more
helpful, even without the comment.

The `EQU` lines are only used by the assembler. They never generate
any machine language. The assembler sees them, and when it is
converting your source program to binary machine language, it does a
"find and replace" replacing all occurances of `ESC` with the actual
value `1B` (or if you prefer, `0001 1011`)

### ORG and DB

Later in the code, near the very end, you see:

```
    ; Data segment
    
            ORG   200H    ; Load at memory locaton 200 (hex)
    
    WATCH:  DB    000H    ; Initialize to zero

    WORDS:  DB    ESC,"[2J"
            DB    CR,LF
            DB    CR,LF,"            "
            ...
            DB    "Press a printable key slowly several times,"
            ...
            DB    "D0-D7 will equal to the number of key presses."
            ...
            DB    NUL    ; NULL string terminator
```

### ORG

First, let's look at the pseudo-instruction `ORG`.

`ORG` lets you tell the assembler where you would like to deposit
code. If you want, you can think of it as toggling switches on the
front panel of the Altair, and then flicking the "EXAMINE" toggle.
You have "primed the pump" so to speak, setting the Altair up so
that the a "DEPOSIT" will deposit at the memory location you just
examined.  So, the first line above says, "Now, start depositing
code at memory address 200 (hexadecimal)."

### DB (Define Bytes)

`DB` is also a pseudo-instruction, but, unlike `EQU` it does actually
generate binary values that get embedded in your binary machine code
file and loaded into the computer's memory.

`DB`: "Define Bytes". As Sigmund Freud never said, "Sometimes a byte
is just a byte". (He was reputed to have said "Sometimes a cigar is
just a cigar" in reference to dream interpretation but I won't get
into that.) It's all in how you and your code choose to interpret the
values.

Notice the comment at the top of that section: "Data segment". I chose
some arbitrary location in memory where I'm going to keep data --
bytes that I do not want to be interpreted as instructions.  I've
taken a wild guess that my program -- the executable machine
instructions -- will not use up more than 200 hexadecimal bytes of
memory, and I know that the Altair normally expects the first
executable instruction to be at memory address 0.

So, I'm thinking I'm probably "safe" storing data at addresses beyond
200 hex.

The first byte that I define is at memory location 200. When the code
is assembled, the assembler deposits 000 in memory address 200 hex
which is the same as memory address 1000 octal. I have labeled that
address "`WATCH`".

The next very many bytes, seperated by commas, begin one byte after
that.  So, 201 hexadecimal or 1001 octal. I use the constants I
defined at the beginning of the program to insert characters that are
not printable.  In Python and other languages you've probably seen
"`\n`" for the "newline" character. If I wanted to split "Hello World"
into two lines, in many languages, I could write `"Hello\nWorld"` and
where the "`\n`" occurs the compiler or interpreter would replace it
with a new line.

### Too much information (ancient history)

What your textbooks, videos or other sources of info might not tell
you any more is that computers used to require two characters in order
to split a string across two lines, and in some cases still do. This
goes back to the era of non-electric, purely mechanical typewriters.
There was a cylindrical roller attached to a movable "bed". When you
pressed a key on the typewriter, a wee hammer with an embossed
character on the end of it would be forced up hard against a cloth
ribbon that had been saturated with ink, and the ribbon would press
against the paper, leaving an ink stain in the shape of the character
on the hammer head.

Then, the "bed" would move one "character's width" to the left. (The
hammers could not move to the right. So the paper had to move to the
left in order to place the next character to the right of the previous
one.

When the bed -- actually referred to as a "carriage" had shifted so
far to the left that the next hammer strike would be off the edge of
the paper, the person using it would have to return the carriage to
its starting position -- a.k.a. a "**Carriage Return**" often
abbreviated "CR" However, once that was done, the typewriter was still
on the same line as it was originally. It was merely back to pointing
to the beginning of the line. This meant that you could overstrike the
letters. Sometimes, this was a good thing: Typing the same text a
second time would double the amount of ink, and make the text darker
and, because of slight mechanical inprecision, not exactly 100% in the
same position as the original keystroke. This would produce **bold**
text. Or, if you wanted to underline the text, you could now type a
bunch of underscore characters and they would be hammered into place
at the bottom of each letter.

But typically, you would want to turn the roller holding the paper
slightly counter-clockwise to move the paper up so that you could
begin typing on the next line. This is known as a "**Line Feed**",
typically abbreviated "LF".

In the ASCII character set, several non-printable characters that have
their own names are also accessible via control characters.  Carriage
Return is a Ctrl-M, Line Feed a Ctrl-J, Tab is Ctrl-I, ESCape is
Ctrl-[. There are several others.

The Altair is old-school enough to require the "crillif" -- CRLF a
carriage return followed by a line feed. So, you'll see lots of
that in the data.

"Fun?" fact: To complicate the lives of programmers, there was a bit
of a schism: Microsoft and many of its ancestors went with the
original new line scheme: a carriage return followed by a line
feed. Unix-like systems, including Linux, went with the line feed
character to represent a new line. And Apple chose the carriage return
to represent a new line.

### ANSI Escape codes / ANSI Escape sequences

"But, how do you change color in a terminal window?" I'm glad you
asked. Even if you didn't, I can imagine you did.

There are a set of character combinations, that typically begin with
the ASCII code for the ESC character. These are collectively called
the ANSI Escape sequences or [ANSI Escape
codes](https://en.wikipedia.org/wiki/ANSI_escape_code). When a
terminal capable of handling color receieves a combination like ESC
followed by "[31m;" it will change the color of the text to red. ESC
[0m; resets to whatever the default is. The curious can dig into ANSI
escapes by searching Wikipeda.

### NULL terminator

And finally the NULL character: In C and other programming languages,
you need a way to indicate where a string of characters end, when you
do not know how long a string is, or are too busy to count it out. This
is often done by putting a byte 0000 0000 at the end of the string.
This is referred to as the NULL character.

----

## Serial Input and Output

Think for a minute about what you know about [Morse
code](https://en.wikipedia.org/wiki/Morse_code). I'm hoping you've
seen some old movie or television drama somewhere where it was used.

It is a series of tones or electrical signals, some short, some long.
Dots and dashes. Or, if you rotate the dashes 90 degrees, you can think
of ...-.--- as 00010111. 

The key concept here is "a series of". Serial. In order to represent
the letter "S" in Morse code, a sender must send three short pulses --
dots, one after the other. The letter "O" is three long pulses, or
dashes.


We've been talking about bytes as groups of eight bits. However, when
sending information over a serial cable or a phone line, you cannot
send eight at a time. You have to send each bit one by one, and then,
have some sort of indicator that you have finished a set of eight. The
receiving end of the connection must then group those eight individual
bits back into a byte and handle the data -- store it, write it to
disk, add it to some other data, interpret it as a printable character
and send it to the screen, interpret it as a machine instruction and
execute it or any number of other possibilities.

In Morse code, and in computer transmission, there has to be some sort
of synchronization. You've all had the experience of two people
talking simultaneously on a phone or computer, and having the
transmission garbled. Even when you're not talking at the same time,
connections can be fickle and noisy, dropping out part of what is
being said.

So you need a protocol -- an agreement between the participants on
when each of them will get a chance to speak to the others, and how
missing information will be handled. 

"What? I didn't hear that. Could you repeat?"

"I'm sorry. It's still pretty garbled. Could you spell it for me?"

Also, if you're a musician -- or just a fan of music, you'll know that
rhythm is important. If I'm sending dots and dashes, or ones and
zeroes, you need to know what rate I am sending them at, so that you
can be ready to receive them in the right order. We also need to agree
on a "gap" -- a quiet time when no long or short pulses are being sent
-- so that you will know when one piece of information ends, and the
next begins.

As programmers, we've got it a bit (pun intended) easier than
telegraph operators did. Morse code used a different number of pulses
for each letter, number or punctuation mark. For example, "S" and "O"
are both three pulses -- or bits -- while the letters "E" and "T" are
a single bits (a dot for "E" and a dash for "T"). "H" is four dots. In
other words, like a variable-length byte. If we were thinking about them
as binary,

Letter | Code
------ | ----
E      | 0
H      | 0000
O      | 111
S      | 000
T      | 1

(The lengths and patterns were chosen largely based on frequency of
use: E is the most commonly used letter in English, and "T" is the
second most frequently used.)

Fortunately, we can depend upon all bytes being the same number of bits.
So, for example, if we agree that:

* a byte will be eight consecutive (sequential) pulses (bits).
* a 0 will be represented by a short tone or pulse of 0.25 seconds
* a 1 will be represented by a long tone or pulse of 0.50 seconds
* there will be a 0.25 pause between consecutive bits
* there will be a 0.50 second pause between groups of bits.
* there will be a weak verification method: a nineth bit will be added
  to the sequence. If an odd number of 1 bits have been sent in the
  byte (e.g. 0000 1011) the nineth bit will be a 1. If an even number
  of 1 bits in a byte have been sent (e.g. 1010 1010) the extra bit
  will be a 0. Both sides will always expect an even number of 1 bits
  in the transmission.  If that doesn't happen, then the transmission
  of that byte is assumed to be garbled, and the receiving end asks
  for it to be sent again.
* some sort of acknowledgement that a byte has been received correctly
  -- possibly just the absence of a message back saying "What?" is
  incorporated into the protocol.

There. We've established a rudimentary set of rules -- a protocol --
for communicating using nothing but tones or flashes of light.

Except for the timing (waiting 0.5 seconds for example is hellishly
s-l-o-w!) the above is really, almost identical to what is happening
on that cable connecting your laptop to the Altair.

The rate of transmission is measured in units known as
[Baud](https://en.wikipedia.org/wiki/Baud) - named after the gent who
created a code that replaced Morse code.

The weak checking system I described above is known as a parity check
using a [parity bit](https://en.wikipedia.org/wiki/Parity_bit).

The "gap" between consecutive groups of bits is the "stop bit".

We set `minicom` up for 

* 9600 baud
* 8 data bits
* 1 stop bit
* no parity (i.e. no nineth bit added. The above example describes
  **even parity**. The other alternative, logically enough, is **odd
  parity** where you just flip the rule above.)

This is often written as some variation of "9600, 8N1".

See [asyncrhonous serial
communication](https://en.wikipedia.org/wiki/Asynchronous_serial_communication)
if you **REALLY** want to dig into this, but for now, it's really not
worth your time.

### Waiting...

Even the Altair, slow as it was, was faster than data sent serially
over a phone line -- or a serial cable.

Ever wonder what **USB** means? Well, now you should because it's
relevant. Particularly the "S" in the [Universal Serial
Bus](https://en.wikipedia.org/wiki/USB).

The cable connecting your computer to the Altair is "modern" at one
end, and "ancient" at the other end, but both ends are using a shared
understanding of serial communication.

I mentioned above that there should be some sort of acknowledgement that
data has been successfully sent or received.

In the Altair, it's not so good about "successfully" but it is good
about determining if information has finished being sent or received.

It has two methods for determining this. There is a set of status flags
that determine the settings for communication -- the 9600,8N1 -- and
also whether or not a byte is waiting to be sent, or a byte has been
received but not handled. 

One way to check the status is to set the communications up so that
the Altiair interrupts itself every time a byte is sent or received.
(RI enabled = receive (RECV) interrupt enabled, XI enabled = transmit
(XMIT) interrupt enabled).

The other method is "polling" where you first disable RI and XI. Then
in your code, you loop, checking the status bit and jumping out of the
loop when the bit indicates that a byte has been successfully sent or
received.

Conceptually, sort of, in Python, sending from the Altair to your 
screen is:

```
    sent = 0                # Reset communications status
    output = None           # communications data register
    settings = "9600,8N1"   # Establish communcations protocol

    words = "Here is a string I want to be sent from the Altair"
    for byte in words:
        while not sent:     # check the transmit (XMIT) flag
            pass            # Nope. Still zero. Check it again.
        output = byte       # Put thw byte on the serial output bus
```

So, first, reset the serial I/O board. Next, make sure that the baud
rate, parity, etc -- and also the RI and XI -- are set for however
you've chosen to communicate.

In [slow-count's](slow-count.asm) data segment, I have `WORDS`
referring to the first byte of the memory address where I've stored a
few short paragraphs that guide users of this program, telling them
what to do.

Then, I loop and loop and loop, spinning my wheels waiting for the
Altair to say, "Yep. I'm ready to send dots and dashes out on that
wire hanging off the back of me." 

So, we take the byte that `WORDS` is pointing to, check it to make
sure it is not a NULL byte, and if we're okay, put it on the I/O bus
and send it with `OUT`. Increment the pointer to point to the next
byte of information and go back to checking the status (loop, loop,
loop, spin wheels).

If the byte we fetched IS a NULL byte, we're out of the sending loop and
on to the next part of the program, which is the input loop.

The input loop is very much like the output loop: Spin wheels until there
appears to be data waiting to be received. Once the system says "Yep. 
Something's waiting in the input hopper." do something with it. 

In the case of slow-count, all it does is send the byte out as output
to the screen -- there's no reason that input coming from your computer
to the Altair would be displayed on your computer. So, the Altair has
to explicitly "echo" it back. Then, it increments a counter, counting
the number of bytes received, until you stop the program and examine
the memory location referred to by `WATCH` (octal 1000).

And that's essentially it.

A lot to absorb, though.

----
