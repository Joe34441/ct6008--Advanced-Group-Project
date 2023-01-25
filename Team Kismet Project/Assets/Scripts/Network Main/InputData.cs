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

	JUMP = 1 << 4,

	LMB = 1 << 5,
	RMB = 1 << 6,

	NUM1 = 1 << 7,
	NUM2 = 1 << 8,
	NUM3 = 1 << 9,
}

public struct InputData : INetworkInput
{
	public ButtonFlag ButtonFlags;

	public bool GetButton(ButtonFlag button)
	{
		return (ButtonFlags & button) == button;
	}
}