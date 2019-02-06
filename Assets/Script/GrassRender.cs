using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class GrassRender : MonoBehaviour {

    public Mesh Grass_mesh;
    public Material Grass_material;

    public Vector2 dimension;
    [Range(1,1000)]
    public int grass_number;
    public float heigth_start = 200;
    public int seed;
    private List<Matrix4x4> matrices;
    // Update is called once per frame
    void Update () {
        Random.InitState(seed);
        Vector3 offset = new Vector3(0, 0.0f, 0);
        matrices = new List<Matrix4x4>(grass_number);
        for(int i = 0; i < grass_number; i++) {
            Vector3 origin = transform.position;
            origin.y = heigth_start;
            origin.x += dimension.x * Random.Range(-0.5f, 0.5f);
            origin.z += dimension.y * Random.Range(-0.5f, 0.5f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && 4.0f < hit.point.y && hit.point.y < 15f )
            {
                origin = hit.point +offset;
                matrices.Add(Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one));
            }
        }
        Graphics.DrawMeshInstanced(Grass_mesh, 0, Grass_material, matrices);
	}
}
