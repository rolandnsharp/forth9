# Forth9 Development Log

## 2026-02-18: First Flash — Zeptoforth on Pico 2W

### Hardware

- **Board:** Raspberry Pi Pico 2W (RP2350B, 520KB SRAM, WiFi/BT, no PSRAM)
- **Connection:** USB cable to Linux desktop
- **Serial terminal:** picocom

### Flashing Zeptoforth

1. Entered bootloader mode: held BOOT button while plugging in USB
2. Pico mounted as USB mass storage at `/media/roland/RP2350` (128MB virtual drive)
3. **First attempt (wrong UF2):** Copied `zeptoforth_full-1.16.0.uf2` — this is the UART build. Pico rebooted but no serial device appeared. The non-USB build expects console on UART pins, not USB CDC.
4. **Second attempt (correct UF2):** Copied `zeptoforth_full_usb-1.16.0.uf2` — this is the USB CDC build. Pico rebooted and `/dev/ttyACM0` appeared immediately.
   ```
   cp zeptoforth-1.16.0/bin/1.16.0/rp2350/zeptoforth_full_usb-1.16.0.uf2 /media/roland/RP2350/
   ```

### Key Lesson

There are two console variants:
- `zeptoforth_full-1.16.0.uf2` — console on **UART pins** (for PicoCalc or breadboard with USB-serial adapter)
- `zeptoforth_full_usb-1.16.0.uf2` — console on **USB CDC** (for direct USB connection, shows as `/dev/ttyACM0`)

### Connecting

```bash
picocom -b 115200 /dev/ttyACM0
```

Permission denied without sudo. Fix permanently by adding yourself to the `dialout` group:

```bash
sudo usermod -a -G dialout roland
```

(Requires logout/login to take effect.) Until then, use sudo:

```bash
sudo picocom -b 115200 /dev/ttyACM0
```

### First Boot

```
Terminal ready

Welcome to zeptoforth
Built for rp2350, version 1.16.0, on Sat Jan 31 10:05:16 AM CST 2026
zeptoforth comes with ABSOLUTELY NO WARRANTY: for details type `license'
 ok
```

It's alive.

### Permission Fix

`picocom` needs access to `/dev/ttyACM0`. Without group membership, requires sudo.

```bash
sudo usermod -a -G dialout roland
# logout/login to take effect
```

### First Commands

```
lol unable to parse: lol
```

Unrecognised input gives `unable to parse: <token>`. No crash, just a message.

```
words
```

Dumps the full dictionary — hundreds of words. Zeptoforth is big. Notable modules visible:

- `fat32`, `simple-fat32`, `fat32-tools` — filesystem support
- `sd`, `block-dev` — SD card / block devices
- `psram-blocks`, `init-psram` — PSRAM support (for Pico Plus 2)
- `spi`, `i2c`, `uart`, `adc`, `pwm`, `dma` — hardware peripherals
- `pio`, `pio-pool` — PIO programmable I/O
- `usb`, `usb-core`, `usb-cdc-buffers` — USB stack
- `wifi` — (not listed yet, may need separate load)
- `task`, `task-pool`, `action`, `action-pool` — preemptive multitasking
- `oo` — object-oriented system
- `chan`, `fchan`, `rchan`, `schan` — channels (inter-task communication)
- `sema`, `lock`, `slock`, `core-lock` — synchronisation primitives
- `closure`, `lambda` — closures
- `see`, `disassemble` — decompiler / disassembler
- `edit` — built-in editor
- `timer`, `alarm`, `rtc`, `aon-timer` — timing
- `rng` — random number generator
- `float32`, `armv7m-fp` — hardware floating point
- `heap`, `pool`, `map` — data structures
- `gpio`, `pin` — GPIO control
- `led` — onboard LED

### Zeptoforth Version

- **Release:** v1.16.0
- **UF2 used:** `zeptoforth_full-1.16.0.uf2` (full build — includes all libraries)
- **Source:** https://github.com/tabemann/zeptoforth/releases/tag/v1.16.0

### Notes

- The Pico 2W has no PSRAM — image persistence to PSRAM not possible on this board
- Ordered: Pimoroni Pico Plus 2W (8MB PSRAM, 16MB flash, WiFi/BT) for full Forth9 development
- For now: learning Zeptoforth, testing the REPL, exploring the language
