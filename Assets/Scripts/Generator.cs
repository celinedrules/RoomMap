using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRx;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private MapChip mapChip;
    [SerializeField] private GameObject searchRoot;
    [SerializeField] private Vector2 worldMargin = new Vector2(100.0f, 100.0f);
    [SerializeField] private int cellSize;

    private float leftEdge;
    private float rightEdge;
    private float topEdge;
    private float bottomEdge;

    private Vector2 worldSize;

    // The number of x and y cells in the room (roomWidth / cellSize, roomHeight / cellSize)
    private Vector2 gridSize;
    private Vector2[][] worldCellPoints;
    private int[][] roomMatrix;
    public Dictionary<int, List<MapCell>> CellGroups { get; private set; }

    public Dictionary<int, IRoom> Rooms { get; private set; }
    public MapCell[][] MapGrid { get; private set; }

    public void Setup()
    {
        ClearConsole();
        ClearMap();

        Rooms = new Dictionary<int, IRoom>();
        searchRoot.GetComponentsInChildren<IRoom>().ToList().ForEach(room => { Rooms[room.RoomIndex] = room; });

        if (Rooms.Count == 0)
            return;

        leftEdge = Rooms.Values.ToList().Select(room => (room.Center - room.RoomSize / 2.0f).x).Min() -
                   worldMargin.x;
        rightEdge = Rooms.Values.ToList().Select(room => (room.Center + room.RoomSize / 2.0f).x).Max() +
                    worldMargin.x;
        topEdge = Rooms.Values.ToList().Select(room => (room.Center + room.RoomSize / 2.0f).y).Max() +
                  worldMargin.y;
        bottomEdge = Rooms.Values.ToList().Select(room => (room.Center - room.RoomSize / 2.0f).y).Min() -
                     worldMargin.y;

        worldSize = new Vector2(rightEdge - leftEdge, topEdge - bottomEdge);
        gridSize = new Vector2(Mathf.Ceil(worldSize.x / cellSize), Mathf.Ceil(worldSize.y / cellSize));

        worldCellPoints = new Vector2[(int)gridSize.y][];

        for (var j = 0; j < gridSize.y; j++)
        {
            worldCellPoints[j] = new Vector2[(int)gridSize.x];

            for (var i = 0; i < gridSize.x; i++)
                worldCellPoints[j][i] = Grid2SampleWorldPoint(j, i);
        }

        MapGrid = new MapCell[(int)gridSize.y][];
        roomMatrix = new int[(int)gridSize.y][];

        for (var j = 0; j < gridSize.y; j++)
        {
            MapGrid[j] = new MapCell[(int)gridSize.x];
            roomMatrix[j] = new int[(int)gridSize.x];

            for (var i = 0; i < gridSize.x; i++)
            {
                roomMatrix[j][i] = -1;

                foreach (var room in Rooms.Values.OrderByDescending(r => r.Priority))
                {
                    if (!room.IsIn(worldCellPoints[j][i]))
                        continue;

                    roomMatrix[j][i] = room.RoomIndex;
                    break;
                }
            }
        }

        CellGroups = new Dictionary<int, List<MapCell>>();
    }

    public void Generate()
    {
        if (Rooms == null)
            Setup();

        if (Rooms.Count == 0)
        {
            Debug.Log("There are no rooms in the scene");
            return;
        }

        for (var j = 0; j < gridSize.y; j++)
        {
            for (var i = 0; i < gridSize.x; i++)
            {
                var index = roomMatrix[j][i];

                if (index < 0)
                    continue;

                MapChip chip = Instantiate(mapChip, canvas.transform, false);
                chip.RectTransform.anchoredPosition = new Vector2(i * cellSize, j * cellSize);

                var cell = new MapCell(chip, index, j, i);
                MapGrid[j][i] = cell;

                if (!CellGroups.ContainsKey(index))
                    CellGroups[index] = new List<MapCell>();

                CellGroups[index].Add(cell);
            }
        }

        for (var j = 0; j < gridSize.y; j++)
        {
            for (var i = 0; i < gridSize.x; i++)
            {
                var index = roomMatrix[j][i];

                if (index < 0)
                    continue;

                MapGrid[j][i].Shape = GuessShapeType(j, i);

                var j1 = j;
                var i1 = i;

                this.ObserveEveryValueChanged(_ => MapGrid[j1][i1].Shape)
                    .Subscribe(s => MapGrid[j1][i1].Chip.OnChangeShape(s)).AddTo(MapGrid[j][i].Chip);
            }
        }
    }

    private ShapeType GuessShapeType(int row, int col)
    {
        var index = roomMatrix[row][col];

        if (index < 0)
            return ShapeType.Blank;

        var ns = GetConnectedNeighbors(row, col);
        var nConnect = ns.Count(x => x == index);

        return nConnect switch
        {
            0 => ShapeType.Square,
            1 when ns[0] == index => ShapeType.BottomCorner,
            1 when ns[1] == index => ShapeType.RightCorner,
            1 => ns[2] == index ? ShapeType.LeftCorner : ShapeType.TopCorner,
            2 when ns[0] == index && ns[1] == index => ShapeType.RightBottomCorner,
            2 when ns[0] == index && ns[2] == index => ShapeType.LeftBottomCorner,
            2 when ns[3] == index && ns[1] == index => ShapeType.RightTopCorner,
            2 when ns[3] == index && ns[2] == index => ShapeType.LeftTopCorner,
            2 when ns[0] == index && ns[3] == index => ShapeType.LeftRightEdge,
            2 => ShapeType.TopBottomEdge,
            3 when ns[0] != index => ShapeType.TopEdge,
            3 when ns[1] != index => ShapeType.LeftEdge,
            3 => ns[2] != index ? ShapeType.RightEdge : ShapeType.BottomEdge,
            _ => ShapeType.Whole
        };
    }

    private int[] GetConnectedNeighbors(int row, int col)
    {
        var tmp = new int[4];

        tmp[0] = row + 1 < roomMatrix.Length ? roomMatrix[row + 1][col] : -1;
        tmp[1] = col - 1 >= 0 ? roomMatrix[row][col - 1] : -1;
        tmp[2] = col + 1 < roomMatrix[row].Length ? roomMatrix[row][col + 1] : -1;
        tmp[3] = row - 1 >= 0 ? roomMatrix[row - 1][col] : -1;
        
        return tmp;
    }

    private Vector2 Grid2SampleWorldPoint(int row, int col) => new Vector2(
        leftEdge + col * cellSize + (float)cellSize / 2,
        bottomEdge + row * cellSize + (float)cellSize / 2);

    public void ClearMap()
    {
        for (var i = canvas.transform.childCount; i > 0; --i)
            DestroyImmediate(canvas.transform.GetChild(0).gameObject);
    }

    private void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        MethodInfo clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
        clearMethod?.Invoke(null, null);
    }
}