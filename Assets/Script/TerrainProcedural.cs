using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter),typeof(MeshCollider))]
//[ExecuteInEditMode]

public class TerrainProcedural : MonoBehaviour {


    public bool renderMesh = false;
    public int xSize = 64;
    public int ySize = 64;
    public float hOffset = 0.0f;
    public float plateauH = 0.0f;
    public float plateauL = 0.0f;
    public float ampliOctave = 20.0f;
    public float lacunarity = 2.0f;
    public float gain = 0.5f;

    //Range(0f,30f)]public float ampliOctave0 = 0f;
    public bool render_river = true;
    [Range(0f, 5f)] public float scaleOctave = 4f;
    public float[] perlin_power = new float[3];
    public float offsetXOct0 = 100f;
    public float offsetYOct0 = 100f;

    //[Range(0f, 5f)] public float ampliOctave1 = 0f;

    [Range(10f, 40f)] public float scaleOctave1 = 20f;

    public float offsetXOct1 = 100f;
    public float offsetYOct1 = 100f;

    //[Range(-2f, 2f)] public float ampliOctave2 = 0f;

    [Range(80f, 150f)] public float scaleOctave2 = 100f;

    public float offsetXOct2 = 100f;
    public float offsetYOct2 = 100f;



    private Vector3[] vertices;
    private Mesh mesh;





    private void Awake()
    {
        Generate();
        
        //UpdatePerlin();
        //Triangulate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x - xSize/2f, 0f, y - ySize/2f);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        Triangulate();

        GetComponent<MeshCollider>().sharedMesh = mesh;

    }


    

    private void Triangulate()
    {
        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
    }

    private void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i] + transform.position, 0.1f);
        }
    }

    private void UpdatePerlin()
    {
        float gain_fr = gain;
        float lacunarity_fr = lacunarity;
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        vertices = mesh.vertices;
        PerlinOctave(vertices, perlin_power[0], scaleOctave, offsetXOct0, offsetYOct0,false);
        for(int k = 1; k < 8; k++)
        {
            PerlinOctave(vertices, perlin_power[k], scaleOctave* lacunarity_fr, offsetXOct1, offsetYOct1,true);
            lacunarity_fr *= lacunarity;
            gain_fr *= gain;
        }
        
      
       // PerlinOctave(vertices, ampliOctave2, scaleOctave2, offsetXOct2, offsetYOct2,true);

        mesh.vertices = vertices;

    }
    
    private void PerlinOctave(Vector3[] inVertices,  float ampli,
        float scale, float offsetX, float offsetY, bool add = false) 
        {


        //vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        //vertices = inVertices;

        float perlin;
        for (int y = 0, i = 0; y <= ySize; y++)
                {
            for (int x = 0; x <= xSize; x++, i++)
            {

               if (add)
                    {
                        inVertices[i].y += (CalculatePerlin(x, y, scale, offsetX, offsetY) - 0.5f) * ampli;
                    }
               else
                    {

                        perlin = CalculatePerlin(x, y, scale, offsetX, offsetY) - hOffset;

                        perlin = Mathf.Max(perlin - plateauH, 0f) + Mathf.Min(perlin - plateauL, 0f);
                       inVertices[i].y = perlin * ampli;
                }
                if (x == 0 || y == 0)
                {
                   // inVertices[i].y = -2.5f + inVertices[i].y;
                }


                

            }

            }

        }

    private float CalculatePerlin(int x, int y, float scale, float offsetX, float offsetY)
    {
        float xCoord = (float)x / xSize * scale + offsetX;
        float yCoord = (float)y / ySize * scale + offsetY;

        return (Mathf.PerlinNoise(xCoord,yCoord));
    }



    Texture2D PerlinHeightMap()
    {
        Texture2D heightmap = new Texture2D(xSize, ySize);
        float gain_fr = gain;
        float lacunarity_fr = lacunarity;
        float sum_perlin_power = 0;
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float perlin = CalculatePerlin(x, y, scaleOctave, offsetXOct0, offsetYOct0) * perlin_power[0];
                sum_perlin_power = perlin_power[0];
                for (int k = 1; k < 8; k++)
                {
                    perlin += CalculatePerlin(x, y, scaleOctave, offsetXOct1, offsetYOct1) * perlin_power[k];//Mathf.Pow(perlin,10* perlin_power[k - 1]);
                    lacunarity_fr *= lacunarity;
                    gain_fr *= gain;
                    sum_perlin_power += perlin_power[k];
                }
                //perlin += CalculatePerlin(x, y, scaleOctave1, offsetXOct1, offsetYOct1) * ampliOctave1;
                //perlin += CalculatePerlin(x, y, scaleOctave2, offsetXOct2, offsetYOct2) * ampliOctave2;
                perlin *= 1f /sum_perlin_power;
                heightmap.SetPixel(x, y, new Color(perlin,perlin,perlin)); 
            }
        }
        return heightmap;
    }

    // Update is called once per frame
    private void Update ()
    {

        //UpdateVerticesCoSinWave();

        if (Time.captureFramerate % 2 != 1)
        {
            Texture2D heightmap = PerlinHeightMap();
            GetComponent<MeshRenderer>().material.mainTexture = heightmap;
            GetComponent<MeshRenderer>().material.SetFloat("_amplification", ampliOctave);
            heightmap.Apply();

            if (renderMesh)
            {
                UpdatePerlin();
                Triangulate();
                GetComponent<MeshCollider>().sharedMesh = mesh;
                //water_generate();
            }
        }

    }
}
