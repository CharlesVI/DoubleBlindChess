using UnityEngine;
using System.Collections;

public class Piece
{
    public GameObject gameObject;
    public enum Type {PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING };
    public int player;
    public Color color;
    public bool canMove;
    public bool active;



}
