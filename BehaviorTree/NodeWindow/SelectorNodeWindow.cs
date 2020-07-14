using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace ROIdleEditor
{
    public class SelectorNodeWindow : BehaviorNodeWindow
    {
        /// <summary>
        /// 前置条件执行函数
        /// </summary>
        private string preConditionFunction;

        /// <summary>
        /// 子节点
        /// </summary>
        private readonly List<BehaviorNodeWindow> childNodes = new List<BehaviorNodeWindow>();

        public SelectorNodeWindow(int id) : base(BehaviorNodeType.Selector, id, "选择节点")
        {
            //设置输入输出区域
            inputArea = new Rect(7, 31, 15, 15);

            //输入区域预留空白
            inputSpace = 10;

            //输出区域
            outputAreas.Add(new Rect(160, 31, 15, 15));

            //输出区域预留空白
            outputSpace = 10;

            //设置窗口属性
            WindowWidth = 180;
            WindowHeight = 62;
            WindowBackgroundColor = new Color(1.0f, 0.533333f, 0.533333f);
        }

        public override void Destroy()
        {
            base.Destroy();

            foreach (var childNode in childNodes)
            {
                childNode.Parent = null;
            }
        }

        protected override void DrawDataArea()
        {
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
            return true;
        }

        public override bool AddChild(int outputIndex, BehaviorNodeWindow child)
        {
            if (childNodes.Contains(child))
            {
                return false;
            }

            child.Parent = this;
            childNodes.Add(child);
            return true;
        }

        public override bool RemoveChild(BehaviorNodeWindow child)
        {
            var result = base.RemoveChild(child);
            if (result)
            {
                childNodes.Remove(child);
            }
            return result;
        }

        /// <summary>
        /// 保存数据到Xml
        /// </summary>
        public override XmlElement SaveToXml(XmlDocument document, XmlElement root)
        {
            //对接点根据上下位置进行排序，上面的节点先执行，下面的节点后执行
            childNodes.Sort((a, b) =>
            {
                if (a.WindowArea.y < b.WindowArea.y)
                {
                    return -1;
                }

                if (a.WindowArea.y > b.WindowArea.y)
                {
                    return 1;
                }

                return 0;
            });

            var element = base.SaveToXml(document, root);

            //写入前置条件
            if (!string.IsNullOrEmpty(preConditionFunction))
            {
                element.SetAttribute("preConditionFunction", preConditionFunction);
            }

            //写入子节点
            var ids = string.Empty;
            foreach (var childNode in childNodes)
            {
                ids += childNode.ID;
                ids += ",";
            }
            element.SetAttribute("children", ids.TrimEnd(','));

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
            var childIDs = node.Attributes["children"].Value.Split(',');
            foreach (var id in childIDs)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var nodeWindow = nodeWindows[int.Parse(id)];
                    AddChild(0, nodeWindow);
                    nodeTransitionLines.Add(new NodeTransitionLine(this, 0, nodeWindow));
                }
            }
        }
    }
}