using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Piece
{
    public GameObject gameObject;
    public enum Type {PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING };
    public Type type;
    public int player;
    public Color color;
    public bool canMove;
    public bool active;

    public void SetPositionOneAbove(GameObject go)
    {
        this.gameObject.transform.position = new Vector3(go.transform.position.x,
            go.transform.position.y + 1, go.transform.position.z);
    }

    //NOTE TO ME POSSIBLY ADD HIGHLIGHTING HERE TO MAKE IT MORE CLEAR WHAT PIECE WAS CLICKED?
    public List<Vector2> PawnMoves(Vector3 position, int player, Tile[,] tiles)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        //I guess it is possible that going from float to int can introduce an error here and it may be
        //better to make the ints floats. However They should be real numbers and therfore a non issue in
        //this case.
        int x = (int)position.x / 3;
        int y = (int)position.z / 3;

        if (player == 1)
        {
            if (!tiles[x, y + 1].occupied)
            {
                possibleMoves.Add(new Vector2(x, y + 1));

                if (y == 1 && !tiles[x, y+2].occupied)
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

}
