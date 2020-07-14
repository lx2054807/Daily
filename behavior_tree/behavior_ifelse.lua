local BehaviorNode = require("behavior_node")
local BehaviorIfElse = class("BehaviorIfElse", BehaviorNode)

function BehaviorIfElse:ctor(id, ai, preConditionFunc)
    BehaviorIfElse.super.ctor(self, id)

    self.rawset(self,"m_ai",ai)
    self.m_preConditionFunc = preConditionFunc

    self.m_condition = nil
    self.m_successBranch = nil
    self.m_failureBranch = nil

    -- 正在执行的分支
    self.m_runningBranch = nil
    self.m_isReset = true
end

function BehaviorIfElse:reset()
    if not self.m_isReset then
        self.m_isReset = true
        self.m_successBranch:reset()
        self.m_failureBranch:reset()
        self.m_runningBranch = nil
    end
end

function BehaviorIfElse:dtor()
    delete(self.m_condition)
    delete(self.m_successBranch)
    delete(self.m_failureBranch)

    BehaviorIfElse.super.dtor(self)
end

-- 设置条件
function BehaviorIfElse:setCondition(node)
    self.m_condition = node
end

-- 设置执行器
function BehaviorIfElse:setSuccessBranch(node)
    self.m_successBranch = node
end

function BehaviorIfElse:setFailureBranch(node)
    self.m_failureBranch = node
end

-- 更新
function BehaviorIfElse:execute(results)
    if results ~= nil then
        table.insert(results, self.m_id)
    end
    -- 检测前置条件
    if self.m_preConditionFunc ~= nil and self.m_preConditionFunc(self.m_ai) ~= BehaviorStatus.Success then
        self:reset()
        return BehaviorStatus.Failure
    end
    self.m_isReset = false
    if self.m_runningBranch == nil then
        -- 根据条件结果执行对应的分支
        local status = self.m_condition:execute(results)
        if status == BehaviorStatus.Success then
            status = self.m_successBranch:execute(results)
            if status == BehaviorStatus.Running then
                self.m_runningBranch = self.m_successBranch
            end
        else
            status = self.m_failureBranch:execute(results)
            if status == BehaviorStatus.Running then
                self.m_runningBranch = self.m_failureBranch
            end
        end
        return status
    else
        -- 执行正在运行的分支
        local status = self.m_runningBranch:execute(results)
        if status ~= BehaviorStatus.Running then
            self.m_runningBranch = nil
        end
        return status
    end
end

return BehaviorIfElse