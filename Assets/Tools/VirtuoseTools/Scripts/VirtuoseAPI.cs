using System.Runtime.InteropServices;

/// Return Type: void
///param0: VirtContext->VirtContextS*
///param1: void*
public delegate void VirtuoseCallbackFn(System.IntPtr param0, System.IntPtr param1);

public partial class VirtuoseAPI
{
    public enum VirtCommandType
    {
        /* Default command type: no movement possible */
        COMMAND_TYPE_NONE = 0,
        /* Simple joint control */
        COMMAND_TYPE_JOINT,
        /* Simple cartesian control */
        COMMAND_TYPE_CARTESIAN,
        /* Force/position control */
        COMMAND_TYPE_IMPEDANCE,
        /* Position/force control */
        COMMAND_TYPE_ADMITTANCE,
        /* Position/force control with virtual kinematics */
        COMMAND_TYPE_VIRTMECH,
        /* articular position control */
        COMMAND_TYPE_ARTICULAR,
        /* articular force control */
        COMMAND_TYPE_ARTICULAR_IMPEDANCE,
    }
 
    public enum VirtGripperCommandType
    {
        /* Default command type: no movement possible */
        GRIPPER_COMMAND_TYPE_NONE = 0,
        /* Position/position control */
        GRIPPER_COMMAND_TYPE_POSITION,
        /* Force/position control */
        GRIPPER_COMMAND_TYPE_IMPEDANCE,
    }



    public enum VirtIndexingType
    {
        /* Indexing is allowed on translations and rotations */
        INDEXING_ALL = 0,
        /* Indexing is allowed on translations only */
        INDEXING_TRANS = 1,
        /* No indexing allowed, even without deadman  */
        INDEXING_NONE = 2,
        /* Indexing is allowed on translations and rotations, no force while indexing */
        INDEXING_ALL_FORCE_FEEDBACK_INHIBITION = 3,
        /* Indexing is allowed on translations only, no force while indexing */
        INDEXING_TRANS_FORCE_FEEDBACK_INHIBITION = 4,
        /* Indexing is allowed on rotations only, no force while indexing */
        INDEXING_ROT_FORCE_FEEDBACK_INHIBITION = 6,
        /* Indexing is allowed on rotations only */
        INDEXING_ROT = 7,
    }
 
    const string DLLNAME = "VirtuoseAPI";

    /// Return Type: int
    ///major: int*
    ///minor: int*
    [DllImport(DLLNAME, EntryPoint = "virtAPIVersion")]
    public static extern int virtAPIVersion(ref int major, ref int minor);

    [DllImport(DLLNAME, EntryPoint = "virtGetControlerVersion")]
    public static extern int virtGetControlerVersion(System.IntPtr VC, ref int major, ref int minor);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///mass: float
    ///mxmymz: float[]
    [DllImport(DLLNAME, EntryPoint="virtAttachVO")]
    public static extern int virtAttachVO(System.IntPtr VC, float mass, float[] mxmymz);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///Ks: float[]
    ///Bs: float[]
    [DllImport(DLLNAME, EntryPoint="virtAttachQSVO")]
    public static extern int virtAttachQSVO(System.IntPtr VC, float[] Ks, float[] Bs);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///mass: float
    ///mxmymz: float[]
    [DllImport(DLLNAME, EntryPoint="virtAttachVOAvatar")]
    public static extern int virtAttachVOAvatar(System.IntPtr VC, float mass, float[] mxmymz);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtClose")]
    public static extern int virtClose(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtDetachVO")]
    public static extern int virtDetachVO(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtDetachVOAvatar")]
    public static extern int virtDetachVOAvatar(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///avatar: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetAvatarFrame")]
    public static extern int virtGetAvatarFrame(System.IntPtr VC, float[] avatar);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///frame: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetBaseFrame")]
    public static extern int virtGetBaseFrame(System.IntPtr VC, float[] frame);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///button_number: int
    ///state: int*
    [DllImport(DLLNAME, EntryPoint="virtGetButton")]
    public static extern int virtGetButton(System.IntPtr VC, int button_number, ref int state);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///type: int*
    [DllImport(DLLNAME, EntryPoint="virtGetCommandType")]
    public static extern int virtGetCommandType(System.IntPtr VC, ref int type);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///dead_man: int*
    [DllImport(DLLNAME, EntryPoint="virtGetDeadMan")]
    public static extern int virtGetDeadMan(System.IntPtr VC, ref int dead_man);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///emergency_stop: int*
    [DllImport(DLLNAME, EntryPoint="virtGetEmergencyStop")]
    public static extern int virtGetEmergencyStop(System.IntPtr VC, ref int emergency_stop);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///error: int*
    [DllImport(DLLNAME, EntryPoint="virtGetError")]
    public static extern int virtGetError(System.IntPtr VC, ref int error);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtGetErrorCode")]
    public static extern int virtGetErrorCode(System.IntPtr VC);

    
    /// Return Type: char*
    ///code: int
    [DllImport(DLLNAME, EntryPoint = "virtGetErrorMessage")]
    public static extern System.IntPtr virtGetErrorMessage(int code);

    [DllImport(DLLNAME, EntryPoint = "virtGetFailure")]
    public static extern int virtGetFailure(System.IntPtr VC, ref uint code);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///force: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetForce")]
    public static extern int virtGetForce(System.IntPtr VC, float[] force);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///force_factor: float*
    [DllImport(DLLNAME, EntryPoint="virtGetForceFactor")]
    public static extern int virtGetForceFactor(System.IntPtr VC, ref float force_factor);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///torque: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetLimitTorque")]
    public static extern int virtGetLimitTorque(System.IntPtr VC, float[] torque);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///obs: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetObservationFrame")]
    public static extern int virtGetObservationFrame(System.IntPtr VC, float[] obs);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetPosition")]
    public static extern int virtGetPosition(System.IntPtr VC, float[] pos);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint = "virtGetNbAxes")]
    public static extern int virtGetNbAxes(System.IntPtr VC, ref int nbAxes);

    [DllImport(DLLNAME, EntryPoint = "virtGetDeviceID")]
    public static extern int virtGetDeviceID(System.IntPtr VC, ref int deviceId, ref int serialNumber);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint = "virtGetArticularPosition")]
    public static extern int virtGetArticularPosition(System.IntPtr VC, float[] pos);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint = "virtSetArticularPosition")]
    public static extern int virtSetArticularPosition(System.IntPtr VC, float[] pos);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint = "virtGetArticularSpeed")]
    public static extern int virtGetArticularSpeed(System.IntPtr VC, float[] speeds);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint = "virtSetArticularSpeed")]
    public static extern int virtSetArticularSpeed(System.IntPtr VC, float[] speeds);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///power: int*
    [DllImport(DLLNAME, EntryPoint="virtGetPowerOn")]
    public static extern int virtGetPowerOn(System.IntPtr VC, ref int power);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///speed: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetSpeed")]
    public static extern int virtGetSpeed(System.IntPtr VC, float[] speed);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///speed_factor: float*
    [DllImport(DLLNAME, EntryPoint="virtGetSpeedFactor")]
    public static extern int virtGetSpeedFactor(System.IntPtr VC, ref float speed_factor);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///time: unsigned int*
    [DllImport(DLLNAME, EntryPoint="virtGetTimeLastUpdate")]
    public static extern int virtGetTimeLastUpdate(System.IntPtr VC, ref uint time);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///time_value: float*
    [DllImport(DLLNAME, EntryPoint="virtGetTimeoutValue")]
    public static extern int virtGetTimeoutValue(System.IntPtr VC, ref float time_value);

    
    /// Return Type: VirtContext->VirtContextS*
    ///nom: char*
    [DllImport(DLLNAME, EntryPoint="virtOpen")]
    public static extern System.IntPtr virtOpen([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string nom);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///avatar: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetAvatarFrame")]
    public static extern int virtSetAvatarFrame(System.IntPtr VC, float[] avatar);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///frame: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetBaseFrame")]
    public static extern int virtSetBaseFrame(System.IntPtr VC, float[] frame);


    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///type: unsigned short
    [DllImport(DLLNAME, EntryPoint = "virtSetCommandType")]
    public static extern int virtSetCommandType(System.IntPtr VC, ushort type);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///type: unsigned short
    [DllImport(DLLNAME, EntryPoint = "virtSetGripperCommandType")]
    public static extern int virtSetGripperCommandType(System.IntPtr VC, ushort type);


    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///flag: unsigned short
    [DllImport(DLLNAME, EntryPoint="virtSetDebugFlags")]
    public static extern int virtSetDebugFlags(System.IntPtr VC, ushort flag);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///force: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetForce")]
    public static extern int virtSetForce(System.IntPtr VC, float[] force);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///force: float[]
    [DllImport(DLLNAME, EntryPoint = "virtAddForce")]
    public static extern int virtAddForce(System.IntPtr VC, float[] force);


    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///force_factor: float
    [DllImport(DLLNAME, EntryPoint="virtSetForceFactor")]
    public static extern int virtSetForceFactor(System.IntPtr VC, float force_factor);


    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///mode: unsigned short
    [DllImport(DLLNAME, EntryPoint = "virtSetIndexingMode")]
    public static extern int virtSetIndexingMode(System.IntPtr VC, ushort mode);

    [DllImport(DLLNAME, EntryPoint = "virtGetIndexingMode")]
    public static extern int virtGetIndexingMode(System.IntPtr VC, ref ushort mode);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///torque: float
    [DllImport(DLLNAME, EntryPoint="virtSetLimitTorque")]
    public static extern int virtSetLimitTorque(System.IntPtr VC, float torque);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///obs: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetObservationFrame")]
    public static extern int virtSetObservationFrame(System.IntPtr VC, float[] obs);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///speed: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetObservationFrameSpeed")]
    public static extern int virtSetObservationFrameSpeed(System.IntPtr VC, float[] speed);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///name: char*
    [DllImport(DLLNAME, EntryPoint="virtSetOutputFile")]
    public static extern int virtSetOutputFile(System.IntPtr VC, System.IntPtr name);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///fn: VirtuoseCallbackFn
    ///period: float*
    ///arg: void*
    [DllImport(DLLNAME, EntryPoint="virtSetPeriodicFunction")]
    public static extern int virtSetPeriodicFunction(System.IntPtr VC, VirtuoseCallbackFn fn, ref float period, System.IntPtr arg);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint = "virtStartLoop")]
    public static extern int virtStartLoop(System.IntPtr VC);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint = "virtStopLoop")]
    public static extern int virtStopLoop(System.IntPtr VC);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetPosition")]
    public static extern int virtSetPosition(System.IntPtr VC, float[] pos);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///power: int
    [DllImport(DLLNAME, EntryPoint="virtSetPowerOn")]
    public static extern int virtSetPowerOn(System.IntPtr VC, int power);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///speed: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetSpeed")]
    public static extern int virtSetSpeed(System.IntPtr VC, float[] speed);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///speed_factor: float
    [DllImport(DLLNAME, EntryPoint="virtSetSpeedFactor")]
    public static extern int virtSetSpeedFactor(System.IntPtr VC, float speed_factor);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///position: float[]
    ///intensite: float[]
    ///reinit: int
    [DllImport(DLLNAME, EntryPoint="virtSetTexture")]
    public static extern int virtSetTexture(System.IntPtr VC, float[] position, float[] intensite, int reinit);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///texture_force: float[]
    [DllImport(DLLNAME, EntryPoint="virtSetTextureForce")]
    public static extern int virtSetTextureForce(System.IntPtr VC, float[] texture_force);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///step: float
    [DllImport(DLLNAME, EntryPoint="virtSetTimeStep")]
    public static extern int virtSetTimeStep(System.IntPtr VC, float step);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///time_value: float
    [DllImport(DLLNAME, EntryPoint="virtSetTimeoutValue")]
    public static extern int virtSetTimeoutValue(System.IntPtr VC, float time_value);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float*
    [DllImport(DLLNAME, EntryPoint = "virtGetArticularPositionOfAdditionalAxe")]
    public static extern int virtGetArticularPositionOfAdditionalAxe(System.IntPtr VC, ref float pos);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float*
    [DllImport(DLLNAME, EntryPoint = "virtSetArticularPositionOfAdditionalAxe")]
    public static extern int virtSetArticularPositionOfAdditionalAxe(System.IntPtr VC, ref float pos);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float*
    [DllImport(DLLNAME, EntryPoint = "virtGetArticularSpeedOfAdditionalAxe")]
    public static extern int virtGetArticularSpeedOfAdditionalAxe(System.IntPtr VC, ref float pos);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float*
    [DllImport(DLLNAME, EntryPoint = "virtSetArticularSpeedOfAdditionalAxe")]
    public static extern int virtSetArticularSpeedOfAdditionalAxe(System.IntPtr VC, ref float pos);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtWaitForSynch")]
    public static extern int virtWaitForSynch(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtTrajRecordStart")]
    public static extern int virtTrajRecordStart(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtTrajRecordStop")]
    public static extern int virtTrajRecordStop(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///timeStep: float
    ///recordTime: unsigned int*
    [DllImport(DLLNAME, EntryPoint="virtTrajSetSamplingTimeStep")]
    public static extern int virtTrajSetSamplingTimeStep(System.IntPtr VC, float timeStep, ref uint recordTime);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///nbSamples: unsigned int
    [DllImport(DLLNAME, EntryPoint="virtVmStartTrajSampling")]
    public static extern int virtVmStartTrajSampling(System.IntPtr VC, uint nbSamples);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///samples: float[]
    [DllImport(DLLNAME, EntryPoint="virtVmGetTrajSamples")]
    public static extern int virtVmGetTrajSamples(System.IntPtr VC, float[] samples);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtVmActivate")]
    public static extern int virtVmActivate(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtVmDeactivate")]
    public static extern int virtVmDeactivate(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///frame: float[]
    [DllImport(DLLNAME, EntryPoint="virtVmSetBaseFrame")]
    public static extern int virtVmSetBaseFrame(System.IntPtr VC, float[] frame);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///bounds: float[]
    [DllImport(DLLNAME, EntryPoint="virtVmSetMaxArtiBounds")]
    public static extern int virtVmSetMaxArtiBounds(System.IntPtr VC, float[] bounds);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///bounds: float[]
    [DllImport(DLLNAME, EntryPoint="virtVmSetMinArtiBounds")]
    public static extern int virtVmSetMinArtiBounds(System.IntPtr VC, float[] bounds);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetPhysicalPosition")]
    public static extern int virtGetPhysicalPosition(System.IntPtr VC, float[] pos);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///pos: float[]
    [DllImport(DLLNAME, EntryPoint="virtGetAvatarPosition")]
    public static extern int virtGetAvatarPosition(System.IntPtr VC, float[] pos);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///forceThreshold: float
    ///momentThreshold: float
    [DllImport(DLLNAME, EntryPoint="virtSaturateTorque")]
    public static extern int virtSaturateTorque(System.IntPtr VC, float forceThreshold, float momentThreshold);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtVmSetDefaultToTransparentMode")]
    public static extern int virtVmSetDefaultToTransparentMode(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtVmSetDefaultToCartesianPosition")]
    public static extern int virtVmSetDefaultToCartesianPosition(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtVmSetBaseFrameToCurrentFrame")]
    public static extern int virtVmSetBaseFrameToCurrentFrame(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///rgb: float[]
    ///gray: float[]
    [DllImport(DLLNAME, EntryPoint="virtConvertRGBToGrayscale")]
    public static extern int virtConvertRGBToGrayscale(System.IntPtr VC, float[] rgb, float[] gray);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///frame: float[]
    [DllImport(DLLNAME, EntryPoint="virtVmGetBaseFrame")]
    public static extern int virtVmGetBaseFrame(System.IntPtr VC, float[] frame);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///button_number: int
    [DllImport(DLLNAME, EntryPoint="virtWaitPressButton")]
    public static extern int virtWaitPressButton(System.IntPtr VC, int button_number);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///step: float*
    [DllImport(DLLNAME, EntryPoint="virtGetTimeStep")]
    public static extern int virtGetTimeStep(System.IntPtr VC, ref float step);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///OnOff: int
    [DllImport(DLLNAME, EntryPoint="virtVmSetRobotMode")]
    public static extern int virtVmSetRobotMode(System.IntPtr VC, int OnOff);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///file_name: char*
    [DllImport(DLLNAME, EntryPoint="virtVmSaveCurrentSpline")]
    public static extern int virtVmSaveCurrentSpline(System.IntPtr VC, System.IntPtr file_name);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///file_name: char*
    [DllImport(DLLNAME, EntryPoint="virtVmLoadSpline")]
    public static extern int virtVmLoadSpline(System.IntPtr VC, System.IntPtr file_name);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///file_name: char*
    [DllImport(DLLNAME, EntryPoint="virtVmDeleteSpline")]
    public static extern int virtVmDeleteSpline(System.IntPtr VC, System.IntPtr file_name);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    [DllImport(DLLNAME, EntryPoint="virtVmWaitUpperBound")]
    public static extern int virtVmWaitUpperBound(System.IntPtr VC);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///disable: int
    [DllImport(DLLNAME, EntryPoint="virtDisableControlConnexion")]
    public static extern int virtDisableControlConnexion(System.IntPtr VC, int disable);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///bounds: unsigned int[]
    [DllImport(DLLNAME, EntryPoint="virtIsInBounds")]
    public static extern int virtIsInBounds(System.IntPtr VC, ref uint bounds);

    
    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///alarm: unsigned int*
    [DllImport(DLLNAME, EntryPoint="virtGetAlarm")]
    public static extern int virtGetAlarm(System.IntPtr VC, ref uint alarm);

    [DllImport(DLLNAME, EntryPoint = "virtIsInShiftPosition")]
    public static extern int virtIsInShiftPosition(System.IntPtr VC, ref int indexing);

    /// Return Type: int
    ///VC: VirtContext->VirtContextS*
    ///x: int*
    ///y: int*
    [DllImport(DLLNAME, EntryPoint="virtGetTrackball")]
    public static extern int virtGetTrackball(System.IntPtr VC, ref int x, ref int y);

}
