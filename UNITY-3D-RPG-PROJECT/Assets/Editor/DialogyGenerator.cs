using UnityEditor;
using UnityEngine;

public class DialogyGenerator
{
    [MenuItem("Hra/Vytvor dialogy")]
    public static void Vytvor()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Dialogy"))
        {
            AssetDatabase.CreateFolder("Assets", "Dialogy");
        }

        VytvorDialog("Aldric", new DialogUzol[]
        {
            Uzol("Stoj. Do dediny sa teraz nechodi len tak.",
                Odp("Kto si?", 1),
                Odp("Preco nie?", 2),
                Odp("Uz odchadzam.", -1)),

            Uzol("Aldric. Strazim tuto branu, odkedy si pamatam.",
                Odp("A co strazis pred nami?", 2),
                Odp("Rozumiem.", -1)),

            Uzol("Z lesa na severe zaculi vlci zavijanie. Tri noci po sebe.",
                Odp("Moznem s tym pomoct?", 3),
                Odp("To nie je moj problem.", -1)),

            Uzol("Ak sa vratis so vlcou kozou, dedina ti to nezabudne.",
                Odp("Pozriem sa na to.", -1))
        });

        VytvorDialog("Mira", new DialogUzol[]
        {
            Uzol("Pozor kam slapes, tie bylinky rastu len tu.",
                Odp("Co je to za bylinky?", 1),
                Odp("Prepacte.", -1)),

            Uzol("Krvavnik. Zastavi krvacanie rychlejsie ako modlitba.",
                Odp("Predas mi nejaky?", 2),
                Odp("Zaujimave.", -1)),

            Uzol("Nazbieraj mi ich pri potoku a polovicu si nechas.",
                Odp("Plati.", -1))
        });

        VytvorDialog("Borek", new DialogUzol[]
        {
            Uzol("Pila stoji. Drevo z juhu uz tyzden nikto nedoviezol.",
                Odp("Co sa stalo?", 1),
                Odp("Smola.", -1)),

            Uzol("Cesta je zavalena. Skala spadla po tej burke.",
                Odp("Kto to odprace?", 2),
                Odp("Uvidime.", -1)),

            Uzol("Nikto. Kazdy caka, ze to spravi ten druhy.",
                Odp("Pozriem sa na tu cestu.", -1))
        });

        VytvorDialog("Vela", new DialogUzol[]
        {
            Uzol("Zvon uz dva mesiace nezvonil. Nemam silu vytiahnut lano.",
                Odp("Preco na tom zalezi?", 1),
                Odp("Pomozem vam.", 2)),

            Uzol("Zvon volal ludi z poli, ked prisla burka. Teraz ich nikto nevola.",
                Odp("Pomozem vam s nim.", 2),
                Odp("Chapem.", -1)),

            Uzol("Si dobry clovek. Vystup do veze, ked budes mat cas.",
                Odp("Spravim to.", -1))
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Dialogy vytvorene v Assets/Dialogy");
    }

    private static void VytvorDialog(string meno, DialogUzol[] uzly)
    {
        DialogAsset asset = ScriptableObject.CreateInstance<DialogAsset>();
        asset.meno = meno;
        asset.uzly = uzly;

        AssetDatabase.CreateAsset(asset, "Assets/Dialogy/Dialog_" + meno + ".asset");
    }

    private static DialogUzol Uzol(string text, params Odpoved[] odpovede)
    {
        DialogUzol u = new DialogUzol();
        u.text = text;
        u.odpovede = odpovede;
        return u;
    }

    private static Odpoved Odp(string text, int kam)
    {
        Odpoved o = new Odpoved();
        o.text = text;
        o.kamVedie = kam;
        return o;
    }
}