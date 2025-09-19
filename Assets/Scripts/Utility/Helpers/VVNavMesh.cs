using UnityEngine;

namespace VV.Utility
{
    public static class VVNavMesh
    {
        public const float DistanceNavMeshToPhysics = 1f;
        
        // public static bool RaycastToNavmesh(Vector3 start, Vector3 end, out NavMeshHit navMeshHit, int areasLayers) 
        // {
        //     navMeshHit = default;
        //     Ray ray = new Ray(start, end - start);
        //
        //     if (!Physics.Raycast(ray.origin, ray.direction, out RaycastHit raycastHit)) return false;
        //     
        //     var result = NavMesh.SamplePosition(raycastHit.point, out navMeshHit, DistanceNavMeshToPhysics, areasLayers);
        //     return result;
        //
        // }
    }
}