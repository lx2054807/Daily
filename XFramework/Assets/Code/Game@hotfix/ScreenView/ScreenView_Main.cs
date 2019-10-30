﻿using BDFramework.ScreenView;
using BDFramework.UI;

[ScreenView("Main",false)]
public class ScreenView_Main : IScreenView
{
    public string Name { get; private set; }
    public bool IsLoad { get; private set;     }

    public void BeginInit()
    {
        //一定要设置为true，否则当前是未加载状态
        this.IsLoad = true;

        UIManager.Inst.ShowWindow((int) WinEnum.Main);
    }

    public void BeginExit()
    {
    }

    public void Update(float delta)
    {
        
    }

    public void FixedUpdate(float delta)
    {
       
    }
}