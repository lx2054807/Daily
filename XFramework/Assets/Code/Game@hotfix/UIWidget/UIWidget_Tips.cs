using System;
using System.Collections;
using System.Collections.Generic;
using BDFramework.ResourceMgr;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[UIWidget((int) WidgetEnum.Tips, "Widget/Widget_Tips")]
public class UIWidget_Tips : WidgetWindow
{
    public UIWidget_Tips(string path) : base(path)
    {
    }

    private List<RectTransform> list_Transform = new List<RectTransform>();
    private GameObject go_Item;
    private int num_MaxItem;
    private float height_Item;

    public override void Init()
    {
        base.Init();
        num_MaxItem = 4;
        height_Item = 120;
        go_Item = BResources.Load<GameObject>("Widget/Tips");
    }

    public override void Hide()
    {
        base.Hide();
    }


    private void Fade(RectTransform trans)
    {
        trans.DOAnchorPos(new Vector2(trans.anchoredPosition.x, trans.anchoredPosition.y + height_Item), 1f);
        trans.GetComponent<Image>().DOFade(0, 2f);
        trans.GetComponentInChildren<Text>().DOFade(0, 2f).OnComplete(() =>
        {
            list_Transform.Remove(trans);
            GameObject.DestroyImmediate(trans.gameObject);
            if (list_Transform.Count == 0)
            {
                Hide();
            }
        });
    }

    public override void Show(object o = null, Action callback = null, Action cancelcallback = null)
    {
        GetRecList(o);
        base.Show(o, callback);
//        ShowRecList();
    }

    private void GetRecList(object o)
    {
        if (list_Transform.Count < num_MaxItem)
        {
            var tmp = GameObject.Instantiate(go_Item).GetComponent<RectTransform>();
            tmp.GetComponentInChildren<Text>().text = o.ToString();
            tmp.transform.SetParent(this.Transform, false);
            tmp.sizeDelta = new Vector2(tmp.GetComponentInChildren<Text>().preferredWidth + 500,
                tmp.sizeDelta.y);
            list_Transform.Add(tmp);
            Fade(tmp);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}