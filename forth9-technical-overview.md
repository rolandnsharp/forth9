# Forth9 — Technical Overview

Bare metal Forth OS on an RP2350 microcontroller. No Linux, no RTOS, no C runtime. One language from boot to REPL to mesh radio driver.

---

## Hardware

### Current Platform: PicoCalc + Pimoroni Pico Plus 2

| Component | Part | Interface | Notes |
|---|---|---|---|
| MCU | RP2350B (Pimoroni Pico Plus 2) | — | Dual-core ARM Cortex-M33 @ 150MHz, also has RISC-V Hazard3 cores (selectable at boot) |
| RAM | 8MB PSRAM (AP Memory APS6404L) | QSPI | On the Pimoroni board, directly wired to RP2350 QSPI bus. This is the living image — entire OS state lives here |
| Flash | 16MB (W25Q128) | QSPI | Kernel only. ~4KB Forth kernel baked into flash, everything else loads from SD into PSRAM |
| Display | 320×320 4" IPS LCD (PicoCalc) | SPI (ILI9488) | Square display with tempered glass. Golden ratio split: 320×198 canvas + 320×122 prompt (~10 lines) |
| Keyboard | 65-key mechanical (PicoCalc) | GPIO matrix | Directly scanned from Forth — no HID layer, no USB stack |
| Storage | microSD | SPI (bit-bang or hardware SPI on GP2–GP5) | Image persistence + flat files. SD protocol implemented in Forth against SPI registers |
| LoRa | SX1262 module (Semtech) | SPI + GPIO (BUSY, DIO1, RESET) | Wired to exposed GPIOs on PicoCalc. 868/915 MHz, up to ~2km range |
| WiFi | CYW43439 (on Pico Plus 2) | SPI (via RP2350 PIO) | 802.11b/g/n. Proximity gossip sync |
| Bluetooth | CYW43439 (on Pico Plus 2) | Same as WiFi | BLE 5.2. Key exchange, trust establishment |
| Battery | LiPo (PicoCalc) | — | PicoCalc provides charging circuit. MCU draws ~50mW active, µW in dormant |
| Audio out | I2S DAC or PWM | I2S / GPIO | External module or PWM direct. Synth, playback, signal gen |
| Audio in | I2S MEMS mic or ADC | I2S / ADC | External module. Tuner, recorder, oscilloscope input |
| Camera | OV2640 or similar SPI camera | SPI | Future add-on via exposed GPIOs. Stills and video to SD |

### Radio Add-on Modules

All optional. Connected via SPI or audio interface on exposed GPIOs.

| Module | Part | Interface | Frequency | Use |
|---|---|---|---|---|
| HF transceiver | uSDX or QDX | Audio in/out (ADC/DAC) + PTT GPIO | 3-30 MHz | International contact. JS8Call, FT8, WSPR. Ionospheric skip — global range, no infrastructure. 5W on a wire antenna. |
| VHF transceiver | SA818 / DRA818V | Audio in/out + PTT GPIO + serial config | 144-148 MHz | APRS, packet radio, repeater access. ~$8 module. |
| Sub-GHz transceiver | CC1101 | SPI (SCK, MOSI, MISO, CS, GDO0, GDO2) | 300-928 MHz | General-purpose RF. OOK, ASK, FSK, GFSK, MSK modulation. Decode/replay weather stations, remotes, keyfobs, TPMS, POCSAG pagers. Protocol analysis at the REPL. ~$3 module. |
| SDR receiver | Si4732 | I2C | 150kHz-30MHz AM/SW, 64-108MHz FM | Broadcast reception. All-band scanning. SSB for ham monitoring. ~$5 module. |

#### HF Digital Modes — How It Works

The RP2350 does all DSP. No laptop needed.

```
Receive:  HF transceiver audio out → RP2350 ADC → Forth DSP words → decode → screen
Transmit: Forth DSP words → RP2350 DAC/PWM → HF transceiver audio in → antenna → ionosphere → world
```

- **JS8Call** — Keyboard-to-keyboard chat over HF. Weak-signal, store-and-forward, mesh-like relay between ham stations. Messages hop. You type on the PicoCalc, someone on another continent reads it. This is Forth9's mesh philosophy at global range.
- **FT8** — Ultra-weak-signal structured exchanges. Contacts at -24dB SNR. Thousands of km on 1 watt.
- **WSPR** — Beacon mode. Transmit a few watts, see where in the world you were heard.
- **All decoded/encoded in Forth.** `hf:waterfall` shows the spectrum. `js8:send` transmits. `hf:decode` parses incoming. Every step is a word you can inspect with `see`.

#### Sub-GHz (CC1101) — Flipper-Class RF

The CC1101 is what gives the Flipper Zero its RF capabilities. On Forth9, the difference is: you write the protocol decoder yourself at the REPL in real time.

```forth
cc1101:init #433920 cc1101:freq!           ( set to 433.92 MHz )
cc1101:rx-mode                              ( start receiving )
cc1101:capture                              ( raw sample buffer )
[ decode-ook .bits ] each                   ( decode and display OOK bitstream )
: weather-station ( bits -- temp humidity )  ( define a decoder — save it forever )
  ... ;
save
```

Supported modulations: OOK, ASK, 2-FSK, 4-FSK, GFSK, MSK. Raw capture and replay. Configurable bandwidth, data rate, deviation. All via SPI register writes from Forth.

#### Ham License

HF and VHF transmit requires an amateur radio license. Receive is unrestricted everywhere. The CC1101 on ISM bands (433 MHz, 868/915 MHz) does not require a license within power limits. LoRa (SX1262) operates on ISM bands — no license needed.

### PicoCalc GPIO Allocation

The PicoCalc exposes the Pico's pins but uses some for its own hardware. Key available GPIOs for LoRa and peripherals:

| GPIO | Assignment |
|---|---|
| GP2 | SX1262 SCK |
| GP3 | SX1262 MOSI |
| GP4 | SX1262 MISO |
| GP5 | SX1262 CS |
| GP6 | SX1262 BUSY |
| GP7 | SX1262 DIO1 (interrupt) |
| GP8 | SX1262 RESET |
| Remaining | Audio, ADC inputs, camera — depends on final allocation |

**Note:** PicoCalc does NOT expose the Pico's QSPI pins. The Pimoroni Pico Plus 2's PSRAM is wired directly on its own board via QSPI to the RP2350 — no conflict with PicoCalc's GPIO routing. The PSRAM is invisible to PicoCalc; it's between the RP2350 and the PSRAM chip on the Pimoroni board.

### RP2350 Relevant Peripherals

| Peripheral | Use |
|---|---|
| SPI0 / SPI1 | SD card, display, LoRa — directly accessed via memory-mapped registers from Forth |
| PIO (x2, 12 state machines) | CYW43439 WiFi/BT driver, custom protocols, WS2812, anything bit-bangy |
| ADC (4 channels, 12-bit, 500ksps) | Oscilloscope, multimeter, audio input |
| PWM (12 channels) | Audio output, signal generator, LED |
| DMA (12 channels) | Bulk transfers — SD block reads, display framebuffer push, audio streaming |
| Hardware interpolator (x2) | DSP — audio synthesis, FFT acceleration |
| Hardware AES | Image encryption at rest, mesh packet encryption |
| TRNG | Entropy source for key generation |
| ARM TrustZone | Potential secure boot partition for kernel |
| eFuse (OTP) | Production firmware signing — irreversible, one-time |

---

## OS Architecture

### Memory Map

```
Flash (16MB)
┌─────────────────────────┐ 0x10000000
│ Mecrisp Stellaris kernel │ ~4KB — native ARM Thumb-2 Forth compiler
│ SD card driver (Forth)   │ ~1KB — SPI init, block read/write
│ Image loader (Forth)     │ ~0.5KB — reads SD blocks into PSRAM on boot
│ Prefix listener (Forth)  │ ~1KB — outer interpreter replacement
│ Quotation compiler       │ ~2KB — [ ] anonymous native compilation
│ (unused flash)           │ 16MB available, mostly empty
└─────────────────────────┘

PSRAM (8MB) — THE IMAGE
┌─────────────────────────┐ 0x11000000 (typical XIP range for PSRAM)
│ Dictionary               │ All user-defined words, compiled to native ARM
│ Variables / data         │ All system state, mesh routing tables, config
│ Stacks                   │ Data stack, return stack
│ Buffers                  │ Display framebuffer, radio packet buffers
│ User data                │ Notes, logs, anything the user stores
└─────────────────────────┘
    ↕ save/load ↕
SD Card
┌─────────────────────────┐
│ image.nga                │ Raw PSRAM dump — one file = entire system state
│ event.log                │ Append-only input journal
│ user files               │ Flat files — text, WAV, images
└─────────────────────────┘
```

### Boot Sequence

```
Power on
  → RP2350 boots from flash (hardware reset vector)
  → Mecrisp kernel initialises: stacks, dictionary pointer, UART
  → image:load runs from flash:
      → SPI init for SD card
      → CMD0 → CMD8 → ACMD41 → SD card ready
      → Read sequential blocks into PSRAM
      → Set dictionary pointer, stack pointers from image header
  → REPL starts — user is exactly where they left off
```

Total boot time: kernel init (microseconds) + SD read (milliseconds for 8MB at ~10MHz SPI ≈ ~7 seconds for a full image, faster for partial/compressed).

No bootloader chain. No init system. No filesystem mount. No kernel modules. No login.

### Save Sequence

```
User types: save
  → image:save runs (compiled in flash or PSRAM, callable either way):
      → Write image header: magic number, dictionary pointer, stack state, checksum
      → Dump PSRAM to SD as sequential blocks
      → Verify checksum
      → Done
```

### The Kernel (~750 lines of Forth)

Mecrisp Stellaris provides:

- Native ARM Thumb-2 compiler (words compile to machine code, not threaded)
- Inner interpreter (DOCOL/EXIT for colon definitions)
- Outer interpreter (replaced by Forth9 prefix listener)
- Dictionary management (CREATE, DOES>, VARIABLE, CONSTANT)
- Arithmetic, logic, memory access, stack manipulation
- `compiletoflash` / `compiletoram` switching
- Interrupt handler registration (Forth word as ISR)

Forth9 adds on top (all in Forth, compiled to flash):

| Component | Lines | Purpose |
|---|---|---|
| SD card driver | ~200 | SPI register pokes, CMD sequences, block read/write |
| Image load/save | ~100 | PSRAM ↔ SD card, header, checksum |
| Prefix listener | ~150 | `#` `$` `&` `'` `/` `@` `~` dispatch |
| Quotation compiler | ~300 | `[` `]` anonymous native-compiled blocks |
| **Total kernel addition** | **~750** | |

Everything above this line lives in flash. Everything below lives in PSRAM and is part of the image.

### No C, No Assembly Shims

The entire system is Forth. Hardware register access is direct:

```forth
$400140CC constant GPIO_OUT         \ GPIO output register
: led-on   #1 #25 lshift GPIO_OUT ! ;
: led-off  #0 GPIO_OUT ! ;
```

SPI communication to the SD card, LoRa, and display are all memory-mapped register writes from Forth. Mecrisp compiles these to native ARM `LDR`/`STR` instructions. No function call overhead, no HAL, no SDK.

Interrupt handlers are Forth words:

```forth
: lora-isr ( -- )
  spi:read-fifo packet-buffer packet-len !
  packet-ready flag:set ;
' lora-isr irq-dio1 !   \ register as DIO1 interrupt handler
```

The RP2350's NVIC fires the vector, which jumps to compiled ARM code that IS the Forth word. No context switch, no C wrapper.

---

## Radio Architecture

### Radio Architecture

**Built-in (Core):**

```
┌──────────────────────────────────────────────────┐
│ LoRa (SX1262)                                    │
│ Range: 1-10 km    BW: 0.3-50 kbps               │
│ Always-on backbone                               │
│ Short messages, commands, Forth quotations        │
│ Packet format: src_key | dst_key | ttl | payload │
│ Encrypted per-peer (AES-128-CTR + HMAC)          │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│ WiFi (CYW43439)                                  │
│ Range: ~50m        BW: 54 Mbps                   │
│ Proximity gossip                                 │
│ Auto-sync when two devices discover each other   │
│ Bulk transfer: files, journal entries, images     │
│ Store-and-forward — human movement = transport   │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│ Bluetooth (CYW43439 BLE 5.2)                     │
│ Range: ~10m        BW: 2 Mbps                    │
│ Intimate / trust establishment                   │
│ Cryptographic key exchange (in-person only)       │
│ Direct file transfer, device pairing              │
└──────────────────────────────────────────────────┘
```

**Add-on Modules:**

```
┌──────────────────────────────────────────────────┐
│ HF Transceiver (uSDX / QDX)                     │
│ Range: GLOBAL     BW: ~50 bps (JS8/FT8)         │
│ 3-30 MHz, ionospheric skip                       │
│ Interface: audio ADC/DAC + PTT GPIO              │
│ JS8Call, FT8, WSPR — all DSP in Forth            │
│ 5W output, wire antenna                          │
│ Requires ham license for transmit                │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│ VHF Transceiver (SA818 / DRA818V)                │
│ Range: 10-50 km   BW: 1200/9600 baud            │
│ 144-148 MHz ham band                             │
│ Interface: audio ADC/DAC + PTT + serial config   │
│ APRS, packet radio, repeater access              │
│ Requires ham license for transmit                │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│ Sub-GHz RF (CC1101)                              │
│ Range: 10-200m    BW: 0.6-500 kbps              │
│ 300-928 MHz, configurable modulation             │
│ Interface: SPI (SCK, MOSI, MISO, CS, GDO0/2)    │
│ OOK, ASK, FSK, GFSK, MSK                        │
│ Decode/replay: weather, remotes, TPMS, pagers   │
│ Protocol analysis at the REPL                    │
│ ISM band — no license within power limits        │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│ SDR Receiver (Si4732)                            │
│ Range: receive only                              │
│ 150kHz-30MHz (AM/SW), 64-108MHz (FM)            │
│ Interface: I2C                                   │
│ Broadcast reception, all-band scan, SSB          │
│ No license required (receive only)               │
└──────────────────────────────────────────────────┘
```

**Full range coverage:**

```
Bluetooth   10m     ──┐
WiFi        50m       │
LoRa        10km      │  zero infrastructure
CC1101      200m      │  at every range
VHF         50km      │
HF          GLOBAL  ──┘
```

### LoRa Packet Format

```
┌────────┬────────┬─────┬──────┬─────────────┬──────┐
│ src_pk │ dst_pk │ ttl │ type │ payload     │ HMAC │
│ 8B     │ 8B     │ 1B  │ 1B   │ variable    │ 16B  │
└────────┴────────┴─────┴──────┴─────────────┴──────┘
```

- `src_pk` / `dst_pk`: truncated public key hashes (8 bytes each)
- `ttl`: hop count, decremented on relay
- `type`: 0x01 = text, 0x02 = quotation (executable Forth), 0x03 = routing, 0x04 = gossip request
- `payload`: AES-128-CTR encrypted
- `HMAC`: authentication tag over entire packet

Maximum LoRa payload ~250 bytes. A Forth quotation like `[ sensor:read . ]` compiles to ~20 bytes of native ARM. Plenty of room.

### Gossip Protocol

Devices maintain a vector clock or event counter per peer. When two devices encounter each other (WiFi range), they exchange counters and sync missing events. No central server. Data propagates through the network as people move through the world.

---

## Security

| Layer | Mechanism | Hardware |
|---|---|---|
| Identity | Ed25519 keypair, generated at first boot | RP2350 TRNG for entropy |
| Image at rest | AES-256-CBC, key derived from passphrase (PBKDF2) | RP2350 hardware AES |
| Mesh traffic | AES-128-CTR + HMAC-SHA256, per-peer pre-shared keys | Software (or hardware AES) |
| Incoming code | Sandboxed vocabulary — restricted dictionary, no `save`, no raw register access | Software enforcement |
| Trust | In-person Bluetooth key exchange only. No CA, no server, no blockchain | BLE 5.2 |
| Secure boot | RP2350 eFuse-based firmware signing (production only, irreversible) | OTP fuses |

---

## Future Hardware Targets

### Near Term: ESP32-P4 (Premium Variant)

| Spec | Value |
|---|---|
| CPU | Dual-core RISC-V @ 400MHz |
| RAM | 32MB PSRAM |
| Video | Hardware H.264 codec, MIPI-CSI camera interface |
| I/O | USB-OTG, SDIO, SPI, I2C, UART, ADC, DAC |
| WiFi/BT | Requires external module (ESP32-C6 companion) |
| Price | ~$8 |

Needs a Mecrisp port to RISC-V (or write a new native Forth compiler for the Hazard3/RV32 ISA). The RP2350 already has RISC-V cores — start there.

### Long Term: Custom PCB

Purpose-built Forth9 board:

- RP2350 (or successor) + PSRAM + flash
- SX1262 LoRa on-board (not a module, integrated)
- CYW43439 or ESP32-C6 for WiFi/BT
- Display connector (SPI or MIPI)
- Keyboard connector (matrix)
- SD card slot
- LiPo charging circuit (TP4056 or similar)
- USB-C for charging + serial debug
- I2S audio codec
- Camera connector
- Exposed GPIO header for user peripherals
- 2-layer or 4-layer PCB, all standard components, hand-solderable QFN at worst

BOM target: $25-35 for the board. Full device with case, keyboard, screen, battery: $60-80.

### Longer Term: RISC-V Custom Silicon

Open instruction set, no licensing. Small teams are taping out chips now. Within a decade, feasible to design a chip where you control the ISA, the microarchitecture, the Forth compiler, and the OS. Full stack sovereignty.

---

## Comparison

| | Phone | Linux SBC | Forth9 |
|---|---|---|---|
| Boot time | 30-60s | 10-30s | Microseconds + image load |
| Battery life | 1 day | Hours | Weeks |
| Lines of OS code | ~50M | ~30M | ~750 |
| Languages in stack | Java/Kotlin/Swift, C, shell | C, Python, shell, config | Forth |
| Cross-compiler needed | Yes | Yes | No |
| One person can audit | No | No | Yes |
| Develop on device | No | Barely | Yes — that's the point |
| Network dependency | Total | Heavy | None — mesh is self-contained |
| Cost | $800+ | $50-100 | $60-80 |
| Who controls it | Apple/Google | Mostly you | You |

---

## Key Resources

- RP2350 datasheet: <https://datasheets.raspberrypi.com/rp2350/rp2350-datasheet.pdf>
- Mecrisp Stellaris: <https://mecrisp.sourceforge.net/>
- SX1262 datasheet: <https://www.semtech.com/products/wireless-rf/lora-connect/sx1262>
- Pimoroni Pico Plus 2: <https://shop.pimoroni.com/products/pimoroni-pico-plus-2>
- ClockworkPi PicoCalc: <https://www.clockworkpi.com/>
- RetroForth 12 (syntax reference): <http://retroforth.org/>
- CYW43439 (WiFi/BT): <https://www.infineon.com/cms/en/product/wireless-connectivity/airoc-wi-fi-plus-bluetooth-combos/wi-fi-4-702.11n/cyw43439/>
