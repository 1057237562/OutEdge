//
// Rain Maker (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
//

using System;

using UnityEngine;
using UnityEditor;

namespace DigitalRuby.RainMaker
{
    public class RainMakerEditor : Editor
    {
    }

    [CustomEditor(typeof(RainScript))]
    public class RainMakerEditor3D : RainMakerEditor
    {
    }

    [CustomEditor(typeof(RainScript2D))]
    public class RainMakerEditor2D : RainMakerEditor
    {
    }
}