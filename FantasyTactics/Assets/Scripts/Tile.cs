using UnityEngine;
using System.Collections;

public class Tile
{

    public Vector2 position = new Vector2(-1,-1);
    public FSprite sprite;
    enum Unit { bigGuy, littleGUy };
    enum Type { mountian, swamp, forest, desert };

    public Tile()
    { }

    
    public Tile(Vector2 pos)
    {
        position = pos;
    }

}

    
    // Add bool ocuupied; ?

	
	

