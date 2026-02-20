# Forth9 Research Notes

References, design comparisons, and lessons from other projects.

---

## DuskOS

**Source:** <https://duskos.org/> | **License:** CC0 (public domain)

A 32-bit Forth OS that boots bare metal on i386, amd64, ARM, RISC-V, and m68k. Built by one person. The entire system — OS, C compiler, Oberon compiler, Lisp, assemblers, text editor — is under 6,000 lines of Forth.

### Architecture

| Component | Size |
|---|---|
| i386 kernel | ~10KB |
| amd64 kernel | ~9KB |
| ARM kernel | ~14KB |
| RISC-V kernel | ~14KB |
| Boot payload (minus HAL) | ~44KB |
| "Almost C" compiler | ~38KB |
| Oberon compiler | ~49KB |
| Lisp | ~16KB |
| Text editor | ~16KB |

### Threading Model: STC vs Mecrisp's Native Code

DuskOS uses **Subroutine Threaded Code (STC)**. Mecrisp Stellaris uses **native code generation**. This is the most important architectural difference.

**Subroutine Threaded Code (DuskOS):**

```
: square  dup * ;

Compiles to:
  CALL dup
  CALL *
  RET
```

- Every word is a `CALL` to another word's address, ending with `RET`.
- Simple to implement — the compiler just emits CALL instructions.
- Portable — the only architecture-specific part is the CALL/RET encoding.
- Overhead: every word invocation is a subroutine call (push return address, jump, pop, return). On ARM this is `BL` + `BX LR`.
- Performance: ~70-80% of native C speed. Good enough for most things.
- Code density: moderate — each word reference is a 4-byte CALL instruction.

**Native Code Generation (Mecrisp Stellaris):**

```
: square  dup * ;

Compiles to (ARM Thumb-2, approximate):
  PUSH {r0}        ; dup — push TOS, keep copy in r0
  MULS r0, r0, r0  ; * — multiply TOS by itself
  BX LR             ; return
```

- Mecrisp inlines primitives. `dup` doesn't generate a CALL to a `dup` subroutine — it emits the actual machine instructions that duplicate the top of stack.
- The compiler knows the target architecture intimately. It uses ARM's register file, condition codes, and addressing modes.
- Performance: near-C speed. Primitives have zero call overhead.
- Complexity: the compiler is much harder to write. Mecrisp's compiler is architecture-specific — the ARM version can't run on RISC-V without a rewrite.
- Code density: better for small words (no CALL overhead), worse for large words (same as STC).

### Comparison for Forth9

| | STC (DuskOS) | Native (Mecrisp) |
|---|---|---|
| Speed | Good (~70-80% of C) | Excellent (~95% of C) |
| Interrupt latency | Moderate — CALL chain | Low — inlined primitives |
| Compiler complexity | Simple | Complex |
| Portability | Easy — change CALL encoding | Hard — rewrite per arch |
| Code size | Moderate | Smaller for primitives |
| Debugging | Easy — `see` shows CALL addresses | Harder — `see` shows machine code |
| Self-hosting | Easier to bootstrap | Harder to bootstrap |

**Why Forth9 uses native code (Mecrisp):**

1. **Interrupt handlers must be fast.** A LoRa packet ISR or audio sample ISR can't afford CALL chain overhead. Native-compiled Forth words as ISRs have near-zero latency.
2. **DSP.** FT8 decoding, FFT, audio synthesis — tight inner loops benefit from inlined primitives.
3. **Mecrisp already exists for RP2350.** Writing a native code compiler from scratch is hard. Matthias Koch already did it. Forking is easier than building.

**Why you might reconsider STC later:**

1. **Portability.** When Forth9 moves to ESP32-P4 (RISC-V) or custom silicon, the Mecrisp compiler must be rewritten. An STC compiler ports in days.
2. **Self-hosting on device.** STC is simpler to bootstrap — the on-device compiler is smaller and easier to modify from the REPL.
3. **Debugging.** STC disassembly is human-readable (just CALL addresses you can look up). Native disassembly requires knowing ARM Thumb-2 encoding.

**Possible future path:** Start with Mecrisp (native) for performance. If Forth9 moves to a new architecture, evaluate whether to port Mecrisp's native compiler or switch to STC. The user-facing language (prefixes, quotations, words) stays the same regardless of threading model.

### Design Ideas Worth Stealing

**HAL per platform.** DuskOS has a thin Hardware Abstraction Layer per target. The HAL handles:
- Boot sequence
- Serial I/O
- Timer
- Memory map

Everything above the HAL is portable Forth. When Forth9 supports multiple chips (RP2350 now, ESP32-P4 later), structure it the same way:

```
Flash:
  HAL (chip-specific)     — GPIO registers, SPI addresses, interrupt vectors
  Kernel (portable Forth) — dictionary, compiler, prefix listener, image load/save
PSRAM:
  Image (portable Forth)  — all user words, data, state
```

Changing chips means rewriting the HAL (~500 lines), not the kernel or the image.

**Documentation = code.** DuskOS documentation volume nearly equals code volume. Every Forth9 word should have a stack comment and a one-line description. `see` shows the source. `help` shows the docs. Both exist for every word.

**Civilizational resilience.** DuskOS is designed to work when modern hardware production has stopped — running on 30-year-old machines that still exist. Forth9 is designed to work when modern infrastructure has stopped — communicating without cell towers, computing without cloud services. Same philosophy, different failure mode.

**CC0 license.** DuskOS is public domain. Mecrisp is GPL (copyleft — derivatives must stay open). Forth9's license choice matters:
- **GPL:** Forces all forks to remain open source. Protects the community. But companies may avoid it.
- **CC0 / MIT / BSD:** Maximum freedom. Anyone can use it, including in closed products. Broader adoption.
- **Recommendation:** The Forth9 kernel additions (prefix listener, quotation compiler, image persistence) could be MIT or CC0, even though the Mecrisp fork underneath is GPL. The GPL only applies to the Mecrisp-derived code. Consult a lawyer before shipping a product.

---

## FURS — Forth Unified Register System

**Source:** <https://mecrisp-stellaris-folkdoc.sourceforge.io/furs/blog-furs.html>

Terry Porter's solution to the register definition problem. ARM MCUs have hundreds of hardware registers (GPIO, SPI, ADC, DMA, timers). In C, vendor header files name them all. In Forth, you have to define them yourself — tedious, error-prone, and time-consuming.

**The solution:** Every ARM chip ships with a CMSIS-SVD file (XML describing every register, field, and bit). Parse it and auto-generate Forth constants.

**Tools Porter built:**

| Tool | What it does |
|---|---|
| svd2forth (v1-v3) | Generates Forth peripheral word definitions from SVD |
| svd2db | Creates a searchable SQLite database from SVD |
| FURS | External upload system for flash-constrained MCUs |

**What this means for Forth9:**

The RP2350 has an SVD file published by Raspberry Pi. Run it through svd2forth or a simple custom parser to generate all register definitions:

```forth
\ Auto-generated from RP2350 SVD — never type these by hand
$400140CC constant GPIO_OUT
$40014008 constant GPIO_OE
$4003C000 constant SPI0_BASE
$4003C004 constant SPI0_SSPCR0
\ ... hundreds more
```

On the Pico Plus 2W (8MB PSRAM), all register definitions fit comfortably in the image. Generate once, save to image, every register is a named word forever. No raw hex addresses, no typos.

**Action:** In Phase 1, find the RP2350 SVD file, generate register definitions, load into Mecrisp. This is a one-time setup that makes all subsequent hardware work dramatically easier.

Both the Pico 2W and Pico Plus 2W use the same RP2350B chip — same SVD file, same registers. Generate once, use on both boards.

---

## Mecrisp Stellaris

**Source:** <https://mecrisp.sourceforge.net/> | **License:** GPL

The native ARM Forth compiler used as Forth9's kernel. Written by Matthias Koch.

### Key Technical Details

- Compiles Forth to native ARM Thumb-2 machine code
- Targets: many ARM Cortex-M chips including RP2040 and RP2350
- `compiletoflash` / `compiletoram` — choose where definitions live
- Interrupt handlers can be Forth words — register an xt as an ISR vector
- Full ANS-style outer interpreter (replaced by Forth9 prefix listener)
- Dictionary is a linked list in memory — walkable, inspectable
- `see` disassembles any word to machine code
- Single-file kernel, typically ~4KB in flash

### What Forth9 Replaces

| Mecrisp Component | Forth9 Replacement |
|---|---|
| Outer interpreter (ANS-style) | Prefix listener with `#` `$` `&` `'` `/` `@` `~` |
| No quotations | `[ ]` anonymous native-compiled blocks |
| Flat word names | `namespace:word` naming convention |
| No image persistence | PSRAM snapshot to SD card |
| UART-only I/O | PicoCalc keyboard + display drivers |

### What Forth9 Keeps

- The native code compiler — untouched
- The dictionary structure — untouched
- `compiletoflash` / `compiletoram` — essential for image model
- Interrupt handler registration — essential for radio and audio
- Stack and memory words — untouched
- Arithmetic and logic — untouched

---

## RetroForth 12

**Source:** <http://retroforth.org/> | **License:** ISC

The syntax and semantics reference for Forth9's language design. Written by Charles Childers.

### What Forth9 Takes from RetroForth

- **Prefix system:** `#` (number), `$` (char), `&` (pointer), `'` (lookup) — first character of token determines how it's handled
- **Quotations:** `[ ]` as first-class anonymous functions
- **Namespace naming:** `s:length`, `n:add`, `d:lookup` — `namespace:word` convention
- **Image persistence model:** Entire system state as a single snapshot

### What Forth9 Does NOT Take

- **The NGA VM.** RetroForth runs on a 30-instruction virtual machine implemented in C. Forth9 compiles to native ARM. No VM, no C.
- **The instruction set.** NGA's 30 instructions are elegant but interpreted. Mecrisp's native compiler is faster.
- **The I/O model.** RetroForth's I/O goes through the VM host. Forth9 talks to hardware registers directly.

### Why Not Just Use RetroForth

RetroForth VM on RP2350 would mean: C runtime (VM host) → NGA VM (interpreter) → Forth words. Two languages, interpretation overhead, can't write interrupt handlers in Forth.

Mecrisp on RP2350 means: Forth words → ARM machine code → hardware. One language, native speed, Forth ISRs.

Forth9 takes RetroForth's superior language design and puts it on Mecrisp's superior execution engine.

---

## Threading Models Reference

For anyone evaluating Forth implementations:

| Model | How a word call works | Speed | Complexity | Examples |
|---|---|---|---|---|
| **Indirect Threaded (ITC)** | Cell contains pointer to code field, which points to machine code | Slowest | Simple | Classic FIG-Forth, early Gforth |
| **Direct Threaded (DTC)** | Cell contains pointer directly to machine code | Moderate | Simple | Gforth (modern), many traditional Forths |
| **Subroutine Threaded (STC)** | Cell is a native CALL instruction | Good | Moderate | DuskOS, SwiftForth |
| **Native Code** | Primitives inlined as machine instructions | Best | Complex | Mecrisp Stellaris, VFX Forth |

Forth9 uses native code (Mecrisp). If portability becomes critical, STC (DuskOS-style) is the fallback. ITC and DTC are not considered — too slow for ISRs and DSP.
