using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    [Tooltip("Supported colors for this Painter")]
    public Material[] colors;
    [Tooltip("The lines drawed in this Painter")]
    public List<LineRenderer> lines = new List<LineRenderer>();
    [Tooltip("Precision, a less value draw more smoothing")]
    public float minDistanceToDraw = 0.5f;
    [Range(0, 1)]
    [Tooltip("Start width of the lines")]
    public float startLineWidth = 0.1f;
    public float maxTimeToGrowLine = 1f;
    [Tooltip("From the colors list, which if the first color to pick")]
    public int startColorIndex = 0;
    [Range(0.1f, 2f)]
    public float eraserSize = 1f;

    [SerializeField]
    private bool _erasing;
    private Vector3 _lastPosition;
    private float _zDepth;
    private LineRenderer _editingLine;
    private float _timeInPression;
    private Vector3 _startPosition;

    void Start()
    {
        // Get a z from a reference object
        _zDepth = Camera.main.WorldToScreenPoint(transform.position).z;
    }

    void OnMouseDown()
    {
        var go = new GameObject("Line " + (lines.Count + 1));
        var line = go.AddComponent<LineRenderer>();
        go.transform.SetParent(gameObject.transform);
        line.numPositions = 0;
        line.material = colors[startColorIndex];
        line.startWidth = startLineWidth;
        _editingLine = line;
        lines.Add(line);

        _startPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _zDepth));
        _lastPosition = _startPosition;
    }

    void OnMouseUp()
    {
        _editingLine = null;

        _timeInPression = 0f;

        _startPosition = Vector3.zero;
    }

    void OnMouseDrag()
    {
        if (!_erasing)
        {
            if (_editingLine)
            {
                var mouseInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _zDepth));
                var distanceFromLast = Vector3.Distance(mouseInWorld, _lastPosition);

                if (distanceFromLast >= minDistanceToDraw)
                {
                    _editingLine.numPositions++;
                    _editingLine.SetPosition(_editingLine.numPositions - 1, mouseInWorld);
                    _lastPosition = mouseInWorld;
                    // Reset start position to disable continue grow of the line width
                    _startPosition = Vector3.zero;
                }

                if (_startPosition != Vector3.zero)
                {
                    var distanceFromStart = Vector3.Distance(mouseInWorld, _startPosition);
                    // Draw a new position

                    if (minDistanceToDraw >= distanceFromStart)
                    {
                        if (_timeInPression < maxTimeToGrowLine)
                        {
                            _timeInPression += Time.deltaTime;
                            _editingLine.startWidth = CalculateWidthTimeBased(_timeInPression);
                        }
                    }
                }
            }
        }
        else
        {

        }
    }

    private float CalculateWidthTimeBased(float timeElapsed)
    {
        return timeElapsed / maxTimeToGrowLine;
    }

    public void ChangeColor(bool up)
    {
        startColorIndex = up ? startColorIndex + 1 : startColorIndex - 1;

        startColorIndex = Mathf.Clamp(startColorIndex, 0, colors.Length - 1);
    }

    public void SetEraser(bool eraser)
    {
        _erasing = eraser;
    }

    public void CleanPainter()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].numPositions = 0;
        }
    }
}
