using UnityEngine;

public class RockSpawnerScript : MonoBehaviour
{
    [Header("Odkazy")]
    public Terrain terrain;
    public TerrainGenerationScript generator;
    public GameObject[] prefabyKamenov;

    [Header("Hustota")]
    public float krok = 5f;
    public float okrajPrah = 0.5f;
    public float sanca = 0.9f;

    [Header("Vzhlad")]
    public float minVelkost = 0.6f;
    public float maxVelkost = 1.5f;
    [Range(0f, 0.5f)] public float zapustenie = 0.15f;

    private void Start()
    {
        if (prefabyKamenov.Length == 0)
        {
            return;
        }

        Random.InitState(generator.seed + 100);

        Vector3 roh = terrain.transform.position;
        float sirka = terrain.terrainData.size.x;
        float dlzka = terrain.terrainData.size.z;

        int pocet = 0;

        for (float z = 0f; z < dlzka; z += krok)
        {
            for (float x = 0f; x < sirka; x += krok)
            {
                if (generator.OkrajPodiel(x / sirka, z / dlzka) < okrajPrah)
                {
                    continue;
                }

                if (Random.value > sanca)
                {
                    continue;
                }

                float px = roh.x + x + Random.Range(-krok * 0.4f, krok * 0.4f);
                float pz = roh.z + z + Random.Range(-krok * 0.4f, krok * 0.4f);

                GameObject prefab = prefabyKamenov[Random.Range(0, prefabyKamenov.Length)];
                Quaternion otocenie = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                float velkost = Random.Range(minVelkost, maxVelkost);

                GameObject kamen = Instantiate(prefab, new Vector3(px, 0f, pz), otocenie, transform);
                kamen.transform.localScale = Vector3.one * velkost;

                Bounds hranice;

                if (SkusHranice(kamen, out hranice))
                {
                    float polomer = Mathf.Max(hranice.size.x, hranice.size.z) * 0.5f;
                    float dno = NajnizsiaVyska(px, pz, polomer) - zapustenie * hranice.size.y;

                    kamen.transform.position += new Vector3(0f, dno - hranice.min.y, 0f);
                }

                pocet++;
            }
        }

        Debug.Log("Kamenov: " + pocet);
    }

    private bool SkusHranice(GameObject objekt, out Bounds hranice)
    {
        Renderer[] renderery = objekt.GetComponentsInChildren<Renderer>();

        if (renderery.Length == 0)
        {
            hranice = new Bounds();
            return false;
        }

        hranice = renderery[0].bounds;

        for (int i = 1; i < renderery.Length; i++)
        {
            hranice.Encapsulate(renderery[i].bounds);
        }

        return true;
    }

    private float NajnizsiaVyska(float px, float pz, float polomer)
    {
        float najnizsia = terrain.SampleHeight(new Vector3(px, 0f, pz));

        for (int i = 0; i < 8; i++)
        {
            float uhol = i * Mathf.PI * 2f / 8f;
            float vx = px + Mathf.Cos(uhol) * polomer;
            float vz = pz + Mathf.Sin(uhol) * polomer;

            float v = terrain.SampleHeight(new Vector3(vx, 0f, vz));

            if (v < najnizsia)
            {
                najnizsia = v;
            }
        }

        return terrain.transform.position.y + najnizsia;
    }
}