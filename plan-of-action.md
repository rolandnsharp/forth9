# Forth9 Plan of Action

A phased build plan from Zeptoforth to sovereign handheld.

**Architecture:** Zeptoforth is the base layer — it handles boot, hardware drivers, and native code compilation. Forth9 is a persistent world built on top — compiling native code into PSRAM via Zeptoforth's code generator, with its own dictionary and image save/load. Since PSRAM is memory-mapped at a fixed base address on the RP2350, native code saved to SD and restored to the same address needs no relocation — all branch targets remain valid. No threaded code, no GC, no interpreter overhead. Full native speed. The user only sees Forth9. Zeptoforth is plumbing.

---

## Phase 0: Setup (DONE)

- [x] Order **Pimoroni Pico Plus 2W** (RP2350 + 8MB PSRAM + 16MB flash + WiFi/BT)
- [x] Order **Raspberry Pi Pico 2W** (for early development)
- [x] Order **ClockworkPi PicoCalc** (case, keyboard, screen, battery) — awaiting delivery
- [x] Flash **Zeptoforth** to Pico 2W
- [x] Set up serial terminal (picocom) and zeptocom.js (web terminal with handshaking)
- [x] Learn Zeptoforth basics: variables, constants, loops, flash compilation
- [x] Customise prompt (`f9>`)
- [ ] Order **SX1262 LoRa module** (868/915 MHz, SPI interface)
- [ ] Order **microSD card** (small capacity fine — 1-4GB)

**Milestone:** Zeptoforth running on Pico 2W, serial connection working, comfortable with the REPL.

---

## Phase 1: Learn Zeptoforth Internals

_Goal: Understand Zeptoforth well enough to build on top of it._

- [ ] Study Zeptoforth's outer interpreter — how it reads tokens, finds words, compiles
- [ ] Study the `hook` system — `prompt-hook`, outer interpreter hook, etc.
- [ ] Study `begin-module` / `end-module` — how namespacing works
- [ ] Study Zeptoforth's memory layout — flash, SRAM, PSRAM regions
- [ ] Study Zeptoforth's multitasking model — tasks, channels, semaphores
- [ ] Read the FAT32 and SD card modules — understand how file I/O works
- [ ] Read the PSRAM module — understand how PSRAM is accessed
- [ ] Experiment: write words to PSRAM manually, read them back
- [ ] Map out which Zeptoforth words your persistent world will call into

**Milestone:** You understand where Zeptoforth ends and your world begins. You know the boundary.

---

## Phase 2: Persistent World — Core

_Goal: Build the Forth9 world that lives in PSRAM and survives power cycles._

- [ ] Design the PSRAM memory layout for your world:
  - Dictionary (word headers + native code, compiled by Zeptoforth's code generator)
  - Data heap (named buffers — your "files")
  - Variables and state
  - Free space pointer
- [ ] Configure Zeptoforth's compiler to target PSRAM for Forth9 definitions
- [ ] Implement the Forth9 dictionary — linked list of word headers in PSRAM
- [ ] Implement `find` — search the Forth9 dictionary first, fall through to Zeptoforth's dictionary if not found
- [ ] Verify native code in PSRAM executes correctly (branch targets, call addresses)
- [ ] Test: define a word in the Forth9 world, execute it, see it work
- [ ] Use Zeptoforth's preemptive multitasking directly (no GC = no reason to avoid it)
- [ ] Implement `image:save` — dump the PSRAM world region to SD card as one blob via Zeptoforth's FAT32 module
- [ ] Implement `image:load` — on boot, read the blob from SD back into PSRAM
- [ ] Wire `image:load` into Zeptoforth's boot sequence
- [ ] **Critical test:** Define words in Forth9 → `save` → power off → power on → words still exist
- [ ] **Restarts on error (Lisp machine style):**
  - On error, preserve stacks instead of hard `ABORT`
  - Display the faulting word, stack contents, and return stack trace
  - Offer numbered recovery options: `1) retry  2) drop-and-continue  3) abort`
  - User can fix the word and retry without losing context
- [ ] **Source location tracking:**
  - When compiling from a named buffer, record `( buffer-name line# )` in dictionary entry
  - `where mesh:send` shows `mesh.fs:42`
- [ ] **Doc strings:**
  - `doc" sends a packet" : mesh:send ... ;` attaches a doc string to the word
  - `help mesh:send` prints doc string + stack effect + "see also" references
  - Doc strings stored in image alongside definitions

**Milestone:** The persistent world works. Power cycle preserves your dictionary and data. Errors don't lose your context. Every word knows where it came from and what it does.

---

## Phase 3: Prefix Listener

_Goal: RetroForth-style prefix dispatch for the Forth9 world._

- [ ] Hook into Zeptoforth's outer interpreter (the outer interpreter hook Travis confirmed exists)
- [ ] Write the prefix dispatch table: first character → handler word
- [ ] Implement core prefixes:
  - `#` — numeric literal (parse rest as number, push)
  - `$` — character literal (push ASCII value)
  - `&` — pointer to word (push execution token)
  - `'` — word lookup (find in dictionary, push xt)
- [ ] Implement Forth9 extended prefixes:
  - `/` — path literal (push as string, no quotes needed)
  - `@` — peer address (expand to mesh peer reference)
  - `~` — file reference (named buffer in the image)
- [ ] Make unrecognised tokens fall through to Forth9 dictionary → Zeptoforth dictionary
- [ ] Test: `#42 .` prints 42, `~memo.txt` references a named buffer, `@alice` references a peer

**Milestone:** The system feels like Forth9, not raw Zeptoforth. Prefixes work. Keystrokes are minimised.

---

## Phase 4: Named Buffers (Files in the Image)

_Goal: Create, read, write, and list named data buffers inside the PSRAM image._

- [ ] Design named buffer format: name (counted string) + version counter + properties + size + data + next pointer
- [ ] Implement `touch ( "name" -- )` — create an empty named buffer
- [ ] Implement `ls` — list all named buffers (one entry per file, not per version)
- [ ] Implement `read ( "name" -- )` — display buffer contents (latest version)
- [ ] Implement `cat ( "name1" "name2" -- )` — concatenate buffers
- [ ] Implement `rm ( "name" -- )` — delete a named buffer
- [ ] Implement `cp ( "src" "dst" -- )` — copy a buffer
- [ ] Implement `mv ( "old" "new" -- )` — rename a buffer
- [ ] Implement `write ( addr len "name" -- )` — write data into a named buffer
- [ ] Implement `export ( "name" -- )` — copy a named buffer to a FAT32 file on SD (via Zeptoforth)
- [ ] Implement `import ( "filename" -- )` — copy a FAT32 file from SD into a named buffer
- [ ] All buffer operations work on the PSRAM image — no SD card I/O during normal use
- [ ] **Automatic versioning (Lisp machine style):**
  - Every `w` (write/save) auto-increments the version number — no manual branching needed
  - `~memo.txt read` shows latest version
  - `~memo.txt #2 read` shows version 2
  - `~memo.txt versions` lists all versions
  - `~memo.txt #1 restore` rolls back to version 1
  - Configurable retention: `3 ~memo.txt keep` prunes to last 3 versions
- [ ] **File properties (Lisp machine style):**
  - Key-value metadata per file: `~mesh.fs "status" "draft" prop!`
  - `~mesh.fs props` shows all properties
  - Properties stored alongside file data in the image, not as separate files

**Milestone:** You have a working filesystem inside the image with automatic versioning and metadata. `ls`, `cat`, `read`, `touch`, `rm` all work. Files survive power cycles via `save`.

---

## Phase 5: Quotations and Combinators

_Goal: `[ ]` as first-class anonymous functions in the Forth9 world._

- [ ] Design the quotation compiler: `[` starts an anonymous definition in the Forth9 dictionary, `]` ends it and pushes the xt
- [ ] Handle nesting: `[ [ dup * ] map ]` must work
- [ ] Quotations compile to native code in PSRAM (saveable, persistent, full speed)
- [ ] Implement `call` / `execute` for quotations
- [ ] Implement `dip`, `bi`, `tri`, `each`, `map`, `filter` — combinators that take quotations
- [ ] Test: `[ dup * ] map`, `[ . ] each`, etc.

**Milestone:** Quotations work. You can pass logic as values. The language has functional power.

---

## Phase 6: PicoCalc Integration

_Goal: Run Forth9 on the PicoCalc screen and keyboard._

- [ ] PicoCalc arrives — assemble with Pimoroni Pico Plus 2W
- [ ] Load Zeptoforth's PicoCalc display driver and terminal emulator
- [ ] Load Zeptoforth's keyboard driver
- [ ] Verify Zeptoforth REPL runs on PicoCalc screen+keyboard
- [ ] Wire Forth9's persistent world to run on top — same prompt, same prefixes, but on device
- [ ] Implement golden ratio screen split: 320x198 canvas + 320x122 prompt
- [ ] Implement `gfx:pixel`, `gfx:clear`, `gfx:text` — drawing words that write to canvas region
- [ ] Test: type at the keyboard, see Forth9 output on screen, define words, `save`
- [ ] **Activity switching (Lisp machine style):**
  - Define activities: prompt/listener, editor, file browser, radio monitor
  - Each activity saves its state (cursor position, scroll offset, buffer pointer) in a struct
  - `Fn+1/2/3/4` switches instantly — saves current activity, restores target, redraws
  - Can use Zeptoforth's preemptive multitasking for background tasks (radio listener, timers)
  - Activity switching is UI-level — swap which activity draws to screen and handles input
  - State preserved: switch away from editor and back, cursor is exactly where you left it
- [ ] **Describe everything (Lisp machine style):**
  - `describe` word dispatches on type
  - For a word: name, stack effect, source location, doc string, `see` disassembly
  - For a number: decimal, hex, binary
  - For a file: size, version count, properties
  - `describe` output fits in 5-8 lines on the prompt area
- [ ] **Presentation-typed output (Lisp machine style):**
  - When words print output, track type and value of each chunk (output record buffer)
  - Map screen regions (line, start-col, end-col) to (type, value) pairs
  - Tapping a printed filename offers: `read`, `ed`, `rm`
  - Tapping a printed word name offers: `see`, `describe`, `help`
  - Tapping a printed number offers: hex view, binary view, describe
  - Input context: when a command expects a filename, only filename presentations highlight

**Milestone:** The PicoCalc is a self-contained Forth9 computer with a live, interactive screen. No serial cable needed. Everything on screen is tappable and introspectable.

---

## Phase 7: Text Editors

_Goal: Edit named buffers on the device._

- [ ] Implement `ed` — line-number command editor (see implementation-ideas.md)
  - Buffer displayed on canvas, commands at prompt
  - `3 p`, `3 "old" "new" s`, `5 d`, `7 i"text"`, `w`, `q`
- [ ] Implement `nano` — simple screen editor for the canvas area
  - Cursor movement, insert, delete, save
- [ ] Both editors operate on named buffers in the image, not SD files
- [ ] `~memo.txt ed` opens a named buffer for editing

**Milestone:** You can create and edit text on the device. Self-hosted development is possible.

---

## Phase 8: Device Layer

_Goal: Named streams for all hardware, accessed through Zeptoforth's drivers._

- [ ] Design the `/dev/` abstraction: each device wraps a Zeptoforth driver with a named stream interface
- [ ] Implement `/dev/key` — keyboard input (wraps Zeptoforth's keyboard driver)
- [ ] Implement `/dev/gfx` — display output (wraps Zeptoforth's display driver)
- [ ] Implement `/dev/sd` — SD card (wraps Zeptoforth's FAT32 module)
- [ ] Implement `/dev/wifi` — WiFi (wraps Zeptoforth's WiFi/TCP stack)
- [ ] Implement `/dev/lora` (placeholder until Phase 9)
- [ ] Implement `ls /dev/` — list all registered devices
- [ ] Implement the `>` pipe operator for redirecting to devices/files

**Milestone:** Everything looks like a named stream. Hardware access is uniform.

---

## Phase 9: LoRa Radio Driver

_Goal: Talk to the SX1262 module via Zeptoforth's SPI._

- [ ] Wire SX1262 to PicoCalc GPIOs (SPI: MOSI, MISO, SCK, CS + BUSY, DIO1, RESET)
- [ ] Write SX1262 driver in Forth using Zeptoforth's SPI words
- [ ] Implement radio init: frequency, bandwidth, spreading factor, TX power
- [ ] Implement `lora:send ( addr len -- )` — transmit a packet
- [ ] Implement `lora:recv ( -- addr len )` — receive a packet
- [ ] Wire to `/dev/lora` stream
- [ ] Test: two PicoCalcs, send a message from one, receive on the other

**Milestone:** Two Forth9 devices talking over LoRa. The mesh backbone exists.

---

## Phase 10: Mesh Protocol

_Goal: Multi-hop routing, peer discovery, and gossip._

- [ ] Implement peer discovery — broadcast presence, maintain peer table
- [ ] Implement packet format: source key, destination key, hop count, TTL, payload
- [ ] Implement routing — relay packets, decrement TTL, duplicate detection
- [ ] Implement `send ( msg peer -- )` — encrypt and transmit to a peer
- [ ] Implement `@alice send` — use peer prefix for addressing
- [ ] Implement `/net/ ls` — list known mesh peers
- [ ] Implement WiFi gossip — sync bulk data over WiFi when in proximity (using Zeptoforth's TCP stack)
- [ ] Implement Bluetooth key exchange — pair in person, establish trust
- [ ] Test multi-hop: device A → device B → device C

**Milestone:** The mesh works. Messages route. Devices sync. No infrastructure needed.

---

## Phase 11: Encryption and Security

_Goal: Sovereign security — you control who sees what._

- [ ] Use Zeptoforth's SHA-256 (hardware-accelerated on RP2350)
- [ ] Implement Ed25519 keypair generation (RP2350 hardware TRNG for entropy)
- [ ] Implement `my:id` — display public key
- [ ] Implement Bluetooth key exchange — share public keys in person
- [ ] Implement per-peer encryption on all mesh traffic (AES-128-CTR + HMAC)
- [ ] Implement image encryption at rest (AES-256, passphrase-derived key)
- [ ] Implement sandbox vocabulary for incoming mesh quotations — restricted dictionary
- [ ] Test: encrypted send/receive, sandbox escape attempts fail

**Milestone:** Encrypted at rest, encrypted in transit, sandboxed on execution.

---

## Phase 12: Package Manager

_Goal: Share Forth9 code with others._

- [ ] Design package format: named module + dependency list + version
- [ ] Build CLI tool (Node.js) for publishing and downloading packages
- [ ] Build on-device `pkg:install` and `pkg:list` words
- [ ] Set up a simple registry (GitHub-based or custom)
- [ ] Publish first packages: crypto library, mesh tools, editor extensions
- [ ] Test: install a package on a fresh Forth9 image, verify it works

**Milestone:** Forth has a package manager. Code flows between developers. The community can grow.

---

## Phase 13: Radio Add-ons

_Goal: Full radio coverage from Bluetooth to HF._

- [ ] **CC1101** (~$3, sub-GHz) — capture, decode, replay RF signals. Flipper-class.
- [ ] **uSDX/QDX** (HF transceiver) — FT8, JS8Call, WSPR. International contact.
- [ ] **SA818** (VHF) — APRS, packet radio. Regional.
- [ ] **Si4732** (SDR receiver) — AM/FM/SW scanning.
- [ ] All drivers written in Forth using Zeptoforth's SPI/I2C/UART
- [ ] All wired to `/dev/` streams
- [ ] `hf:waterfall` — FFT display on canvas

**Milestone:** Every radio band from Bluetooth (10m) to HF (global). Zero infrastructure.

---

## Phase 14: Event Sourcing

_Goal: Append-only event log alongside image snapshots._

- [ ] Design event format: timestamp, event type, payload
- [ ] Implement `event:log` — append events to a named buffer in the image
- [ ] Log all inputs: keystrokes, mesh packets, timer ticks
- [ ] Implement `event:replay ( log -- )` — deterministic replay from a log
- [ ] Implement `event:diff ( peer -- )` — calculate missing events vs a peer
- [ ] Implement `event:pull ( peer -- )` — sync missing events from a peer
- [ ] Implement `event:prune` — delete events older than last save
- [ ] Test deterministic replay: same log → same state on two devices

**Milestone:** Full audit trail. Deterministic sync. The Urbit lesson applied.

---

## Phase 15: Tools and Applications

_Goal: Build the dictionary of useful words._

- [ ] **Calculator** — convenience words on top of the REPL
- [ ] **Oscilloscope** — ADC sampling, waveform on canvas
- [ ] **Multimeter** — voltage/current/resistance via ADC
- [ ] **Guitar tuner** — frequency detection from audio input
- [ ] **Synthesiser** — wavetable oscillators, DAC output
- [ ] **Spectrum analyser** — FFT on ADC samples
- [ ] **The Tower** — interactive fiction tutorial (see implementation-ideas.md)
- [ ] **Rogue9** — roguelike game
- [ ] **Chess** — algebraic notation as Forth words, mesh play over LoRa
- [ ] **HTTP server** — the "Ryan Dahl demo" — web server in 3 lines on a $5 chip

**Milestone:** The device is genuinely useful for daily work, music, electronics, and play.

---

## Phase 16: Polish and Documentation

_Goal: Make it ready for other humans._

- [ ] Write a quick reference card (one page, fits in the PicoCalc case)
- [ ] Write `help` word — on-device documentation
- [ ] Write the Forth9 manual — language, devices, mesh, image management
- [ ] Create a "first 10 minutes" tutorial
- [ ] Create a standard base image — the starting point for new users
- [ ] Kelvin-version the kernel — assign version number, begin counting down
- [ ] Publish the blog post

**Milestone:** Someone else can pick up a PicoCalc and start using Forth9 without you in the room.

---

## Phase 17: Custom Hardware (Future)

_Goal: Move beyond the PicoCalc to a purpose-built Forth9 device._

- [ ] Design a custom PCB: RP2350 + PSRAM + SX1262 + display + keyboard + battery
- [ ] Evaluate ESP32-P4 for a premium variant (400MHz RISC-V, camera, video codec)
- [ ] Investigate RISC-V custom silicon (longer term — sovereign instruction set)
- [ ] Small-batch manufacturing run
- [ ] Build the community / business around the device

**Milestone:** Forth9 is a product, not a project.

---

## Development Principles

1. **Zeptoforth is plumbing.** It handles hardware, native code compilation, and preemptive multitasking. You don't modify it, you build on top. You benefit from Travis's updates.
2. **Your world is sovereign.** The Forth9 dictionary, data, and state live in PSRAM. You own every word.
3. **Native speed.** No threaded code, no interpreter, no GC. Zeptoforth's code generator compiles directly to ARM Thumb-2. PSRAM is at a fixed address so saved native code needs no relocation.
4. **Image-first.** Everything lives in the image. SD card is for `save`/`load` only. Export to FAT32 when sharing with a laptop.
5. **One thing at a time.** Each phase has a clear milestone. Don't start the next until the current one works.
6. **Keep it small.** If a component exceeds ~300 lines of Forth, it's too complex. Factor it.
7. **Test on hardware.** The serial cable and zeptocom.js are your friends until Phase 6.
8. **Two paths.** Run Zeptoforth on one Pico for learning, build Forth9 on the other. They feed each other.

---

## Starting Point

You have Zeptoforth running on a Pico 2W. Phase 0 is mostly done. Phase 1 is learning Zeptoforth's internals well enough to know where your world begins. The PicoCalc is in transit — you can do Phases 1-5 over serial before it arrives.

Start here: study Zeptoforth's outer interpreter and hook system. That's the seam where Forth9 attaches.
