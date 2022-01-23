using UnityEditor;
using UnityEngine;

public class Room : MonoBehaviour, IRoom
{
    [SerializeField] private int roomIndex;
    [SerializeField] private int priority = 0;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private Generator generator;

    private Collider2D Collider2D => collider2d ? collider2d : collider2d = GetComponent<Collider2D>();
    
    public int RoomIndex => roomIndex;
    public int Priority => priority;
    public Vector2 Center => transform.position;
    public Vector2 Size => transform.localScale;
    public Vector2 RoomSize => collider2d.bounds.size;
    public bool IsIn(Vector2 pos) => IsIn(new Vector3(pos.x, pos.y, transform.position.z));
    private bool IsIn(Vector3 pos) => Collider2D.bounds.Contains(pos);

    public void Visited()
    {
        //generator.MapGrid[0][0].Chip.Visited();
        generator.CellGroups[roomIndex][0].Chip.Visited();
        
    }

    

    
}