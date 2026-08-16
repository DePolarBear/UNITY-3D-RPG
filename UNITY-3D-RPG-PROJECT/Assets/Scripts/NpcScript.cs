using System;
using UnityEngine;

[Serializable]
public class Odpoved
{
    public string text;
    public int kamVedie = -1;   // -1 = zavriet rozhovor
}

[Serializable]
public class DialogUzol
{
    [TextArea(2, 5)] public string text;
    public Odpoved[] odpovede;
}

public class NpcScript : MonoBehaviour
{
    public string meno = "Dedincan";
    public DialogUzol[] uzly;
}