using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueLine")]
public class DialogueLine : ScriptableObject
{
    public int characterId = -1;
    [TextArea]
    public string text;
    public bool isChoice;
    public string[] choices;
    public DialogueLine[] nextLines;
    public bool isSpellInput;
    public DialogueLine spellSuccessLine;
    public DialogueLine spellFailureLine;
}
