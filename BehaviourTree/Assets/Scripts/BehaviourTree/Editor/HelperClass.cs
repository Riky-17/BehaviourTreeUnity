using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.Editor
{
    public static class HelperClass
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

        public static void AddToClassList(this VisualElement element, params string[] classes)
        {
            foreach (string classString in classes)
            {
                element.AddToClassList(classString);
            }
        }
    }
}