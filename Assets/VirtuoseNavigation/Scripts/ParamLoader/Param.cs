using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Param<T>
{
    public string paramName;
    public T defaultValue;
    public char separator;

    public Param(string _paramName, T _defaultValue){
        paramName = _paramName;
        defaultValue = _defaultValue;
        separator = '=';
    }
    
    public Param(string _paramName, T _defaultValue, char _separator){
        paramName = _paramName;
        defaultValue = _defaultValue;
        separator = _separator;
    }

    protected abstract T fromString(string value);

    public (bool wasHere, T value) fromCommandeLine(){
        string[] arguments = System.Environment.GetCommandLineArgs();
        foreach (string argument in arguments)
            if (argument.Contains(paramName))
                return (true,fromString(argument.Split(separator)[1]));
        return (false,defaultValue);
    }
}
