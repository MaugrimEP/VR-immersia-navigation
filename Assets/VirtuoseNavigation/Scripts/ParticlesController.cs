using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    public ParticleSystem DustPS;
    public TornadoController tornadoController;

    public HumanController humanController;
    public float particleTresholdSpeed = 0.2f;

    private static int id;
    private vrCommand VRControleParticlesDirt;
    private vrCommand VRControleParticlesTornado;

    private HumanController.State previousState;

    private void Start()
    {
        id++;
        VRControleParticlesDirt = new vrCommand($"ParticlesController_{name}_{id}", UpdateParticlesDirt);
        VRControleParticlesTornado = new vrCommand($"ParticlesController_{name}_{id}", UpdateParticlesTornado);

        previousState = HumanController.State.Walking;
    }

    private void Update()
    {
        if (previousState != humanController.state)
            if (humanController.state == HumanController.State.Walking)
            {
                VRControleParticlesDirt.Do(humanController.GetSpeed());
            }
            else
            {
                VRControleParticlesTornado.Do();
            }
        previousState = humanController.state;
    }

    [VRCommand]
    private vrValue UpdateParticlesDirt(vrValue VRSpeed)
    {
        float realSpeed = VRSpeed.GetFloat();
        float speed = Mathf.Abs(realSpeed);

        tornadoController.StopParticles();

        if (particleTresholdSpeed < speed)
            DustPS.Play();
        else
            DustPS.Pause();

        return null;
    }

    [VRCommand]
    private vrValue UpdateParticlesTornado(vrValue _)
    {
        tornadoController.PlayParticles();
        DustPS.Pause();
        return null;
    }
}
