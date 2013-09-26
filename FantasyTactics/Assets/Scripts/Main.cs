using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{

    int maxX = 8;
    int maxY = 8;

    public GameObject tile;
    Tile[,] tiles = new Tile[8, 8];

    public GameObject piecePawn;
    public GameObject pieceBishop;
    public GameObject pieceRook;
    public GameObject pieceKnight;
    public GameObject pieceQueen;
    public GameObject pieceKing;

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
        for (int ii = 0; ii < 8; ii++)
        {
            pieces[ii] = DeclarePiece(Piece.Type.PAWN, ii, 6);        
        }

        //Ugh
        pieces[8] = DeclarePiece(Piece.Type.ROOK, 0, 7);
        pieces[9] = DeclarePiece(Piece.Type.ROOK, 7, 7);
        pieces[10] = DeclarePiece(Piece.Type.KNIGHT, 1, 7);
        pieces[11] = DeclarePiece(Piece.Type.KNIGHT, 6, 7);
        pieces[12] = DeclarePiece(Piece.Type.BISHOP, 5, 7);
        pieces[13] = DeclarePiece(Piece.Type.BISHOP, 2, 7);
        pieces[14] = DeclarePiece(Piece.Type.QUEEN, 3, 7);
        pieces[15] = DeclarePiece(Piece.Type.KING, 4, 7);


        pieces[24] = DeclarePiece(Piece.Type.ROOK, 0, 0);
        pieces[25] = DeclarePiece(Piece.Type.ROOK, 7, 0);
        pieces[26] = DeclarePiece(Piece.Type.KNIGHT, 1, 0);
        pieces[27] = DeclarePiece(Piece.Type.KNIGHT, 6, 0);
        pieces[28] = DeclarePiece(Piece.Type.BISHOP, 5, 0);
        pieces[29] = DeclarePiece(Piece.Type.BISHOP, 2, 0);
        pieces[30] = DeclarePiece(Piece.Type.QUEEN, 3, 0);
        pieces[31] = DeclarePiece(Piece.Type.KING, 4, 0);

        for(int ii = 0; ii < 8; ii++)
        {
            pieces[ii + 16] = DeclarePiece(Piece.Type.PAWN, ii, 1);
        }

        for (int ii = 0; ii < 32; ii++)
        {
            pieces[ii].gameObject.name = "Piece" + ii;

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
    }

    public Piece DeclarePiece(Piece.Type type, int x, int y)
    { 
        Piece piece = new Piece();

        switch (type)
        {
            case Piece.Type.PAWN:

                piece.gameObject = (GameObject)Instantiate(piecePawn, Vector3.zero, Quaternion.identity);
                piece.type = Piece.Type.PAWN;  
          
                break;

            case Piece.Type.BISHOP:

                piece.gameObject = (GameObject)Instantiate(pieceBishop, Vector3.zero, Quaternion.identity);
                piece.type = Piece.Type.BISHOP;

                break;

            case Piece.Type.KNIGHT:

                piece.gameObject = (GameObject)Instantiate(pieceKnight, Vector3.zero, Quaternion.identity);
                piece.type = Piece.Type.KNIGHT;

                break;

            case Piece.Type.ROOK:

                piece.gameObject = (GameObject)Instantiate(pieceRook, Vector3.zero, Quaternion.identity);
                piece.type = Piece.Type.ROOK;

                break;

            case Piece.Type.QUEEN:

                piece.gameObject = (GameObject)Instantiate(pieceQueen, Vector3.zero, Quaternion.identity);
                piece.type = Piece.Type.QUEEN;

                break;

            case Piece.Type.KING:

                piece.gameObject = (GameObject)Instantiate(pieceKing, Vector3.zero, Quaternion.identity);
                piece.type = Piece.Type.KING;

                break;
        }

        piece.SetPositionOneAbove(tiles[x,y].gameObject);

        return piece;
    }

       

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (playerOneReady && playerTwoReady)
        {
            Debug.Log("Move called");
            ResolveTurn();

            playerOneReady = false;
            playerTwoReady = false;
        }
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

    void ResolveTurn()
    { 
        //Note on collisions: Peice A moves into where peice B is leaving is a collision iff
        //Piece B moving into the square Piece a is leaving as well (or has become stationary)?. 

        //TODO clean this up into 2-3 methods instead of one big multi tasker.
        
        int p1x1 = (int)p1Origin.x;
        int p1x2 = (int)p1Destination.x;
        int p1y1 = (int)p1Origin.y;
        int p1y2 = (int)p1Destination.y;
        Vector2 p1Direction;

        int p2x1 = (int)p2Orgin.x;
        int p2x2 = (int)p2Destination.x;
        int p2y1 = (int)p2Orgin.y;
        int p2y2 = (int)p2Destination.y;
        Vector2 p2Direction;


        p1Direction = new Vector2(LeftOrRight(p1x1,p1x2), UpOrDown(p1y1, p1y2));
        p2Direction = new Vector2(LeftOrRight(p2x1, p2x2), UpOrDown(p2y1, p2y2));
        
        Vector2 p1Location = p1Origin;
        Vector2 p2Location = p2Orgin;

        do
        {
            Vector2 p1FormerLocation = p1Location;
            Vector2 p2FormerLocation = p2Location;

            if (p1Location != p1Destination)
            {
                p1Location += p1Direction;
            }

            if (p2Location != p2Destination)
            {
                p2Location += p2Direction;
            }

            Debug.Log("Player 1's location is " + p1Location);
            Debug.Log("Player 2's location is " + p2Location);

            if (p1Location == p2FormerLocation && p2Location == p1FormerLocation)
            { 
                //Mid Movement collision
                Debug.Log("Mid Movement collision detected");
            }

            if (p1Location == p2Location)
            {
                Debug.Log("collision detected");
                //Is the mid movement method needed?
            }
   
        }
        while (p1Location != p1Destination || p2Location != p2Destination);

        CaptureCheck(p1Destination);
        CaptureCheck(p2Destination);

        MovePieces(p1Origin, p1Destination);
        MovePieces(p2Orgin, p2Destination);

        //ebug.Log("Log Moves Here");
    }//Move Pieces

    int LeftOrRight(int x1, int x2)
    {
        if (x1 == x2)
        {
            return 0;
        }
        else if (x1 > x2)
        {
            return -1;     
        }
        else if (x1 < x2)
        {
            return 1;
        }
        else
        {
            Debug.Log("someting happened in left or right");
            return 2;
        }
        

    }

    int UpOrDown(int y1, int y2)
    {
        if (y1 == y2)
        {
            return 0;
        }
        else if (y1 > y2)
        {
            return -1;
        }
        else if (y1 < y2)
        {
            return 1;
        }
        else
        {
            Debug.Log("someting happened in up or down");
            return 2;
        }
    }

    void CaptureCheck(Vector2 destination)
    {
        foreach (Piece piece in pieces)
        {
            if (new Vector2(piece.gameObject.transform.position.x / 3,
                piece.gameObject.transform.position.z / 3) == destination)
            {
                Debug.Log("Piece captured at " + destination);
            }
        }
    }

    void MovePieces(Vector2 origin, Vector2 destination)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.gameObject.transform.position.x / 3 == origin.x &&
                piece.gameObject.transform.position.z / 3 == origin.y)
            {
                piece.MovePieceTo(destination);
            }
        }
        tiles[(int)origin.x, (int)origin.y].occupied = false;
        tiles[(int)destination.x, (int)destination.y].occupied = true;
    }

    void CapturePiece()
    { }

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
                        //TODO attack and enpassant 
                        HighlightMoves(clickedPiece.PawnMoves(origin, clickedPiece.player, tiles,
                            pieces));
                        break;

                    case Piece.Type.BISHOP:
                        HighlightMoves(clickedPiece.BishopMoves(origin, clickedPiece.player,
                            tiles, pieces));
                        break;

                    case Piece.Type.KNIGHT:
                        //TODO knight logic. Once movement is fully tested I'll tackle this special guy.
                        Debug.Log("I cant move yet!");
                        break;

                    case Piece.Type.ROOK:
                        HighlightMoves(clickedPiece.RookMoves(origin, clickedPiece.player,
                            tiles, pieces));
                        break;

                    case Piece.Type.QUEEN:
                        HighlightMoves(clickedPiece.QueenMoves(origin, clickedPiece.player,
                            tiles, pieces));
                        break;

                    case Piece.Type.KING:
                        HighlightMoves(clickedPiece.KingMoves(origin, clickedPiece.player,
                            tiles, pieces));
                        break;

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
        if (whatPlayerAmI == 1)
        {
            playerOneReady = true;
            p1Destination = destination;
            p1Origin = origin;
        }

        if (whatPlayerAmI == 2)
        {
            playerTwoReady = true;
            p2Destination = destination;
            p2Orgin = origin;
        }
        Clear();
    }

    public void AbortMove()
    {
        Clear();
    }

    public void PlayerSelector()
    {
        

        if (GUI.Button(new Rect(Screen.width - 130, 10, 120, 40), "Player 1"))
        {
            whatPlayerAmI = 1;
            Clear();
        }

        if (GUI.Button(new Rect(Screen.width - 130, 50, 120, 40), "Player 2"))
        {
            whatPlayerAmI = 2;
            Clear();
        }

        GUI.Label(new Rect(Screen.width - 130, 90, 120, 40), "I am player " + whatPlayerAmI);

        GUI.Label(new Rect(Screen.width - 130, 130, 120, 40), "Player 2 ready: " + playerTwoReady);
        GUI.Label(new Rect(Screen.width - 130, 170, 120, 40), "Player 1 ready: " + playerOneReady);
    }
}//Class     

