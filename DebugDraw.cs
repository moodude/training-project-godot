using Godot;
using System;


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

       //Constructor
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
      
        
        //call for QueueRedraw
       QueueRedraw();
    }

    public override void _Draw()
    {
        foreach(DebugLine debugLine in _lines)
        {
            DrawLine(debugLine._lineStart, debugLine._lineEnd, debugLine._colour);
        }
       
        _lines.Clear();
    }

}
