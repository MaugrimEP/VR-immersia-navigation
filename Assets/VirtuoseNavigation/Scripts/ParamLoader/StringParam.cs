using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringParam : Param<string>
{
    public StringParam(string _paramName, string _defaultValue) : base(_paramName, _defaultValue)
    {
    }

    public StringParam(string _paramName, string _defaultValue, char _separator) : base(_paramName, _defaultValue, _separator)
    {
    }

    protected override string fromString(string value)
    {
        return value;
    }
}
