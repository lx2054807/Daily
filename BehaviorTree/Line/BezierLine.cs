using UnityEditor;
using UnityEngine;

namespace ROIdleEditor
{
    public abstract class BezierLine
    {
        protected Vector3 startPosition;
        protected Vector3 endPosition;

        protected Vector3 startTan;
        protected Vector3 endTan;

        protected Color lineColor = Color.black;

        public abstract void Update();

        public void Draw()
        {
            lineColor.a = 0.7f;
            for (var i = 0; i < 3; ++i)
            {
                Handles.DrawBezier(startPosition, endPosition, startTan, endTan, lineColor, null, i + i * 1.5f);
            }

            lineColor.a = 1.0f;
            Handles.DrawBezier(startPosition, endPosition, startTan, endTan, lineColor, null, 1);
        }
    }
}
