using UnityEngine;
using System.Collections;

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

}
