using System;
using System.IO;
using System.Xml;
using UnityEditor;

namespace ROIdleEditor
{
    public static class BehaviorTreeExport
    {
        private static string Content = @"
local BehaviorTree = require(""behavior_tree"")
local BehaviorTreeManager = singleton_class(""BehaviorTreeManager"")

function BehaviorTreeManager:initialize()
    self.m_allCreators = table.new()
#INITCREATORS#
end

function BehaviorTreeManager:createBehaviorTree(btName, ai)
    return self.m_allCreators[btName](ai)
end

#DECLARECREATORS#

return BehaviorTreeManager
";

        public static void ExportBehaviorTree()
        {
            string initCreatorContents = "";
            string declareCreatorContents = string.Empty;
            var allXmlFilePaths = Directory.GetFiles("Assets/Code/Project/Editor/Tools/BehaviorTree/Config", "*.xml");
            foreach (var xmlFilePath in allXmlFilePaths)
            {
                ExportFromXml(xmlFilePath, ref initCreatorContents, ref declareCreatorContents);
            }

            var finalContent = Content.Replace("#INITCREATORS#", initCreatorContents).Replace("#DECLARECREATORS#", declareCreatorContents);
            File.WriteAllText("Assets/res/luascripts/utility/behavior_tree/behavior_tree_manager.lua", finalContent);
        }

        private static void ExportFromXml(string xmlFilePath, ref string initCreatorContents, ref string declareCreatorContents)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlFilePath);

            XmlNode root = document.SelectSingleNode("root");
            if (root != null && root.Attributes != null)
            {
                var aiName = root.Attributes["aiName"].Value;
                initCreatorContents += $"\tself.m_allCreators[\"{aiName}\"] = self.createBt_{aiName}\n";

                var declareCreatorContent = string.Empty;
                CreateNode(root.Attributes["rootNodeID"].Value, root, ref declareCreatorContent);
                CreateNodeRelationship(root.Attributes["rootNodeID"].Value, root, ref declareCreatorContent);

                declareCreatorContents += $@"
function BehaviorTreeManager.createBt_{aiName}(ai)
    local bt = BehaviorTree.new(ai)
{declareCreatorContent}
    bt:setRoot(node{root.Attributes["rootNodeID"].Value})
    return bt
end
";
            }
        }

        //创建节点
        private static void CreateNode(string nodeID, XmlNode root, ref string declareCreatorContent)
        {
            var nodeConfig = root.SelectSingleNode("ID" + nodeID);
            if (nodeConfig != null && nodeConfig.Attributes != null)
            {
                var nodeType = (BehaviorNodeType)int.Parse(nodeConfig.Attributes["type"].Value);
                switch (nodeType)
                {
                    case BehaviorNodeType.Action:
                    {
                        var preCondition = nodeConfig.Attributes["preConditionFunction"] == null
                            ? "nil"
                            : "ai." + nodeConfig.Attributes["preConditionFunction"].Value;
                        var func = nodeConfig.Attributes["function"] == null
                            ? "nil"
                            : "ai." + nodeConfig.Attributes["function"].Value;
                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createAction({nodeID}, {func}, {nodeConfig.Attributes["returnType"].Value}, {preCondition})\n";
                        break;
                    }

                    case BehaviorNodeType.Condition:
                    {
                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createCondition({nodeID}, ai.{nodeConfig.Attributes["function"].Value})\n";
                        break;
                    }
                        
                    case BehaviorNodeType.IfElse:
                    {
                        var preCondition = nodeConfig.Attributes["preConditionFunction"] == null
                            ? "nil"
                            : "ai." + nodeConfig.Attributes["preConditionFunction"].Value;

                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createIfElse({nodeID}, {preCondition})\n";
                        CreateNode(nodeConfig.Attributes["condition"].Value, root, ref declareCreatorContent);
                        CreateNode(nodeConfig.Attributes["success"].Value, root, ref declareCreatorContent);
                        CreateNode(nodeConfig.Attributes["failure"].Value, root, ref declareCreatorContent);
                        break;
                    }

                    case BehaviorNodeType.Selector:
                    {
                        var preCondition = nodeConfig.Attributes["preConditionFunction"] == null
                            ? "nil"
                            : "ai." + nodeConfig.Attributes["preConditionFunction"].Value;

                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createSelector({nodeID}, {preCondition})\n";
                        foreach (var id in nodeConfig.Attributes["children"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                CreateNode(id, root, ref declareCreatorContent);
                            }
                        }
                        break;
                    }

                    case BehaviorNodeType.Sequence:
                    {
                        var preCondition = nodeConfig.Attributes["preConditionFunction"] == null
                            ? "nil"
                            : "ai." + nodeConfig.Attributes["preConditionFunction"].Value;

                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createSequence({nodeID}, {preCondition})\n";
                        foreach (var id in nodeConfig.Attributes["children"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                CreateNode(id, root, ref declareCreatorContent);
                            }
                        }
                        break;
                    }

                    case BehaviorNodeType.Parallel:
                    {
                        var preCondition = nodeConfig.Attributes["preConditionFunction"] == null
                            ? "nil"
                            : "ai." + nodeConfig.Attributes["preConditionFunction"].Value;

                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createParallel({nodeID}, {preCondition})\n";
                        foreach (var id in nodeConfig.Attributes["children"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                CreateNode(id, root, ref declareCreatorContent);
                            }
                        }
                        break;
                    }

                    case BehaviorNodeType.Time:
                    {
                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createTime({nodeID}, {nodeConfig.Attributes["time"].Value})\n";
                        CreateNode(nodeConfig.Attributes["child"].Value, root, ref declareCreatorContent);
                        break;
                    }
                    case BehaviorNodeType.Loop:
                    {
                        declareCreatorContent += $"\tlocal node{nodeID} = bt:createLoop({nodeID}, {nodeConfig.Attributes["loopTime"].Value})\n";
                        CreateNode(nodeConfig.Attributes["child"].Value, root, ref declareCreatorContent);
                        break;
                    }
                }
            }
        }

        //设置节点关系
        private static void CreateNodeRelationship(string nodeID, XmlNode root, ref string declareCreatorContent)
        {
            var nodeConfig = root.SelectSingleNode("ID" + nodeID);
            if (nodeConfig != null && nodeConfig.Attributes != null)
            {
                var nodeType = (BehaviorNodeType)int.Parse(nodeConfig.Attributes["type"].Value);
                switch (nodeType)
                {
                    case BehaviorNodeType.IfElse:
                        declareCreatorContent += $"\tnode{nodeID}:setCondition(node{nodeConfig.Attributes["condition"].Value})\n";
                        declareCreatorContent += $"\tnode{nodeID}:setSuccessBranch(node{nodeConfig.Attributes["success"].Value})\n";
                        declareCreatorContent += $"\tnode{nodeID}:setFailureBranch(node{nodeConfig.Attributes["failure"].Value})\n";
                        CreateNodeRelationship(nodeConfig.Attributes["condition"].Value, root, ref declareCreatorContent);
                        CreateNodeRelationship(nodeConfig.Attributes["success"].Value, root, ref declareCreatorContent);
                        CreateNodeRelationship(nodeConfig.Attributes["failure"].Value, root, ref declareCreatorContent);
                        break;
                    case BehaviorNodeType.Selector:
                        foreach (var id in nodeConfig.Attributes["children"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                declareCreatorContent += $"\tnode{nodeID}:addChild(node{id})\n";
                                CreateNodeRelationship(id, root, ref declareCreatorContent);
                            }
                        }
                        break;
                    case BehaviorNodeType.Sequence:
                        foreach (var id in nodeConfig.Attributes["children"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                declareCreatorContent += $"\tnode{nodeID}:addChild(node{id})\n";
                                CreateNodeRelationship(id, root, ref declareCreatorContent);
                            }
                        }
                        break;
                    case BehaviorNodeType.Parallel:
                        foreach (var id in nodeConfig.Attributes["children"].Value.Split(','))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                declareCreatorContent += $"\tnode{nodeID}:addChild(node{id})\n";
                                CreateNodeRelationship(id, root, ref declareCreatorContent);
                            }
                        }
                        break;
                    case BehaviorNodeType.Time:
                        var childID = nodeConfig.Attributes["child"].Value;
                        declareCreatorContent += $"\tnode{nodeID}:setChild(node{childID})\n";
                        CreateNodeRelationship(childID, root, ref declareCreatorContent);
                        break;
                    case BehaviorNodeType.Loop:
                        childID = nodeConfig.Attributes["child"].Value;
                        declareCreatorContent += $"\tnode{nodeID}:setChild(node{childID})\n";
                        CreateNodeRelationship(childID, root, ref declareCreatorContent);
                        break;
                }
            }
        }
    }
}