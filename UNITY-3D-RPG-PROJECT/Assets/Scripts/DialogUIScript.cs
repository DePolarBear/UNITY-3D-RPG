using UnityEngine;
using TMPro;

public class DialogUIScript : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text textMena;
    public TMP_Text textObsahu;

    public bool JeOtvoreny { get; private set; }

    private void Start()
    {
        Skry();
    }

    public void Zobraz(string meno, DialogUzol uzol)
    {
        panel.SetActive(true);
        textMena.text = meno;

        string s = uzol.text + "\n";

        for (int i = 0; i < uzol.odpovede.Length; i++)
        {
            s += "\n" + (i + 1) + ") " + uzol.odpovede[i].text;
        }

        textObsahu.text = s;
        JeOtvoreny = true;
    }

    public void Skry()
    {
        panel.SetActive(false);
        JeOtvoreny = false;
    }
}