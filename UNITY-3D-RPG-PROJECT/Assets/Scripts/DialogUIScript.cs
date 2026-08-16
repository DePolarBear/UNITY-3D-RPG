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

    public void Zobraz(string meno, string obsah)
    {
        panel.SetActive(true);
        textMena.text = meno;
        textObsahu.text = obsah;
        JeOtvoreny = true;
    }

    public void Skry()
    {
        panel.SetActive(false);
        JeOtvoreny = false;
    }
}