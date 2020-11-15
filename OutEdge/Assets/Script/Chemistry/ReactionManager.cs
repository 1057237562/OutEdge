using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemManager;

public class ReactionManager : MonoBehaviour
{
    [Serializable]
    public class Recipe
    {
        public ItemStack[] input;
        public ItemStack[] output;
        public int minTemperature;
        public int time;

        public Recipe(ItemStack[] i,ItemStack[] o,int mt,int t)
        {
            input = i;
            output = o;
            minTemperature = mt;
            time = t;
        }
    }

    public static ReactionManager rm;
    [SerializeField]
    public List<Recipe> b_recipes = new List<Recipe>();

    private void Start()
    {
        rm = this;
        //b_recipes.Add(new Recipe(new ItemStack[2] { new ItemStack(new Item(2, 0, ""), 1, 0), new ItemStack(new Item(3, 0, ""), 1, 0) }, new ItemStack[1] { new ItemStack(new Item(9, 0, ""), 1, 0) }, 675 ,3000));
    }
}
