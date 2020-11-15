using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataEntity : DecorationEntity
{
    public Func<string> save;
    public Action<string> load;
}
