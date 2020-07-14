using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace ROIdleEditor
{
    public class IfElseNodeWindow : BehaviorNodeWindow
    {
        /// <summary>
        /// 前置条件执行函数
        /// </summary>
        private string preConditionFunction;

        /// <summary>
        /// 条件节点
        /// </summary>
        private BehaviorNodeWindow conditionNode;

        /// <summary>
        /// 条件成功执行的节点
        /// </summary>
        private BehaviorNodeWindow successNode;

        /// <summary>
        /// 条件失败执行的节点
        /// </summary>
        private BehaviorNodeWindow failureNode;

        public IfElseNodeWindow(int id) : base(BehaviorNodeType.IfElse, id, "条件执行节点")
        {
            //设置输入区域
            inputArea = new Rect(6, 39, 15, 15);

            //输入区域预留空白
            inputSpace = 18;

            //设置输出区域
            outputAreas.Add(new Rect(160, 23, 15, 15));
            outputAreas.Add(new Rect(160, 40, 15, 15));
            outputAreas.Add(new Rect(160, 57, 15, 15));

            //输出区域预留空白
            outputSpace = 1;

            //设置窗口属性
            WindowWidth = 180;
            WindowHeight = 80;
            WindowBackgroundColor = new Color(0.917647f, 0.478431f, 0.980392f);
        }

        public override void Destroy()
        {
            base.Destroy();

            if (conditionNode != null)
            {
                conditionNode.Parent = null;
            }

            if (successNode != null)
            {
                successNode.Parent = null;
            }

            if (failureNode != null)
            {
                failureNode.Parent = null;
            }
        }

        protected override void DrawDataArea()
        {
            GUILayout.Space(10);
            base.DrawDataArea();

            DrawExecutionFunction("前置条件:", preConditionFunction, 70, s => { preConditionFunction = s; });
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        public override void DrawProperty()
        {
            base.DrawProperty();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("前置条件:" + preConditionFunction, GUILayout.Width(180));
            GUILayout.EndHorizontal();
        }

        public override bool CanStartTransition(int outputIndex)
        {
            if (outputIndex == 0)
            {
                return conditionNode == null;
            }
            if (outputIndex == 1)
            {
                return successNode == null;
            }
            return failureNode == null;
        }

        public override bool AddChild(int outputIndex, BehaviorNodeWindow child)
        {
            if (outputIndex == 0 && conditionNode == null)
            {
                if (child.NodeType == BehaviorNodeType.Condition)
                {
                    conditionNode = child;
                    conditionNode.Parent = this;
                    return true;
                }
                return false;
            }

            if (outputIndex == 1 && successNode == null)
            {
                successNode = child;
                successNode.Parent = this;
                return true;
            }

            if (outputIndex == 2 && failureNode == null)
            {
                failureNode = child;
                failureNode.Parent = this;
                return true;
            }

            return false;
        }

        public override bool RemoveChild(BehaviorNodeWindow child)
        {
            var result = base.RemoveChild(child);
            if (result)
            {
                if (conditionNode == child)
                {
                    conditionNode = null;
                }
                else if (successNode == child)
                {
                    successNode = null;
                }
                else if (failureNode == child)
                {
                    failureNode = child;
                }
            }
            return result;
        }

        /// <summary>
        /// 保存数据到Xml
        /// </summary>
        public override XmlElement SaveToXml(XmlDocument document, XmlElement root)
        {
            var element = base.SaveToXml(document, root);

            //写入前置条件
            if (!string.IsNullOrEmpty(preConditionFunction))
            {
                element.SetAttribute("preConditionFunction", preConditionFunction);
            }

            //写入条件
            if (conditionNode != null)
            {
                element.SetAttribute("condition", conditionNode.ID.ToString());
            }

            //写入成功节点
            if (successNode != null)
            {
                element.SetAttribute("success", successNode.ID.ToString());
            }

            //写入失败节点
            if (failureNode != null)
            {
                element.SetAttribute("failure", failureNode.ID.ToString());
            }

            return element;
        }

        /// <summary>
        /// 从Xml中读取数据
        /// </summary>
        public override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);

            //读取前置条件
            if (node.Attributes["preConditionFunction"] != null)
            {
                preConditionFunction = node.Attributes["preConditionFunction"].Value;
            }
        }

        /// <summary>
        /// 从Xml中读取子节点
        /// </summary>
        public override void LoadChildFromXml(XmlNode node, Dictionary<int, BehaviorNodeWindow> nodeWindows, List<NodeTransitionLine> nodeTransitionLines)
        {
            if (node.Attributes["condition"] != null)
            {
                AddChild(0, nodeWindows[int.Parse(node.Attributes["condition"].Value)]);
                nodeTransitionLines.Add(new NodeTransitionLine(this, 0, conditionNode));
            }

            if (node.Attributes["success"] != null)
            {
                AddChild(1, nodeWindows[int.Parse(node.Attributes["success"].Value)]);
                nodeTransitionLines.Add(new NodeTransitionLine(this, 1, successNode));
            }

            if (node.Attributes["failure"] != null)
            {
                AddChild(2, nodeWindows[int.Parse(node.Attributes["failure"].Value)]);
                nodeTransitionLines.Add(new NodeTransitionLine(this, 2, failureNode));
            }
        }
    }
}