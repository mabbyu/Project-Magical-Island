using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Line
{
    public Character character;

    [TextArea(8, 10)]
    public string text;
}

[CreateAssetMenu(fileName ="New Conversation", menuName ="Conversation")]
public class Conversation : ScriptableObject
{
    public Character speakerLeft;
    public Character speakerRight;
    public Line[] lines;
}
