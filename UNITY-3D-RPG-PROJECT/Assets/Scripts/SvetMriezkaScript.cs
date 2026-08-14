using UnityEngine;

public class SvetMriezkaScript : MonoBehaviour
{
    public Terrain terrain;
    public TerrainGenerationScript generator;
    public float velkostBunky = 4f;

    private bool[,] obsadene;
    private int pocetX;
    private int pocetZ;
    private Vector3 roh;

    private void Awake()
    {
        roh = terrain.transform.position;

        pocetX = Mathf.CeilToInt(generator.velkost.x / velkostBunky);
        pocetZ = Mathf.CeilToInt(generator.velkost.z / velkostBunky);

        obsadene = new bool[pocetX, pocetZ];
    }

    public bool JeVolne(float px, float pz, float polomer)
    {
        int bx = Mathf.FloorToInt((px - roh.x) / velkostBunky);
        int bz = Mathf.FloorToInt((pz - roh.z) / velkostBunky);
        int dosah = Mathf.CeilToInt(polomer / velkostBunky);

        for (int dz = -dosah; dz <= dosah; dz++)
        {
            for (int dx = -dosah; dx <= dosah; dx++)
            {
                int x = bx + dx;
                int z = bz + dz;

                if (x < 0 || z < 0 || x >= pocetX || z >= pocetZ)
                {
                    continue;
                }

                if (obsadene[x, z])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void Obsad(float px, float pz, float polomer)
    {
        int bx = Mathf.FloorToInt((px - roh.x) / velkostBunky);
        int bz = Mathf.FloorToInt((pz - roh.z) / velkostBunky);
        int dosah = Mathf.CeilToInt(polomer / velkostBunky);

        for (int dz = -dosah; dz <= dosah; dz++)
        {
            for (int dx = -dosah; dx <= dosah; dx++)
            {
                int x = bx + dx;
                int z = bz + dz;

                if (x < 0 || z < 0 || x >= pocetX || z >= pocetZ)
                {
                    continue;
                }

                obsadene[x, z] = true;
            }
        }
    }
}