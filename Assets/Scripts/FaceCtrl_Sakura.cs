using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceCtrl_Sakura : FaceCtrlBase
{
    //---------- Data --------------
    public FaceData faceData;
    public AudioSource audioSource;

    //---------- Material ----------
    public Material matEye_L;
    public Material matEye_R;
    public Material matMouth;

    //------- Controller --------
    [Header("Auto Controller")]
    public bool enableAutoBlink = true;
    public AnimationCurve blinkCurve;
    public float minInterval = 4, maxInterval = 8, duration = 0.5f;
    public bool enableAutoMouth = true;
    public float scale = 10;
    public int band = 8;

    [Header("Controller")]
    public bool enableCtrl = true;
    [Range(0.01f, 1)] public float mouthOpenCtrl;
    [Range(-1, 1)] public float mouthLaughSadCtrl;
    [Range(0.01f, 1)] public float eyeCloseCtrl;
    [Range(0, 1)] public float eyeCloseCtrlL;
    [Range(0, 1)] public float eyeCloseCtrlR;
    [Range(-0.25f, 0.75f)] public float eyeLaughSadCtrl;
    [Range(-0.49f, 0.5f)] public float eyeLRCtrl;

    //---------- Value ----------
    // Eye L
    float blinkSad2L, blinkSad2L_L;
    float blinkSad1L, blinkSad1L_L;
    float blinkNormalL, blinkNormalL_L, blinkNormalL_R;
    float blinkLaughL;
    float L2R_NormalL, L2R_Sad1L, L2R_Sad2L;
    // Eye R
    float blinkSad2R, blinkSad2R_L;
    float blinkSad1R, blinkSad1R_L;
    float blinkNormalR, blinkNormalR_L, blinkNormalR_R;
    float blinkLaughR;
    float L2R_NormalR, L2R_Sad1R, L2R_Sad2R;
    // Mouth
    float mouthOpenNormal, mouthOpenLaugh1, mouthOpenLaugh2;
    float mouthOpenSad1, mouthOpenSad2;

    protected override void Start()
    {
        if (faceData && matEye_L && matEye_R && matMouth)
            FaceManager.Instance.AddFace(new Face(matEye_L, matEye_R, matMouth, faceData,
                                         () => { return _valueEye_L; },
                                         () => { return _valueEye_R; },
                                         () => { return _valueMouth; }),
                                         name);
    }

    // CTRL
    protected override void Update()
    {
        if (enableCtrl)
        {
            Init();
            // 自动表情
            if (audioSource)
            {
                if (!audioSource.isPlaying && enableAutoBlink)
                {
                    eyeCloseCtrl = Mathf.Clamp(AutoBlinkCtrl(minInterval, maxInterval, duration, blinkCurve), 0.01f, 1);
                }
                else if (audioSource.isPlaying && enableAutoMouth)
                {
                    mouthOpenCtrl = Mathf.Clamp(AutoMouthCtrl(audioSource, scale, band), 0.01f, 1);
                }
            }
            else
            {
                Debug.LogError("Audio Source is Missing!");
            }
            StateMachine();
        }
        SetValue();
    }

    private void StateMachine()
    {
        // mouth
        Switch(ref mouthOpenLaugh2, ref mouthOpenLaugh1, ref mouthOpenNormal, ref mouthOpenSad1, ref mouthOpenSad2,
         mouthOpenCtrl, mouthLaughSadCtrl * 0.5f + 0.5f);
        // eye blink only
        float eyeCloseL = Mathf.Clamp01(eyeCloseCtrl + eyeCloseCtrlL);
        float eyeCloseR = Mathf.Clamp01(eyeCloseCtrl + eyeCloseCtrlR);
        if (eyeLRCtrl == 0)
        {
            Switch(ref blinkLaughL, ref blinkNormalL, ref blinkSad1L, ref blinkSad2L, eyeCloseL, eyeLaughSadCtrl + 0.25f);
            Switch(ref blinkLaughR, ref blinkNormalR, ref blinkSad1R, ref blinkSad2R, eyeCloseR, eyeLaughSadCtrl + 0.25f);
        }
        else if (eyeCloseCtrl + eyeCloseCtrlL + eyeCloseCtrlR == 0.01f)// LR only
        {
            Switch(ref L2R_NormalL, ref L2R_Sad1L, ref L2R_Sad2L, eyeLRCtrl + 0.5f, Mathf.Abs(eyeLaughSadCtrl) * 1.3f);
            Switch(ref L2R_NormalR, ref L2R_Sad1R, ref L2R_Sad2R, eyeLRCtrl + 0.5f, Mathf.Abs(eyeLaughSadCtrl) * 1.3f);
        }
        else
        {
            if (eyeLaughSadCtrl < 0.25f) // Normal
            {
                Switch(ref blinkNormalL_L, ref blinkNormalL, ref blinkNormalL_R, eyeCloseL, eyeLRCtrl + 0.5f);
                Switch(ref blinkNormalR_L, ref blinkNormalR, ref blinkNormalR_R, eyeCloseR, eyeLRCtrl + 0.5f);
            }
            else if (eyeLaughSadCtrl < 0.5f) // Sad1
            {
                Switch(ref blinkSad1L_L, ref blinkSad1L, eyeCloseL, eyeLRCtrl + 0.5f);
                Switch(ref blinkSad1R_L, ref blinkSad1R, eyeCloseR, eyeLRCtrl + 0.5f);
            }
            else // Sad2
            {
                Switch(ref blinkSad2L_L, ref blinkSad2L, eyeCloseL, eyeLRCtrl + 0.5f);
                Switch(ref blinkSad2R_L, ref blinkSad2R, eyeCloseR, eyeLRCtrl + 0.5f);
            }
        }
    }

    void SetValue()
    {
        _valueEye_L["眨眼-哭2"] = blinkSad2L;
        _valueEye_L["眨眼-哭2-L"] = blinkSad2L_L;
        _valueEye_L["眨眼-哭1"] = blinkSad1L;
        _valueEye_L["眨眼-哭1-L"] = blinkSad1L_L;
        _valueEye_L["眨眼-正常"] = blinkNormalL;
        _valueEye_L["眨眼-正常-L"] = blinkNormalL_L;
        _valueEye_L["眨眼-正常-R"] = blinkNormalL_R;
        _valueEye_L["眨眼-笑"] = blinkLaughL;
        _valueEye_L["L2R_Normal"] = L2R_NormalL;
        _valueEye_L["L2R_Sad1"] = L2R_Sad1L;
        _valueEye_L["L2R_Sad2"] = L2R_Sad2L;

        _valueEye_R["眨眼-哭2"] = blinkSad2R;
        _valueEye_R["眨眼-哭2-L"] = blinkSad2R_L;
        _valueEye_R["眨眼-哭1"] = blinkSad1R;
        _valueEye_R["眨眼-哭1-L"] = blinkSad1R_L;
        _valueEye_R["眨眼-正常"] = blinkNormalR;
        _valueEye_R["眨眼-正常-L"] = blinkNormalR_L;
        _valueEye_R["眨眼-正常-R"] = blinkNormalR_R;
        _valueEye_R["眨眼-笑"] = blinkLaughR;
        _valueEye_R["L2R_Normal"] = L2R_NormalR;
        _valueEye_R["L2R_Sad1"] = L2R_Sad1R;
        _valueEye_R["L2R_Sad2"] = L2R_Sad2R;

        _valueMouth["张嘴-正常"] = mouthOpenNormal;
        _valueMouth["张嘴-笑1"] = mouthOpenLaugh1;
        _valueMouth["张嘴-笑2"] = mouthOpenLaugh2;
        _valueMouth["张嘴-哭1"] = mouthOpenSad1;
        _valueMouth["张嘴-哭2"] = mouthOpenSad2;
    }

    void Init()
    {
        blinkSad2L = 0;
        blinkSad2L_L = 0;
        blinkSad1L = 0;
        blinkSad1L_L = 0;
        blinkNormalL = 0;
        blinkNormalL_L = 0;
        blinkNormalL_R = 0;
        blinkLaughL = 0;
        L2R_NormalL = 0;
        L2R_Sad1L = 0;
        L2R_Sad2L = 0;
        blinkSad2R = 0;
        blinkSad2R_L = 0;
        blinkSad1R = 0;
        blinkSad1R_L = 0;
        blinkNormalR = 0;
        blinkNormalR_L = 0;
        blinkNormalR_R = 0;
        blinkLaughR = 0;
        L2R_NormalR = 0;
        L2R_Sad1R = 0;
        L2R_Sad2R = 0;
        mouthOpenLaugh2 = 0;
        mouthOpenLaugh1 = 0;
        mouthOpenNormal = 0;
        mouthOpenSad1 = 0;
        mouthOpenSad2 = 0;
    }

}