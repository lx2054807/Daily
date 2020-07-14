using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace ROIdleEditor
{
    public class BehaviorNodeWindow
    {
        #region 窗口布局相关
        /// <summary>
        /// 输入区域
        /// </summary>
        protected Rect inputArea;
        public Rect InputArea => inputArea;

        /// <summary>
        /// 输入区域预留空白
        /// </summary>
        protected int inputSpace = 0;

        /// <summary>
        /// 输出区域列表
        /// </summary>
        protected List<Rect> outputAreas = new List<Rect>();
        public List<Rect> OutputAreas => outputAreas;

        /// <summary>
        /// 输出区域预留空白
        /// </summary>
        protected int outputSpace = 0;

        /// <summary>
        /// 窗口区域
        /// </summary>
        protected Rect windowArea;
        public ref Rect WindowArea => ref windowArea;

        /// <summary>
        /// 窗口宽度
        /// </summary>
        public int WindowWidth { get; protected set; }

        /// <summary>
        /// 窗口高度
        /// </summary>
        public int WindowHeight { get; protected set; }

        /// <summary>
        /// 窗口颜色
        /// </summary>
        public Color WindowBackgroundColor { get; protected set; }
        #endregion

        #region 节点数据相关
        /// <summary>
        /// 窗口ID
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// 窗口对应的节点类型
        /// </summary>
        public BehaviorNodeType NodeType { get; }

        /// <summary>
        /// 节点名称
        /// </summary>
        private string nodeName;

        /// <summary>
        /// 窗口标题
        /// </summary>
        public string Title { get; private set; }
        #endregion

        #region 节点关系相关
        /// <summary>
        /// 父节点
        /// </summary>
        protected BehaviorNodeWindow parent;
        public BehaviorNodeWindow Parent
        {
            get => parent;
            set => parent = value;
        }
        #endregion

        protected BehaviorNodeWindow(BehaviorNodeType nodeType, int id, string title)
        {
            NodeType = nodeType;
            ID = id;
            Title = title;
        }

        public virtual void Destroy()
        {
            if (parent != null)
            {
                parent.RemoveChild(this);
            }
        }

        /// <summary>
        /// 绘制窗口
        /// </summary>
        public void DrawWindow()
        {
            GUILayout.BeginHorizontal();

            // 绘制输入区域
            GUILayout.BeginVertical();
            GUILayout.Space(inputSpace);
            GUILayout.Label("◀", GUILayout.Width(12));
            GUILayout.EndVertical();

            // 数据配置区
            GUILayout.BeginVertical();
            DrawDataArea();
            GUILayout.EndVertical();

            // 绘制输出区域
            GUILayout.BeginVertical();
            GUILayout.Space(outputSpace);
            for (var i = 0; i < outputAreas.Count; ++i)
            {
                GUILayout.Label("▶", GUILayout.Width(13));
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        protected virtual void DrawDataArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("节点名称:", GUILayout.Width(50));
            nodeName = EditorGUILayout.TextField(nodeName);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        public virtual void DrawProperty()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("节点名称:" + nodeName);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("节点类型:" + NodeType);
            GUILayout.EndHorizontal();
        }

        protected void DrawExecutionFunction(string label, string str, int width, System.Action<string> handler)
        {
            //执行函数选
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(50));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(str, GUILayout.Width(width));
            EditorGUI.EndDisabledGroup();

            var buttonSkin = GUI.skin.button;
            buttonSkin.alignment = TextAnchor.MiddleCenter;
            buttonSkin.padding = new RectOffset(1, 0, 0, 1);
            if (GUILayout.Button("●", buttonSkin, GUILayout.Height(10), GUILayout.Width(10)))
            {//显示选择函数窗口
                var window = EditorWindow.GetWindow<BehaviorTreeSelectFunctionWindow>(true, "选择执行函数", true);
                window.name = "选择执行函数";
                window.minSize = new Vector2(550, 600);
                window.OnSelectFunction = handler;
                window.ShowModalUtility();
            }
            GUILayout.EndHorizontal();
        }

        public virtual bool CanStartTransition(int outputIndex)
        {
            return false;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        public virtual bool AddChild(int outputIndex, BehaviorNodeWindow child)
        {
            return false;
        }

        /// <summary>
        /// 删除子节点
        /// </summary>
        public virtual bool RemoveChild(BehaviorNodeWindow child)
        {
            if (child.parent == this)
            {
                child.parent = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存数据到Xml
        /// </summary>
        public virtual XmlElement SaveToXml(XmlDocument document, XmlElement root)
        {
            var element = document.CreateElement("ID" + ID);
            element.SetAttribute("name", nodeName);
            element.SetAttribute("type", ((int)NodeType).ToString());
            element.SetAttribute("windowPosition_x", windowArea.x.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute("windowPosition_y", windowArea.y.ToString(CultureInfo.InvariantCulture));
            root.AppendChild(element);

            return element;
        }

        /// <summary>
        /// 从Xml中读取数据
        /// </summary>
        public virtual void LoadFromXml(XmlNode node)
        {
            nodeName = node.Attributes["name"].Value;
        }

        /// <summary>
        /// 从Xml中读取子节点
        /// </summary>
        public virtual void LoadChildFromXml(XmlNode node, Dictionary<int, BehaviorNodeWindow> nodeWindows, List<NodeTransitionLine> nodeTransitionLines)
        {

        }
    }
}