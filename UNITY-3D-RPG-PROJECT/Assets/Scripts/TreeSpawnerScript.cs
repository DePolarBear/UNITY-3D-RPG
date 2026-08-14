using UnityEngine;

public class TreeSpawnerScript : MonoBehaviour
{
    [Header("Odkazy")]
    public Terrain terrain;
    public TerrainGenerationScript generator;
    public SvetMriezkaScript mriezka;
    public GameObject[] prefabyStromov;

    [Header("Kde rastu")]
    public float krok = 6f;
    public float lesFrequency = 0.006f;
    public float lesPrah = 0.5f;
    public float sanca = 0.4f;
    public float maxSklon = 30f;
    public float okrajStop = 0.3f;

    [Header("Vzhlad")]
    public float minVelkost = 0.8f;
    public float maxVelkost = 1.4f;
    public float polomerObsadenia = 2f;

    public float zapustenie = 0.2f;

    private void Start()
    {
        if (prefabyStromov.Length == 0)
        {
            return;
        }

        Random.InitState(generator.seed + 200);

        FastNoiseLite les = new FastNoiseLite();
        les.SetSeed(generator.seed + 200);
        les.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        les.SetFractalType(FastNoiseLite.FractalType.FBm);
        les.SetFractalOctaves(3);
        les.SetFrequency(lesFrequency);

        Vector3 roh = terrain.transform.position;
        float sirka = terrain.terrainData.size.x;
        float dlzka = terrain.terrainData.size.z;

        int pocet = 0;

        for (float z = 0f; z < dlzka; z += krok)
        {
            for (float x = 0f; x < sirka; x += krok)
            {
                float fx = x / sirka;
                float fz = z / dlzka;

                // nie na okrajovy svah
                if (generator.OkrajPodiel(fx, fz) > okrajStop)
                {
                    continue;
                }

                // maska lesa
                float m = (les.GetNoise(x, z) + 1f) * 0.5f;

                if (m < lesPrah)
                {
                    continue;
                }

                if (Random.value > sanca)
                {
                    continue;
                }

                // na strmom svahu strom nerastie
                if (terrain.terrainData.GetSteepness(fx, fz) > maxSklon)
                {
                    continue;
                }

                float px = roh.x + x + Random.Range(-krok * 0.4f, krok * 0.4f);
                float pz = roh.z + z + Random.Range(-krok * 0.4f, krok * 0.4f);

                float velkost = Random.Range(minVelkost, maxVelkost);
                float polomer = velkost * polomerObsadenia;

                if (!mriezka.JeVolne(px, pz, polomer))
                {
                    continue;
                }

                mriezka.Obsad(px, pz, polomer);

                float py = roh.y + terrain.SampleHeight(new Vector3(px, 0f, pz)) - zapustenie * velkost;

                GameObject prefab = prefabyStromov[Random.Range(0, prefabyStromov.Length)];
                Quaternion otocenie = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                GameObject strom = Instantiate(prefab, new Vector3(px, py, pz), otocenie, transform);
                strom.transform.localScale = Vector3.one * velkost;

                pocet++;
            }
        }

        Debug.Log("Stromov: " + pocet);
    }
}