using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TerrainManager;
using static DataSerializer;

public class OutEdgeNetworkManager : NetworkManager
{

    public static OutEdgeNetworkManager networkManager;

    public override void Start()
    {
        base.Start();
        networkManager = this;
        DontDestroyOnLoad(this);

        string[] args = Environment.GetCommandLineArgs();
        for(int i = 0; i < args.Length; i++)
        {
            string command = args[i];
            switch (command)
            {
                case "startserver":
                    string name = args[i + 1];
                    var outputseed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                    GameControll.globalRandomize = new System.Random(outputseed);
                    GameControll.randomseed = outputseed;

                    StartGame.savePath = name;
                    if (Directory.Exists(Environment.CurrentDirectory + "/saves/" + name))
                    {
                        int apply = 1;
                        while (Directory.Exists(Environment.CurrentDirectory + "/saves/" + name + apply.ToString()))
                        {
                            apply++;
                        }
                        StartGame.savePath = name + apply.ToString();
                    }
                    StartServer();
                    break;
            }
        }
    }

    public class CreatePlayerMessage : MessageBase
    {
        public string mapName;
        public int randomSeed;
    }

    public class SyncChunkMessage : MessageBase
    {
        public int3 chunkPos;
        public string modifiedTime;
    }

    public class SyncChunkCallBack : MessageBase
    {
        public int3 chunkPos;
        public byte[] density;
        public byte[] type;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public override void  OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        //Debug.Log("sent");
        // tell the server to create a player with this name
        conn.Send(new CreatePlayerMessage { randomSeed = GameControll.randomseed,mapName = StartGame.savePath });
    }

    public void ChunkUpdated(int3 pos, float[,,] density, int[,,] type)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            SyncChunkCallBack scb = new SyncChunkCallBack();
            scb.chunkPos = pos;
            scb.density = ObjectToBytes(density);
            scb.type = ObjectToBytes(type);
            conn.Send(scb);
        }
    }
    public void ChunkUpdated(int3 pos, byte[] density, byte[] type)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            int3 m_pos = math.int3(conn.identity.transform.position/size) - pos;
            if (math.abs(m_pos.x) < range && math.abs(m_pos.y) < range && math.abs(m_pos.z) < range) {
                SyncChunkCallBack scb = new SyncChunkCallBack();
                scb.chunkPos = pos;
                scb.density = density;
                scb.type = type;
                conn.Send(scb);
            }
        }
    }

    public void ChunkUpdated(int3 pos, byte[] density, byte[] type,NetworkConnection originConn)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.connectionId != originConn.connectionId)
            {
                int3 m_pos = math.int3(conn.identity.transform.position / size) - pos;
                if (math.abs(m_pos.x) < range && math.abs(m_pos.y) < range && math.abs(m_pos.z) < range)
                {
                    SyncChunkCallBack scb = new SyncChunkCallBack();
                    scb.chunkPos = pos;
                    scb.density = density;
                    scb.type = type;
                    conn.Send(scb);
                }
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<SyncChunkMessage>(SyncChunk);
        NetworkServer.RegisterHandler<SyncChunkCallBack>(CallBackSyncChunkServer);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<CreatePlayerMessage>(OnConnected);
        NetworkClient.RegisterHandler<SyncChunkCallBack>(CallBackSyncChunk);
    }

    void SyncChunk(NetworkConnection connection, SyncChunkMessage scm)
    {
        int3 IdPos = scm.chunkPos;
        SyncChunkCallBack scb = new SyncChunkCallBack();
        if (tm.isChunkLoaded(IdPos))
        {
            MarchingUnit mu = ((MarchingUnit)tm.GetChunkD(new Vector2(IdPos.x, IdPos.z)).marchingObj[Shrink(IdPos.y, loadHeight)]);
            if (mu.modified)
            {
                scb.chunkPos = IdPos;
                scb.density = ObjectToBytes(mu.densities);
                scb.type = ObjectToBytes(mu.types);
                connection.Send(scb);
            }
            else if (File.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs"))
            {
                FileInfo fi = new FileInfo(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs");
                if (fi.LastWriteTime > DateTime.Parse(scm.modifiedTime))
                {
                    scb.chunkPos = IdPos;
                    scb.density = ObjectToBytes(mu.densities);
                    scb.type = ObjectToBytes(mu.types);
                    connection.Send(scb);
                }
            }
        }
        else if (File.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs"))
        {
            FileInfo fi = new FileInfo(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs");
            if (fi.LastWriteTime > DateTime.Parse(scm.modifiedTime))
            {
                Thread fileThread = new Thread(() =>
                {
                    scb.chunkPos = IdPos;
                    scb.density = ObjectToBytes(FileSystem.DeserializeFromFile<float[,,]>(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs"));
                    scb.type = ObjectToBytes(FileSystem.DeserializeFromFile<int[,,]>(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".cd"));
                    connection.Send(scb);
                });
            }
        }
    }
    void CallBackSyncChunkServer(NetworkConnection connection, SyncChunkCallBack scb)
    {
        int3 IdPos = scb.chunkPos;
        if (tm.isChunkLoaded(IdPos))
        {
            MarchingUnit mu = ((MarchingUnit)tm.GetChunkD(new Vector2(IdPos.x, IdPos.z)).marchingObj[Shrink(IdPos.y, loadHeight)]);
            mu.densities = BytesToObject(scb.density) as float[,,];
            mu.types = BytesToObject(scb.type) as int[,,];
            mu.modified = true;
            mu.ReloadChunks();
        }
        else
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
                thread.Start(new FileSystem.Values(scb.density, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs"));
                Thread thread1 = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
                thread1.Start(new FileSystem.Values(scb.type, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".cd"));
            }
            catch (Exception e) { Debug.Log(e); }
        }
        ChunkUpdated(IdPos, scb.density, scb.type,connection);
    }

    void CallBackSyncChunk(NetworkConnection connection, SyncChunkCallBack scb)
    {
        int3 IdPos = scb.chunkPos;
        if (tm.isChunkLoaded(IdPos))
        {
            MarchingUnit mu = ((MarchingUnit)tm.GetChunkD(new Vector2(IdPos.x, IdPos.z)).marchingObj[Shrink(IdPos.y, loadHeight)]);
            mu.densities = BytesToObject(scb.density) as float[,,];
            mu.types = BytesToObject(scb.type) as int[,,];
            mu.modified = true;
            mu.ReloadChunks();
        }
        else
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
                thread.Start(new FileSystem.Values(scb.density, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs"));
                Thread thread1 = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
                thread1.Start(new FileSystem.Values(scb.type, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".cd"));
            }
            catch (Exception e) { Debug.Log(e); }
        }
    }

    void OnConnected(NetworkConnection connection, CreatePlayerMessage attributes)
    {
        // create a gameobject using the name supplied by client
        //Debug.Log("Connected to " + attributes.mapName + " randomseed:" + attributes.randomSeed);
        GameControll.randomseed = attributes.randomSeed;
        GameControll.globalRandomize = new System.Random(attributes.randomSeed);
        StartGame.savePath = "LocalTemp_"+attributes.mapName;

        tm.OnSyncWithServer();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        tm.OnSyncWithServer();
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        //TerrainManager.tm.OnSyncWithServer();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("StartScene");
        Time.timeScale = 1f;
    }
}
