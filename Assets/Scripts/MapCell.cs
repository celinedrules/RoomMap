public class MapCell
{
    public MapChip Chip { get; }
    public int RoomId { get; }
    public int Row { get; }
    public int Col { get; }
    public bool Visted { get; set; }
    
    public ShapeType Shape
    {
        get => Chip.Shape;
        set => Chip.Shape = value;
    }

    public MapCell(MapChip chip, int roomId, int row, int col)
    {
        Chip = chip;
        RoomId = roomId;
        Row = row;
        Col = col;
    }
}