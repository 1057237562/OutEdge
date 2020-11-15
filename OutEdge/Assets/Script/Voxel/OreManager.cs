using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreManager : MonoBehaviour
{
    public static OreManager om;
    public OreDictionary[] oreDictionaries;
    public TreeGenerator[] treeGenerators;

    [Serializable]
    public struct OreDictionary
    {
        public int oreId;
        public int maxY;
        public int minY;
        public int minVienSize;
        public int maxVienSize;
        public int chance;

        public OreDictionary(int Id, int maxy, int miny, int minvien, int maxvien, int c)
        {
            oreId = Id;
            maxY = maxy;
            minY = miny;
            minVienSize = minvien;
            maxVienSize = maxvien;
            chance = c;
        }

    }

    [Serializable]
    public struct StructDictionary
    {
        public GameObject prefab;
        public int chance;
        public int id;

        public StructDictionary(GameObject gameObject,int c,int Id)
        {
            prefab = gameObject;
            chance = c;
            id = Id;
        }
    }

    [Serializable]
    public struct TreeGenerator
    {
        public StructDictionary structDictionary;
        public int maxAltitude;
        public int minAltitude;
        public float maxMoist;
        public float minMoist;
    }

    // Start is called before the first frame update
    void Awake()
    {
        om = this;
        //oreDictionaries.Add(new OreDictionary(2,25,0,10,20,64));
        //oreDictionaries.Add(new OreDictionary(3, 30,0,20,40, 128));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
