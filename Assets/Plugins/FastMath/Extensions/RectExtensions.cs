using UnityEngine;

public static class RectExtensions
{
    public static Vector2 TopLeft(this Rect r)
    {
        return new Vector2(r.xMin, r.yMax);
    }

    public static Vector2 TopCenter(this Rect r)
    {
        return new Vector2(r.center.x, r.yMax);
    }

    public static Vector2 TopRight(this Rect r)
    {
        return new Vector2(r.xMax, r.yMax);
    }

    public static Vector2 MiddleLeft(this Rect r)
    {
        return new Vector2(r.xMin, r.center.y);
    }

    public static Vector2 MiddleRight(this Rect r)
    {
        return new Vector2(r.xMax, r.center.y);
    }

    public static Vector2 BottomLeft(this Rect r)
    {
        return new Vector2(r.xMin, r.yMin);
    }

    public static Vector2 BottomCenter(this Rect r)
    {
        return new Vector2(r.center.x, r.yMin);
    }

    public static Vector2 BottomRight(this Rect r)
    {
        return new Vector2(r.xMax, r.yMin);
    }
}