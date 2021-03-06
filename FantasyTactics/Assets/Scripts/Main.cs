﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
    // TODO
    //Alpha******
    // **Basic Client
    //CheckMait logic.
    //Game Reset
    //Beta ((((((((((((((
    //server split.
    //Launcher Client (New client for game play?
    //Server 
    //Basic MM.

    int maxX = 8;
    int maxY = 8;

    enum GameState { waiting, playing, over };
    GameState game;

    enum InGameState { p1Moving, p2Moving, p1Up, p2Up, resultReady, resultPlaying }
    InGameState inGameState;

    public GameObject tile;
    
    Tile[,] tiles = new Tile[8, 8];

    public GameObject piecePawn;
    public GameObject pieceBishop;
    public GameObject pieceRook;
    public GameObject pieceKnight;
    public GameObject pieceQueen;
    public GameObject pieceKing;

    public AudioClip check;
    public AudioClip checkMate;
    public AudioClip victory;

    Piece[] pieces = new Piece[32];
   
    Ray ray;
    RaycastHit hit;

    Vector2 origin;
    Vector2 destination;

    bool readyToMove;

    bool promotionPossible;

    int whatPlayerAmI = 0;

    Piece.Type selectedType = Piece.Type.PAWN;

    bool playerOneReady;
    Vector2 p1Origin;
    Vector2 p1Destination;
    Piece p1Piece;
    Piece.Type p1PromoChoice;
    bool p1Promo;
    
    bool playerTwoReady;
    Vector2 p2Orgin;
    Vector2 p2Destination;
    Piece p2Piece;
    Piece.Type p2PromoChoice;
    bool p2Promo;

    bool p1Check;
    bool p2Check;

    bool p1Wins;
    bool p2Wins;
    bool tie;

    Vector2 p1EnPassantLocation;
    Vector2 p1EnPassantCapture;

    Vector2 p2EnPassantLocation;
    Vector2 p2EnPassantCapture;

    GUIStyle winStyle = new GUIStyle();

    GUIStyle waitingStyle = new GUIStyle();


	void Start () 
    {
        DefineStyles();
        SetupBoard();
        SetupPieces();
        ThreatCheck(tiles, pieces);
	}

    void Update()
    {
        
        GetInput();

        GameOn();

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

        switch (inGameState)
        {

            case InGameState.p1Up:

                PlayerOneUp();

                break;

            case InGameState.p2Up:

                PlayerTwoUP();

                break;

            case InGameState.p1Moving:

                whatPlayerAmI = 1;

                break;

            case InGameState.p2Moving:

                whatPlayerAmI = 2;

                break;

            case InGameState.resultReady:

                whatPlayerAmI = 0;
                ResultReady();

                break;

            case InGameState.resultPlaying:
                break;

        }


        if (promotionPossible) //Promo!
        {
            PromotionSelection();    
        }

        UserDebugStuff();

        //Later put in a config Menu

        if (GUI.Button(new Rect(10, Screen.height - 50, btnWidth, btnHeight), "Threat On/Off"))
        {
            foreach (Tile tile in tiles)
            {
                tile.showThreats = !tile.showThreats;
                ClearHighlights();
            }
        }

        if (p1Wins || p2Wins || tie)
        {
            GameOver();
        }

    }//OnGUI

    void DefineStyles()
    {
        winStyle.fontSize = 40;
        winStyle.normal.textColor = Color.yellow;
        winStyle.fontStyle = FontStyle.Bold;
        winStyle.alignment = TextAnchor.MiddleCenter;


        waitingStyle.fontSize = 40;
        waitingStyle.normal.textColor = Color.grey;
        waitingStyle.normal.background = MakeTex(1, 1, Color.white);
        waitingStyle.alignment = TextAnchor.UpperCenter;
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {

        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; ++i)
        {

            pix[i] = col;

        }

        Texture2D result = new Texture2D(width, height);

        result.SetPixels(pix);

        result.Apply();

        return result;

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
                tiles[xx, yy].StartColor(Color.black, true);
                //tiles[xx, yy].gameObject.transform.renderer.material.color = Color.black;
            }

            if (counter % 2 != 0)
            { 
                tiles[xx, yy].StartColor(Color.white, false);
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
                pieces[ii].playerColor = Color.red;
                pieces[ii].movedColor = new Color32(190, 120, 120, 255);
            }

            if(ii > 15)
            {
                pieces[ii].player = 1;
                pieces[ii].gameObject.transform.renderer.material.color = Color.blue;
                pieces[ii].playerColor = Color.blue;
                pieces[ii].movedColor = new Color32(110, 110, 170, 255);
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

    bool GameOn()
    {
        if (playerOneReady && playerTwoReady && inGameState == InGameState.resultPlaying)
        {

            UnMovePieces();

            CaptureAndMove();

            PromotePeices();

            ClearPromotion();

            UpdateTileOccupation(tiles, pieces);

            ClearThreats(tiles);

            ThreatCheck(tiles, pieces);

            p1Check = CheckCheck(1, pieces, tiles);

            p2Check = CheckCheck(2, pieces, tiles);

            IsItCheckMate();

            ClearHighlights();

            playerOneReady = false;

            playerTwoReady = false;

            return true;
        }
        return true;
    }

    void GameOver()
    {
        Rect fullScreen = new Rect(0,0,Screen.width,Screen.height);

        GUI.Box(fullScreen, "");

        if (p1Wins)
        {
            GUI.Box(fullScreen, "Victory! Player One", winStyle);
        }

        if (p2Wins)
        {
            GUI.Box(fullScreen, "Player Two Wins!", winStyle);
        }

        if (tie)
        {
            GUI.Box(fullScreen, "Draw, Everybody Loses!", winStyle);
        }

        //Record Stats.
        //Stop the game loop.
        //Discconect and close the server.

        if (GUI.Button(new Rect(new Rect((Screen.width / 2) - 120, Screen.height / 3, 240, 60)), "New Game"))
        {
            
        }

        if (GUI.Button(new Rect((Screen.width / 2) - 120, Screen.height / 4 * 3, 240, 60), "Exit Game"))
        {
            Application.Quit();
        }

    }

    #region GameStateGUI

    void PlayerOneUp()
    {
        Rect fullScreen = new Rect(0, 0, Screen.width, Screen.height);

        GUI.Box(fullScreen, "", waitingStyle);

        //Use a P1 Name Variable here.
        if (GUI.Button(new Rect(new Rect((Screen.width / 2) - 120, Screen.height / 3, 240, 60)), "I'm Ready!"))
        {
            whatPlayerAmI = 1;
            inGameState = InGameState.p1Moving;
        }
    }

    void PlayerTwoUP()
    {
        Rect fullScreen = new Rect(0, 0, Screen.width, Screen.height);

        GUI.Box(fullScreen, "", waitingStyle);

        //Use a P1 Name Variable here.
        if (GUI.Button(new Rect(new Rect((Screen.width / 2) - 120, Screen.height / 3, 240, 60)), "I'm Ready!"))
        {
            whatPlayerAmI = 2;
            inGameState = InGameState.p2Moving;
        }
    }

    void ResultReady()
    {
        Rect fullScreen = new Rect(0, 0, Screen.width, Screen.height);

        GUI.Box(fullScreen, "", waitingStyle);

        //Use a P1 Name Variable here.
        if (GUI.Button(new Rect(new Rect((Screen.width / 2) - 120, Screen.height / 3, 240, 60)), "I'm Ready!"))
        {
            whatPlayerAmI = 1;
            inGameState = InGameState.resultPlaying;
        }
    }

    #endregion

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
            
            //Find out what kind of piece I clicked in the list of pieces that exist.
            foreach (Piece piece in pieces)
            {
                if (hit.collider.gameObject.transform.position == piece.gameObject.transform.position)
                {

                    clickedPiece = piece;                  
                    //Debug.Log("Cliecked " + clickedPiece.type);
                }             
            }

            StorePiece(clickedPiece);

            //Make sure the piece is owned by the player.
            if (clickedPiece.player != whatPlayerAmI)
            { 
                int x = (int)clickedPiece.gameObject.transform.position.x / 3;
                int y = (int)clickedPiece.gameObject.transform.position.z / 3;

                clickTile(x, y);
            }

            if (clickedPiece.player == whatPlayerAmI && !clickedPiece.moved)
            {
                ClearHighlights();

                origin = clickedPiece.MyCoordinates();
                //Determin where that peice may move / attack

                List<Vector2> possibleMoves = new List<Vector2>();

                //TODO so right here I think all the non King Pieces need a filter
                //That will run with their moves and the kings COORDs to see if they 
                //Put him in check.
                switch (clickedPiece.type)
                {
                    case Piece.Type.PAWN:
                        possibleMoves = ClickedPieceMoves(clickedPiece);
                        break;

                    case Piece.Type.BISHOP:
                        possibleMoves = ClickedPieceMoves(clickedPiece);
                        break;

                    case Piece.Type.KNIGHT:
                        possibleMoves = ClickedPieceMoves(clickedPiece);
                        break;

                    case Piece.Type.ROOK:
                        possibleMoves = ClickedPieceMoves(clickedPiece);
                        break;

                    case Piece.Type.QUEEN:
                        possibleMoves = ClickedPieceMoves(clickedPiece);
                        break;

                    case Piece.Type.KING:
                        KingStuff(clickedPiece);

                        possibleMoves = ClickedPieceMoves(clickedPiece);
                        break;

                    default:
                        Debug.Log("Something wrong in piece type");
                        break;
                }//Piece selection

                if (whatPlayerAmI == 1 && p1Check)
                {
                    HighlightMoves(GetOutOfCheck(possibleMoves, clickedPiece));
                }

                if (whatPlayerAmI == 1 && !p1Check)
                {
                    HighlightMoves(IsTheKingPutInCheck(possibleMoves, clickedPiece));
                }

                if (whatPlayerAmI == 2 && p2Check)
                {
                    HighlightMoves(GetOutOfCheck(possibleMoves, clickedPiece));
                }

                if (whatPlayerAmI == 2 && !p2Check)
                {
                    HighlightMoves(IsTheKingPutInCheck(possibleMoves, clickedPiece));
                }
            }//Ownership Check
        }//Clicked a Piece

        if (hit.collider.gameObject.tag == tagTile)
        {     
            int x = (int)hit.collider.gameObject.transform.position.x / 3;
            int y = (int)hit.collider.gameObject.transform.position.z / 3;

            clickTile(x, y);
        }//Shoot tile.

    }//Left Click Logic

    public void clickTile(int x, int y)
    {
        //This is so we can click "through" a piece if needed. I'm passing coords not
        //Tiles[] just cuz. can change later if needed.

        Tile selectedTile = new Tile();

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

            //Later this will be on the UI and only one will be needed.
            if (whatPlayerAmI == 1)
            {
                PromotionCheck(1, destination, p1Piece.type);
            }

            if (whatPlayerAmI == 2)
            {
                PromotionCheck(2, destination, p2Piece.type);
            }
        }


    }

    void StorePiece(Piece clickedPiece)
    {
        if (clickedPiece.player == 1)
        {
            p1Piece = clickedPiece;
        }

        if (clickedPiece.player == 2)
        {
            p2Piece = clickedPiece;
        }
    }

    void CaptureAndMove()
    { 
        //Note on collisions: Peice A moves into where peice B is leaving is a collision iff
        //Piece B moving into the square Piece a is leaving as well (or has become stationary)?. 

        //TODO clean this up into 2-3 methods instead of one big multi tasker.
        
        int p1x1 = (int)p1Origin.x;
        int p1x2 = (int)p1Destination.x;
        int p1y1 = (int)p1Origin.y;
        int p1y2 = (int)p1Destination.y;
        Vector2 p1Direction;
        Piece.Type p1Type;
        Piece p1Piece = new Piece();


        int p2x1 = (int)p2Orgin.x;
        int p2x2 = (int)p2Destination.x;
        int p2y1 = (int)p2Orgin.y;
        int p2y2 = (int)p2Destination.y;
        Vector2 p2Direction;
        Piece.Type p2Type;
        Piece p2Piece = new Piece();

        p1Type = PieceType(p1x1, p1y1);
        p2Type = PieceType(p2x1, p2y1);

        bool pawnsBounced = false;

        int x = (int)p2Orgin.x;
        int y = (int)p2Orgin.y;

        foreach (Piece piece in pieces)
        {
            if (x == piece.MyCoordinates().x && y == (int)piece.MyCoordinates().y)
            {
                p2Piece = piece;
            }

            if (p1Origin == piece.MyCoordinates())
            {
                p1Piece = piece;
            }            
        }

        //Not sure if this is needed but leaving it for now.
        p1Piece.moved = true;
        p2Piece.moved = true;

        //Move the Knights
        if (p1Type != Piece.Type.KNIGHT)
        {
            p1Direction = new Vector2(LeftOrRight(p1x1, p1x2), UpOrDown(p1y1, p1y2));
        }
        else
        {
            p1Direction = KnightDirection(p1x1, p1x2, p1y1, p1y2);
        }

        if (p2Type != Piece.Type.KNIGHT)
        {
            p2Direction = new Vector2(LeftOrRight(p2x1, p2x2), UpOrDown(p2y1, p2y2));
        }
        else 
        {
            p2Direction = KnightDirection(p2x1, p2x2, p2y1, p2y2);
        }

        Vector2 p1Location = p1Origin;
        Vector2 p2Location = p2Orgin;

        //move non knights
        do
        {
            Vector2 p1FormerLocation = p1Location;
            Vector2 p2FormerLocation = p2Location;

            if (p1Location != p1Destination && p1Type != Piece.Type.KNIGHT)
            {
                p1Location += p1Direction;
            }
            else if(p1Type == Piece.Type.KNIGHT)
            {
                if (p1Direction.x == 0 && p1Location.y != p1Destination.y)
                {
                    p1Location += p1Direction;
                }
                else if (p1Location.y == p1Destination.y)
                {
                    p1Location = p1Destination;
                }
                if (p1Direction.y == 0 && p1Location.x != p1Destination.x)
                {
                    p1Location += p1Direction;
                }
                else if (p1Location.x == p1Destination.x)
                {
                    p1Location = p1Destination;
                }
            }

            if (p2Location != p2Destination && p2Type != Piece.Type.KNIGHT)
            {
                p2Location += p2Direction;
            }
            else if(p2Type == Piece.Type.KNIGHT)
            {
                if (p2Direction.x == 0 && p2Location.y != p2Destination.y)
                {
                    p2Location += p2Direction;
                }
                else if (p2Location.y == p2Destination.y)
                {
                    p2Location = p2Destination;
                }
                if (p2Direction.y == 0 && p2Location.x != p2Destination.x)
                {
                    p2Location += p2Direction;
                }
                else if (p2Location.x == p2Destination.x)
                {
                    p2Location = p2Destination;
                }
            }

            if (p1Location == p2FormerLocation && p2Location == p1FormerLocation)
            {
                //Mid Movement collision
                Debug.Log("Mid Movement collision detected");
                Debug.Log("Player 1 Location " + p1Location + " Player 2 location " + p2Location);
                if (p1Type == Piece.Type.KNIGHT && p2Type == Piece.Type.KNIGHT)
                {
                    CaptureLocation(p1Location);
                    CaptureLocation(p2Location);
                }

                if (p1Type != Piece.Type.KNIGHT && p2Type != Piece.Type.KNIGHT)
                {
                    Debug.Log("Non Knight collision");
                    CaptureLocation(p1Location);
                    CaptureLocation(p2Location);
                }
            }

            if (p1Location == p2Location)
            {
                //This is where head to head pawn collisions occure for both single and double move.
                //If I want this off just remove the top if and open up the else.
                if (p1Type == Piece.Type.PAWN || p2Type == Piece.Type.PAWN)
                {
                    Debug.Log("pawn collision detected");

                    if (p1Type == Piece.Type.PAWN && p2Type == Piece.Type.PAWN)
                    {
                        Debug.Log("pawn bounce");
                        pawnsBounced = true;
                    }
                    else if (p1Type == Piece.Type.PAWN)
                    {
                        CapturePiece(p1Piece);
                    }
                    else if (p2Type == Piece.Type.PAWN)
                    {
                        CapturePiece(p2Piece);
                    }
                }
                else
                {
                    Debug.Log("collision detected");

                    CapturePiece(p1Piece);

                    CapturePiece(p2Piece);
                }
            }

        }
        while (p1Location != p1Destination || p2Location != p2Destination);
        

        CaptureCheck(p1Destination);
        CaptureCheck(p2Destination);

        //Promotion Stuff
        if (p1Type == Piece.Type.PAWN && p1Destination.y == 7)
        {
            PromotionLogic(p1Piece, p1PromoChoice);
        }

        if (p2Type == Piece.Type.PAWN && p2Destination.y == 0)
        {
            Debug.Log(p2Piece.type + "p2 piece type, and player > " + p2Piece.player);
            PromotionLogic(p2Piece, p2PromoChoice);
        }

        if (pawnsBounced)
        {
            p1Destination.y--;
            p2Destination.y++;

            MoveGameObject(p1Origin, p1Destination, pieces);
            MoveGameObject(p2Orgin, p2Destination, pieces); //NOTE no longer think i need pieces
            //to be a feild in this method.
        }
        else if (p1Destination == p2Orgin)
        {
            MoveGameObject(p2Orgin, p2Destination, pieces);
            MoveGameObject(p1Origin, p1Destination, pieces );
        }
        else
        {
            MoveGameObject(p1Origin, p1Destination, pieces);
            MoveGameObject(p2Orgin, p2Destination, pieces);
        }

        //Is this really better than the way I did it below? NOTE to self: Yes it is.
        EnPassant(p1Type, 1, p1Origin, p1Destination);
        EnPassant(p2Type, 2, p2Orgin, p2Destination);

        EnPassantCapture(p1Type, p1Destination, p1EnPassantLocation, p1EnPassantCapture);
        EnPassantCapture(p2Type, p2Destination, p2EnPassantLocation, p2EnPassantCapture);


        //Castle Logic
        if (p1Type == Piece.Type.KING && p1Origin == new Vector2(4,0))
        {
            if (p1Destination == new Vector2(6, 0))
            {
                MoveGameObject(new Vector2(7, 0), new Vector2(5, 0), pieces);
            }

            if (p1Destination == new Vector2(2, 0))
            {
                MoveGameObject(new Vector2(0, 0), new Vector2(3, 0), pieces);
            }
        }

        if (p2Type == Piece.Type.KING && p2Orgin == new Vector2(4,7))
        {
            if (p2Destination == new Vector2(6, 7))
            {
                MoveGameObject(new Vector2(7, 7), new Vector2(5, 7), pieces);
            }

            if (p2Destination == new Vector2(2, 7))
            {
                MoveGameObject(new Vector2(0, 7), new Vector2(3, 7), pieces);
            }
        }



        //TODO LOG MOVES

    }//Move Pieces

    void PromotePeices() // Seems obsoleate.
    {
        // A there is no check the peice was not captured. This should move into caputre and move
        // or at least get a signal flag from here.
        if (p1Promo)
        {
            PromotionLogic(p1Piece, p1PromoChoice);
            Debug.Log("promo choice for p1 is " + p1PromoChoice);
        }

        if (p2Promo)
        {
            PromotionLogic(p2Piece, p2PromoChoice);
            Debug.Log("Prm Choice p2 is " + p2PromoChoice);
        }
    
    }

    void PromotionLogic(Piece piece, Piece.Type type)
    {
        //Going to instansiate a new game object and then clone a peice to it.
        //I will simply place the new peice in the pawns index.

        //CreateNewPiece();
    
        Debug.Log("type is " + type + " formerly a " + piece.type) ;

        for (int ii = 0; ii < pieces.Length; ii++)
        {
            if (pieces[ii] == piece)
            {
                pieces[ii] = ClonePieceAndPromote(piece, type);
            }
        }


        Debug.Log("Piece for player " + piece.player + " set to " + piece.type + " At pos " + piece.MyCoordinates());

    }

    Piece ClonePieceAndPromote(Piece piece, Piece.Type promotion)
    {
        Piece clone = new Piece();

        clone = DeclarePiece(promotion, 0, 0);
        clone.movedColor = piece.movedColor;
        clone.playerColor = piece.playerColor;
        clone.player = piece.player;

        clone.MoveGameObject(piece.position);

        piece.CapturePiece();

        return clone;
    }

    void UnMovePieces()
    {
        foreach (Piece piece in pieces)
        {
            piece.UnMovePiece();
        }
    }

    Piece.Type PieceType(int x, int y)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.MyCoordinates().x == x && piece.MyCoordinates().y == y)
            {
                return piece.type;
            }
        }
        Debug.Log("This should not happen!");
        return Piece.Type.KING;
    }

    void EnPassant(Piece.Type type, int player, Vector2 origin, Vector2 destination)
    {
        if (type == Piece.Type.PAWN)
        {
            if (Mathf.Abs(origin.y - destination.y) == 2)
            {
                int x1 = (int)destination.x + 1;
                int x2 = (int)destination.x - 1;
                int y = (int)destination.y;

                foreach (Piece piece in pieces)
                {
                    if (piece.player != player)
                    {
                        if (piece.type == Piece.Type.PAWN)
                        {
                            if (piece.MyCoordinates() == new Vector2(x1, y) ||
                                piece.MyCoordinates() == new Vector2(x2, y))
                            {
                                if (piece.player == 1)
                                {
                                    Vector2 location = new Vector2(destination.x, destination.y + 1);

                                    piece.EnPassantPossible(location);

                                    p1EnPassantLocation = location;

                                    p1EnPassantCapture = destination;
                                }

                                if (piece.player == 2)
                                {
                                    Vector2 location = new Vector2(destination.x, destination.y - 1);

                                    piece.EnPassantPossible(location);

                                    p2EnPassantLocation = location;

                                    p2EnPassantCapture = destination;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void EnPassantCapture(Piece.Type type, Vector2 destination, Vector2 enPassantLocation, Vector2 captureLocation)
    {
        if (type == Piece.Type.PAWN && destination == enPassantLocation)
        {
            CaptureLocation(captureLocation);
        }
    }

    #region movement stuff 

    List<Vector2> ClickedPieceMoves(Piece clickedPiece)
        //I now notice that passing the origin or piece.position is completly un needed
        //I'll leave it be for now but it may get moved out on the server split.
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        switch (clickedPiece.type)
        {
            case Piece.Type.PAWN:
                
                possibleMoves = clickedPiece.PawnMoves(clickedPiece.position, clickedPiece.player, tiles,
                    pieces);
                break;

            case Piece.Type.BISHOP:
                possibleMoves = clickedPiece.BishopMoves(clickedPiece.position, clickedPiece.player,
                    tiles, pieces);
                break;

            case Piece.Type.KNIGHT:
                possibleMoves = clickedPiece.KnightMoves(clickedPiece.position, clickedPiece.player,
                    tiles, pieces);
                break;

            case Piece.Type.ROOK:
                possibleMoves = clickedPiece.RookMoves(clickedPiece.position, clickedPiece.player,
                    tiles, pieces);
                break;

            case Piece.Type.QUEEN:
                possibleMoves = clickedPiece.QueenMoves(clickedPiece.position, clickedPiece.player,
                    tiles, pieces);
                break;

            case Piece.Type.KING:
                KingStuff(clickedPiece);

                possibleMoves = clickedPiece.KingMoves(clickedPiece.position, clickedPiece.player,
                    tiles, pieces);
                break;

            default:
                Debug.Log("Something wrong in piece type");
                break;
        }

        return possibleMoves;
    }

    Vector2 KnightDirection(int x1, int x2, int y1, int y2)
    {
        Vector2 direction = new Vector2();

        float dY = Mathf.Abs(y2 - y1);
        float dX = Mathf.Abs(x2 - x1);

        if (dY > dX)
        {
            if (y1 > y2)
            {
                direction = new Vector2(0, -1);
            }
            else if (y1 < y2)
            {
                direction = new Vector2(0, 1);
            }
        }
        else if (dX > dY)
        {
            if (x1 > x2)
            {
                direction = new Vector2(-1, 0);
            }
            else if (x1 < x2)
            {
                direction = new Vector2(1, 0);
            }
        
        }

        return direction;
    }

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

    void MoveGameObject(Vector2 origin, Vector2 destination, Piece[] pieceSet)
    {
        foreach (Piece piece in pieceSet)
        {
            if (piece.gameObject.transform.position.x / 3 == origin.x &&
                piece.gameObject.transform.position.z / 3 == origin.y)
            {
                piece.MoveGameObject(destination);
            }
        }
        //tiles[(int)origin.x, (int)origin.y].occupied = false;
        //tiles[(int)destination.x, (int)destination.y].occupied = true; //This causes an error on capture.
    }
    #endregion

    void CaptureCheck(Vector2 destination)
    {

        foreach (Piece piece in pieces)
        {
            if (piece.MyCoordinates() == destination && !piece.moved)
            {
                Debug.Log("capture check calls for a capture");
                CaptureLocation(destination);
            }
        }
    }

    void CaptureLocation(Vector2 location)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.MyCoordinates() == location)
            {
                CapturePiece(piece);
            }
        }
    }

    void CapturePiece(Piece piece)
    {
        //NOTE this really should be moved into pieces.
        piece.MoveGameObject( new Vector3(-10, -10, -10)) ; //may not be needed.
        piece.gameObject.SetActive(false);
        //tiles[(int)piece.MyCoordinates().x, (int)piece.MyCoordinates().y].occupied = false;
        piece.captured = true;
        Debug.Log("A " + piece.type + "for Player " + piece.player + " Has been captured");
    }

    void UpdateTileOccupation(Tile[,] tiles, Piece[] pieces)
    {
        foreach (Tile tile in tiles)
        {
            tile.occupied = false;
        }

        foreach (Piece piece in pieces)
        {
            if (piece.gameObject.activeInHierarchy)
            {
                tiles[(int)piece.MyCoordinates().x, (int)piece.MyCoordinates().y].occupied = true;
            }
        }
    }

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

    public void ClearHighlights()
    {
        foreach (Tile tile in tiles)
        {
            tile.UnHighlight();
        }
        origin = new Vector2(9, 9); 
        destination = new Vector2(9, 9); // setting these two to null really is not needed. but it might trap errors.
        readyToMove = false;
    }

    public void ConfirmMove()
    {
        if (whatPlayerAmI == 1)
        {
            playerOneReady = true;
            p1Destination = destination;
            p1Origin = origin;
            inGameState = InGameState.p2Up;
        }

        if (whatPlayerAmI == 2)
        {
            playerTwoReady = true;
            p2Destination = destination;
            p2Orgin = origin;
            inGameState = InGameState.resultReady;
        }
        ClearHighlights();
    }

    public void PromotionCheck(int player, Vector2 destination, Piece.Type type)
    {

        //This just does not work out.
        int row = (int)destination.y;

        if (player == 1 && row == 7 && type == Piece.Type.PAWN)
        {
            promotionPossible = true;
        }

        else if (player == 2 && row == 0 && type == Piece.Type.PAWN)
        {
            promotionPossible = true;
        }
        else
        {
            readyToMove = true;
        }
    }

    public void ClearPromotion()
    {
        p1PromoChoice = Piece.Type.PAWN;
        p2PromoChoice = Piece.Type.PAWN;
        selectedType = Piece.Type.PAWN;
        p1Promo = false;
        p2Promo = false;
        promotionPossible = false;
    }

    public void AbortMove()
    {
        ClearHighlights();
    }

    public void UserDebugStuff()
    {
        

        if (GUI.Button(new Rect(Screen.width - 130, 50, 120, 40), "BLUE Player"))
        {
            whatPlayerAmI = 1;
            ClearHighlights();
        }

        if (GUI.Button(new Rect(Screen.width - 130, 10, 120, 40), "RED Player"))
        {
            whatPlayerAmI = 2;
            ClearHighlights();
        }

        if (GUI.Button(new Rect(Screen.width - 130, 90, 40, 40), "P1 UP"))
        {
            inGameState = InGameState.p1Up;
            Debug.Log(inGameState);
        }

        if (GUI.Button(new Rect(Screen.width - 170, 90, 40, 40), "P2 UP"))
        {
            inGameState = InGameState.p2Up;
        }

        GUI.Label(new Rect(Screen.width - 130, 90, 120, 40), "I am player " + whatPlayerAmI);

        GUI.Label(new Rect(Screen.width - 130, 130, 120, 40), "Player 2 ready: " + playerTwoReady);
        GUI.Label(new Rect(Screen.width - 130, 170, 120, 40), "Player 1 ready: " + playerOneReady);
        GUI.Label(new Rect(Screen.width - 130, 210, 120, 40), "Player 1 check? " + p1Check);
        GUI.Label(new Rect(Screen.width - 130, 250, 120, 40), "Player 2 check? " + p2Check);
    }

    void PromotionSelection()
    { 
        int btnWidth = 120;
        int btnHeight = 40;



        if (GUI.Button(new Rect(10, 90, btnWidth, btnHeight), "Queen"))
        {
            selectedType = Piece.Type.QUEEN;
        }

        if (GUI.Button(new Rect(10, 130, btnWidth, btnHeight), "Knight"))
        {
            selectedType = Piece.Type.KNIGHT;
        }

        if (GUI.Button(new Rect(10, 170, btnWidth, btnHeight), "Rook"))
        {
            selectedType = Piece.Type.ROOK;
        }

        if (GUI.Button(new Rect(10, 210, btnWidth, btnHeight), "Bishop"))
        {
            selectedType = Piece.Type.BISHOP;
            Debug.Log("selected " + selectedType);
        }

        if (selectedType != Piece.Type.PAWN)
        {
            if (GUI.Button(new Rect(10, 250, btnWidth, btnHeight), "Confirm " + selectedType ))
            {
                Debug.Log("Selected a " + selectedType);
                if (whatPlayerAmI == 1)
                {
                    p1PromoChoice = selectedType;
                }

                if (whatPlayerAmI == 2)
                {
                    p2PromoChoice = selectedType;
                }

                promotionPossible = false;
                readyToMove = true;
                //Honestly Should probally just open confirm dialog here.
               
            }
        }
    }

    #region threat & check logic

    void ThreatCheck(Tile[,] tileSet, Piece[] pieceSet)
    {
        foreach (Piece piece in pieceSet)
        {
            //If I want moved pieces to not threaten it would go here ish. I think it is a bad idea though.
            if (!piece.captured)
            {
                switch (piece.type)
                {
                    case Piece.Type.PAWN:

                        RegisterThreat(piece.player, piece.PawnThreats(piece.MyCoordinates(), piece.player,
                            tileSet, pieces), tileSet);

                        break;

                    case Piece.Type.BISHOP:

                        RegisterThreat(piece.player, piece.BishopThreats(piece.MyCoordinates(), piece.player,
                            tileSet, pieces), tileSet);

                        break;

                    case Piece.Type.KNIGHT:

                        RegisterThreat(piece.player, piece.KnightThreats(piece.MyCoordinates(), piece.player,
                            tileSet, pieces), tileSet);

                        break;

                    case Piece.Type.ROOK:

                        RegisterThreat(piece.player, piece.RookThreats(piece.MyCoordinates(), piece.player,
                            tileSet, pieces), tileSet);

                        break;

                    case Piece.Type.QUEEN:

                        RegisterThreat(piece.player, piece.QueenThreats(piece.MyCoordinates(), piece.player,
                            tileSet, pieces), tileSet);

                        break;

                    case Piece.Type.KING:

                        RegisterThreat(piece.player, piece.KingThreats(piece.MyCoordinates(), piece.player,
                            tileSet, pieces), tileSet);

                        break;

                    default:
                        Debug.Log("Something wrong in piece type");
                        break;
                }
            }//If moved
        }//for each piece
    }

    public void RegisterThreat(int player, List<Vector2> threatList, Tile[,] tileSet)
    {
        foreach (Vector2 threat in threatList)
        {
            //Debug.Log(threatList.Count);
            Tile tile = tileSet[(int)threat.x, (int)threat.y];
            if(player == 1)
            {
                tile.p1Threat = true;
            }

            if (player == 2)
            {    
                tile.p2Threat = true;   
            }
            tile.Threatened();
            
        }
    }

    void ClearThreats(Tile[,] tiles)
    {
        foreach (Tile tile in tiles)
        {
            tile.p1Threat = false;
            tile.p2Threat = false;
        }
    }

    public void KingStuff(Piece piece)
    {
        //Removes the king momentarily from the board to 
        // Accurately check if his move would put him in check
        // Namely if he retreats along a line of attack.

        Piece[] virtualPieces = MakeVirtualPieces(pieces);

        Tile[,] virtualTiles = MakeVirtualTiles(tiles);

        int x = (int)piece.MyCoordinates().x;
        int y = (int)piece.MyCoordinates().y;

        //Remove the king in question for check check reasons.
        virtualTiles[x, y].occupied = false;

        ThreatCheck(virtualTiles, virtualPieces);

        virtualTiles[x, y].occupied = true;
        
    }

    public bool CheckCheck(int player, Piece[] pieceSet, Tile[,] tileSet)
    {
        //Simply Returns False if not in check and true if you are.
        bool check = false;

        foreach (Piece piece in pieceSet)
        {
            if (piece.type == Piece.Type.KING && piece.player == player)
            {
                int x = (int)piece.MyCoordinates().x;
                int y = (int)piece.MyCoordinates().y;

                //Debug.Log("Piece type + player " + tiles[x, y].p1Threat + " " + piece.player);

                if (player == 2)
                {
                    if (tileSet[x, y].p1Threat)
                    {
                        check = true;
                    }
                    else if (!tileSet[x, y].p1Threat)
                    {
                        check = false;
                    }
                }

                if (player == 1)
                {
                    if (tileSet[x, y].p2Threat)
                    {
                        check = true;
                    }
                    else if (!tileSet[x, y].p2Threat)
                    {
                        check = false;
                    }
                }
            }
        }
        return check;
    }

    List<Vector2> IsTheKingPutInCheck(List<Vector2> moves, Piece piece)
    {
        //In this method I am going to simulate the board state and then
        //Move the piece in question to see if its movement will open
        //an attack route to the king, if it does the move will be prohibited.

        List<Vector2> allowedMoves = new List<Vector2>();

        Tile[,] clonedTiles = MakeVirtualTiles(tiles);
        
        Piece[] clonedPieces = MakeVirtualPieces(pieces);

        Piece pieceClone = new Piece();

        //Deep Clones the Peice so I can move it around.
        foreach (Piece clonedPiece in clonedPieces)
        {
            if (clonedPiece.MyCoordinates() == piece.MyCoordinates())
            {
                pieceClone = clonedPiece;
            }
        }

        Vector2 origin = piece.MyCoordinates();

        foreach (Vector2 move in moves)
        {
            pieceClone.MovePosition(move);

            //These did not help
            clonedTiles[(int)origin.x, (int)origin.y].occupied = false;
            clonedTiles[(int)move.x, (int)move.y].occupied = true;

            ThreatCheck(clonedTiles, clonedPieces);

            if (!CheckCheck(pieceClone.player, clonedPieces, clonedTiles))
            {
                allowedMoves.Add(move);
            }

            pieceClone.MovePosition(origin);

            clonedTiles[(int)origin.x, (int)origin.y].occupied = true;
            clonedTiles[(int)move.x, (int)move.y].occupied = false;
        }

        return allowedMoves;
    }

    List<Vector2> GetOutOfCheck(List<Vector2> moves, Piece piece)
    {
        //NOTE problem with pawns captureing other pawns to remove the king from check
        //By the time I get here I get 6 make Virtual Piece calls gotta fix that. (Should be 4) TODO

        // The way I do this will be horribly inneficent I think. If performance becomes an issue
        //I should head here first. Most notably if I pass a list instead of a single peice when
        //Used to determin checkmate it will reduce the generation of virtual stuff to once
        //Instead of once per piece.

        //TODO we gotta add captures to the equation.

        List<Vector2> allowedMoves = new List<Vector2>();

        Tile[,] virtualTiles = MakeVirtualTiles(tiles);
        Piece[] virtualPieces = MakeVirtualPieces(pieces);
        Piece virtualPiece = new Piece();

        //Clones the piece in question
        foreach (Piece vPiece in virtualPieces)
        {
            if (piece.MyCoordinates() == vPiece.MyCoordinates())
            {
                virtualPiece = vPiece;
            }
        }

        Vector2 origin = virtualPiece.MyCoordinates();

        Debug.Log(moves.Count + " moves to check");
        foreach (Vector2 move in moves) 
        {
            foreach (Piece vPiece in virtualPieces)
            {
                if (vPiece.MyCoordinates() == move)
                {
                    Debug.Log("Captured @ " + move);
                    vPiece.captured = true;
                }
            }


            virtualPiece.MovePosition(move);

            UpdateTileOccupation(virtualTiles, virtualPieces);
            
            ClearThreats(virtualTiles);
            
            ThreatCheck(virtualTiles, virtualPieces); 

            if (!CheckCheck(virtualPiece.player, virtualPieces, virtualTiles))
            {
                allowedMoves.Add(move);
            }

            //Return Peice to location and try next move.
            virtualPiece.MovePosition(origin);
        }

        return allowedMoves;
    }

    void IsItCheckMate()
    {
        bool p1CheckMate = false;
        bool p2CheckMate = false;

        if (p1Check)
        {
           p1CheckMate = CheckMateCheck(1);
        }

        if (p2Check)
        {
            p2CheckMate = CheckMateCheck(2);
        }

        if (p1CheckMate && p2CheckMate)
        {
            Debug.Log("tie");
            tie = true;
        }
        else if (p1CheckMate)
        {
            Debug.Log("P2 wins");
            p2Wins = true;
        }
        else if (p2CheckMate)
        {
            Debug.Log("P1 wins");
            p1Wins = true;
        }
        else
        {
            Debug.Log("Game goes on");
        }
    }

    bool CheckMateCheck(int player)
    {
        foreach (Piece piece in pieces)
        {
            if (!piece.captured && piece.player == player)
            {
                List<Vector2> moves = GetOutOfCheck(ClickedPieceMoves(piece), piece);
                Debug.Log("Piece being checked is " + piece.type + " " + piece.gameObject.name);
                if (moves.Count > 0) //Seems like you get one extra move?
                {
                    Debug.Log("not check mate");
                    return false;
                }
            }
        }
        return true;
    }

    #endregion

    Tile[,] MakeVirtualTiles(Tile[,] tileSet)
    {
        Tile[,] virtualTiles = new Tile[8, 8];

        for (int xx = 0; xx < 8; xx++)
        {
            for (int yy = 0; yy < 8; yy++)
            {
                virtualTiles[xx, yy] = new Tile();
                virtualTiles[xx, yy].occupied = tileSet[xx, yy].occupied;
            }
        }
        return virtualTiles;
    }

    //At this moment I would really like to note that I probally should have just had peices
    //and tile combined, at the very least make peices a [,] and do away with bool occupied in tile.
    Piece[] MakeVirtualPieces(Piece[] pieceSet)
    {
        Piece[] virtualPieces = new Piece[32];
        
        for(int ii = 0; ii < 32; ii++)
        {
            virtualPieces[ii] = new Piece();

            virtualPieces[ii].gameObject = pieceSet[ii].gameObject;
            virtualPieces[ii].position = pieceSet[ii].position;
            virtualPieces[ii].type = pieceSet[ii].type;
            virtualPieces[ii].player = pieceSet[ii].player;
            virtualPieces[ii].moved = pieceSet[ii].moved;
        }

        return virtualPieces;
    }

}//Class     

