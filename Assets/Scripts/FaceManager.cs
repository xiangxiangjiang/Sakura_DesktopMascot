using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceManager
{
    FaceManager() { }
    static FaceManager _instance;
    public static FaceManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new FaceManager();
            return _instance;
        }
    }
    Dictionary<string, Face> _faceDic = new Dictionary<string, Face>();
    int frameCount = 0;// 由于单例要在编辑器中运行存在很多问题，这里只处理数据，根据帧数判断是否刷新

    public void UpdateFace(int currentFrame)
    {
        Debug.Log(currentFrame);
        if (currentFrame == frameCount)
            return;
        foreach (var face in _faceDic)
        {
            var valueEyeL = face.Value.valueGetterEye_L();
            var valueEyeR = face.Value.valueGetterEye_R();
            var valueMouth = face.Value.valueGetterMouth();
            SetFace(valueEyeL, face.Value.lastValueEye_L, face.Value.sprites.eye_L, face.Value.matEye_L);
            SetFace(valueEyeR, face.Value.lastValueEye_R, face.Value.sprites.eye_R, face.Value.matEye_R);
            SetFace(valueMouth, face.Value.lastValueMouth, face.Value.sprites.mouth, face.Value.matMouth);
        }
        frameCount = currentFrame;
    }


    void SetFace(Dictionary<string, float> values,
                 Dictionary<string, float> lastValues,
                 Dictionary<string, Sprite[]> spritesData,
                 Material material)
    {
        if (!Equals(values, lastValues))
        {
            string maxKey = "";
            float maxValue = -1;
            foreach (var item in values)
            {
                if (item.Value > maxValue)
                {
                    maxKey = item.Key;
                    maxValue = item.Value;
                }
                lastValues[item.Key] = item.Value;
            }
            if (spritesData.ContainsKey(maxKey))
            {
                var sprites = spritesData[maxKey];
                var sprite = sprites[(int)(Mathf.Clamp01(maxValue) * (sprites.Length - 1))];
                if (sprite)
                {
                    var t = sprite.texture;
                    var rect = sprite.rect;
                    if (t.width == t.height)
                        material.SetTextureScale("_MainTex", new Vector2(0.25f, 0.25f));
                    else
                        material.SetTextureScale("_MainTex", new Vector2(0.25f, 0.5f));
                    material.SetTexture("_MainTex", t);
                    material.SetTextureOffset("_MainTex", new Vector2(rect.x / t.width, rect.y / t.height));
                }
                else
                    Debug.LogError($"Sprite is None, key:{maxKey}");
            }
            else
                Debug.LogError($"Sprite's key:{maxKey} Not Found, Check that ValueGetters's Keys is the same as FaceData!");
        }
    }

    bool Equals(Dictionary<string, float> a, Dictionary<string, float> b)
    {
        if (a.Count != b.Count)
            return false;
        foreach (var item in a)
        {
            if (b.ContainsKey(item.Key))
            {
                if (b[item.Key] != item.Value)
                    return false;
            }
            else
                return false;
        }
        return true;
    }

    public void AddFace(Face face, string gameObjectName)
    {
        _faceDic[gameObjectName] = face;
    }
}
