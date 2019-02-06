using UnityEngine;

public class CameraControl : MonoBehaviour {

    public TerrainProcedural Terrain;
    public float Speed;

    
	// Use this for initialization
	void Start ()
    {
        transform.position = new Vector3(0,10,-Terrain.ySize/2f + 3f);
        transform.rotation = Quaternion.identity;
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
    }
}
