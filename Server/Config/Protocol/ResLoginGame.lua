-- automatically generated by the FlatBuffers compiler, do not modify

-- namespace: Protocol

local flatbuffers = require('flatbuffers')

local ResLoginGame = {} -- the module
local ResLoginGame_mt = {} -- the class metatable

function ResLoginGame.New()
    local o = {}
    setmetatable(o, {__index = ResLoginGame_mt})
    return o
end
function ResLoginGame.GetRootAsResLoginGame(buf, offset)
    local n = flatbuffers.N.UOffsetT:Unpack(buf, offset)
    local o = ResLoginGame.New()
    o:Init(buf, n + offset)
    return o
end
function ResLoginGame_mt:Init(buf, pos)
    self.view = flatbuffers.view.New(buf, pos)
end
function ResLoginGame_mt:Result()
    local o = self.view:Offset(4)
    if o ~= 0 then
        return (self.view:Get(flatbuffers.N.Bool, o + self.view.pos) ~= 0)
    end
    return false
end
function ResLoginGame_mt:AccountId()
    local o = self.view:Offset(6)
    if o ~= 0 then
        return self.view:String(o + self.view.pos)
    end
end
function ResLoginGame_mt:RoleInfoLites(j)
    local o = self.view:Offset(8)
    if o ~= 0 then
        local x = self.view:Vector(o)
        x = x + ((j-1) * 4)
        x = self.view:Indirect(x)
        local obj = require('Protocol.RoleInfoLite').New()
        obj:Init(self.view.bytes, x)
        return obj
    end
end
function ResLoginGame_mt:RoleInfoLitesLength()
    local o = self.view:Offset(8)
    if o ~= 0 then
        return self.view:VectorLen(o)
    end
    return 0
end
function ResLoginGame.Start(builder) builder:StartObject(3) end
function ResLoginGame.AddResult(builder, result) builder:PrependBoolSlot(0, result, 0) end
function ResLoginGame.AddAccountId(builder, accountId) builder:PrependUOffsetTRelativeSlot(1, accountId, 0) end
function ResLoginGame.AddRoleInfoLites(builder, roleInfoLites) builder:PrependUOffsetTRelativeSlot(2, roleInfoLites, 0) end
function ResLoginGame.StartRoleInfoLitesVector(builder, numElems) return builder:StartVector(4, numElems, 4) end
function ResLoginGame.End(builder) return builder:EndObject() end

return ResLoginGame -- return the module