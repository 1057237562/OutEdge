using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedGrid : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        GridLayoutGroup gl = GetComponent<GridLayoutGroup>();
        int count = transform.childCount;

        gl.cellSize = new Vector2(GetComponent<RectTransform>().rect.width / count, GetComponent<RectTransform>().rect.height);
    }
}
