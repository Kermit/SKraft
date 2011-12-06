using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SKraft
{
    public class Sun
    {
        public float Light { get; private set; }
        public delegate void SunChanged();
        private Timer timer = new Timer(500);
        private bool grow;
        private readonly float day = 120.0f;
        Color skyboxColor = new Color(0, 0, 20);

        public Sun()
        {
            Light = 3;
            grow = true;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public void Draw()
        {
         
            SKraft.Graphics.Clear(skyboxColor);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (grow)
            {
                Light += 12/day;
                skyboxColor.R += (byte)(20 / day);
                skyboxColor.G += (byte)(150 / day);
                skyboxColor.B += (byte)(235 / day);

                if (Light > 15)
                {
                    grow = false;
                }
            }
            else
            {
                Light -= 12 / day;
                skyboxColor.R -= (byte)(20 / day);
                skyboxColor.G -= (byte)(150 / day);
                skyboxColor.B -= (byte)(235 / day);

                if (Light < 3)
                {
                    grow = true;
                }
            }
        }
    }
}
