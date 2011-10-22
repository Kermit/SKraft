using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SKraft.Cubes;
using System.IO;

namespace SKraft.MapGen
{
    class MemoryMap
    {
        private class Sector
        {
            internal const int SizeX = 300;
            internal const int SizeY = 200;
            internal const int SizeZ = 300;
            private const string Path = "\\Maps\\";
            private bool isExist = true;

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
                List<Cube> cubes = new List<Cube>();

                if (isExist)
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

                    for (int x = fromX; x < toX; ++x)
                    {
                        for (int y = fromY; y < toY; ++y)
                        {
                            for (int z = fromZ; z < toZ; ++z)
                            {
                                switch (bytes[x, y, z])
                                {
                                    case 1:
                                        cubes.Add(new SampleCube(new Vector3((multipierX*SizeX) + x, y, (multipierZ*SizeZ) + z)));
                                        break;
                                }
                            }
                        }
                    }
                }
                return cubes.ToArray();
            }

            public void SaveSector(string mapName, int sectorX, int sectorY)
            {
                BinaryWriter bw = null;
                try
                {
                    if (!Directory.Exists(System.Environment.CurrentDirectory + Path + mapName + "\\"))
                    {
                        Directory.CreateDirectory(System.Environment.CurrentDirectory + Path + mapName + "\\");
                    }

                    bw = new BinaryWriter(new FileStream(System.Environment.CurrentDirectory + Path + mapName + "\\" + sectorX + sectorY + ".sec", FileMode.Create));

                    for (int x = 0; x < SizeX; ++x)
                    {
                        for (int y = 0; y < SizeY; ++y)
                        {
                            for (int z = 0; z < SizeZ; ++z)
                            {
                                bw.Write(bytes[x, y, z]);
                            }
                        }
                    }
                }
                finally
                {
                    bw.Flush();
                    bw.Close();
                }
            }

            private void LoadSector(object args)
            {
                object[] argsArray = (object[])args;
                string mapName = (string)argsArray[0];
                int sectorX = (int)argsArray[1];
                int sectorY = (int)argsArray[2];

                for (int i = 0; i < 5; ++i) //5-ciokrotna próba wczytania pliku
                {
                    BinaryReader br = null;
                    try
                    {
                        if (!Directory.Exists(System.Environment.CurrentDirectory + Path + mapName + "\\"))
                        {
                            Directory.CreateDirectory(System.Environment.CurrentDirectory + Path + mapName + "\\");
                        }

                        if (!File.Exists(System.Environment.CurrentDirectory + Path + mapName + "\\" + sectorX + sectorY + ".sec"))
                        {
                            isExist = false;
                            return;
                        }

                        br = new BinaryReader(new FileStream(System.Environment.CurrentDirectory + Path + mapName + "\\" + sectorX + sectorY +
                                    ".sec", FileMode.Open));

                        for (int x = 0; x < SizeX; ++x)
                        {
                            for (int y = 0; y < SizeY; ++y)
                            {
                                for (int z = 0; z < SizeZ; ++z)
                                {
                                    bytes[x, y, z] = br.ReadByte();
                                }
                            }
                        }

                        i = 5;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(50);
                    }
                    finally
                    {
                        if (br != null)
                        {
                            br.Close();
                        }
                    }
                }
            }

            public void LoadSectorThread(string mapName, int sectorX, int sectorY)
            {
                object[] argsArray = new object[] { mapName, sectorX, sectorY };
                object args = argsArray;

                Thread thread = new Thread(LoadSector);
                thread.IsBackground = true;
                thread.Priority = ThreadPriority.Lowest;
                thread.Start(args);
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
                    if (x == (int)(SectorSize.X / 2) && z == (int)(SectorSize.Z / 2))
                    {
                        continue;
                    }
                    sectors[0].bytes[x, 0, z] = 1;
                }
            }

            sectors[0].SaveSector("Test", 0, 0);

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

                if (posInSector.Y < cubesLength)
                {
                    if (sectors[2] != null)
                    {
                        //gracz w lewym prawym górnym rogu sektora
                        cubes.AddRange(sectors[2].GetCubes(0, 0, Sector.SizeZ - (cubesLength - (int)posInSector.Y),
                                                          cubesLength - Sector.SizeX + (int)posInSector.X + 1,
                                                          Sector.SizeY,
                                                          0,
                                                          (int)currentSector.X + 1, (int)currentSector.Y - 1));
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

                    if (posInSector.X < cubesLength)
                    {
                        if (sectors[6] != null)
                        {
                            //gracz w lewym dolnym rogu sektora
                            cubes.AddRange(sectors[6].GetCubes(Sector.SizeX - (cubesLength - (int)posInSector.X), 0, 0,
                                                                0,
                                                                Sector.SizeY,
                                                                cubesLength - Sector.SizeZ + (int)posInSector.Y + 1,
                                                                (int)currentSector.X - 1, (int)currentSector.Y + 1));
                        }
                    }
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
                        //gracz w lewym górnym rogu sektora
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

                    sectors[2].LoadSectorThread("Test", 0, 0);
                    sectors[5].LoadSectorThread("Test", 0, 0);
                    sectors[8].LoadSectorThread("Test", 0, 0);
                }
                else
                {
                    sectors[2] = sectors[1];
                    sectors[5] = sectors[4];
                    sectors[8] = sectors[7];

                    sectors[1] = sectors[0];
                    sectors[4] = sectors[3];
                    sectors[7] = sectors[6];

                    sectors[0].LoadSectorThread("Test", 0, 0);
                    sectors[3].LoadSectorThread("Test", 0, 0);
                    sectors[6].LoadSectorThread("Test", 0, 0);
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

                    sectors[6].LoadSectorThread("Test", 0, 0);
                    sectors[7].LoadSectorThread("Test", 0, 0);
                    sectors[8].LoadSectorThread("Test", 0, 0);
                }
                else
                {
                    sectors[6] = sectors[3];
                    sectors[7] = sectors[4];
                    sectors[8] = sectors[5];

                    sectors[3] = sectors[0];
                    sectors[4] = sectors[1];
                    sectors[5] = sectors[2];

                    sectors[0].LoadSectorThread("Test", 0, 0);
                    sectors[1].LoadSectorThread("Test", 0, 0);
                    sectors[2].LoadSectorThread("Test", 0, 0);
                }
            }
        }
    }
}
