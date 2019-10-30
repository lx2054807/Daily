﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BDFramework.ScreenView;
using BDFramework.Sql;
using BDFramework.UI;

[ScreenView("Login",false)]
public class ScreenView_Login : IScreenView
{
    public string Name { get; private set; }
    public bool IsLoad { get; private set;     }

    public void BeginInit()
    {
        //一定要设置为true，否则当前是未加载状态
        this.IsLoad = true;

        UIManager.Inst.ShowWindow((int) WinEnum.Login);
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