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
    //---------- Value ----------
    [Header("Eye L")]
    [Range(0, 1)] public float blinkNormalL;
    [Range(0, 1)] public float blinkLaughL;
    [Header("Eye R")]
    [Range(0, 1)] public float blinkNormalR;
    [Range(0, 1)] public float blinkLaughR;
    [Header("Mouth")]
    [Range(0, 1)] public float mouthOpenNormal;
    [Range(0, 1)] public float mouthOpenLaugh;
    [Range(0, 1)] public float mouthOpenCry;

    //---------- Getter ----------
    Dictionary<string, float> _valueEye_L = new Dictionary<string, float>();
    Dictionary<string, float> _valueEye_R = new Dictionary<string, float>();
    Dictionary<string, float> _valueMouth = new Dictionary<string, float>();

    protected override Dictionary<string, float> valueGetterEye_L
    {
        get
        {
            _valueEye_L["眨眼-正常"] = blinkNormalL;
            _valueEye_L["眨眼-笑"] = blinkLaughL;
            return _valueEye_L;
        }
    }

    protected override Dictionary<string, float> valueGetterEye_R
    {
        get
        {
            _valueEye_R["眨眼-正常"] = blinkNormalR;
            _valueEye_R["眨眼-笑"] = blinkLaughR;
            return _valueEye_R;
        }
    }

    protected override Dictionary<string, float> valueGetterMouth
    {
        get
        {
            _valueMouth["张嘴-正常"] = mouthOpenNormal;
            _valueMouth["张嘴-笑"] = mouthOpenLaugh;
            _valueMouth["张嘴-哭"] = mouthOpenCry;
            return _valueMouth;
        }
    }

    private void Start()
    {
        if (faceData && matEye_L && matEye_R && matMouth)
            FaceManager.Instance.AddFace(new Face(matEye_L, matEye_R, matMouth, faceData,
                                         () => { return valueGetterEye_L; },
                                         () => { return valueGetterEye_R; },
                                         () => { return valueGetterMouth; }),
                                         name);
    }

    public override void OnValidate()
    {
        Start();
        FaceManager.Instance.LateUpdate();
    }

}