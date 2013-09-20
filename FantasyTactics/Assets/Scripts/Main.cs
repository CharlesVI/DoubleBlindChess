using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour 
{

    const int yMax = 16; //Outputs grid 0-15, 0-31
    const int xMax = 8;
    Tile[,] tile = new Tile[xMax, yMax];

    FContainer board;

	// Use this for initialization
	void Start ()
    {
        FutileParams fprams = new FutileParams(true, true, false, false);
        
        fprams.AddResolutionLevel(480.0f, 1.0f, 1.0f, "");
        
        fprams.origin = new Vector2(0.5f,0.5f);
        
        Futile.instance.Init(fprams);

        Futile.atlasManager.LoadAtlas("Atlases/FantasyTacticsAtlas");
       
        Futile.stage.AddChild(board = new FContainer());

        //board.x = Futile.screen.halfWidth;
        //board.y = Futile.screen.halfHeight;

        SetupTiles();

        Debug.Log("x,y of 0,0 = " + tile[0, 10].sprite.x + " " + tile[0, 10].sprite.y);
	}

    public void SetupTiles()
    {


        for (int xx = 0; xx < xMax; xx++) for (int yy = 0; yy < yMax; yy++)
         {
            tile[xx, yy] = new Tile();
            tile[xx, yy].position = new Vector2(xx, yy);
            tile[xx, yy].sprite = new FSprite("DesertTile");
            
            board.AddChild(tile[xx, yy].sprite);

            tile[xx, yy].sprite.scale = 0.25f;
   
            tile[xx, yy].sprite.x = xx * tile[xx, yy].sprite.width;
               
            tile[xx, yy].sprite.y = -yy * tile[xx, yy].sprite.height;



            

            /*
                if (tile[xx, yy].sprite == null)
                {
                    Debug.Log("error is here"); //Seems like it is not here.
                }
              */ 
                //Debug.Log("Created a tile @ " + tile[xx, yy].position + "! XX,YY = " + xx + yy); //Works as expected.

         }

    }

    // Update is called once per frame
    void Update() 
    {
        GetInput();
	}

    void GetInput()
    {
        int scrollSpeed =2;

        if (Input.GetKey(KeyCode.UpArrow)) { board.y-= scrollSpeed; }
        if (Input.GetKey(KeyCode.DownArrow)) { board.y+= scrollSpeed; }
        if (Input.GetKey(KeyCode.LeftArrow)) { board.x+= scrollSpeed; }
        if (Input.GetKey(KeyCode.RightArrow)) { board.x-= scrollSpeed; }
    }
}
