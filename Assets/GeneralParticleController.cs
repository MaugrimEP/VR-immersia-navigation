using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralParticleController : MonoBehaviour
{
    public List<GameObject> ParticlesElements;

    private List<ParticleSystem> _particleSystemElement;

    private void Start()
    {
        _particleSystemElement = new List<ParticleSystem>();
        foreach (GameObject go in ParticlesElements)
            _particleSystemElement.Add(go.GetComponent<ParticleSystem>());
        StopParticles();
    }

    public void PlayParticles()
    {
        foreach (ParticleSystem ps in _particleSystemElement)
        {
            ps.Play();
        }
    }

    public void StopParticles()
    {
        foreach (ParticleSystem ps in _particleSystemElement)
        {
            ps.Stop();
        }
    }

}
