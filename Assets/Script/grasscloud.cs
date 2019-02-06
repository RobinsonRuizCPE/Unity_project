using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class grasscloud : MonoBehaviour
{
    private Mesh mesh_updated;
    public MeshFilter filter;
    public Material Grass_material;
    private Vector3 lastPosition;
    public Vector2 dimension;
    [Range(1, 5000)]
    public int grass_number;
    public float heigth_start = 200;
    public int seed;

    // Update is called once per frame
    int indices_index = 0;
    Vector3 normal_offset = new Vector3(0, 3, 0);
    void Start()
    {
    }
    void Update()
    {   indices_index = 0;
        Random.InitState(seed);
        Vector3 offset = new Vector3(0, 2.5f, 0);
        List<Vector3> vertex_pos = new List<Vector3>(grass_number);
        List<Vector3> normals = new List<Vector3>(grass_number);
        int[] indicies = new int[grass_number];
        List<Color> colors = new List<Color>(grass_number);
        for (int i = 0; i < grass_number; i++)
        {
            Vector3 origin = transform.position;
            origin.y = heigth_start;
            origin.x += dimension.x * Random.Range(-0.5f, 0.5f);
            origin.z += dimension.y * Random.Range(-0.5f, 0.5f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.point.y>2.5 && (hit.normal.x + hit.normal.z)/2<1.2)
            {
                origin = hit.point;
                //origin -= offset;
                vertex_pos.Add(origin);
                indicies[indices_index] = indices_index; indices_index++;
                colors.Add(new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f));
                normals.Add(hit.normal+new Vector3(0,1,0 ));
            }
        }
        mesh_updated = new Mesh();
        if (indices_index != 0)
        {
            
            mesh_updated.SetVertices(vertex_pos);
            mesh_updated.SetIndices(indicies, MeshTopology.Points, 0);
            mesh_updated.SetColors(colors);
            mesh_updated.SetNormals(normals);
            
            
        }
        filter.mesh = mesh_updated;
    }
}
