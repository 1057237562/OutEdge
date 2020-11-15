using UnityEngine;
using System;


namespace TMPro
{
    /// <summary>
    /// EXample of a Custom Character input Validator to only allow digits from 0 to 9.
    /// </summary>
    [Serializable]
    //[CreateAssetMenu(fileName = "inputValidator - Digits.asset", menuName = "TextMeshPro/input Validators/Digits", order = 100)]
    public class TMP_DigitValidator : TMP_InputValidator
    {
        // Custom text input validation function
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (ch >= '0' && ch <= '9')
            {
                pos += 1;
                return ch;
            }

            return (char)0;
        }
    }
}
