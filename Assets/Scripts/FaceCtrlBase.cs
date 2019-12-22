using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FaceCtrlBase : MonoBehaviour
{
    protected abstract Dictionary<string, float> valueGetterEye_L { get; }
    protected abstract Dictionary<string, float> valueGetterEye_R { get; }
    protected abstract Dictionary<string, float> valueGetterMouth { get; }
    protected abstract void Start();
    protected abstract void Update();
    public void OnValidate()
    {
        Start();
        Update();
        FaceManager.Instance.LateUpdate();
    }


    /// <summary>
    /// 混合表情
    /// </summary>
    /// <param name="expn1">表情1</param>
    /// <param name="expn2">表情2</param>
    /// <param name="value">最终值</param>
    /// <param name="blend">混合值</param>
    protected void Switch(ref float expn1, ref float expn2, float value, float blend)
    {
        expn1 = blend < 0.5f ? value : 0;
        expn2 = blend >= 0.5f ? value : 0;
    }
    protected void Switch(ref float expn1, ref float expn2, ref float expn3, float value, float blend)
    {
        expn1 = blend < 0.33f ? value : 0;
        expn2 = blend >= 0.33f && blend < 0.66f ? value : 0;
        expn3 = blend >= 0.66f ? value : 0;
    }
    protected void Switch(ref float expn1, ref float expn2, ref float expn3, ref float expn4, float value, float blend)
    {
        expn1 = blend < 0.25f ? value : 0;
        expn2 = blend >= 0.25f && blend < 0.5f ? value : 0;
        expn3 = blend >= 0.5f && blend < 0.75f ? value : 0;
        expn4 = blend >= 0.75f ? value : 0;
    }
    protected void Switch(ref float expn1, ref float expn2, ref float expn3, ref float expn4, ref float expn5, float value, float blend)
    {
        expn1 = blend < 0.2f ? value : 0;
        expn2 = blend >= 0.2f && blend < 0.4f ? value : 0;
        expn3 = blend >= 0.4f && blend < 0.6f ? value : 0;
        expn4 = blend >= 0.6f && blend < 0.8f ? value : 0;
        expn5 = blend >= 0.8f ? value : 0;
    }
}