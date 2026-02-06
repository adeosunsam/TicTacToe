using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Diagnostics
{
    public class TMPFontCharacterChecker : MonoBehaviour
    {
        [Header("Font to Check")]
        [SerializeField]
        private TMP_FontAsset fontAsset;

        [Header("Test Strings (from your game)")]
        [SerializeField]
        private string[] testStrings = new string[]
        {
            "Player X's Turn",
            "Player O's Turn",
            "Player X Wins!",
            "Player O Wins!",
            "It's a Draw!",
            "EASY",
            "MEDIUM",
            "HARD",
            "TIC TAC TOE",
            "Play vs Bot",
            "Play vs Friend",
            "Select Difficulty:",
            "Reset",
            "Back"
        };

		private void Start()
		{
            //ListAllFontCharacters();
            CheckFontCharacters();
		}

		[ContextMenu("Check Font Characters")]
        public void CheckFontCharacters()
        {
            if (fontAsset == null)
            {
                Debug.LogError("No font asset assigned!");
                return;
            }

            Debug.Log($"<color=cyan>===== Checking Font: {fontAsset.name} =====</color>");

            HashSet<char> missingChars = new HashSet<char>();
            Dictionary<string, List<char>> missingByString = new Dictionary<string, List<char>>();

            foreach (string testStr in testStrings)
            {
                List<char> missing = new List<char>();
                
                foreach (char c in testStr)
                {
                    if (!fontAsset.HasCharacter(c))
                    {
                        missingChars.Add(c);
                        missing.Add(c);
                    }
                }

                if (missing.Count > 0)
                {
                    missingByString[testStr] = missing;
                }
            }

            if (missingChars.Count == 0)
            {
                Debug.Log($"<color=green>✓ All characters found in font asset!</color>");
            }
            else
            {
                Debug.LogWarning($"<color=yellow>Found {missingChars.Count} missing characters:</color>");
                
                foreach (char c in missingChars.OrderBy(x => (int)x))
                {
                    int ascii = (int)c;
                    string displayChar = c == ' ' ? "SPACE" : c == '\t' ? "TAB" : c == '\n' ? "NEWLINE" : c.ToString();
                    Debug.LogWarning($"  • '{displayChar}' (ASCII: {ascii}, Hex: 0x{ascii:X})");
                }

                Debug.Log("\n<color=yellow>Affected strings:</color>");
                foreach (var kvp in missingByString)
                {
                    string chars = string.Join(", ", kvp.Value.Select(c => 
                    {
                        int ascii = (int)c;
                        string displayChar = c == ' ' ? "SPACE" : c == '\t' ? "TAB" : c == '\n' ? "NEWLINE" : c.ToString();
                        return $"'{displayChar}'({ascii})";
                    }));
                    Debug.LogWarning($"  \"{kvp.Key}\" → Missing: {chars}");
                }

                Debug.Log("\n<color=cyan>Copy this to GameCharacters.txt:</color>");
                string allChars = string.Join("", missingChars);
                Debug.Log($"<color=white>{allChars}</color>");
            }

            // Check character table size
            Debug.Log($"\n<color=cyan>Font Info:</color>");
            Debug.Log($"  Character Table: {fontAsset.characterTable.Count} characters");
            Debug.Log($"  Glyph Table: {fontAsset.glyphTable.Count} glyphs");
        }

        [ContextMenu("List All Font Characters")]
        public void ListAllFontCharacters()
        {
            if (fontAsset == null)
            {
                Debug.LogError("No font asset assigned!");
                return;
            }

            Debug.Log($"<color=cyan>===== Characters in {fontAsset.name} =====</color>");
            
            var chars = fontAsset.characterTable.OrderBy(c => c.unicode).ToList();
            
            string output = "Characters: ";
            foreach (var character in chars)
            {
                char c = (char)character.unicode;
                output += c;
            }
            
            Debug.Log(output);
            Debug.Log($"Total: {chars.Count} characters");
        }
    }
}
