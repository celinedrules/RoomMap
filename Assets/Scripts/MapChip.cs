using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MapChip : MonoBehaviour
{
    [SerializeField] private int edgeWidth = 2;
    [SerializeField] private Image edgeImage;
    [SerializeField] private Image insideImage;
    [SerializeField] private Image visitedImage;
    [SerializeField] private ShapeType shape;
    [SerializeField] private bool visited;

    private RectTransform rectTransform;

    public RectTransform RectTransform => rectTransform ? rectTransform : rectTransform = GetComponent<RectTransform>();

    public ShapeType Shape
    {
        get => shape;
        set => shape = value;
    }

    private void Awake()
    {
        if (!edgeImage)
            edgeImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (visited)
        {
            if (insideImage.gameObject.activeSelf)
            {
                insideImage.gameObject.SetActive(false);
                visitedImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (visitedImage.gameObject.activeSelf)
            {
                insideImage.gameObject.SetActive(true);
                visitedImage.gameObject.SetActive(false);
            }
        }
    }

    private void ChangeEdge(float left, float bottom, float right, float top)
    {
        insideImage.rectTransform.offsetMin = new Vector2(left, bottom);
        insideImage.rectTransform.offsetMax = new Vector2(-right, -top);
        visitedImage.rectTransform.offsetMin = new Vector2(left, bottom);
        visitedImage.rectTransform.offsetMax = new Vector2(-right, -top);
    }

    public void OnChangeShape(ShapeType newShape)
    {
        switch (newShape)
        {
            case ShapeType.Whole:
                ChangeEdge(0, 0, 0, 0);
                break;
            case ShapeType.Square:
                ChangeEdge(edgeWidth, edgeWidth, edgeWidth, edgeWidth);
                break;
            case ShapeType.LeftEdge:
                ChangeEdge(edgeWidth, 0, 0, 0);
                break;
            case ShapeType.RightEdge:
                ChangeEdge(0, 0, edgeWidth, 0);
                break;
            case ShapeType.BottomEdge:
                ChangeEdge(0, edgeWidth, 0, 0);
                break;
            case ShapeType.TopEdge:
                ChangeEdge(0, 0, 0, edgeWidth);
                break;
            case ShapeType.LeftRightEdge:
                ChangeEdge(edgeWidth, 0, edgeWidth, 0);
                break;
            case ShapeType.TopBottomEdge:
                ChangeEdge(0, edgeWidth, 0, edgeWidth);
                break;
            case ShapeType.LeftCorner:
                ChangeEdge(edgeWidth, edgeWidth, 0, edgeWidth);
                break;
            case ShapeType.RightCorner:
                ChangeEdge(0, edgeWidth, edgeWidth, edgeWidth);
                break;
            case ShapeType.BottomCorner:
                ChangeEdge(edgeWidth, edgeWidth, edgeWidth, 0);
                break;
            case ShapeType.TopCorner:
                ChangeEdge(edgeWidth, 0, edgeWidth, edgeWidth);
                break;
            case ShapeType.LeftTopCorner:
                ChangeEdge(edgeWidth, 0, 0, edgeWidth);
                break;
            case ShapeType.LeftBottomCorner:
                ChangeEdge(edgeWidth, edgeWidth, 0, 0);
                break;
            case ShapeType.RightTopCorner:
                ChangeEdge(0, 0, edgeWidth, edgeWidth);
                break;
            case ShapeType.RightBottomCorner:
                ChangeEdge(0, edgeWidth, edgeWidth, 0);
                break;
            case ShapeType.Blank:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newShape), newShape, null);
        }
    }

    public void Visited()
    {
        visited = true;
    }
}