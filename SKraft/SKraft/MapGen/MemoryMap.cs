using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SKraft.Cameras;
using SKraft.Cubes;
using System.IO;

namespace SKraft.MapGen
{
    class MemoryMap
    {
        private class Sector
        {
            internal const int SizeX = 100;
            internal const int SizeY = 300;
            internal const int SizeZ = 100;
            private const string Path = "\\Maps\\";
            private bool isExist = true;
            public bool IsLoaded { get; private set; }
            internal List<Cube> Cubes { get; set; }
            internal Thread GetCubesThread { get; set; }

            internal byte[, ,] bytes = new byte[SizeX, SizeY, SizeZ];

            internal Sector()
            {
                 Cubes = new List<Cube>();
            }

            /// <summary>
            /// Zwraca cuby w odpowiedniej ilości. From - od jakiego. to - do którego klocka (0 = wszystkie). Multipier - mnożnik - w zależności od sektora
            /// </summary>
            /// <param name="toX"></param>
            /// <param name="toY"></param>
            /// <param name="toZ"></param>
            /// <param name="multipierX"></param>
            /// <param name="multipierZ"></param>
            /// <returns></returns>
            public void GetCubes(int fromX, int fromY, int fromZ, int toX, int toY, int toZ, int multipierX, int multipierZ, bool behind)
            {
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
                                        if (!behind)
                                        {
                                            Vector3 position = new Vector3((multipierX*SizeX) + x, y,
                                                                           (multipierZ*SizeZ) + z);
                                            Vector3 cubePosPlayer = Vector3.Transform(position, Camera.ActiveCamera.View);
                                            if (cubePosPlayer.Z < -1) //jeśli cube bedzie przed kamerą to wyświetlać
                                            {
                                                Cubes.Add(new SampleCube(position));
                                            }
                                        }
                                        else
                                        {
                                            Vector3 position = new Vector3((multipierX * SizeX) + x, y,
                                                                           (multipierZ * SizeZ) + z);
                                            Cubes.Add(new SampleCube(position));
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
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

                    bw = new BinaryWriter(new FileStream(System.Environment.CurrentDirectory + Path + mapName + "\\" + mapName + sectorX + sectorY + ".sec", FileMode.Create));

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
                int slowing = (int) argsArray[3];

                for (int i = 0; i < 5; ++i) //5-ciokrotna próba wczytania pliku
                {
                    BinaryReader br = null;
                    try
                    {
                        if (!Directory.Exists(System.Environment.CurrentDirectory + Path + mapName + "\\"))
                        {
                            Directory.CreateDirectory(System.Environment.CurrentDirectory + Path + mapName + "\\");
                        }

                        if (!File.Exists(System.Environment.CurrentDirectory + Path + mapName + "\\" + mapName + sectorX + sectorY + ".sec"))
                        {
                            isExist = false;
                            IsLoaded = true;
                            return;
                        }

                        br = new BinaryReader(new FileStream(System.Environment.CurrentDirectory + Path + mapName + "\\" + mapName + sectorX + sectorY +
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
                            Thread.Sleep(slowing);
                        }

                        i = 5;
                        IsLoaded = true;
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(1000);
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

            /// <summary>
            /// Ładuje sektor w wątku
            /// </summary>
            /// <param name="mapName">Nazwa mapy</param>
            /// <param name="sectorX">Ktory to sektor - x</param>
            /// <param name="sectorY">Który to sektor - y</param>
            /// <param name="slowing">Zwolnienie wczytywania - aby nie ścinało w trakcie gry</param>
            public void LoadSectorThread(string mapName, int sectorX, int sectorY, int slowing)
            {
                IsLoaded = false;
                object[] argsArray = new object[] { mapName, sectorX, sectorY, slowing };
                object args = argsArray;

                Thread thread = new Thread(LoadSector);
                thread.IsBackground = true;
                if (slowing > 0)
                {
                    thread.Priority = ThreadPriority.Lowest;
                }
                thread.Start(args);
            }
        }

        private Sector[] sectors = new Sector[9];
        private Vector2 currentSector = new Vector2(0, 0);
        private bool threading;
        public bool Loading { get; private set; }

        /// <summary>
        /// Wielość jednego sektora
        /// </summary>
        public static readonly Vector3 SectorSize = new Vector3(Sector.SizeX, Sector.SizeY, Sector.SizeZ);

        public MemoryMap(Vector3 playerPos, bool threading)
        {
            this.threading = threading;
            /*sectors[4] = new Sector();
            for (int x = 0; x < SectorSize.X; ++x)
            {
                for (int z = 0; z < SectorSize.Z; ++z)
                {
                    /*if (x == (int)(SectorSize.X / 2) && z == (int)(SectorSize.Z / 2))
                    {
                        continue;
                    }
                    sectors[4].bytes[x, 0, z] = 1;
                }
            }
            
            sectors[4].SaveSector("Test", 0, 0);
            
            for (int i = 0; i < sectors.Length; ++i)
            {
                if (i != 4)
                {
                    sectors[i] = new Sector();
                    sectors[i].LoadSectorThread("Test", 0, 0, 0);
                }
            }*/

            currentSector.X = (int)(playerPos.X / Sector.SizeX);
            currentSector.Y = (int)(playerPos.Z / Sector.SizeZ);

            sectors[4] = new Sector();
            sectors[4].LoadSectorThread("Test", (int)currentSector.X, (int)currentSector.Y, 0);
            //Ładowanie sektorów
            for (int y = 0; y < 3; ++y)
            {
                for (int x = 0; x < 3; ++x)
                {
                    if (x % 3 + 3 * y != 4)
                    {
                        sectors[x%3 + 3*y] = new Sector();
                        sectors[x % 3 + 3 * y].LoadSectorThread("Test", (int)currentSector.X + x - 1, (int)currentSector.Y + y - 1, 0);
                    }
                }
            }

            Loading = true;
        }

        public void LoadContent(ContentManager content)
        {
            new SampleCube(Vector3.Zero).LoadContent(content);
        }

        private Cube[] GetCubes(Vector3 playerPos, int cubesLength, bool behind)
        {
            List<Cube> cubes = new List<Cube>();

            for (int i = 0; i < sectors.Length; ++i)
            {
                if (sectors[i] != null)
                {
                    sectors[i].Cubes = new List<Cube>();
                    sectors[i].GetCubesThread = null;
                }
            }

            Loading = false;
            for (int i = 0; i < sectors.Length; ++i)
            {
                if (!sectors[i].IsLoaded)
                {
                    Loading = true;
                    break;
                }
            }
 
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
                    currentSector.X = sectorX;
                    ChangeSectors(true, true);
                }
                else
                {
                    currentSector.X = sectorX;
                    ChangeSectors(false, true);
                }
            }

            if (sectorY != currentSector.Y)
            {
                if (sectorY > currentSector.Y)
                {
                    currentSector.Y = sectorY;
                    ChangeSectors(true, false);
                }
                else
                {
                    currentSector.Y = sectorY;
                    ChangeSectors(false, false);
                }
            }

            //pozycja gracza, w danym sektorze
            Vector2 posInSector = new Vector2(playerPos.X - currentSector.X * Sector.SizeX, playerPos.Z - currentSector.Y * Sector.SizeZ);

            //sektor środkowy
            if (sectors[4] != null)
            {
                sectors[4].GetCubes((int)posInSector.X - cubesLength, (int)playerPos.Y - cubesLength,
                                                   (int)posInSector.Y - cubesLength,
                                                   (int)posInSector.X + cubesLength + 1,
                                                   (int)playerPos.Y + cubesLength, (int)posInSector.Y + cubesLength + 1,
                                                   (int)currentSector.X, (int)currentSector.Y, behind);
            }

            //TODO zmienić obliczanie Y
            if (Sector.SizeX - posInSector.X < cubesLength)
            {
                //gracz po prawej stronie ekranu
                if (sectors[5] != null)
                {
                    sectors[5].GetCubes(0, (int)playerPos.Y - cubesLength, (int)posInSector.Y - cubesLength,
                                                       cubesLength - Sector.SizeX + (int)posInSector.X + 1,
                                                       (int)playerPos.Y + cubesLength, (int)posInSector.Y + cubesLength + 1,
                                                       (int)currentSector.X + 1, (int)currentSector.Y, behind);
                }

                if (sectors[8] != null)
                {
                    if (Sector.SizeZ - posInSector.Y < cubesLength)
                    {
                        //gracz w prawym dolnym rogu sektora
                        sectors[8].GetCubes(0, (int)playerPos.Y - cubesLength, 0,
                                                           cubesLength - Sector.SizeX + (int) posInSector.X + 1,
                                                           (int)playerPos.Y + cubesLength,
                                                           cubesLength - Sector.SizeZ + (int) posInSector.Y + 1,
                                                           (int)currentSector.X + 1, (int)currentSector.Y + 1, behind);
                    }
                }

                if (posInSector.Y < cubesLength)
                {
                    if (sectors[2] != null)
                    {
                        //gracz w lewym górnym rogu sektora
                        sectors[2].GetCubes(0, (int)playerPos.Y - cubesLength, Sector.SizeZ - (cubesLength - (int)posInSector.Y),
                                                          cubesLength - Sector.SizeX + (int)posInSector.X + 1,
                                                          (int)playerPos.Y + cubesLength,
                                                          0,
                                                          (int)currentSector.X + 1, (int)currentSector.Y - 1, behind);
                    }
                }
            }

           
                if (Sector.SizeZ - posInSector.Y < cubesLength)
                {
                    //gracz na dole
                    if (sectors[7] != null)
                    {
                        sectors[7].GetCubes((int)posInSector.X - cubesLength, (int)playerPos.Y - cubesLength, 0,
                                                           (int) posInSector.X + cubesLength + 1,
                                                           (int)playerPos.Y + cubesLength,
                                                           cubesLength - Sector.SizeZ + (int) posInSector.Y + 1,
                                                           (int)currentSector.X, (int)currentSector.Y + 1, behind);
                    }

                    if (posInSector.X < cubesLength)
                    {
                        if (sectors[6] != null)
                        {
                            //gracz w lewym dolnym rogu sektora
                            sectors[6].GetCubes(Sector.SizeX - (cubesLength - (int)posInSector.X), (int)playerPos.Y - cubesLength, 0,
                                                                0,
                                                                (int)playerPos.Y + cubesLength,
                                                                cubesLength - Sector.SizeZ + (int)posInSector.Y + 1,
                                                                (int)currentSector.X - 1, (int)currentSector.Y + 1, behind);
                        }
                    }
                }

            if (posInSector.X < cubesLength)
            {
                //gracz po lewej
                if (sectors[3] != null)
                {
                    sectors[3].GetCubes(Sector.SizeX - cubesLength + (int)posInSector.X, (int)playerPos.Y - cubesLength, (int)posInSector.Y - cubesLength,
                                                       0,
                                                       (int)playerPos.Y + cubesLength, (int)posInSector.Y + cubesLength + 1,
                                                       (int)currentSector.X - 1, (int)currentSector.Y, behind);
                }

                if (sectors[0] != null)
                {
                    if (posInSector.Y < cubesLength)
                    {
                        //gracz w lewym górnym rogu sektora
                        sectors[0].GetCubes(Sector.SizeX - cubesLength + (int)posInSector.X, (int)playerPos.Y - cubesLength,
                                                           Sector.SizeZ - cubesLength + (int) posInSector.Y,
                                                           0,
                                                           (int)playerPos.Y + cubesLength, 0,
                                                           (int)currentSector.X - 1, (int)currentSector.Y - 1, behind);
                    }
                }
            }

            if (sectors[1] != null)
            {
                if (posInSector.Y < cubesLength)
                {
                    //gracz u góry
                    sectors[1].GetCubes((int)posInSector.X - cubesLength, (int)playerPos.Y - cubesLength,
                                                       Sector.SizeZ - cubesLength + (int) posInSector.Y,
                                                       (int) posInSector.X + cubesLength + 1,
                                                       (int)playerPos.Y + cubesLength, 0,
                                                       (int)currentSector.X, (int)currentSector.Y - 1, behind);
                }
            }

            for (int i = 0; i < sectors.Length; ++i)
            {
                if (sectors[i] != null)
                {
                    if (threading && sectors[i].GetCubesThread != null)
                    {
                        sectors[i].GetCubesThread.Join();
                    }
                    cubes.AddRange(sectors[i].Cubes);
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

                    sectors[2] = new Sector();
                    sectors[5] = new Sector();
                    sectors[8] = new Sector();
                    sectors[2].LoadSectorThread("Test", (int)currentSector.X + 1, (int)currentSector.Y - 1, 20);
                    sectors[5].LoadSectorThread("Test", (int)currentSector.X + 1, (int)currentSector.Y, 10);
                    sectors[8].LoadSectorThread("Test", (int)currentSector.X + 1, (int)currentSector.Y + 1, 20);
                }
                else
                {
                    sectors[2] = sectors[1];
                    sectors[5] = sectors[4];
                    sectors[8] = sectors[7];

                    sectors[1] = sectors[0];
                    sectors[4] = sectors[3];
                    sectors[7] = sectors[6];

                    sectors[0] = new Sector();
                    sectors[3] = new Sector();
                    sectors[6] = new Sector();
                    sectors[0].LoadSectorThread("Test", (int)currentSector.X - 1, (int)currentSector.Y - 1, 20);
                    sectors[3].LoadSectorThread("Test", (int)currentSector.X - 1, (int)currentSector.Y, 10);
                    sectors[6].LoadSectorThread("Test", (int)currentSector.X - 1, (int)currentSector.Y + 1, 20);
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

                    sectors[6] = new Sector();
                    sectors[7] = new Sector();
                    sectors[8] = new Sector();
                    sectors[6].LoadSectorThread("Test", (int)currentSector.X - 1, (int)currentSector.Y + 1, 20);
                    sectors[7].LoadSectorThread("Test", (int)currentSector.X, (int)currentSector.Y + 1, 10);
                    sectors[8].LoadSectorThread("Test", (int)currentSector.X + 1, (int)currentSector.Y + 1, 20);
                }
                else
                {
                    sectors[6] = sectors[3];
                    sectors[7] = sectors[4];
                    sectors[8] = sectors[5];

                    sectors[3] = sectors[0];
                    sectors[4] = sectors[1];
                    sectors[5] = sectors[2];

                    sectors[0] = new Sector();
                    sectors[1] = new Sector();
                    sectors[2] = new Sector();
                    sectors[0].LoadSectorThread("Test", (int)currentSector.X - 1, (int)currentSector.Y - 1, 20);
                    sectors[1].LoadSectorThread("Test", (int)currentSector.X, (int)currentSector.Y - 1, 10);
                    sectors[2].LoadSectorThread("Test", (int)currentSector.X + 1, (int)currentSector.Y - 1, 20);
                }
            }
        }

        /// <summary>
        /// Oblicza jakie cuby trzeba namalować
        /// </summary>
        /// <param name="playerPos">Pozycja gracza</param>
        /// <param name="cubesLength">Ilość cubów do namalowania w jedną stronę od gracza</param>
        /// <returns>Cuby do namalowania</returns>
        public Cube[] GetDrawingCubes(Vector3 playerPos, int cubesLength)
        {
            return GetCubes(playerPos, cubesLength, false);
        }

        public Cube[] GetNearestCubes(Vector3 position)
        {
            return GetCubes(position, 2, true);
        }
    }
}
