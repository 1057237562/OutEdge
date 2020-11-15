using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;

public class TerrainManager : MonoBehaviour
{

    public GameObject chunk;

    public static int numThreads = 0;
    public static int size = 16;

    public static int loadHeight = 7;

    public GameObject follower;
    public static int range = 4;

    public Vector3 loadedArea;

    public MarchingStack[][] chunks;

    public static TerrainManager tm;

    public List<int> digginglevel;

    public bool debugSmooth;
    public bool debugReload;

    public Material orange;

    public List<MarchingUnit> loadedChunks = new List<MarchingUnit>();
    public List<MarchingUnit> loadedVoxels = new List<MarchingUnit>();
    public List<MarchingUnit> loadedLoads = new List<MarchingUnit>();
    public List<MarchingUnit> loadedSmooths = new List<MarchingUnit>();
    public List<MarchingUnit> loadedReloads = new List<MarchingUnit>();
    public List<MarchingUnit> loadedMarchingUnits = new List<MarchingUnit>();

    public static int maxNumThreads()
    {
        return Environment.ProcessorCount;
    }
    public static Mutex mut = new Mutex();

    public GameObject upspite;

    [NonSerialized]
    public NoiseGeneratorPerlin ngp;
    [NonSerialized]
    public NoiseGeneratorPerlin mg;
    [NonSerialized]
    public VoronoiGenerator vg;

    public int maxLoadingChunk = 4;


    public void AddLoadedVoxels(MarchingUnit obj)
    {
        if(obj == null)
        {
            Debug.LogError("null interception0");
            return;
        }
        loadedVoxels.Add(obj);
    }

    public void RunLoadGraphics()
    {
        {

            MarchingUnit[] load = new MarchingUnit[maxLoadingChunk];
            for (int i = 0; i < loadedSmooths.Count; i++)
            {
                if (!loadedSmooths[i].graphicsLoaded && !loadedSmooths[i].loading)
                {
                    MarchingUnit c = loadedSmooths[i];
                    for (int ii = 0; ii < load.Length; ii++)
                    {
                        if (load[ii] == null)
                        {
                            load[ii] = c;
                            ii = load.Length;
                        }
                        else
                        if ((c.pos - pos).magnitude < (load[ii].pos - pos).magnitude)
                        {
                            MarchingUnit cc = c;
                            c = load[ii];
                            load[ii] = cc;
                        }
                    }
                }
            }
            for (int i = 0; i < load.Length; i++)
            {
                if (load[i] != null)
                {
                    if (debugSmooth)
                    {
                        Vector3 pre = load[i].transform.position;
                        Instantiate(upspite, load[i].transform.position + new Vector3(8, 8, 8), Quaternion.identity);
                        //Time.timeScale = 1f;
                        load[i].LoadGraphics();// Main thread load physic and graphic
                        if (pre != load[i].transform.position)
                        {
                            Instantiate(upspite, load[i].transform.position + new Vector3(8, 8, 8), Quaternion.identity);
                        }
                    }
                    else {
                        //Time.timeScale = 1f;
                        load[i].LoadGraphics();// Main thread load physic and graphic
                    }
                }
            }
        }
    }

    /*static bool IntersectsBox(Bounds box, float frustumPadding)
    {

        var center = box.center;
        var extents = box.extents;

        for (int i = 0; i < (planes != null ? planes.Length : 0); i++)
        {
            Plane plane = planes[i];
            var abs = plane.normal;
            abs.x = Mathf.Abs(abs.x);
            abs.y = Mathf.Abs(abs.y);
            abs.z = Mathf.Abs(abs.z);
            var planeNormal = plane.normal;
            var planeDistance = plane.distance;

            float r = extents.x * abs.x + extents.y * abs.y + extents.z * abs.z;
            float s = planeNormal.x * center.x + planeNormal.y * center.y + planeNormal.z * center.z;

            if (s + r < -planeDistance - frustumPadding)
            {
                return false;
            }
        }

        return true;
    }*/

    public void Run()
    {
        while (true)//Not good can move to Update or start when update
        {
            try
            {
                {

                    MarchingUnit[] load = new MarchingUnit[maxNumThreads()];
                    loadedReloads.RemoveAll(x => x == null);
                    mut.WaitOne();
                    for (int i = 0; i < loadedReloads.Count; i++)
                    {
                        if (!loadedReloads[i].loading && loadedReloads[i].graphicsLoaded)
                        {
                            MarchingUnit c = loadedReloads[i];
                            for (int ii = 0; ii < load.Length; ii++)
                            {
                                if (load[ii] == null)
                                {
                                    load[ii] = c;
                                    ii = load.Length;
                                }
                                else
                                if ((c.pos - pos).magnitude < (load[ii].pos - pos).magnitude)
                                {
                                    MarchingUnit cc = c;
                                    c = load[ii];
                                    load[ii] = cc;
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    mut.ReleaseMutex();
                    for (int i = 0; i < load.Length; i++)
                    {
                        if (load[i] != null)
                        {
                            load[i].RunReloadChunk(); // Regenerate Chunk model and data
                        }
                    }
                }
                /*{
                    for (int i = 0; i < loadedMarchingUnits.Count; i++)
                    {
                        if (loadedMarchingUnits[i].shouldMove() && loadedMarchingUnits[i].isCanMove())
                        {
                            MarchingUnit c = loadedMarchingUnits[i];
                            c.Unload();
                            int index1 = c.index1;
                            int index2 = c.index2;
                            int index3 = c.index3;
                            index1 = c.pos.x - pos.x > size * (2 * range + 1) / 2f + size ? index1 - (2 * range + 1) : c.pos.x - pos.x < -(size * (2 * range + 1) / 2f + size) ? index1 + (2 * range + 1) : index1;
                            index2 = c.pos.z - pos.z > size * (2 * range + 1) / 2f + size ? index2 - (2 * range + 1) : c.pos.z - pos.z < -(size * (2 * range + 1) / 2f + size) ? index2 + (2 * range + 1) : index2;
                            index3 = c.pos.y - pos.y > size * loadHeight / 2f + size ? index3 - loadHeight : c.pos.y - pos.y < -(size * loadHeight / 2 + size) ? index3 + loadHeight : index3;// Loading
                            c.LoadChunk(index1, index2, index3); // Add to need load List // Use old chunk obj
                        }
                    }
                }*/
                {
                    List<MarchingUnit> removal = new List<MarchingUnit>();
                    loadedMarchingUnits.RemoveAll(x => x == null);
                    for (int i = 0; i < loadedMarchingUnits.Count; i++)
                    {
                        if (loadedMarchingUnits[i].isCanMove())
                        {
                            loadedChunks.Add(loadedMarchingUnits[i]);
                            removal.Add(loadedMarchingUnits[i]);
                        }
                    }
                    foreach (MarchingUnit mu in removal)
                    {
                        loadedMarchingUnits.Remove(mu);
                    }
                }
                {

                    MarchingUnit[] load = new MarchingUnit[maxNumThreads() - numThreads];
                    //loadedVoxels.RemoveAll(x => x == null);
                    for (int i = 0; i < loadedVoxels.Count; i++)
                    {
                        if (!loadedVoxels[i].loaded)// && IntersectsBox(new Bounds(new Vector3(loadedVoxels[i].index1 * size, 0, loadedVoxels[i].index2 * size), new Vector3(16, 1000, 16)), 0))
                        {
                            if (loadedVoxels[i].isVoxelsLoaded())
                            {
                                MarchingUnit c = loadedVoxels[i];
                                for (int ii = 0; ii < load.Length; ii++)
                                {
                                    if (load[ii] == null)
                                    {
                                        load[ii] = c;
                                        ii = load.Length;//Update Variable
                                    }
                                    else
                                    if ((c.pos - pos).magnitude < (load[ii].pos - pos).magnitude)// Load nearest Chunk first
                                    {
                                        MarchingUnit cc = c;
                                        c = load[ii];
                                        load[ii] = cc;
                                    }
                                }
                            }
                        }
                    }
                    for (int i = 0; i < load.Length; i++)
                    {
                        if (load[i] != null)
                        {
                            load[i].RunLoad();//Load Data but not generate Model
                        }
                    }
                }
                {

                    MarchingUnit[] load = new MarchingUnit[maxNumThreads() - numThreads];
                    loadedChunks.RemoveAll(x => x == null);
                    for (int i = 0; i < loadedChunks.Count; i++)
                    {
                        if (!loadedChunks[i].voxelsLoaded)
                        {
                            MarchingUnit c = loadedChunks[i];
                            for (int ii = 0; ii < load.Length; ii++)
                            {
                                if (load[ii] == null)
                                {
                                    load[ii] = c;
                                    ii = load.Length;
                                }
                                else
                                if ((c.pos - pos).magnitude < (load[ii].pos - pos).magnitude)
                                {
                                    MarchingUnit cc = c;
                                    c = load[ii];
                                    load[ii] = cc;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < load.Length; i++)
                    {
                        if (load[i] != null)
                        {
                            load[i].RunLoadChunk(); // Generate Model
                        }
                    }
                }
            }
            catch(Exception e) { Debug.LogError(e); }
        }
    }


    //public MarchingUnit Chunk;

    public int yaxis = 4;

    public Vector2 AlignToMesh(Vector2 chunk)
    {
        return new Vector2(Floor(chunk.x/size)*size, Floor(chunk.y/size)*size);
    }

    public Vector2 GetId(Vector3 chunk)
    {
        return new Vector2(chunk.x, chunk.z);
    }

    public MarchingStack GetChunkD(Vector2 id)
    {
        return GetChunk(id.x, id.y);
    }

    public MarchingStack GetChunk(Vector2 id)
    {
        return GetChunk(id.x/size,id.y/size);
    }
    public MarchingStack GetChunk(float x, float z)
    {
        return chunks[Shrink(x,2*range + 1)][Shrink(z,2*range + 1)];
    }
    
    public int[] GetChunkIdFromPos(Vector2 pos)
    {
        var x = pos.x / size;
        var z = pos.y / size;
        return new int[2] { Shrink(x, 2 * range + 1),Shrink(z, 2 * range + 1) };
    }

    public bool isChunkLoaded(int3 Id)
    {
        if(Mathf.Abs(loadedArea.x - Id.x) < range && Mathf.Abs(loadedArea.z - Id.z) < range)
        {
            MarchingStack marching = GetChunkD(new Vector2(Id.x, Id.z));
            if (((MarchingUnit)marching.marchingObj[Shrink(Id.y, loadHeight)]).voxelsLoaded)
            {
                return true;
            }
        }
        return false;
    }

    public static int Shrink(float input,int r)
    {
        int t = Floor(input + (r - 1) / 2) % r;
        return t < 0 ? t + r : t;
    }

    [Serializable]
    public class TerrainType{
        public float lambada;
        public float gamma;

        public TerrainType(float l ,float g)
        {
            lambada = l;
            gamma = g;
        }

        public static TerrainType operator *(TerrainType a1,TerrainType a2)
        {
            return new TerrainType(a1.lambada*a2.lambada,a1.gamma*a2.gamma);
        }

        public static TerrainType operator *(TerrainType a1, float m)
        {
            return new TerrainType(a1.lambada * m, a1.gamma * m);
        }

        public static TerrainType operator *(float m,TerrainType a1)
        {
            return new TerrainType(a1.lambada * m, a1.gamma * m);
        }

        public static TerrainType operator +(TerrainType a1, TerrainType a2)
        {
            return new TerrainType(a1.lambada + a2.lambada, a1.gamma + a2.gamma);
        }
    }
    
    private float Timer = 0f;

    public int globalGeneratingCode = 0;

    Thread thread;

    public TerrainType[] terrainTypes;

    void Start()
    {
        tm = this;

        enabled = false;

        if (Directory.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath))
        {
            try
            {
                FileSystem.ReadWorld(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/world.dat");
                OnSyncWithServer();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    public void OnSyncWithServer()
    {
        if (enabled) return;

        chunks = new MarchingStack[2 * range + 1][];

#if UNITY_EDITOR
        if (GameControll.globalRandomize == null)
        {
            GameControll.globalRandomize = new System.Random(0);
            GameControll.randomseed = 0;
        }
#endif

        try
        {
            globalGeneratingCode = GameControll.globalRandomize.Next();
        }
        catch { }

        ngp = new NoiseGeneratorPerlin(new System.Random(GameControll.randomseed), 4);
        mg = new NoiseGeneratorPerlin(new System.Random(GameControll.randomseed), 4);
        vg = new VoronoiGenerator(GameControll.randomseed, terrainTypes.Length - 1);

        Time.timeScale = 0f; // Stop Physics
        //RigidbodyFirstPersonController.rfpc.gameObject.SetActive(false);

    }

    public bool isAreaLoaded(Vector3 position){
        if(GetChunk(GetId(position).pos == AlignToMesh(GetId(position))){
            return true;
        }
        return false;
    }

    public IEnumerator FirstLoad()
    {
        GameControll.loadGame();

        loadedArea = new Vector3(Floor(follower.transform.parent.position.x / size), Floor(follower.transform.parent.position.y / size), Floor(follower.transform.parent.position.z / size));
        for(int i = 0; i < 2*range + 1; i++)
        {
            int x = Shrink(loadedArea.x - range + i, 2 * range + 1);
            chunks[x] = new MarchingStack[2 * range + 1];
            for (int j = 0; j < 2 * range + 1; j++)
            {
                int z = Shrink(loadedArea.z - range + j, 2 * range + 1);
                chunks[x][z] = new MarchingStack(new Vector2((loadedArea.x - range + i) * size, (loadedArea.z - range + j) * size));
                chunks[x][z].Load(yaxis);
            }
        }
        for (int i = 0; i < 2 * range + 1; i++)
        {
            for (int ii = 0; ii < 2 * range + 1; ii++)
            {
                for (int iii = 0; iii < loadHeight; iii++)
                {
                    MarchingObject[,,] chunkers = new MarchingObject[3, 3, 3];
                    for (int ier = 0; ier < 3; ier++)
                    {
                        for (int iier = 0; iier < 3; iier++)
                        {
                            for (int iiier = 0; iiier < 3; iiier++)
                            {
                                int o = i - 1 + ier < 0 ? (2*range + 1) - 1 : i - 1 + ier >= (2*range + 1) ? 0 : i - 1 + ier;
                                int oo = ii - 1 + iier < 0 ? (2*range + 1) - 1 : ii - 1 + iier >= (2*range + 1) ? 0 : ii - 1 + iier;
                                int ooo = iii - 1 + iiier < 0 ? loadHeight - 1 : iii - 1 + iiier >= loadHeight ? 0 : iii - 1 + iiier;
                                chunkers[ier, iier, iiier] = chunks[o][oo].marchingObj[ooo];
                            }
                        }
                    }
                    chunks[i][ii].marchingObj[iii].LoadChunks(chunkers);// assign the nearby Chunk
                }
            }
        }

        
        if (!Directory.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath))
        {
            int tarx = (int)(RigidbodyFirstPersonController.rfpc.transform.position.x);
            int tarz = (int)(RigidbodyFirstPersonController.rfpc.transform.position.z);
            float tary = GetChunk(new Vector2(tarx, tarz)).GetHeight((size + tarx % size) % size, (size + tarz % size) % size);
            yaxis = (int)tary / size;
            RigidbodyFirstPersonController.rfpc.transform.position -= new Vector3(0, RigidbodyFirstPersonController.rfpc.transform.position.y - tary, 0);
        }

        enabled = true;
        if (thread == null)
        {
            thread = new Thread(Run);
            numThreads++;
            thread.Start();
        }
        yield return 0;
    }

    private void OnDestroy()
    {
        if (thread != null)
        {
            thread.Abort();
        }
    }

    Vector3 cm;

    void Update()
    {
        if (OutEdgeNetworkManager.networkManager.mode != NetworkManagerMode.ServerOnly)
        {
            Timer -= Time.deltaTime;

            if (Timer <= 0)
            {
                //planes = GeometryUtility.CalculateFrustumPlanes(follower.GetComponent<Camera>());
                RunLoadGraphics();
                if (!UIManager.ui.interacting && loadedSmooths.Count == 0)
                {
                    Time.timeScale = 1f;
                }

                pos = follower.transform.parent.position;
                cm = new Vector3(Floor(pos.x / size - loadedArea.x), Floor(pos.y / size - loadedArea.y), Floor(pos.z / size - loadedArea.z)); // seperate 32 - 0 - (-32) 
                if ((int)cm.x == 0 && (int)cm.z == 0)
                {
                    return;
                }
                LoadChunk();
                Timer = 1.0f;
            }
        }

        //TODO:Optimize This
    }

    public void SaveChunks()
    {
        for (int i = 0; i < 2 * range + 1; i++)
        {
            for (int j = 0; j < 2 * range + 1; j++)
            {
                chunks[i][j].Save();
            }
        }
    }

    public static int Floor(float c)
    {
        return Mathf.FloorToInt(c);
    }

    public static Vector3 pos;
    void LoadChunk()
    {
        for (int i = 0; i < Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1); i++)
        {
            for (int j = (int)Math.Max(0,cm.z); j < 2 * range + 1 - Math.Max(0, -cm.z); j++)
            {
                if (cm.x > 0)
                {
                    MarchingStack chunk = GetChunk(loadedArea.x - range + i,loadedArea.z - range + j);
                    chunk.UnLoad();
                    chunk.Relocate(new Vector3((loadedArea.x + cm.x + range + 1 - Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1) + i) * size, Floor(pos.y / size) * size, (loadedArea.z - range + j) * size));
                }
                else
                {
                    MarchingStack chunk = GetChunk(loadedArea.x + range - i, loadedArea.z - range + j);
                    chunk.UnLoad();
                    chunk.Relocate(new Vector3((loadedArea.x + cm.x - range - 1 + Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1) - i) * size, Floor(pos.y / size) * size, (loadedArea.z - range + j) * size));
                }
            }
        }

        for (int j = 0; j < Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1); j++)
        {
            for (int i = (int)Math.Max(0,cm.x); i < 2 * range + 1 - (int)Math.Max(0, -cm.x); i++)
            {
                if (cm.z > 0)
                {
                    MarchingStack chunk = GetChunk(loadedArea.x - range + i, loadedArea.z - range + j);
                    chunk.UnLoad();
                    chunk.Relocate(new Vector3((loadedArea.x - range + i) * size, Floor(pos.y / size) * size, (loadedArea.z + cm.z + range + 1 - Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1) + j) * size));
                }
                else
                {
                    MarchingStack chunk = GetChunk(loadedArea.x - range + i, loadedArea.z + range - j);
                    chunk.UnLoad();
                    chunk.Relocate(new Vector3((loadedArea.x - range + i) * size, Floor(pos.y / size) * size, (loadedArea.z + cm.z - range - 1 + Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1) - j) * size));
                }
            }
        }

        for (int i = 0; i < Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1); i++)
        {
            for (int j = 0; j < Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1); j++)
            {
                if (cm.x > 0)
                {
                    if (cm.z > 0)
                    {
                        MarchingStack chunk = GetChunk(loadedArea.x - range + i, loadedArea.z - range + j);
                        chunk.UnLoad();
                        chunk.Relocate(new Vector3((loadedArea.x + cm.x + range + 1 - Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1) + i) * size, Floor(pos.y / size) * size, (loadedArea.z + cm.z + range + 1 - Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1) + j) * size));
                    }
                    else
                    {
                        MarchingStack chunk = GetChunk(loadedArea.x - range + i, loadedArea.z + range - j);
                        chunk.UnLoad();
                        chunk.Relocate(new Vector3((loadedArea.x + cm.x + range + 1 - Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1) + i) * size, Floor(pos.y / size) * size, (loadedArea.z + cm.z - range - 1 + Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1) - j) * size));
                    }
                }
                else
                {
                    if (cm.z > 0)
                    {
                        MarchingStack chunk = GetChunk(loadedArea.x - range + i, loadedArea.z - range + j);
                        chunk.UnLoad();
                        chunk.Relocate(new Vector3((loadedArea.x + cm.x - range - 1 + Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1) - i) * size, Floor(pos.y / size) * size, (loadedArea.z + cm.z + range + 1 - Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1) + j) * size));
                    }
                    else
                    {
                        MarchingStack chunk = GetChunk(loadedArea.x - range + i, loadedArea.z + range - j);
                        chunk.UnLoad();
                        chunk.Relocate(new Vector3((loadedArea.x + cm.x - range - 1 + Mathf.Min(Mathf.Abs(cm.x), 2 * range + 1) - i) * size, Floor(pos.y / size) * size, (loadedArea.z + cm.z - range - 1 + Mathf.Min(Mathf.Abs(cm.z), 2 * range + 1) - j) * size));
                    }
                }
            }
        }

        //Load Chunk

        loadedArea += cm;
    }

}