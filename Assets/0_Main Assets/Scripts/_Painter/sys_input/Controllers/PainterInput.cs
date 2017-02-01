using UnityEngine;

[RequireComponent(typeof(Painter))]
public class PainterInput : MonoBehaviour
{
    [Tooltip("Set in the inspector or let get it in Awake")]
    public Painter painter;

    protected void Awake()
    {
        if (!painter)
            painter = GetComponent<Painter>();

        if (!painter)
            Debug.LogError("Need a Painter: " + gameObject.name);
    }

    void Update()
    {
        if (painter)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                painter.ChangeColor(true);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                painter.ChangeColor(false);
        }
    }
}
