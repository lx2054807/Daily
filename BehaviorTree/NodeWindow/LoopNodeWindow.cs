using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace ROIdleEditor
{
    public class LoopNodeWindow : BehaviorNodeWindow
    {
        /// <summary>
        /// 循环次数
        /// </summary>
        private string loopTime;

        /// <summary>
        /// 子节点
        /// </summary>
        private BehaviorNodeWindow childNode;

        public LoopNodeWindow(int id) : base(BehaviorNodeType.Loop, id, "循环节点")
        {
            //设置输入区域
            inputArea = new Rect(7, 31, 15, 15);

            //输入区域预留空白
            inputSpace = 10;

            //设置输出区域
            outputAreas.Add(new Rect(160, 31, 15, 15));

            //输出区域预留空白
            outputSpace = 10;

            //设置窗口属性
            WindowWidth = 180;
            WindowHeight = 62;
            WindowBackgroundColor = new Color(1, 0.5f, 0);
        }

        public override void Destroy()
        {
            base.Destroy();

            if (childNode != null)
            {
                childNode.Parent = null;
            }
        }

        protected override void DrawDataArea()
        {
            base.DrawDataArea();

            //循环次数
            GUILayout.BeginHorizontal();
            GUILayout.Label("循环次数:", GUILayout.Width(50));
            loopTime = EditorGUILayout.TextField(loopTime);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        public override void DrawProperty()
        {
            base.DrawProperty();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("循环次数:" + loopTime, GUILayout.Width(180));
            GUILayout.EndHorizontal();
        }

        public override bool CanStartTransition(int outputIndex)
        {
            return childNode == null;
        }

        public override bool AddChild(int outputIndex, BehaviorNodeWindow child)
        {
            if (childNode != null || child.NodeType != BehaviorNodeType.Action)
            {
                return false;
            }

            childNode = child;
            childNode.Parent = this;
            return true;
        }

        public override bool RemoveChild(BehaviorNodeWindow child)
        {
            var result = base.RemoveChild(child);
            if (result)
            {
                childNode = null;
            }
            return result;
        }

        /// <summary>
        /// 保存数据到Xml
        /// </summary>
        public override XmlElement SaveToXml(XmlDocument document, XmlElement root)
        {
            var element = base.SaveToXml(document, root);

            //写入循环次数
            if (!string.IsNullOrEmpty(loopTime))
            {
                element.SetAttribute("loopTime", loopTime);
            }

            //写入子节点
            if (childNode != null)
            {
                element.SetAttribute("child", childNode.ID.ToString());
            }

            return element;
        }

        /// <summary>
        /// 从Xml中读取数据
        /// </summary>
        public override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);

            //读取循环次数
            if (node.Attributes["loopTime"] != null)
            {
                loopTime = node.Attributes["loopTime"].Value;
            }
        }

        /// <summary>
        /// 从Xml中读取子节点
        /// </summary>
        public override void LoadChildFromXml(XmlNode node, Dictionary<int, BehaviorNodeWindow> nodeWindows, List<NodeTransitionLine> nodeTransitionLines)
        {
            if (node.Attributes["child"] != null)
            {
                AddChild(0, nodeWindows[int.Parse(node.Attributes["child"].Value)]);
                nodeTransitionLines.Add(new NodeTransitionLine(this, 0, childNode));
            }
        }
    }
}
