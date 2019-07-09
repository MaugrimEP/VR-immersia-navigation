using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    public List<GameObject> TornadoElements;

    private List<ParticleSystem> _tordnadoPSElements;

    private void Start()
    {
        _tordnadoPSElements = new List<ParticleSystem>();
        foreach(GameObject go in TornadoElements)
            _tordnadoPSElements.Add(go.GetComponent<ParticleSystem>());
        StopParticles();
    }

    public void PlayParticles()
    {
        foreach (ParticleSystem ps in _tordnadoPSElements)
        {
            ps.Play();
        }
    }

    public void StopParticles()
    {
        foreach(ParticleSystem ps in _tordnadoPSElements)
        {
            ps.Stop();
        }
    }

}
