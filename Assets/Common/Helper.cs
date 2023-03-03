using UnityEngine;

namespace Assets.Common
{
    public static class Helper
    {
        public static void DrewRectGizmo(Rect rect, Color color)
        {
            Gizmos.color = color;
            // Draw the rectangle area for every quadrant
            Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0));
            Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0));
            Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMax, 0), new Vector3(rect.xMin, rect.yMax, 0));
            Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMin, rect.yMin, 0));
        }
    }
}
