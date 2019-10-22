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
    private int num_MaxItem = 4;
    private int height_Item = 120;

    public override void Init()
    {
        base.Init();
        go_Item = BResources.Load<GameObject>("Widget/Tips");
    }

    public override void Hide()
    {
        base.Hide();
    }

//    private RectTransform last_Trans;
//    private void PushUp(RectTransform trans)
//    {
//        if (last_Trans != null)
//        {
//            if (trans.anchoredPosition.y <= height_Item * (num_MaxItem - 1) &&
//                trans.anchoredPosition.y <= last_Trans.anchoredPosition.y - height_Item)
//            {
//                trans.DOAnchorPos(new Vector2(trans.anchoredPosition.x, trans.anchoredPosition.y + height_Item), 0.5f).OnComplete(
//                    () =>
//                    {
//                        last_Trans = trans;
//                    });
//            }
//        }
//        else
//        {
//            trans.DOAnchorPos(new Vector2(trans.anchoredPosition.x, trans.anchoredPosition.y + height_Item), 0.5f).OnComplete(
//                () =>
//                {
//                    last_Trans = trans;
//                });
//        }
//    }


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

//    private void ShowRecList()
//    {
//        for (int i = 0; i < list_Transform.Count; i++)
//        {
//            Fade(list_Transform[i]);
//            if (list_Transform[i])
//            {
//                if (i != 0)
//                {
//                    PushUp(list_Transform[i - 1]);
//                    
//                    if (list_Transform[i - 1].anchoredPosition.y >= height_Item)
//                    {
//                        list_Transform[i].anchoredPosition = new Vector2(list_Transform[i - 1].anchoredPosition.x,
//                            list_Transform[i - 1].anchoredPosition.y - height_Item);
//                    }
//                }
//            }
//        }
//    }

    public override void Destroy()
    {
        base.Destroy();
    }
}