using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstantValues
{
    #region Physics Layers
    public const int LAYER_GROUND = 1 << 6;
	public const int LAYER_MOVEMENT = 1 << 7;
	public const int LAYER_COMBAT =  1 << 8;
	public const int LAYER_PROJECTILE = 1 << 9;
    #endregion

    #region Animation Parameters
    public const string ANIMATION_SPEED = "Speed";
    public const string ANIMATION_AIM = "Aim";
    #endregion

    #region Game Parameters
    public const int Max_Team_Count = 2;
    #endregion
}