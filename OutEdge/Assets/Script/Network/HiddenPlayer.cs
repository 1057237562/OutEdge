using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPlayer : NetworkProximityChecker
{
    // Start is called before the first frame update
    public void Start()
    {
        /*if(OutEdgeNetworkManager.networkManager.mode == NetworkManagerMode.ServerOnly)
        {
            visRange = TerrainManager.size * (2 * TerrainManager.range + 1);
        }*/
        visRange = TerrainManager.size * TerrainManager.range;
    }
}
