using System.Collections.Generic;

namespace NESEmulator
{
    /* Define our Header Class, with every Header Property as a Byte.
     * Comments indicate what location in the ROM these bytes should
     * be loaded from */
    class Header
    {
        public byte _PRG_ROM; // 4
        public byte _CHR_ROM; // 5
        public byte _MAPPER; // 6
        public byte _MAPPER2; // 7

        /* RARELY USED */
        public byte _PRG_RAM; // 8
        public byte _TV; // 9
        public byte _TV2; // 10
    }

    /* Define our Cart Class. A Cart contains a Header and two Lists
     * for a ROM's Program Code Data and Graphics Data. */
    class Cart
    {
        public Header header = new Header();

        public List<byte> PRGRom = new List<byte>();
        public List<byte> CHRRom = new List<byte>();
    }
}
