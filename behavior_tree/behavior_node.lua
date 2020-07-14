require("behavior_status")

local BehaviorNode = class("BehaviorNode")

function BehaviorNode:ctor(id)
    self.m_id = id
    self.m_status = BehaviorStatus.Invalid
end

function BehaviorNode:reset()
end

-- 执行节点
function BehaviorNode:execute(results)
    return BehaviorStatus.Failure
end

-- 获取当前状态
function BehaviorNode:getStatus()
    return self.m_status
end

return BehaviorNode