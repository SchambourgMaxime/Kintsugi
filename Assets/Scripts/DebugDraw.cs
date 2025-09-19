using UnityEngine;

public static class DebugDraw
{
    
    public static void DrawSphere(Vector3 center, float radius = 1f, Color? color = null, float lifetime = 2f)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        
        sphere.transform.position = center;
        sphere.transform.localScale = new Vector3(radius, radius, radius);
        
        Color defaultColor = Color.magenta;
        sphere.GetComponent<MeshRenderer>().material = new Material(sphere.GetComponent<MeshRenderer>().material);
        sphere.GetComponent<MeshRenderer>().material.color = color.GetValueOrDefault(defaultColor);
        
        if(lifetime >= 0f)
            Object.Destroy(sphere, lifetime);
        #endif
    }
}