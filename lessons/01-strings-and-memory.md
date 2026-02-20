# Lesson 1: Strings and Memory

## What you already know
- Stack: `dup`, `swap`, `drop`, `over`
- Arithmetic: `+`, `-`, `*`, `/`, `mod`
- Defining words: `: name ... ;`
- Variables: `variable`, `!`, `@`
- Constants: `constant`
- Conditionals: `if...else...then`
- Loops: `begin...until`, `do...loop`

## Strings in Forth

Forth has no string type. A string is just two things on the stack:

```
address length
```

That's it. An address in memory and how many characters long it is.

### Printing strings

`." hello"` — prints immediately. The `."` word reads up to the closing `"` and prints. Only works inside a definition or at the prompt.

`s" hello"` — puts the address and length on the stack. Doesn't print.

`type` — takes an address and length from the stack, prints those characters.

So:
```forth
s" hello" type    \ prints: hello
```

is the same as:
```forth
." hello"
```

But `s"` is more powerful because the string is on the stack. You can pass it to other words.

### Why this matters

When you build your own Forth, every input line is a string — an address and a length. The outer interpreter reads that string word by word. Understanding that strings are just (address, length) pairs is essential.

## Memory in Forth

You already know `variable` — it creates a named slot that holds one number.

But what if you want to store 80 characters? You use `create` and `allot`:

```forth
create buffer 80 allot
```

This creates a word called `buffer` that pushes the address of an 80-byte region onto the stack. The memory is yours to use however you want.

### Writing to memory

```forth
65 buffer c!       \ store byte 65 (ASCII 'A') at start of buffer
66 buffer 1 + c!   \ store byte 66 (ASCII 'B') at second position
```

`c!` is "character store" — stores a single byte (vs `!` which stores a full cell, usually 4 bytes).

`c@` is "character fetch" — reads a single byte.

```forth
buffer c@ emit     \ prints 'A' (emit prints one character by ASCII code)
```

### Storing a string in a buffer

```forth
s" hello" buffer swap move    \ copy the string "hello" into buffer
```

`move` copies n bytes from one address to another: `( source dest count -- )`

Wait, that's wrong. `s"` gives `( addr len )`. You'd do:

```forth
s" hello" buffer over move    \ source=addr, dest=buffer, count=len
```

Then to print it back:
```forth
buffer 5 type    \ prints: hello
```

You have to remember the length. That's why real Forth systems either:
- Store the length alongside the data (counted strings)
- Keep the length in a variable

## Counted strings

A counted string stores the length in the first byte:

```
byte 0: length (5)
byte 1: 'h'
byte 2: 'e'
byte 3: 'l'
byte 4: 'l'
byte 5: 'o'
```

`count` is a Forth word that converts a counted string address to an (addr, len) pair:

```forth
my-counted-string count type    \ prints the string
```

## Key symbols so far

| Symbol | Name | What it does |
|--------|------|-------------|
| `c!`   | c-store | Store one byte |
| `c@`   | c-fetch | Fetch one byte |
| `!`    | store | Store one cell (4 bytes on ARM) |
| `@`    | fetch | Fetch one cell |
| `emit` | emit | Print one character by ASCII value |
| `type` | type | Print a string (addr len) |
| `move` | move | Copy n bytes from one place to another |
| `allot`| allot | Reserve n bytes of memory |
| `create`| create | Make a word that pushes an address |

## What this means for your Forth

In the Forth you'll build:
- The input buffer is a `create` + `allot` — just a chunk of memory
- Each word in the dictionary is a counted string (name) followed by code
- `type`, `emit`, `c@`, `c!` are your primitives for all text I/O
- There is no string type. There are only addresses and lengths.

Everything is memory. Strings are memory. Variables are memory. The dictionary is memory. The stack is memory. Once you see that, you see Forth.

## Next lesson: The Dictionary

How Forth stores word definitions, how `find` looks them up, and how the outer interpreter works. That's the core of building your own Forth.
