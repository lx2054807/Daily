﻿using System.Collections.Generic;
using BDFramework;
using BDFramework.ScreenView;
using UnityEngine;
using BDFramework.UI;
using UnityEngine.UI;

[UI((int)WinEnum.Logo,"Windows/Window_Logo") ]
public class Window_Logo : AWindow
{
    private int count;
    private int loadIndex;
    [TransformPath("Text")] private Text txt;
    [TransformPath("Slider")] private Slider slider_Load;
    [TransformPath("Slider/Handle Slide Area/Handle")]
    private Transform handle;
    
    List<int> list_Window = new List<int>()
    {
        (int) WinEnum.Login,
        (int) WinEnum.Main,
    };
    
    List<int> list_Widget = new List<int>(){};
    
    public Window_Logo(string path) : base(path)
    {
    }

    public override void Init()
    {
        base.Init();
        RegisterAction("OnLoadWindows", Msg_OnLoadWindows);
        count = list_Window.Count;
    }

    private void Msg_OnLoadWindows(WindowData data)
    {
        UIManager.Inst.AsyncLoadWindows(list_Window, (x, y) =>
        {
            loadIndex++;
            SetSliderValue();
        });
    }
    
    private void SetSliderValue()
    {
        txt.text = "正在加载中......" + loadIndex + "/" + count;
        float scrollbar_size = (float) loadIndex / count;

        //光标大小变化
        if (scrollbar_size <= 0.5)
        {
            handle.localScale = new Vector3(scrollbar_size, scrollbar_size, 1);
        }
        else
        {
            handle.localScale = new Vector3(1 - scrollbar_size, 1 - scrollbar_size, 1);
        }

        slider_Load.value = scrollbar_size;
        
        if (loadIndex == count)
        {
            IEnumeratorTool.WaitingForExec(0.5f, () =>
            {
                Close();
                var config = BDLauncher.Inst.Config; // GameObject.Find("BDFrame").GetComponent<Config>();
                if (config.IsNeedNet)
                {
                }
                else
                {
                    ScreenViewManager.Inst.MainLayer.BeginNavTo("Login");
                }
            });
            loadIndex = 0;
            handle.localScale = new Vector3(0, 0, 1);
            UIWidgetManager.Inst.AsyncLoadWindows(list_Widget, null);
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Open(WindowData data = null)
    {
        base.Open();
    }

    public override void Destroy()
    {
        base.Destroy();
    }
    
}