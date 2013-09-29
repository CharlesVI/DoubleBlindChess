﻿using UnityEngine;
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
    public bool moved;
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
        moved = false;
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

    //TODO add EnPassant
    public List<Vector2> PawnMoves(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        int x = (int)position.x;
        int y = (int)position.y;
        int xx = 0;
        int yy = 0;

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

            xx = x + 1;
            yy = y + 1;
            if (xx > -1 && xx < 8 && yy > -1 && yy < 8 && tiles[xx, yy].occupied)
            {
                foreach (Piece piece in pieces)
                {
                    if (piece.MyCoordinates().x == xx && piece.MyCoordinates().y == yy && piece.player != player)
                    {
                        possibleMoves.Add(new Vector2(xx, yy));
                    }
                }
            }

            xx = x - 1;
            yy = y + 1;
            if (xx > -1 && xx < 8 && yy > -1 && yy < 8 && tiles[xx, yy].occupied)
            {
                foreach (Piece piece in pieces)
                {
                    if (piece.MyCoordinates().x == xx && piece.MyCoordinates().y == yy && piece.player != player)
                    {
                        possibleMoves.Add(new Vector2(xx, yy));
                    }
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

            xx = x + 1;
            yy = y - 1;
            if (xx > -1 && xx < 8 && yy > -1 && yy < 8 && tiles[xx, yy].occupied)
            {
                foreach (Piece piece in pieces)
                {
                    if (piece.MyCoordinates().x == xx && piece.MyCoordinates().y == yy && piece.player != player)
                    {
                        possibleMoves.Add(new Vector2(xx, yy));
                    }
                }
            }

            xx = x - 1;
            yy = y - 1;
            if (xx > -1 && xx < 8 && yy > -1 && yy < 8 && tiles[xx, yy].occupied)
            {
                foreach (Piece piece in pieces)
                {
                    if (piece.MyCoordinates().x == xx && piece.MyCoordinates().y == yy && piece.player != player)
                    {
                        possibleMoves.Add(new Vector2(xx, yy));
                    }
                }
            }
        }

        return possibleMoves;
    }

    public List<Vector2> PawnThreats(Vector2 position, int player, Tile[,] tiles, Piece[] pieces)
    {
        List<Vector2> threats = new List<Vector2>();

        int x = (int)position.x;
        int y = (int)position.y;

        if (player == 1)
        {
            {
                int xx = x + 1;
                int yy = y + 1;

                if (xx > -1 && xx < 8 && yy > -1 && yy < 8)
                {
                    threats.Add(new Vector2(xx, yy));
                }

                xx = x - 1;
                yy = y + 1;

                if (xx > -1 && xx < 8 && yy > -1 && yy < 8)
                {
                    threats.Add(new Vector2(xx, yy));
                }
            }
        }

        if (player == 2)
        {
            int xx = x + 1;
            int yy = y - 1;

            if (xx > -1 && xx < 8 && yy > -1 && yy < 8)
            {
                threats.Add(new Vector2(xx, yy));
            }

            xx = x - 1;
            yy = y - 1;

            if (xx > -1 && xx < 8 && yy > -1 && yy < 8)
            {
                threats.Add(new Vector2(xx, yy));
            }
        }

        return threats;
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

        //I dont know if this is just how it has to be or I'm not smart enough but...
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces, 2, 1));
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces, 2, -1));
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces, -2, 1));
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces, -2, -1));
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces, 1, 2));
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces, 1, -2));
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces, -1, 2));
        possibleMoves.AddRange(KnightMove(position, player, tiles, pieces,-1, -2));

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

    List<Vector2> KnightMove(Vector2 origin, int player, Tile[,] tiles, Piece[] pieces, int xDirection, int yDirection)
    {
        int x = (int)origin.x + xDirection;
        int y = (int)origin.y + yDirection;
        Vector2 move = new Vector2(x, y);
        List<Vector2> knightMove = new List<Vector2>();

        if (x > -1 && x < 8 && y > -1 && y < 8)
        {
            if (!tiles[x, y].occupied)
            {
                knightMove.Add(move);
            }

            if (tiles[x, y].occupied)
            {
                foreach (Piece piece in pieces)
                {
                    if (piece.player != player && piece.MyCoordinates() == move)
                    {
                        knightMove.Add(move);
                    }
                }
            }
        }
        return knightMove;
    }
}