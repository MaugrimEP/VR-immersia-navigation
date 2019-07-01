#if MIDDLEVR && TEST

using UnityEngine;
using System.Collections;

using UnityEngine.TestTools;
using NUnit.Framework;

public class VRToolsMiddleVRTest : IPrebuildSetup
{
    bool init = false;

    /// <summary>
    /// MiddleVR initialisation is not complete, but it's enough for unit test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        if (!init)
        {
            init = true;

            VRManagerScript vms = new VRManagerScript();
            vms.ConfigFile = "Assets/Tools/VRTools/mouse_as_joystick.vrx";
            vms.call("Awake");
        }

        VRTools.Mode = VRToolsMode.MIDDLEVR;
    }

    [Test]
    public void IsCluster()
    {
        Assert.False(VRTools.IsCluster());
    }

	[Test]
	public void IsMaster() 
    {
        Assert.True(VRTools.IsMaster());
	}

    [Test]
    public void IsClient()
    {
        Assert.False(VRTools.IsClient());
    }

    [Test]
    public void IsButtonPressed([NUnit.Framework.Range(0, 6)] int b)
    {
        Assert.False(VRTools.IsButtonPressed((uint) b));
    }


    [Test]
    public void IsButtonToggled([NUnit.Framework.Range(0, 6)] int b)
    {
        Assert.False(VRTools.IsButtonToggled((uint)b));
        Assert.False(VRTools.IsButtonToggled((uint)b, 1));
    }

    [Test]
    public void GetDeltaTime()
    {
        Assert.Greater(VRTools.GetDeltaTime(), 0);
    }

    [Test]
    public void GetFrameCount()
    {
        Assert.AreEqual(VRTools.GetFrameCount(), 0);
    }

    [Test]
    public void GetKeyDown()
    {
        Assert.False(VRTools.GetKeyDown(KeyCode.A));
    }

    [Test]
    public void GetKeyUp()
    {
        Assert.False(VRTools.GetKeyUp(KeyCode.A));
    }

    [Test]
    public void GetKeyPressed()
    {
        Assert.False(VRTools.GetKeyPressed(KeyCode.A));
    }

    [Test]
    public void GetMode()
    {
        Assert.AreEqual(VRToolsMode.MIDDLEVR.ToString(), VRTools.GetMode());
    }

    [Test]
    public void GetTime()
    {
        Assert.Greater(VRTools.GetTime(), 0);
    }

    [Test]
    public void GetTrackerPosition()
    {
        Assert.AreEqual(Vector3.zero, VRTools.GetTrackerPosition("NotExistingNode"));
        Assert.AreEqual(new Vector3(0.1f, 1f, 0.2f), VRTools.GetTrackerPosition("HeadNode"));
    }

    [Test]
    public void GetTrackerRotation()
    {
        Assert.AreEqual(Quaternion.identity, VRTools.GetTrackerRotation("NotExistingNode"));
        Quaternion expected = new Quaternion(0, 0.7071069f, 0, -0.7071068f);
        for (int a = 0; a < 4; a++)
            Assert.AreEqual(expected[a], VRTools.GetTrackerRotation("HeadNode")[a], 0.001f);
    }

    [Test]
    public void GetWandAxisValue()
    {
        Assert.AreEqual(0, VRTools.GetWandAxisValue(0));
        Assert.AreEqual(0, VRTools.GetWandAxisValue(0, 1));
        Assert.AreEqual(0, VRTools.GetWandAxisValue(1));
        Assert.AreEqual(0, VRTools.GetWandAxisValue(1, 1));
    }

    [Test]
    public void GetWandHorizontalValue()
    {
        Assert.AreEqual(0, VRTools.GetWandHorizontalValue(0));
    }

    [Test]
    public void GetWandVerticalValue()
    {
        Assert.AreEqual(0, VRTools.GetWandVerticalValue(0));
    }


	[UnityTest]
    public IEnumerator WaitForSecond([NUnit.Framework.Range(1,5)] float w) 
    {
        float time = VRTools.GetTime();
        yield return VRTools.Instance.StartCoroutine(VRTools.WaitForSeconds(w / 10.0f));
        Assert.Greater(VRTools.GetTime(), time);
    }
}


/// <summary>
/// Allow to call private method for testing purpose.
/// </summary>
static class AccessExtensions
{
	public static object call(this object o, string methodName, params object[] args)
	{
		var method = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (method != null)
			return method.Invoke(o, args);	
		
		return null;
	}
}

#endif
