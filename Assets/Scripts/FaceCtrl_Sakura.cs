using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCtrl_Sakura : FaceCtrlBase
{
    //---------- Data --------------
    public FaceData faceData;

    //---------- Material ----------
    public Material matEye_L;
    public Material matEye_R;
    public Material matMouth;

    //------- Controller --------
    [Header("Controller")]
    public bool enableCtrl = true;
    [Range(0.01f, 1)] public float mouthOpenCtrl;
    [Range(-1, 1)] public float mouthLaughSadCtrl;
    [Range(0.01f, 1)] public float eyeCloseCtrl;
    [Range(-0.25f, 0.75f)] public float eyeLaughSadCtrl;
    [Range(-0.5f, 0.5f)] public float eyeLRCtrl;

    //---------- Value ----------
    [Header("Eye L")]
    [Range(0, 1)] public float blinkSad2L;
    [Range(0, 1)] public float blinkSad2L_L;
    [Range(0, 1)] public float blinkSad1L;
    [Range(0, 1)] public float blinkSad1L_L;
    [Range(0, 1)] public float blinkNormalL;
    [Range(0, 1)] public float blinkNormalL_L;
    [Range(0, 1)] public float blinkNormalL_R;
    [Range(0, 1)] public float blinkLaughL;
    [Range(0, 1)] public float L2R_NormalL;
    [Range(0, 1)] public float L2R_Sad1L;
    [Range(0, 1)] public float L2R_Sad2L;
    [Header("Eye R")]
    [Range(0, 1)] public float blinkSad2R;
    [Range(0, 1)] public float blinkSad2R_L;
    [Range(0, 1)] public float blinkSad1R;
    [Range(0, 1)] public float blinkSad1R_L;
    [Range(0, 1)] public float blinkNormalR;
    [Range(0, 1)] public float blinkNormalR_L;
    [Range(0, 1)] public float blinkNormalR_R;
    [Range(0, 1)] public float blinkLaughR;
    [Range(0, 1)] public float L2R_NormalR;
    [Range(0, 1)] public float L2R_Sad1R;
    [Range(0, 1)] public float L2R_Sad2R;
    [Header("Mouth")]
    [Range(0, 1)] public float mouthOpenLaugh2;
    [Range(0, 1)] public float mouthOpenLaugh1;
    [Range(0, 1)] public float mouthOpenNormal;
    [Range(0, 1)] public float mouthOpenSad1;
    [Range(0, 1)] public float mouthOpenSad2;

    //---------- Getter ----------
    Dictionary<string, float> _valueEye_L = new Dictionary<string, float>();
    Dictionary<string, float> _valueEye_R = new Dictionary<string, float>();
    Dictionary<string, float> _valueMouth = new Dictionary<string, float>();

    protected override Dictionary<string, float> valueGetterEye_L
    {
        get
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
            return _valueEye_L;
        }
    }

    protected override Dictionary<string, float> valueGetterEye_R
    {
        get
        {
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
            return _valueEye_R;
        }
    }

    protected override Dictionary<string, float> valueGetterMouth
    {
        get
        {
            _valueMouth["张嘴-正常"] = mouthOpenNormal;
            _valueMouth["张嘴-笑1"] = mouthOpenLaugh1;
            _valueMouth["张嘴-笑2"] = mouthOpenLaugh2;
            _valueMouth["张嘴-哭1"] = mouthOpenSad1;
            _valueMouth["张嘴-哭2"] = mouthOpenSad2;
            return _valueMouth;
        }
    }

    protected override void Start()
    {
        if (faceData && matEye_L && matEye_R && matMouth)
            FaceManager.Instance.AddFace(new Face(matEye_L, matEye_R, matMouth, faceData,
                                         () => { return valueGetterEye_L; },
                                         () => { return valueGetterEye_R; },
                                         () => { return valueGetterMouth; }),
                                         name);
    }

    // CTRL
    protected override void Update()
    {
        if (!enableCtrl) return;
        // mouth
        Switch(ref mouthOpenLaugh2, ref mouthOpenLaugh1, ref mouthOpenNormal, ref mouthOpenSad1, ref mouthOpenSad2,
         mouthOpenCtrl, mouthLaughSadCtrl * 0.5f + 0.5f);
        // eye blink
        if (eyeLRCtrl == 0)
        {
            Switch(ref blinkLaughL, ref blinkNormalL, ref blinkSad1L, ref blinkSad2L, eyeCloseCtrl, eyeLaughSadCtrl + 0.25f);
            Switch(ref blinkLaughR, ref blinkNormalR, ref blinkSad1R, ref blinkSad2R, eyeCloseCtrl, eyeLaughSadCtrl + 0.25f);
        }
        else if(eyeCloseCtrl == 0)// LR
        {

        }

    }
}