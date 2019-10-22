
using BDFramework;
using BDFramework.GameStart;
using UnityEngine;


namespace Code.Game
{
    [GameStartAtrribute(1)]
    public class HotfixGameStart : IGameStart
    {

        public void Start()
        {
          // Application.targetFrameRate = 24;
           BDebug.Log("hotfix代码 启动器连接成功!" ,"red");
           GameObject.Find("BDFrame").GetComponent<BDLauncher>().Launch();
        }
        
        public void Update()
        {
            
        }

        public void LateUpdate()
        {
            
        }

    }
}