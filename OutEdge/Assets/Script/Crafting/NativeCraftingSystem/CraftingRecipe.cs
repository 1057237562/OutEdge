using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemManager;

public class CraftingRecipe:MonoBehaviour
{
    [Serializable]
    public struct ItemRecipe
    {
        public List<ItemStack> materials;
        public ItemStack result;

        public ItemRecipe(List<ItemStack> ma,ItemStack re)
        {
            materials = ma;
            result = re;
        }
    }

    [Serializable]
    public struct ObjectRecipe
    {
        public List<ItemStack> materials;
        public GameObject result;
        public Texture snap;

        public ObjectRecipe(List<ItemStack> ma, GameObject re,Texture sn)
        {
            materials = ma;
            result = re;
            snap = sn;
        }
    }

    public List<ObjectRecipe> objectRecipes = new List<ObjectRecipe>();
    public List<ItemRecipe> itemRecipes = new List<ItemRecipe>();

    public GameObject item;
    private void Start()
    {
        foreach(ObjectRecipe objectRecipe in objectRecipes)
        {
            GameObject nobj = Instantiate(item,item.transform.parent);
            nobj.SetActive(true);
            RecipeUI ui = nobj.GetComponent<RecipeUI>();

            ui.target = objectRecipe.result;
            ui.mater = objectRecipe.materials;

            ui.result.GetComponent<RawImage>().texture = objectRecipe.snap;
            foreach(ItemStack itemstack in objectRecipe.materials)
            {
                GameObject nm = Instantiate(ui.materialItem, ui.materialsList.transform);
                nm.SetActive(true);
                nm.GetComponent<ItemHolder>().SetItem(itemstack);
            }
        }
        foreach (ItemRecipe itemRecipe in itemRecipes)
        {
            GameObject nobj = Instantiate(item, item.transform.parent);
            nobj.SetActive(true);
            RecipeUI ui = nobj.GetComponent<RecipeUI>();

            ui.itemresult = itemRecipe.result;
            ui.mater = itemRecipe.materials;

            ui.result.GetComponent<RawImage>().texture = im.texture2d[itemRecipe.result.item.id];
            ui.text.GetComponent<Text>().text = itemRecipe.result.count + "";
            foreach (ItemStack itemstack in itemRecipe.materials)
            {
                GameObject nm = Instantiate(ui.materialItem, ui.materialsList.transform);
                nm.SetActive(true);
                nm.GetComponent<ItemHolder>().SetItem(itemstack);
            }
        }
    }
}
