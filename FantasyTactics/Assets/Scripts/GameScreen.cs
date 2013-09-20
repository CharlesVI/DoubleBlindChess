using UnityEngine;
using System.Collections;

public class GameScreen : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
	    SetupTiles();
	}

    private void SetupTiles()
    {
        int yMax = 32; //Outputs grid 0-15, 0-31
        int xMax = 16;
        Tile[,] tile = new Tile[xMax, yMax];

        for (int xx = 0; xx < xMax; xx++) for (int yy = 0; yy < yMax; yy++)
            {
                tile[xx, yy] = new Tile();
                tile[xx, yy].position = new Vector2(xx, yy);
                //Debug.Log("Created a tile @ " + tile[xx, yy].position + "! XX,YY = " + xx + yy); //Works as expected.

            }

    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
