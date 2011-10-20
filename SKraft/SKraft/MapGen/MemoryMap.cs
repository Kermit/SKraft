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
            internal const byte SizeX = 100;
            internal const byte SizeY = 200;
            internal const byte SizeZ = 100;

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

        public MemoryMap(Vector3 playerPos)
        {
            sectors[0] = new Sector();
            for (int x = 0; x < SectorSize.X; ++x)
            {
                for (int z = 0; z < SectorSize.Z; ++z)
                {
                    sectors[0].bytes[x, 0, z] = 1;
                }
            }

            for (int i = 1; i < sectors.Length; ++i)
            {
                sectors[i] = new Sector();
                sectors[i] = sectors[0];
            }

            currentSector.X = (int)(playerPos.X / Sector.SizeX);
            currentSector.Y = (int)(playerPos.Z / Sector.SizeZ);
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

            int sectorX, sectorY;
            if (playerPos.X / Sector.SizeX < 0)
            {
                sectorX = (int)(playerPos.X / Sector.SizeX) - 1;
            }
            else
            {
                sectorX = (int)(playerPos.X / Sector.SizeX);
            }

            if (playerPos.Z / Sector.SizeZ < 0)
            {
                sectorY = (int)playerPos.Z / Sector.SizeZ - 1;
            }
            else
            {
                sectorY = (int)playerPos.Z / Sector.SizeZ;
            }

            if (sectorX != currentSector.X)
            {
                if (sectorX > currentSector.X)
                {
                    ChangeSectors(true, true);
                }
                else
                {
                    ChangeSectors(false, true);
                }

                currentSector.X = sectorX;
            }

            if (sectorY != currentSector.Y)
            {
                if (sectorY > currentSector.Y)
                {
                    ChangeSectors(true, false);
                }
                else
                {
                    ChangeSectors(false, false);
                }

                currentSector.Y = sectorY;
            }
            
            
            //Debug.AddString("Sektor: " + currentSector);

            //pozycja gracza, w danym sektorze
            Vector2 posInSector = new Vector2(playerPos.X - currentSector.X * Sector.SizeX, playerPos.Z - currentSector.Y * Sector.SizeZ);

            //sektor środkowy
            if (sectors[4] != null)
            {
                cubes.AddRange(sectors[4].GetCubes((int)posInSector.X - cubesLength, 0,
                                                   (int)posInSector.Y - cubesLength,
                                                   (int)posInSector.X + cubesLength + 1,
                                                   Sector.SizeY, (int)posInSector.Y + cubesLength + 1,
                                                   (int)currentSector.X, (int)currentSector.Y));
            }

            //TODO zmienić obliczanie Y
            if (Sector.SizeX - posInSector.X < cubesLength)
            {
                //gracz po prawej stronie ekranu
                if (sectors[5] != null)
                {
                    cubes.AddRange(sectors[5].GetCubes(0, 0, (int)posInSector.Y - cubesLength,
                                                       cubesLength - Sector.SizeX + (int)posInSector.X + 1,
                                                       Sector.SizeY, (int)posInSector.Y + cubesLength + 1,
                                                       (int)currentSector.X + 1, (int)currentSector.Y));
                }

                if (sectors[8] != null)
                {
                    if (Sector.SizeZ - posInSector.Y < cubesLength)
                    {
                        //gracz w prawym dolnym rogu sektora
                        cubes.AddRange(sectors[8].GetCubes(0, 0, 0,
                                                           cubesLength - Sector.SizeX + (int) posInSector.X + 1,
                                                           Sector.SizeY,
                                                           cubesLength - Sector.SizeZ + (int) posInSector.Y + 1,
                                                           (int) currentSector.X + 1, (int) currentSector.Y + 1));
                    }
                }
            }

            if (sectors[7] != null)
            {
                if (Sector.SizeZ - posInSector.Y < cubesLength)
                {
                    //gracz na dole
                    cubes.AddRange(sectors[7].GetCubes((int) posInSector.X - cubesLength, 0, 0,
                                                       (int) posInSector.X + cubesLength + 1,
                                                       Sector.SizeY,
                                                       cubesLength - Sector.SizeZ + (int) posInSector.Y + 1,
                                                       (int) currentSector.X, (int) currentSector.Y + 1));
                }
            }

            if (posInSector.X < cubesLength)
            {
                //gracz po lewej
                if (sectors[3] != null)
                {
                    cubes.AddRange(sectors[3].GetCubes(Sector.SizeX - cubesLength + (int)posInSector.X, 0, (int)posInSector.Y - cubesLength,
                                                       0,
                                                       Sector.SizeY, (int)posInSector.Y + cubesLength + 1,
                                                       (int)currentSector.X - 1, (int)currentSector.Y));
                }

                if (sectors[0] != null)
                {
                    if (posInSector.Y < cubesLength)
                    {
                        //gracz w lewym dolnym rogu sektora
                        cubes.AddRange(sectors[0].GetCubes(Sector.SizeX - cubesLength + (int) posInSector.X, 0,
                                                           Sector.SizeZ - cubesLength + (int) posInSector.Y,
                                                           0,
                                                           Sector.SizeY, 0,
                                                           (int) currentSector.X - 1, (int) currentSector.Y - 1));
                    }
                }
            }

            if (sectors[1] != null)
            {
                if (posInSector.Y < cubesLength)
                {
                    //gracz u góry
                    cubes.AddRange(sectors[1].GetCubes((int) posInSector.X - cubesLength, 0,
                                                       Sector.SizeZ - cubesLength + (int) posInSector.Y,
                                                       (int) posInSector.X + cubesLength + 1,
                                                       Sector.SizeY, 0,
                                                       (int) currentSector.X, (int) currentSector.Y - 1));
                }
            }

            return cubes.ToArray();
        }

        private void ChangeSectors(bool positive, bool axisX)
        {
            if (axisX)
            {
                if (positive)
                {
                    sectors[0] = sectors[1];
                    sectors[3] = sectors[4];
                    sectors[6] = sectors[7];

                    sectors[1] = sectors[2];
                    sectors[4] = sectors[5];
                    sectors[7] = sectors[8];

                    sectors[2] = null;
                    sectors[5] = null;
                    sectors[8] = null;
                }
                else
                {
                    sectors[2] = sectors[1];
                    sectors[5] = sectors[4];
                    sectors[8] = sectors[7];

                    sectors[1] = sectors[0];
                    sectors[4] = sectors[3];
                    sectors[7] = sectors[6];

                    sectors[0] = null;
                    sectors[3] = null;
                    sectors[6] = null;
                }
            }
            else
            {
                if (positive)
                {
                    sectors[0] = sectors[3];
                    sectors[1] = sectors[4];
                    sectors[2] = sectors[5];

                    sectors[3] = sectors[6];
                    sectors[4] = sectors[7];
                    sectors[5] = sectors[8];

                    sectors[6] = null;
                    sectors[7] = null;
                    sectors[8] = null;
                }
                else
                {
                    sectors[6] = sectors[3];
                    sectors[7] = sectors[4];
                    sectors[8] = sectors[5];

                    sectors[3] = sectors[0];
                    sectors[4] = sectors[1];
                    sectors[5] = sectors[2];

                    sectors[0] = null;
                    sectors[1] = null;
                    sectors[2] = null;
                }
            }
        }
    }
}
