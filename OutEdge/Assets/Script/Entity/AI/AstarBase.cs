using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

public class AstarBase
{

    public class Location
    {
        public int X;
        public int Y;
        public int Z;
        public int F;
        public int G;
        public int H;
        public Location Parent;
    }

    public Location current;

    public Location start;
    public Location target;

    public Stack<Vector3> path = new Stack<Vector3>();
    public bool locked = false;

    /*static void DrawMapToConsole()
    {
        for (var i = 0; i < map.Length; ++i)
        {
            Console.WriteLine(map[i]);
        }

    }*/

    /*static void GenerateMap(int size)
    {
        map = new string[size, size];
        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (x == 1 && y == size - 2)
                    {
                        map[y, z] += "S";
                        continue;
                    }
                    if (x == size - 2 && y == 1)
                    {
                        map[y, z] += "E";
                        continue;
                    }
                    if (x == 0 || y == 0 || x == size - 1 || y == size - 1 || z == 0 || z == size - 1)
                    {
                        map[y, z] += "|";
                        continue;
                    }
                    int ran = random.Next(3);
                    switch (ran)
                    {
                        case 1:
                            map[y, z] += "X";
                            break;
                        default:
                            map[y, z] += " ";
                            break;
                    }

                }
            }
        }
    }*/

    static int size = 128;

    public Thread thread;

    public AstarBase(Vector3 startpos, Vector3 targetpos)
    {
        start = new Location { X = (int)startpos.x, Y = (int)startpos.y, Z = (int)startpos.z };
        target = new Location { X = (int)targetpos.x, Y = (int)targetpos.y, Z = (int)targetpos.z };
    }

    public AstarBase()
    {

    }


    public void StartSearch()
    {
        if(thread == null || !thread.IsAlive)
        {
            thread = new Thread(new ThreadStart(SearchPath));
            thread.Start();
        }
        /*else
        {
            thread.Interrupt();
            thread = new Thread(new ThreadStart(SearchPath));
            thread.Start();
        }*/
    }

    public void ChangeTarget(Vector3 startpos,Vector3 tar)
    {
        target = new Location { X = (int)tar.x, Y = (int)tar.y, Z = (int)tar.z };
        if(thread == null || !thread.IsAlive)
        {
            start = new Location { X = (int)startpos.x, Y = (int)startpos.y, Z = (int)startpos.z };
            thread = new Thread(new ThreadStart(SearchPath));
            thread.Start();
        }
    }

    public void SearchPath()
    {

        //GenerateMap(size);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        //DrawMapToConsole();
        var openList = new List<Location>();
        var closedList = new List<Location>();
        //g is represented length of each step;
        int g = 1;

        // start by adding the original position to the open list
        openList.Add(start);

        while (openList.Count > 0)
        {
            // algorithm's logic goes here
            // get the square with the lowest F score
            var lowest = openList.Min(l => l.F);
            current = openList.First(l => l.F == lowest);

            // add the current square to the closed list
            closedList.Add(current);
            // remove it from the open list
            openList.Remove(current);

            // show current square on the map
            //Console.SetCursorPosition(current.X, current.Y);
            //Console.Write('.');
            //Console.SetCursorPosition(current.X, current.Y);
            ///System.Threading.Thread.Sleep(1000);

            // if we added the destination to the closed list, we've found a path
            if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y && l.Z == target.Z) != null)
            {
                UnityEngine.Debug.LogWarning("Founded");
                break;
            }

            var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, current.Z, TerrainManager.tm);

            foreach (var adjacentSquare in adjacentSquares)
            {
                // if this adjacent square is already in the closed list, ignore it
                if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y && l.Z == adjacentSquare.Z) != null)
                    continue;

                // if it's not in the open list...
                if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y && l.Z == adjacentSquare.Z) == null)
                {
                    // compute its score, set the parent
                    adjacentSquare.G = g + current.G;
                    adjacentSquare.H = ComputeHScore(adjacentSquare.X,
                    adjacentSquare.Y, adjacentSquare.Z, target.X, target.Y, target.Z);
                    adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                    adjacentSquare.Parent = current;

                    // and add it to the open list
                    openList.Insert(0, adjacentSquare);
                }
                else
                {
                    // test if using the current G score makes the adjacent square's F score
                    // lower, if yes update the parent because it means it's a better path
                    if (current.G + g < adjacentSquare.G)
                    {
                        adjacentSquare.G = g + current.G;
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;
                    }
                }
            }

        }

        sw.Stop();

        // assume path was found; let's show it
        locked = true;
        Location location = current;
        while (location.Parent != null)
        {
            path.Push(new Vector3(location.X,location.Y,location.Z));
            location = location.Parent;
        }
        locked = false;
        UnityEngine.Debug.LogWarning("Time used:" + sw.ElapsedMilliseconds + "ms");
        //Console.ReadKey();
    }

    public List<Vector3> SearchPathToList()
    {

        //GenerateMap(size);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        //DrawMapToConsole();
        var openList = new List<Location>();
        var closedList = new List<Location>();
        //g is represented length of each step;
        int g = 1;

        // start by adding the original position to the open list
        openList.Add(start);

        while (openList.Count > 0)
        {
            // algorithm's logic goes here
            // get the square with the lowest F score
            var lowest = openList.Min(l => l.F);
            current = openList.First(l => l.F == lowest);

            // add the current square to the closed list
            closedList.Add(current);
            // remove it from the open list
            openList.Remove(current);

            // show current square on the map
            //Console.SetCursorPosition(current.X, current.Y);
            //Console.Write('.');
            //Console.SetCursorPosition(current.X, current.Y);
            ///System.Threading.Thread.Sleep(1000);

            // if we added the destination to the closed list, we've found a path
            if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y && l.Z == target.Z) != null)
            {
                UnityEngine.Debug.LogWarning("Founded");
                break;
            }

            var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, current.Z, TerrainManager.tm);

            foreach (var adjacentSquare in adjacentSquares)
            {
                // if this adjacent square is already in the closed list, ignore it
                if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y && l.Z == adjacentSquare.Z) != null)
                    continue;

                // if it's not in the open list...
                if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y && l.Z == adjacentSquare.Z) == null)
                {
                    // compute its score, set the parent
                    adjacentSquare.G = g + current.G;
                    adjacentSquare.H = ComputeHScore(adjacentSquare.X,
                    adjacentSquare.Y, adjacentSquare.Z, target.X, target.Y, target.Z);
                    adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                    adjacentSquare.Parent = current;

                    // and add it to the open list
                    openList.Insert(0, adjacentSquare);
                }
                else
                {
                    // test if using the current G score makes the adjacent square's F score
                    // lower, if yes update the parent because it means it's a better path
                    if (current.G + g < adjacentSquare.G)
                    {
                        adjacentSquare.G = g + current.G;
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;
                    }
                }
            }

        }

        sw.Stop();

        // assume path was found; let's show it
        locked = true;
        Location location = current;
        List<Vector3> node = new List<Vector3>();
        while (location.Parent != null)
        {
            node.Add(new Vector3(location.X, location.Y, location.Z));
            location = location.Parent;
        }
        locked = false;
        UnityEngine.Debug.LogWarning("Time used:" + sw.ElapsedMilliseconds + "ms");

        return node;
    }

    public List<Location> GetWalkableAdjacentSquares(int x, int y, int z, TerrainManager worldobj)
    {
        var proposedLocations = new List<Location>()
                {
                    //X
                    new Location { X = x - 1, Y = y,Z = z },
                    new Location { X = x + 1, Y = y,Z = z },
                    new Location { X = x - 1, Y = y + 1,Z = z },
                    new Location { X = x + 1, Y = y + 1,Z = z },
                    new Location { X = x - 1, Y = y - 1,Z = z },
                    new Location { X = x + 1, Y = y - 1,Z = z },

                    //XZ
                    new Location { X = x - 1, Y = y,Z = z-1 },
                    new Location { X = x + 1, Y = y,Z = z -1},
                    new Location { X = x - 1, Y = y,Z = z +1},
                    new Location { X = x + 1, Y = y,Z = z +1},

                    //Z
                    new Location { X = x, Y = y,Z = z - 1 },
                    new Location { X = x, Y = y,Z = z + 1 },
                    new Location { X = x, Y = y + 1,Z = z - 1 },
                    new Location { X = x, Y = y + 1,Z = z + 1 },
                    new Location { X = x, Y = y - 1,Z = z - 1 },
                    new Location { X = x, Y = y - 1,Z = z + 1 }
                };

        return proposedLocations.Where(
            l =>
            {
                /*TerrainManager tm = worldobj;
                Vector2 chunkpos = tm.GetId(new Vector3(l.X, l.Y, l.Z));
                MarchingStack mc = tm.GetChunk(chunkpos);
                //UnityEngine.Debug.LogWarning(new Vector3(ReflectTo(l.X, tm.chunksize), (l.Y), ReflectTo(l.Z, tm.chunksize)));
                //UnityEngine.Debug.LogWarning(new Vector3(l.X, l.Y, l.Z));
                try
                {
                    return mc[ReflectTo(l.X, size) + (l.Y - 1) * size + (ReflectTo(l.Z, size))] > 0 && mc.values[(ReflectTo(l.X, tm.chunksize)) + (l.Y) * MarchingCubes.size + (ReflectTo(l.Z, tm.chunksize)) * MarchingCubes.size2] <= 0f && mc.values[(ReflectTo(l.X, tm.chunksize) + 1) + (l.Y) * MarchingCubes.size + (ReflectTo(l.Z, tm.chunksize)) * MarchingCubes.size2] <= 0f && mc.values[(ReflectTo(l.X, tm.chunksize) - 1) + (l.Y) * MarchingCubes.size + (ReflectTo(l.Z, tm.chunksize)) * MarchingCubes.size2] <= 0f && mc.values[(ReflectTo(l.X, tm.chunksize)) + (l.Y) * MarchingCubes.size + (ReflectTo(l.Z, tm.chunksize) + 1) * MarchingCubes.size2] <= 0.5f && mc.values[(ReflectTo(l.X, tm.chunksize)) + (l.Y) * MarchingCubes.size + (ReflectTo(l.Z, tm.chunksize) - 1) * MarchingCubes.size2] <= 0f;
                }
                catch
                {
                    try
                    {
                        return mc.values[ReflectTo(l.X, tm.chunksize) + (l.Y - 1) * MarchingCubes.size + (ReflectTo(l.Z, tm.chunksize)) * MarchingCubes.size2] > 0 && mc.values[(ReflectTo(l.X, tm.chunksize)) + (l.Y) * MarchingCubes.size + (ReflectTo(l.Z, tm.chunksize)) * MarchingCubes.size2] <= 0f;
                    }
                    catch
                    {
                        UnityEngine.Debug.LogError(new Vector3(ReflectTo(l.X, tm.chunksize), (l.Y), ReflectTo(l.Z, tm.chunksize)));
                        return false;
                    }
                
                }*/
                return false;
            }).ToList();
    }
    
    static int ReflectTo(int n,int duration){
        return n >= 0 ? n % duration : n % duration + duration - 1;
    }

    static int ComputeHScore(int x, int y, int z, int targetX, int targetY, int targetZ)
    {
        return Math.Abs(targetX - x) + Math.Abs(targetY - y) + Math.Abs(targetZ - z);
    }
}