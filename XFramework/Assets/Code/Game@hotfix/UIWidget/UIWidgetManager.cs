using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BDFramework.Mgr;
using BDFramework;
using BDFramework.UI;


public class UIWidgetManager : ManagerBase<UIWidgetManager, UIWidgetAttribute>
{
    public UIWidgetManager()
    {
    }

    private Dictionary<int, WidgetWindow> widgetWindows = null;

    /// <summary>
    /// UI的最高层级
    /// </summary>
    private Transform Top;

    public override void Init()
    {
        base.Init();
        widgetWindows = new Dictionary<int, WidgetWindow>();
        Top = GameObject.Find("UIRoot/Top").transform;
        BDebug.Log(Top, "red");
    }

    private WidgetWindow CreateWindow(int uiIndex)
    {
        var classData = this.GetCalssData(uiIndex.ToString());
        if (classData == null)
        {
            Debug.LogError("未注册窗口，无法加载:" + uiIndex);
            return null;
        }

        //
        var attr = classData.Attribute as UIWidgetAttribute;
        var window = Activator.CreateInstance(classData.Type, new object[] {attr.ResourcePath}) as WidgetWindow;
        //
        return window;
    }

    /// <summary>
    /// 加载窗口
    /// </summary>
    /// <param name="uiIndexs">窗口枚举</param>
    public void LoadWidget(params int[] uiIndexs)
    {
        foreach (var i in uiIndexs)
        {
            var index = i.GetHashCode();

            if (widgetWindows.ContainsKey(index))
            {
                var uvalue = widgetWindows[index];
                if (uvalue.IsLoad)
                {
                    BDebug.Log("已经加载过并未卸载" + index, "red");
                }
            }
            else
            {
                //创建ui
                var window = CreateWindow(index);
                if (window == null)
                {
                    BDebug.Log("不存在UI:" + index, "red");
                }
                else
                {
                    widgetWindows[index] = window;
                    window.Load();
                    window.Transform.SetParent(this.Top, false);
                    BDebug.LogFormat("加载{0}", index);
                }
            }
        }
    }

    /// <summary>
    /// 异步加载窗口
    /// </summary>
    /// <param name="indexes"></param>
    /// <param name="loadProcessAction"></param>
    public void AsyncLoadWindows(List<int> indexes, Action<int, int> loadProcessAction)
    {
        int allCount = indexes.Count;
        int curTaskCount = 0;
        foreach (var i in indexes)
        {
            var index = i.GetHashCode();
            if (widgetWindows.ContainsKey(index))
            {
                var uvalue = widgetWindows[index];
                if (uvalue.IsLoad)
                {
                    Debug.LogError("已经加载过并未卸载" + index);
                    //任务直接完成
                    {
                        curTaskCount++;
                        loadProcessAction(allCount, curTaskCount);
                    }
                }
            }
            else
            {
                //创建窗口
                var win = CreateWindow(index);
                if (win == null)
                {
                    Debug.LogErrorFormat("不存在UI:{0}", index);
                    curTaskCount++;
                    loadProcessAction(allCount, curTaskCount);
                }
                else
                {
                    widgetWindows[index] = win;
                    //开始窗口加载
                    win.AsyncLoad(() =>
                    {
                        IEnumeratorTool.WaitingForExec(0.1f, () =>
                        {
                            curTaskCount++;
                            if (loadProcessAction != null)
                            {
                                loadProcessAction(allCount, curTaskCount);
                            }
                            win.Transform.SetParent(this.Top, false);
                        });
                    });
                }
            }
        }
    }

    /// <summary>
    /// 卸载窗口
    /// </summary>
    /// <param name="indexs">窗口枚举</param>
    public void UnLoadWidgetWindow(params int[] indexs)
    {
        foreach (var i in indexs)
        {
            var index = i.GetHashCode();
            if (widgetWindows.ContainsKey(index))
            {
                var uvalue = widgetWindows[index];
                uvalue.Destroy();
                widgetWindows.Remove(index);
            }
            else
            {
                Debug.LogErrorFormat("不存在UI：{0}", indexs);
            }
        }
    }

    /// <summary>
    /// 卸载所有窗口 
    /// </summary>
    public void UnLoadallWidget()
    {
        var keys = new List<int>(this.widgetWindows.Keys);
        foreach (var v in this.widgetWindows.Values)
        {
            v.Destroy();
        }

        this.widgetWindows.Clear();
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="index"></param>
    public WidgetWindow GetWidgetWindow(int index)
    {
        int uiIndex = index.GetHashCode();
        if (widgetWindows.ContainsKey(uiIndex))
        {
            var v = widgetWindows[uiIndex];
            return v;
        }
        else
        {
            return null;
        }
    }

    public override void Start()
    {
        base.Start();
    }
}