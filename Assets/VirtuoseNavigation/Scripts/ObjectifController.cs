using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectifController : MonoBehaviour
{
    [HideInInspector]
    public Transform Target
    {
        get
        {
            if (_target == null) return ResetAoe;
            return _target;
        }
        set { _target = value; }
    }
    private Transform _target;

    public GameObject IconePrefab;

    public Transform Player;
    private GameObject Icone;
    public float CircleRadius;

    public Transform ResetAoe;

    private Plane plane;
    private Transform PointToLook;

    private (GameObject go, TextBehaviour tb) Distance;

    private void Start()
    {
        Icone = Instantiate(IconePrefab);
        Icone.transform.parent = transform;

        PointToLook = GameObject.Find("RootNode").transform;
        Distance = VectorManager.DrawTextS(Icone.transform, PointToLook, Vector3.down * 0.2f, $"{Vector3.Distance(Target.position, Player.position).ToString("0.00")}", Color.black);
    }

    private Vector3 GetPointOnCircle()
    {
        Vector3 p1 = Player.position;
        Vector3 p2 = Player.position + Player.up;
        Vector3 p3 = Player.position + Player.right;
        plane = new Plane(p1, p2, p3);

        Vector3 pointOfPlane = plane.ClosestPointOnPlane(Target.position);
        Vector3 center = Player.position;
        Vector3 centerToPointVector = pointOfPlane - center;

        return Player.position + centerToPointVector.normalized * CircleRadius;
    }

    private void Update()
    {
        UpdateIcon();
        UpdateText();
    }

    private void UpdateIcon()
    {
        Icone.transform.position = GetPointOnCircle();
        Icone.transform.LookAt(Target.position);
    }

    private void UpdateText()
    {
        float distance = Vector3.Distance(Target.position, Player.position);
        Distance.go.transform.position = Icone.transform.position;
        //Distance.tb.SetText($"{distance.ToString("0.00")}");
        Distance.tb.SetText($"");//TODO a voir
    }
}
