using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemManager;

public class ClickableObject : MonoBehaviour
{
    public Func<ItemStack,bool> action;
}
