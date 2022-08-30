using System;
using System.Threading;
using System.Windows.Forms;

namespace NESEmulator
{
    public partial class downTenButton : Form
    {
        /* Define our Global CPU, PPU and ROM Cart Objects. */
        Cpu cpu;
        Ppu ppu;
        Cart cart;
        // Initialise our Render Pos ushort to 0. This will ensure our Memory Map Viewer starts at 0.
        ushort renderPos = 0;

        // Bool that dictates whether we're showing our Program Code or Graphics Data within our Memory Mao Viewer.
        bool renderingPrg = true;

        // The Width and Height of the Memory Map Viewer, in Hexadecimal Bytes.
        int debugWidth = 69;
        int debugHeight = 18;

        // Nullable Byte Array which will contain our Data to be Rendered within our Memory Map Viewer
        byte?[] renderData;

        /* Initialise our Form. */
        public downTenButton()
        {
            InitializeComponent();
        }

        /* When the Start Button is Clicked... */
        private void startButton_Click(object sender, EventArgs e)
        {
            // Clear our Debug TextBoxes.
            debugOutput.Clear();
            debugOutput2.Clear();

            // Create a new RomParser Object.
            RomParser rp = new RomParser();

            // Create a Cart from our ROM, which will be loaded in from our "ROM TESTS" directory.
            cart = rp.CreateCart(rp.GetHex("ROM TESTS/" + romBox.Text + ".nes"));

            /* Display Debug Data of our Cart's Header, including all of it's Properties. */
            debugOutput2.AppendText("---BEGIN HEADER---\n");

            debugOutput2.AppendText("PRG ROM: " + RomParser.HexToStr(cart.header._PRG_ROM));
            debugOutput2.AppendText("\nCHR ROM: " + RomParser.HexToStr(cart.header._CHR_ROM));
            debugOutput2.AppendText("\nMAPPER: " + RomParser.HexToStr(cart.header._MAPPER));
            debugOutput2.AppendText("\nMAPPER2: " + RomParser.HexToStr(cart.header._MAPPER2));
            debugOutput2.AppendText("\n");
            debugOutput2.AppendText("\nPRG RAM: " + RomParser.HexToStr(cart.header._PRG_RAM));
            debugOutput2.AppendText("\nTV: " + RomParser.HexToStr(cart.header._TV));
            debugOutput2.AppendText("\nTV2: " + RomParser.HexToStr(cart.header._TV2));

            debugOutput2.AppendText("\n---END HEADER---\n\n");

            debugOutput2.AppendText("Loaded!\n");
        }

        /* Render Hexadecimal Bytes within our Memory Map Viewer */
        private void Render()
        {
            // Clear our Debug TextBox
            debugOutput.Clear();

            // Define our current Render Pos Offset, which will increment after every Hexadecimal value within our Memory Map Viewer.
            ushort _renderPos = renderPos;

            /* Loop through our Memory Map Viewer's Height and Width. */
            for (int i = 0; i < debugHeight; i++)
            {
                for (int ii = 0; ii < debugWidth; ii++)
                {
                    // Does our current Render Pos Offset exist within the Render Data?
                    if (_renderPos >= renderData.Length)
                    {
                        // If not, display out-of-bounds characters.
                        debugOutput.AppendText("?? ");
                    }
                    // If our Render Pos Offset is valid...
                    else
                    {
                        // Is our Render Data at our current Render Pos Offset null?
                        if (renderData[_renderPos] == null)
                        {
                            // If so, display null characters.
                            debugOutput.AppendText("XX ");
                        }
                        // If our Render Data is Valid...
                        else
                        {
                            // Display the current Render Data Byte @ our Render Pos Offset, after converting it into a Hexadecimal.
                            debugOutput.AppendText(RomParser.HexToStr((byte)renderData[_renderPos]) + " ");
                        }

                        // Increment our Render Pos Offset.
                        _renderPos++;
                    }
                }

                // Print a New Line after filling the Width of our Render Data.
                debugOutput.AppendText("\n");
            }

            // Finally, update our Memory Map Viewer Label to represent what portion of memory we're viewing.
            debugViewLabel.Text = "Debug Viewing:\nFrom: " + renderPos + "\nTo: " + (_renderPos - 1).ToString() + "\nOut of: " + renderData.Length.ToString();
        }

        /* When the Run button is clicked... */
        private void runButton_Click(object sender, EventArgs e)
        {
            /* Clear our Debug Textboxes */
            debugOutput2.Clear();
            debugOutput3.Clear();

            // Create a new CPU Object.
            cpu = new Cpu();
            // Initialise it's memory to 16kb.
            cpu.mem.Init(65536);

            /* Configure the CPU's Debug Boxes to this form's Debug Boxes. */
            cpu.debugBox = debugOutput2;
            cpu.debugBox2 = debugOutput3;
            cpu.mem.debugBox = debugOutput2;

            /* Configure the CPU's Debug Flags to this form's Debug CheckBoxes. */
            cpu.debugWait = slowExecuteBox.Checked;
            cpu.CPUDebug = CPUDebug.Checked;
            cpu.debug = DEBUG.Checked;
            cpu.debug2 = DEBUG2.Checked;

            /* For every value up to 16kb... */
            for (int i = 0; i < 65536; i++)
            {
                // Set our CPU's Memory at this Value to 0. This initialises our CPU's Memory.
                cpu.mem.memory[i] = 0;
            }

            // Load our Program Code Data into our CPU's Memory.
            cpu.mem.Load(0x8000, cart.PRGRom.ToArray());
            // Is our PRGRom only 1 bank?
            if (cart.PRGRom.Count < 32767)
            {
                // If so, load the same bank we've just loaded into the second bank @ C000. These are mirrored due to the current Mapper.
                cpu.mem.Load(0xC000, cart.PRGRom.ToArray());
            }

            // This modifies a value within memory referring to the PPU's status. Since we don't have a PPU, this effectively tricks the
            // emulator into thinking the PPU has been fully initialised, and continues execution.
            cpu.mem.memory[0x2002] = 0xFF;

            // Are we not loading from a Program Counter Override?
            if (PCOverrideBox.Text == "")
            {
                // If not, initialise our CPU normally to grab our Program Counter from the Reset Vector. See Cpu.cs
                cpu.Init();
            }
            // If we are loading from a Program Counter Override...
            else
            {
                // Does our Program Counter Override start with a '$'?
                if (PCOverrideBox.Text.StartsWith("$"))
                {
                    // If so, load our Program Counter from the user's inputted Override, after converting it from a String to a uint16, also taking into
                    // consideration the fact it's a Hexadecimal Number.
                    cpu.registers.PC = (ushort)Convert.ToUInt16(UInt16.Parse(PCOverrideBox.Text.Substring(1), System.Globalization.NumberStyles.HexNumber));
                }
                // If our Program Counter Override doesn't start with a '$'...
                else
                {
                    // Load our Program Counter to just the user's inputted Override, converted to a Ushort. This should be in Denary.
                    cpu.registers.PC = (ushort)Convert.ToUInt16(PCOverrideBox.Text);
                }
            }

            // Create a new PPU.
            ppu = new Ppu();
            // Initialise the PPU's memory.
            ppu.mem.Init(16384);

            // Configure the PPU's Debug Boxes to this form's Debug Boxes.
            ppu.mem.debugBox = debugOutput2;

            // Load our Graphics Data into our PPU's Memory.
            ppu.mem.Load(0, cart.CHRRom.ToArray());

            // Set our Memory Map Viewer's Render Data to our CPU's current Memory.
            renderData = cpu.mem.memory;
            // Render our Memory Map Viewer.
            Render();

            /* Create a new Background Thread to run the CPU. */
            Thread thread = new Thread(cpu.Run);
            thread.IsBackground = true;
            thread.Start();
        }

        /* Define a function which will handle navigation through the Memory Map Viewer. Takes a Short as an Argument. */
        private void handleRenderPos(short handleBy)
        {
            // Sets the Render Pos to the current Render Pos + our Increment/Decrement Argument.
            renderPos = (ushort)(renderPos + handleBy);
            // Render a new Memory Map View from our new Render Pos.
            Render();
        }

        /* These Functions all add or subtract values to the Render Pos. Uses handleRenderPos Function. */
        private void upOneButton_Click(object sender, EventArgs e)
        {
            handleRenderPos(-1);
        }

        private void upFiveButton_Click(object sender, EventArgs e)
        {
            handleRenderPos(-5);
        }

        private void upTenButton_Click(object sender, EventArgs e)
        {
            handleRenderPos(-10);
        }

        private void up25Button_Click(object sender, EventArgs e)
        {
            handleRenderPos(-25);
        }

        private void up50Button_Click(object sender, EventArgs e)
        {
            handleRenderPos(-50);
        }

        private void up100Button_Click(object sender, EventArgs e)
        {
            handleRenderPos((short)(0 - debugWidth));
        }

        private void up200Button_Click(object sender, EventArgs e)
        {
            handleRenderPos((short)(0 - (debugHeight * debugWidth)));
        }

        private void downOneButton_Click(object sender, EventArgs e)
        {
            handleRenderPos(1);
        }

        private void downFiveButton_Click(object sender, EventArgs e)
        {
            handleRenderPos(5);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            handleRenderPos(10);
        }

        private void down25Button_Click(object sender, EventArgs e)
        {
            handleRenderPos(25);
        }

        private void down50Button_Click(object sender, EventArgs e)
        {
            handleRenderPos(50);
        }

        private void down100Button_Click(object sender, EventArgs e)
        {
            handleRenderPos((short)debugWidth);
        }

        private void down200Button_Click(object sender, EventArgs e)
        {
            handleRenderPos((short)(debugHeight * debugWidth));
        }

        private void gotoPRGRom_Click(object sender, EventArgs e)
        {
            // Set our Render Pos to the Location of our Program Code Data within our Memory Map, and Render.
            renderPos = 32768;
            Render();
        }

        private void gotoPRGRom2_Click(object sender, EventArgs e)
        {
            // Set our Render Pos to the Location of our second (or mirror) Program Code Data within our Memory Map, and Render.
            renderPos = 49152;
            Render();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Set our Render Pos to the End of our CPU's Memory, and Render.
            renderPos = 0xFFFF;
            Render();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Set our Render Pos to the Start of our CPU'S Stack, and Render.
            renderPos = 0x01FF;
            Render();
        }

        /* When the Goto Specified Button is Clicked... */
        private void gotoSpecified_Click(object sender, EventArgs e)
        {
            // Make sure our Program Counter Goto Text isn't empty.
            if (pcChecker.Text != "")
            {
                // If it isn't, does it start with '$'?
                if (pcChecker.Text.StartsWith("$"))
                {
                    // If so, load our Render Pos from the user's inputted Program Counter, after converting it from a String to a uint16, also
                    // taking into consideration the fact it's a Hexadecimal Number.
                    renderPos = (ushort)Convert.ToUInt16(UInt16.Parse(pcChecker.Text.Substring(1), System.Globalization.NumberStyles.HexNumber));
                }
                // If it doesn't...
                else
                {
                    // Just set our Render Pos to the user's inputted Program Counter, converted to a Denary Unsigned 16-bit Integer.
                    renderPos = (ushort)Convert.ToUInt16(pcChecker.Text);
                }

                // Render our Render Data from our new Render Pos.
                Render();
            }
        }

        /* When the Stop Button is Clicked... */
        private void stopButton_Click(object sender, EventArgs e)
        {
            // Ensure the CPU Aborts On Errors. This is needed because we're about to cause an error to abort it.
            cpu.abortOnError = true;
            // Set our Program Counter to the end of the CPU's Memory, which should force the emulation to end.
            cpu.registers.PC = ushort.MaxValue - 1;
        }

        /* When the Toggle Preview button is clicked... */
        private void togglePreview_Click(object sender, EventArgs e)
        {
            // Are we rendering the Program Code Data?
            if (renderingPrg)
            {
                // If so, we're not anymore...
                renderingPrg = false;

                // Because our Render Data is now the PPU's Graphics Data!
                renderData = ppu.mem.memory;
                // Render our new Render Data.
                Render();
                return;
            }
            // Are we rendering the Graphics Data?
            else
            {
                // If so, we're not anymore...
                renderingPrg = true;

                // Because our Render Data is now the CPU'S Program Code Data!
                renderData = cpu.mem.memory;
                // Render our new Render Data.
                Render();
                return;
            }
        }

        /* When our Registers Button is Clicked... */
        private void registersButton_Click(object sender, EventArgs e)
        {
            // Update the Registers TextBox to display all of our Registers, in both Denary and Hex formats.
            registersBox.Text = "A: " + cpu.registers.A + "/" + RomParser.HexToStr(cpu.registers.A) + "\nX: " + cpu.registers.X + "/" + RomParser.HexToStr(cpu.registers.X) + " | Y: " + cpu.registers.Y + "/" + RomParser.HexToStr(cpu.registers.Y) + "\nPC: " + cpu.registers.PC + "\nS: " + cpu.registers.S + "/" + RomParser.HexToStr(cpu.registers.S);
        }
    }
}
