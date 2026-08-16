using System;
using UnityEngine;

[Serializable]
public class Odpoved
{
    public string text;
    public int kamVedie = -1;
}

[Serializable]
public class DialogUzol
{
    [TextArea(2, 5)] public string text;
    public Odpoved[] odpovede;
}

public class NpcScript : MonoBehaviour
{
    public DialogAsset dialog;

    public string Meno
    {
        get { return dialog != null ? dialog.meno : "Neznamy"; }
    }

    public DialogUzol[] Uzly
    {
        get { return dialog != null ? dialog.uzly : new DialogUzol[0]; }
    }
}