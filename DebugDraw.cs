using Godot;
using System;
using System.Collections.Generic;


public partial class DebugDraw : Node2D
{

#region Fields
private List<DebugLine> _lines = new();
#endregion
struct DebugLine
    {
       //Field
       public Vector2 _lineStart;
       public Vector2 _lineEnd;
       public Color _colour;

       //Constructor - C# checks those types for me.
       public DebugLine(Vector2 lineStart, Vector2 lineEnd, Color colour)
            {
                
                _lineStart = lineStart;
                _lineEnd = lineEnd;
                _colour = colour;
            }

        //colour of line
    }

    //make a list of DebugLine
public void AddLine(Vector2 vector, Vector2 position, Color colour )
    {
        //add lines to a list
      _lines.Add(new DebugLine(vector,position, colour));
      
        
        //call for QueueRedraw - QueueRedraw() requests a future redraw; it doesn't immediately call _Draw(). Godot can perform an initial draw without me explicitly calling QueueRedraw().
       QueueRedraw();
    }

    public override void _Draw() //Multiple QueueRedraw() calls can be handled by a single draw pass.
    {
         //GD.Print($"Drawing {_lines.Count} lines");
        foreach(DebugLine debugLine in _lines)
        {
        //    GD.Print(
        //$"Start: {debugLine._lineStart}, " +
        //$"End: {debugLine._lineEnd}");
    
            DrawLine(debugLine._lineStart, debugLine._lineEnd, debugLine._colour, 10f);
        }
       
        _lines.Clear();
    }

}
