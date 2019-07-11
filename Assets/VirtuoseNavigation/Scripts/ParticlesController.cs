using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    public ParticleSystem DustPS;
    public TornadoController tornadoController;
    public GeneralParticleController HandLeftExplosions;
    public GeneralParticleController HandRightExplosions;

    public HumanController humanController;
    public float particleTresholdSpeed = 0.2f;

    private HumanController.State previousState;

    private void Start()
    {
        MVRTools.RegisterCommands(this);
        previousState = HumanController.State.Default;
    }

    private void Update()
    {
        if (humanController.state == HumanController.State.Flying && previousState != HumanController.State.Flying)
        {
            MVRTools.GetCommand("UpdateParticlesTornado").Do();
        }
        if (humanController.state == HumanController.State.Walking)
        {
            MVRTools.GetCommand("UpdateParticlesDirt").Do(humanController.GetSpeed());
        }
        previousState = humanController.state;
    }

    [VRCommand]
    private void UpdateParticlesDirt(vrValue VRSpeed)
    {
        float realSpeed = VRSpeed.GetFloat();
        float speed = Mathf.Abs(realSpeed);

        tornadoController.StopParticles();
        HandLeftExplosions.StopParticles();
        HandRightExplosions.StopParticles();

        DustPS.enableEmission = particleTresholdSpeed < speed;
    }

    [VRCommand]
    private void UpdateParticlesTornado()
    {
        tornadoController.PlayParticles();
        HandLeftExplosions.PlayParticles();
        HandRightExplosions.PlayParticles();

        DustPS.enableEmission = false;
    }
}
