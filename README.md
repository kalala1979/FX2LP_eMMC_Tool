This project provides a firmware and PC software solution for interfacing with eMMC memory devices using an FX2LP microcontroller.

## Project Structure

- **Firmware**: Contains the C code for the FX2LP device to communicate with eMMC devices
- **PC_Software**: Windows application for controlling the FX2LP and managing eMMC operations

## Hardware Configuration

The firmware is designed to work with the following pin configuration:

| Signal | FX2LP Port | Notes           |
|--------|------------|-----------------|
| CMD    | PB7        | Output from FX2LP |
| CLK    | CTL0       | Output from FX2LP |
| DAT0   | PB6        | Input to FX2LP  |
| VCC    | 3.3V       | Power to eMMC   |
| GND    | GND        | Ground          |

## Firmware Features

- SPI mode communication with eMMC device
- eMMC initialization and standard commands
- Block read and write operations
- USB vendor commands for PC communication

## PC Software Features

- Device detection and connection
- eMMC initialization
- Block read/write operations
- Hex data viewer and editor
- Block dumping to file
- Works on Windows 7/10

## Building the Project

### Firmware

1. Install the Cypress FX2LP SDK
2. Compile the firmware using the Keil C51 compiler
3. Program the FX2LP using the Cypress USB Control Center or equivalent tool

### PC Software

1. Open the solution in Visual Studio 2019 or later
2. Build the project
3. Run the application with administrator privileges (required for USB device access)

## Installation

1. Install the LibUSB filter driver for the FX2LP device
2. Run the application as administrator
3. Connect to the FX2LP device
4. Initialize the eMMC

## Usage

1. Connect to the device using the "Connect" button
2. Initialize the eMMC using the "Initialize eMMC" button
3. Enter a block address and use "Read Block" to read data
4. Edit the hex data and use "Write Block" to write data
5. Use "Dump Blocks" to save multiple blocks to a file

## Troubleshooting

- If the device is not detected, check if the LibUSB driver is properly installed
- Ensure the FX2LP device has the correct firmware loaded
- Run the application as administrator to ensure proper USB access
- Verify the eMMC device is properly connected to the FX2LP pins

## Limitations

- The current implementation only supports SPI mode (1-bit) communication
- Block transfers are limited to 64 bytes at a time (full implementation would handle 512 bytes)
- Error recovery mechanisms are basic

## Future Improvements

- Support for 4-bit data mode
- Complete 512-byte block transfer
- More advanced eMMC commands
- Better error handling and recovery
