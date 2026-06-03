using System;
using UnityEngine;

[Serializable]
public class ConversationSet
{
    [Header("Start phrase")]
    [TextArea(2, 5)]
    public string pregunta;

    [Header("Answers")]
    [TextArea(2, 5)]
    public string fraseMeh;

    [TextArea(2, 5)]
    public string fraseGood;

    [TextArea(2, 5)]
    public string fraseVeryGood;
}