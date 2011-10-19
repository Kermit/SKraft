using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SKraft.Cubes;

namespace SKraft.MapGen
{
    class MemoryMap
    {
        private class Sector
        {
            internal const byte SizeX = 5;
            internal const byte SizeY = 200;
            internal const byte SizeZ = 5;

            internal byte[, ,] bytes = new byte[SizeX, SizeY, SizeZ];

            /// <summary>
            /// Zwraca cuby w odpowiedniej ilości. From - od jakiego. to - do którego klocka (0 = wszystkie). Multipier - mnożnik - w zależności od sektora
            /// </summary>
            /// <param name="toX"></param>
            /// <param name="toY"></param>
            /// <param name="toZ"></param>
            /// <param name="multipierX"></param>
            /// <param name="multipierZ"></param>
            /// <returns></returns>
            internal Cube[] GetCubes(int fromX, int fromY, int fromZ, int toX, int toY, int toZ, int multipierX, int multipierZ)
            {
                if (toX == 0 || toX > SizeX)
                {
                    toX = SizeX;
                }
                if (toY == 0 || toY > SizeY)
                {
                    toY = SizeY;
                }
                if (toZ == 0 || toZ > SizeZ)
                {
                    toZ = SizeZ;
                }

                if (fromX < 0)
                {
                    fromX = 0;
                }
                if (fromY < 0)
                {
                    fromY = 0;
                }
                if (fromZ < 0)
                {
                    fromZ = 0;
                }

                List<Cube> cubes = new List<Cube>();
                for (int x = fromX; x < toX; ++x)
                {
                    for (int y = fromY; y < toY; ++y)
                    {
                        for (int z = fromZ; z < toZ; ++z)
                        {
                            switch (bytes[x, y, z])
                            {
                                case 1:
                                    cubes.Add(new SampleCube(new Vector3((multipierX * SizeX) + x, y, (multipierZ * SizeZ) + z)));
                                    break;
                            }
                        }
                    }
                }

                return cubes.ToArray();
            }
        }

        private Sector[] sectors = new Sector[9];
        private Vector2 currentSector = new Vector2(0, 0);

        /// <summary>
        /// Wielość jednego sektora
        /// </summary>
        public static readonly Vector3 SectorSize = new Vector3(Sector.SizeX, Sector.SizeY, Sector.SizeZ);

        public MemoryMap()
        {
            sectors[5] = new Sector();
            for (int x = 0; x < SectorSize.X; ++x)
            {
                for (int z = 0; z < SectorSize.Z; ++z)
                {
                    sectors[5].bytes[x, 0, z] = 1;
                }
            }

            sectors[6] = new Sector();
            for (int x = 0; x < SectorSize.X; ++x)
            {
                for (int z = 0; z < SectorSize.Z; ++z)
                {
                    sectors[6].bytes[x, 0, z] = 1;
                }
            }
        }

        public void LoadContent(ContentManager content)
        {
            new SampleCube(Vector3.Zero).LoadContent(content);
        }

        /// <summary>
        /// Oblicza jakie cuby trzeba namalować
        /// </summary>
        /// <param name="playerPos">Pozycja gracza</param>
        /// <param name="cubesLength">Ilość cubów do namalowania w jedną stronę od gracza</param>
        /// <returns>Cuby do namalowania</returns>
        public Cube[] GetDrawingCubes(Vector3 playerPos, int cubesLength)
        {
            List<Cube> cubes = new List<Cube>();

            currentSector.X = (int) (playerPos.X / Sector.SizeX);
            currentSector.Y = (int) (playerPos.Z / Sector.SizeZ);

            //pozycja gracza, w danym sektorze
            Vector2 posInSector = new Vector2(playerPos.X - currentSector.X * Sector.SizeX, playerPos.Z - currentSector.Y * Sector.SizeZ);

            cubes.AddRange(sectors[6].GetCubes((int)(posInSector.X - cubesLength), 0, (int)(posInSector.Y - cubesLength), (int)(posInSector.X + cubesLength),
                        Sector.SizeY, (int)(posInSector.Y + cubesLength), (int)(currentSector.X), (int)(currentSector.Y)));

            //TODO zmienić obliczanie Y
            if (Sector.SizeX - posInSector.X > cubesLength)
            {
                //gracz po prawej stronie ekranu
                cubes.AddRange(sectors[6].GetCubes(0, 0, (int)(posInSector.Y - cubesLength), (int)(Sector.SizeX - posInSector.X + cubesLength),
                        Sector.SizeY, (int)(Sector.SizeY - posInSector.Y + cubesLength), (int)(currentSector.X), (int)(currentSector.Y)));

                if (Sector.SizeY - posInSector.Y > cubesLength)
                {
                    //gracz w prawym dolnym rogu sektora
                    
                }
            }

            return cubes.ToArray();
        }
    }
}
