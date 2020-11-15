using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutEdge
{
    public class EntityManager : MonoBehaviour
    {
        public static EntityManager em;
        public EntityDictionary[] entities;

        public AnimationCurve[] curves;

        public EntityGen[] genbase;

        [Serializable]
        public struct EntityGen
        {
            public int id;
            public int chance;
            public int maxcount;
            public int mincount;
        }

        [Serializable]
        public struct EntityDictionary
        {
            public GameObject prefab;
            public int id;

            public EntityDictionary(GameObject gameObject, int Id)
            {
                prefab = gameObject;
                id = Id;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            em = this;
            //oreDictionaries.Add(new OreDictionary(2,25,0,10,20,64));
            //oreDictionaries.Add(new OreDictionary(3, 30,0,20,40, 128));

        }
    }

}