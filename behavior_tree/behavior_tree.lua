local BehaviorAction = require("behavior_action")
local BehaviorCondition = require("behavior_condition")
local BehaviorIfElse = require("behavior_ifelse")
local BehaviorSelector = require("behavior_selector")
local BehaviorSequence = require("behavior_sequence")
local BehaviorTime = require("behavior_time")
local BehaviorParallel = require("behavior_parallel")
local BehaviorLoop = require("behavior_loop")

local BehaviorTree = class("BehaviorTree")

function BehaviorTree:ctor(ai)
    self.m_enable = true
    self.m_root = nil
    self.m_ai = ai
end

function BehaviorTree:dtor()
    delete(self.m_root)
	BehaviorTree.super.dtor(self)
end

function BehaviorTree:execute(results)
    if self.m_enable then
        self.m_root:execute(results)
    end
end

function BehaviorTree:setEnable(enable)
    self.m_enable = enable
end

-- 设置根节点
function BehaviorTree:setRoot(root)
    self.m_root = root
end

-- 创建Action节点
function BehaviorTree:createAction(id, func, returnType, preConditionFunc)
    return BehaviorAction.new(id, self.m_ai, func, returnType, preConditionFunc)
end

-- 创建Condition节点
function BehaviorTree:createCondition(id, func)
    return BehaviorCondition.new(id, self.m_ai, func)
end

-- 创建IfElse节点
function BehaviorTree:createIfElse(id, preConditionFunc)
    return BehaviorIfElse.new(id, self.m_ai, preConditionFunc)
end

-- 创建Selector节点
function BehaviorTree:createSelector(id, preConditionFunc)
    return BehaviorSelector.new(id, self.m_ai, preConditionFunc)
end

-- 创建Sequence节点
function BehaviorTree:createSequence(id, preConditionFunc)
    return BehaviorSequence.new(id, self.m_ai, preConditionFunc)
end

-- 创建Time节点
function BehaviorTree:createTime(id, time)
    return BehaviorTime.new(id, time)
end

-- 创建Parallel节点
function BehaviorTree:createParallel(id, preConditionFunc)
    return BehaviorParallel.new(id, self.m_ai, preConditionFunc)
end

-- 创建Loop节点
function BehaviorTree:createLoop(id, loopTime)
    return BehaviorLoop.new(id, self.m_ai, loopTime)
end

return BehaviorTree