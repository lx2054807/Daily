using System;
using System.Collections;
using System.IO;
using System.Reflection;
using BDFramework.Helper;
using UnityEngine;

namespace BDFramework
{
    static public class ScriptLoder
    {
        static public Assembly Assembly { get; private set; }

        /// <summary>
        /// 开始热更脚本逻辑
        /// </summary>
        static public void Load(string root, HotfixCodeRunMode mode)
        {
            if (root != "")
            {
                IEnumeratorTool.StartCoroutine(IE_LoadScript(root, mode));
            }
            else
            {
#if UNITY_EDITOR
                BDebug.Log("内置code模式!");
                BDLauncherBridge.Start();    
#endif
            }
           
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="copyto"></param>
        /// <returns></returns>
        static IEnumerator IE_LoadScript(string root, HotfixCodeRunMode mode)
        {
            string dllPath = "";
            if (Application.isEditor)
            {
                dllPath = root + "/" + BDUtils.GetPlatformPath(Application.platform) + "/hotfix/hotfix.dll";
            }
            else
            {
                //这里情况比较复杂,Mobile上基本认为Persistent才支持File操作,
                //可寻址目录也只有 StreamingAsset
                var firstPath = Application.persistentDataPath + "/" + BDUtils.GetPlatformPath(Application.platform) +
                                "/hotfix/hotfix.dll";
                var secondPath = Application.streamingAssetsPath + "/" + BDUtils.GetPlatformPath(Application.platform) +
                                 "/hotfix/hotfix.dll";
                if (!File.Exists(firstPath))
                {
                    var www = new WWW(secondPath);
                    yield return www;
                    if (www.isDone && www.error == null)
                    {
                        FileHelper.WriteAllBytes(firstPath, www.bytes);
                    }
                }
                dllPath = firstPath;
            }

            //反射执行
            if (mode == HotfixCodeRunMode.ByReflection)
            {
                var bytes = File.ReadAllBytes(dllPath);
                var mdb = dllPath + ".mdb";
                if (File.Exists(mdb))
                {
                    var bytes2 = File.ReadAllBytes(mdb);
                    Assembly = Assembly.Load(bytes, bytes2);
                }
                else
                {
                    Assembly = Assembly.Load(bytes);
                }
                var type = Assembly.GetType("BDLauncherBridge");
                var method = type.GetMethod("Start", BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, new object[] {false, true});
            }
            //解释执行
            else if (mode == HotfixCodeRunMode.ByILRuntime)
            {
                //解释执行模式
                ILRuntimeHelper.LoadHotfix(dllPath);
                ILRuntimeHelper.AppDomain.Invoke("BDLauncherBridge", "Start", null, new object[] {true, false});
            }
        }
    }
}