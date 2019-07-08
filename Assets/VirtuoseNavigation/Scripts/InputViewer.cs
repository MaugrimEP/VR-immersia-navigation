using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputViewer : MonoBehaviour
{
    public VirtuoseElasticNavigation InputController;
    public Transform HeadNode;

    public Material SphereMaterial;

    public float SphereRadius;
    public float arrowWidth;
    public float arrowLength;

    public Vector3 offsetFromRootNode;

    private GameObject Sphere;
    private GameObject arrows;

    private void Start()
    {
        Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.parent = transform;
        Sphere.transform.localScale = Vector3.one * SphereRadius;
        Sphere.transform.localPosition = Vector3.zero;
        Sphere.transform.localRotation = Quaternion.identity;
        Sphere.GetComponent<Renderer>().material = SphereMaterial;

        arrows = CreateArrows();
        arrows.transform.parent = Sphere.transform;

        HeadNode = GameObject.Find("HeadNode").transform;
    }

    private GameObject CreateArrow(GameObject parent, Color color)
    {
        GameObject arrowWrapper = new GameObject();
        arrowWrapper.transform.localPosition = Vector3.zero;
        arrowWrapper.transform.localRotation = Quaternion.identity;
        arrowWrapper.transform.parent = parent.transform;
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        arrow.transform.parent = arrowWrapper.transform;
        arrow.transform.localScale = new Vector3(arrowWidth, arrowLength, arrowWidth);
        arrow.transform.localPosition = Vector3.up * arrowLength / 2f;
        arrow.GetComponent<Renderer>().material.color = color;
        return arrowWrapper;
    }

    private GameObject CreateArrows()
    {
        GameObject arrows = new GameObject();
        GameObject arrowForward = CreateArrow(arrows, Color.blue);
        arrowForward.transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);

        GameObject arrowRight = CreateArrow(arrows, Color.red);
        arrowRight.transform.localRotation = Quaternion.AngleAxis(-90f, Vector3.forward);

        GameObject arrowUp = CreateArrow(arrows, Color.green);

        return arrows;
    }

    private void Update()
    {
        transform.position = HeadNode.localRotation * offsetFromRootNode;

        Vector3 inputs = InputController.GetTranslation();
        Vector3 inputsInSphere = inputs * SphereRadius;

        Quaternion inputsRot = InputController.GetRotation();

        arrows.transform.localPosition = inputsInSphere;
        arrows.transform.localRotation = inputsRot;
    }
}
