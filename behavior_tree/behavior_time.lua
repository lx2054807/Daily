local BehaviorNode = require("behavior_node")
local BehaviorTime = class("BehaviorTime", BehaviorNode)

function BehaviorTime:ctor(id, time)
    BehaviorTime.super.ctor(self, id)

    self.m_child = nil
    self.m_maxTime = time
    self.m_curTime = 0
    self.m_FPS = 30
    self.m_isReset = true
end

function BehaviorTime:dtor()
    delete(self.m_child)

    BehaviorTime.super.dtor(self)
end

function BehaviorTime:reset()
    if not self.m_isReset then
        self.m_curTime = 0
        self.m_isReset = true
        self.m_child:reset()
    end
end

-- 设置条件
function BehaviorTime:setChild(node)
    self.m_child = node
end

-- 更新
function BehaviorTime:execute(results)
    if results ~= nil then
        table.insert(results, self.m_id)
    end
    self.m_curTime = self.m_curTime + 1/self.m_FPS
    if self.m_curTime >= self.m_maxTime then
        self.m_curTime = 0
        return BehaviorStatus.Success
    else
        self.m_isReset = false
        local status = self.m_child:execute(results)
        if status == BehaviorStatus.Failure then
            self.m_curTime = 0
            return BehaviorStatus.Failure
        else
            return BehaviorStatus.Running
        end
    end
end

return BehaviorTime