﻿using BDFramework.ScreenView;
using UnityEngine;
using BDFramework.UI;
using UnityEngine.UI;

[UI((int)WinEnum.Login,"Windows/Window_Login") ]
public class Window_Login : AWindow
{

    [TransformPath("Btn_Confirm")] 
    private IButton _btnConfirm;

    [TransformPath("InputField_Ac")] private InputField _inputAccount;
    [TransformPath("InputField_Pw")] private InputField _inputPassword;
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
        _btnConfirm.onClick.AddListener(() =>
        {
            Close();
            ScreenViewManager.Inst.MainLayer.BeginNavTo("Main");
        });
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