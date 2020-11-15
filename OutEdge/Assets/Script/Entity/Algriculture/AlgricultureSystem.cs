using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class AlgricultureSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        float ct = TimeManager.GetCurrentPlayTime();
        Entities.ForEach((ref IGrowable data,ref CompositeScale scale) => {
            float rate = OutEdge.EntityManager.em.curves[data.curve].Evaluate(math.clamp((ct - data.startTime) / (data.matureTime - data.startTime), 0, 1));

            float4x4 matrix = float4x4.identity;
            matrix.c0.x = rate;
            matrix.c1.y = rate;
            matrix.c2.z = rate;
            matrix.c3.w = 1;
            scale.Value = matrix;
        });
        Entities.ForEach((ref IGrowable data, ref NonUniformScale scale) => {
            float rate = OutEdge.EntityManager.em.curves[data.curve].Evaluate(math.clamp((ct - data.startTime) / (data.matureTime - data.startTime), 0, 1));

            scale.Value = math.float3(rate);
        });
    }
}
