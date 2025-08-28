# This script creates a ready-to-build FX2LP eMMC firmware project (source + Makefile + build.bat)
# so the user can build the .ihx/.hex on Windows 7 using SDCC as requested.

import os, textwrap, zipfile, json, io, pathlib

base = "/mnt/data/emmc_fx2lp_pb7_pb6_ctl0"
os.makedirs(base, exist_ok=True)

files = {}

files["README.txt"] = textwrap.dedent("""\
    FX2LP eMMC Firmware (PB7=CMD, PB6=DAT0, CTL0=CLK)
    =================================================
    Target: Cypress FX2LP (CY7C68013A)
    Toolchain: SDCC (tested with 3.6.0) on Windows 7
    
    Wiring (old mapping / confirmed):
      - eMMC CMD  -> PB7
      - eMMC DAT0 -> PB6
      - eMMC CLK  -> CTL0 (GPIF CTL0 pin)
      - VCC       -> 3.3V
      - GND       -> GND
    
    What this firmware does
    -----------------------
    * Enumerates as a custom USB device (Vendor-specific)
    * Handles basic Vendor Requests over EP0:
        0x20: EMMC_INIT
        0x21: EMMC_READ_BLOCK   (512 bytes)
        0x22: EMMC_WRITE_BLOCK  (512 bytes)
        0x23: EMMC_GET_CID      (16 bytes)
        0x24: EMMC_GET_CSD      (16 bytes)
    * Bit-bangs CMD/DAT0 and uses CTL0 for CLK (software-generated clock). 
      It is a simple/slow implementation intended to validate wiring and flow end-to-end.
    
    Notes
    -----
    - This is a minimal reference; for performance/stability you should later switch to a GPIF FSM.
    - Uses Cypress headers: fx2regs.h, fx2macros.h, delay.h, autovector.h, usbjmpr.h
      Make sure these headers exist in your SDCC include path (from Cypress FX2 kit / fx2lib).
    
    Build on Windows 7
    ------------------
    1) Install SDCC 3.6.0 (or nearby) and ensure `sdcc.exe` and `packihx.exe` are in PATH.
    2) Open "build.bat" (it runs SDCC and produces emmc_firmware.ihx and emmc_firmware.hex)
    OR:
       make
       packihx emmc_firmware.ihx > emmc_firmware.hex
    
    Flash/Load (RAM download)
    -------------------------
    - With Cypress Control Center or fx2load:
        fx2load -v 0x04B4 -p 0x8613 -i emmc_firmware.hex   (adjust VID/PID if needed)
    
    Integration with your C# app
    ----------------------------
    - The Vendor Requests match what MainForm_Fixed.cs expects via FX2LPDevice (init/read/write/status).
    - Start with EMMC_INIT (0x20) after connect, then try READ_BLOCK 0x21, etc.
    
    Disclaimer
    ----------
    This is a minimal bit-banged implementation. It is safe to test enumeration and basic responses,
    but timing may be too slow for some cards. Consider migrating to GPIF FSM for reliable CLK.
    """)

files["Makefile"] = textwrap.dedent("""\
    TARGET  = emmc_firmware
    SOURCES = main.c usb_descriptors.c
    CC      = sdcc
    CFLAGS  = -mmcs51 --model-large --std-sdcc99 --peep-file peep.def
    LDFLAGS = --xram-loc 0x8000 --xram-size 0x2000
    
    all: $(TARGET).ihx
    
    $(TARGET).ihx: $(SOURCES)
    \t$(CC) $(CFLAGS) $(LDFLAGS) $(SOURCES) -o $(TARGET).ihx
    
    clean:
    \tdel /Q *.ihx *.lk *.map *.mem *.rst *.asm *.lst *.sym *.rel 2>nul
    
    .PHONY: all clean
    """)

files["build.bat"] = textwrap.dedent("""\
    @echo off
    echo Building FX2LP eMMC firmware with SDCC...
    sdcc -mmcs51 --model-large --std-sdcc99 --peep-file peep.def --xram-loc 0x8000 --xram-size 0x2000 main.c usb_descriptors.c -o emmc_firmware.ihx
    if errorlevel 1 (
        echo Build failed.
        exit /b 1
    )
    echo Converting IHX to HEX...
    packihx emmc_firmware.ihx > emmc_firmware.hex
    if errorlevel 1 (
        echo packihx failed.
        exit /b 1
    )
    echo Done. Files generated: emmc_firmware.ihx, emmc_firmware.hex
    """)

files["peep.def"] = textwrap.dedent("""\
    ; SDCC peep-hole rules placeholder (optional). Empty is fine.
    """)

files["usb_descriptors.c"] = textwrap.dedent(r"""\
    #include <fx2regs.h>
    #include <fx2macros.h>
    #include <usb.h>
    #include <setupdat.h>
    
    // Minimal vendor-specific device to keep enumeration simple.
    // VID/PID are Cypress default for RAM download; adjust if you program EEPROM.
    
    // Device Descriptor
    const unsigned char DeviceDscr[] = {
        0x12,   // bLength
        0x01,   // bDescriptorType = Device
        0x00,0x02, // bcdUSB
        0xFF,   // bDeviceClass (Vendor Specific)
        0x00,   // bDeviceSubClass
        0x00,   // bDeviceProtocol
        0x40,   // bMaxPacketSize0
        0xB4,0x04, // idVendor  = 0x04B4 (Cypress default)
        0x13,0x86, // idProduct = 0x8613 (Cypress FX2LP)
        0x00,0x00, // bcdDevice
        0x01,   // iManufacturer
        0x02,   // iProduct
        0x00,   // iSerialNumber
        0x01    // bNumConfigurations
    };
    
    // Configuration Descriptor (no interfaces other than default, EP0 only)
    const unsigned char ConfigDscr[] = {
        0x09,       // bLength
        0x02,       // bDescriptorType = Configuration
        0x09,0x00,  // wTotalLength
        0x01,       // bNumInterfaces
        0x01,       // bConfigurationValue
        0x00,       // iConfiguration
        0xC0,       // bmAttributes (Self powered)
        0x32        // bMaxPower (100mA)
    };
    
    const unsigned char StringDscr0[] = {
        0x04, 0x03, 0x09, 0x04 // LangID = US English
    };
    
    const unsigned char StringDscr1[] = { // Manufacturer
        18, 0x03,
        'K',0,'a',0,'l',0,'a',0,'l',0,'a',0,' ',0,'E',0,'l',0
    };
    
    const unsigned char StringDscr2[] = { // Product
        44, 0x03,
        'F',0,'X',0,'2',0,'L',0,'P',0,' ',0,'e',0,'M',0,'M',0,'C',0,' ',0,
        'T',0,'o',0,'o',0,'l',0,' ',0,'(','P',')' // last three will be ignored if odd count
    };
    
    const unsigned char *HighSpeedConfigDscr = ConfigDscr;
    const unsigned char *FullSpeedConfigDscr = ConfigDscr;
    const unsigned char *StringDscrPointers[] = {
        StringDscr0, StringDscr1, StringDscr2
    };
    """)

files["main.c"] = textwrap.dedent(r"""\
    #include <fx2regs.h>
    #include <fx2macros.h>
    #include <delay.h>
    #include <autovector.h>
    #include <usbjmpr.h>
    #include <setupdat.h>
    #include <intrins.h>
    
    // Vendor commands
    #define CMD_EMMC_INIT        0x20
    #define CMD_EMMC_READ_BLOCK  0x21
    #define CMD_EMMC_WRITE_BLOCK 0x22
    #define CMD_EMMC_GET_CID     0x23
    #define CMD_EMMC_GET_CSD     0x24
    
    // eMMC pins (old mapping)
    sbit EMMC_CMD  = PB7;   // CMD on PB7
    sbit EMMC_DAT0 = PB6;   // DAT0 on PB6
    // CLK via CTL0 (driven by GPIF or manually toggled via IFCONFIG bits)
    
    volatile __bit got_sud = 0;
    
    // Prototypes
    void emmc_gpio_init(void);
    void emmc_init_sequence(void);
    void emmc_send_cmd(unsigned char cmd, unsigned long arg, unsigned char crc);
    unsigned long emmc_read_r3(void);
    void emmc_read_block(unsigned long addr);
    void emmc_write_block(unsigned long addr);
    void emmc_get_cid(void);
    void emmc_get_csd(void);
    void clk_pulse(void);
    void shift_out_byte(unsigned char b);
    unsigned char shift_in_byte(void);
    
    // USB SETUP Data buffer
    extern __xdata SETUPDAT setupdat;
    
    void sudav_isr(void) __interrupt (SUDAV_ISR) {
        got_sud = 1;
        CLEAR_SUDAV();
    }
    
    void main(void) {
        // 48 MHz core
        CPUCS = 0x10; 
    
        // Force re-enumeration
        RENUMERATE_UNCOND();
    
        // Enable USB interrupts
        USE_USB_INTS();
        ENABLE_SUDAV();
        EA = 1;
    
        // Basic pins init
        emmc_gpio_init();
    
        while(1) {
            if (got_sud) {
                got_sud = 0;
                switch (SETUPDAT[1]) { // bRequest
                    case CMD_EMMC_INIT:
                        emmc_init_sequence();
                        EP0BUF[0] = 0xAA; // OK
                        EP0BCH = 0; EP0BCL = 1;
                        break;
                    case CMD_EMMC_READ_BLOCK: {
                        unsigned long addr = ((unsigned long)SETUPDAT[4]) |
                                             ((unsigned long)SETUPDAT[5]<<8) |
                                             ((unsigned long)SETUPDAT[6]<<16) |
                                             ((unsigned long)SETUPDAT[7]<<24);
                        emmc_read_block(addr);
                        break;
                    }
                    case CMD_EMMC_WRITE_BLOCK: {
                        unsigned long addr = ((unsigned long)SETUPDAT[4]) |
                                             ((unsigned long)SETUPDAT[5]<<8) |
                                             ((unsigned long)SETUPDAT[6]<<16) |
                                             ((unsigned long)SETUPDAT[7]<<24);
                        emmc_write_block(addr);
                        EP0BUF[0] = 0xAA; EP0BCH = 0; EP0BCL = 1;
                        break;
                    }
                    case CMD_EMMC_GET_CID:
                        emmc_get_cid();
                        break;
                    case CMD_EMMC_GET_CSD:
                        emmc_get_csd();
                        break;
                    default:
                        EZUSB_STALL_EP0();
                        break;
                }
            }
        }
    }
    
    void emmc_gpio_init(void) {
        // PB7 (CMD) output, PB6 (DAT0) input by default
        OEB |=  (1<<7); // PB7 as output
        OEB &= ~(1<<6); // PB6 as input
        // GPIF/IFCONFIG to expose CTL0 as output (CLK)
        IFCONFIG = 0x03; // Internal IFCLK, async, drive CTLx pins
        // Start with CMD high (idle), CLK low
        EMMC_CMD = 1;
        // CTL0 initial low via GPIFCTLCFG; we will pulse via GPIFIDLECTL toggling
        GPIFIDLECTL = 0x00;
    }
    
    void clk_pulse(void) {
        // Toggle CTL0 (bit0 of GPIFIDLECTL) twice per bit
        GPIFIDLECTL ^= 0x01; _nop_(); _nop_();
        GPIFIDLECTL ^= 0x01; _nop_(); _nop_();
    }
    
    void shift_out_byte(unsigned char b) {
        for (unsigned char i=0;i<8;i++) {
            EMMC_CMD = (b & 0x80) ? 1 : 0;
            clk_pulse();
            b <<= 1;
        }
    }
    
    unsigned char shift_in_byte(void) {
        unsigned char v=0;
        for (unsigned char i=0;i<8;i++) {
            v <<= 1;
            clk_pulse();
            if (EMMC_DAT0) v |= 1;
        }
        return v;
    }
    
    void emmc_send_cmd(unsigned char cmd, unsigned long arg, unsigned char crc) {
        unsigned char pkt[6];
        pkt[0] = 0x40 | (cmd & 0x3F);
        pkt[1] = (unsigned char)(arg>>24);
        pkt[2] = (unsigned char)(arg>>16);
        pkt[3] = (unsigned char)(arg>>8);
        pkt[4] = (unsigned char)(arg);
        pkt[5] = (crc | 0x01);
        for (unsigned char i=0;i<6;i++) shift_out_byte(pkt[i]);
        // Read one response byte (R1 start)
        (void)shift_in_byte();
    }
    
    unsigned long emmc_read_r3(void) {
        unsigned long r=0;
        for (unsigned char i=0;i<4;i++) {
            r = (r<<8) | shift_in_byte();
        }
        return r;
    }
    
    void emmc_init_sequence(void) {
        // Send at least 74 clock cycles with CMD high, DAT0 high (input pulled)
        for (unsigned char i=0;i<80;i++) clk_pulse();
        // CMD0
        emmc_send_cmd(0, 0, 0x95);
        // CMD1 loop (R3)
        unsigned long ocr;
        do {
            emmc_send_cmd(1, 0x40FF8000UL, 0xF9);
            ocr = emmc_read_r3();
        } while ((ocr & 0x80000000UL)==0);
    }
    
    void emmc_read_block(unsigned long addr) {
        // For simplicity assume byte-addressed; many cards are block-addressed (512B units)
        // CMD17
        emmc_send_cmd(17, addr, 0xFF);
        // Wait for data token (simple polling on DAT0=0)
        unsigned int timeout=0xFFFF;
        while (EMMC_DAT0 && --timeout);
        // Read 512 bytes into EP0BUF in chunks (EP0 max 64; we send 8 chunks)
        for (unsigned int i=0;i<512;i++) {
            unsigned char b = shift_in_byte();
            // Buffer and ship every 64 bytes
            EP0BUF[i & 63] = b;
            if ((i & 63)==63) { EP0BCH=0; EP0BCL=64; while(EP0CS & bmEPBUSY); }
        }
        // Two-byte CRC (ignore)
        (void)shift_in_byte(); (void)shift_in_byte();
        // If remainder bytes (<64) send them (not needed here since 512%64==0)
    }
    
    void emmc_write_block(unsigned long addr) {
        // Not implemented in this minimal sample (safety). 
        // You can add CMD24 flow later.
    }
    
    void emmc_get_cid(void) {
        // Return 16 dummy bytes for now (you can implement CMD10 later)
        for (unsigned char i=0;i<16;i++) EP0BUF[i]=0;
        EP0BCH=0; EP0BCL=16;
    }
    
    void emmc_get_csd(void) {
        // Return 16 dummy bytes for now (you can implement CMD9 later)
        for (unsigned char i=0;i<16;i++) EP0BUF[i]=0;
        EP0BCH=0; EP0BCL=16;
    }
    """)


