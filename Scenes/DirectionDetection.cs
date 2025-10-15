using Godot;
using System;

public partial class DirectionDetection : Player
{
    //Field
    private RayCast2D RayLeft, RayRight, RayUp, RayDown;
    
    //~

    //Property
    public RayCast2D Left { get { return RayLeft; } set{ RayLeft = value; } } 
    public RayCast2D Right { get; set; }
    public RayCast2D Up { get; set; }
    public RayCast2D Down { get; set; }
    //~

    //Constructor
    //Einfach halten
    public DirectionDetection()
    {
        Left = GetNode<RayCast2D>("DirectionDetection/RayCastLeft");
        Right = GetNode<RayCast2D>("RayCastRight");
        Up = GetNode<RayCast2D>("RayCastUp");
        Down = GetNode<RayCast2D>("RayCastDown");
    }
    //~
}

