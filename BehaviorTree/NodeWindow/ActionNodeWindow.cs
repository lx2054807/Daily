using System;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace ROIdleEditor
{
    public class ActionNodeWindow : BehaviorNodeWindow
    {
        public static string[] ReturnTypes = { "成功", "失败", "正在运行", "返回函数结果" };

        /// <summary>
        /// 前置条件执行函数
        /// </summary>
        private string preConditionFunction;

        /// <summary>
        /// 执行函数
        /// </summary>
        private string executionFunction;

        /// <summary>
        /// 返回类型
        /// </summary>
        private string returnType;

        /// <summary>
        /// 返回类型索引
        /// </summary>
        private int returnTypeIndex;


        public ActionNodeWindow(int id) : base(BehaviorNodeType.Action, id, "动作节点")
        {
            //设置输入输出区域
            inputArea = new Rect(6, 49, 15, 15);

            //输入区域预留空白
            inputSpace = 28;

            //设置窗口属性
            WindowWidth = 180;
            WindowHeight = 95;
            WindowBackgroundColor = new Color(1.0f, 0.788235f, 0.290196f);
        }

        protected override void DrawDataArea()
        {
            base.DrawDataArea();

            DrawExecutionFunction("前置条件:", preConditionFunction, 85, s => { preConditionFunction = s; });
            DrawExecutionFunction("执行函数:", executionFunction, 85, s => { executionFunction = s; });

            //返回类型
            GUILayout.BeginHorizontal();
            GUILayout.Label("返回类型:", GUILayout.Width(50));
            returnTypeIndex = EditorGUILayout.Popup(returnTypeIndex, ReturnTypes);
            GUILayout.EndHorizontal();

            returnType = ReturnTypes[returnTypeIndex];
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        public override void DrawProperty() 
        {
            base.DrawProperty();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("前置条件:" + preConditionFunction,GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("执行函数:" + executionFunction, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("返回类型:" + returnType, GUILayout.Width(180));
            GUILayout.EndHorizontal();
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

            //写入执行函数
            if (!string.IsNullOrEmpty(executionFunction))
            {
                element.SetAttribute("function", executionFunction);
            }
            
            //写入返回类型
            element.SetAttribute("returnType", Array.IndexOf(ReturnTypes, returnType).ToString());

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

            //读取执行函数
            if (node.Attributes["function"] != null)
            {
                executionFunction = node.Attributes["function"].Value;
            }

            //读取返回类型
            var index = int.Parse(node.Attributes["returnType"].Value);
            returnType = ReturnTypes[index];
            returnTypeIndex = index;
        }
    }
}