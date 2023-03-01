using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Flags]
public enum ButtonFlag
{
	FORWARD = 1 << 0,
	BACKWARD = 1 << 1,
	LEFT = 1 << 2,
	RIGHT = 1 << 3,

	CROUCH = 1 << 4,

	JUMP = 1 << 5,

	LMB = 1 << 6,
	RMB = 1 << 7,

	NUM1 = 1 << 8,
	NUM2 = 1 << 9,
	NUM3 = 1 << 10,
}

public struct InputData : INetworkInput
{
	public ButtonFlag ButtonFlags;

	public Vector2 lookRotation;

	public bool GetButton(ButtonFlag button)
	{
		return (ButtonFlags & button) == button;
	}

	public Vector2 GetLookRotation()
	{
		return lookRotation;
	}

	public void SetLookRotation(Vector2 look)
    {
		lookRotation = look;
    }
}