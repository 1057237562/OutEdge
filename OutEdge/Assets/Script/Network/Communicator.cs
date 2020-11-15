using CommandTerminal;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainManager;
using static OutEdgeNetworkManager;
using static DataSerializer;
using System.IO;
using Unity.Mathematics;
using OutEdge;

public class Communicator : NetworkBehaviour
{
    public static string playerName = "";
    public static Communicator comm;
    public static event Action<string> OnMessage;

    private void Start()
    {
        if (!transform.GetComponent<NetworkIdentity>().isLocalPlayer) { return; }
        comm = this;
        if (OutEdgeNetworkManager.networkManager.mode != NetworkManagerMode.ServerOnly)
        {
            StartCoroutine(TerrainManager.tm.FirstLoad());
        }
    }

    [Command]
    public void CmdSendMsg(string message)
    {
        if (message.Trim() != "")
            RpcReceiveMsg(message.Trim());
    }

    [ClientRpc]
    public void RpcReceiveMsg(string message)
    {
        Terminal.Log(TerminalLogType.Warning , message);

        OnMessage?.Invoke(message);
    }

    public void CallSyncChunk(Vector3 pos)
    {
        int3 IdPos = math.int3((int)pos.x / size, (int)pos.y / size, (int)pos.z / size);
        SyncChunkMessage scm = new SyncChunkMessage();
        scm.chunkPos = IdPos;
        if (File.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs"))
        {
            FileInfo fi = new FileInfo(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + IdPos.x + "-" + IdPos.y + "-" + IdPos.z + ".rcs");
            scm.modifiedTime = fi.LastWriteTime.ToString();
            connectionToServer.Send(scm);
        }
        else
        {
            scm.modifiedTime = DateTime.MinValue.ToString();
            connectionToServer.Send(scm);
        }
    }

    [Command]
    public void CmdSpawnEntity(int id,Vector3 pos,Quaternion quaternion)
    {
        SpawnEntity(id,pos,quaternion);
    }

    public void SpawnEntity(int id, Vector3 pos, Quaternion quaternion)
    {
        GameObject obj = Instantiate(EntityManager.em.entities[id].prefab, pos, quaternion);
        NetworkServer.Spawn(obj);
    }

    public void UploadChunkData(Vector3 pos,float[,,] density,int[,,] type)
    {
        int3 IdPos = math.int3((int)pos.x / size, (int)pos.y / size, (int)pos.z / size);
        SyncChunkCallBack scb = new SyncChunkCallBack();
        scb.chunkPos = IdPos;
        scb.density = ObjectToBytes(density);
        scb.type = ObjectToBytes(type);
        connectionToServer.Send(scb);
    }
}
