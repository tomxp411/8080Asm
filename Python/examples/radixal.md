# Binary, Octal and Hex in "everyday life"

Well... everyday life for computer folks. :wink:

If you don't know Python or don't know HTML, don't worry. The point of
these two exercises is to get a sense of how often octal and hex come
in handy, and they don't depend on your understanding of Python or
HTML.

(This assumes you are using Linux or Mac OS X.)

----

## Octal

Open a terminal and enter the following four lines.

```text
cat > hello.py <<EOF
#!/usr/bin/env python
print("Hello world!")
EOF
```

That is the infamous "Hello world" program. Now, show some information
about the file with the `ls` command:

```text
ls -lF hello.py
```

Then, type:

```text
./hello.py
```

That should have said something like:

```text
bash: ./hello.py: Permission denied
```

OK... So, now try:

```text
chmod 700 hello.py
ls -lF hello.py

./hello.py
```

The second time, it should have printed "Hello world!".

Going back to the two `ls` commands, compare the first:

```text
    -rw-rw-r-- 1 ?????? ?????? 45 Nov 13 11:48 hello.py
```

and the second:

```text
    -rwx------ 1 ?????? ?????? 45 Nov 13 11:48 hello.py*
```

The `??????` will vary from user to user. The first set of question
marks is the "owner" of the file and will be the username of the person
who created the file (you). The second set of question marks indicates
which groups of users may have access to the file. Typically, there is
a group with the same name as the user, and will only have one member.

But, getting to the **octal**, the part to compare is the first ten
characters. The first indicates what kind of file it is:

* `-` for regular files
* `d` for directories
* `l` for _symbolic links_ which are like aliases, or shortcuts:
  Symbolic links point to another file. Don't sweat that one for now.

There are other first letters but the three above are the most
common. It's the next nine characters we're **REALLY** interested in.
Nine = three groups of three. The first group of three characters
indicate what the owner of a file is allowed to do with the file. The
second set of three characters indicate what members of the group can
do with the file.  And the rightmost set of three indicates what
"everone else" -- users who are not the owner, nor members of the
group -- can do with the file.

* `r` = Read
* `w` = Write
* `x` = eXecute (run it as a command)

If you think of `-` as `0` and anything other than a `-` as a `1`:

Binary | Pattern | Octal
------ | ------- | -----
000    |   ---   |   0
001    |   --x   |   1
010    |   -w-   |   2
011    |   -wx   |   3
100    |   r--   |   4
101    |   r-x   |   5
110    |   rw-   |   6
111    |   rwx   |   7

Armed with the information above, can you determine what `chmod`
followed by three-digit octal number and a file name did?

----

## Hexadecimal

Moving on to hexadecimal, type in the following:

```text
cat > hello.html <<EOF
<html>
  <head>
    <title>Color</title>
    <style>
      body {background-color: #FF0000}
    </style>
  </head>

  <body>
  <h1>Hexadecimal</h1>
  <p>Edit me and change the background color,</p>
  </body>
</html>
EOF

readlink -f hello.html
```

Open a web browser, and set the URL to "`//`" followed by the text
from the `readlink` command. It should look like `///home/.../hello.html`
or `///User/.../hello.html` where "`...`" is your username.

In computers, color is indicated as a combination of different
intensities of Red, Green and Blue -- the notorious RGB as opposed to
the notorious RBG. :wink:

In some languages and applications, the intensities are indicated as
percentages, either as a number between **0** and **100** followed by
a percent sign, or as a decimal number ranging from **0.00** to
**1.00**.

However, in HTML and a lot of other languages, the intensities are
indicated by numbers in the range **0** to **255** -- or in hex,
**00** to **FF**.

00 indicates no intensity. So, #000000 indicates no red (00), no green
(00) and no blue (00) i.e. black, whereas #FFFFFF indicates maximum
red (FF), maximum green (FF) and maximum blue (FF), i.e. white.

Experiment: Change the hexadecimal digits in the `background-color`
to other hexadecimal digits, and refresh your web browser to see
what effects you've had on the color.

**NOTE**: You can use the `rgb()` function in place of the
hexadecimal. So, for example, you could replace `#FF0000` with
`rgb(255,0,0)`. There are also named colors, e.g. `yellow` (a.k.a. 
`#FFFF00` or `rgb(255,255,0)`) and other shortcuts, but the
hexadecimal representation is very common.

----
