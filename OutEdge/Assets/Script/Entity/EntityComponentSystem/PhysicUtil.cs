using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using static Unity.Entities.World;

public class PhysicUtil
{
    public static Entity Raycast(float3 RayFrom, float3 RayTo, NoAllocReadOnlyCollection<World> worlds,out Unity.Physics.RaycastHit hit)
    {
        foreach (World world in worlds) {
            try
            {
                var physicsWorldSystem = world.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
                var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
                RaycastInput input = new RaycastInput()
                {
                    Start = RayFrom,
                    End = RayTo,
                    Filter = new CollisionFilter()
                    {
                        BelongsTo = ~0u,
                        CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                        GroupIndex = 0
                    }
                };

                hit = new Unity.Physics.RaycastHit();
                bool haveHit = collisionWorld.CastRay(input, out hit);
                if (haveHit)
                {
                    // see hit.Position
                    // see hit.SurfaceNormal
                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    return e;
                }
            }
            catch { }
        }

        hit = default;
        return Entity.Null;
    }

    public static Entity Raycast(float3 RayFrom, float3 RayTo,out Unity.Physics.RaycastHit hit)
    {
        return Raycast(RayFrom, RayTo, All, out hit);
    }

    public static float3 Convert(Vector3 input)
    {
        return math.float3(input.x, input.y, input.z);
    }
}
