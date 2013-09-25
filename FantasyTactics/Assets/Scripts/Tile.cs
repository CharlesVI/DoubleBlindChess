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

    Color startColor;
    Color moveableColor = Color.blue;
    Color attackColor = Color.red; //These need to turn into shades of the startColor
    Color destinationColor = Color.green; // same here.

    //Not sure if this is a really good idea or not but I think it is.
    //Going to use this to sorta define all the other colors as shades of the
    //Start color
    public void StartColor(Color color)
    {
        startColor = color;
        //moveableColor = new Color(startColor.r, startColor.g, 255, 255);
        gameObject.transform.renderer.material.color = color;
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
        gameObject.transform.renderer.material.color = destinationColor;
    }

    public void UnHighlight()
    {
        gameObject.transform.renderer.material.color = startColor;
        moveable = false;
        destination = false;
    }
}
