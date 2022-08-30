using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NESEmulator
{
    class RomParser
    {
        /* This function converts a Byte into a Hexadecimal String. */
        public static string HexToStr(byte hex)
        {
            // Format our Hex Byte into Hexadecimal, and then return it.
            return string.Format("{0:X2}", hex);
        }

        /* This function converts a String Hex Byte String back into a Byte. */
        public static byte StrToHex(string hex)
        {
            // Parse our String Hex Byte and try to convert it into a Byte by looking at is as a Hex Number.
            return byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        /* This function converts an array of Booleans into a Byte. */
        public static byte BoolsToByte(bool[] input)
        {
            // Our Result Byte starts off at 0.
            byte result = 0;
            // Loop through all 8 bits of a byte, from 7 to 0.
            for (int i = 7; i > -1; i--)
            {
                // Is our Boolean True or '1' at our current byte bit?
                if (input[i])
                {
                    // If so, this sets the bit within the Result Byte @ position i to 1
                    result |= (byte)(1 << (byte)i);
                }
            }

            // Return our final Result Byte.
            return result;
        }

        /* Open a ROM File and get it's entire Hexadecimal Content. */
        public List<byte> GetHex(string location)
        {
            // Open a FileStream to our File Location String
            FileStream fs = new FileStream(location, FileMode.Open);
            // Create our Int to load Bytes into.
            int hexIn;
            // Create a list of Hex Bytes.
            List<byte> hexes = new List<byte>();

            // Loop through every Byte within the ROM File. Store each byte within our hexIn Int.
            for (int i = 0; (hexIn = fs.ReadByte()) != -1; i++)
            {
                // Add our current hexIn Int Byte (After being converted to a Byte from an Int) to our Hexes List.
                hexes.Add((byte)hexIn);
            }

            // When we're done, close our File Stream.
            fs.Close();

            // Return our Hexes List.
            return hexes;
        }

        /* Create a Cart Object from our ROM Data */
        public Cart CreateCart(List<byte> hexes)
        {
            // Instantiate a new Cart object
            Cart cart = new Cart();

            /* Start building our Cart Header based on the first few Bytes in our ROM's Header. */
            cart.header._PRG_ROM = hexes[4];
            cart.header._CHR_ROM = hexes[5];
            cart.header._MAPPER = hexes[6];
            cart.header._MAPPER2 = hexes[7];

            cart.header._PRG_RAM = hexes[8];
            cart.header._TV = hexes[9];
            cart.header._TV2 = hexes[10];
            /* We're no longer building our Cart Header. */

            // Our ROM's Graphics Data Size is calculated by reading a Propety in our Header and then multiplying it by 0x2000, taken from our mapper, since Mapper
            // 17 contains data in 8kb chunks.
            int CHRROMSize = hexes[5] * 0x2000;

            // Create a Byte array to hold our Graphics Data. Initialise it to be the size required.
            byte[] CHRROM = new byte[CHRROMSize];

            // Copy our Graphics Data the same way we did our Program Code Data. This accesses everything after the Program Code (stored here as the Graphics Data Size
            // since they should both be the same size for this mapper), as Graphics Data is typically stored after the Program Code Data, and copies it into the CHRROM Array.
            Array.Copy(hexes.ToArray(), 16 + CHRROMSize, CHRROM, 0, CHRROMSize);

            // Assign our Program Code Data and Graphic Data to our Cart's PRGRom and CHRRom Lists, after converting them.
            cart.CHRRom = CHRROM.ToList();

            // We do something different with our PRGRom. Since Mapper 17 likes to Mirror our PRGRom into two Memory Banks, I'm manually writing our Program Code Data from 16
            // to a 16kb 0x4000, which is double our 8kb 0x2000 chunks.
            cart.PRGRom = new ArraySegment<byte>(hexes.ToArray(), 0x0010, 0x4000).ToList();

            // Return our Created Cart.
            return cart;
        }
    }
}
