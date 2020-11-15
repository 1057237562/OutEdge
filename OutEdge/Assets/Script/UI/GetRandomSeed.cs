using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetRandomSeed : MonoBehaviour
{
    public InputField target;

    public void Generate()
    {
        target.text = Random.Range(int.MinValue, int.MaxValue) + "";
    }
}
