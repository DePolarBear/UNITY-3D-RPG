using System.Collections.Generic;
using UnityEngine;

public class VillageSpawnerScript : MonoBehaviour
{
    [Header("Odkazy")]
    public Terrain terrain;
    public TerrainGenerationScript generator;
    public SvetMriezkaScript mriezka;
    public GameObject prefabZnacky;

    [Header("Dediny")]
    public int pocetDedin = 4;
    public float polomerDediny = 25f;
    public float maxRozdielVysky = 3f;
    public float minVzdialenost = 150f;
    public float okrajStop = 0.05f;

    [Header("Domy")]
    public Vector3 opravaRotacie = new Vector3(-90f, 0f, 0f);
    public GameObject[] prefabyDomov;
    public int minDomov = 4;
    public int maxDomov = 8;
    public float polomerZastavby = 15f;

    private List<Vector3> dediny = new List<Vector3>();

    private void Start()
    {
        Random.InitState(generator.seed + 300);

        Vector3 roh = terrain.transform.position;
        float sirka = terrain.terrainData.size.x;
        float dlzka = terrain.terrainData.size.z;

        int pokusy = 0;

        while (dediny.Count < pocetDedin && pokusy < 2000)
        {
            pokusy++;

            float x = Random.Range(0f, sirka);
            float z = Random.Range(0f, dlzka);

            // nie na okrajovy svah
            if (generator.OkrajPodiel(x / sirka, z / dlzka) > okrajStop)
            {
                continue;
            }

            float px = roh.x + x;
            float pz = roh.z + z;

            if (!JeRovne(px, pz))
            {
                continue;
            }

            if (!DostDaleko(px, pz))
            {
                continue;
            }

            float vyskaSveta = terrain.SampleHeight(new Vector3(px, 0f, pz));
            Zarovnaj(px, pz, vyskaSveta / terrain.terrainData.size.y);

            float py = roh.y + terrain.SampleHeight(new Vector3(px, 0f, pz));
            Vector3 poloha = new Vector3(px, py, pz);
            PostavDomy(poloha);

            dediny.Add(poloha);
            mriezka.Obsad(px, pz, polomerDediny);

            if (prefabZnacky != null)
            {
                Instantiate(prefabZnacky, poloha, Quaternion.identity, transform);
            }
        }

        Debug.Log("Dedin: " + dediny.Count + " (pokusov " + pokusy + ")");
    }

    // je plocha okolo dost rovna?
    private bool JeRovne(float px, float pz)
    {
        float najnizsia = terrain.SampleHeight(new Vector3(px, 0f, pz));
        float najvyssia = najnizsia;

        for (int i = 0; i < 8; i++)
        {
            float uhol = i * Mathf.PI * 2f / 8f;
            float vx = px + Mathf.Cos(uhol) * polomerDediny;
            float vz = pz + Mathf.Sin(uhol) * polomerDediny;

            float v = terrain.SampleHeight(new Vector3(vx, 0f, vz));

            if (v < najnizsia) najnizsia = v;
            if (v > najvyssia) najvyssia = v;
        }

        return (najvyssia - najnizsia) <= maxRozdielVysky;
    }

    private bool DostDaleko(float px, float pz)
    {
        foreach (Vector3 d in dediny)
        {
            float dx = d.x - px;
            float dz = d.z - pz;

            if (dx * dx + dz * dz < minVzdialenost * minVzdialenost)
            {
                return false;
            }
        }

        return true;
    }

    private void Zarovnaj(float px, float pz, float cielovaVyska)
    {
        TerrainData data = terrain.terrainData;
        Vector3 roh = terrain.transform.position;
        int rozl = data.heightmapResolution;

        // metre -> body heightmapy
        float naBod = (rozl - 1) / data.size.x;

        int stredX = Mathf.RoundToInt((px - roh.x) * naBod);
        int stredZ = Mathf.RoundToInt((pz - roh.z) * naBod);

        float polomerB = polomerDediny * naBod;
        int r = Mathf.CeilToInt(polomerB * 1.5f);

        int zacX = Mathf.Clamp(stredX - r, 0, rozl - 1);
        int zacZ = Mathf.Clamp(stredZ - r, 0, rozl - 1);
        int sirkaB = Mathf.Clamp(stredX + r, 0, rozl - 1) - zacX + 1;
        int dlzkaB = Mathf.Clamp(stredZ + r, 0, rozl - 1) - zacZ + 1;

        float[,] vysky = data.GetHeights(zacX, zacZ, sirkaB, dlzkaB);

        for (int z = 0; z < dlzkaB; z++)
        {
            for (int x = 0; x < sirkaB; x++)
            {
                float dx = (zacX + x) - stredX;
                float dz = (zacZ + z) - stredZ;
                float d = Mathf.Sqrt(dx * dx + dz * dz);

                // 1 v strede dediny, 0 za prechodovou zonou
                float t = 1f - Mathf.InverseLerp(polomerB, polomerB * 1.5f, d);
                t = t * t * (3f - 2f * t);

                vysky[z, x] = Mathf.Lerp(vysky[z, x], cielovaVyska, t);
            }
        }

        data.SetHeights(zacX, zacZ, vysky);
    }

    private void PostavDomy(Vector3 stred)
    {
        if (prefabyDomov.Length == 0)
        {
            return;
        }

        int pocet = Random.Range(minDomov, maxDomov + 1);

        for (int i = 0; i < pocet; i++)
        {
            // rovnomerne po kruhu, ale s vychylenim
            float uhol = (i + Random.Range(-0.3f, 0.3f)) * Mathf.PI * 2f / pocet;
            float vzdialenost = polomerZastavby * Random.Range(0.6f, 1f);

            float px = stred.x + Mathf.Cos(uhol) * vzdialenost;
            float pz = stred.z + Mathf.Sin(uhol) * vzdialenost;
            float py = terrain.transform.position.y + terrain.SampleHeight(new Vector3(px, 0f, pz));

            // otocit tvarou do stredu dediny
            Vector3 doStredu = new Vector3(stred.x - px, 0f, stred.z - pz);
            Quaternion otocenie = Quaternion.LookRotation(doStredu);

            GameObject prefab = prefabyDomov[Random.Range(0, prefabyDomov.Length)];
            Instantiate(prefab, new Vector3(px, py, pz), otocenie * Quaternion.Euler(opravaRotacie), transform);
        }
    }
}