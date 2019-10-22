using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BDFramework.UI;
using UnityEngine;

static public partial class UITools
{
    public static void AutoSetTransforPathFromWidget(WidgetWindow win)
    {
        var vt = win.GetType();
        var fields = vt.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        var vTransform = win.Transform;

        foreach (var f in fields)
        {
            if (f.FieldType.IsSubclassOf(checkType) == false)
            {
                continue;
            }

            //1.自动获取节点
            //TODO 热更层必须这样获取属性
            var _attrs = f.GetCustomAttributes(typeof(TransformPath), false); //as Attribute[];
            if (_attrs != null && _attrs.Length > 0)
            {
                var attr = _attrs.ToList().Find((a) => a is TransformPath) as TransformPath;
                if (attr == null) continue;
                //获取节点,并且获取组件
                var trans = vTransform.Find(attr.Path);
                if (trans == null)
                {
                    BDebug.LogError(string.Format("自动设置节点失败：{0} - {1}", vt.FullName, attr.Path));
                    continue;
                }

                var com = trans.GetComponent(f.FieldType);

                if (com == null)
                {
                    BDebug.LogError(string.Format("节点没有对应组件：type【{0}】 - {1}", f.FieldType, attr.Path));
                }

                //设置属性
                f.SetValue(win, com);
                //Debug.LogFormat("字段{0}获取到setTransform ，path：{1}" , f.Name , attr.Path);
            }
        }
    }
}