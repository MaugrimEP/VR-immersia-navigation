using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBehaviour : MonoBehaviour
{
    private Color color;
    private Vector3 offsetPosition;
    private Transform target;
    private Transform viewer;
    private string text;

    private TextMeshPro tm;

    private void Start()
    {
        tm = GetComponentsInChildren<TextMeshPro>()[0];
    }

    public void SetParam(Transform target, Transform viewer, Vector3 offsetPosition, string text, Color color)
    {
        tm = GetComponentsInChildren<TextMeshPro>()[0];

        this.target = target;
        this.viewer = viewer;
        this.offsetPosition = offsetPosition;
        this.text = text;
        this.color = color;

        tm.color = this.color;
        tm.SetText(this.text);
    }

    public void SetText(string text)
    {
        this.text = text;
        tm.SetText(this.text);
    }

    private void Update()
    {
        transform.position = target.position + offsetPosition;
        if(viewer!=null)
            transform.LookAt(viewer);
        transform.rotation *= Quaternion.Euler(Vector3.up * 180);
    }
}
