using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    public ParticleSystem ps;
    public HumanController humanController;
    private vrCommand VRControleParticles;
    public float particleTresholdSpeed = 0.2f;

    private static int id;

    private void Start()
    {
        id++;
        VRControleParticles = new vrCommand($"ParticlesController_{name}_{id}", UpdateParticles);
    }

    private void Update()
    {
        VRControleParticles.Do(humanController.GetAbsSpeed());
    }

    [VRCommand]
    private vrValue UpdateParticles(vrValue VRSpeed)
    {
        float realSpeed = VRSpeed.GetFloat();
        float speed = Mathf.Abs(realSpeed);

        if (particleTresholdSpeed < speed)
            ps.Play();
        else
            ps.Pause();

        return null;
    }
}
