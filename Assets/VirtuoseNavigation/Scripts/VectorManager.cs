using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorManager : MonoBehaviour
{
    public static VectorManager VECTOR_MANAGER;

    public float vectorRadius;
    public float lifeTime;

    public GameObject textPrefab;

    private void Awake()
    {
        VECTOR_MANAGER = this;
    }

    public static void DrawVectorS(Vector3 position, Vector3 val, Color color, string name = "not assigned")
    {
        VECTOR_MANAGER.DrawVector(position, val, color, name);
    }
    
    public static void DrawLineS(Vector3 start, Vector3 end, float radius, Color color)
    {
        VECTOR_MANAGER.DrawLine(start, end, radius, color);
    }

    public void DrawLine(Vector3 start, Vector3 end, float radius, Color color)
    {
        float distance = Vector3.Distance(start, end);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = Vector3.Lerp(start, end, 0.5f);
        cylinder.transform.localScale = new Vector3(radius, distance / 2.0f, radius);
        cylinder.transform.LookAt(end);
        cylinder.transform.Rotate(Vector3.right, 90.0f);

        cylinder.GetComponent<Renderer>().material.color = color;

        //add the vector under the VectorCreator
        cylinder.transform.parent = transform;
    }

    public void DrawVector(Vector3 position, Vector3 val, Color color, string name = "not assigned")
    {
        Vector3 endPoint = position + val;
        float distance = Vector3.Distance(position, endPoint);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = Vector3.Lerp(position, endPoint, 0.5f);
        cylinder.transform.localScale = new Vector3(vectorRadius, distance / 2.0f, vectorRadius);
        cylinder.transform.LookAt(endPoint);
        cylinder.transform.Rotate(Vector3.right, 90.0f);


        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = Vector3.one * vectorRadius;
        cube.transform.position = endPoint;

        cylinder.GetComponent<Renderer>().material.color = cube.GetComponent<Renderer>().material.color = color;

        //add the vector under the VectorCreator
        cylinder.transform.parent = transform;
        cube.transform.parent = cylinder.transform;

        //just to be able to see the data in the inspector
        {
            VectorData cubeData = cube.AddComponent<VectorData>();
            VectorData cylinderData = cylinder.AddComponent<VectorData>();

            cubeData.origin = cylinderData.origin = position;
            cubeData.value = cylinderData.value = val;
            cubeData.endPoint = cylinderData.endPoint = endPoint;
            cubeData.magnitude = cylinderData.magnitude = distance;
            cubeData.nameReference = cylinderData.nameReference = name;
        }

        Destroy(cylinder, lifeTime);
        Destroy(cube, lifeTime);
    }

    public static void Clear()
    {
        VECTOR_MANAGER.ClearVector();
    }

    public static GameObject DrawSphereS(Vector3 position, Vector3 scale, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = scale;
        sphere.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));
        sphere.GetComponent<Renderer>().material.color = color;
        sphere.transform.parent = VECTOR_MANAGER.transform;

        return sphere;
    }

    public static (GameObject go, TextBehaviour tb) DrawTextS(Transform target, Transform viewer, Vector3 offset, string text, Color color)
    {
        return VECTOR_MANAGER.DrawTextAboveTransform(target, viewer, offset, text, color);
    }

    public (GameObject, TextBehaviour) DrawTextAboveTransform(Transform target, Transform viewer, Vector3 offset, string text, Color color)
    {
        GameObject textGO = Instantiate(textPrefab,Vector3.zero,Quaternion.identity);
        textGO.transform.parent = transform;

        TextBehaviour tb = textGO.GetComponent<TextBehaviour>();
        tb.SetParam(target, viewer, offset, text, color);

        return (textGO, tb);

    }

    public static GameObject DrawLineS(Vector3 start, Vector3 end, Color color, float lineSize = 0.03f)
    {
        return VECTOR_MANAGER.DrawLine(start, end, color, lineSize);
    }

    public GameObject DrawLine(Vector3 start, Vector3 end, Color color, float lineSize = 0.03f)
    {
        GameObject lineGO = new GameObject("Line");
        LineRenderer lineR = lineGO.AddComponent<LineRenderer>();
        lineR.material = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineR.startColor = color;
        lineR.endColor = color;
        lineR.startWidth = lineSize;
        lineR.endWidth = lineSize;
        lineR.positionCount = 2;
        lineR.SetPosition(0, start);
        lineR.SetPosition(1, end);

        return lineGO;
    }

    public void ClearVector()
    {
        List<GameObject> vectors = new List<GameObject>();
        foreach (Transform vector in transform)
            vectors.Add(vector.gameObject);
        foreach (GameObject vector in vectors)
            Destroy(vector);
    }
}


public class VectorData : MonoBehaviour
{
    public Vector3 origin;
    public Vector3 value;
    public Vector3 endPoint;
    public float magnitude;
    public string nameReference;
}
