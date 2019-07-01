#if TEST

using UnityEngine;
using System.Collections;

using UnityEngine.TestTools;
using NUnit.Framework;

public class VRToolsUnityTest : IPrebuildSetup
{
    [SetUp]
    public void Setup()
    {
        VRTools.Mode = VRToolsMode.UNITY;
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

        Assert.Throws<System.ArgumentException>(delegate { VRTools.IsButtonPressed(7); });
    }


    [Test]
    public void IsButtonToggled([NUnit.Framework.Range(0, 6)] int b)
    {
        Assert.False(VRTools.IsButtonToggled((uint)b));
        Assert.False(VRTools.IsButtonToggled((uint)b, 1));

        Assert.Throws<System.ArgumentException>(delegate { VRTools.IsButtonToggled(7); });
        Assert.Throws<System.ArgumentException>(delegate { VRTools.IsButtonToggled(7, 1); });
    }

    [Test]
    public void GetDeltaTime()
    {
        Debug.Log(VRTools.GetMode());
        Assert.Greater(VRTools.GetDeltaTime(), 0);
        Assert.AreEqual(UnityEngine.Time.deltaTime, VRTools.GetDeltaTime());
    }

    [Test]
    public void GetFrameCount()
    {
        Assert.Greater(VRTools.GetFrameCount(), 0);
        Assert.AreEqual(UnityEngine.Time.frameCount, VRTools.GetFrameCount());
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
        Assert.AreEqual(VRToolsMode.UNITY.ToString(), VRTools.GetMode());
    }

    [Test]
    public void GetTime()
    {
        Assert.Greater(VRTools.GetTime(), 0);
        Assert.AreEqual(Time.time, VRTools.GetTime());
    }

    [Test]
    public void GetTrackerPosition()
    {
        Assert.AreEqual(Vector3.zero, VRTools.GetTrackerPosition("HeadNode"));
        Assert.AreEqual(Vector3.zero, VRTools.GetTrackerPosition("NotExistingNode"));
    }

    [Test]
    public void GetTrackerRotation()
    {
        Assert.AreEqual(Quaternion.identity, VRTools.GetTrackerRotation("HeadNode"));
        Assert.AreEqual(Quaternion.identity, VRTools.GetTrackerRotation("NotExistingNode"));
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
        float time = Time.time;
        yield return VRTools.Instance.StartCoroutine(VRTools.WaitForSeconds(w / 10));
        Assert.GreaterOrEqual(Time.time, time + w / 10);
    }
}

#endif