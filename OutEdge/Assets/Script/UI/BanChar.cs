using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BanChar : MonoBehaviour
{
    public string banned;

   public void ValueChange(string input)
    {
        string output = input;
        foreach(char c in banned)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(c);
            output = output.Replace(sb.ToString(), "");
        }
        GetComponent<TMPro.TMP_InputField>().text = output;
    }
}
