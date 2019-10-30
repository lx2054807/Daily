﻿using System.Collections;
using BDFramework;
using UnityEngine;
using BDFramework.ScreenView;
using BDFramework.UI;

[ScreenView("Logo",true)]
public class ScreenView_Logo : IScreenView
{
    public string Name { get; private set; }
    public bool IsLoad { get; private set;     }

    public void BeginInit()
    {
        this.IsLoad = true;
        Debug.Log("进入 logo");
        
        UIManager.Inst.LoadWindows((int) WinEnum.Logo);
        UIManager.Inst.ShowWindow((int) WinEnum.Logo);

        //本地配置
        var config = GameObject.Find("BDFrame").GetComponent<Config>();
        
        //是否连接服务器
        if (config.IsNeedNet)
        {
        }
        else
        {
            IEnumeratorTool.StartCoroutine(IE_StartLogic());
        }
    }

    IEnumerator IE_StartLogic()
    {
        //1.初始数据,预留半秒
        yield return  new WaitForSeconds(0.5f);
        //2.通知加载界面
        var data = WindowData.Create("OnLoadWindows");
        UIManager.Inst.SendMessage((int) WinEnum.Logo ,data);
    }
    
    public void BeginExit()
    {
        IsLoad = false;
        Destroy();
    }

    public void Update(float delta)
    {
        
    }

    public void Destroy()
    {
        
    }
    
    public void FixedUpdate(float delta)
    {
       
    }
}