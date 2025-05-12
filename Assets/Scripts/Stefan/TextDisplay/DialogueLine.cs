/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;


namespace Dialogue
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [TextArea]
        public string text;

        public DialogueLine(string characterName, string text)
        {
            this.characterName = characterName;
            this.text = text;
        }

        public string GetCombinedText()
        {
            
            return characterName + ": " + text ;
        }

        public string GetUnmarkedText()
        {
            return Regex.Replace(text, @"\[(.*?)\]", "");
        }

        public List<LineAttribute> GetAttributes()
        {
            var results = new List<LineAttribute>();
            var matches = Regex.Matches(text, @"\[(.*?)\]");
            foreach (Match match in matches)
            {
                results.Add(new LineAttribute(match.Index, match.Groups[1].Value));
            }

            return results;
        }
    }
}
