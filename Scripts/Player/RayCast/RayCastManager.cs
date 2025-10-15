using Godot;
using System;
using System.Collections.Generic;


/// <summary>
/// Manages directional RayCast2D nodes (Left, Right, Up, Down) and checks which ones are colliding.
/// </summary>
public partial class RayCastManager : Node2D  ///Inheritance von Node2D oder vielleicht nur Node weil RayCheck RayCasts managed 
                                              ///Schlagwort: "is-a" rule


{

    //private field --- bleibt unused

    //Property --- get; private set; weil ich hier intern setten kann, kein zugriff außerhalb der class aber readable außerhalb class

    public RayCast2D Left { get; private set; }
    public RayCast2D Right { get; private set; }
    public RayCast2D Up { get; private set; }
    public RayCast2D Down { get; private set; }



    // --- Lifecycle Callbacks ---
    public override void _Ready() //GetNode funktioniert nur in nachdem RayCheck ready ist
                                  //daher kann GetNode nicht in einen Constructor gepackt werden
    {
        Left = GetRayCastOrError("RayCastLeft");
        Right = GetRayCastOrError("RayCastRight");
        Up = GetRayCastOrError("RayCastUp");
        Down = GetRayCastOrError("RayCastDown");

    }

    #region Public Methods
    // --- Public Methods (API your class exposes) ---

    public List<RayCast2D> GetDirectionRays()
    {
        //local variables werden in camelCase geschrieben
        List<RayCast2D> rayList = GetCollidingRays();
        return rayList; 
    }
    #endregion

    #region Private Helpers
    // --- Private Helper Methods (internal logic) --- 
    // convention ist, dass private helper nach den public methods angesiedelt sind
    /// <summary>
    /// Collects and returns all RayCast2D nodes that are currently colliding.
    /// </summary>
    /// <returns>A list of RayCast2D nodes that are colliding; the list may be empty.</returns>
// Note:
// Using an enum + Dictionary<Direction, RayCast2D> is a neat and scalable approach.
// It simplifies managing multiple directions, especially when adding new ones,
// avoids repetitive properties, and can reduce memory allocations by reusing the collection.
// Worth considering if the project grows beyond a few fixed directions.
    private List<RayCast2D> GetCollidingRays()
    {
        List<RayCast2D> collidingRays = new(); //kann man auch durch '[]' wenn list or enumerable klar ist.
        RayCast2D[] rays = { Left, Right, Up, Down };
        foreach (var ray in rays)
        {
            if (ray is not null && ray.IsColliding())//'ray is not null' == 'ray != null' -> nach empfehlung nutze ich für null checks 'is' bzw 'is not'
            {
                collidingRays.Add(ray);
            }
        }
        return collidingRays;
    }
    /// <summary>
    /// Tries to get RayCast2D node or logs error if not found.
    /// </summary>
    /// <param name="rayName">The name of RayCast2D node to look for.</param>
    /// <returns>RayCast2D node is found; otherwise null</returns>
    private RayCast2D GetRayCastOrError(string rayName)
    {
        var ray = GetNodeOrNull<RayCast2D>(rayName);
        if (ray is null)
        {
            GD.PrintErr($"{rayName} is not found"); //interpolation ($"{rayname} ...") cleaner version.
        }
        return ray;
    }
    #endregion
}