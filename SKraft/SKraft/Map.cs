using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SKraft.Cubes;

namespace SKraft
{
    public class Map
    {
        List<Cube> cubes = new List<Cube>();

        public void Initialize()
        {
            for (int x = 0; x < 50; ++x)
            {
                for (int z = 0; z < 30; ++z)
                {
                    cubes.Add(new SampleCube(new Vector3(x, 0, z)));
                }
            }

            for (int z = 10; z < 30; ++z)
            {
                for (int y = 0; y < 5; ++y)
                {
                    cubes.Add(new SampleCube(new Vector3(10, y, z)));
                }
            }
        }

        public void LoadContent(ContentManager content)
        {
            foreach (Cube cube in cubes)
            {
                cube.LoadContent(content);
            }
        }

        public void Draw()
        {
            foreach (Cube cube in cubes)
            {
                cube.Draw();
            }
        }
    }
}
