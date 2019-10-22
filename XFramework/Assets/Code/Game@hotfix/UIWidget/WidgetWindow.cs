using System;
using UnityEngine;
using BDFramework.UI;
using BDFramework.ResourceMgr;

public class WidgetWindow
{
    /// <summary>
    /// 资源路径
    /// </summary>
    private string resourcePath = null;

    /// <summary>
    /// 
    /// </summary>
    public Transform Transform { get; protected set; }

    public WidgetWindow(string path)
    {
        resourcePath = path;
    }

    /// <summary>
    /// 是否加载
    /// </summary>
    public bool IsLoad { get; private set; }


    public void Load()
    {
        BDebug.Log(resourcePath, "yellow");
        var o = BResources.Load<GameObject>(resourcePath);
        if (o == null)
        {
            Debug.LogError("窗口资源不存在:" + resourcePath);
            return;
        }

        CreateGameObject(o);
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="callback"></param>
    public void AsyncLoad(Action callback)
    {
        BResources.AsyncLoad<GameObject>(resourcePath, ( o) =>
        {
            CreateGameObject(o);

            if (callback != null)
            {
                callback();
            }
        });
    }

    /// <summary>
    /// 创建预设体
    /// </summary>
    /// <param name="o"></param>
    private void CreateGameObject(GameObject o)
    {
        var go = GameObject.Instantiate(o);
        Transform = go.transform;
        Transform.gameObject.SetActive(false);
        IsLoad = true;
        UITools.AutoSetTransforPathFromWidget(this);
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Init()
    {
    }

    /// <summary>
    /// 打开
    /// </summary>
    public virtual void Show(object o = null, Action truecallback = null, Action cancelcallback = null)
    {
        this.Transform.SetAsLastSibling();
        this.Transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 打开2
    /// </summary>
    /// <param name="o"></param>
    /// <param name="truecallback">带参回调</param>
    /// <param name="cancelcallback">带参回调</param>
    public virtual void ShowAAA(object o = null, Action<object> truecallback = null,
        Action<object> cancelcallback = null)
    {
        this.Transform.SetAsLastSibling();
        this.Transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public virtual void Hide()
    {
        this.Transform.gameObject.SetActive(false);
    }

    public virtual void Destroy()
    {
        //卸载
        if (Transform)
        {
            BResources.Destroy(this.Transform);
        }
    }
}