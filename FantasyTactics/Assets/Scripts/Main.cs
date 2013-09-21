using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    int maxX = 8;
    int maxY = 8;

    public GameObject tile;
    GameObject[,] tiles = new GameObject[8, 8];

    Ray ray;
    RaycastHit hit;

	// Use this for initialization
	void Start () 
    {
        
        SetupBoard();
	}

	void SetupBoard()
    {
        for (int xx = 0; xx < maxX; xx++) for (int yy = 0; yy < maxY; yy++)
        {
            //tiles[xx,yy] = new GameObject("tile " +xx + "," +yy);
            tiles[xx, yy] = (GameObject)Instantiate(tile,new Vector3(xx * 3, 0,yy*3), Quaternion.identity);
            tiles[xx, yy].SetActive(true);
            tiles[xx,yy].name = "tile " + xx + "," + yy;

            int counter = xx + yy;
            if(counter % 2 == 0)
            {
                tiles[xx, yy].transform.renderer.material.color = Color.black;
            }

            if (counter % 2 != 0)
            { 
                tiles[xx, yy].transform.renderer.material.color = Color.white;
            }
        }
            
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                hit.collider.renderer.material.color = Color.red;
                Debug.Log(hit.transform.name);
                //Debug.Log(hit);
            }
            
        }
	
	}
}
