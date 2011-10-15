using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SKraft
{
    class FpsCounter
    {
        private int elapsed;
        private int frames;
        private int fps;

        public int Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.Milliseconds;

            ++frames;

            if (elapsed >= 1000)
            {
                fps = frames;
                elapsed = 0;
                frames = 0;
            }

            return fps;
        }
    }
}
