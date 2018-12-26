﻿using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Module
{
    public class ActorController : UpdateModule
    {
        protected override void InitRequiredDataType()
        {
            _requiredDataTypeList.Add(typeof(ActorData));
            _requiredDataTypeList.Add(typeof(ActorJumpData));
            _requiredDataTypeList.Add(typeof(DirectionData));
            _requiredDataTypeList.Add(typeof(SpeedData));
            _requiredDataTypeList.Add(typeof(ServerJoyStickData));
            _requiredDataTypeList.Add(typeof(ResourceData));
            _requiredDataTypeList.Add(typeof(ResourceStateData));
        }

        public override bool IsUpdateRequired(Data.Data data)
        {
            return data.GetType() == typeof(ServerJoyStickData);
        }

        public override void Refresh(ObjectData objData)
        {
            var resourceStateData = objData.GetData<ResourceStateData>();
            if (!resourceStateData.isInstantiated)
            {
                return;
            }

            var joyStickData = objData.GetData<ServerJoyStickData>();
            var serverActionList = joyStickData.actionList;
            if (serverActionList.Count == 0)
            {
                Stop(objData.ObjectId);
                return;
            }

            var worldMgr = WorldManager.Instance;

            var gameSystemData = worldMgr.GameCore.GetData<GameSystemData>();

            var actorData = objData.GetData<ActorData>();
            var actorInfo = worldMgr.ActorConfig.Get(actorData.actorId);

            var speedData = objData.GetData<SpeedData>();
            var directionData = objData.GetData<DirectionData>();

            var jumpData = objData.GetData<ActorJumpData>();
            var resourceData = objData.GetData<ResourceData>();

            for (var i = 0; i < serverActionList.Count;)
            {
                var serverAction = serverActionList[i];
                if (serverAction.frame == gameSystemData.clientFrame)
                {
                    switch (serverAction.actionType)
                    {
                        case JoyStickActionType.Run:
                            speedData.speed = actorInfo.speed;
                            speedData.friction = 0;
                            directionData.direction.x = serverAction.actionParam == JoyStickActionFaceType.Right ? 1 : -1;

                            objData.SetDirty(speedData, directionData);
                            break;
                        case JoyStickActionType.CancelRun:
                            speedData.friction = actorInfo.friction;

                            objData.SetDirty(speedData);
                            break;
                        case JoyStickActionType.Jump:
                            jumpData.currentJump = actorInfo.jump;

                            var position = resourceData.gameObject.transform.position;
                            actorData.ground.x = Mathf.CeilToInt(position.x * Constant.UNITY_UNIT_TO_GAME_UNIT);
                            actorData.ground.y = Mathf.CeilToInt(position.y * Constant.UNITY_UNIT_TO_GAME_UNIT);
                            actorData.ground.z = Mathf.CeilToInt(position.z * Constant.UNITY_UNIT_TO_GAME_UNIT);

                            objData.SetDirty(actorData, jumpData);
                            break;
                    }

                    serverActionList.Remove(serverAction);
                    worldMgr.PoolMgr.Release(serverAction);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
