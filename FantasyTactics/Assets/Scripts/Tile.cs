using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Tile
{
    public GameObject gameObject { get; set; }
    public Vector2 id {get; set;}

    public bool occupied;
    public bool moveable;
    public bool destination;
    public bool p1Threat;
    public bool p2Threat;

    public bool showThreats;

    Color startColor;
    Color moveableColor;
    Color attackColor; //These need to turn into shades of the startColor
    Color destinationColor; // same here.
    Color p1ThreatColor;
    Color p2ThreatColor;
    Color contestedColor;

    //Not sure if this is a really good idea or not but I think it is.
    //Going to use this to sorta define all the other colors as shades of the
    //Start color
    public void StartColor(Color color, bool darkTile)
    {
        startColor = color;

        if (darkTile)
        {
            moveableColor = new Color(10, 50, 90);
            attackColor = new Color(100, 0, 0);
            destinationColor = new Color(0, 100, 0);
            p1ThreatColor = new Color(0, 0, 50);
            p2ThreatColor = new Color(100, 50, 0);
            contestedColor = new Color(100, 50, 100);
        }

        if (!darkTile)
        {
            moveableColor = new Color(60, 0, 0);
            attackColor = new Color(100, 0, 0);
            destinationColor = new Color(0, 100, 0);
            p1ThreatColor = new Color(0, 0, 50);
            p2ThreatColor = new Color(100, 50, 0);
            contestedColor = new Color(100, 50, 100);
        }
        //This is to apply a tint to the color instead of a set color, to preserve the light dark thing.
        //This is not working, TODO

        SetMyColor(color);
    }

    public void Moveable()
    {
        gameObject.transform.renderer.material.color = moveableColor;
        
        if (occupied) { gameObject.transform.renderer.material.color = attackColor; }
        
        moveable = true;
        
        destination = false;
    }

    public void Destination()
    {
        destination = true;
        SetMyColor(destinationColor);
    }

    public void Threatened()
    {
        if (showThreats)
        {
            if (p1Threat)
            {
                SetMyColor(p1ThreatColor);
            }
            if (p2Threat)
            {
                SetMyColor(p2ThreatColor);
            }
            if (p1Threat && p2Threat)
            {
                SetMyColor(contestedColor);
            }
        }
    }

    void SetMyColor(Color color)
    {
        gameObject.transform.renderer.material.color = color;
    }


    // Add theat method here. ALSO in unhighlight check if threatened before going to start color.
    //Threat method will just be for the setting of threatened status and the coloring of it.... since clear calls unhighlight 
    //FIRST TRY HAVING UNHIGHLIGHT BE THE ONLY SOURCE OF THREAT COLOR.
    public void UnHighlight()
    {
        gameObject.transform.renderer.material.color = startColor;
        Threatened(); // keep threat highlight through Clear call.

        moveable = false;
        destination = false;
    }
}
