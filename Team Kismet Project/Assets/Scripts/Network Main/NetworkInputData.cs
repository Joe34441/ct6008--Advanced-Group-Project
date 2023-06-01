using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

//this is the old version, see InputData.cs

[System.Flags]
public enum OldInputButton
{
    LEFT = 1 << 0,
    RIGHT = 1 << 1,
    RESPAWN = 1 << 3,
    JUMP = 1 << 4,
}

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON1 = 0x01;
    public const byte MOUSEBUTTON2 = 0x02;

    public byte buttons;

    public Vector3 direction;
}
