using System;
using System.Collections;
using System.Collections.Generic;
using BDFramework.ResourceMgr;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[UIWidget((int) WidgetEnum.NewTips, "Widget/Widget_Tips")]
public class UIWidget_NewTips : WidgetWindow
{
    public UIWidget_NewTips(string path) : base(path)
    {
    }

    private GameObject _goItem; //预设体

    /// <summary>
    /// 可配置(读表)参数
    /// </summary>
    private int _maxItem; //最多tips数
    private float _itemHeight;    //预设物高
    private float _startPos;    //初始位置
    private float _endPos;        //自由结束位置
    private float _disappearTime;    //计时器延时,结束时开始fade
    private float _fadeTime;    //fade延时
    private float _moveSpeed;    //上升速度
    private List<string> _listMsgs = new List<string>();    
    private List<RectTransform> _listRecs = new List<RectTransform>();

    public override void Init()
    {
        base.Init();
        _maxItem = 4;
        _itemHeight = 40;
        _startPos = 0;
        _endPos = 0f;
        _disappearTime = 1.5f;
        _fadeTime = 0.5f;
        _moveSpeed = 80f;

        _goItem = BResources.Load<GameObject>("Widget/Tips");
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void Show(object o = null, Action callback = null, Action cancelcallback = null)
    {
        base.Show(o, callback);
        _listMsgs.Add(o.ToString());
        IEnumeratorTool.StartCoroutine(RefreshTips());
    }

    public override void Send(object o = null, Action truecallback = null, Action cancelcallback = null)
    {
        _listMsgs.Add(o.ToString());
    }

    private IEnumerator RefreshTips()
    {
        while (true)
        {
            CreateTips();
            MoveTips();
            if (_listMsgs.Count <= 0 && _listRecs.Count <= 0)
            {
                Hide();
                yield break;
            }

            yield return null;
        }
    }

    private void CreateTips()
    {
        var num = _listRecs.Count;
        if (num >= _maxItem) return;
        if (_listMsgs.Count <= 0) return;
        if (num > 0)
        {
            var lasttips = _listRecs[num - 1].anchoredPosition;
            if (lasttips.y < _endPos) return;
            var pos = lasttips.y - _itemHeight;
            var msg = _listMsgs[0];
            _listMsgs.Remove(msg);
            NewTips(msg, pos);
        }
        else
        {
            var msg = _listMsgs[0];
            _listMsgs.Remove(msg);
            NewTips(msg, _startPos);
        }
    }

    private void NewTips(string msg, float pos)
    {
        var tips = GameObject.Instantiate(_goItem).transform as RectTransform;
        tips.GetComponentInChildren<Text>().text = msg;
        tips.anchoredPosition = new Vector2(tips.anchoredPosition.x, pos);
        tips.transform.SetParent(this.Transform, false);
        tips.sizeDelta = new Vector2(tips.GetComponentInChildren<Text>().preferredWidth + 160,
            tips.sizeDelta.y);
        IEnumeratorTool.StartCoroutine(DestroyTips(tips.gameObject));
        _listRecs.Add(tips);
    }

    private IEnumerator DestroyTips(GameObject go)
    {
        yield return new WaitForSeconds(_disappearTime);
        go.transform.GetComponentInChildren<Text>().DOFade(0, _fadeTime);
        go.transform.GetComponent<Image>().DOFade(0, _fadeTime).OnComplete(() =>
        {
            Object.DestroyImmediate(go);
            if (_listRecs.Count > 0)
            {
                _listRecs.RemoveAt(0);
            }
        });
    }

    private void MoveTips()
    {
        var num = _listRecs.Count;
        if (num > 0)
        {
            var tips = _listRecs[num - 1];
            if (tips.anchoredPosition.y >= _endPos) return;
        }

        foreach (var tips in _listRecs)
        {
            tips.anchoredPosition = new Vector2(tips.anchoredPosition.x,
                tips.anchoredPosition.y + Time.deltaTime * _moveSpeed);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}