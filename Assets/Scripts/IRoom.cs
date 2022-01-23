using UnityEngine;

public interface IRoom
{
    // Room index number
    int RoomIndex { get; }

    // Priority order
    int Priority { get; }

    // Center position in the world
    Vector2 Center { get; }

    // Width and height
    Vector2 Size { get; }

    Vector2 RoomSize { get; }
    
    // Return whether the given point is in room or not
    bool IsIn(Vector2 pos);
}