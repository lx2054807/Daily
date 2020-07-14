using UnityEditor;
using UnityEngine;

namespace ROIdleEditor
{
    public class BehaviorTreeSelectFunctionWindow : EditorWindow
    {
        /// <summary>
        /// 选择函数的回调函数
        /// </summary>
        public System.Action<string> OnSelectFunction { get; set; }

        private Vector2 scrollPosition;

        private void OnGUI()
        {
            var labelSkin = GUI.skin.label;
            var oriLabelAlignment = labelSkin.alignment;
            var oriLabelFontSize = labelSkin.fontSize;
            labelSkin.alignment = TextAnchor.LowerLeft;
            labelSkin.fontSize = 14;

            var buttonSkin = GUI.skin.button;
            var oriBtnAlignment = buttonSkin.alignment;
            var oriBtnFontSize = buttonSkin.fontSize;
            buttonSkin.alignment = TextAnchor.MiddleLeft;
            buttonSkin.fontSize = 14;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            var functions = BehaviorTreeManager.Instance.AIFunctions[BehaviorTreeManager.Instance.CurrentAIType];
            foreach (var funcData in functions)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("  " + funcData.FunctionName, buttonSkin, GUILayout.Width(275), GUILayout.Height(24)))
                {
                    OnSelectFunction(funcData.FunctionName);
                    Close();
                }

                GUILayout.BeginVertical();
                GUILayout.Space(6);
                GUILayout.Label(funcData.FunctionDesc);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            labelSkin.alignment = oriLabelAlignment;
            labelSkin.fontSize = oriLabelFontSize;
            buttonSkin.alignment = oriBtnAlignment;
            buttonSkin.fontSize = oriBtnFontSize;
        }
    }
}