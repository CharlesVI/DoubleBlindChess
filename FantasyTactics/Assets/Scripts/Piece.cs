using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Piece
{
    public GameObject gameObject;
    public enum Type { PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING };
    public Type type;
    public int player;
    public Color color;
    public bool canMove;
    public bool active; //I think this is un Needed.

    public Material pawn;
    public Material rook;
    public Material knight;
    public Material bishop;
    public Material queen;
    public Material king;

    public void SetPositionOneAbove(GameObject go)
    {
        this.gameObject.transform.position = new Vector3(go.transform.position.x,
            go.transform.position.y + 1, go.transform.position.z);
    }

    public void MovePieceTo(Vector2 destination)
    {
        gameObject.transform.position = new Vector3(destination.x * 3, 1, destination.y * 3);
        canMove = false;
    }

    public Vector2 MyCoordinates()
    {
        Vector2 location = new Vector2(gameObject.transform.position.x /
            3, gameObject.transform.position.z / 3);

        return location;
    }

    //Second Note: Starting to think that I could just passes peice type and lowered redundancy
    //A lot here. Not sure if it would be that much better though.

    //NOTE TO ME POSSIBLY ADD HIGHLIGHTING HERE TO MAKE IT MORE CLEAR WHAT PIECE WAS CLICKED?

    //No capture logic has been implimented yet.
    public List<Vector2> PawnMoves(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        //I guess it is possible that going from float to int can introduce an error here and it may be
        //better to make the ints floats. However They should be real numbers and therfore a non issue in
        //this case.
        int x = (int)position.x;
        int y = (int)position.y;

        if (player == 1)
        {
            if (!tiles[x, y + 1].occupied)
            {
                possibleMoves.Add(new Vector2(x, y + 1));

                if (y == 1 && !tiles[x, y + 2].occupied)
                {
                    possibleMoves.Add(new Vector2(x, y + 2));
                }
            }

        }

        if (player == 2)
        {
            if (!tiles[x, y - 1].occupied)
            {
                possibleMoves.Add(new Vector2(x, y - 1));

                if (y == 6 && !tiles[x, y - 2].occupied)
                {
                    possibleMoves.Add(new Vector2(x, y - 2));
                }
            }

        }

        return possibleMoves;
    }

    //Recetnly added Pieces to the list of passed things. Makes me think tiles is not needed.
    public List<Vector2> RookMoves(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 1, 0));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, -1, 0));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 0, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 0, -1));

        return possibleMoves;
    }

    public List<Vector2> KnightMoves(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        return possibleMoves;
    }

    public List<Vector2> BishopMoves(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 1, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, -1, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 1, -1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, -1, -1));

        return possibleMoves;
    }

    public List<Vector2> QueenMoves(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 1, 0));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, -1, 0));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 0, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 0, -1));

        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 1, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, -1, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, 1, -1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 8, -1, -1));

        return possibleMoves;
    }

    //TODO add check check and castle.
    public List<Vector2> KingMoves(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, 1, 0));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, -1, 0));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, 0, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, 0, -1));

        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, 1, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, -1, 1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, 1, -1));
        possibleMoves.AddRange(LineMoves(position, player, tiles, pieces, 1, -1, -1));

        return possibleMoves;
    }

    //List<Vector2> UpMoves(Vector2 position, int player,
    //    Tile[,] tiles, Piece[] pieces, int maxDistance)
    //{
    //    List<Vector2> upMoves = new List<Vector2>();

    //    int x = (int)position.x;
    //    int y = (int)position.y;

    //    int distance = 0;

    //    for (int xx = x; xx < 8; xx++)
    //    {
    //        if (!tiles[xx, y].occupied)
    //        {
    //            upMoves.Add(new Vector2(xx, y));
    //        }
    //        if (tiles[xx, y].occupied)
    //        {
    //            foreach (Piece piece in pieces)
    //            {
    //                //Somewhat clear I should be using another method to store board coords
    //                //However I sorta like that they are tied like this, I guess I could do it the
    //                //Other way and  * 3 when I'm looking to move them... maybe later.
    //                int pieceX = (int)piece.gameObject.transform.position.x / 3;
    //                int pieceY = (int)piece.gameObject.transform.position.z / 3;

    //                if (pieceX == xx && pieceY == y && piece.player != player)
    //                {
    //                    upMoves.Add(new Vector2(xx, y));
    //                }
    //            }
    //            return upMoves;
    //        }
    //        distance++;
    //        if (distance == maxDistance)
    //        {
    //            return upMoves;
    //        }
    //    }

    //    return upMoves;
    //}

    //List<Vector2> DownMoves(Vector2 position, int player,
    //    Tile[,] tiles, Piece[] pieces, int maxDistance)
    //{
    //    List<Vector2> downMoves = new List<Vector2>();

    //    int x = (int)position.x;
    //    int y = (int)position.y;

    //    int distance = 0;

    //    for (int xx = x; xx < -1; xx--)
    //    {
    //        if (!tiles[xx, y].occupied)
    //        {
    //            downMoves.Add(new Vector2(xx, y));
    //        }
    //        if (tiles[xx, y].occupied)
    //        {
    //            foreach (Piece piece in pieces)
    //            {
    //                //Somewhat clear I should be using another method to store board coords
    //                //However I sorta like that they are tied like this, I guess I could do it the
    //                //Other way and  * 3 when I'm looking to move them... maybe later.
    //                int pieceX = (int)piece.gameObject.transform.position.x / 3;
    //                int pieceY = (int)piece.gameObject.transform.position.z / 3;

    //                if (pieceX == xx && pieceY == y && piece.player != player)
    //                {
    //                    downMoves.Add(new Vector2(xx, y));
    //                }
    //                return downMoves;
    //            }
    //        }

    //        distance++;
    //        if (distance == maxDistance)
    //        {
    //            return downMoves;
    //        }
    //    }

    //    return downMoves;
    //}

    //List<Vector2> LeftMoves(Vector2 position, int player,
    //    Tile[,] tiles, Piece[] pieces, int maxDistance)
    //{
    //    List<Vector2> leftMoves = new List<Vector2>();

    //    int x = (int)position.x;
    //    int y = (int)position.y;

    //    int distance = 0;
    //    for (int yy = y; yy < 0; yy--)
    //    {
    //        if (!tiles[x, yy].occupied)
    //        {
    //            leftMoves.Add(new Vector2(x, yy));
    //        }
    //        if (tiles[x, yy].occupied)
    //        {
    //            foreach (Piece piece in pieces)
    //            {
    //                //Somewhat clear I should be using another method to store board coords
    //                //However I sorta like that they are tied like this, I guess I could do it the
    //                //Other way and  * 3 when I'm looking to move them... maybe later.
    //                int pieceX = (int)piece.gameObject.transform.position.x / 3;
    //                int pieceY = (int)piece.gameObject.transform.position.z / 3;

    //                if (pieceX == x && pieceY == yy && piece.player != player)
    //                {
    //                    leftMoves.Add(new Vector2(x, yy));
    //                }
    //            }
    //            return leftMoves;
    //        }

    //        distance++;
    //        if (distance == maxDistance)
    //        {
    //            return leftMoves;
    //        }
    //    }

    //    return leftMoves;
    //}

    //List<Vector2> RightMoves(Vector2 position, int player,
    //    Tile[,] tiles, Piece[] pieces, int maxDistance)
    //{
    //    List<Vector2> rightMoves = new List<Vector2>();

    //    int x = (int)position.x;
    //    int y = (int)position.y;

    //    int distance = 0;
    //    for (int yy = y; yy < 8; yy++)
    //    {
    //        if (!tiles[x, yy].occupied)
    //        {
    //            rightMoves.Add(new Vector2(x, yy));
    //        }
    //        if (tiles[x, yy].occupied)
    //        {
    //            foreach (Piece piece in pieces)
    //            {
    //                //Somewhat clear I should be using another method to store board coords
    //                //However I sorta like that they are tied like this, I guess I could do it the
    //                //Other way and  * 3 when I'm looking to move them... maybe later.
    //                int pieceX = (int)piece.gameObject.transform.position.x / 3;
    //                int pieceY = (int)piece.gameObject.transform.position.z / 3;

    //                if (pieceX == x && pieceY == yy && piece.player != player)
    //                {
    //                    rightMoves.Add(new Vector2(x, yy));
    //                }
    //            }
    //            return rightMoves;
    //        }

    //        distance++;
    //        if (distance == maxDistance)
    //        {
    //            return rightMoves;
    //        }
    //    }

    //    return rightMoves;
    //}

    List<Vector2> LineMoves(Vector2 origin, int player, Tile[,] tiles,
        Piece[] pieces, int maxDistance, int xDirection, int yDirection)
    {
        List<Vector2> lineMoves = new List<Vector2>();

        int x = (int)origin.x;
        int y = (int)origin.y;

        //can i use x direction to determin < 8 or > 0?
        for (int ii = 0; ii < maxDistance; ii++)
        {

            x += xDirection;
            y += yDirection;

            if (x < 8 && x > -1 && y < 8 && y > -1)
            {
                if (!tiles[x, y].occupied)
                {
                    lineMoves.Add(new Vector2(x, y));
                }

                if (tiles[x, y].occupied)
                {
                    foreach (Piece piece in pieces)
                    {
                        if (piece.gameObject.transform.position.x / 3 == x &&
                            piece.gameObject.transform.position.z / 3 == y &&
                            piece.player != player)
                        {
                            lineMoves.Add(new Vector2(x, y));
                        }
                    }
                    return lineMoves;
                }
            }
        }
        return lineMoves;
    }
}