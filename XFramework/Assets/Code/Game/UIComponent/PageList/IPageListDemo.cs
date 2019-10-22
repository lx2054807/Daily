﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using BDFramework.UI;

public class IPageListDemo : MonoBehaviour
{
    public IPageList list;

    public InputField indexText;

    public Button moveButton;

    public class TestData
    {
        public int id;
        public string name;

        public TestData(int id)
        {
            this.id = id;
            this.name = "szc" + this.id;
        }
    }

    private void Awake()
    {
        moveButton.onClick.AddListener(TestMove);
    }

    private void TestMove()
    {
        int index = Int32.Parse(indexText.text);
        list.MoveToItemByIndex(index);
    }

    private List<TestData> datas;

    private void Start()
    {
        datas = new List<TestData>();
        for (int i = 1; i <=100; i++)
        {
            TestData data = new TestData(i);
            datas.Add(data);
        }

        list.onItemUpdateAction = this.OnTestUpdate;
        list.onItemRemoveAction = OnTestRemove;
        list.Data(datas.Count, "UI/TpItem");
    }

    public void OnTestUpdate(int index, GameObject go)
    {
        TestData data = datas[index];
        var txt = go.transform.Find("txt_t").GetComponent<Text>();
        txt.text = "index = " + index;
        var btn = go.transform.Find("btn_b").GetComponent<Button>();
        btn.onClick.AddListener(() => { Debug.Log("I'm " + data.name); });
    }

    public void OnTestRemove(int index, GameObject go)
    {
        var btn = go.transform.Find("btn_b").GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
    }
}