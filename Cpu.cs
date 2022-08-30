using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace NESEmulator
{
    // Create our Opcode Class. This will store the Opcode Byte, the Action Function which will be called to Run the Opcode, the Opcode's AddressingMode and the number of
    // CPU Cycles our Opcode will take up.
    class Opcode
    {
        public byte opcode;
        public Action action;
        public AddressingModes addressingMode;
        public byte cycles;

        public bool pageBoundary;

        public Opcode(byte _opcode = 0xEA, Action _action = null, AddressingModes _addressingMode = AddressingModes.Implied, byte _cycles = 2, bool _pageboundary = false)
        {
            opcode = _opcode;
            action = _action;
            addressingMode = _addressingMode;
            cycles = _cycles;

            pageBoundary = _pageboundary;
        }
    }

    // Create our enum of different CPU Opcode Addressing Modes.
    enum AddressingModes
    {
        Implied,
        Accumulator,
        Immediate,
        ZeroPage,
        ZeroPageX,
        ZeroPageY,
        Relative,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        Indirect,
        IndirectX,
        IndirectY
    }

    // The AddressingMode Definition for each AddressingMode. Stores the AddressingMode Reference, the number of Bytes each
    // AddressingMode is (or the amount of arguments it has) and the Action Function which will be called to initialise the
    // AddressingMode.
    class AddressingMode
    {
        public AddressingModes addressingMode;
        public byte bytes;
        public Action action;

        public AddressingMode(AddressingModes _addressingMode = AddressingModes.Implied, byte _bytes = 1, Action _action = null)
        {
            addressingMode = _addressingMode;
            bytes = _bytes;
            action = _action;
        }
    }

    // Define our Flags class. This contains every Flag within the NES.
    class Flags
    {
        public bool N = false; // Negative Flag
        public bool V = false; // Overflow Flag
        public bool B2 = true; // Break Command (2)
        public bool B1 = false; // Break Command (1)
        public bool D = false; // Decimal Mode Flag
        public bool I = true; // Interrupt Disable
        public bool Z = false; // Zero Flag
        public bool C = false; // Carry Flag        
    }

    // Define our Registers class. This contains every Register within the NES.
    class Registers
    {
        public byte A = 0; // Accumulator

        public byte X = 0; // X Register (General Purpose)
        public byte Y = 0; // Y Register (General Purpose)

        public ushort PC = 0; // Program Counter

        public byte S = 0xFD; // Stack Pointer
    }

    // Define an Extension Function for our Form's RichTextBoxes.
    public static class RichTextBoxExtensions
    {
        // Create a new Function, "AppendTextInvoked". This takes a String argument.
        public static void AppendTextInvoked(this RichTextBox box, string text)
        {
            /* Invoke Actions from the Box. This runs code within the UI Thread, and not the
             * background one, as Winform Controls can only be modified through the UI Thread. 
             This Appends Text, Selects the End of the Box and then Scrolls to it. */
            box.Invoke(new Action(() => box.AppendText(text)));
            box.Invoke(new Action(() => box.Select(box.Text.Length, 0)));
            box.Invoke(new Action(() => box.ScrollToCaret()));
        }
    }

    // Define our CPU Class.
    class Cpu
    {
        // Define our lists of Global Opcodes and Addressing Modes/
        Opcode[] opcodes;
        AddressingMode[] addressingModes;

        // Create this CPU's Memory.
        public Memory mem = new Memory();

        // Create this CPU's Flags and Registers.
        public Flags flags = new Flags();
        public Registers registers = new Registers();

        // Define a Variable to keep track of how many CPU Cycles we've ran.
        uint cycleCounter = 0;

        // Create references to our two Debug Boxes within Form1 we'll be writing to.
        public RichTextBox debugBox;
        public RichTextBox debugBox2;

        // Should Emulator Abort on Error?
        public bool abortOnError = true;

        // Displays Simple Debug Messages
        public bool debug = true;
        // Displays the Standardised Debug Messages for Comparison with other Emulators
        public bool debug2 = true;
        // Displays More Detailed Rubtime Debug Messages
        public bool bigDebug = false;
        // Displays Opcode Runtime Debug Messages
        public bool CPUDebug = true;
        // Wait for a short period of time after Opcode execution. This slows down performance enough to allow
        // the Stop button to receive presses over the otherwise UI TextBox Spam.
        public bool debugWait = false;

        Opcode opcode = new Opcode(); // Current Global Opcode
        byte[] arguments; // Current Global Opcode's Arguments

        byte input; // Current Global Addressing Input. This is most likely the Byte @ A Memory Address
        ushort output; // Current Global Addressing Output. This is most likely the Memory Address point to A Byte

        /* Initialise our CPU. */
        public void Init()
        {
            // This initialises our Program Counter to the short stored within the memory's Reset Vector. This details where the Program Counter Starts.
            registers.PC = (ushort)((mem.memory[0xFFFC]) | mem.memory[0xFFFD] << 8);
        }

        /* Push a Byte to the Stack. */
        void PushB(byte value)
        {
            if (CPUDebug)
            {
                debugBox.AppendTextInvoked("[STACK-PUSHB] Pushing Byte " + value.ToString() + "/$" + RomParser.HexToStr(value) + " Starting @ $01" + RomParser.HexToStr(registers.S) + "\n");
            }

            // Store our Value at 0x01 + the Stack Pointer, which is 0x01FD by default.
            mem.memory[(ushort)(0x01 << 8 | registers.S)] = value;
            // Decrement our Stack Pointer to point to a new empty location.
            registers.S--;
        }

        /* Push a Word to the Stack. */
        void PushW(ushort value)
        {
            if (CPUDebug)
            {
                debugBox.AppendTextInvoked("[STACK-PUSHW] Pushing Word " + value.ToString() + " Starting @ $01" + RomParser.HexToStr(registers.S) + "\n");
            }

            // Store the first 8 bits of our Short in the Stack Pointer.
            mem.memory[(ushort)(0x01 << 8 | registers.S)] = (byte)(value >> 8);
            // Store the last 8 bits of our Short in the Stack Pointer.
            mem.memory[(ushort)(0x01 << 8 | registers.S - 1)] = (byte)(value & 0xff);
            // Decrement our Stack Pointer to point to a new empty location.
            registers.S -= 2;
        }

        /* Pull a Byte from the Stack. */
        byte PullB()
        {
            // Define a Byte to hold our pulled Stack Byte. This accesses 0x01 + the Stack Pointer + 1, because the Stack Pointer will always point to the available space next to the last Byte.
            byte value = (byte)mem.memory[(ushort)(0x01 << 8 | registers.S + 1)];
            if (CPUDebug)
            {
                debugBox.AppendTextInvoked("[STACK-PULLB] Pulling Byte " + value.ToString() + "/$" + RomParser.HexToStr(value) + " Starting @ $01" + RomParser.HexToStr((byte)(registers.S + 1)) + "\n");
            }
            // Increment our Stack Pointer to point to the next previous location.
            registers.S++;
            // Return our Pulled Byte.
            return value;
        }

        /* Pull a Word from the Stack. */
        ushort PullW()
        {
            // Define a Ushort to hold our pulled Stack Word. This pulls the first 8 bits and then the next 8 bits, before combining them into a 16 bit ushort.
            ushort value = (ushort)((byte)mem.memory[(ushort)0x01 << 8 | registers.S + 1] | (byte)mem.memory[(ushort)0x01 << 8 | (registers.S + 2)] << 8);
            if (CPUDebug)
            {
                debugBox.AppendTextInvoked("[STACK-PULLW] Pulling Word " + value.ToString() + " Starting @ $01" + RomParser.HexToStr((byte)(registers.S + 1)) + "\n");
            }
            // Increment our Stack Pointer to point to the next previous location.
            registers.S += 2;
            // Return our Pulled Byte.
            return value;
        }

        /* Set our Processor Flags to a Stack Byte */
        void PullSetProcessorStatus()
        {
            // Pull the Byte from the Stack.
            byte flagsByte = PullB();

            /* Set our CPU's Flags to the values of the individual bits from our Stack Byte */
            flags.C = (flagsByte & (1 << 0)) != 0;
            flags.Z = (flagsByte & (1 << 1)) != 0;
            flags.I = (flagsByte & (1 << 2)) != 0;
            flags.D = (flagsByte & (1 << 3)) != 0;
            /* Bits 4 & 5 are Not Affected */
            flags.V = (flagsByte & (1 << 6)) != 0;
            flags.N = (flagsByte & (1 << 7)) != 0;

            if (CPUDebug)
            {
                debugBox.AppendTextInvoked("[STACK-PULLS] C Flag = " + flags.C + "\n");
                debugBox.AppendTextInvoked("[STACK-PULLS] Z Flag = " + flags.Z + "\n");
                debugBox.AppendTextInvoked("[STACK-PULLS] I Flag = " + flags.I + "\n");
                debugBox.AppendTextInvoked("[STACK-PULLS] D Flag = " + flags.D + "\n");
                debugBox.AppendTextInvoked("[STACK-PULLS] B1 Flag = " + flags.B1 + "\n");
                debugBox.AppendTextInvoked("[STACK-PULLS] B2 Flag = " + flags.B2 + "\n");
                debugBox.AppendTextInvoked("[STACK-PULLS] V Flag = " + flags.V + "\n");
                debugBox.AppendTextInvoked("[STACK-PULLS] N Flag = " + flags.N + "\n");

            }
        }

        /* Run our Program! */
        public void Run()
        {
            // Define a Bool that represents whether or not we've Handled the current Opcode.
            bool handled_opcode = false;
            // Let the User know we're Now Running!
            debugBox.AppendTextInvoked("Now Running!\n");

            // Create a reference to our current Opcode's Addressing Mode.
            AddressingMode addressingMode = new AddressingMode();
            // Run through the Memory using our Program Counter.
            for (; registers.PC < mem.memory.Length; registers.PC++)
            {
                // If we're Debug Waiting...
                if (debugWait)
                {
                    // Wait 1 Millisecond on our Thread.
                    Thread.Sleep(1);
                }
                // As of right now we haven't handled our Opcode.
                handled_opcode = false;

                /* Debug Information that displays what Opcode we're current looking at. */
                if (bigDebug)
                {
                    if (mem.memory[registers.PC] == null)
                    {
                        debugBox.AppendTextInvoked("Looking at NULL. Counter " + cycleCounter + "\n");
                    }
                    else
                    {
                        debugBox.AppendTextInvoked("Looking at " + RomParser.HexToStr((byte)mem.memory[registers.PC]) + ". Counter " + cycleCounter + "\n");
                    }
                }

                // Increment the number of Cycles our CPU has run.
                cycleCounter++;                

                // If our current Opcode is null...
                if (mem.memory[registers.PC] == null)
                {
                    // Let the User know and Return.
                    MessageBox.Show("Missing Null Area Error @ PC " + registers.PC.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                // If it isn't null...
                else
                {
                    // Has our Program Counter left the Memory Map?
                    if (registers.PC > mem.memory.Length)
                    {
                        // If so, we've finished execution! Let the User know and return.
                        debugBox.AppendTextInvoked("Finished!");
                        return;
                    }

                    // Run through every available Opcode.
                    for (int i = 0; i < opcodes.Length; i++)
                    {
                        // Is our current Opcode at our Program Counter equal to the current Opcode in the array at i?
                        if ((byte)mem.memory[registers.PC] == opcodes[i].opcode)
                        {
                            // We've found our opcode. Reflect this, since we're about to handle it.
                            handled_opcode = true;
                            // Update our Global Opcode to the current Opcode.
                            opcode = opcodes[i];

                            /* This Loop runs through every available Addressing Mode. */
                            for (int ii = 0; ii < addressingModes.Length; ii++)
                            {
                                // Is our current Opcode's Addressing Mode equal to the current Addressing Mode in the array at ii?
                                if (opcodes[i].addressingMode == addressingModes[ii].addressingMode)
                                {
                                    // Update our global AddressingMode to reflect our new AddressingMode.
                                    addressingMode = addressingModes[ii];
                                }
                            }

                            // Clear our Arguments Array.
                            arguments = null;

                            try
                            {
                                // If our current AddressingMode takes Arguments...
                                if (addressingMode.bytes > 1)
                                {
                                    // Create a new Arguments array with the size of the current AddressingMode's Bytes Length - 1
                                    arguments = new byte[addressingMode.bytes - 1];

                                    // Cycle through all of our AddressingMode Bytes - 1, aka cycle through all of our Arguments
                                    for (int ii = 0; ii < addressingMode.bytes - 1; ii++)
                                    {
                                        // Add this Argument to our Arguments array, from the new values after our Opcode within our Memory Map.
                                        arguments[ii] = (byte)mem.memory[registers.PC + 1 + ii];
                                    }
                                }
                            }
                            // If there was an error creating our Arguments array...
                            catch
                            {
                                // Return, or stop execution.
                                return;
                            }

                            /* Debug Code to display our Arguments within our Debug Logs. */
                            if (debug)
                            {
                                // Create a blank argumentsString String.
                                string argumentsString = "";
                                // If we have Arguments beyond our Opcode...
                                if (addressingMode.bytes - 1 > 0)
                                {
                                    // Our Arguments String is going to start with a Bracket now, to hold our Arguments.
                                    argumentsString = " (";
                                    // Loop through our Arguments.
                                    for (int ii = 0; ii < addressingMode.bytes - 1; ii++)
                                    {
                                        // Add our Argument to the end of our Arguments string, converted into a Hexadecimal String.
                                        argumentsString += RomParser.HexToStr(arguments[ii]);
                                        // If we're not at the end of our Arguments yet...
                                        if (ii < addressingMode.bytes - 2)
                                        {
                                            // Add a Space. This will ensure there isn't a trailing space left when we finish in our loop.
                                            argumentsString += " ";
                                        }
                                    }
                                    // Close our Arguments String Bracket.
                                    argumentsString += ")";
                                }

                                // Debug. Displays Opcode Information and Program Counter Information.
                                debugBox.Invoke(new Action(() => debugBox.AppendTextInvoked("      Executing opcode " + RomParser.HexToStr(opcodes[i].opcode) + argumentsString + " @ " + registers.PC.ToString() + "/$" + registers.PC.ToString("X") + " PC, " + addressingMode.bytes.ToString() + " bytes & " + opcodes[i].cycles.ToString() + " cycles\n")));
                            }

                            // Skips our Program Counter ahead of the opcode Arguments
                            registers.PC += (ushort)(addressingMode.bytes - 1);

                            /* Displays Standardised Debug Logs */
                            if (debug2)
                            {
                                // Define our empty Arguments String
                                string argumentsString = " ";
                                // If we have arguments...
                                if (addressingMode.bytes - 1 > 0)
                                {
                                    // Loop through our Arguments.
                                    for (int ii = 0; ii < addressingMode.bytes - 1; ii++)
                                    {
                                        // Add our current Argument to the end of our Arguments string, converted into a Hexadecimal String.
                                        argumentsString += RomParser.HexToStr(arguments[ii]) + " ";
                                    }
                                }
                                // Loop through our remaining spaces between our Arguments and the next section after displaying our Arguments.
                                for (int ii = 0; ii < (6 - ((addressingMode.bytes - 1) * 2) - (addressingMode.bytes - 1)); ii++)
                                {
                                    // Keep adding spaces until the Arguments Section is a constant length with every other Arguments section, regardless of the number of Arguments.
                                    argumentsString += " ";
                                }
                                // Add a Tabulator at the end of our Arguments String.
                                argumentsString += "\t";
                                // Debug. Displays our Opcode, it's Arguments and our Registers + Flag Byte.
                                debugBox2.AppendTextInvoked((registers.PC - addressingMode.bytes + 1).ToString("X") + "  " + RomParser.HexToStr(opcodes[i].opcode) + argumentsString + "??????\t\t\t\tA:" + RomParser.HexToStr(registers.A) + " X:" + RomParser.HexToStr(registers.X) + " Y:" + RomParser.HexToStr(registers.Y) + " P:" + RomParser.HexToStr(RomParser.BoolsToByte(new bool[] { flags.C, flags.Z, flags.I, flags.D, flags.B1, flags.B2, flags.V, flags.N })) + " SP:" + RomParser.HexToStr(registers.S) + "\n");
                            }
                            // Run our AddressMode, which grabs the needed information from our Arguments.
                            addressingMode.action();
                            // Run our Opcode.
                            opcodes[i].action();
                            // Break from our loop of trying to find the valid Opcode, since we've just Executed it!
                            break;
                        }
                    }
                }

                // If we haven't handled the Opcode...
                if (!handled_opcode)
                {
                    // Call an Opcode Error.
                    OpcodeError();
                    // If we're aborting on error...
                    if (abortOnError)
                        // Abort.
                        return;
                }
            }
        }

        /* Our CPU Class. */
        public Cpu()
        {
            // Define our List of Opcodes. This will be translated into an Array to fit into our Global Opcodes Array after it has been populated.
            List<Opcode> _opcodes = new List<Opcode>();
            // Define our List of AddressingModes. This will be translated into an Array to fit into our Global AddressingModes Array after it has been populated.
            List<AddressingMode> _addressingModes = new List<AddressingMode>();

            /* Update our Zero and Negative Flags based on Register Values. This is called by several Opcodes. */
            void UpdateFlags(string opcodeDebug, byte register)
            {
                // If the supplied value is Zero...
                if (register == 0)
                {
                    // Set our Zero flag to True.
                    flags.Z = true;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[" + opcodeDebug + "] Z Flag = True\n");
                    }
                }
                // If it isn't Zero...
                else
                {
                    // Set our Zero flag to False.
                    flags.Z = false;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[" + opcodeDebug + "] Z Flag = False\n");
                    }
                }

                // If the 7th bit of our value is set, aka it's a negative value...
                if ((register & (1 << 7)) != 0)
                {
                    // Set our Negative flag to True.
                    flags.N = true;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[" + opcodeDebug + "] N Flag = True\n");
                    }
                }
                // If our value isn't negative...
                else
                {
                    // Set our Negative flag to False.
                    flags.N = false;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[" + opcodeDebug + "] N Flag = False\n");
                    }
                }
            }

            /* Add all of our AddressingModes into the temporary List. This include every AddressingMode's AddressingMode, Byte Length/Arguments and the Action it should run to grab these Arguments. */
            _addressingModes.Add(new AddressingMode(AddressingModes.Implied, 1, Implied));
            _addressingModes.Add(new AddressingMode(AddressingModes.Accumulator, 1, Accumulator));
            _addressingModes.Add(new AddressingMode(AddressingModes.Immediate, 2, Immediate));
            _addressingModes.Add(new AddressingMode(AddressingModes.ZeroPage, 2, ZeroPage));
            _addressingModes.Add(new AddressingMode(AddressingModes.ZeroPageX, 2, ZeroPageX));
            _addressingModes.Add(new AddressingMode(AddressingModes.ZeroPageY, 2, ZeroPageY));
            _addressingModes.Add(new AddressingMode(AddressingModes.Relative, 2, Relative));
            _addressingModes.Add(new AddressingMode(AddressingModes.Absolute, 3, Absolute));
            _addressingModes.Add(new AddressingMode(AddressingModes.AbsoluteX, 3, AbsoluteX));
            _addressingModes.Add(new AddressingMode(AddressingModes.AbsoluteY, 3, AbsoluteY));
            _addressingModes.Add(new AddressingMode(AddressingModes.Indirect, 3, Indirect));
            _addressingModes.Add(new AddressingMode(AddressingModes.IndirectX, 2, IndirectX));
            _addressingModes.Add(new AddressingMode(AddressingModes.IndirectY, 2, IndirectY));

            /* Define the Functions that will run in order to grab the Input and Output values from the Arguments of every AddressingMode. */
            void Implied() { input = 0; output = 0; }
            void Accumulator() { input = 0; output = 0; }
            void Immediate() { input = arguments[0]; output = 0; }
            void ZeroPage() { input = (byte)mem.memory[arguments[0]]; output = arguments[0]; }
            void ZeroPageX() { /* TODO */ OpcodeError(); }
            void ZeroPageY() { /* TODO */ OpcodeError(); }
            void Relative() { input = 0; output = Convert.ToUInt16((uint)((uint)registers.PC + (sbyte)arguments[0])); }
            void Absolute() { input = (byte)mem.memory[(ushort)((arguments[0]) | arguments[1] << 8)]; output = (ushort)((arguments[0]) | arguments[1] << 8); }
            void AbsoluteX() { input = (byte)((byte)mem.memory[(ushort)((arguments[0]) | arguments[1] << 8)] + registers.X); output = 0; }
            void AbsoluteY() { /* TODO */ OpcodeError(); }
            void Indirect() { /* TODO */ OpcodeError(); }
            void IndirectX() { input = (byte)mem.memory[(ushort)((mem.memory[(byte)((arguments[0] + registers.X) & 0xFF)]) | (mem.memory[((byte)((arguments[0] + registers.X) & 0xFF) + 1) & 0xFF]) << 8)]; output = (ushort)((mem.memory[(byte)((arguments[0] + registers.X) & 0xFF)]) | (mem.memory[((byte)((arguments[0] + registers.X) & 0xFF) + 1) & 0xFF]) << 8); }
            void IndirectY() { /* TODO */ OpcodeError(); }

            addressingModes = _addressingModes.ToArray();

            /* Define all of our CPU Instructions, or Opcodes. */

            // No Operation
            _opcodes.Add(new Opcode(0xEA, NOP, AddressingModes.Implied, 2));
            void NOP() { /* Return. Do nothing. */ return; }

            // Jump
            _opcodes.Add(new Opcode(0x4C, JMP, AddressingModes.Absolute, 3));
            _opcodes.Add(new Opcode(0x6C, JMP, AddressingModes.Indirect, 5));
            void JMP()
            {
                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[JMP] JMPing to: " + ((ushort)(output - 1)).ToString() + "/" + RomParser.HexToStr((byte)mem.memory[(ushort)(output - 1)]) + "\n");
                }
                // Jump to our Output Memory Location.
                registers.PC = (ushort)(output - 1);
            }

            // Jump to Subroutine
            _opcodes.Add(new Opcode(0x20, JSR, AddressingModes.Absolute, 6));
            void JSR()
            {
                // Push our Program Counter as a Word to the Stack.
                PushW(registers.PC);

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[JSR] JMPing to: " + ((ushort)(output - 1)).ToString() + "/" + RomParser.HexToStr((byte)mem.memory[(ushort)(output - 1)]) + "\n");
                }
                // Jump to our Output Memory Location.
                registers.PC = (ushort)(output - 1);
            }

            // Return from Subroutine
            _opcodes.Add(new Opcode(0x60, RTS, AddressingModes.Implied, 6));
            void RTS()
            {
                // Set our Program Counter to a Pulled Word from the Stack.
                registers.PC = PullW();

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[RTS] Returning from Subroutine into PC: " + registers.PC.ToString() + "\n");
                }
            }

            // Return from Interrupt
            _opcodes.Add(new Opcode(0x40, RTI, AddressingModes.Implied, 6));
            void RTI()
            {
                // Set our Flags equal to a pulled Byte from the Stack.
                PullSetProcessorStatus();

                // Set our Program Counter equal to the next pulled Word from our Stack.
                registers.PC = (ushort)(PullW() - 1);

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[RTI] Returning from Interrupt into PC: " + registers.PC.ToString() + "\n");
                }
            }

            // Push Processor Status
            _opcodes.Add(new Opcode(0x08, PHP, AddressingModes.Implied, 3));
            void PHP()
            {
                // Create an array of Booleans representing the current status of our Flags, with each Flag being stored as a boolean within the array.
                bool[] s = new bool[] { flags.C, flags.Z, flags.I, flags.D, true, true, flags.V, flags.N };

                // Push our Boolean Array converted into a Byte into our Stack.
                PushB(RomParser.BoolsToByte(s));
            }

            // Pull Processor Status
            _opcodes.Add(new Opcode(0x28, PLP, AddressingModes.Implied, 4));
            void PLP()
            {
                // Set our Flags equal to a pulled Byte from the Stack.
                PullSetProcessorStatus();
            }

            // Push Accumulator
            _opcodes.Add(new Opcode(0x48, PHA, AddressingModes.Implied, 3));
            void PHA()
            {
                // Push our Accumulator Register to the Stack as a Byte.
                PushB(registers.A);
            }

            // Pull Accumulator
            _opcodes.Add(new Opcode(0x68, PLA, AddressingModes.Implied, 4));
            void PLA()
            {
                // Set our Accumulator equal to the next pulled Byte from the Stack.
                registers.A = PullB();

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[PLA] A Register = " + registers.A + " From Stack\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("PLA", registers.A);
            }

            // Logical AND
            _opcodes.Add(new Opcode(0x29, AND, AddressingModes.Immediate, 3));
            _opcodes.Add(new Opcode(0x25, AND, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x35, AND, AddressingModes.ZeroPageX, 4));
            _opcodes.Add(new Opcode(0x2D, AND, AddressingModes.Absolute, 4));
            _opcodes.Add(new Opcode(0x3D, AND, AddressingModes.AbsoluteX, 4));
            _opcodes.Add(new Opcode(0x39, AND, AddressingModes.AbsoluteY, 4));
            _opcodes.Add(new Opcode(0x21, AND, AddressingModes.IndirectX, 6));
            _opcodes.Add(new Opcode(0x31, AND, AddressingModes.IndirectY, 5));
            void AND()
            {
                // Set our Accumulator equal to the result of a bitwise AND operation with our Input Byte. This sets the Accumulator equal to a byte where only both bits from the old Accumulator and our Input have matched up.
                registers.A &= input;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[AND] A Register = " + registers.X + " After Logical Anding With " + input + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("AND", registers.A);
            }

            // Logical Inclusive OR
            _opcodes.Add(new Opcode(0x09, ORA, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0x05, ORA, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x15, ORA, AddressingModes.ZeroPageX, 4));
            _opcodes.Add(new Opcode(0x0D, ORA, AddressingModes.Absolute, 4));
            _opcodes.Add(new Opcode(0x1D, ORA, AddressingModes.AbsoluteX, 4));
            _opcodes.Add(new Opcode(0x19, ORA, AddressingModes.AbsoluteY, 4));
            _opcodes.Add(new Opcode(0x01, ORA, AddressingModes.IndirectX, 6));
            _opcodes.Add(new Opcode(0x11, ORA, AddressingModes.IndirectY, 5));
            void ORA()
            {
                // Set our Accumulator equal to the result of a bitwise Inclusive OR operation with our Input Byte. This sets the Accumulator equal to a byte where either bits from the old Accumulator or our Input were 1.
                registers.A |= input;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[ORA] A Register = " + registers.X + " After Logical Inclusive Oring With " + input + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("ORA", registers.A);
            }

            // Exclusive OR
            _opcodes.Add(new Opcode(0x49, EOR, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0x45, EOR, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x55, EOR, AddressingModes.ZeroPageX, 4));
            _opcodes.Add(new Opcode(0x4D, EOR, AddressingModes.Absolute, 4));
            _opcodes.Add(new Opcode(0x5D, EOR, AddressingModes.AbsoluteX, 4));
            _opcodes.Add(new Opcode(0x59, EOR, AddressingModes.AbsoluteY, 4));
            _opcodes.Add(new Opcode(0x41, EOR, AddressingModes.IndirectX, 6));
            _opcodes.Add(new Opcode(0x51, EOR, AddressingModes.IndirectY, 5));
            void EOR()
            {
                // Set our Accumulator equal to the result of a bitwise Exclusive OR operation with our Input Byte. This sets the Accumulator equal to a byte where either bits from the old Accumulator or our Input were 1, but not both.
                registers.A ^= input;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[EOR] A Register = " + registers.X + " After Exclusive Oring With " + input + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("EOR", registers.A);
            }

            // Increment Memory
            _opcodes.Add(new Opcode(0xE6, INC, AddressingModes.ZeroPage, 5));
            _opcodes.Add(new Opcode(0xF6, INC, AddressingModes.ZeroPageX, 6));
            _opcodes.Add(new Opcode(0xEE, INC, AddressingModes.Absolute, 6));
            _opcodes.Add(new Opcode(0xFE, INC, AddressingModes.AbsoluteX, 7));
            void INC()
            {
                // Increment the Memory Location at our Output Memory Location by 1.
                mem.memory[output] += 1;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[INC] " + output + " in Memory += 1 = " + mem.memory[output] + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("INC", (byte)mem.memory[output]);
            }

            // Increment X Register
            _opcodes.Add(new Opcode(0xE8, INX, AddressingModes.Implied, 2));
            void INX()
            {
                // Increment our X Register by 1.
                registers.X += 1;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[INX] X Register += 1 = " + registers.X + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("INX", registers.X);
            }

            // Increment Y Register
            _opcodes.Add(new Opcode(0xC8, INY, AddressingModes.Implied, 2));
            void INY()
            {
                // Increment our Y Register by 1.
                registers.Y += 1;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[INY] Y Register += 1 = " + registers.Y + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("INY", registers.Y);
            }

            // Bit Test
            _opcodes.Add(new Opcode(0x24, BIT, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x2C, BIT, AddressingModes.Absolute, 4));
            void BIT()
            {
                /* Performs a variety of bit-level tests. The Z Flag is set if result of an AND between the Byte Input and the Accumulator is 0.
                 * The Negative Flag is set to the 7th bit of the Input Byte.
                 The Overflow Flag is set to the 6th bit of the Input Byte. */
                flags.Z = !Convert.ToBoolean(input & registers.A);
                flags.N = Convert.ToBoolean(input & (1 << 7));
                flags.V = Convert.ToBoolean(input & (1 << 6));

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[BIT] Z Flag = " + flags.Z.ToString() + "\n");
                    debugBox.AppendTextInvoked("[BIT] N Flag = " + flags.N.ToString() + "\n");
                    debugBox.AppendTextInvoked("[BIT] V Flag = " + flags.V.ToString() + "\n");
                }
            }

            // Subtract with Carry
            _opcodes.Add(new Opcode(0xE9, SBC, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0xE5, SBC, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0xE1, SBC, AddressingModes.IndirectX, 6));
            void SBC()
            {
                // Invert all of the bits of our Input. This essentially treats an Add as if it were a Subtract, since we're adding an inverted Input.
                input = (byte)~input;

                // Create an Output and set it to our Accumulator + our Input to Subtract. The Carry Bit is also included if the Carry Flag is True.
                int output = (sbyte)registers.A + (sbyte)((byte)input) + (sbyte)(flags.C ? 1 : 0);
                // The Overflow Flag is set to whether the Output is overflowing either positively or negatively.
                flags.V = output < -128 || output > 127;
                // The Carry Flag is set to whether or not there's an overflow after subtracting the Accumulator, the Input Byte and the Carry Flag.
                flags.C = (registers.A + (byte)input + (flags.C ? 1 : 0)) > 0xFF;
                // The Accumulator is finally set to the result of the Output, converted into a Byte. The 0xFF here ensures only the lowest 8 bits of
                // the output int are kept, which ensures only the 8 bits we want from the int are converted into the 8 bites within the Byte.
                registers.A = (byte)(output & 0xFF);

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[SBC] V Flag = " + flags.V + "\n");
                    debugBox.AppendTextInvoked("[SBC] C Flag = " + flags.C + "\n");
                    debugBox.AppendTextInvoked("[SBC] A Register = " + registers.A + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("SBC", registers.A);
            }

            // Add with Carry
            _opcodes.Add(new Opcode(0x69, ADC, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0x65, ADC, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x61, ADC, AddressingModes.IndirectX, 6));
            void ADC()
            {
                // Create an Output and set it to our Accumulator + our Input to Add. The Carry Bit is also included if the Carry Flag is True.
                int output = (sbyte)registers.A + (sbyte)((byte)input) + (sbyte)(flags.C ? 1 : 0);
                // The Overflow Flag is set to whether the Output is overflowing either positively or negatively.
                flags.V = output < -128 || output > 127;
                // The Carry Flag is set to whether or not there's an overflow after adding the Accumulator, the Input Byte and the Carry Flag.
                flags.C = (registers.A + (byte)input + (flags.C ? 1 : 0)) > 0xFF;
                // The Accumulator is finally set to the result of the Output, converted into a Byte. The 0xFF here ensures only the lowest 8 bits of
                // the output int are kept, which ensures only the 8 bits we want from the int are converted into the 8 bites within the Byte.
                registers.A = (byte)(output & 0xFF);

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[ADC] V Flag = " + flags.V + "\n");
                    debugBox.AppendTextInvoked("[ADC] C Flag = " + flags.C + "\n");
                    debugBox.AppendTextInvoked("[ADC] A Register = " + registers.A + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("ADC", registers.A);
            }

            // Compare
            _opcodes.Add(new Opcode(0xC9, CMP, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0xC5, CMP, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0xCD, CMP, AddressingModes.Absolute, 4));
            _opcodes.Add(new Opcode(0xC1, CMP, AddressingModes.IndirectX, 6));
            void CMP()
            {
                // Create our Comparison. The Comparison is created by subtracting our Byte Input from our Accumulator, which should only leave us with the differences within the "comparison" byte.
                byte comparison = (byte)(registers.A - input);

                // The Carry Flag is set to whether or not the Accumulator is greater than or equal to the Input Byte.
                flags.C = registers.A >= input;
                // The Zero Flag is set to whether or not the Accumulator is equal to the Input Byte.
                flags.Z = registers.A == input;
                // The Negative Flag is set to whether or not the 7th bit of the Comparison result is set.
                flags.N = (comparison & (1 << 7)) != 0;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[CMP] C Flag = " + flags.C + "\n");
                    debugBox.AppendTextInvoked("[CMP] Z Flag = " + flags.Z + "\n");
                    debugBox.AppendTextInvoked("[CMP] N Flag = " + flags.N + "\n");
                }
            }

            // Compare X Register
            _opcodes.Add(new Opcode(0xE0, CPX, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0xE4, CPX, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0xEC, CPX, AddressingModes.Absolute, 4));
            void CPX()
            {
                // Create our Comparison. The Comparison is created by subtracting our Byte Input from our X Register, which should only leave us with the differences within the "comparison" byte.
                byte comparison = (byte)(registers.X - input);

                // The Carry Flag is set to whether or not the X Register is greater than or equal to the Input Byte.
                flags.C = registers.X >= input;
                // The Zero Flag is set to whether or not the X Register is equal to the Input Byte.
                flags.Z = registers.X == input;
                // The Negative Flag is set to whether or not the 7th bit of the Comparison result is set.
                flags.N = (comparison & (1 << 7)) != 0;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[CPX] C Flag = " + flags.C + "\n");
                    debugBox.AppendTextInvoked("[CPX] Z Flag = " + flags.Z + "\n");
                    debugBox.AppendTextInvoked("[CPX] N Flag = " + flags.N + "\n");
                }
            }

            // Compare Y Register
            _opcodes.Add(new Opcode(0xC0, CPY, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0xC4, CPY, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0xCC, CPY, AddressingModes.Absolute, 4));
            void CPY()
            {
                // Create our Comparison. The Comparison is created by subtracting our Byte Input from our Y Register, which should only leave us with the differences within the "comparison" byte.
                byte comparison = (byte)(registers.Y - input);

                // The Carry Flag is set to whether or not the Y Register is greater than or equal to the Input Byte.
                flags.C = registers.Y >= input;
                // The Zero Flag is set to whether or not the Y Register is equal to the Input Byte.
                flags.Z = registers.Y == input;
                // The Negative Flag is set to whether or not the 7th bit of the Comparison result is set.
                flags.N = (comparison & (1 << 7)) != 0;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[CPY] C Flag = " + flags.C + "\n");
                    debugBox.AppendTextInvoked("[CPY] Z Flag = " + flags.Z + "\n");
                    debugBox.AppendTextInvoked("[CPY] N Flag = " + flags.N + "\n");
                }
            }

            // Decrement X Register
            _opcodes.Add(new Opcode(0xCA, DEX, AddressingModes.Implied, 2));
            void DEX()
            {
                // Decrement our X Register by 1.
                registers.X--;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[DEX] X Register = " + registers.X + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("DEX", registers.X);
            }

            // Decrement Y Register
            _opcodes.Add(new Opcode(0x88, DEY, AddressingModes.Implied, 2));
            void DEY()
            {
                // Decrement our Y Register by 1.
                registers.Y--;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[DEY] Y Register = " + registers.Y + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("DEY", registers.Y);
            }

            // Load Accumulator
            _opcodes.Add(new Opcode(0xA9, LDA, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0xA5, LDA, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0xAD, LDA, AddressingModes.Absolute, 4));
            _opcodes.Add(new Opcode(0xBD, LDA, AddressingModes.AbsoluteX, 4));
            _opcodes.Add(new Opcode(0xA1, LDA, AddressingModes.IndirectX, 6));
            _opcodes.Add(new Opcode(0xB1, LDA, AddressingModes.IndirectY, 5));
            void LDA()
            {
                // Load an Input Byte and set it to our Accumulator Byte.
                registers.A = input;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[LDA] A Register = " + input + "/" + RomParser.HexToStr(input) + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("LDA", registers.A);
            }

            // Load X Register
            _opcodes.Add(new Opcode(0xA2, LDX, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0xA6, LDX, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0xAE, LDX, AddressingModes.Absolute, 4));
            void LDX()
            {
                // Load an Input Byte and set it to our X Register Byte.
                registers.X = input;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[LDX] X Register = " + input + "/" + RomParser.HexToStr(input) + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("LDX", registers.X);
            }

            // Load Y Register
            _opcodes.Add(new Opcode(0xA0, LDY, AddressingModes.Immediate, 2));
            _opcodes.Add(new Opcode(0xA4, LDY, AddressingModes.ZeroPage, 3));
            void LDY()
            {
                // Load an Input Byte and set it to our Y Register Byte.
                registers.Y = input;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[LDY] Y Register = " + input + "/" + RomParser.HexToStr(input) + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("LDY", registers.Y);
            }

            // Clear Carry Flag
            _opcodes.Add(new Opcode(0x18, CLC, AddressingModes.Implied, 2));
            void CLC()
            {
                // Set the Carry Flag to False.
                flags.C = false;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[CLC] C Flag = False\n");
                }
            }

            // Clear Decimal Flag
            _opcodes.Add(new Opcode(0xD8, CLD, AddressingModes.Implied, 2));
            void CLD()
            {
                // Set the Decimal Flag to False.
                flags.D = false;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[CLD] D Flag = False\n");
                }
            }

            // Clear Interrupt Disable
            _opcodes.Add(new Opcode(0x58, CLI, AddressingModes.Implied, 2));
            void CLI()
            {
                // Set the Interrupt Disable Flag to False, effectively allowing Interrupts.
                flags.I = false;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[CLI] I Flag = False\n");
                }
            }

            // Clear Overflow Flag
            _opcodes.Add(new Opcode(0xB8, CLV, AddressingModes.Implied, 2));
            void CLV()
            {
                // Set the Overflow Flag to False.
                flags.V = false;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[CLV] V Flag = False\n");
                }
            }

            // Set Carry Flag
            _opcodes.Add(new Opcode(0x38, SEC, AddressingModes.Implied, 2));
            void SEC()
            {
                // Set the Clear Flag to True.
                flags.C = true;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[SEC] C Flag = True\n");
                }
            }

            // Set Decimal Flag
            _opcodes.Add(new Opcode(0xF8, SED, AddressingModes.Implied, 2));
            void SED()
            {
                // Set the Decimal Flag to True.
                flags.D = true;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[SED] D Flag = True\n");
                }
            }

            // Set Interrupt Disable
            _opcodes.Add(new Opcode(0x78, SEI, AddressingModes.Implied, 2));
            void SEI()
            {
                // Set the Interrupt Disable Flag to true, effectively disallowing Interrupts.
                flags.I = true;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[SEI] I Flag = True\n");
                }
            }

            // Store Accumulator
            _opcodes.Add(new Opcode(0x85, STA, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x8D, STA, AddressingModes.Absolute, 4));
            _opcodes.Add(new Opcode(0x81, STA, AddressingModes.IndirectX, 6));
            void STA()
            {
                // Store our Accumulator at our Output Memory Address Location.
                mem.memory[output] = registers.A;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[STA] A Register = " + registers.A + "/" + RomParser.HexToStr(registers.A) + " Stored @ " + output + "\n");
                }
            }

            // Store X Register
            _opcodes.Add(new Opcode(0x86, STX, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x96, STX, AddressingModes.ZeroPageY, 4));
            _opcodes.Add(new Opcode(0x8E, STX, AddressingModes.Absolute, 4));
            void STX()
            {
                // Store our X Register at our Output Memory Address Location.
                mem.memory[output] = registers.X;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[STX] X Register = " + registers.X + "/" + RomParser.HexToStr(registers.X) + " Stored @ " + output + "\n");
                }
            }

            // Store Y Register
            _opcodes.Add(new Opcode(0x84, STY, AddressingModes.ZeroPage, 3));
            _opcodes.Add(new Opcode(0x8C, STY, AddressingModes.Absolute, 4));
            void STY()
            {
                // Store our Y Register at our Output Memory Address Location.
                mem.memory[output] = registers.Y;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[STY] Y Register = " + registers.Y + "/" + RomParser.HexToStr(registers.Y) + " Stored @ " + output + "\n");
                }
            }

            // Transfer Accumulator to X
            _opcodes.Add(new Opcode(0xAA, TAX, AddressingModes.Implied, 2));
            void TAX()
            {
                // Transfer our Accumulator over to our X Register.
                registers.X = registers.A;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[TAX] X Register = A Register = " + registers.X + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("TAX", registers.X);
            }

            // Transfer Accumulator to Y
            _opcodes.Add(new Opcode(0xA8, TAY, AddressingModes.Implied, 2));
            void TAY()
            {
                // Transfer our Accumulator over to our Y Register.
                registers.Y = registers.A;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[TAY] Y Register = A Register = " + registers.Y + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("TAY", registers.Y);
            }

            // Transfer Stack Pointer to X
            _opcodes.Add(new Opcode(0xBA, TSX, AddressingModes.Implied, 2));
            void TSX()
            {
                // Transfer our Stack Pointer Register over to our X Register.
                registers.X = registers.S;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[TSX] X Register = S Register = " + registers.X + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("TSX", registers.X);
            }

            // Transfer X to Accumulator
            _opcodes.Add(new Opcode(0x8A, TXA, AddressingModes.Implied, 2));
            void TXA()
            {
                // Transfer our X Register over to our Accumulator.
                registers.A = registers.X;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[TXA] A Register = X Register = " + registers.A + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("TXA", registers.A);
            }

            // Transfer Y to Accumulator
            _opcodes.Add(new Opcode(0x98, TYA, AddressingModes.Implied, 2));
            void TYA()
            {
                // Transfer our Y Register over to our Accumulator.
                registers.A = registers.Y;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[TYA] A Register = Y Register = " + registers.A + "\n");
                }

                // Update our Zero and Negative flags.
                UpdateFlags("TYA", registers.A);
            }

            // Transfer X to Stack Pointer
            _opcodes.Add(new Opcode(0x9A, TXS, AddressingModes.Implied, 2));
            void TXS()
            {
                // Transfer our X Register over to our Stack Pointer Register.
                registers.S = registers.X;

                if (CPUDebug)
                {
                    debugBox.AppendTextInvoked("[TXS] S Register = X Register = " + registers.S + "\n");
                }
            }

            // Rotate Left
            _opcodes.Add(new Opcode(0x2A, ROL, AddressingModes.Accumulator, 2));
            void ROL()
            {
                // If we're operating on the Accumulator...
                if (opcode.opcode == 0x2A)
                {
                    // Store the old 7th bit of our Accumulator.
                    bool old7 = Convert.ToBoolean(registers.A & (1 << 7));

                    // Bit-shift on the Accumulator 1 to the left.
                    registers.A <<= 1;

                    // Fill bit 0 of our Accumulator with the current value of the Carry Flag.
                    registers.A = Convert.ToByte(registers.A | ((flags.C ? 1 : 0) << 0));

                    // Set our Carry Flag to our old 7th Accumulator bit.
                    flags.C = old7;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[ROL] A Register = " + registers.A + "\n[ROL] C Flag = " + flags.C.ToString() + "\n");
                    }

                    // Update our Zero and Negative flags.
                    UpdateFlags("ROL", registers.A);
                }
            }

            // Arithmetic Shift Left
            _opcodes.Add(new Opcode(0x0A, ASL, AddressingModes.Accumulator, 2));
            void ASL()
            {
                // If we're operating on the Accumulator...
                if (opcode.opcode == 0x0A)
                {
                    // Store the 7th bit of our Accumulator in our Carry Flag.
                    flags.C = Convert.ToBoolean(registers.A & (1 << 7));

                    // Bit-shift on the accumulator 1 to the left.
                    registers.A <<= 1;

                    // Set bit 0 of our Accumulator to 0.
                    registers.A = Convert.ToByte(registers.A | (0 << 0));

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[ASL] A Register = " + registers.A + "\n[ASL] C Flag = " + flags.C.ToString() + "\n");
                    }

                    // Update our Zero and Negative flags.
                    UpdateFlags("ASL", registers.A);
                }
            }

            // Rotate Right
            _opcodes.Add(new Opcode(0x6A, ROR, AddressingModes.Accumulator, 2));
            void ROR()
            {
                // If we're operating on the Accumulator...
                if (opcode.opcode == 0x6A)
                {
                    // Store the old 0th (first) bit of our Accumulator.
                    bool old0 = Convert.ToBoolean(registers.A & (1 << 0));

                    // Bit-shift on the accumulator 1 to the right.
                    registers.A >>= 1;

                    // Fill bit 7 of our Accumulator with the current value of the Carry Flag.
                    registers.A = Convert.ToByte(registers.A | ((flags.C ? 1 : 0) << 7));

                    // Set our Carry Flag to our old 0th (first) Accumulator bit.
                    flags.C = old0;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[ROR] A Register = " + registers.A + "\n[ROR] C Flag = " + flags.C.ToString() + "\n");
                    }

                    // Update our Zero and Negative flags.
                    UpdateFlags("ROR", registers.A);
                }
            }

            // Logical Shift Right
            _opcodes.Add(new Opcode(0x4A, LSR, AddressingModes.Accumulator, 2));
            void LSR()
            {
                // If we're operating on the Accumulator...
                if (opcode.opcode == 0x4A)
                {
                    // Store the 0th (first) bit of our Accumulator in our Carry Flag.
                    flags.C = Convert.ToBoolean(registers.A & (1 << 0));

                    // Bit-shift on the accumulator 1 to the right.
                    registers.A >>= 1;

                    // Set bit 7 of our Accumulator to 0.
                    registers.A = Convert.ToByte(registers.A | (0 << 7));

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[LSR] A Register = " + registers.A + "\n[LSR] C Flag = " + flags.C.ToString() + "\n");
                    }

                    // Update our Zero and Negative flags.
                    UpdateFlags("LSR", registers.A);
                }
            }

            // Branch if Minus
            _opcodes.Add(new Opcode(0x30, BMI, AddressingModes.Relative, 2));
            void BMI()
            {
                // If the Negative Flag is Set...
                if (flags.N)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BMI] Negative Flag Set. Branching to " + (registers.PC + 1) + " due to Branch if Minus.\n");
                    }
                }
                // If it isn't Set, do nothing but debug the fact we didn't branch because it wasn't set.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BMI] Negative Flag is Not Set. Branch if Minus Not Branching.\n");
                    }
                }
            }

            // Branch if Not Equal
            _opcodes.Add(new Opcode(0xD0, BNE, AddressingModes.Relative, 2));
            void BNE()
            {
                // If the Zero Flag is Clear...
                if (!flags.Z)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BNE] Zero Flag Clear. Branching to " + (registers.PC + 1) + " due to Branch if Not Equal.\n");
                    }
                }
                // If it isn't Clear, do nothing but debug the fact we didn't branch because it wasn't clear.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BNE] Zero Flag is Not Clear. Branch if Not Equal Not Branching.\n");
                    }
                }
            }

            // Branch if Equal
            _opcodes.Add(new Opcode(0xF0, BEQ, AddressingModes.Relative, 2));
            void BEQ()
            {
                // If the Zero Flag is Set...
                if (flags.Z)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BEQ] Zero Flag Set. Branching to " + (registers.PC + 1) + " due to Branch if Equal.\n");
                    }
                }
                // If it isn't Set, do nothing but debug the fact we didn't branch because it wasn't set.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BEQ] Zero Flag is Not Set. Branch if Equal Not Branching.\n");
                    }
                }
            }

            // Branch if Positive
            _opcodes.Add(new Opcode(0x10, BPL, AddressingModes.Relative, 2));
            void BPL()
            {
                // If the Negative Flag is Clear...
                if (!flags.N)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BPL] Negative Flag Clear. Branching to " + (registers.PC + 1) + " due to Branch if Positive.\n");
                    }
                }
                // If it isn't Clear, do nothing but debug the fact we didn't branch because it wasn't clear.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BPL] Negative Flag is Not Clear. Branch if Positive Not Branching.\n");
                    }
                }
            }

            // Branch if Carry Set
            _opcodes.Add(new Opcode(0xB0, BCS, AddressingModes.Relative, 2));
            void BCS()
            {
                // If the Carry Flag is Set...
                if (flags.C)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BCS] Carry Flag Set. Branching to " + (registers.PC + 1) + " due to Branch if Carry Set.\n");
                    }
                }
                // If it isn't Set, do nothing but debug the fact we didn't branch because it wasn't set.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BCS] Carry Flag is Not Set. Branch if Carry Set Not Branching.\n");
                    }
                }
            }

            // Branch if Carry Clear
            _opcodes.Add(new Opcode(0x90, BCC, AddressingModes.Relative, 2));
            void BCC()
            {
                // If the Carry Flag is Clear...
                if (!flags.C)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BCC] Carry Flag Clear. Branching to " + (registers.PC + 1) + " due to Branch if Overflow Set.\n");
                    }
                }
                // If it isn't Clear, do nothing but debug the fact we didn't branch because it wasn't clear.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BCC] Carry Flag is Not Clear. Branch if Carry Clear Not Branching.\n");
                    }
                }
            }

            // Branch if Overflow Clear
            _opcodes.Add(new Opcode(0x50, BVC, AddressingModes.Relative, 2));
            void BVC()
            {
                // If the Overflow Flag is Clear...
                if (!flags.V)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BVC] Overflow Flag Clear. Branching to " + (registers.PC + 1) + " due to Branch if Overflow Clear.\n");
                    }
                }
                // If it isn't Clear, do nothing but debug the fact we didn't branch because it wasn't clear.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BVC] Overflow Flag is Not Clear. Branch if Overflow Clear Not Branching.\n");
                    }
                }
            }

            // Branch if Overflow Set
            _opcodes.Add(new Opcode(0x70, BVS, AddressingModes.Relative, 2));
            void BVS()
            {
                // If the Overflow Flag is Set...
                if (flags.V)
                {
                    // Branch to the Output Memory Address Location!
                    registers.PC = output;

                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BVS] Overflow Flag Set. Branching to " + (registers.PC + 1) + " due to Branch if Overflow Set.\n");
                    }
                }
                // If it isn't Set, do nothing but debug the fact we didn't branch because it wasn't set.
                else
                {
                    if (CPUDebug)
                    {
                        debugBox.AppendTextInvoked("[BVS] Overflow Flag is Not Set. Branch if Overflow Set Not Branching.\n");
                    }
                }
            }

            /* The End of our Opcode Definitions. */

            // Convert our Opcodes List into an Array to be stored in our Global Opcodes Array.
            opcodes = _opcodes.ToArray();
        }

        /* This function runs if we've encountered an Opcode-related Error. */
        void OpcodeError()
        {
            // Let the User know we've hit an unknown opcode, and at which location.
            debugBox.AppendTextInvoked("FATAL ERROR: UNKNOWN OPCODE " + RomParser.HexToStr((byte)mem.memory[registers.PC]) + " @ " + registers.PC + " / Ran " + cycleCounter.ToString() + " Cycles");
            // Are we not aborting on error?
            if (!abortOnError)
            {
                // If we're not, add a New Line to the Fatal Error message, since it won't be the last!
                debugBox.AppendTextInvoked("\n");
                // Return out of our OpcodeError Function.
                return;
            }
            // If we are aborting on error. set our Program Counter to the end of the Memory Map, which should end all execution.
            registers.PC = ushort.MaxValue - 1;
        }
    }
}
