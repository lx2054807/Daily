local BehaviorNode = require("behavior_node")
local BehaviorLoop = class("BehaviorLoop", BehaviorNode)

function BehaviorLoop:ctor(id, ai, loopTime)
    BehaviorLoop.super.ctor(self, id)

    self.rawset(self,"m_ai",ai)

    self.m_child = nil
    self.m_curLoopTime = 1
    self.m_loopTime = loopTime
    self.m_isReset = true
end

function BehaviorLoop:dtor()
    delete(self.m_child)

    BehaviorLoop.super.dtor(self)
end

function BehaviorLoop:reset()
    if not self.m_isReset then
        self.m_curLoopTime = 1
        self.m_isReset = true
        self.m_child:reset()
    end
end

-- 添加节点
function BehaviorLoop:setChild(node)
    self.m_child = node
end

function BehaviorLoop:execute(results)
    if results ~= nil then
        table.insert( results, self.m_id )
    end

    while self.m_curLoopTime <= self.m_loopTime do
        self.m_isReset = false
        local status = self.m_child:execute(results)
        if status == BehaviorStatus.Failure then
            self.m_curLoopTime = 1
            return status
        elseif status == BehaviorStatus.Running then
            return status
        else
            self.m_curLoopTime = self.m_curLoopTime + 1
        end
    end

    self.m_curLoopTime = 1
    return BehaviorStatus.Success
end

return BehaviorLoop