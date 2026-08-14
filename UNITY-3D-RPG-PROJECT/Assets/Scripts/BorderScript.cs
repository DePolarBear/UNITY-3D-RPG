using UnityEngine;

public class BorderScript : MonoBehaviour
{
    public Terrain terrain;
    public TerrainGenerationScript generator;
    public float vyska = 80f;
    public float hrubka = 5f;

    private void Awake()
    {
        Vector3 roh = terrain.transform.position;

        float sirka = generator.velkost.x;
        float dlzka = generator.velkost.z;

        float stredX = roh.x + sirka / 2f;
        float stredZ = roh.z + dlzka / 2f;
        float y = roh.y + vyska / 2f;

        Stena("Sever", new Vector3(stredX, y, roh.z + dlzka), new Vector3(sirka, vyska, hrubka));
        Stena("Juh", new Vector3(stredX, y, roh.z), new Vector3(sirka, vyska, hrubka));
        Stena("Vychod", new Vector3(roh.x + sirka, y, stredZ), new Vector3(hrubka, vyska, dlzka));
        Stena("Zapad", new Vector3(roh.x, y, stredZ), new Vector3(hrubka, vyska, dlzka));
    }

    private void Stena(string nazov, Vector3 poloha, Vector3 velkost)
    {
        GameObject stena = new GameObject(nazov);
        stena.transform.SetParent(transform, false);
        stena.transform.position = poloha;

        BoxCollider bc = stena.AddComponent<BoxCollider>();
        bc.size = velkost;
    }
}