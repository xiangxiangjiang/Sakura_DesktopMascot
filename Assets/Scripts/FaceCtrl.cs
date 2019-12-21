using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;


public class FaceCtrl : MonoBehaviour
{

    [InlineProperty(LabelWidth = 40)]
    public struct PreExpression
    {
        [Range(0, 1)] public float value;
    }

    [OnValueChanged("LoadFaceData", true)]
    public FaceData faceData;


    [ShowInInspector, OnValueChanged("OnExpressionChange", true)]
    public Dictionary<string, PreExpression> eye_L = new Dictionary<string, PreExpression>();
    [ShowInInspector, OnValueChanged("OnExpressionChange", true)]
    public Dictionary<string, PreExpression> eye_R = new Dictionary<string, PreExpression>();
    [ShowInInspector, OnValueChanged("OnExpressionChange", true)]
    public Dictionary<string, PreExpression> mouth = new Dictionary<string, PreExpression>();

    bool _needUpdate = false;


    void Start()
    {
        LoadFaceData();

        print(FaceManager.Instance.name);
    }


    void FixedUpdate()
    {

        if (!_needUpdate)
            return;
        _needUpdate = false;
        // Debug.Log("刷新表情");
    }

    void OnValidate()
    {
        FixedUpdate();
        if (eye_L.Count == 0 ||
            eye_R.Count == 0 ||
            mouth.Count == 0)
        {
            LoadFaceData();
        }
    }

    public void LoadFaceData()
    {
        if (faceData == null)
            return;
        eye_L.Clear();
        eye_R.Clear();
        mouth.Clear();
        foreach (var item in faceData.eye_L)
        {
            eye_L.Add(item.Key, new PreExpression());
        }
        foreach (var item in faceData.eye_R)
        {
            eye_R.Add(item.Key, new PreExpression());
        }
        foreach (var item in faceData.mouth)
        {
            mouth.Add(item.Key, new PreExpression());
        }
    }

    public void OnExpressionChange()
    {
        _needUpdate = true;
    }

}
