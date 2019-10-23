using System.Collections;
using System.Collections.Generic;
using BDFramework.ScreenView;
using UnityEngine;
using BDFramework.UI;
using DG.Tweening;
using UnityEngine.UI;
using BDFramework;
//using UnityEditor.Graphs;

/// <summary>
/// 这个是ui的标签，
/// index 
/// resource 目录
/// </summary>
[UI((int)WinEnum.Login,"Windows/Window_Login") ]
public class Window_Login : AWindow
{

    [TransformPath("Btn_")] 
    private IButton btn_01;

    [TransformPath("sPageList")] private SelectPageList spl_Test;
    
    public class TestItem
    {
        [Component("number",ComponentType.Text,"text")]
        public string str_Text;
    }
    
    public Window_Login(string path) : base(path)
    {
    }

    public override void Init()
    {
        base.Init();
        spl_Test.onItemUpdateAction = OnAddTestItem;
        //01按钮
        btn_01.onClick.AddListener(() =>
        {
            if (!UIWidgetManager.Inst.GetWidgetWindow((int) WidgetEnum.NewTips).IsOpen)
            {
                UIWidgetManager.Inst.GetWidgetWindow((int)WidgetEnum.NewTips).Show("再睡会");
            }
            else
            {
                UIWidgetManager.Inst.GetWidgetWindow((int)WidgetEnum.NewTips).Send("就再睡五分钟");
            }
        });
        
        
    }

    private void OnAddTestItem(int index,GameObject go)
    {
        TestItem item = new TestItem()
        {
            str_Text = index.ToString()
        };
        UITools.AutoSetComValue(go.transform,item);
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Open(WindowData data = null)
    {
        base.Open();
        spl_Test.Data(10,"Windows/SPItem");
    }

    public override void Destroy()
    {
        base.Destroy();
    }
    
}