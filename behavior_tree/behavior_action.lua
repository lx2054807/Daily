local BehaviorNode = require("behavior_node")
local BehaviorAction = class("BehaviorAction", BehaviorNode)

function BehaviorAction:ctor(id, ai, func, returnType, preConditionFunc)
    BehaviorAction.super.ctor(self, id)
    
    self.rawset(self,"m_ai",ai)
    self.m_func = func
    self.m_preConditionFunc = preConditionFunc

    if returnType == 0 then
        self.m_returnValue = BehaviorStatus.Success
    elseif returnType == 1 then
        self.m_returnValue = BehaviorStatus.Failure
    elseif returnType == 2 then
        self.m_returnValue = BehaviorStatus.Running
    else
        self.m_returnValue = nil
    end
end

-- 更新
function BehaviorAction:execute(results)
    if results ~= nil then
        table.insert(results, self.m_id)
    end
    if self.m_preConditionFunc ~= nil then
        local preStatus = self.m_preConditionFunc(self.m_ai)
        if preStatus ~= BehaviorStatus.Success then
            return BehaviorStatus.Failure
        end
    end

    if self.m_func == nil then
        return self.m_returnValue
    else
        local status = self.m_func(self.m_ai)
        if self.m_returnValue == nil then
            return status
        else
            return self.m_returnValue
        end
    end
end

return BehaviorAction