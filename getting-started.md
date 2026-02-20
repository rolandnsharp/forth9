# Getting Started with Forth9

You need a Pimoroni Pico Plus 2W and a USB cable. That's it.

---

## Hardware

| Item | Notes |
|---|---|
| **Pimoroni Pico Plus 2W** | RP2350, 8MB PSRAM, 16MB flash, WiFi/BT. ~$12 |
| **USB-C cable** | Data cable, not charge-only |
| **A computer** | Linux, Mac, or Windows. Anything with a serial terminal |

You do NOT need the PicoCalc, the LoRa module, or any other hardware yet. Start here.

---

## 1. Install a Serial Terminal

**Linux (Debian/Ubuntu):**

```bash
sudo apt install picocom
```

**Linux (Arch):**

```bash
sudo pacman -S picocom
```

**Mac:**

```bash
brew install picocom
```

**Windows:** Use [PuTTY](https://www.putty.org/) or [Tera Term](https://teratermproject.github.io/) in serial mode.

---

## 2. Download Zeptoforth

Download the latest release from: https://github.com/tabemann/zeptoforth/releases

Extract it. The UF2 file you need is at:

```
zeptoforth-1.16.0/bin/1.16.0/rp2350_16mib/zeptoforth_full_usb-1.16.0.uf2
```

Use `rp2350_16mib` because the Pimoroni Pico Plus 2W has 16MB flash.

---

## 3. Flash to the Pico

1. **Hold the BOOTSEL button** on the Pico.
2. **Plug in the USB cable** while holding BOOTSEL.
3. **Release BOOTSEL.** The Pico appears as a USB drive called `RP2350`.
4. **Copy the UF2 file** to the drive:

```bash
cp zeptoforth-1.16.0/bin/1.16.0/rp2350_16mib/zeptoforth_full_usb-1.16.0.uf2 /media/$USER/RP2350/
```

The Pico reboots automatically. Zeptoforth is now running.

---

## 4. Connect

```bash
sudo picocom -b 115200 /dev/ttyACM0
```

If `/dev/ttyACM0` doesn't exist, check:

```bash
ls /dev/ttyACM*
ls /dev/ttyUSB*
```

You should see:

```
Welcome to zeptoforth
Built for rp2350_16mib, version 1.16.0
```

Type `enable-line` for command history and arrow key support.

---

## 5. First Words

```forth
1 2 + .
```

Output: `3` — the stack works.

```forth
: hello ." sovereign" cr ;
hello
```

Output: `sovereign` — you just defined and ran your first word.

```forth
: square dup * ;
7 square .
```

Output: `49` — composition works.

---

## 6. Persisting Definitions to Flash

Definitions compiled to flash survive reboot. The correct workflow:

```forth
compile-to-flash
: square dup * ;
: cube dup square * ;
compile-to-ram
```

**Important:** Variables compiled to flash must NOT be accessed until after a reboot. Always:
1. Define everything in `compile-to-flash` mode
2. Switch back with `compile-to-ram`
3. Reboot (unplug/replug)
4. THEN access your variables

**Never use `erase-all`** — it wipes the entire firmware and you'll need to reflash from step 3.

---

## 7. Recovery

If the board freezes (infinite loop, crash):
- Unplug USB, plug back in. Your flash definitions survive.

If the board won't boot (bad definition in flash):
1. Hold BOOTSEL while plugging in
2. Copy the UF2 file again (see step 3)
3. This reflashes Zeptoforth and erases user flash

---

## 8. Essential Words

| Word | Stack effect | What it does |
|---|---|---|
| `dup` | `( a -- a a )` | Duplicate top |
| `drop` | `( a -- )` | Discard top |
| `swap` | `( a b -- b a )` | Swap top two |
| `over` | `( a b -- a b a )` | Copy second to top |
| `.` | `( a -- )` | Print and consume top |
| `.s` | `( -- )` | Show stack without consuming |
| `variable` | `( "name" -- )` | Create a named memory slot |
| `!` | `( val addr -- )` | Store value at address |
| `@` | `( addr -- val )` | Fetch value from address |
| `constant` | `( val "name" -- )` | Create a named constant |
| `c!` | `( byte addr -- )` | Store one byte |
| `c@` | `( addr -- byte )` | Fetch one byte |
| `emit` | `( char -- )` | Print one ASCII character |
| `type` | `( addr len -- )` | Print a string |
| `words` | `( -- )` | List all defined words |
| `enable-line` | `( -- )` | Enable line editing and history |

---

## Resources

- **Zeptoforth** — https://github.com/tabemann/zeptoforth
- **Zeptoforth docs** — included in the release under `docs/`
- **RetroForth 12** (syntax inspiration) — http://retroforth.org/
- **Starting Forth** by Leo Brodie — https://www.forth.com/starting-forth/
- **Easy Forth** — https://skilldrick.github.io/easyforth/
