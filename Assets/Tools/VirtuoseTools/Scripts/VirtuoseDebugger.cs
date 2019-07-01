using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class VirtuoseDebugger : MonoBehaviour
{
    VirtuoseManager vm;

    public VRInput toggleDebugInfo;

    Canvas canvas;
    Text debugText;

    public Material BaseMaterial;
    public Material BubbleInsideMaterial;
    public Material BubbleOutsideMaterial;

    public Color positionColor = Color.blue;
    public Color articularPositionsColor = Color.red;
    public Color physicalPositionColor = Color.yellow;
    public Color avatarPositionColor = Color.gray;
    public Color baseFramePositionColor = Color.green;
    public Color observationFramePositionColor = Color.cyan;
    public Color baseColor = Color.white;
    public Color bubbleColor = Color.black;

    public Vector3 Offset;

    StringBuilder debugInfo;

    int axesNumber;
    GameObject articularObject;
    GameObject postionObject;
    GameObject physicalObject;
    GameObject physicalOffsetObject;
    GameObject avatarObject;
    GameObject baseFrameObject;
    GameObject observationFrameObject;
    GameObject baseObject;
    GameObject bubbleInsideObject;
    GameObject bubbleOutsideObject;

    float[] referenceArticulars;

    public bool IsOverlay;

    void Start()
    {
        debugInfo = new StringBuilder();

        vm = this.GetComponentInParentLogIfNull<VirtuoseManager>();
        canvas = this.GetComponentInChildrenLogIfNull<Canvas>();
        debugText = canvas.GetComponentInChildrenLogIfNull<Text>();

        if (IsOverlay)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        else
            canvas.renderMode = RenderMode.WorldSpace;

        CreateObjects();

        StartCoroutine(vm.WaitVirtuoseConnexion(Init));
    }

    private void Init()
    {
        referenceArticulars = vm.Virtuose.Articulars;
    }

    void CreateObjects()
    {
        CreateArticularObject();
        CreatePositionObject();
        CreatePhysicalObjects();
        CreateAvatarObject();
        CreateBaseFrameObject();
        CreateObservationFrameObject();
        CreateBaseObject();
        CreateBubbleObjects();
    }

    void CreateArticularObject()
    {
        Material material = new Material(BaseMaterial);
        material.color = articularPositionsColor;
        articularObject = CreateObject(PrimitiveType.Cube, material);
        articularObject.name = "ArticularObject";
    }

    void CreatePositionObject()
    {
        Material material = new Material(BaseMaterial);
        material.color = positionColor;
        postionObject = CreateObject(PrimitiveType.Cube, material);
        postionObject.name = "GetPostion";
    }

    void CreateAvatarObject()
    {
        Material material = new Material(BaseMaterial);
        material.color = avatarPositionColor;
        avatarObject = CreateObject(PrimitiveType.Cube, material);
        avatarObject.name = "GetAvatarPosition";
    }

    void CreatePhysicalObjects()
    {
        Material material = new Material(BaseMaterial);
        material.color = physicalPositionColor;
        physicalObject = CreateObject(PrimitiveType.Cube, material);
        physicalObject.name = "GetPhysicalPosition";

        material.color = physicalPositionColor;
        physicalOffsetObject = CreateObject(PrimitiveType.Cube, material);
        physicalOffsetObject.name = "GetPhysicalOffsetPosition";
    }

    void CreateBaseFrameObject()
    {
        Material material = new Material(BaseMaterial);
        material.color = baseFramePositionColor;
        baseFrameObject = CreateObject(PrimitiveType.Cube, material);
        baseFrameObject.name = "GetBaseFrame";
    }

    void CreateObservationFrameObject()
    {
        Material material = new Material(BaseMaterial);
        material.color = observationFramePositionColor;
        observationFrameObject = CreateObject(PrimitiveType.Cube, material);
        observationFrameObject.name = "GetObservationFrame";
    }

    void CreateBaseObject()
    {
        Material material = new Material(BaseMaterial);
        material.color = baseColor;
        baseObject = CreateObject(PrimitiveType.Cube, material);
        baseObject.transform.localScale += Vector3.up * 0.05f;
        baseObject.name = "Base";
    }

    void CreateBubbleObjects()
    {
        bubbleInsideObject = CreateObject(PrimitiveType.Sphere, BubbleInsideMaterial);
        bubbleInsideObject.transform.localScale = Vector3.one * 0.15f * 2;
        bubbleInsideObject.name = "BubbleInside";

        Material material = new Material(BubbleOutsideMaterial);
        bubbleOutsideObject = CreateObject(PrimitiveType.Sphere, material);
        bubbleOutsideObject.transform.localScale = Vector3.one * 0.18f * 2;
        bubbleOutsideObject.name = "BubbleOutisde";
    }

    GameObject CreateObject(PrimitiveType primitiveType, Material material)
    {
        GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
        gameObject.transform.parent = transform;
        gameObject.transform.localScale = Vector3.one * 0.05f;
#if MIDDLEVR
        if(VRTools.Mode == VRToolsMode.MIDDLEVR)
            gameObject.AddComponent<VRClusterObject>();
#endif
        Collider collider = gameObject.GetComponent<Collider>();
        Destroy(collider);
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.material = material;
        return gameObject;
    }

    void Update()
    {
        if (VRTools.IsMaster() && vm.Initialized)
        {
            UpdateObjects();

            if (canvas.enabled)         
                UpdateDebugInfo();

            if (toggleDebugInfo != null && toggleDebugInfo.IsToggled())
                canvas.enabled = !canvas.enabled;

            if (vm.Virtuose.IsButtonToggled())
                referenceArticulars = vm.Virtuose.Articulars;
        }
    }

    void UpdateObjects()
    {
        UpdateObjectPose(postionObject, vm.Virtuose.Pose);
        UpdateObjectPose(avatarObject, vm.Virtuose.AvatarPose);
        UpdateObjectPose(physicalObject, vm.Virtuose.PhysicalPose);
        UpdateObjectPose(physicalOffsetObject, vm.Virtuose.ComputePhysicalPose(Offset));
        UpdateObjectPose(baseFrameObject, vm.Virtuose.BaseFrame);
        UpdateObjectPose(observationFrameObject, vm.Virtuose.ObservationFrame);

        if (vm.Virtuose.IsScaleOne)
        {
            UpdateObjectPose(articularObject, (VirtuoseAPIHelper.VirtuoseToUnityPosition(vm.Virtuose.Articulars), Quaternion.identity));
            UpdateObjectPose(baseObject, vm.Virtuose.PhysicalBasePose);
            var bubblePose = (vm.Virtuose.ComputeBubblePosition(Offset), Quaternion.identity);
            UpdateObjectPose(bubbleInsideObject, bubblePose);
            UpdateObjectPose(bubbleOutsideObject, bubblePose);
        }
    }

    void UpdateObjectPose(GameObject go, ValueTuple<Vector3, Quaternion> pose)
    {
        go.transform.position = pose.Item1;
        go.transform.rotation = pose.Item2;
    }

    void UpdateDebugInfo()
    {
        if (canvas.enabled)
        {
            debugInfo.Clear();
            debugInfo.Append(vm.Arm.ToString());
            debugInfo.Append("Scale1? ").Append(vm.Virtuose.IsScaleOne).AppendLine();
            if(referenceArticulars != null && vm.Virtuose.IsScaleOne)
                debugInfo.Append("Joystick ").Append(vm.Virtuose.Joystick(referenceArticulars).ToString("F3")).AppendLine();

            debugInfo.Append("virtGetDeviceType ").Append(vm.Virtuose.DeviceID).Append(" Serial ").Append(vm.Virtuose.SerialNumber).AppendLine();
            debugInfo.Append("virtGetButton ");
            for (int b = 0; b < 3; b++)
            {
                debugInfo.Append(b).Append(":").Append(vm.Virtuose.Button(b));
                if (b != 2)
                    debugInfo.Append(", ");
            }

            debugInfo.AppendLine();
            AppendInfo("virtGetDeadMan ", vm.Virtuose.DeadMan);
            AppendInfo("virtGetEmergencyStop ", vm.Virtuose.EmergencyStop);
            AppendInfo("virtGetError ", vm.Virtuose.ErrorCode);
            AppendInfo("virtGetErrorCode ", vm.Virtuose.ErrorMessage);
            AppendInfo("virtGetFailure ", vm.Virtuose.Failure);
            AppendInfo("virtGetPowerOn ", vm.Virtuose.Power);
            AppendInfo("virtIsInBounds ", vm.Virtuose.IsInBound + " bit:" + Convert.ToString(vm.Virtuose.IsInBound, 2) +
                " InBound(0)" + vm.Virtuose.IsAxeInBound(0) + " (2)" + vm.Virtuose.IsAxeInBound(2));
            AppendInfo("virtGetAlarm ", vm.Virtuose.Alarm);
            AppendInfo("virtIsInShiftPosition ", vm.Virtuose.IsInShiftPosition);
            AppendInfo("virtGetTimeLastUpdate ", vm.Virtuose.TimeLastUpdate);
            AppendInfo("virtGetNbAxes ", vm.Virtuose.AxesNumber);
            if (vm.Virtuose.IsScaleOne)
            {
                AppendInfo("Scale1CarrierPosition ", vm.Virtuose.Scale1CarrierPosition);
                AppendInfo("Base ", vm.Virtuose.PhysicalBasePose.position + Offset);
                AppendInfo("Bubble ", vm.Virtuose.BubblePosition() + Offset);
            }
            AppendInfo("RelativePhysicalPosition ", vm.Virtuose.LocalPhysicalPosition);
            AppendColoredPosition("ArticularsPosition ", vm.Virtuose.ArticularsPosition, articularPositionsColor);
            AppendColoredArray("virtGetArticularPositions ", vm.Virtuose.Articulars, articularPositionsColor);
            AppendColoredArray("virtGetArticularSpeed ", vm.Virtuose.ArticularsSpeed, articularPositionsColor);

            AppendColoredPose("virtGetPositions ", vm.Virtuose.Pose, positionColor);
            AppendColoredArray("virtGetSpeed ", vm.Virtuose.Speed, positionColor);

            AppendColoredPose("virtGetPhysicalPositions ", vm.Virtuose.PhysicalPose, physicalPositionColor);
            AppendColoredPose("virtGetPhysicalOffsetPositions ", vm.Virtuose.ComputePhysicalPose(Offset), physicalPositionColor);
            AppendColoredPose("virtGetAvatarsPositions ", vm.Virtuose.AvatarPose, avatarPositionColor);
            AppendColoredPose("virtGetBaseFrame ", vm.Virtuose.BaseFrame, baseFramePositionColor);
            AppendColoredPose("virtGetObservationFrame ", vm.Virtuose.ObservationFrame, observationFramePositionColor);

            debugText.text = debugInfo.ToString();
        }
    }

    void AppendColoredPoses(string name, ValueTuple<Vector3, Quaternion>[] poses, Color color)
    {
        AppendBeginColor(color);
        debugInfo.Append(name);
        int p = 0;
        foreach ((Vector3, Quaternion) pose in poses)
        {
            if(pose.Item1 != Vector3.zero && (pose.Item2 != Quaternion.identity || pose.Item2.eulerAngles != Vector3.zero))
                AppendPose(pose, p++);
        }
        AppendEndColor();
    }

    void AppendPose(string name, ValueTuple<Vector3, Quaternion> pose)
    {
        debugInfo.Append(name);
        AppendPose(pose);
    }

    void AppendColoredPosition(string name, Vector3 position, Color color)
    {
        AppendBeginColor(color);
        debugInfo.Append(name);
        AppendPosition(position);
        AppendEndColor();
        debugInfo.AppendLine();
    }

    void AppendColoredPose(string name, ValueTuple<Vector3, Quaternion> pose, Color color)
    {
        AppendBeginColor(color);
        debugInfo.Append(name);
        AppendPose(pose);
        AppendEndColor();
    }

    void AppendBeginColor(Color color)
    {
        debugInfo.
            Append("<color=#").
            Append(ColorUtility.ToHtmlStringRGBA(color)).
            Append(">");
    }

    void AppendEndColor()
    {
        debugInfo.Append("</color>");
    }

    void AppendPosition(Vector3 position, string descriptor = "F3")
    {
        debugInfo.
            Append(position.ToString(descriptor));
    }

    void AppendPose(ValueTuple<Vector3, Quaternion> pose, string descriptor = "F3")
    {
        debugInfo.
            Append(pose.Item1.ToString(descriptor)).
            AppendLine(pose.Item2.ToString(descriptor));
    }

    void AppendPose(ValueTuple<Vector3, Quaternion> pose, int index, string descriptor = "F3")
    {
        debugInfo.
            Append(index).
            Append(pose.Item1.ToString(descriptor)).
            AppendLine(pose.Item2.ToString(descriptor));
    }

    void AppendColoredArray<T>(string name, T[] array, Color color)
    {
        AppendBeginColor(color);
        debugInfo.
            Append(name).
            Append(array.ToFlattenString());
        AppendEndColor();
        debugInfo.AppendLine();
    }

    void AppendInfo<T>(string text, T data)
    {
        debugInfo.
            Append(text).
            Append(data).
            AppendLine();
    }

    void AppendInfo(string text, Vector3 data)
    {
        debugInfo.
            Append(text).
            Append(data.ToString("F3")).
            AppendLine();
    }

    void AppendInfo(string text, Vector2 data)
    {
        debugInfo.
            Append(text).
            Append(data.ToString("F3")).
            AppendLine();
    }

}
