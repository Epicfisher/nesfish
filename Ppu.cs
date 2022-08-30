namespace NESEmulator
{
    /* Define our PPU Class. Since this Emulator doesn't feature any PPU Implemenation,
     * the only thing stored within the PPU here is it's Graphics Data Memory. */
    class Ppu
    {
        public Memory mem = new Memory();
    }
}
