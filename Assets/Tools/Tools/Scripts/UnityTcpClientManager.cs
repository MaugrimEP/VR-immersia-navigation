using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityTcpClientManager : UnitySingleton<UnityTcpClientManager> {

    protected List<UnityTcpClient> unityTcpClients = new List<UnityTcpClient>();

	// Use this for initialization
	void Start () {
		
	}

    public UnityTcpClient CreateClient(string hostName, int port, Action<string> handleIncomingData = null)
    {
        UnityTcpClient client = new UnityTcpClient(hostName, port, handleIncomingData);
        return client;
    }

    // Update is called once per frame
    void Update () {
        foreach (UnityTcpClient client in unityTcpClients)
        {
            client.TryReceiveDataThroughCallback();
        }
    }

    void OnApplicationQuit()
    {
        foreach (UnityTcpClient client in unityTcpClients)
        {
            client.Close();
        }
        
    }

    internal void Register(UnityTcpClient unityTcpClient)
    {
        if (!unityTcpClients.Contains(unityTcpClient))
            unityTcpClients.Add(unityTcpClient);
    }
}
