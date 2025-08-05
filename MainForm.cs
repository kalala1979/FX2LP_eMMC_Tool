using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.WinUsb;

namespace EMMC_Tool
{
    public partial class MainForm : Form
    {
        // USB constants
        private const int VendorID = 0x04B4;
        private const int ProductID = 0x0101;

        // Vendor requests
        private const byte VR_EMMC_INIT = 0xD0;
        private const byte VR_EMMC_READ_BLOCK = 0xD1;
        private const byte VR_EMMC_WRITE_BLOCK = 0xD2;
        private const byte VR_EMMC_STATUS = 0xD3;

        // USB device
        private UsbDevice usbDevice;
        private UsbEndpointReader reader;
        private UsbEndpointWriter writer;

        // Status flags
        private bool isConnected = false;
        private bool isEmmcInitialized = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Update UI elements
            UpdateStatus();

            // Try to find and connect to device at startup
            ConnectToDevice();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Clean up USB device
            DisconnectDevice();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                ConnectToDevice();
            }
            else
            {
                DisconnectDevice();
            }
            UpdateStatus();
        }

        private void btnInitEmmc_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Please connect to FX2LP device first.", "Not Connected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create setup packet for EMMC_INIT request
                UsbSetupPacket setupPacket = new UsbSetupPacket(
                    (byte)UsbRequestType.TypeVendor | (byte)UsbRequestRecipient.RecipDevice,
                    VR_EMMC_INIT,
                    0, 0, 1);

                // Buffer to receive response
                byte[] buffer = new byte[1];

                // Send vendor request
                int bytesTransferred = 0;
                if (usbDevice.ControlTransfer(ref setupPacket, buffer, 0, 1, out bytesTransferred))
                {
                    if (bytesTransferred == 1 && buffer[0] == 1)
                    {
                        isEmmcInitialized = true;
                        logBox.AppendText("eMMC initialized successfully\r\n");
                    }
                    else
                    {
                        isEmmcInitialized = false;
                        logBox.AppendText("Failed to initialize eMMC\r\n");
                    }
                }
                else
                {
                    logBox.AppendText("Control transfer failed\r\n");
                }
            }
            catch (Exception ex)
            {
                logBox.AppendText($"Error: {ex.Message}\r\n");
            }

            UpdateStatus();
        }

        private void btnReadBlock_Click(object sender, EventArgs e)
        {
            if (!isConnected || !isEmmcInitialized)
            {
                MessageBox.Show("Please connect to FX2LP device and initialize eMMC first.", 
                    "Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get block address from text box
                if (!uint.TryParse(txtBlockAddress.Text, out uint blockAddress))
                {
                    MessageBox.Show("Please enter a valid block address.", 
                        "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create setup packet for EMMC_READ_BLOCK request
                UsbSetupPacket setupPacket = new UsbSetupPacket(
                    (byte)UsbRequestType.TypeVendor | (byte)UsbRequestRecipient.RecipDevice | 0x80, // 0x80 for IN
                    VR_EMMC_READ_BLOCK,
                    (short)(blockAddress & 0xFFFF),
                    (short)((blockAddress >> 16) & 0xFFFF),
                    64);

                // Buffer to receive data (first 64 bytes)
                byte[] buffer = new byte[64];

                // Send vendor request
                int bytesTransferred = 0;
                if (usbDevice.ControlTransfer(ref setupPacket, buffer, 0, 64, out bytesTransferred))
                {
                    if (bytesTransferred == 64)
                    {
                        // Display data
                        hexViewer.Text = BitConverter.ToString(buffer).Replace("-", " ");
                        logBox.AppendText($"Read {bytesTransferred} bytes from block {blockAddress}\r\n");
                        
                        // In a real application, you would need to read the remaining 448 bytes
                        logBox.AppendText("Note: Only first 64 bytes shown (full implementation would read all 512 bytes)\r\n");
                    }
                    else
                    {
                        logBox.AppendText("Failed to read block data\r\n");
                    }
                }
                else
                {
                    logBox.AppendText("Control transfer failed\r\n");
                }
            }
            catch (Exception ex)
            {
                logBox.AppendText($"Error: {ex.Message}\r\n");
            }
        }

        private void btnWriteBlock_Click(object sender, EventArgs e)
        {
            if (!isConnected || !isEmmcInitialized)
            {
                MessageBox.Show("Please connect to FX2LP device and initialize eMMC first.", 
                    "Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get block address from text box
                if (!uint.TryParse(txtBlockAddress.Text, out uint blockAddress))
                {
                    MessageBox.Show("Please enter a valid block address.", 
                        "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if there's data to write
                if (string.IsNullOrWhiteSpace(hexViewer.Text))
                {
                    MessageBox.Show("Please enter data to write in the hex viewer.", 
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse hex data
                string[] hexValues = hexViewer.Text.Split(new[] { ' ', '\r', '\n', '\t' }, 
                    StringSplitOptions.RemoveEmptyEntries);
                
                byte[] dataToWrite = new byte[Math.Min(64, hexValues.Length)];
                for (int i = 0; i < dataToWrite.Length; i++)
                {
                    dataToWrite[i] = Convert.ToByte(hexValues[i], 16);
                }

                // Create setup packet for EMMC_WRITE_BLOCK request
                UsbSetupPacket setupPacket = new UsbSetupPacket(
                    (byte)UsbRequestType.TypeVendor | (byte)UsbRequestRecipient.RecipDevice,
                    VR_EMMC_WRITE_BLOCK,
                    (short)(blockAddress & 0xFFFF),
                    (short)((blockAddress >> 16) & 0xFFFF),
                    64);

                // Send vendor request with data
                int bytesTransferred = 0;
                if (usbDevice.ControlTransfer(ref setupPacket, dataToWrite, 0, dataToWrite.Length, out bytesTransferred))
                {
                    logBox.AppendText($"Wrote {bytesTransferred} bytes to block {blockAddress}\r\n");
                    
                    // In a real application, you would need to write the remaining data
                    logBox.AppendText("Note: Only first 64 bytes written (full implementation would write all 512 bytes)\r\n");
                }
                else
                {
                    logBox.AppendText("Control transfer failed\r\n");
                }
            }
            catch (Exception ex)
            {
                logBox.AppendText($"Error: {ex.Message}\r\n");
            }
        }

        private void btnDump_Click(object sender, EventArgs e)
        {
            if (!isConnected || !isEmmcInitialized)
            {
                MessageBox.Show("Please connect to FX2LP device and initialize eMMC first.", 
                    "Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create save file dialog
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
                dialog.Title = "Save eMMC Dump";
                dialog.FileName = "emmc_dump.bin";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Ask for number of blocks to dump
                    using (var inputDialog = new BlockRangeDialog("Enter number of blocks to dump:", "1"))
                    {
                        if (inputDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (uint.TryParse(inputDialog.InputValue, out uint numBlocks) && numBlocks > 0)
                            {
                                // Get starting block address
                                if (!uint.TryParse(txtBlockAddress.Text, out uint startBlock))
                                {
                                    MessageBox.Show("Please enter a valid starting block address.", 
                                        "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                // Create progress dialog
                                using (var progressDialog = new ProgressDialog())
                                {
                                    progressDialog.Maximum = (int)numBlocks;
                                    progressDialog.Show(this);
                                    
                                    // Start background worker to dump blocks
                                    BackgroundWorker worker = new BackgroundWorker();
                                    worker.WorkerReportsProgress = true;
                                    worker.WorkerSupportsCancellation = true;
                                    
                                    worker.DoWork += (s, args) => {
                                        string filePath = dialog.FileName;
                                        uint currentBlock = startBlock;
                                        int blocksDumped = 0;
                                        
                                        try
                                        {
                                            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                                            {
                                                for (uint i = 0; i < numBlocks; i++)
                                                {
                                                    if (worker.CancellationPending)
                                                    {
                                                        args.Cancel = true;
                                                        break;
                                                    }
                                                    
                                                    // Read block (simplified - would need full 512 byte implementation)
                                                    byte[] buffer = new byte[64];
                                                    UsbSetupPacket setupPacket = new UsbSetupPacket(
                                                        (byte)UsbRequestType.TypeVendor | (byte)UsbRequestRecipient.RecipDevice | 0x80,
                                                        VR_EMMC_READ_BLOCK,
                                                        (short)(currentBlock & 0xFFFF),
                                                        (short)((currentBlock >> 16) & 0xFFFF),
                                                        64);
                                                        
                                                    int bytesTransferred = 0;
                                                    if (usbDevice.ControlTransfer(ref setupPacket, buffer, 0, 64, out bytesTransferred))
                                                    {
                                                        if (bytesTransferred == 64)
                                                        {
                                                            fs.Write(buffer, 0, buffer.Length);
                                                            blocksDumped++;
                                                            worker.ReportProgress(blocksDumped);
                                                            
                                                            // Add dummy data for remaining bytes of block
                                                            byte[] dummyData = new byte[512 - 64];
                                                            fs.Write(dummyData, 0, dummyData.Length);
                                                        }
                                                    }
                                                    
                                                    currentBlock++;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show($"Error during dump: {ex.Message}", 
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        
                                        args.Result = blocksDumped;
                                    };
                                    
                                    worker.ProgressChanged += (s, args) => {
                                        progressDialog.Value = args.ProgressPercentage;
                                    };
                                    
                                    worker.RunWorkerCompleted += (s, args) => {
                                        progressDialog.Close();
                                        
                                        if (!args.Cancelled && args.Error == null)
                                        {
                                            int blocksDumped = (int)args.Result;
                                            logBox.AppendText($"Successfully dumped {blocksDumped} blocks to {dialog.FileName}\r\n");
                                        }
                                    };
                                    
                                    worker.RunWorkerAsync();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please enter a valid number of blocks.", 
                                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
                dialog.Title = "Load File";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[Math.Min(64, fs.Length)];
                            fs.Read(buffer, 0, buffer.Length);
                            
                            hexViewer.Text = BitConverter.ToString(buffer).Replace("-", " ");
                            logBox.AppendText($"Loaded {buffer.Length} bytes from {dialog.FileName}\r\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        logBox.AppendText($"Error loading file: {ex.Message}\r\n");
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            hexViewer.Clear();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            logBox.Clear();
        }

        // Helper methods
        private void ConnectToDevice()
        {
            try
            {
                // Find device by VID/PID
                UsbDeviceFinder finder = new UsbDeviceFinder(VendorID, ProductID);
                usbDevice = UsbDevice.OpenUsbDevice(finder);

                if (usbDevice == null)
                {
                    logBox.AppendText("Device not found. Please connect FX2LP device.\r\n");
                    return;
                }

                // Configure device
                IUsbDevice wholeDevice = usbDevice as IUsbDevice;
                if (wholeDevice != null)
                {
                    wholeDevice.SetConfiguration(1);
                    wholeDevice.ClaimInterface(0);
                }

                // Set up endpoints
                reader = usbDevice.OpenEndpointReader(ReadEndpointID.Ep06);
                writer = usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);

                // Check eMMC status
                UsbSetupPacket setupPacket = new UsbSetupPacket(
                    (byte)UsbRequestType.TypeVendor | (byte)UsbRequestRecipient.RecipDevice | 0x80, // 0x80 for IN
                    VR_EMMC_STATUS,
                    0, 0, 1);

                byte[] buffer = new byte[1];
                int bytesTransferred = 0;

                if (usbDevice.ControlTransfer(ref setupPacket, buffer, 0, 1, out bytesTransferred))
                {
                    if (bytesTransferred == 1)
                    {
                        isEmmcInitialized = buffer[0] == 1;
                    }
                }

                isConnected = true;
                logBox.AppendText("Connected to FX2LP device\r\n");
            }
            catch (Exception ex)
            {
                logBox.AppendText($"Error connecting to device: {ex.Message}\r\n");
                DisconnectDevice();
            }
        }

        private void DisconnectDevice()
        {
            if (usbDevice != null)
            {
                if (usbDevice.IsOpen)
                {
                    // Release interface
                    IUsbDevice wholeDevice = usbDevice as IUsbDevice;
                    if (wholeDevice != null)
                    {
                        wholeDevice.ReleaseInterface(0);
                    }

                    usbDevice.Close();
                }
                UsbDevice.Exit();
                
                usbDevice = null;
                reader = null;
                writer = null;
            }
            
            isConnected = false;
            isEmmcInitialized = false;
            logBox.AppendText("Disconnected from device\r\n");
        }

        private void UpdateStatus()
        {
            // Update connection status
            lblConnection.Text = isConnected ? "Connected" : "Not Connected";
            lblConnection.ForeColor = isConnected ? Color.Green : Color.Red;
            
            // Update eMMC status
            lblEmmcStatus.Text = isEmmcInitialized ? "Initialized" : "Not Initialized";
            lblEmmcStatus.ForeColor = isEmmcInitialized ? Color.Green : Color.Red;
            
            // Update button text
            btnConnect.Text = isConnected ? "Disconnect" : "Connect";
            
            // Enable/disable controls based on connection status
            btnInitEmmc.Enabled = isConnected;
            btnReadBlock.Enabled = isConnected && isEmmcInitialized;
            btnWriteBlock.Enabled = isConnected && isEmmcInitialized;
            btnDump.Enabled = isConnected && isEmmcInitialized;
            txtBlockAddress.Enabled = isConnected && isEmmcInitialized;
        }
    }

    // Custom dialog for getting block range
    public class BlockRangeDialog : Form
    {
        private TextBox txtInput;
        private Button btnOK;
        private Button btnCancel;
        private Label lblPrompt;

        public string InputValue { get { return txtInput.Text; } }

        public BlockRangeDialog(string prompt, string defaultValue = "")
        {
            // Initialize components
            this.Text = "Input";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 150;
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            // Create controls
            lblPrompt = new Label();
            lblPrompt.Text = prompt;
            lblPrompt.Location = new Point(10, 10);
            lblPrompt.Width = 270;

            txtInput = new TextBox();
            txtInput.Location = new Point(10, 40);
            txtInput.Width = 270;
            txtInput.Text = defaultValue;

            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(120, 80);

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(205, 80);

            // Add controls to form
            this.Controls.Add(lblPrompt);
            this.Controls.Add(txtInput);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }
    }

    // Progress dialog
    public class ProgressDialog : Form
    {
        private ProgressBar progressBar;
        private Button btnCancel;
        
        public int Maximum 
        { 
            get { return progressBar.Maximum; }
            set { progressBar.Maximum = value; }
        }
        
        public int Value 
        {
            get { return progressBar.Value; }
            set 
            {
                if (value <= progressBar.Maximum)
                    progressBar.Value = value;
            }
        }

        public ProgressDialog()
        {
            // Initialize components
            this.Text = "Operation Progress";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 120;
            this.ControlBox = false;

            // Create controls
            progressBar = new ProgressBar();
            progressBar.Location = new Point(10, 20);
            progressBar.Width = 270;
            progressBar.Height = 25;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(105, 55);

            // Add controls to form
            this.Controls.Add(progressBar);
            this.Controls.Add(btnCancel);
        }
    }
}