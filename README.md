# Forth9 — The Sovereign Handheld

**A declaration of independence for the handheld.**

Forth9 rejects the "App Store" model of consumption in favour of a "Live Dictionary" model of creation. It combines the [Plan 9](https://plan9.io/plan9/) file-philosophy with a native Forth dialect inspired by [RetroForth 12](http://retroforth.org/)'s functional elegance to build a computer that a human can actually understand in its entirety.

---

## Hardware Platform

| Component | Role |
|---|---|
| **[ClockworkPi PicoCalc](https://www.clockworkpi.com/)** | Case, keyboard, screen |
| **[Pimoroni Pico Plus 2](https://shop.pimoroni.com/products/pimoroni-pico-plus-2)** | RP2350 (ARM Cortex-M33 / RISC-V), 8MB PSRAM, 16MB flash — drop-in replacement for the standard Pico |
| **SX1262 LoRa module** | Long-range mesh radio, connected via SPI on exposed GPIOs (GP2–GP5) |
| **CYW43439 (built into Pico Plus 2)** | WiFi + Bluetooth — proximity gossip sync and intimate data exchange |
| **microSD card** | Image persistence and source file storage |

**Radio Add-on Modules** (optional, SPI/audio interface via exposed GPIOs):

| Module | Role |
|---|---|
| **uSDX / QDX** | HF transceiver — international contact via ionospheric skip, digital modes (JS8Call, FT8, WSPR), 5W, all-band 3-30 MHz |
| **SA818 / DRA818V** | VHF transceiver — 144 MHz, APRS, packet radio, local repeater access |
| **CC1101 module** | Sub-GHz general-purpose RF — 300-928 MHz, OOK/ASK/FSK, protocol analysis, decode/replay |
| **Si4732 module** | SDR receiver — AM/FM/SW broadcast, all-band scanning, SSB ham receive |

---

## The Manifesto

### I. The Device is an Extension of the Mind, Not a Portal for the Corporation

Current mobile devices are glass slabs of surveillance. Forth9 transforms the PicoCalc into a **Primary Tool**.

- **The Living Image** — The OS is not "installed." It is a single, persistent memory image — a snapshot of your entire system state. To change the OS, you simply think (code) a new definition.
- **No Binary Bloat** — Every tool is a human-readable Forth word. If you cannot see how a tool works, it does not belong on your device.

### II. Everything is a Stream (The Plan 9 Protocol)

We reject the complexity of modern APIs. In Forth9, the world is unified by **Named Streams**.

- **Hardware as File:** `/dev/lora`, `/dev/wifi`, `/dev/bt`, `/dev/gfx`, `/dev/cam`, `/dev/audio`, `/dev/key`.
- **Networking as Path:** Accessing a friend's device over the mesh is as simple as `"/net/alice" use`.
- **Uniformity:** If you can pipe text to the screen, you can pipe it to the radio. The interface is the logic.

### III. Functional Sovereignty

We utilise the concatenative/functional power of Forth to eliminate the "Garbage Collection" tax.

- **The Self-Cleaning Stack** — We do not manage memory; we manage flow.
- **Quotations as Law** — Logic is passed as `[ blocks ]`. This allows for hot-reloading: change the behaviour of your mesh-radio or your synth-engine while it is running, without a reboot.

### IV. The Great Mesh (Replacing Mobile)

The "Cell Tower" is a leash. Forth9 utilises radios at every range — from 10 metres to the other side of the planet — to build a peer-to-peer reality with zero infrastructure.

- **Built-in Radios (Core):**
  - **LoRa** — Long range (kilometres), low bandwidth. Always-on backbone for real-time commands, short messages, and Forth quotations.
  - **WiFi** — Medium range (50m), high bandwidth. Proximity gossip — when two devices discover each other, they sync bulk data: files, journal entries, code, images, video.
  - **Bluetooth** — Close range (10m), intimate. Key exchange, trust establishment, direct file transfer between people who physically meet.

- **Radio Add-on Modules (Expand Your Reach):**
  - **HF Transceiver** (uSDX / QDX) — Ionospheric skip, 3-30 MHz. International contact with no infrastructure on Earth. Digital modes (JS8Call, FT8, WSPR) decoded and generated in Forth. Keyboard-to-keyboard chat across continents on 5 watts and a wire antenna.
  - **VHF Transceiver** (SA818 / DRA818V) — 144 MHz ham band. APRS, packet radio, local repeater access.
  - **Sub-GHz Transceiver** (CC1101) — 300-928 MHz general-purpose RF. OOK/ASK/FSK modulation. Decode weather stations, TPMS sensors, remote controls, pagers (POCSAG), and any simple RF protocol. Raw capture and replay. Protocol analysis at the REPL.
  - **SDR Receiver** (Si4732) — AM/FM/SW broadcast reception. All-band scanning. SSB for ham receive.

- **Gossip Propagation** — Data spreads like a rumour. You carry posts, code, and files in your image. When you encounter another device — at a cafe, on a train, walking past — your devices sync automatically. No server, no internet. Human movement is the transport layer.
- **Radio Freedom** — We advocate for the expansion of sub-GHz bands for data. Every Forth9 user is a node, a router, and a server.
- **The Protocol is the Language** — We do not send JSON packets; we send Forth Quotations. To talk to another device is to offer it code to execute (within a sandboxed vocabulary for security).
- **The Lineage** — Chuck Moore invented Forth to control radio telescopes. Forth9 puts it in a pocket to control radio networks — from a LoRa mesh to an HF transceiver bouncing signals off the ionosphere. Same language, same directness, every range from 10 metres to 10,000 kilometres.

### V. Design Principles for the Forth9 Architect

1. **Minimise the Distance Between Thought and Expression** — Every keystroke between having an idea and seeing it run is friction. Parentheses are friction. Boilerplate is friction. Named parameters you must invent names for are friction. The language must disappear — you think `square this`, your fingers type `dup *`, and it's done.
2. **Be Succinct** — A word should do one thing and do it perfectly.
3. **Stay Persistent** — If a change is good, it belongs in the image.
4. **Encourage Play** — Graphics and music generation are not "apps" — they are the primary way we test the health of our streams.
5. **Trust the Stack** — The stack is the only truth.

---

## The Vision

We are building a historical artifact from the future. A device that doesn't ping a server to ask permission to exist.

### One Device, One Dictionary

None of these are "apps." They are words in your dictionary — present simultaneously, composable, and built incrementally at the REPL:

- **Oscilloscope** — ADC sampling via interrupt handler, waveform plotted to screen. Trigger, timebase, and voltage scaling are each a word.
- **Multimeter** — ADC reads voltage, current (with a shunt resistor), resistance, continuity. Results displayed or piped to a log.
- **Guitar Tuner** — Microphone or jack input, autocorrelation frequency detection, note name and cents display.
- **Calculator** — The REPL *is* the calculator. Forth is RPN. `#355 #113 /` — you already have one.
- **Synthesiser** — Wavetable oscillators driven sample-by-sample from an interrupt handler. Redefine the waveform while it plays.
- **Game Machine** — Sprite blitting, input polling, frame timing. Write a game in 100 words.

**Included Games:**

- **The Tower** (Interactive Fiction) — The system tutorial. You wake in a room with a terminal. Each room teaches one Forth concept — the stack, defining words, stack manipulation, conditionals, loops, strings, the dictionary, prefixes. The player solves puzzles by writing Forth. By the end, they can define words, inspect the dictionary, and use the device. They've become a programmer without noticing. Ships as the default first-boot image.
- **Rogue9** (Roguelike) — Every entity is a word. Dungeons are procedurally generated. Inspect any monster with `see`, redefine any item, reshape the game while playing. Share dungeon floors over mesh — brutal floors propagate between devices as players encounter each other.
- **Chess** — Standard algebraic notation at the Forth prompt. `e2 e4`, `Nf3`, `O-O`. Play over mesh: `@alice e2 e4`. Chess words load into scope with `chess:start` and unload with `chess:stop` to avoid dictionary clashes.
- **Notebook** — Text editor writing to SD card files. Your journal, your ideas, your code drafts.
- **LoRa Mesh Terminal** — Send and receive messages, relay packets, monitor network health.
- **Spectrum Analyser** — FFT on ADC samples, frequency bins plotted as bars.
- **Signal Generator** — DAC output of sine, square, triangle, sawtooth at arbitrary frequencies.

**Radio (Add-on Modules):**

- **HF Digital Modes** — JS8Call (keyboard-to-keyboard chat across continents), FT8 (weak-signal contacts at -24dB SNR), WSPR (beacon — map your global reach). All DSP done in Forth on the RP2350.
- **VHF Packet Radio** — APRS position/message network, packet radio BBS. The original digital mesh, still alive.
- **Sub-GHz Protocol Analyser** — CC1101 raw RF capture, decode OOK/ASK/FSK signals from weather stations, remotes, TPMS sensors, pagers. Write a decoder at the REPL, test it live, save it to your image.
- **Broadcast Receiver** — AM/FM/shortwave reception. All-band scanning. SSB for ham monitoring.

**Audio and Visual:**

- **Audio Playback/Recording** — WAV files read from or written to SD. I2S DAC for output, ADC or I2S microphone for input. `/dev/audio` is a stream like everything else.
- **Audio Synthesis** — Wavetable oscillators driven sample-by-sample from interrupt handlers. Redefine the waveform while it plays.
- **Camera** — SPI camera (OV2640 or similar) on exposed GPIOs. Still capture and video recording to SD.
- **Video Recording** — Capture to SD for documentation, evidence, field recording. Every frame is a stream you own.
- **Procedural Graphics** — Generate visuals, animations, and data visualisations directly in Forth.

The device is not crippled by philosophy. A sovereign device should be *more* capable, not less. The line is not "creation good, consumption bad." The line is: **every capability is a stream you control.** No black-box apps, no DRM, no corporate permission. You can pipe, inspect, record, and transmit every byte.

```forth
cam:stream gfx:display                          ( live viewfinder )
cam:stream "evidence.raw" file:write             ( record to SD )
cam:stream "/net/alice" pipe                     ( stream to a mesh peer )
cam:stream [ timestamp overlay ] map gfx:display ( add timestamp and display )
hf:waterfall                                     ( spectrum display — who's transmitting? )
hf:call "CQ CQ" js8:send                        ( call the world from your pocket )
cc1101:capture [ decode-ook .bits ] each         ( capture and decode sub-GHz RF live )
```

Because every tool is just words in the same image, they compose freely. Pipe the oscilloscope into the LoRa radio and stream a waveform to a friend. Feed the signal generator into the tuner to test it. Log multimeter readings to SD while monitoring mesh traffic. Stream video while recording audio. Nothing prevents this because there are no app boundaries — only the dictionary.

---

## Forth9 Forth: The Language

### Why Forth?

As [William Edward Hahn, PhD](https://www.youtube.com/watch?v=a_6LPvtAUVE) observes, Forth is "both a high-level language and a low-level language at the same time" — an oxymoron to most programmers, but the precise quality a sovereign handheld demands. In one breath you poke a hardware register. In the next you define a high-level mesh protocol. No other language does both without a seam between them.

Forth was born controlling radio telescopes at a national observatory in the 1960s — created by a single individual, Chuck Moore, to talk to radio hardware with high-level expressiveness. Sixty years later, Forth9 puts it in a pocket to control a LoRa mesh radio. The lineage is direct. The language that was invented to command radio dishes is now commanding radio networks.

Hahn also makes a deeper observation: modern programming has disconnected us from the machine. "We're caught in API calls and very bloated libraries to do even the simplest tasks." High-level abstractions are powerful, but "we've lost touch with something." The sovereign handheld is a reconnection — Forth eliminates the layers between your thought and the machine's action. The device becomes an extension of the mind because there is no abstraction barrier.

As Hahn says: Forth will change not just how you program. "It'll change your mind."

Forth9 Forth is a new Forth dialect — a persistent world built on top of [Zeptoforth](https://github.com/tabemann/zeptoforth) with RetroForth-inspired syntax. Zeptoforth handles boot, hardware drivers, and native code compilation. Forth9 compiles native ARM Thumb-2 code into PSRAM via Zeptoforth's code generator. Since PSRAM is memory-mapped at a fixed base address on the RP2350, saved native code needs no relocation — all branch targets remain valid. No threaded code, no GC, no interpreter overhead. Full native speed with full image persistence. One language, top to bottom, no seams.

### Why Not RetroForth Directly?

RetroForth 12 runs on a virtual machine (the NGA VM, a 30-instruction interpreter). This means a C layer underneath and Forth on top — two languages. Forth9 eliminates the VM entirely. Zeptoforth compiles Forth to **native ARM machine code** on the RP2350. The result:

- **Native speed** — Forth words compile to ARM Thumb-2 instructions with constant folding and inlining. No dispatch loop, no interpretation overhead.
- **Native interrupts** — A Forth word can be a hardware interrupt handler. LoRa packet arrives, your Forth word runs immediately. Audio synthesis sample-by-sample. Cycle-accurate timing for protocols.
- **One language** — No C layer, no bridge, no second language to understand. Forth talks to hardware registers directly.
- **Preemptive multitasking** — Zeptoforth provides real preemptive multitasking with dual-core support. No GC means no GIL, no coroutine limitations.

### Why Zeptoforth?

Zeptoforth is an incredibly complete Forth for the RP2350, written entirely in Forth and ARM assembly by Travis Bemann. It provides:

- Native ARM Thumb-2 compiler with constant folding and inlining
- WiFi driver, full IPv4/IPv6 TCP/IP stack, HTTP client/server, MQTT — all in Forth
- SHA-256 (software and hardware-accelerated), CRC-32, Base64
- FAT32 filesystem, SD card driver, PSRAM block device
- PicoCalc display driver and terminal emulator
- Preemptive multitasking with dual-core support
- Module/namespace system (`begin-module` / `end-module`)

Forth9 builds on top of all this rather than reinventing it. Zeptoforth is plumbing — it handles hardware. Forth9 is the persistent world the user sees and interacts with.

### What Forth9 Adds

- **Prefix listener** — the first character of each token tells the listener how to handle it. RetroForth defines `#` for numbers, `$` for characters, `&` for pointers, `'` for word lookup. Forth9 extends this with device-specific prefixes:
  - `/` — path literal. `/dev/lora` pushes a path string with no quotes needed.
  - `@` — peer address. `@alice` expands to `/net/alice`. Reads like every messaging platform.
  - `~` — file reference. `~memo.txt` references a named buffer in the image.
- **Quotations** — `[ blocks ]` as first-class anonymous functions, compiled to native code.
- **Namespace naming** — `s:length`, `lora:send`, `mesh:route` instead of ad hoc traditional names.
- **Image persistence** — the entire PSRAM world (dictionary, variables, files, state) saves as one blob to SD.
- **Automatic file versioning** — Lisp machine-style: every save increments a version number. Roll back to any previous version.
- **File properties** — key-value metadata per file, stored in the image.
- **Error restarts** — Lisp machine-style: errors preserve stacks and offer recovery options instead of hard abort.
- **Activity switching** — instant swap between prompt, editor, file browser, radio with state preserved.
- **Presentation-typed output** — printed values are tappable on the PicoCalc touchscreen, offering context-relevant actions.
- **Describe everything** — one word to introspect any value: words, numbers, files, devices.

The prefix system is what makes Forth feel like a shell without needing a shell. Every prefix eliminates keystrokes on the small keyboard — `@alice` instead of `"/net/alice"` saves 11 keypresses including shifts. If you find yourself repeating a pattern, define a new prefix. The language adapts to your usage.

The syntax starts as RetroForth's — a proven, well-designed dialect — and evolves only when the hardware or the mesh demands it.

### Image Persistence

Everything lives in the **8MB PSRAM** as one contiguous image. The image contains everything — word definitions, variable values, user files (notes, code, data), mesh routing tables, configuration. There is no filesystem for daily use. Files are named buffers inside the image, not FAT32 entries.

```
Power on  → Zeptoforth starts from flash
            → Inits hardware (GPIO, SPI, display, WiFi)
            → image:load reads one 8MB blob from SD to PSRAM
            → Full Forth9 world restored: definitions, data, files, state, everything

save      → image:save writes PSRAM to SD as one blob
            → One write, ~1 second, one file = your entire world

export    → Copies a named buffer from the image to a FAT32 file on SD
            → For sharing with a laptop — only when needed
```

Between saves, the SD card is never touched. Zero wear, zero power wasted on I/O. At one save per day, an SD card lasts centuries.

The image captures everything — native compiled word definitions, variable values, user notes, named data buffers, configuration. Power off, power on, you're exactly where you left off. Copy the image to another PicoCalc and you've cloned your brain.

Native code in PSRAM needs no relocation because PSRAM is memory-mapped at a fixed base address on the RP2350. Save the blob, restore it to the same address, all branch and call targets are still valid. This is why image persistence works with full native speed — no threaded code, no interpreter, just compiled ARM Thumb-2 sitting at the same address it was compiled to.

Zeptoforth in flash is separate from the image. It manages hardware setup on boot, then loads the image. Hardware state is never saved — it's reconstructed by Zeptoforth each boot. The image is pure user state with no hardware entanglement.

### Syntax Examples

**Everyday use — feels like a shell, is always Forth:**

```forth
/dev/ ls                      ( List all devices )
/dev/lora cat                 ( Listen to the radio )
"hello" @alice send           ( Send a message — @ is the peer prefix )
/dev/cam capture              ( Take a photo )
#440 tone:play                ( Play a tone )
~memo.txt edit                ( Edit a file on SD — ~ is the file prefix )
/net/ ls                      ( List mesh peers )
save                          ( Save everything )
```

**Prefixes eliminate keystrokes:**

```forth
#440                          ( Number literal )
$A                            ( Character code )
&audio:freq                   ( Pointer to a word )
'mesh:send                    ( Look up a word )
/dev/lora                     ( Path literal — no quotes needed )
@alice                        ( Peer address — expands to /net/alice )
~notes.txt                    ( SD card file path )
```

**Quotations — pass logic as values:**

```forth
[ puts ] lora:each-packet     ( Print every incoming mesh message )
[ dup * ] map                 ( Square each element )
/dev/audio cat [ encrypt ] | @alice send   ( Pipe audio through encryption )
```

**Direct hardware access — no abstraction layer:**

```forth
$400140cc @ hex.              ( Read a GPIO register )
: lora-busy? $400X0010 @ #1 and ;
```

**Image persistence:**

```forth
save                          ( Snapshot your entire system to SD )
~experiment.img save-as       ( Save to a named image )
~stable.img load-image        ( Switch to a different system state )
```

**Hot-reloading — redefine live words without rebooting:**

```forth
: mesh:relay ( packet -- )
  dup seen? if drop exit then
  dup ttl@ #0 gt? [ mark-seen retransmit ] [ drop ] choose ;
```

---

## The Future of the Language

Forth9 Forth starts with RetroForth's syntax. It will evolve — not toward a different paradigm, but toward a more expressive Forth. The concatenative model and postfix syntax are permanent choices, because the constraint that drives them is permanent: a human who wants to communicate at the speed of thought, whether on a tiny keyboard or a full-size one. Every keystroke matters. Nesting and verbosity will always lose to flat, terse expression.

As hardware grows (and it will — 100x more RAM and CPU within a generation), the temptation will be to switch to a richer language like Lisp or Scheme. Resist it. More resources should mean more capability per word, not more words per idea.

### Why Not a Lisp Machine?

A Lisp machine is for someone who thinks in abstractions — trees of data, recursive transformations, symbolic reasoning. The parentheses aren't noise to a researcher; they're structure made visible. A desk, a large screen, a full keyboard, and hours of deep thought. That is a fine machine for that purpose.

A Forth machine is for someone who thinks in actions — do this, then this, then this. The stack isn't a limitation; it's how you naturally sequence intent. A pocket, a small screen, and moments of direct intervention.

Lisp is a language you build applications *in*. Forth is a language you build the machine *from*. On a pocket computer, there is no distinction between the OS, the application, and the shell — it's all one dictionary. Lisp systems accumulate layers. Forth systems stay flat by nature. One vocabulary, one namespace, one image. The person holding the device can see the bottom.

At 1000x the power, a Lisp user fills the space with deeper abstractions — taller systems. A Forth user fills it with more words — a wider dictionary of direct actions, each still simple, still one definition deep. The system gets wider, not taller. That's the right scaling philosophy for something that fits in your pocket.

Forth9 is for the person who wants to reach into their pocket, pull out a machine, and *act*.

**Why the Lisp machine is still waiting.** Nobody has shipped a Lisp machine product — not in 1990, not in 2026. The reason is structural: Lisp needs garbage collection, which needs RAM, which needs a memory management unit, which means a full processor, which means Linux is already there and "good enough." The Lisp people keep rebuilding Emacs instead of building hardware. Forth runs on a $5 chip with no GC, no MMU, no OS. The sovereign handheld is buildable TODAY in Forth. A Lisp equivalent needs a much beefier chip and still doesn't have instant-on. The Lisp machine was the right idea at the wrong price in the wrong language for the form factor.

### The Lisp Machine Revisited — Why Not Lisp on a Handheld?

The Lisp machine was the greatest personal computing environment ever built. If we are picking up its dropped thread, why not pick up its language too? The answer is not hardware constraints — those will vanish within a generation. The answer is that Lisp and Forth embody different mental models, and only one fits the handheld.

**Lisp thinks in trees.** Every expression nests inside another. To write `(mesh:send alice (encrypt key (serialize (sensor:read))))`, your mind holds a four-level tree. You plan the structure before you type, or build inside-out and wrap layers around it. Every opening parenthesis is a mental stack push. Every close is a pop. This is architectural thinking — powerful at a desk with a full screen and hours ahead of you.

**Forth thinks in sequences.** One action after another: `sensor:read serialize key encrypt alice mesh:send`. Your mind follows a flow. Each word transforms what came before. You never go deeper than one level. You never track nesting. You just take the next step. This is how you naturally think about *doing things*.

When you pull a device from your pocket, you think in actions — check the mesh, send a message, take a reading, jot a note. Sequences, not trees. Forth matches this. Lisp doesn't.

**The screen is permanent.** A handheld screen shows 15 lines. That's not a constraint that improves — it's a form factor. Lisp needs vertical space for readable nesting. A single Lisp function can consume half the screen, and scrolling breaks comprehension because the structure (indentation, nesting depth) IS the meaning. Forth is flat — three lines for the same logic, readable left to right, no structure lost when you scroll.

**The nesting tax compounds.** Every expression, every day, for years. Experienced Lispers stop seeing parentheses consciously, but the cognitive load remains. Forth has zero nesting tax — the deepest structure is a single `[ quotation ]`, one level. On a device used hundreds of times daily for a lifetime, that mental energy budget matters.

**Wide vs tall.** Lisp builds towers of abstraction — macros on functions on macros, each layer more powerful, the base farther away. Six months later you must reconstruct the tower to read your own code. Forth builds wide — a flat vocabulary where each word is one definition deep. You can always see the bottom. For a sovereign device you want to understand in its entirety, wide beats tall.

Lisp is the right language for building complex systems at a desk. Forth is the right language for living with a device in your pocket. Forth9 is a pocket relationship. The choice is not about what's more powerful. It's about what fits.

### Planned Language Extensions

These evolve the language only when real use on the device demands them:

**Terse data structures** — first-class dictionaries and sequences without verbose constructors:

```forth
{ 'alice' $4F2A 'bob' $3E1C } peers !    ( key-value pairs )
peers @ 'alice' get                        ( lookup )
```

**Pattern matching on the stack** — dispatch by shape instead of nested conditionals:

```forth
: handle ( msg -- )
  { :ping } match? [ pong ] when
  { :data } match? [ process ] when
  drop ;
```

**Lightweight optional types** — checked at compile time, zero runtime cost:

```forth
: mesh:send ( packet:mesh-pkt peer:addr -- status:err )
  ... ;
```

**Closures and partial application** — quotations that capture values:

```forth
peer [ swap mesh:send ] bind    ( returns a quotation bound to peer )
```

Each extension must pass a test: **does it reduce keystrokes per idea without adding syntax noise?** If it needs parentheses, brackets, or keywords that don't carry meaning, it doesn't belong.

---

## Why Not Linux?

Microcontrollers are becoming computers. The RP2350 is a dual-core 150MHz processor with megabytes of RAM. The ESP32-P4 is a 400MHz dual-core RISC-V with hardware video codecs and camera interfaces. Within five years, MCUs will hit 1GHz+ with hundreds of megabytes of RAM — the specs of a 2015 smartphone in a chip that costs $5 and runs for weeks on a battery.

The instinct is to put Linux on these chips. Resist it.

Linux is a 30-second boot, a kernel that's millions of lines of code nobody fully understands, and an entire philosophy of computing that assumes you have time to wait. Forth9 Forth boots in **microseconds**. Power on, the CPU starts executing from flash, your kernel loads the image from SD, and you're at the REPL. No BIOS, no bootloader chain, no init system, no kernel modules, no userspace daemons, no filesystem check. You flip the switch like a flashlight. It's on. It's yours.

Linux assumes:

- **A separation between kernel and user** — MMU, virtual memory, process isolation. An MCU doesn't have an MMU. Forth doesn't need one. There is one user and one process: you.
- **A filesystem** — ext4, inode tables, journaling, mount points. Forth9 has an image and flat files on SD. No filesystem driver, no VFS layer, no mount.
- **Dynamic linking** — shared libraries, dependency resolution, version conflicts. Forth has one dictionary. A word exists or it doesn't.
- **Preemptive multitasking** — process scheduling, context switching, priority queues. Forth uses cooperative multitasking and interrupt-driven concurrency. Simpler, predictable, no scheduler overhead.

Every one of those assumptions adds boot time, memory overhead, complexity, and code that no single human can audit.

| | Linux | RTOS (Zephyr/FreeRTOS) | Forth9 Forth |
|---|---|---|---|
| Boot time | 10-30 seconds | 100ms-1s | Microseconds + image load |
| Kernel size | Millions of lines | Tens of thousands of lines | Zeptoforth + Forth9 layer |
| Languages involved | C, shell, Python, config formats | C, CMake, YAML | Forth |
| Cross-compiler needed | Yes | Yes | No — develop on the device |
| One person understands it | No | Barely | Yes |
| Instant on | No | Nearly | Yes |

The MCU revolution doesn't need a smaller Linux. It needs no Linux.

### The Future of Handheld Computing

The word "microcontroller" is becoming meaningless. A $5 chip already does everything most people actually need — and does it on a battery that lasts all month, running code you can read, on a network you own.

The convergence is real and accelerating. In 2020, the ESP32 at 240MHz was "a WiFi chip." In 2024, the ESP32-P4 at 400MHz with MIPI camera interfaces and hardware video codecs is... what? It's not a microcontroller by any historical definition. It's a computer that costs less than lunch.

RISC-V accelerates this further. Open instruction sets, no licensing fees, no corporate control over the silicon. Small teams are already taping out custom chips. Within a decade, the Forth9 philosophy goes all the way down — a sovereign chip running a sovereign language on a sovereign network.

The phone becomes what the mainframe became: still around, still useful for some things, but no longer the default assumption about what computing is. The future pocket computer doesn't run Android. It doesn't run iOS. It doesn't run Linux. It runs a living Forth image on an instant-on chip, communicating over a mesh network with no cell tower, no cloud, and no corporate permission.

The phone is a leash you pay for monthly. The sovereign handheld is a tool you own forever.

### The Two Lost Futures

Forth9 isn't inventing something new. It's picking up the dropped threads of two timelines that were winning, then lost.

**The Lisp Machine (1979–1990).** At MIT in the late 1970s, Richard Greenblatt and Tom Knight built computers where Lisp was everything — the OS, the applications, the shell, the compiler, the debugger. Symbolics and LMI commercialised them. The Symbolics Genera operating system was the peak of personal computing:

- Single language, top to bottom. The entire OS was Lisp. You could inspect and modify any part of the running system — including the operating system itself — from the same REPL you used to write your application.
- Image persistence. Save your world, restore it later. Your entire computing state was one persistent image.
- Incremental compilation. Change one function, it recompiled just that function, live. No rebuild, no restart.
- No boundary between use and programming. Using the computer WAS programming it. Every user was a developer.
- Source-level debugging of the OS. Something crashes? You're in a debugger showing OS source. Fix it. Continue.

A Genera user in 1985 had capabilities that a macOS user in 2026 still doesn't have. These machines cost $50,000–$100,000.

**Plan 9 from Bell Labs (1987–2002).** Rob Pike, Ken Thompson, and Dennis Ritchie — the creators of Unix — looked at what Unix had become and built what it should have been:

- Everything truly is a file. Not just device nodes. The network, processes, the window system, other computers' resources — all files. One abstraction for everything.
- Per-process namespaces. Each process sees its own customised filesystem. Mount a remote machine's `/dev/` over your local one, and your programs talk to remote hardware without knowing it.
- 9P protocol. Nine messages. That's the entire network protocol. Compare that to HTTP, REST, GraphQL, gRPC — Plan 9 replaced all of them with nine messages.
- Small and auditable. The entire Plan 9 OS was smaller than the Linux kernel alone.

**Why they lost.**

The Lisp machines died because of price. A Symbolics 3600 cost $70,000. An IBM PC cost $2,000. When the AI Winter hit, the funding dried up. Symbolics went bankrupt.

Plan 9 lost to its own ancestor. Unix was already everywhere — millions of lines of software, thousands of trained administrators. Richard Gabriel called this **"worse is better."** The worse design wins because it's simpler to implement, easier to port, and good enough. Plan 9 was better. Unix was good enough. Good enough won.

Then the web made it all irrelevant. The browser became the universal platform, papering over the broken filesystem, the broken networking, the broken security model — replacing them all with HTTP and JavaScript. A worse solution to every problem, but a uniform one.

Then mobile sealed it. Apple and Google didn't want sovereign computing. They wanted walled gardens. The App Store generates revenue. The image model doesn't. The Lisp machine philosophy — where every user is a developer, where the system is open to modification — is the exact opposite of what a trillion-dollar ecosystem wants.

**What was lost.** If Lisp machines had gotten cheap and Plan 9 had replaced Unix, computing today would look like: no apps, just capabilities in a shared namespace. No app stores, just shared code. No cloud, just mounted filesystems. No distinction between using and programming. Persistent state that follows you and never resets without your consent.

That world was not hypothetical. It existed on Symbolics machines in 1985. It was working. It was better. It lost to cheaper hardware and corporate incentives.

**TempleOS (2003–2018).** While the Lisp machines and Plan 9 were institutional projects, one person proved the philosophy alone. Terry A. Davis, working by himself over a decade, built [TempleOS](https://templeos.org/) — a complete 64-bit operating system from scratch. One language (HolyC) top to bottom. Single address space, ring-0 only, no memory protection — the user and the machine with nothing between them. The entire OS is a live programming environment: edit the kernel source, recompile it, run it, without leaving the system. 100,000 lines of code, one developer, a full OS.

Davis was a troubled person, but a real systems programmer who understood what the industry had lost. TempleOS proved that a single individual can build a complete, self-hosted operating system — and that the "one language, no barriers" philosophy works in practice, not just in theory. His 22,000-line kernel is still 30x larger than Forth9's, because Forth is terser than C. But the proof of concept stands: one person, one language, one system. It's been done.

**Forth9 picks up all three threads.**

| Lost Future | Forth9 Adaptation |
|---|---|
| Lisp machine: single-language system | Forth9 Forth: one language, top to bottom |
| Lisp machine: image persistence | RAM snapshot to SD, full system state |
| Lisp machine: live modification | Hot-reload any word, no reboot |
| Lisp machine: no use/dev boundary | The REPL is the shell is the IDE |
| Plan 9: everything is a file | Named streams: `/dev/lora`, `/net/alice` |
| Plan 9: network as namespace | Mesh peers mounted as paths |
| Plan 9: 9P protocol | Forth quotations as the protocol |
| Plan 9: small, auditable OS | Zeptoforth + Forth9 layer of kernel |
| TempleOS: one person, one OS | Forth9 is built by individuals, not institutions |
| TempleOS: live programming environment | Edit, compile, run — all from the same prompt |
| TempleOS: single address space, no barriers | MCU has no MMU — one user, one machine, direct access |

The difference: a Symbolics 3600 cost $70,000. A PicoCalc costs $60. The ideas didn't fail. The price did. The price is solved.

### The Lisp Machine Lesson: No Shell, No Layers

The Lisp machines didn't have a shell. They didn't have a "friendly" language on top and a "real" language underneath. They had one language — Lisp — and it WAS the interface. Opening a file was a Lisp expression. Sending mail was a Lisp expression. There was no boundary between "using" and "programming." Every user was always in the language, whether they knew it or not.

Forth9 follows the same principle. There is no shell. There is no BASIC. There is no bash layer. **Forth IS the interface.** The prefix system and well-named vocabulary make it feel like a command line to someone who doesn't know it's a programming language:

```forth
/dev/ ls                      ( list devices — / prefix, no quotes needed )
/dev/lora cat                 ( listen to the radio )
"hello" @alice send           ( send a message — @ is the peer prefix )
/net/ ls                      ( list mesh peers )
~memo.txt edit                ( edit a file — ~ prefix for SD card )
save                          ( save everything )
words                         ( see all available commands )
'send see                     ( see how send works — you're reading source )
```

That's not a shell. That's Forth. But a new user doesn't know or care. They're typing commands and getting results. The prefixes — `/` for paths, `@` for peers, `~` for files, `#` for numbers — eliminate the quotes that would make Forth verbose. `@alice` instead of `"/net/alice"` saves 11 keypresses including shifts. On a small keyboard, that's the difference between fluid and tedious.

**The invisible gradient.** The moment a user wants more, they don't switch languages. They're already there:

```forth
/dev/ ls                                          ( using the device )
: backup /dev/cam capture ~photo.raw file:write ; ( now you're programming )
backup                                            ( now you're using your new tool )
save                                              ( now it's permanent )
```

The transition from "user" to "programmer" is invisible. There is no door to walk through. You were always in the same room. Define a word, and you've extended your device. Save, and it's permanent. This is the Lisp machine experience — one language that scales from simple commands to full system programming, with no layers between them.

If you find yourself typing the same pattern repeatedly, define a prefix or a word. The language adapts to you. No other interface model offers this — not bash, not BASIC, not any app launcher. Only a language that is simultaneously the shell, the OS, and the development environment.

---

## Building the Forth9

### What Already Exists (Provided by Zeptoforth)

| Component | Status |
|---|---|
| Native ARM Thumb-2 compiler | Zeptoforth — with constant folding and inlining |
| SD card + FAT32 filesystem | Zeptoforth — fully working |
| PSRAM driver | Zeptoforth — memory-mapped access |
| WiFi + TCP/IP + HTTP | Zeptoforth — full stack, all in Forth |
| SHA-256 (hardware accelerated) | Zeptoforth — ready to use |
| PicoCalc display + keyboard | Zeptoforth — drivers and terminal emulator |
| Preemptive multitasking | Zeptoforth — dual-core support |
| PicoCalc hardware | Exists — ships ready |
| Pimoroni Pico Plus 2W (8MB PSRAM) | Exists — drop-in for PicoCalc |
| SX1262 LoRa drivers | Reference exists (RadioLib) — Forth9 driver written in Forth against SPI registers |

### What Forth9 Builds On Top

| Component | Description |
|---|---|
| **Persistent world** | Own dictionary in PSRAM, image save/load, boot sequence integration |
| **Prefix listener** | Hook into Zeptoforth's outer interpreter, RetroForth-style prefix dispatch |
| **Quotation compiler** | `[ ]` support — anonymous native-compiled blocks as first-class values |
| **Named buffers** | In-image filesystem with auto-versioning, properties, `ls`/`cat`/`read`/`touch`/`rm` |
| **Error restarts** | Lisp machine-style error recovery — preserve stacks, offer numbered options |
| **Activity switching** | Instant swap between prompt/editor/browser/radio with state preserved |
| **Presentations** | Typed output — tappable values on screen with context-relevant actions |
| **Plan 9 device layer** | Named streams wrapping Zeptoforth's hardware drivers |
| **Mesh protocol** | Peer discovery, routing, quotation transport over LoRa |
| **Encryption layer** | Using Zeptoforth's SHA-256 + RP2350 hardware AES |

### Build Order

1. **Learn Zeptoforth internals** — Outer interpreter, hooks, memory layout, module system. Know the boundary.
2. **Persistent world** — Own dictionary in PSRAM, native code via Zeptoforth's compiler, image save/load via Zeptoforth's FAT32.
3. **Prefix listener** — Hook into Zeptoforth's outer interpreter. `#`, `$`, `&`, `'`, `/`, `@`, `~` prefix dispatch.
4. **Named buffers** — In-image filesystem with auto-versioning and properties.
5. **Quotations** — `[ ]` as anonymous native-compiled definitions. Nesting, passing, executing.
6. **PicoCalc integration** — Wire Forth9 world to Zeptoforth's display and keyboard drivers. Activity switching.
7. **Text editors** — `ed` and `nano` operating on named buffers.
8. **Device layer** — `/dev/` named streams wrapping Zeptoforth's drivers.
9. **LoRa driver** — SX1262 via Zeptoforth's SPI words.
10. **Mesh protocol** — Peer discovery, routing, encrypted quotation transport.
11. **Bootstrap on-device** — Once the console works, all further development happens on the PicoCalc itself.

### What We Learn from Urbit and Hoon

Urbit is the most ambitious attempt at sovereign computing since the Lisp machines. Its language, Hoon, is described by its creators as "more Lisp-y than Lisp" — a statically typed functional language compiling to Nock, a frozen combinator VM with the simplicity of a mathematical axiom. The [Hoon for Lispers](https://urbit.org/blog/hoon-4-lispers) essay lays out the philosophy: no opaque data, no mutable state, no side effects, everything is an inspectable binary tree.

Forth9 takes Urbit's best architectural ideas seriously while rejecting its language design for the handheld.

**What Hoon gets right that Forth9 adopts:**

- **Subject-oriented scoping.** In Hoon, every expression evaluates against a "subject" — a single tree that IS the scope and state. Narrow the subject and you restrict what code can access. This is Forth9's sandboxing model for mesh code: evaluate incoming quotations against a restricted dictionary. The word can only see and call what you expose. Hoon validates this approach architecturally.
- **No opaque data.** Hoon's precept: *"There are no magic binary blobs or opaque data types."* Every value is inspectable. Forth9 adopts this — every value on the stack should be printable and walkable, not just executable.
- **Fixed arity eliminates delimiters.** Hoon's runes have a fixed number of children, so expressions self-terminate without closing parentheses. Forth arrived at the same solution from the opposite direction — postfix with known stack effects means no grouping syntax. Same insight, different encoding.
- **Frozen kernel.** Nock is frozen forever. Hoon evolves toward freezing. Forth9's Kelvin versioning is this principle applied to the Forth kernel.

**What Hoon gets wrong for a handheld:**

- **100+ symbolic runes.** Hoon replaces parentheses with over 100 two-character symbols (`?:`, `%+`, `|=`, `^-`, `?~`, `=>`). This trades one cognitive load (tracking nesting) for another (memorising a symbolic vocabulary larger than most languages' keyword sets). On a handheld where you act at the speed of thought, consulting a rune table is worse than counting parentheses. Forth words are English: `dup` means duplicate, `drop` means drop. No lookup needed.
- **Static types block exploration.** Hoon's `nest-fail` stops you when types don't match — valuable for large systems, friction on a REPL where you're poking registers and testing ideas. Forth's untyped stack lets you experiment freely. The cost is silent errors. The benefit is zero friction during interactive exploration. For a pocket device, exploration speed beats compile-time safety.
- **No side effects.** Hoon is purely functional — I/O is managed through the OS as effects. Correct for a networked server. Wrong for a device that exists to DO things: toggle a GPIO, fire a radio, write a sample to a DAC. Side effects aren't a problem on a handheld. They're the purpose.
- **Binary tree navigation.** Hoon builds everything from binary cells. Accessing the third element means navigating a tree path. In Forth, it's a memory offset. Flat memory beats tree navigation for directness.

**The deepest lesson.** Urbit precept A.13 states: *"If you don't completely understand your code and the semantics of all the code it depends on, your code is wrong."* That is Forth9's manifesto too. But Hoon requires understanding 100+ runes, a binary tree addressing system, a static type system with molds and vases, wet vs dry gates, and subject-oriented scoping — before you write your first useful program. Forth requires understanding: the stack, the dictionary, and the word you're about to call. Precept A.13 is achievable in Forth. In Hoon, it's aspirational.

Hoon is Lisp perfected for servers. Forth is the language perfected for hands.

**Forth9 borrows four specific ideas from Urbit's architecture:**

**Event Sourcing** — An append-only event log on SD runs alongside image snapshots. Every input — keystroke, mesh packet, timer tick — is recorded. The image is the fast-boot checkpoint; the event log is the history, the audit trail, and the sync mechanism. Gossip between peers works by exchanging event diffs, not opaque blobs.

```forth
save                          ( fast snapshot for boot )
event:log                     ( append-only journal on SD )
peer:alice event:diff         ( what events does Alice have that I don't? )
peer:alice event:pull         ( get them )
```

**Cryptographic Identity** — Each device generates a keypair at first boot. The public key IS your identity. No blockchain, no server, no registration. Verified in person via Bluetooth key exchange.

```forth
my:id                         ( your public key )
"alice" my:name               ( map a human name to your key )
```

**Kelvin Versioning** — The kernel version counts DOWN toward zero. Version 100, 99, 98... Once the Forth9 Forth kernel reaches 0, it is frozen forever. Only user-space words evolve. The foundation becomes permanent.

**Deterministic Replay** — Same events produce the same state. Two devices that have replayed the same event log are in the same state, verifiable by comparing hashes. This makes sync provable and debugging reproducible.

### Security Model

- **Image encryption at rest** — AES-256 via the RP2350's hardware accelerator. Boot passphrase derives the key.
- **Mesh packet encryption** — Per-peer pre-shared keys. AES-128-CTR + HMAC on every LoRa packet.
- **Sandboxed execution** — Incoming mesh quotations run in a restricted vocabulary. No `save`, no raw register writes, no dictionary modification unless explicitly trusted.
- **Secure boot** (production) — RP2350 eFuse-based firmware signing. Irreversible; save for final hardware.

### Key Resources

- **Zeptoforth** — <https://github.com/tabemann/zeptoforth> — The native ARM Forth that Forth9 builds on top of.
- **RetroForth 12** — <http://retroforth.org/> — Syntax and semantics reference.
- **ClockworkPi PicoCalc** — <https://www.clockworkpi.com/> — The handheld platform.
- **Pimoroni Pico Plus 2W** — <https://shop.pimoroni.com/products/pimoroni-pico-plus-2w> — RP2350 + 8MB PSRAM + WiFi/BT.
- **RP2350 Datasheet** — <https://datasheets.raspberrypi.com/rp2350/rp2350-datasheet.pdf> — Register-level hardware reference.
- **RadioLib** — <https://github.com/jgromes/RadioLib> — SX1262 reference (or write your own driver in Forth).
- **zeptocom.js** — Zeptoforth web serial terminal with handshaking for reliable code upload.

---

By following this path, you move from a consumer of a device to the architect of its reality.
