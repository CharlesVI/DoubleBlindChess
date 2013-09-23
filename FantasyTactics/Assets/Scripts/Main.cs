using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{

    int maxX = 8;
    int maxY = 8;

    public GameObject tile;
    Tile[,] tiles = new Tile[8, 8];

    public GameObject pieceGO;
    Piece[] pieces = new Piece[32];
   
    Ray ray;
    RaycastHit hit;

    Vector2 origin;
    Vector2 destination;

    bool readyToMove;

    int whatPlayerAmI = 0;
    
    bool playerOneReady;
    Vector2 p1Origin;
    Vector2 p1Destination;
    
    bool playerTwoReady;
    Vector2 p2Orgin;
    Vector2 p2Destination;

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
                tiles[xx, yy].StartColor(Color.black);
                //tiles[xx, yy].gameObject.transform.renderer.material.color = Color.black;
            }

            if (counter % 2 != 0)
            { 
                tiles[xx, yy].StartColor(Color.white);
            }
        }
            
    }

    /// <summary>
    /// This is the Inital set up of a standard chess board. Nothing really clever happens here and its a
    /// Huge section. But it works and I got it done fast so thats good.
    /// </summary>
    void SetupPieces()
    {
        for (int ii = 0; ii < 32; ii++)
        { 
            pieces[ii] = new Piece();
            pieces[ii].gameObject = (GameObject)Instantiate(pieceGO, new Vector3(0, 0, 0), Quaternion.identity);
            pieces[ii].gameObject.name = "Piece " + ii;

            if(ii < 16)
            {
                pieces[ii].player = 2;
                pieces[ii].gameObject.transform.renderer.material.color = Color.red;
            }

            if(ii > 15)
            {
                pieces[ii].player = 1;
                pieces[ii].gameObject.transform.renderer.material.color = Color.blue;
            }
        }

        for (int xx = 0; xx < 8; xx++) for (int yy = 0; yy < 2; yy++)
            {
                //Debug.Log(xx + "," + yy + " set to occupied");
                tiles[xx, yy].occupied = true;
            }

        for (int xx = 0; xx < 8; xx++) for (int yy = 6; yy < 8; yy++)
            {
                //Debug.Log(xx + "," + yy + " set to occupied");
                tiles[xx, yy].occupied = true;
            }

        //If I run this the other way around I can set occupied to false as well. Or I could just handle that
        //as a part of the movement procedure. No idea what would be more cost effective or reliable.


        
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
        GetInput();
	}

    void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 200.0f))
            {
                LeftClickLogic(hit);
            }  
        }
    }//GetInput


    void LeftClickLogic(RaycastHit hit)
    {
        string tagPiece = "Piece";
        string tagTile = "Tile";

        if (hit.collider.gameObject.tag == tagPiece)
        {
            Piece clickedPiece = new Piece();

            Clear();
            
            //Find out what kind of piece I clicked in the list of pieces that exist.
            foreach (Piece piece in pieces)
            {
                if (hit.collider.gameObject.transform.position == piece.gameObject.transform.position)
                {
                    clickedPiece = piece;
                    
                }
            }

            //Make sure the piece is owned by the player.
            if (clickedPiece.player == whatPlayerAmI)
            {
                //Presently this could be converted into a vector2 from what I see. really should stop using world
                //coords as board ones... lots of /3 errors.
                Vector3 position = clickedPiece.gameObject.transform.position;

                origin = new Vector2(position.x / 3, position.z / 3);

                //Determin where that peice may move / attack
                switch (clickedPiece.type)
                {
                    case Piece.Type.PAWN:
                        HighlightMoves(clickedPiece.PawnMoves(position, clickedPiece.player, tiles));
                        break;

                    case Piece.Type.BISHOP:
                    case Piece.Type.KNIGHT:
                    case Piece.Type.ROOK:
                    case Piece.Type.QUEEN:
                    case Piece.Type.KING:

                    default:
                        Debug.Log("Something wrong in piece type");
                        break;
                }
            }//Ownership Check
        }


        if (hit.collider.gameObject.tag == tagTile)
        {
            Tile selectedTile = new Tile();

            int x = (int)hit.collider.gameObject.transform.position.x / 3;
            int y = (int)hit.collider.gameObject.transform.position.z / 3;

            selectedTile = tiles[x, y];

            //If its a blue tile set it as the intended destination and clear any other
            //possible destination
            if (selectedTile.moveable)
            {
                foreach (Tile tile in tiles)
                {
                    if (tile.destination)
                    {
                        tile.Moveable();
                    }
                }

                selectedTile.Destination();
                destination = new Vector2(x, y);
                readyToMove = true;
            }           
        }
    }//Left Click Logic

    public void HighlightMoves(List<Vector2> moveList)
    {
        foreach( Vector2 move in moveList)
        {
            tiles[(int)move.x, (int)move.y].Moveable();
        }
    }

    //I could probally save the list and only undo the tiles that are actually highlighted
    //However I dont think it will make a performance diffrence and it might even be an
    //advantage not having to go through multipul things and refrence stuff.
    public void Clear()
    {
        foreach (Tile tile in tiles)
        {
            tile.UnHighlight();
        }
        origin = new Vector2(9, 9); 
        destination = new Vector2(9, 9); // setting these two to null really is not needed. but it might trap errors.
        readyToMove = false;
    }

    void OnGUI()
    {
        float btnHeight = 40;
        float btnWidth = 120;

        if (readyToMove)
        {
            if (GUI.Button(new Rect(10, 10, btnWidth, btnHeight), "Confirm Move"))
            {
                ConfirmMove();
            }
            if (GUI.Button(new Rect(10, 50, btnWidth, btnHeight), "No don't do it!"))
            {
                AbortMove();
            }          
        }
        PlayerSelector();
    }//OnGUI

    public void ConfirmMove()
    {
        Debug.Log("MOVE CONFIRMED!");
        Debug.Log("We are going from " + origin);
        Debug.Log("To " + destination);
        Clear();
    }
    public void AbortMove()
    {
        Clear();
    }

    public void PlayerSelector()
    {
        GUI.Label(new Rect(Screen.width - 130, 90, 120, 40), "I am player " + whatPlayerAmI);

        if (GUI.Button(new Rect(Screen.width - 130, 10, 120, 40), "Player 1"))
        {
            whatPlayerAmI = 1;
        }
        if (GUI.Button(new Rect(Screen.width - 130, 50, 120, 40), "Player 2"))
        {
            whatPlayerAmI = 2;
        }
    }
}//Class     

