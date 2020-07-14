using UnityEngine;

namespace ROIdleEditor
{
    public class NodeTransitionLine : BezierLine
    {
        public BehaviorNodeWindow StartNodeWindow { get; }
        public int OutputIndex { get; }
        public BehaviorNodeWindow EndNodeWindow { get; }

        public NodeTransitionLine(BehaviorNodeWindow startNodeWindow, int outputIndex, BehaviorNodeWindow endNodeWindow)
        {
            StartNodeWindow = startNodeWindow;
            OutputIndex = outputIndex;
            EndNodeWindow = endNodeWindow;
        }

        public override void Update()
        {
            startPosition = StartNodeWindow.OutputAreas[OutputIndex].center;
            startPosition.x += StartNodeWindow.WindowArea.x;
            startPosition.y += StartNodeWindow.WindowArea.y;
            endPosition = EndNodeWindow.InputArea.center;
            endPosition.x += EndNodeWindow.WindowArea.x;
            endPosition.y += EndNodeWindow.WindowArea.y;

            startTan = startPosition + Vector3.right * 50;
            endTan = endPosition + Vector3.left * 50;
        }
    }
}
