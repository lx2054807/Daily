
local BehaviorTree = require("behavior_tree")
local BehaviorTreeManager = singleton_class("BehaviorTreeManager")

function BehaviorTreeManager:initialize()
    self.m_allCreators = table.new()
	self.m_allCreators["TEST"] = self.createBt_TEST

end

function BehaviorTreeManager:createBehaviorTree(btName, ai)
    return self.m_allCreators[btName](ai)
end


function BehaviorTreeManager.createBt_TEST(ai)
    local bt = BehaviorTree.new(ai)
	local node5 = bt:createSequence(5, nil)
	local node2 = bt:createSequence(2, nil)
	local node3 = bt:createAction(3, nil, 0, nil)
	local node4 = bt:createAction(4, nil, 0, nil)
	local node6 = bt:createParallel(6, nil)
	local node7 = bt:createAction(7, nil, 1, nil)
	local node8 = bt:createAction(8, nil, 0, nil)
	local node9 = bt:createLoop(9, 5)
	local node10 = bt:createAction(10, nil, 0, nil)
	node5:addChild(node2)
	node2:addChild(node3)
	node2:addChild(node4)
	node5:addChild(node6)
	node6:addChild(node7)
	node6:addChild(node8)
	node6:addChild(node9)
	node9:setChild(node10)

    bt:setRoot(node5)
    return bt
end


return BehaviorTreeManager
