using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    int maxX = 8;
    int maxY = 8;

    public GameObject tile;
    Tile[,] tiles = new Tile[8, 8];

    public GameObject pieceGO;
    Piece[] piece = new Piece[16];
   

    Ray ray;
    RaycastHit hit;

	// Use this for initialization
	void Start () 
    {
        
        SetupBoard();
        SetupPieces();
	}

	void SetupBoard()
    {
        for (int xx = 0; xx < maxX; xx++) for (int yy = 0; yy < maxY; yy++)
        {
            tiles[xx, yy] = new Tile();
            tiles[xx,yy].gameObject = (GameObject)Instantiate(tile,new Vector3(xx * 3, 0,yy*3), Quaternion.identity);

            
            //tiles[xx,yy] = new GameObject("tile " +xx + "," +yy);
             //        = 
            tiles[xx, yy].gameObject.SetActive(true);
            tiles[xx,yy].gameObject.name = "tile " + xx + "," + yy;
            tiles[xx, yy].id = new Vector2(xx, yy);

            int counter = xx + yy;
            if(counter % 2 == 0)
            {
                tiles[xx, yy].gameObject.transform.renderer.material.color = Color.black;
            }

            if (counter % 2 != 0)
            { 
                tiles[xx, yy].gameObject.transform.renderer.material.color = Color.white;
            }
        }
            
    }

    void SetupPieces()
    {
        piece[1] = new Piece();
        piece[1].gameObject = (GameObject)Instantiate(pieceGO, new Vector3(3, 1, 3), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //DO click shit.
            }
            
        }
	
	}


    //Doesnt work reliably.
    IEnumerator HighlightTile(GameObject tile)
    { 
        Color originalColor;
        float delay = 0.5f;
        originalColor = tile.transform.renderer.material.color;
        tile.transform.renderer.material.color = Color.red;
        yield return new WaitForSeconds(delay);
        tile.transform.renderer.material.color = originalColor;
        
    }
}
