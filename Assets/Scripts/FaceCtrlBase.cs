using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FaceCtrlBase : MonoBehaviour
{
    protected abstract Dictionary<string, float> valueGetterEye_L { get; }
    protected abstract Dictionary<string, float> valueGetterEye_R { get; }
    protected abstract Dictionary<string, float> valueGetterMouth { get; }
    public abstract void OnValidate();
}