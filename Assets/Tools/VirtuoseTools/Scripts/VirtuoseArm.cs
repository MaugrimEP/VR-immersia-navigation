using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VirtuoseArm
{
    /// <summary>
    /// Ipv4#port -> 192.168.1.1#5125
    /// </summary>
    public string Ip;
    public bool IsConnected { get; set; }
    public bool HasError { get; set; }
    //public int Index {get; set;}
    public IntPtr Context { get;set; }

    public override string ToString()
    {
        return "Name(" +Ip + ") Co(" + IsConnected + ")Err(" + HasError + ")";
    }
}
