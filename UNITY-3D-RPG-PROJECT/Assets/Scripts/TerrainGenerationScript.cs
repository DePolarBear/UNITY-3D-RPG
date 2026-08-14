using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainGenerationScript : MonoBehaviour
{
    [Header("Rozmery")]
    public int rozlisenie = 513;
    public Vector3 velkost = new Vector3(200f, 30f, 200f);

    [Header("Sum")]
    public bool nahodnySeed = true;
    public int seed = 1337;

    [Header("Zakladne vlnenie")]
    public float zakladFrequency = 0.02f;
    public float zakladVyska = 0.08f;

    [Header("Kopce vnutri")]
    public float kopceFrequency = 0.01f;
    public float kopcePrah = 0.55f;
    public float kopceVyska = 0.35f;

    [Header("Hradba po obvode")]
    public float okrajSirka = 0.15f;
    public float okrajVyska = 1f;

    [Header("Low poly")]
    public int pocetSchodov = 12;

    private void Awake()
    {
        if (nahodnySeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        Debug.Log("Seed terenu: " + seed);
        Generuj();
    }

    private void Generuj()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;

        data.heightmapResolution = rozlisenie;
        data.size = velkost;

        // prvy sum - jemne vlnenie roviny
        FastNoiseLite zaklad = new FastNoiseLite();
        zaklad.SetSeed(seed);
        zaklad.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        zaklad.SetFractalType(FastNoiseLite.FractalType.FBm);
        zaklad.SetFractalOctaves(4);
        zaklad.SetFrequency(zakladFrequency);

        // druhy sum - maska, kde stoja kopce
        FastNoiseLite kopce = new FastNoiseLite();
        kopce.SetSeed(seed + 1);
        kopce.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        kopce.SetFractalType(FastNoiseLite.FractalType.FBm);
        kopce.SetFractalOctaves(2);
        kopce.SetFrequency(kopceFrequency);

        float[,] vysky = new float[rozlisenie, rozlisenie];

        for (int z = 0; z < rozlisenie; z++)
        {
            for (int x = 0; x < rozlisenie; x++)
            {
                // index -> metre, aby sum nezavisel od rozlisenia
                float wx = (float)x / (rozlisenie - 1) * velkost.x;
                float wz = (float)z / (rozlisenie - 1) * velkost.z;

                // 1. jemne vlnenie
                float h = (zaklad.GetNoise(wx, wz) + 1f) * 0.5f * zakladVyska;

                // 2. kopce - len tam, kde druhy sum prekroci prah
                float m = (kopce.GetNoise(wx, wz) + 1f) * 0.5f;
                float k = Mathf.InverseLerp(kopcePrah, 1f, m);
                k = k * k * (3f - 2f * k);            // hladky nabeh
                h += k * kopceVyska;

                // 3. hradba po obvode
                float o = Okraj(x, z);
                h = Mathf.Lerp(h, okrajVyska, o);

                // 4. terasovanie
                if (pocetSchodov > 0)
                {
                    h = Mathf.Round(h * pocetSchodov) / pocetSchodov;
                }

                vysky[z, x] = Mathf.Clamp01(h);
            }
        }

        data.SetHeights(0, 0, vysky);
    }

    public float OkrajPodiel(float fx, float fz)
    {
        float d = Mathf.Min(Mathf.Min(fx, 1f - fx), Mathf.Min(fz, 1f - fz));

        if (d > okrajSirka)
        {
            return 0f;
        }

        float t = 1f - (d / okrajSirka);
        return t * t;
    }

    private float Okraj(int x, int z)
    {
        return OkrajPodiel((float)x / (rozlisenie - 1), (float)z / (rozlisenie - 1));
    }
}