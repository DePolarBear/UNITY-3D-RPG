using UnityEngine;

public class RockSpawnerScript : MonoBehaviour
{
    [Header("Odkazy")]
    public Terrain terrain;
    public TerrainGenerationScript generator;
    public SvetMriezkaScript mriezka;

    [Header("Prefaby")]
    public GameObject[] prefabyOkraj;
    public GameObject[] prefabyVnutro;
    public GameObject prefabVelky;

    [Header("Hustota")]
    public float krok = 5f;
    public float okrajPrah = 0.5f;
    public float sanca = 0.9f;
    public float sancaVolne = 0.005f;

    [Header("Velke kamene")]
    public int pocetVelkych = 4;
    public float minVelkostVelky = 1f;
    public float maxVelkostVelky = 1.6f;

    [Header("Vzhlad")]
    public float minVelkost = 0.6f;
    public float maxVelkost = 1.5f;
    public float polomerObsadenia = 1.5f;
    [Range(0f, 0.5f)] public float zapustenie = 0.15f;

    private Vector3 roh;
    private float sirka;
    private float dlzka;

    private void Start()
    {
        Random.InitState(generator.seed + 100);

        roh = terrain.transform.position;
        sirka = terrain.terrainData.size.x;
        dlzka = terrain.terrainData.size.z;

        int pocet = 0;

        for (float z = 0f; z < dlzka; z += krok)
        {
            for (float x = 0f; x < sirka; x += krok)
            {
                bool naOkraji = generator.OkrajPodiel(x / sirka, z / dlzka) > okrajPrah;

                GameObject[] zoznam = naOkraji ? prefabyOkraj : prefabyVnutro;
                float sancaTu = naOkraji ? sanca : sancaVolne;

                if (zoznam.Length == 0)
                {
                    continue;
                }

                if (Random.value > sancaTu)
                {
                    continue;
                }

                // rozhodenie, nech nestoja v radoch
                float px = roh.x + x + Random.Range(-krok * 0.4f, krok * 0.4f);
                float pz = roh.z + z + Random.Range(-krok * 0.4f, krok * 0.4f);

                float velkost = Random.Range(minVelkost, maxVelkost);
                float polomerB = velkost * polomerObsadenia;

                // v stene sa kamene smu prekryvat, vo vnutri nie
                if (!naOkraji && !mriezka.JeVolne(px, pz, polomerB))
                {
                    continue;
                }

                mriezka.Obsad(px, pz, polomerB);

                GameObject prefab = zoznam[Random.Range(0, zoznam.Length)];

                PolozKamen(prefab, px, pz, velkost);
                pocet++;
            }
        }

        PolozVelke();

        Debug.Log("Kamenov: " + pocet);
    }

    private void PolozVelke()
    {
        if (prefabVelky == null)
        {
            return;
        }

        int polozenych = 0;
        int pokusy = 0;

        // poistka na pokusy, nech sa to nezacykli
        while (polozenych < pocetVelkych && pokusy < 500)
        {
            pokusy++;

            float x = Random.Range(0f, sirka);
            float z = Random.Range(0f, dlzka);

            // nie na okrajovy svah
            if (generator.OkrajPodiel(x / sirka, z / dlzka) > 0.05f)
            {
                continue;
            }

            float px = roh.x + x;
            float pz = roh.z + z;

            float velkost = Random.Range(minVelkostVelky, maxVelkostVelky);
            float polomerB = velkost * polomerObsadenia * 3f;

            if (!mriezka.JeVolne(px, pz, polomerB))
            {
                continue;
            }

            mriezka.Obsad(px, pz, polomerB);

            PolozKamen(prefabVelky, px, pz, velkost);
            polozenych++;
        }
    }

    private void PolozKamen(GameObject prefab, float px, float pz, float velkost)
    {
        Quaternion otocenie = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject kamen = Instantiate(prefab, new Vector3(px, 0f, pz), otocenie, transform);
        kamen.transform.localScale = Vector3.one * velkost;

        // posad kamen na najnizsi bod terenu pod nim
        Bounds hranice;

        if (SkusHranice(kamen, out hranice))
        {
            float polomer = Mathf.Max(hranice.size.x, hranice.size.z) * 0.5f;
            float dno = NajnizsiaVyska(px, pz, polomer) - zapustenie * hranice.size.y;

            kamen.transform.position += new Vector3(0f, dno - hranice.min.y, 0f);
        }
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