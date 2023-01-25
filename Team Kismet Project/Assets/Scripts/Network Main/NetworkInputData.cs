using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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

    //public bool GetButton(InputButton button)
    //{
    //    return Buttons.IsSet(button);
    //}

    //public NetworkButtons GetButtonPressed(NetworkButtons prev)
    //{
    //    return Buttons.GetPressed(prev);
    //}

    //public bool AxisPressed()
    //{
    //    return GetButton(InputButton.LEFT) || GetButton(InputButton.RIGHT);
    //}
}
