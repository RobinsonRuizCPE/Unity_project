using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
[ExecuteInEditMode]
public class TreeRender : MonoBehaviour
{
    public Mesh Tree_mesh;
    public Material Tree_material;

    public Vector2 dimension;
    [Range(1, 500)]
    public int tree_number;
    public float heigth_start = 200;
    public float Population = 20;
    public GameObject Procedural_tree;
    public TerrainProcedural _Terrain;
    private List<Matrix4x4> matrices;
    List<Mesh> Tree_List = new List<Mesh>();
    private Mesh TerrainMap;
    int[] subindex;
    void Start()
    {
    }
        // Update is called once per frame
        void Update()
    {
        Random.InitState(1);
        Vector3 offset = new Vector3(0, 2.5f, 0);
        List<Matrix4x4> matrices = new List<Matrix4x4>(tree_number);
        for (int i = 0; i < tree_number; i++)
        {
            Vector3 origin = transform.position;
            origin.y = heigth_start;
            origin.x += dimension.x * Random.Range(-0.5f, 0.5f);
            origin.z += dimension.y * Random.Range(-0.5f, 0.5f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.point.y > 2.5 && (hit.normal.x > 0.5 || hit.normal.z > 0.5))
            {
                matrices.Add(Matrix4x4.TRS(hit.point, Quaternion.identity, Vector3.one));

            }
        }     
         Graphics.DrawMeshInstanced(Tree_mesh, 0, Tree_material, matrices);
        }
    }



    // Update is called once per frame

