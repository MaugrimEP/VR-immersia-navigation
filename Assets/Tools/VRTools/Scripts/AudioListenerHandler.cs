using UnityEngine;
using System.Collections;

public class AudioListenerHandler : MonoBehaviour
{

#if MIDDLEVR

    void Awake()
    {
        VRTools.GetInstance(instance =>
        {
            if (!VRTools.IsMaster())
            {
                AudioListener.volume = 0;
                Debug.Log("[MVRTools] Set global AudioListener volume to 0", this);
            }

            // Disable all audio listeners
            foreach (AudioListener audioListener in FindObjectsOfType<AudioListener>())
            {
                audioListener.enabled = false;
                Debug.Log("[MVRTools] Disabling AudioListener component on game object : " + audioListener.name, this);
            }

            // Re-enable or add audio listener on head node
            GameObject  headNode = GameObject.Find("HeadNode");
            if (headNode != null)
            {
                AudioListener headNodeAudioListener = headNode.GetComponent<AudioListener>();
                
                if (headNodeAudioListener == null)
                {
                    headNodeAudioListener = headNode.AddComponent<AudioListener>();
                    Debug.Log("[MVRTools] Adding AudioListener component to HeadNode", this);
                }
                headNodeAudioListener.enabled = true;
                Debug.Log("[MVRTools] Activating AudioListener component on HeadNode", this);
            }
            
        });
    }

#endif
}
