using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraControl : MonoBehaviour {

    public TerrainProcedural Terrain;
    public float Speed;
    public Transform targetPos; // camera target
    public TreeRender Tree_rend;
    List<Matrix4x4> mat_tree=new List<Matrix4x4>(64);
    // Use this for initialization
    void Start ()
    {
        //transform.position = new Vector3(30,50,-Terrain.ySize/2f + 3f);
        //transform.rotation = Quaternion.identity;
        //transform.LookAt(Vector3.zero);

	}

    void MoveTerrain()
    {
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");
        float offsetX = Terrain.offsetXOct0;
        float offsetY = Terrain.offsetYOct0;

        offsetX += xMove * Time.deltaTime * Speed;
        offsetY += zMove * Time.deltaTime* Speed;

        Terrain.offsetXOct0 = Terrain.offsetXOct1 = Terrain.offsetXOct2 = offsetX;
        Terrain.offsetYOct0 = Terrain.offsetYOct1 = Terrain.offsetYOct2 = offsetY;

    }
	
	// Update is called once per frame
	void Update ()
    {
        MoveTerrain();
       /* if (Input.anyKeyDown) {
            switch (Input.inputString)
            {

                case "up":
                    mat_tree = Tree_rend.matrices.GetRange(0,64);
                    Tree_rend.matrices.RemoveRange(0, 64);
                    Tree_rend.matrices.AddRange( mat_tree);
                    break;

            }
        }*/
        if (targetPos != null) { transform.LookAt(targetPos); }
    }
}
