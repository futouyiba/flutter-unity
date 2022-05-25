using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ET
{
    /// <summary>
    /// store static methods for dance pool
    /// </summary>
    public class DanceFloorHelper
    {
        public static GameObject GetGoFromScene(string tag)
        {
            var goFind = GameObject.FindGameObjectWithTag(tag);
            if (goFind == null)
            {
                Debug.LogError($"no such go found with tag: {tag}");
                return null;
            }

            return goFind;
        }

        public static float GetPivotY()
        {
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot>();
            return pivot.transform.position.y;
        }

        public static Vector2 PosUnified2Scene(Vector2 unifiedPos)
        {
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot>();
            var pos0 = pivot.small.position;
            var pos1 = pivot.big.position;
            var targetX = math.lerp(pos0.x, pos1.x, unifiedPos.x);
            var targetZ = math.lerp(pos0.z, pos1.z, unifiedPos.y);
            var targetPos = new Vector2(targetX, targetZ);
            return targetPos;
        }

        public static Vector2 PosScene2Unified(Vector2 scenePos)
        {
            var pivot = GetGoFromScene("pivot").GetComponent<DanceFloorPivot>();
            var pos0 = pivot.small.position;
            var pos1 = pivot.big.position;

            var resultX = math.unlerp(pos0.x, pos1.x, scenePos.x);
            var resultY = math.unlerp(pos0.z, pos1.z, scenePos.y);
            Vector2 result = new Vector2(resultX, resultY);
            return result;
        }

        public static Vector2 GetRandomDanceFloorPos()
        {
            float randomX = Random.Range(0f, 1f);
            float randomY = Random.Range(0f, 1f);
            // Debug.Log($"randomed pos is {randomX},{randomY}");
            return new Vector2(randomX, randomY);
        }
        
    }
}
