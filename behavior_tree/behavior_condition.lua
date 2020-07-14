local BehaviorNode = require("behavior_node")
local BehaviorCondition = class("BehaviorCondition", BehaviorNode)

function BehaviorCondition:ctor(id, ai, func)
    BehaviorCondition.super.ctor(self, id)

    self.rawset(self,"m_ai",ai)
    self.m_func = func
end

-- 更新
function BehaviorCondition:execute(results)
    if results ~= nil then
        table.insert(results, self.m_id)
    end
    return self.m_func(self.m_ai)
end

return BehaviorCondition