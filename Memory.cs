using System.Windows.Forms;

namespace NESEmulator
{
    class Memory
    {
        // Define a Reference to a Public Debug RichTextBox. This will be modified outside this class, and appended to within this class.
        public RichTextBox debugBox;

        // Define an Array to hold our entire Memory Map.
        public byte?[] memory;

        /* Our Initialisation Function. Fills our Memory Map array with X Nullable Bytes, with X referring to an integer passed as an argument to the function. */
        public void Init(int length)
        {
            memory = new byte?[length];
        }

        /* The Load Function. This handles the loading of Data from a Start position into our Memory Map Array. */
        public void Load(uint start, byte[] data)
        {
            // For each byte of Data we're Loading...
            for (int i = 0; i < data.Length; i++)
            {
                // Is our start offset + location we want to write Memory to in our Memory Map outside of the Memory Map's Maximum Length?
                if ((start + i) >= memory.Length)
                {
                    // If so, tell the User we couldn't load this byte of data!
                    debugBox.AppendText("<LOAD ERROR> ERROR DURING LOADING DATA @ " + (start + i).ToString() + " DOESN'T FIT IN MAP!!!\n");
                }
                // If our Byte of data fits within our Memory Map...
                else
                {   
                    // Load the Byte into Memory based on our Load Offset + Current data byte index.
                    memory[start + i] = data[i];
                }
            }

            // Let the User know we've Loaded data.
            debugBox.AppendText("Loaded data from " + start.ToString() + " to " + (start + data.Length).ToString() + "\n");
        }

        /* The NES Memory Map. Left here since it's helpful for debugging */
        /*
            0000-07FF = RAM
            0800-1FFF = mirrors of RAM
            2000-2007 = PPU registers accessable from the CPU
            2008-3FFF = mirrors of those same 8 bytes over and over again
            4000-401F = sound channels, joypads, and other IO
            6000-7FFF = cartridge PRG-RAM (if present), or PRG-ROM depending on mapper
            8000-FFFF = cartridge memory, usually ROM.

            0-2047 = RAM
            2048-8191 = Mirrors of RAM
            8192-8199 = PPU Registers accessible from the CPU
            8200-16383 = Mirrors of the NES PPU Registers every 8 bytes
            16384-16415 = Sound channels, Joypads, Other IO
            24576-32767 = Cartridge PRG-RAM (if present), or PRG-ROM depending on mapper
            32768-65535 = Cartridge memory, usually ROM
        */
    }
}
