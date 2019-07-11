using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public Color succesColor;
    public List<ParticleSystem> particleSystems;

    private List<Color> startColor;

    private bool isSucced;

    private void Start()
    {
        startColor = new List<Color>();
        foreach (ParticleSystem ps in particleSystems)
            startColor.Add(ps.startColor);
        isSucced = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSucced)
        {
            Succed();
            isSucced = true;
        }
    }

    private void Succed()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            float alpha = ps.startColor.a;
            Color newColor = succesColor;
            newColor.a = alpha;
            ps.startColor = newColor;
        }
    }

    public void ResetColor()
    {
        for(int i = 0; i < particleSystems.Count; ++i)
        {
            ParticleSystem ps = particleSystems[i];
            ps.startColor = startColor[i];
        }
        isSucced = false;
    }
}
