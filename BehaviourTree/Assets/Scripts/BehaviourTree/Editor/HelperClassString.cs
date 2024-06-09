using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees.Editor
{
    public static class HelperClassString
    {
        public static string AddSpaces(this string text)
        {
            string endText = "";
            endText += char.ToUpper(text[0]);
            for(int i = 1; i < text.Length; i++)
            {
                if(text[i - 1] == '/')
                {
                    endText += char.ToUpper(text[i]);
                    continue;
                }
 
                if(char.IsUpper(text[i]))
                {
                    endText += " " + text[i];
                    continue;
                }

                endText += text[i]; 
            }
            return endText;
        }
    }
}