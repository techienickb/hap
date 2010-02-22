using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordVisualizer.Core.Util
{
    /// <summary>
    /// EmuUnit
    /// </summary>
    public static class EmuUnit
    {
        /// <summary>
        /// Convert pixels to EMU
        /// </summary>
        /// <param name="pixels">Size in pixels</param>
        /// <returns>Size in EMU</returns>
        public static int PixelsToEmu(int pixels)
        {
            return (int)Math.Round((decimal)pixels * 9525);
        }

        /// <summary>
        /// Convert EMU to pixels
        /// </summary>
        /// <param name="emu">Size in EMU</param>
        /// <returns>Size in pixels</returns>
        public static int EmuToPixels(int emu)
        {
            if (emu != 0)
            {
                return (int)Math.Round((decimal)emu / 9525);
            }
            else
            {
                return 0;
            }
        }
    }
}
