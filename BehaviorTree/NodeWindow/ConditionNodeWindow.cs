using System.Xml;
using UnityEngine;

namespace ROIdleEditor
{
    public class ConditionNodeWindow : BehaviorNodeWindow
    {
        /// <summary>
        /// 执行函数
        /// </summary>
        private string executionFunction;

        public ConditionNodeWindow(int id) : base(BehaviorNodeType.Condition, id, "条件节点")
        {
            //设置输入输出区域
            inputArea = new Rect(7, 31, 15, 15);

            //输入区域预留空白
            inputSpace = 10;

            //设置窗口属性
            WindowWidth = 180;
            WindowHeight = 62;
            WindowBackgroundColor = new Color(0.545098f, 0.866667f, 0.423529f);
        }

        protected override void DrawDataArea()
        {
            base.DrawDataArea();

            DrawExecutionFunction("执行函数:", executionFunction, 85, s => { executionFunction = s; });
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        public override void DrawProperty()
        {
            base.DrawProperty();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("执行函数:" + executionFunction, GUILayout.Width(180));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 保存数据到Xml
        /// </summary>
        public override XmlElement SaveToXml(XmlDocument document, XmlElement root)
        {
            var element = base.SaveToXml(document, root);

            //写入执行函数
            if (!string.IsNullOrEmpty(executionFunction))
            {
                element.SetAttribute("function", executionFunction);
            }

            return element;
        }

        /// <summary>
        /// 从Xml中读取数据
        /// </summary>
        public override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);

            //读取执行函数
            if (node.Attributes["function"] != null)
            {
                executionFunction = node.Attributes["function"].Value;
            }
        }
    }
}