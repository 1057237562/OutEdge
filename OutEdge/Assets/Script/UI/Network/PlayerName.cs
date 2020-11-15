using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    public void SetName(string name)
    {
        Communicator.playerName = name;
    }
}
