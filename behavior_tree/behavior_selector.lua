local BehaviorNode = require("behavior_node")
local BehaviorSelector = class("BehaviorSelector", BehaviorNode)

function BehaviorSelector:ctor(id, ai, preConditionFunc)
    BehaviorSelector.super.ctor(self, id)

    self.rawset(self,"m_ai",ai)
    self.m_preConditionFunc = preConditionFunc

    self.m_children = object_map()
    self.m_childCount = 0
    self.m_runningIndex = 1
    self.m_isReset = true
end

function BehaviorSelector:dtor()
    for _, child in ipairs(self.m_children) do
        delete(child)
    end

    BehaviorSelector.super.dtor(self)
end

function BehaviorSelector:reset()
    if not self.m_isReset then
        self.m_isReset = true
        for _, child in ipairs(self.m_children) do
            child:reset()
        end
        self.m_runningIndex = nil
    end
end

-- 添加节点
function BehaviorSelector:addChild(node)
    self.m_childCount = self.m_childCount + 1
    self.m_children:set(self.m_childCount,node)
end

-- 更新
function BehaviorSelector:execute(results)
    if results ~= nil then
        table.insert(results, self.m_id)
    end
    -- 检测前置条件
    if self.m_preConditionFunc ~= nil and self.m_preConditionFunc(self.m_ai) ~= BehaviorStatus.Success then
        self:reset()
        return BehaviorStatus.Failure
    end
    self.m_isReset = false
    local index = 1
    if self.m_runningIndex ~= nil then
        index = self.m_runningIndex
        self.m_runningIndex = nil
    end

    for i = index, self.m_childCount do
        local child = self.m_children:get(i)
        local status = child:execute(results)
        if status == BehaviorStatus.Success then
            return status
        elseif status == BehaviorStatus.Running then
            self.m_runningIndex = i
            return status
        end
    end

    return BehaviorStatus.Failure
end

return BehaviorSelector