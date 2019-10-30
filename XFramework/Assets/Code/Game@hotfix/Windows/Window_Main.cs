﻿using UnityEngine;
using BDFramework.UI;

[UI((int)WinEnum.Main,"Windows/Window_Main") ]
public class Window_Main : AWindow
{

    [TransformPath("Btn_")] 
    private IButton btn_01;

    [TransformPath("sPageList")] private SelectPageList spl_Test;
    
    public class TestItem
    {
        [Component("number",ComponentType.Text,"text")]
        public string str_Text;
    }
    
    public Window_Main(string path) : base(path)
    {
    }

    public override void Init()
    {
        base.Init();
        spl_Test.onItemUpdateAction = OnAddTestItem;
        //01按钮
        btn_01.onClick.AddListener(() =>
        {
            BDebug.Log("Btn is pressed!", "red");
        });
    }

    private void OnAddTestItem(int index,GameObject go)
    {
        TestItem item = new TestItem()
        {
            str_Text = "可以选中的下拉列表item" + index + "号"
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