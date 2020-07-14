using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace ROIdleEditor
{
    public class BehaviorTreeWindow : EditorWindow
    {
        /// <summary>
        /// 下一个节点窗口ID
        /// </summary>
        private int nextNodeWindowID;

        /// <summary>
        /// 所有节点
        /// </summary>
        private readonly Dictionary<int, BehaviorNodeWindow> nodeWindows = new Dictionary<int, BehaviorNodeWindow>();

        /// <summary>
        /// 与鼠标相连的线
        /// </summary>
        private MouseTransitionLine mouseTransitionLine;

        /// <summary>
        /// 节点与节点之间的连线
        /// </summary>
        private readonly List<NodeTransitionLine> nodeTransitionLines = new List<NodeTransitionLine>();

        /// <summary>
        /// 当前的鼠标位置
        /// </summary>
        private Vector2 currentMousePosition;
        private Vector2 currentLocalMousePosition;

        /// <summary>
        /// 变换矩阵
        /// </summary>
        private Matrix4x4 matrix;

        private Vector2 translateValue;
        private float scaleValue = 1.0f;

        /// <summary>
        /// Tool窗口
        /// </summary>
        private bool toolVisible = true;
        private Rect toolArea = new Rect(0, 0, 260, 0);
        private readonly Rect toolCreateArea = new Rect(0, 0, 260, 100);
        private Rect toolModifyArea = new Rect(0, 100, 260, 300);
        private readonly Color toolAreaBackgroundColor = new Color(1f, 1f, 0.7f, 1);
        private string toolCreateName;
        private AIType toolCreateType;
        private Vector2 toolModifyScrollViewPosition;

        /// <summary>
        /// 属性窗口
        /// </summary>
        private Rect propertyArea = new Rect(0, 0, 260, 0);
        private readonly Color propertyAreaBackgroundColor = new Color(1f, 1f, 0.7f, 1);
        private int propertyID = -1;

        /// <summary>
        /// 清除
        /// </summary>
        private void Clear()
        {
            nextNodeWindowID = 0;
            translateValue = Vector2.zero;
            scaleValue = 1.0f;

            nodeWindows.Clear();
            nodeTransitionLines.Clear();
            propertyID = -1;
        }

        void OnGUI()
        {
            HandleEvents();

            DrawEditorArea();
            if (toolVisible)
            {
                DrawToolArea();
                DrawPropertyArea();
            }
        }

        #region 事件
        /// <summary>
        /// 处理事件
        /// </summary>
        private void HandleEvents()
        {
            //检测是否保存
            var e = Event.current;

            //记录鼠标位置
            currentMousePosition = e.mousePosition;
            currentLocalMousePosition = ConvertFromWorldToLocal(currentMousePosition, true);

            //检测鼠标按键
            switch (e.type)
            {
                case EventType.MouseUp:
                    if (mouseTransitionLine != null)
                    {//检测是否点击到某个NodeWindow的输入区域
                        MakeTransitionToNode();
                        mouseTransitionLine = null;
                    }
                    else
                    {
                        var clickwindow = GetClickedNodeWindow();
                        if (clickwindow != null)
                        {
                            propertyID = clickwindow.ID;
                        }
                    }
                    break;
                case EventType.MouseDown:
                    if (e.button == 0 && mouseTransitionLine == null)
                    {//检测是否点击到某个NodeWindow的输出区域
                        MakeTransitionToMouse();
                    }
                    else if (e.button == 1)
                    {
                        CreateGenericMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.control)
                    {//移动ScrollView
                        translateValue += e.delta;
                        e.Use();
                    }
                    break;
                case EventType.ScrollWheel:
                    Zoom();
                    e.Use();
                    break;
                case EventType.KeyDown:
                    if (e.control && e.keyCode == KeyCode.S)
                    {//保存配置
                        Save();
                        e.Use();
                    }
                    else if (e.control && e.keyCode == KeyCode.R)
                    {//刷新函数
                        BehaviorTreeManager.Instance.RefreshAIFunctions();
                        ShowNotification(new GUIContent("刷新成功"));
                        e.Use();
                    }
                    else if (e.control && e.keyCode == KeyCode.H)
                    {//显示/隐藏工具面板
                        toolVisible = !toolVisible;
                        e.Use();
                    }
                    break;
            }
        }

        /// <summary>
        /// 检测是否能够创建鼠标关联的线
        /// </summary>
        private void MakeTransitionToMouse()
        {
            bool inWindowArea = false;
            foreach (var pair in nodeWindows)
            {
                var nodeWindow = pair.Value;
                var pos = currentLocalMousePosition;
                pos.x -= nodeWindow.WindowArea.x;
                pos.y -= nodeWindow.WindowArea.y;
                for (var i = 0; i < nodeWindow.OutputAreas.Count; ++i)
                {
                    if (nodeWindow.OutputAreas[i].Contains(pos) &&
                        nodeWindow.CanStartTransition(i))
                    {
                        inWindowArea = true;
                        mouseTransitionLine = new MouseTransitionLine(nodeWindow, i);
                        break;
                    }
                }
            }

            if (inWindowArea)
            {
                Event.current.Use();
            }
        }

        /// <summary>
        /// 检测是否能够创建Node关联的线
        /// </summary>
        private void MakeTransitionToNode()
        {
            var startNodeWindow = mouseTransitionLine.StartNodeWindow;
            var outputIndex = mouseTransitionLine.OutputIndex;
            foreach (var pair in nodeWindows)
            {
                var nodeWindow = pair.Value;
                var pos = currentLocalMousePosition;
                pos.x -= nodeWindow.WindowArea.x;
                pos.y -= nodeWindow.WindowArea.y;
                if (nodeWindow.InputArea.Contains(pos) &&
                    nodeWindow != startNodeWindow &&
                    startNodeWindow.AddChild(outputIndex, nodeWindow))
                {
                    nodeTransitionLines.Add(new NodeTransitionLine(startNodeWindow, outputIndex, nodeWindow));
                    break;
                }
            }
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        private void CreateGenericMenu()
        {
            var genericMenu = new GenericMenu();

            var clickedNodeWindow = GetClickedNodeWindow();
            if (clickedNodeWindow != null)
            {
                var pos = currentLocalMousePosition;
                pos.x -= clickedNodeWindow.WindowArea.x;
                pos.y -= clickedNodeWindow.WindowArea.y;
                if (clickedNodeWindow.InputArea.Contains(pos) && clickedNodeWindow.Parent != null)
                {
                    genericMenu.AddItem(new GUIContent("删除Transition"), false, DeleteTransitionLine, clickedNodeWindow.ID);
                }
                else
                {
                    genericMenu.AddItem(new GUIContent("删除"), false, DeleteBehaviorNodeWindow, clickedNodeWindow.ID);
                }
            }
            else
            {
                GenericMenu.MenuFunction2 createWindow = o =>
                {
                    CreateBehaviorNodeWindow(++nextNodeWindowID, (BehaviorNodeType)o, currentLocalMousePosition);
                };
                genericMenu.AddItem(new GUIContent("动作节点"), false, createWindow, BehaviorNodeType.Action);
                genericMenu.AddItem(new GUIContent("条件节点"), false, createWindow, BehaviorNodeType.Condition);
                genericMenu.AddItem(new GUIContent("条件执行节点"), false, createWindow, BehaviorNodeType.IfElse);
                genericMenu.AddItem(new GUIContent("选择节点"), false, createWindow, BehaviorNodeType.Selector);
                genericMenu.AddItem(new GUIContent("顺序节点"), false, createWindow, BehaviorNodeType.Sequence);
                genericMenu.AddItem(new GUIContent("时间节点"), false, createWindow, BehaviorNodeType.Time);
                genericMenu.AddItem(new GUIContent("并行节点"), false, createWindow, BehaviorNodeType.Parallel);
                genericMenu.AddItem(new GUIContent("循环节点"), false, createWindow, BehaviorNodeType.Loop);
            }

            genericMenu.ShowAsContext();
        }

        /// <summary>
        /// 获取点击到的窗口
        /// </summary>
        private BehaviorNodeWindow GetClickedNodeWindow()
        {
            foreach (var pair in nodeWindows)
            {
                if (pair.Value.WindowArea.Contains(currentLocalMousePosition))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        private BehaviorNodeWindow CreateBehaviorNodeWindow(int nodeID, BehaviorNodeType type, Vector2 pos)
        {
            BehaviorNodeWindow nodeWindow = null;
            switch (type)
            {
                case BehaviorNodeType.Action:
                    nodeWindow = new ActionNodeWindow(nodeID);
                    break;
                case BehaviorNodeType.Condition:
                    nodeWindow = new ConditionNodeWindow(nodeID);
                    break;
                case BehaviorNodeType.IfElse:
                    nodeWindow = new IfElseNodeWindow(nodeID);
                    break;
                case BehaviorNodeType.Selector:
                    nodeWindow = new SelectorNodeWindow(nodeID);
                    break;
                case BehaviorNodeType.Sequence:
                    nodeWindow = new SequenceNodeWindow(nodeID);
                    break;
                case BehaviorNodeType.Time:
                    nodeWindow = new TimeNodeWindow(nodeID);
                    break;
                case BehaviorNodeType.Parallel:
                    nodeWindow = new ParallelNodeWindow(nodeID);
                    break;
                case BehaviorNodeType.Loop:
                    nodeWindow = new LoopNodeWindow(nodeID);
                    break;
            }

            if (nodeWindow != null)
            {
                nodeWindow.WindowArea = new Rect(pos.x, pos.y, nodeWindow.WindowWidth, nodeWindow.WindowHeight);
                nodeWindows.Add(nodeWindow.ID, nodeWindow);
            }

            return nodeWindow;
        }

        /// <summary>
        /// 删除行为节点
        /// </summary>
        /// <param name="o">数据</param>
        private void DeleteBehaviorNodeWindow(object o)
        {
            var nodeWindowID = (int)o;
            if (nodeWindows.TryGetValue(nodeWindowID, out var nodeWindow))
            {
                //删除与该节点相关的线
                for (var i = nodeTransitionLines.Count - 1; i >= 0; --i)
                {
                    var line = nodeTransitionLines[i];
                    if (line.StartNodeWindow == nodeWindow || line.EndNodeWindow == nodeWindow)
                    {
                        nodeTransitionLines.RemoveAt(i);
                    }
                }

                //销毁节点窗口
                nodeWindow.Destroy();
                nodeWindows.Remove(nodeWindowID);
            }
        }

        private void DeleteTransitionLine(object o)
        {
            var nodeWindowID = (int)o;
            if (nodeWindows.TryGetValue(nodeWindowID, out var nodeWindow))
            {
                for (var i = 0; i < nodeTransitionLines.Count; ++i)
                {
                    if (nodeTransitionLines[i].EndNodeWindow == nodeWindow)
                    {
                        nodeTransitionLines[i].StartNodeWindow.RemoveChild(nodeWindow);
                        nodeTransitionLines.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void Zoom()
        {
            scaleValue -= Event.current.delta.y / 50f;
            scaleValue = Mathf.Clamp(scaleValue, 0.3f, 1.7f);

            var delta = ConvertFromWorldToLocal(currentMousePosition - translateValue, false);
            translateValue = currentMousePosition;

            foreach (var pair in nodeWindows)
            {
                var nodeWindow = pair.Value;
                nodeWindow.WindowArea.x -= delta.x;
                nodeWindow.WindowArea.y -= delta.y;
            }
        }
        #endregion

        #region Tool窗口
        /// <summary>
        /// 绘制工具区域
        /// </summary>
        private void DrawToolArea()
        {
            GUI.backgroundColor = toolAreaBackgroundColor;

            toolArea.height = position.height * 0.5f;
            toolModifyArea.height = toolArea.height - 100;

            GUI.Box(toolCreateArea, "新建行为树AI");
            DrawToolCreateArea();

            GUI.Box(toolModifyArea, "修改行为树AI");
            DrawToolModifyArea();
        }

        private void DrawToolCreateArea()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.Label("AI名称:", GUILayout.Width(50));
            toolCreateName = GUILayout.TextField(toolCreateName, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("AI类型:", GUILayout.Width(50));
            toolCreateType = (AIType)EditorGUILayout.EnumPopup(toolCreateType, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Space(180);
            if (GUILayout.Button("创建AI", GUILayout.Width(50)))
            {
                if (!string.IsNullOrEmpty(toolCreateName))
                {
                    BehaviorTreeManager.Instance.CurrentAIName = toolCreateName;
                    BehaviorTreeManager.Instance.CurrentAIType = toolCreateType;
                    Clear();
                    Save();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawToolModifyArea()
        {
            //toolModifyScrollViewPosition = GUILayout.BeginScrollView(toolModifyScrollViewPosition,GUILayout.Width(100));

            GUILayout.BeginVertical();
            GUILayout.Space(30);

            //toolModifyScrollViewPosition = GUILayout.BeginScrollView(toolModifyScrollViewPosition);
            for (int i = 0; i < BehaviorTreeManager.Instance.AIConfigs.Count; ++i)
            {
                var value = i % 2;
                if (value == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(12);
                }

                var aiName = Path.GetFileNameWithoutExtension(BehaviorTreeManager.Instance.AIConfigs[i]);
                if (GUILayout.Button(aiName, GUILayout.Width(115)))
                {
                    var option = EditorUtility.DisplayDialogComplex(aiName, $"选择对{aiName}的操作", "加载", "取消", "删除");
                    if (option == 0)
                    {//加载配置
                        Clear();

                        BehaviorTreeManager.Instance.CurrentAIName = aiName;
                        LoadFromXml(BehaviorTreeManager.Instance.AIConfigs[i].Replace("Assets", Application.dataPath));
                    }
                    else if (option == 2)
                    {//删除配置
                        if (EditorUtility.DisplayDialog(aiName, $"确认删除{aiName}", "删除", "取消"))
                        {//删除配置
                            File.Delete(BehaviorTreeManager.Instance.AIConfigs[i].Replace("Assets", Application.dataPath));

                            //刷新配置
                            BehaviorTreeManager.Instance.RefreshAIConfigs();
                        }
                    }
                    else
                    {//取消

                    }
                }
                if (value == 1)
                {
                    GUILayout.EndHorizontal();
                }
            }
            //GUILayout.EndScrollView();
            GUILayout.EndVertical();
            //GUILayout.EndScrollView();

        }
        #endregion

        #region Editor窗口
        /// <summary>
        /// 绘制编辑区域
        /// </summary>
        private void DrawEditorArea()
        {
            //记录原始数据
            var oriMatrix = GUI.matrix;

            matrix = Matrix4x4.TRS(translateValue, Quaternion.identity, Vector3.one * scaleValue);
            GUI.matrix = matrix;

            DrawNodeWindows();      //绘制节点窗口
            DrawBezierLines();      //绘贝塞尔曲线

            // 还原原始数据
            GUI.matrix = oriMatrix;
        }

        /// <summary>
        /// 绘制所有窗口
        /// </summary>
        private void DrawNodeWindows()
        {
            BeginWindows();
            foreach (var pair in nodeWindows)
            {
                var nodeWindow = pair.Value;

                GUI.backgroundColor = nodeWindow.WindowBackgroundColor;
                GUI.enabled = true;
                nodeWindow.WindowArea = GUI.Window(pair.Key, nodeWindow.WindowArea, DrawBehaviorNode, nodeWindow.Title + " " + nodeWindow.ID);
            }
            EndWindows();
        }

        /// <summary>
        /// 绘制节点
        /// </summary>
        /// <param name="id"></param>
        private void DrawBehaviorNode(int id)
        {
            nodeWindows[id].DrawWindow();
            GUI.DragWindow();
        }

        /// <summary>
        /// 绘制贝塞尔曲线
        /// </summary>
        private void DrawBezierLines()
        {
            if (mouseTransitionLine != null)
            {
                mouseTransitionLine.Update();
                mouseTransitionLine.Draw();
            }

            foreach (var line in nodeTransitionLines)
            {
                line.Update();
                line.Draw();
            }

            Repaint();
        }

        private Vector2 ConvertFromWorldToLocal(Vector2 worldPos, bool isPoint)
        {
            if (isPoint)
            {
                return matrix.inverse.MultiplyPoint(worldPos);
            }
            else
            {
                return matrix.inverse.MultiplyVector(worldPos);
            }
        }
        #endregion

        #region 属性窗口
        /// <summary>
        /// 绘制属性区域
        /// </summary>
        private void DrawPropertyArea()
        {
            GUI.backgroundColor = propertyAreaBackgroundColor;

            //propertyArea.y = toolArea.height;
            //propertyArea.height = position.height - propertyArea.y;
            propertyArea.y = position.height / 2;
            propertyArea.height = propertyArea.y;

            GUI.Box(propertyArea, "属性");
            GUILayout.BeginVertical();
            GUILayout.Space(toolArea.height - toolCreateArea.height);
            DrawNodeProperty();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制节点属性
        /// </summary>
        private void DrawNodeProperty() 
        {
            if (propertyID >= 0)
            {
                nodeWindows[propertyID].DrawProperty();
            }
        }
        #endregion

        #region 加载
        /// <summary>
        /// 从Xml里面加载行为树
        /// </summary>
        /// <param name="xmlPath">xml路径</param>
        public void LoadFromXml(string xmlPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);

            //根节点
            XmlNode root = document.SelectSingleNode("root");
            var x = float.Parse(root.Attributes["scrollPosition_x"].Value);
            var y = float.Parse(root.Attributes["scrollPosition_y"].Value);
            translateValue = new Vector2(x, y);

            BehaviorTreeManager.Instance.CurrentAIName = root.Attributes["aiName"].Value;
            BehaviorTreeManager.Instance.CurrentAIType = (AIType)Enum.Parse(typeof(AIType), root.Attributes["aiType"].Value, true);

            //加载窗口
            LoadWindows(root);
        }

        private void LoadWindows(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                var nodeID = int.Parse(node.Name.Replace("ID", string.Empty));
                var nodeType = (BehaviorNodeType)int.Parse(node.Attributes["type"].Value);
                var x = float.Parse(node.Attributes["windowPosition_x"].Value);
                var y = float.Parse(node.Attributes["windowPosition_y"].Value);
                var nodeWindow = CreateBehaviorNodeWindow(nodeID, nodeType, new Vector2(x, y));
                nodeWindow.LoadFromXml(node);

                //更新nextNodeWindowID
                if (nodeID > nextNodeWindowID)
                {
                    nextNodeWindowID = nodeID + 1;
                }
            }

            foreach (XmlNode node in root.ChildNodes)
            {
                var nodeID = int.Parse(node.Name.Replace("ID", string.Empty));
                var nodeWindow = nodeWindows[nodeID];
                nodeWindow.LoadChildFromXml(node, nodeWindows, nodeTransitionLines);
            }
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
            XmlDocument document = new XmlDocument();

            //声明
            XmlDeclaration xmlDecl = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(xmlDecl);

            //根节点
            XmlElement root = document.CreateElement("root");
            root.SetAttribute("scrollPosition_x", translateValue.x.ToString(CultureInfo.InvariantCulture));
            root.SetAttribute("scrollPosition_y", translateValue.y.ToString(CultureInfo.InvariantCulture));
            root.SetAttribute("aiName", BehaviorTreeManager.Instance.CurrentAIName);
            root.SetAttribute("aiType", BehaviorTreeManager.Instance.CurrentAIType.ToString());
            document.AppendChild(root);

            //存储节点信息
            var rootWindowCount = 0;
            foreach (var pair in nodeWindows)
            {
                var nodeWindow = pair.Value;
                if (nodeWindow.Parent == null)
                {
                    ++rootWindowCount;
                    root.SetAttribute("rootNodeID", nodeWindow.ID.ToString());
                }

                nodeWindow.SaveToXml(document, root);
            }

            //储存根节点数量
            root.SetAttribute("rootNodeCount", rootWindowCount.ToString());

            //保存xml
            document.Save($"Assets/Code/Project/Editor/Tools/BehaviorTree/Config/{BehaviorTreeManager.Instance.CurrentAIName}.xml");

            //刷新配置
            BehaviorTreeManager.Instance.RefreshAIConfigs();

            ShowNotification(new GUIContent("保存成功"));
        }
        #endregion
    }
}
