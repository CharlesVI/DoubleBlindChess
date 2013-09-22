using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    int maxX = 8;
    int maxY = 8;

    public GameObject tile;
    Tile[,] tiles = new Tile[8, 8];

    public GameObject pieceGO;
    Piece[] pieces = new Piece[32];
   

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
        for (int ii = 0; ii < 32; ii++)
        { 
            pieces[ii] = new Piece();
            pieces[ii].gameObject = (GameObject)Instantiate(pieceGO, new Vector3(0, 0, 0), Quaternion.identity);

            if(ii < 16)
            {
                pieces[ii].player = 1;
                pieces[ii].gameObject.transform.renderer.material.color = Color.red;
            }

            if(ii > 15)
            {
                pieces[ii].player = 2;
                pieces[ii].gameObject.transform.renderer.material.color = Color.blue;
            }
        }
        
        //At this point I'm just going to maunally set the pieces up. Either it will always be this set up or some level of custom work will be involved and i'll loop through a list per player.
        
        //Player 1 This really should be a list. However there have been issues with lists interacting
        //refrencing all one thing and i'm comfortable with the array.
        int nn = 0;
        pieces[nn].SetPositionOneAbove(tiles[7, 7].gameObject);
        pieces[nn].type = Piece.Type.ROOK;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[6, 7].gameObject);
        pieces[nn].type = Piece.Type.KNIGHT;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[5, 7].gameObject);
        pieces[nn].type = Piece.Type.BISHOP;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[4, 7].gameObject);
        pieces[nn].type = Piece.Type.KING;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[3, 7].gameObject);
        pieces[nn].type = Piece.Type.QUEEN;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[2, 7].gameObject);
        pieces[nn].type = Piece.Type.BISHOP;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[1, 7].gameObject);
        pieces[nn].type = Piece.Type.KNIGHT;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[0, 7].gameObject);
        pieces[nn].type = Piece.Type.ROOK;
        nn++;

        for(int ii = 0; ii < 8; ii ++)
        {
            pieces[nn].SetPositionOneAbove(tiles[ii, 6].gameObject);
            pieces[nn].type = Piece.Type.PAWN;
            nn++;
        }

        pieces[nn].SetPositionOneAbove(tiles[7, 0].gameObject);
        pieces[nn].type = Piece.Type.ROOK;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[6, 0].gameObject);
        pieces[nn].type = Piece.Type.KNIGHT;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[5, 0].gameObject);
        pieces[nn].type = Piece.Type.BISHOP;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[4, 0].gameObject);
        pieces[nn].type = Piece.Type.KING;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[3, 0].gameObject);
        pieces[nn].type = Piece.Type.QUEEN;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[2, 0].gameObject);
        pieces[nn].type = Piece.Type.BISHOP;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[1, 0].gameObject);
        pieces[nn].type = Piece.Type.KNIGHT;
        nn++;

        pieces[nn].SetPositionOneAbove(tiles[0, 0].gameObject);
        pieces[nn].type = Piece.Type.ROOK;
        nn++;

        for (int ii = 0; ii < 8; ii++)
        {
            pieces[nn].SetPositionOneAbove(tiles[ii, 1].gameObject);
            pieces[nn].type = Piece.Type.PAWN;
            nn++;
        }
    }
       

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 200.0f))
            {
                string tagPiece = "Piece";
                string tagTile = "Tile";

                if (hit.collider.gameObject.tag == tagPiece)
                {
                    Debug.Log("I hit a piece");
                    Debug.Log("it is at " + hit.collider.gameObject.transform.position + "and is a " + hit.collider.gameObject.tag);
                    foreach(Piece piece in pieces)
                    {
                        if (hit.collider.gameObject.transform.position == piece.gameObject.transform.position)
                        {
                            Debug.Log("I am a " + piece.type);
                        }
                    }
                }

                if (hit.collider.gameObject.tag == tagTile)
                {
                    Debug.Log("shot a tile");
                }
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
