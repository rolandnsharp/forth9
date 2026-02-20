# Forth9 Implementation Ideas

Design decisions and ideas captured during planning. Not committed — just recorded for when the time comes.

---

## Text Editor: ed-style with visual display

No modes. No vi. No nano. The editor is just Forth words operating on a text buffer, with the file displayed on screen.

**How it works:**

- `edit` opens a file and displays it on screen
- The REPL prompt stays at the bottom — you're always in Forth
- Editor commands are just words that manipulate the buffer
- The screen updates after each command
- `w` writes, `q` quits, back to normal REPL

**Commands:**

```forth
~memo.txt edit                    ( open file, display on screen )
3 p                               ( highlight/print line 3 )
3 "old" "new" s                   ( substitute on line 3 )
5 d                               ( delete line 5 )
3 10 d                            ( delete lines 3-10 )
7 i" new line of text"            ( insert at line 7 )
12 a" appended line"              ( append after line 12 )
"pattern" /                       ( search forward )
w                                 ( write to SD )
q                                 ( quit — back to REPL )
```

**Why this approach:**

- No modal editing — you're never in a "mode" you have to escape from
- The editor IS Forth — you can script it: `~log.txt edit 3 100 d w q` on one line
- `sed` is free — the same words work non-interactively on files
- Tiny implementation — the "editor" is just: a buffer, a display word, and ~10 editing words
- Consistent with the rest of the system — no special keybindings to learn, just words

**Lineage:** Unix `ed` (1971) → `ex` → `vi`. We take ed's philosophy (commands, not modes) and add what ed lacked (you can see the file). The visual display is passive — it just shows the buffer state. All interaction happens at the Forth prompt.

---

## Prefix System: Why Tokens, Not Parsing Words

**The tradeoff:** Traditional Forth gives words the power to consume arbitrary input from the parser (parsing words). This is maximally powerful but makes the language unpredictable — any word can change how subsequent input is interpreted. RetroForth chose a different path: prefix dispatch, where the first character of a token determines how it's handled.

**Three approaches compared:**

| Approach | Power | Predictability | Complexity |
|---|---|---|---|
| Parsing words (traditional Forth) | Unlimited — any word can consume arbitrary input | Low — tooling can't know what a token means without executing code | Moderate |
| Prefix tokens (RetroForth / Forth9) | Constrained — handler sees current token only | High — first character always determines meaning | Simple |
| Able Forth (all literals as parsing words) | Unlimited — but regularised | High in practice | Elegant but more machinery |

**Why Forth9 uses prefixes:**

1. **Small screen, no IDE.** On a 320x320 display with 10 lines of prompt, you need to know what code means by reading it. Prefixes are visually unambiguous — `#42` is always a number, `@alice` is always a peer, `/dev/lora` is always a path. No context needed.
2. **Keystroke economy.** Every prefix eliminates quotes and boilerplate. `@alice` instead of `"/net/alice"` saves 11 keypresses including shifts. On a tiny keyboard this is the difference between fluid and tedious.
3. **User-extensible.** RetroForth's prefix handlers are user-definable. Forth9 inherits this — if you find a new pattern, define a new prefix. The language adapts to your usage.
4. **Tooling-friendly.** A syntax highlighter, a help system, or a smart editor can always determine token type from the first character without executing code. This matters when the editor and the language run on the same 150MHz chip.

**Why not parsing words:** Charles Childers (RetroForth creator) chose prefixes partly as a reaction to parsing headaches from working on PL/0-to-Forth translators and C-to-Forth compilers. The insight: if you don't need arbitrary parsing power (and on a handheld, you don't), constraining the parser makes everything simpler and more predictable.

**Why not Able Forth's approach:** Able Forth makes all literals parsing words with full input stream access, achieving prefix-like regularity without the constraint. Intellectually cleaner, but adds complexity Forth9 doesn't need. The prefix model is simpler to implement, simpler to understand, and the constraints it imposes are features on a handheld, not limitations.

**References:**
- Reddit thread: "What was the rational behind retroforth having its token system" (r/Forth, ~5 years ago) — dlyund's analysis and _crc's confirmation that prefix handlers are user-extensible
- Able Forth approach described by dlyund: all literals as parsing words, no special status for any literal type

---

## Pipe Operator: `>`

Use `>` as the pipe/redirect character instead of `|`.

```forth
sensor:read > ~log.txt            ( redirect to file )
sensor:read > @alice               ( redirect to peer )
```

- Available on every keyboard without shift
- Reads as direction — data flows left to right
- `|` is an alias if the keyboard has it, but `>` is canonical

Note: most Forth composition doesn't need a pipe at all — stack flow IS piping: `sensor:read encrypt @alice send`. The `>` operator is for redirecting to a destination (file, peer, stream), not for chaining words.

---

## Unix-Familiar Command Names

Steal the vocabulary, not the grammar. Use names people already know:

```forth
ls                                ( list — directory contents or devices )
cat                               ( concatenate files — its actual meaning )
read                              ( read/display a single file )
cp                                ( copy )
mv                                ( move/rename )
rm                                ( remove )
grep                              ( search within files )
pwd                               ( not needed — no working directory concept, but alias if people expect it )
```

**Changed from Unix:**
- `cat` means concatenate (its real meaning), `read` means display a file
- `sed` / `awk` not needed — Forth IS the text processing language
- Terse replacements for common patterns:

```forth
: sel ( file pattern -- lines )  read swap filter ;     ( sed -n /pattern/p )
: col ( file n -- values )  read swap nth-col ;         ( awk '{print $n}' )
```

```forth
~log.txt "error" sel              ( grep-like )
~data.csv #3 col                  ( awk-like )
```

---

## LLM as a Mountable Peer

When on home WiFi, mount a local LLM server as a mesh peer:

```forth
@home /dev/llm mount              ( mount home server's LLM over WiFi )
"refactor mesh:relay" /dev/llm ask ( ask it something )
```

- The LLM is just another named stream — `/dev/llm`
- Same protocol as talking to any mesh peer — send a quotation, receive a response
- The LLM lives on YOUR machine, trained on YOUR dictionary
- Suggests words that exist in your image, not generic code
- Device works perfectly without it — it's a tool, not a dependency
- Unmounts when you walk out of WiFi range

---

## Memory Architecture: Image-First Design

**Core principle:** Everything lives in the image. Files are not separate from the system — they are data inside the image, just like word definitions and variables. The SD card is for backup, not daily I/O.

**Memory layout (8MB PSRAM):**

```
┌──────────────────────────────────┐
│  Dictionary (words, code)        │  ~200-500KB
├──────────────────────────────────┤
│  Variables and state             │  ~50-100KB
├──────────────────────────────────┤
│  User data (notes, files, etc.) │  ~7MB
│  Stored as named buffers inside  │
│  the image — not FAT32 files     │
├──────────────────────────────────┤
│  Free space                      │
└──────────────────────────────────┘
```

**Save/load cycle:**

- `save` — writes the entire 8MB PSRAM to SD card as one blob. One write, ~1 second.
- Boot — reads the blob from SD to PSRAM. One read, ~1 second.
- Between saves, the SD card is never touched. Zero wear, zero power for I/O.

**File export/import:**

- `export memo.txt` — copies a named buffer from the image to a FAT32 file on SD, readable by any laptop.
- `import data.csv` — copies a FAT32 file from SD into the image as a named buffer.
- Day to day, you never export. You only export when you need to share a file with another device.

**SD card longevity:**

At one `save` per day (8MB write), an SD card rated for 100,000 write cycles lasts **273 years**.

**Why this is better than files:**

- One SD write per save vs. many writes for individual file operations
- No filesystem overhead (no FAT32 table updates, no directory entries, no fragmentation)
- Faster — PSRAM access is nanoseconds, SD access is milliseconds
- Simpler — no file handles, no open/close, no path resolution
- Lisp machine proven — Symbolics did exactly this for a decade

**Why not raw memory dump of Zeptoforth:**

Travis Bemann (Zeptoforth creator) confirmed that Zeptoforth's internal state (hardware registers, interrupt state, DMA channels) cannot be cleanly separated from user data. A raw dump would leave hardware out of sync on reload.

**Solution:** Build a custom Forth kernel designed for image persistence from day one. Use a contiguous memory region with no hardware state mixed in. The kernel lives in flash and manages hardware setup on boot. The image is pure user state.

**Compatibility with Zeptoforth libraries:**

The custom kernel provides the same basic primitives as Zeptoforth. Zeptoforth's `.fs` library files (WiFi, TCP/IP, FAT32, display, SHA-256, etc.) load on top as-is. We get image persistence AND the entire Zeptoforth library ecosystem.

---

## Event Log Pruning

The append-only event log grows forever unless managed. Simple policy:

```forth
save                              ( snapshot image — new checkpoint )
event:prune                       ( delete events before this snapshot )
#7 event:keep-days                ( auto-prune anything older than 7 days )
```

- Once an image is saved, events that produced it are redundant
- Keep a window of recent events for peer sync (peers who haven't seen you need the diff)
- User controls the policy — one line to set retention

---

## HAL Structure for Multi-Chip Future

From DuskOS: thin Hardware Abstraction Layer per target chip. Everything above it is portable Forth.

```
Flash:
  HAL (chip-specific)     — GPIO base addresses, SPI registers, interrupt vectors, clock config
  Kernel (portable Forth) — dictionary, compiler, prefix listener, quotation compiler, image load/save
PSRAM:
  Image (portable Forth)  — all user words, data, state — identical across chips
```

Changing from RP2350 to ESP32-P4 or RISC-V means rewriting the HAL (~500 lines), not the kernel or the image. User's dictionary and tools carry over unchanged.

---

## Screen Layout: Canvas + Prompt

The 320×320 square PicoCalc display is split using the golden ratio:

```
┌──────────────────────────────────┐
│                                  │
│     Canvas: 320 x 198            │
│     (320 / 1.618 ≈ 198)         │
│                                  │
│                                  │
├──────────────────────────────────┤
│  Prompt: 320 x 122              │
│  ~10 lines of text              │
│  > scope                         │
│  ok.                             │
│  >                               │
└──────────────────────────────────┘
```

- **Canvas** — top 198 pixels. Passive display area. Every tool draws here: oscilloscope waveforms, spectrum analyser, images, file viewer, game graphics, HF waterfall. Just a framebuffer that words write to.
- **Prompt** — bottom 122 pixels. ~10 lines of text. Always visible. Always the Forth REPL. Shows command history, output, errors. Never goes away.

No windows. No window manager. No overlapping frames. One canvas, one prompt. The canvas changes based on what tool is active. The prompt is always there.

Every tool uses the same layout:

```forth
scope                             ( waveform fills canvas )
spectrum                          ( FFT bars fill canvas )
~photo.raw view                   ( image fills canvas )
hf:waterfall                      ( HF spectrum fills canvas )
~code.fs nano                     ( text editor fills canvas )
~code.fs ed                       ( ed file viewer fills canvas )
gfx:clear                         ( blank canvas )
```

---

## Games

### The Tower (Interactive Fiction Tutorial)

The default first-boot experience. The player wakes in a room with a terminal. Each room is a Forth lesson disguised as a puzzle.

**Room progression:**

| Room | Concept | Puzzle |
|---|---|---|
| 1 | Stack, arithmetic | "The door opens when the stack holds 42" — `6 7 *` |
| 2 | Defining words | "Define 'water' to make the flowers bloom" — `: water ...` |
| 3 | Stack manipulation | Rearrange stones in order using `dup`, `swap`, `rot` |
| 4 | Conditionals | A guard asks riddles — `if`/`then` logic |
| 5 | Loops | Light a row of torches — `do`/`loop` |
| 6 | Strings and output | Write a message on a wall — `."` and string words |
| 7 | Dictionary | A library of spells — `words` to list them, `see` to read them |
| 8 | Prefixes and devices | Escape the tower — use `/dev/`, `@`, `~` to contact the outside world |

The game world IS the dictionary. `look`, `take`, `go` are Forth words. The parser IS the REPL. The player doesn't switch between "game mode" and "Forth mode" — they were in Forth the whole time.

Ships as the default image. Complete the game and you're at the REPL with the skills to use it.

### Rogue9 (Roguelike)

- Dungeon generation: a few Forth words for rooms, corridors, doors
- Every entity is a word: `: goblin #10 hp #3 atk #1 def ;`
- Player can `' goblin see` to read any entity, redefine to modify
- Tile-based graphics on the 320x198 canvas
- Turn-based — no timing complexity
- Mesh integration: share dungeon floors over LoRa, pull floors from peers
- The game is ~100 words. The whole thing is inspectable and modifiable.

### Chess

- Standard algebraic notation as Forth words: `e2 e4`, `Nf3`, `O-O`, `Qxd7`
- ~70 words: 64 squares + piece prefixes + special moves
- `chess:start` loads chess vocabulary, `chess:stop` unloads (avoids dictionary clashes)
- Board displayed on canvas, moves entered at prompt
- Mesh play: `@alice e2 e4` sends a move over LoRa
- Move validation, check/checkmate detection — all Forth words

---

## Optional Type System

**Inspiration:** StrongForth (Stephan Becher) — a statically typed Forth. Used by a Hacker News commenter for Dreamcast homebrew: single process, no MMU, static typing prevented crashes that would take down the whole system. Exactly Forth9's situation.

**Reference:** <https://www.stephan-becher.de/strongforth/intro.htm>

The idea: lightweight optional type annotations checked at compile time, zero runtime cost. You annotate when you want safety, leave them off when you're exploring.

```forth
: mesh:send ( packet:mesh-pkt peer:addr -- status:err )
  ... ;
```

**Design rules:**

- Types are OPTIONAL. Untyped words work exactly as normal Forth. No friction at the REPL during interactive exploration.
- Type checking happens at compile time only. Zero runtime cost — no tags, no boxing, no overhead.
- Types are stack-effect annotations, not a separate type system bolted on. They extend the existing `( -- )` stack comment convention.
- If you're poking registers and testing ideas, leave types off. If you're writing a mesh protocol that must not crash, add them.
- The StrongForth lesson: on a system with no MMU and no process isolation, a type error IS a system crash. Optional types let you choose when that protection matters.

**Not yet decided:**

- How deep does the type system go? Just stack-effect shapes, or full structural types?
- Can user-defined types participate? e.g. `type: mesh-pkt ( src dst ttl payload )`
- Should the type checker be a separate pass, or integrated into the compiler?
- Decision point: Phase 5 or later. Don't design this until real code on the device reveals what errors actually bite.

---

## STC Fallback Path

If porting Mecrisp's native code compiler to a new architecture is too expensive, switch to Subroutine Threaded Code (DuskOS-style):

- STC compiles words as `CALL addr` sequences — portable, simple
- ~70-80% of native speed — acceptable for most things
- Keep performance-critical inner loops (DSP, audio, radio) as hand-written native code
- User-facing Forth is identical — same prefixes, same quotations, same words
- Decision point: Phase 17 (custom hardware), not before

See `research-notes.md` for full STC vs native code comparison.
