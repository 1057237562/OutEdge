using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
using UnityEditor;
using System.Linq;
//using static TerrainManager;
using static TerrainManager;
using Unity.Mathematics;

[ExecuteInEditMode]
public class MarchingUnit : MarchingObject
{
    //public LiquidManager lm;
    //Camera cam;
    public MeshFilter mf;
    public MeshCollider mc;
    public int index1;
    public int index2;
    public int index3;
    public float[,,] densities = new float[size, size, size];
    public int[,,] types = new int[size, size, size];
    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    MarchingObject[,,] chunks = new MarchingObject[3, 3, 3];
    public bool voxelsLoaded;
    public bool chunksLoaded;
    public bool voxelsLoadedAround;
    public bool chunksLoadedAround;
    public bool loading;
    public bool loaded;
    public bool smoothed;
    public bool graphicsLoaded;
    public bool reloading;
    public bool hasGraphics;
    public TextureManager t_manager = new TextureManager(2);
    public Vector3 pos;

    void Start()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        float accelerate = collision.impulse.y;
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint cp = collision.GetContact(i);

        }
    }

    public void ReloadChunks()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                for (int iii = 0; iii < 3; iii++)
                {
                    if (((MarchingUnit)chunks[i, ii, iii]) != null && !tm.loadedReloads.Contains(((MarchingUnit)chunks[i, ii, iii])))
                    {
                        ((MarchingUnit)chunks[i, ii, iii]).hasGraphics = true;
                        ((MarchingUnit)chunks[i, ii, iii]).reloading = true;
                        tm.loadedReloads.Add(((MarchingUnit)chunks[i, ii, iii]));
                    }
                }
            }
        }
    }

    public void GenerateOre()
    {
        //Debug.LogWarning("Generate Ores");
        foreach (OreManager.OreDictionary ore in OreManager.om.oreDictionaries)
        {
            for (int i = 0; i < ore.chance; i++)
            {
                if (ore.maxY >= pos.y && ore.minY <= pos.y + size)
                {
                    int vienSize = parent.localRandomize.Next(ore.minVienSize, ore.maxVienSize);
                    int xRand = parent.localRandomize.Next(size);
                    int yRand = parent.localRandomize.Next((int)Mathf.Max(ore.minY, pos.y), (int)Mathf.Min(ore.maxY, pos.y + size)) - (int)pos.y;
                    int zRand = parent.localRandomize.Next(size);

                    GenerateOreVine(parent.localRandomize, xRand, yRand, zRand, vienSize, ore.oreId);
                }
            }
        }

    }

    public void GenerateOreVine(System.Random random, int xRand, int yRand, int zRand, int VineSize, int oreId)
    {
        float f = (float)random.NextDouble() * (float)Mathf.PI;
        double d0 = (double)((float)(xRand) + Math.Sin(f) * (float)VineSize * 2 / (float)size);
        double d1 = (double)((float)(xRand) - Math.Sin(f) * (float)VineSize * 2 / (float)size);
        double d2 = (double)((float)(zRand) + Math.Cos(f) * (float)VineSize * 2 / (float)size);
        double d3 = (double)((float)(zRand) - Math.Cos(f) * (float)VineSize * 2 / (float)size);
        double d4 = (double)(yRand + random.Next(3) - 2);
        double d5 = (double)(yRand + random.Next(3) - 2);

        for (int l = 0; l <= VineSize; ++l)
        {
            double d6 = d0 + (d1 - d0) * (double)l / (double)VineSize;
            double d7 = d4 + (d5 - d4) * (double)l / (double)VineSize;
            double d8 = d2 + (d3 - d2) * (double)l / (double)VineSize;
            double d9 = random.NextDouble() * (double)VineSize / size;
            double d10 = (double)(Math.Sin((float)l * (float)Math.PI / (float)VineSize) + 1.0F) * d9 + 1.0D;
            double d11 = (double)(Math.Sin((float)l * (float)Math.PI / (float)VineSize) + 1.0F) * d9 + 1.0D;
            int i1 = (int)Math.Floor(d6 - d10 / 2.0D);
            int j1 = (int)Math.Floor(d7 - d11 / 2.0D);
            int k1 = (int)Math.Floor(d8 - d10 / 2.0D);
            int l1 = (int)Math.Floor(d6 + d10 / 2.0D);
            int i2 = (int)Math.Floor(d7 + d11 / 2.0D);
            int j2 = (int)Math.Floor(d8 + d10 / 2.0D);

            for (int k2 = i1; k2 <= l1; ++k2)
            {
                double d12 = ((double)k2 + 0.5D - d6) / (d10 / 2.0D);

                if (d12 * d12 < 1.0D)
                {
                    for (int l2 = j1; l2 <= i2; ++l2)
                    {
                        double d13 = ((double)l2 + 0.5D - d7) / (d11 / 2.0D);

                        if (d12 * d12 + d13 * d13 < 1.0D)
                        {
                            for (int i3 = k1; i3 <= j2; ++i3)
                            {
                                double d14 = ((double)i3 + 0.5D - d8) / (d10 / 2.0D);
                                if (k2 < size && k2 > 0 && l2 < size && l2 >= 0 && i3 < size && i3 > 0)
                                {
                                    if (d12 * d12 + d13 * d13 + d14 * d14 < 1.0D && densities[k2,i3, l2] > 0)// && p_76484_1_.getBlock(k2, l2, i3).isReplaceableOreGen(p_76484_1_, k2, l2, i3, field_150518_c))
                                    {
                                        if (types[k2, i3 ,l2] == 0) // Only Replace Stone
                                        {
                                            types[k2, i3, l2] = oreId;
                                        }

                                        //p_76484_1_.setBlock(k2, l2, i3, this.field_150519_a, mineableBlockMeta, 2);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }

   

    public override void Save()
    {
        if (modified)
        {
            
            if (string.IsNullOrEmpty(StartGame.savePath))
            {
                return;
            }
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
                thread.Start(new FileSystem.Values(densities, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + "-" + (int)(pos.z / size) + ".rcs"));
                Thread thread1 = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
                thread1.Start(new FileSystem.Values(types, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + "-" + (int)(pos.z / size) + ".cd"));
            }
            catch (Exception e) { Debug.Log(e); }
        }
    }

    Vector3 interpolateVerts(float f1, float f2, Vector3 v1, Vector3 v2)
    {
        float t = (isoLevel - f1) / (f2 - f1);
        return v1 + t * (v2 - v1);
    }

    public class TextureManager
    {

        public int length;

        public TextureManager(int len)
        {
            length = len;
        }

        public Vector2[][] getFromId(int id)
        {
            int x = id % length;
            int y = id / length;

            Vector2 firstpoint = new Vector2(x / (float)length, (length - y) / (float)length);
            Vector2 secpoint = new Vector2(x / (float)length, (length - y - 1) / (float)length);
            Vector2 tripoint = new Vector2((x + 1) / (float)length, (length - y - 1) / (float)length);


            Vector2 forpoint = new Vector2((x + 1) / (float)length, (length - y) / (float)length);
            Vector2 fipoint = new Vector2(x / (float)length, (length - y) / (float)length);
            Vector2 sipoint = new Vector2((x + 1) / (float)length, (length - y - 1) / (float)length);

            return new Vector2[][] { new Vector2[] { firstpoint, secpoint, tripoint }, new Vector2[] { forpoint, fipoint, sipoint } };
        }

    }

    public bool modified = false;

    public override void Unload()
    {
        Save();
        mut.WaitOne();

        entityLoaded = false;
        modified = false;
        voxelsLoaded = false;
        chunksLoaded = false;
        voxelsLoadedAround = false;
        chunksLoadedAround = false;
        loading = false;
        loaded = false;
        smoothed = false;
        graphicsLoaded = false;
        reloading = false;
        hasGraphics = false;
        vertices.Clear();
        indices.Clear();
        uvs.Clear();
        tm.loadedChunks.Remove(this);
        tm.loadedVoxels.Remove(this);
        tm.loadedLoads.Remove(this);
        tm.loadedSmooths.Remove(this);
        tm.loadedReloads.Remove(this);
        mut.ReleaseMutex();
    }

    bool entityLoaded = false;

    public void LoadGraphics()
    {
        if (!entityLoaded)
        {
            entityLoaded = true;
            parent.InstantiateES(pos.y, pos.y + size);
        }

        tm.loadedSmooths.Remove(this);

        GetComponent<MeshRenderer>().enabled = true;
        mf.mesh.Clear();
        mf.mesh.vertices = vertices.ToArray();
        mf.mesh.uv = uvs.ToArray();
        mf.mesh.triangles = indices.ToArray();
        mf.mesh.RecalculateNormals();
        mf.mesh.UploadMeshData(false);
        mf.mesh.RecalculateBounds();
        mc.sharedMesh = mf.mesh;
        transform.position = pos;
        graphicsLoaded = true;
        reloading = false;
    }

    [ContextMenu("Check Voxels")]
    public void CheckVoxels()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                for (int iii = 0; iii < 3; iii++)
                {
                    MarchingUnit c = (MarchingUnit)chunks[i, ii, iii];
                    if (c == null)
                    {
                        continue;
                    }
                    else
                    {
                        GameObject gobj = Instantiate(tm.upspite, c.transform.position + new Vector3(8, 8, 8), Quaternion.identity);
                        gobj.GetComponent<Spite>().tracker = c.gameObject;
                        if (i != 0 && ii != 0 && iii != 0)
                        {
                            if (!c.voxelsLoaded)
                            {
                                gobj.GetComponent<Renderer>().material = tm.orange;
                                //continue;
                            }
                            else if ((c.index1 - index1) * (c.index1 - index1) > 1 | (c.index2 - index2) * (c.index2 - index2) > 1 | (c.index3 - index3) * (c.index3 - index3) > 1)
                            {
                                gobj.GetComponent<Renderer>().material = Crafting.m.denymaterial;
                                continue;
                            }
                        }
                        if (!c.hasGraphics)
                        {
                            gobj.GetComponent<Renderer>().material = tm.orange;
                        }
                    }
                }
            }
        }
    }

    public bool isVoxelsLoaded() // Test for chunk that nearby if all nearby chunk is loaded with data then return true Can use My Method
    {
        bool should = false;
        for (int i = 1; i < 3; i++)
        {
            for (int ii = 1; ii < 3; ii++)
            {
                for (int iii = 1; iii < 3; iii++)
                {
                    MarchingUnit c = (MarchingUnit)chunks[i, ii, iii];
                    if (c == null)
                    {
                        return false;
                    }
                    else
                    if (!c.voxelsLoaded)
                    {
                        return false;
                    }
                    else if ((c.index1 - index1) * (c.index1 - index1) > 1 | (c.index2 - index2) * (c.index2 - index2) > 1 | (c.index3 - index3) * (c.index3 - index3) > 1)
                    {
                        return false;
                    }
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                for (int iii = 0; iii < 3; iii++)
                {
                    if (chunks[i, ii, iii] != null && ((MarchingUnit)chunks[i, ii, iii]).hasGraphics)
                    {
                        should = true;
                    }
                }
            }
        }

        if (should)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool shouldMove()
    {
        if (Mathf.Abs(pos.x - TerrainManager.pos.x) > size * (2*range + 1) / 2f + size | Mathf.Abs(pos.y - TerrainManager.pos.y) > size * loadHeight / 2f + size | Mathf.Abs(pos.z - TerrainManager.pos.z) > size * (2*range + 1) / 2f + size)
        {
            return true;
        }
        return false;
    }
    public bool isCanMove()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                for (int iii = 0; iii < 3; iii++)
                {
                    MarchingUnit c = ((MarchingUnit)chunks[i, ii, iii]);
                    if (c == null)
                    {
                        return false;
                    }
                    else
                    if (c.loading)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void RunLoadChunk()
    {
        if (numThreads < maxNumThreads())
        {
        }
        mut.WaitOne();
        Thread thread = new Thread(LoadVoxels);
        loading = true;
        numThreads++;
        tm.loadedChunks.Remove(this);
        tm.AddLoadedVoxels(this);
        //ChunkUpdateEvent();//Bug detected
        thread.Start();
        mut.ReleaseMutex();
    }

    public void RunLoad()
    {
        if (numThreads < maxNumThreads())
        {
            mut.WaitOne();
            Thread thread = new Thread(Load);
            loading = true;
            numThreads++;
            tm.loadedVoxels.Remove(this);
            tm.loadedSmooths.Add(this);
            thread.Start();
            mut.ReleaseMutex();
        }
    }
    public void RunReloadChunk()
    {
        if (numThreads < maxNumThreads())
        {
            mut.WaitOne();
            Thread thread = new Thread(ReloadChunk);
            loading = true;
            numThreads++;
            tm.loadedReloads.Remove(this);
            tm.loadedSmooths.Add(this);
            thread.Start();
            mut.ReleaseMutex();
        }
    }

    public void Destroy()
    {

    }

    public override void LoadChunks(MarchingObject[,,] chunks)
    {
        this.chunks = chunks;
        chunksLoaded = true;
    }

    public override void LoadChunk(int index1, int index2, int index3,bool loaddirect)
    { 
        this.index1 = index1;
        this.index2 = index2;
        this.index3 = index3;
        pos = new Vector3(index1 * size, index3 * size, index2 * size);
        if (loaddirect)
        {
            tm.loadedChunks.Add(this);
        }
        else
        {
            tm.loadedMarchingUnits.Add(this);
        }

        base.LoadChunk(index1, index2, index3,loaddirect);
        if (OutEdgeNetworkManager.networkManager.mode == Mirror.NetworkManagerMode.ClientOnly)
        {
            Communicator.comm.CallSyncChunk(pos);
        }
    }

    /*public void CheckChunk()
    {
        if (!loaded && isVoxelsLoaded())
        {
            tm.loadedVoxels.Add(this);//Point is here 
        }
    }

    public void ChunkUpdateEvent()
    {
        try
        {
            CheckChunk();
        }
        catch { }
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                for (int iii = 0; iii < 3; iii++)
                {
                    ((MarchingUnit)chunks[i, ii, iii])?.CheckChunk();
                }
            }
        }
    }*/


    public void LoadVoxels()
    {
        if (File.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + "-" + (int)(pos.z / size) + ".rcs"))
        {
            bool block = true;

            while (block)
            {
                try
                {
                    densities = FileSystem.DeserializeFromFile<float[,,]>(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + "-" + (int)(pos.z / size) + ".rcs");
                    types = FileSystem.DeserializeFromFile<int[,,]>(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + "-" + (int)(pos.z / size) + ".cd");

                    block = false;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    block = false;
                }
            }
        }
        else
        {
            float c = -1;
            for (int i = 0; i < size; i++)
            {
                for (int ii = 0; ii < size; ii++)
                {
                    float v = parent.GetHeight(i, ii);
                    for (int iii = 0; iii < size; iii++)
                    {
                        if (index3 * size + iii < v)
                        {
                            if (v > 0)
                            {
                                types[i, ii, iii] = v - (index3 * size + iii) >= 3 ? 0 : 1;
                            }
                            else
                            {
                                types[i, ii, iii] = 0;
                            }
                            densities[i, ii, iii] = v - (index3 * size + iii) > 1 ? 1 : (v - (index3 * size + iii));
                        }
                        else
                        {
                            types[i, ii, iii] = 1;
                            densities[i, ii, iii] = 0;
                        }
                        if (c == -1)
                        {
                            c = densities[i, ii, iii];
                        }
                        else if (hasGraphics == false && (c > 0 && densities[i, ii, iii] == 0) || (c == 0 && densities[i, ii, iii] > 0))
                        {
                            hasGraphics = true;
                        }
                    }
                }
            }

            GenerateOre();
        }

        mut.WaitOne();
        numThreads--;
        loading = false;
        voxelsLoaded = true;
        mut.ReleaseMutex();
    }

    float GetDensity(int i, int ii, int iii)
    {
        int ier = 1;
        if (i >= size)
        {
            i -= size;
            ier++;
        }
        else if (i < 0)
        {
            i += size;
            ier--;
        }
        int iier = 1;
        if (ii >= size)
        {
            ii -= size;
            iier++;
        }
        else if (ii < 0)
        {
            ii += size;
            iier--;
        }
        int iiier = 1;
        if (iii >= size)
        {
            iii -= size;
            iiier++;
        }
        else if (iii < 0)
        {
            iii += size;
            iiier--;
        }
        return ((MarchingUnit)chunks[ier, iier, iiier]).densities[i, ii, iii];
    }

    int GetType(int i, int ii, int iii)
    {
        int ier = 1;
        if (i >= size)
        {
            i -= size;
            ier++;
        }
        else if (i < 0)
        {
            i += size;
            ier--;
        }
        int iier = 1;
        if (ii >= size)
        {
            ii -= size;
            iier++;
        }
        else if (ii < 0)
        {
            ii += size;
            iier--;
        }
        int iiier = 1;
        if (iii >= size)
        {
            iii -= size;
            iiier++;
        }
        else if (iii < 0)
        {
            iii += size;
            iiier--;
        }
        return ((MarchingUnit)chunks[ier, iier, iiier]).types[i, ii, iii];
    }
    void SetDensity(int i, int ii, int iii, float density)
    {
        int ier = 1;
        if (i >= size)
        {
            i -= size;
            ier++;
        }
        else if (i < 0)
        {
            i += size;
            ier--;
        }
        int iier = 1;
        if (ii >= size)
        {
            ii -= size;
            iier++;
        }
        else if (ii < 0)
        {
            ii += size;
            iier--;
        }
        int iiier = 1;
        if (iii >= size)
        {
            iii -= size;
            iiier++;
        }
        else if (iii < 0)
        {
            iii += size;
            iiier--;
        }
        ((MarchingUnit)chunks[ier, iier, iiier]).densities[i, ii, iii] = density;
    }

    void SetType(int i, int ii, int iii, int type)
    {
        int ier = 1;
        if (i >= size)
        {
            i -= size;
            ier++;
        }
        else if (i < 0)
        {
            i += size;
            ier--;
        }
        int iier = 1;
        if (ii >= size)
        {
            ii -= size;
            iier++;
        }
        else if (ii < 0)
        {
            ii += size;
            iier--;
        }
        int iiier = 1;
        if (iii >= size)
        {
            iii -= size;
            iiier++;
        }
        else if (iii < 0)
        {
            iii += size;
            iiier--;
        }
        ((MarchingUnit)chunks[ier, iier, iiier]).types[i, ii, iii] = type;
    }

    void ReloadChunk()
    {
        bool rr = true;
        vertices.Clear();
        uvs.Clear();
        indices.Clear();
        int sizer = 0;
        int sizerer = 0;
        for (int i = 0; i < size; i++)
        {
            for (int ii = 0; ii < size; ii++)
            {
                for (int iii = 0; iii < size; iii++)
                {
                    LoadCube(i, ii, iii, sizer, ref rr);
                }
            }
            sizer = sizerer;
            sizerer = vertices.Count;
        }
        mut.WaitOne();
        loading = false;
        numThreads--;
        graphicsLoaded = false;
        mut.ReleaseMutex();
    }

    public void Load()
    {
        //Debug.Log(index1 + ":" + index2 + ":" + index3);
        /*for (int i = 0; i < size / 2; i++)
        {
            types[size / 2, size / 2, size / 2 + i] = 1;
            densities[size / 2, size / 2, size / 2 - 4 + i] = (1.0f - (float)i / (size / 2)) / 2.0f + 0.5f;
        }*/
        bool rr = true;
        int sizer = 0;
        int sizerer = 0;
        for (int i = 0; i < size; i++)
        {
            for (int ii = 0; ii < size; ii++)
            {
                for (int iii = 0; iii < size; iii++)
                {
                    LoadCube(i, ii, iii, sizer, ref rr);
                }
            }
            sizer = sizerer;
            sizerer = vertices.Count;
        }
        mut.WaitOne();
        loading = false;
        numThreads--;
        loaded = true;
        mut.ReleaseMutex();
    }

    void LoadCube(int i, int ii, int iii, int sizer, ref bool rr)
    {
        // 8 corners of the current cube
        float[] cubeCorners = new float[8]
        {
                        GetDensity(i,ii,iii),
                        GetDensity(i+1,ii,iii),
                        GetDensity(i+1,ii+1,iii),
                        GetDensity(i,ii+1,iii),
                        GetDensity(i,ii,iii+1),
                        GetDensity(i+1,ii,iii+1),
                        GetDensity(i+1,ii+1,iii+1),
                        GetDensity(i,ii+1,iii+1)
        };
        /*int[] cubeCornersT = new int[8]
        {
                        ,
                        GetType(i+1,ii,iii),
                        GetType(i+1,ii+1,iii),
                        GetType(i,ii+1,iii),
                        GetType(i,ii,iii+1),
                        GetType(i+1,ii,iii+1),
                        GetType(i+1,ii+1,iii+1),
                        GetType(i,ii+1,iii+1)
        };*/
        Vector3[] cubeCornersV = new Vector3[8]
        {
                        new Vector3(i,iii,ii),
                        new Vector3(i+1,iii,ii),
                        new Vector3(i+1,iii,ii+1),
                        new Vector3(i,iii,ii+1),
                        new Vector3(i,iii+1,ii),
                        new Vector3(i+1,iii+1,ii),
                        new Vector3(i+1,iii+1,ii+1),
                        new Vector3(i,iii+1,ii+1)
        };
        /*{
points[indexFromCoord(id.x, id.y, id.z)],
points[indexFromCoord(id.x + 1, id.y, id.z)],
points[indexFromCoord(id.x + 1, id.y, id.z + 1)],
points[indexFromCoord(id.x, id.y, id.z + 1)],
points[indexFromCoord(id.x, id.y + 1, id.z)],
points[indexFromCoord(id.x + 1, id.y + 1, id.z)],
points[indexFromCoord(id.x + 1, id.y + 1, id.z + 1)],
points[indexFromCoord(id.x, id.y + 1, id.z + 1)]*/

        // Calculate unique index for each cube configuration.
        // There are 256 possible values
        // A value of 0 means cube is entirely inside surface; 255 entirely outside.
        // The value is used to look up the edge table, which indicates which edges of the cube are cut by the isosurface.
        int cubeIndex = 0;
        if (cubeCorners[0] < isoLevel) cubeIndex |= 1;
        if (cubeCorners[1] < isoLevel) cubeIndex |= 2;
        if (cubeCorners[2] < isoLevel) cubeIndex |= 4;
        if (cubeCorners[3] < isoLevel) cubeIndex |= 8;
        if (cubeCorners[4] < isoLevel) cubeIndex |= 16;
        if (cubeCorners[5] < isoLevel) cubeIndex |= 32;
        if (cubeCorners[6] < isoLevel) cubeIndex |= 64;
        if (cubeCorners[7] < isoLevel) cubeIndex |= 128;
        // Create triangles for current cube configuration
        bool edge = (i == 0 | i == size - 1 | ii == 0 | ii == size - 1 | iii == 0 | iii == size - 1);
        for (int ier = 0; triangulation[cubeIndex, ier] != -1; ier += 3)
        {
            // Get indices of corner points A and B for each of the three edges
            // of the cube that need to be joined to form the triangle.
            int a0 = cornerIndexAFromEdge[triangulation[cubeIndex, ier]];
            int b0 = cornerIndexBFromEdge[triangulation[cubeIndex, ier]];

            int a1 = cornerIndexAFromEdge[triangulation[cubeIndex, ier + 1]];
            int b1 = cornerIndexBFromEdge[triangulation[cubeIndex, ier + 1]];

            int a2 = cornerIndexAFromEdge[triangulation[cubeIndex, ier + 2]];
            int b2 = cornerIndexBFromEdge[triangulation[cubeIndex, ier + 2]];

            rr = !rr;

            Vector2[][] vectors = t_manager.getFromId(GetType(i, ii, iii));
            Vector2[] uvset;
            if (rr)
            {
                uvset = vectors[0];
            }
            else
            {
                uvset = vectors[1];
            }

            edge = (cubeCornersV[b0].x == 0 | cubeCornersV[b0].x == size - 1 | cubeCornersV[b0].y == 0 | cubeCornersV[b0].y == size - 1 | cubeCornersV[b0].z == 0 | cubeCornersV[b0].z == size - 1);
            AddVertex(interpolateVerts(cubeCorners[a0], cubeCorners[b0], cubeCornersV[a0], cubeCornersV[b0]), sizer, uvset[0]);

            edge = (cubeCornersV[b2].x == 0 | cubeCornersV[b2].x == size - 1 | cubeCornersV[b2].y == 0 | cubeCornersV[b2].y == size - 1 | cubeCornersV[b2].z == 0 | cubeCornersV[b2].z == size - 1);
            AddVertex(interpolateVerts(cubeCorners[a2], cubeCorners[b2], cubeCornersV[a2], cubeCornersV[b2]), sizer, uvset[1]);

            edge = (cubeCornersV[b1].x == 0 | cubeCornersV[b1].x == size - 1 | cubeCornersV[b1].y == 0 | cubeCornersV[b1].y == size - 1 | cubeCornersV[b1].z == 0 | cubeCornersV[b1].z == size - 1);
            AddVertex(interpolateVerts(cubeCorners[a1], cubeCorners[b1], cubeCornersV[a1], cubeCornersV[b1]), sizer, uvset[2]);
        }
    }

    void AddVertex(Vector3 v, int sizer, Vector2 uv)
    {
        /*for (int ier = sizer; ier < vertices.Count; ier++)
        {
            if ((vertices[ier]-v).magnitude == 0)
            {
                indices.Add(ier);
                return;
            }
        }*/
        indices.Add(vertices.Count);
        uvs.Add(uv);
        vertices.Add(v);
    }

    public override bool AddBlock(Vector3 hit)
    {
        bool suceed = false;
        bool b = true;
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                for (int iii = 0; iii < 3; iii++)
                {
                    if (((MarchingUnit)chunks[i, ii, iii]).reloading)
                    {
                        b = false;
                    }
                }
            }
        }
        if (b)
        {
            //hit -= transform.position;
            //UnityEngine.Debug.Log(hit);
            int sizer = Mathf.FloorToInt(1);
            int x = Mathf.FloorToInt(hit.x);
            int y = Mathf.FloorToInt(hit.y);
            int z = Mathf.FloorToInt(hit.z);
            for (int i = 0; i < sizer * 2 + 2; i++)
            {
                for (int ii = 0; ii < sizer * 2 + 2; ii++)
                {
                    for (int iii = 0; iii < sizer * 2 + 2; iii++)
                    {
                        float d = GetDensity(x - sizer + i, z - sizer + ii, y - sizer + iii);
                        if(d == 1)
                        {
                            continue;
                        }
                        d += 1.0f - (hit - new Vector3(x - sizer + i, y - sizer + iii, z - sizer + ii)).magnitude < 0 ? 0 : 1.0f - (hit - new Vector3(x - sizer + i, y - sizer + iii, z - sizer + ii)).magnitude;
                        d = d > 1 ? 1 : d;
                        if (d == 1)
                        {
                            suceed = true;
                        }
                        SetDensity(x - sizer + i, z - sizer + ii, y - sizer + iii, d);
                    }
                }
            }
            ReloadChunks();
            if(OutEdgeNetworkManager.networkManager.mode == Mirror.NetworkManagerMode.ClientOnly)
            {
                Communicator.comm.UploadChunkData(pos, densities, types);
            }
            if (OutEdgeNetworkManager.networkManager.mode != Mirror.NetworkManagerMode.ClientOnly)
            {
                OutEdgeNetworkManager.networkManager.ChunkUpdated(math.int3((int)pos.x / size, (int)pos.y / size, (int)pos.z / size), densities, types);
            }
        }
        modified = true;
        return suceed;
    }

    public override bool RemoveBlock(Vector3 hit, int v)
    {
        int x = Mathf.FloorToInt(hit.x);
        int y = Mathf.FloorToInt(hit.y);
        int z = Mathf.FloorToInt(hit.z);

        if (tm.digginglevel[GetType(x, z, y)] > v)
        {
            return false;
        }

        bool suceed = false;
        bool b = true;
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                for (int iii = 0; iii < 3; iii++)
                {
                    if (((MarchingUnit)chunks[i, ii, iii]).reloading)
                    {
                        b = false;
                    }
                }
            }
        }
        if (b)
        {
            //hit -= transform.position;
            int sizer = 1;

            for (int i = 0; i < 3; i++)
            {
                for (int ii = 0; ii < 3; ii++)
                {
                    for (int iii = 0; iii < 3; iii++)
                    {
                        float d = GetDensity(x - sizer + i, z - sizer + ii, y - sizer + iii);
                        if (d <= 0)
                        {
                            continue;
                        }
                        d -= 1.0f - (hit - new Vector3(x - sizer + i, y - sizer + iii, z - sizer + ii)).magnitude < 0 ? 0 : 1.0f - (hit - new Vector3(x - sizer + i, y - sizer + iii, z - sizer + ii)).magnitude;
                        if (d < 0)
                        {
                            suceed = true;
                        }
                        d = d < 0 ? 0 : d;
                        SetDensity(x - sizer + i, z - sizer + ii, y - sizer + iii, d);
                    }
                }
            }
            ReloadChunks();
            if (OutEdgeNetworkManager.networkManager.mode == Mirror.NetworkManagerMode.ClientOnly)
            {
                Communicator.comm.UploadChunkData(pos, densities, types);
            }
            if (OutEdgeNetworkManager.networkManager.mode != Mirror.NetworkManagerMode.ClientOnly)
            {
                OutEdgeNetworkManager.networkManager.ChunkUpdated(math.int3((int)pos.x / size, (int)pos.y / size, (int)pos.z / size), densities, types);
            }
        }
        if (suceed)
        {
            ItemManager.SummonItem(pos + hit + (GameControll.localControll.gameObject.transform.position - pos - hit).normalized, new ItemManager.Item(GetType(x, z, y), 0, ""));
        }
        modified = true;
        return suceed;
    }

    float isoLevel = 0.5f;

    int indexFromCoord(int x, int y, int z)
    {
        return z * size * size + y * size + x;
    }

    // Update is called once per frame
    void Update()
    {
    }

    static int[,] triangulation = new int[256, 16] {
    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
    { 8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1 },
    { 3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1 },
    { 4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
    { 4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1 },
    { 9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1 },
    { 10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1 },
    { 5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
    { 5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1 },
    { 8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1 },
    { 2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
    { 2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1 },
    { 11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1 },
    { 5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1 },
    { 11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1 },
    { 11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1 },
    { 2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1 },
    { 6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
    { 3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1 },
    { 6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
    { 6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1 },
    { 8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1 },
    { 7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1 },
    { 3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
    { 0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1 },
    { 9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1 },
    { 8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
    { 5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1 },
    { 0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1 },
    { 6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1 },
    { 10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
    { 1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1 },
    { 0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1 },
    { 3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
    { 6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1 },
    { 9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1 },
    { 8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1 },
    { 3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
    { 6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1 },
    { 10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1 },
    { 10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
    { 2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1 },
    { 7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
    { 7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
    { 2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1 },
    { 1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1 },
    { 11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1 },
    { 8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1 },
    { 0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1 },
    { 7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1 },
    { 7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1 },
    { 10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1 },
    { 0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1 },
    { 7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
    { 6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1 },
    { 6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1 },
    { 4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1 },
    { 10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1 },
    { 8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1 },
    { 1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1 },
    { 10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1 },
    { 10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1 },
    { 9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1 },
    { 7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1 },
    { 3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1 },
    { 7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1 },
    { 3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1 },
    { 6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1 },
    { 9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1 },
    { 1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1 },
    { 4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1 },
    { 7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1 },
    { 6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1 },
    { 0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1 },
    { 6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1 },
    { 0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1 },
    { 11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1 },
    { 6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1 },
    { 5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1 },
    { 9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1 },
    { 1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1 },
    { 10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1 },
    { 0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1 },
    { 10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1 },
    { 11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1 },
    { 9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1 },
    { 7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1 },
    { 2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1 },
    { 9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1 },
    { 9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1 },
    { 1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
    { 5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1 },
    { 0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1 },
    { 10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1 },
    { 2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1 },
    { 0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1 },
    { 0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1 },
    { 9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1 },
    { 5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1 },
    { 5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1 },
    { 8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1 },
    { 9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1 },
    { 1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1 },
    { 3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1 },
    { 4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1 },
    { 9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1 },
    { 11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
    { 11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1 },
    { 2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1 },
    { 9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1 },
    { 3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1 },
    { 1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1 },
    { 4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1 },
    { 0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
    { 9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1 },
    { 1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    { 0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }
    };

    static int[] cornerIndexAFromEdge = new int[12]{
    0,
    1,
    2,
    3,
    4,
    5,
    6,
    7,
    0,
    1,
    2,
    3
};

    static int[] cornerIndexBFromEdge = new int[12]{
    1,
    2,
    3,
    0,
    5,
    6,
    7,
    4,
    4,
    5,
    6,
    7
};
}