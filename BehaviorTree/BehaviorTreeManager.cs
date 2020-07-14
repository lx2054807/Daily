using System;
using System.Collections.Generic;
using System.IO;
using ROCommon;
using UnityEngine;
using static System.String;

namespace ROIdleEditor
{
    public enum AIType
    {
        Hero,
        Monster,
        Boss,
        PlayerTeam,
        MonsterTeam,
    }

    public class AIFunctionData
    {
        public string FunctionName { get; }
        public string FunctionDesc { get; }

        public AIFunctionData(string name, string desc)
        {
            FunctionName = name;
            FunctionDesc = desc;
        }

        public override string ToString()
        {
            var str = FunctionName;
            var delta = 35 - FunctionName.Length;
            for (var i = 0; i < delta; ++i)
            {
                str += " ";
            }
            return " " + str + "\t" + FunctionDesc;
        }
    }

    public class BehaviorTreeManager : Singleton<BehaviorTreeManager>
    {
        /// <summary>
        /// 所有AI配置
        /// </summary>
        public readonly List<string> AIConfigs = new List<string>();

        /// <summary>
        /// AI函数列表
        /// </summary>
        public readonly Dictionary<AIType, List<AIFunctionData>> AIFunctions = new Dictionary<AIType, List<AIFunctionData>>();

        private const string RoleLuaFilePath = "res/luascripts/game/role/ai/role_ai.lua";
        private const string HeroLuaFilePath = "res/luascripts/game/role/ai/hero_ai.lua";
        private const string MonsterLuaFilePath = "res/luascripts/game/role/ai/monster_ai.lua";

        private const string TeamLuaFilePath = "res/luascripts/game/team/ai/team_ai.lua";
        private const string PlayerTeamLuaFilePath = "res/luascripts/game/team/ai/player_team_ai.lua";
        private const string MonsterTeamLuaFilePath = "res/luascripts/game/team/ai/monster_team_ai.lua";

        public string CurrentAIName { get; set; }
        public AIType CurrentAIType { get; set; }

        public override void Initialize()
        {
            CurrentAIType = AIType.Hero;

            InitAIConfigs();
            InitAIFunctions();
        }

        #region AIConfig

        private void InitAIConfigs()
        {
            RefreshAIConfigs();
        }

        public void RefreshAIConfigs()
        {
            var files = Directory.GetFiles("Assets/Code/Project/Editor/Tools/BehaviorTree/Config", "*.xml");
            AIConfigs.Clear();
            AIConfigs.AddRange(files);
        }
        #endregion

        #region AIFunction
        private void InitAIFunctions()
        {
            RefreshAIFunctions();
        }

        public void RefreshAIFunctions()
        {
            AIFunctions.Clear();

            var heroAIFunctions = new List<AIFunctionData>();
            ReadAIFunctionFromFile(RoleLuaFilePath, "Role", heroAIFunctions);
            ReadAIFunctionFromFile(HeroLuaFilePath, "Hero", heroAIFunctions);
            AIFunctions.Add(AIType.Hero, heroAIFunctions);

            var monsterAIFunctions = new List<AIFunctionData>();
            ReadAIFunctionFromFile(RoleLuaFilePath, "Role", monsterAIFunctions);
            ReadAIFunctionFromFile(MonsterLuaFilePath, "Monster", monsterAIFunctions);
            AIFunctions.Add(AIType.Monster, monsterAIFunctions);

            var bossAIFunctions = new List<AIFunctionData>();
            ReadAIFunctionFromFile(RoleLuaFilePath, "Role", bossAIFunctions);
            ReadAIFunctionFromFile(MonsterLuaFilePath, "Monster", bossAIFunctions);
            AIFunctions.Add(AIType.Boss, bossAIFunctions);

            var playerTeamAIFunctions = new List<AIFunctionData>();
            ReadAIFunctionFromFile(TeamLuaFilePath, "TeamAI", playerTeamAIFunctions);
            ReadAIFunctionFromFile(PlayerTeamLuaFilePath, "PlayerTeamAI", playerTeamAIFunctions);
            AIFunctions.Add(AIType.PlayerTeam, playerTeamAIFunctions);

            var monsterTeamAIFunctions = new List<AIFunctionData>();
            ReadAIFunctionFromFile(TeamLuaFilePath, "TeamAI", monsterTeamAIFunctions);
            ReadAIFunctionFromFile(MonsterTeamLuaFilePath, "MonsterTeamAI", monsterTeamAIFunctions);
            AIFunctions.Add(AIType.MonsterTeam, monsterTeamAIFunctions);

            foreach (var pair in AIFunctions)
            {
                pair.Value.Insert(0, new AIFunctionData("", "无"));
            }
        }

        private void ReadAIFunctionFromFile(string filePath, string className, List<AIFunctionData> functionDatas)
        {
            var availableFunctions = new Dictionary<string, string>();

            var allLines = File.ReadAllLines(PathHelper.Combine(Application.dataPath, filePath));
            var list = new List<string>(allLines);
            for (var i = list.Count - 1; i >= 0; --i)
            {
                var line = list[i];
                if (line.StartsWith("function", System.StringComparison.Ordinal))
                {
                    string desc = Empty;
                    if (i > 0)
                    {
                        var preLine = list[i - 1];
                        if (preLine.StartsWith("--", System.StringComparison.Ordinal))
                        {
                            desc = preLine;
                        }
                    }
                    availableFunctions[line] = desc;
                }
            }

            //格式化成需要的格式
            foreach (var pair in availableFunctions)
            {
                var index1 = pair.Key.IndexOf(":", System.StringComparison.Ordinal);
                var index2 = pair.Key.IndexOf("(", System.StringComparison.Ordinal);
                var funcName = pair.Key.Substring(index1 + 1, index2 - index1 - 1);
                var funcDesc = pair.Value.TrimStart('-').Trim();
                if (funcName.StartsWith("ai_"))
                {
                    functionDatas.Add(new AIFunctionData(funcName, funcDesc));
                }
            }

            //排序
            functionDatas.Sort((d1, d2) => Compare(d1.FunctionName, d2.FunctionName, StringComparison.Ordinal));
        }
        #endregion
    }
}