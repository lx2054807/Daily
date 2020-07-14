using UnityEngine;

namespace ROIdleEditor
{
    class MouseTransitionLine : BezierLine
    {
        public BehaviorNodeWindow StartNodeWindow { get; }
        public int OutputIndex { get; }

        public MouseTransitionLine(BehaviorNodeWindow startNodeWindow, int outputIndex)
        {
            StartNodeWindow = startNodeWindow;
            OutputIndex = outputIndex;
        }

        public override void Update()
        {
            startPosition = StartNodeWindow.OutputAreas[OutputIndex].center;
            startPosition.x += StartNodeWindow.WindowArea.x;
            startPosition.y += StartNodeWindow.WindowArea.y;
            endPosition = Event.current.mousePosition;

            startTan = startPosition + Vector3.right * 50;
            endTan = endPosition + Vector3.left * 50;
        }
    }
}
