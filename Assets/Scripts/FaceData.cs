using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Desktop Mascot/Face Data")]
public sealed class FaceData : SerializedScriptableObject
{
    [OnValueChanged("LoadFaceData", true)]
    public Dictionary<string, Sprite[]> eye_L = new Dictionary<string, Sprite[]>();
    [OnValueChanged("LoadFaceData", true)]
    public Dictionary<string, Sprite[]> eye_R = new Dictionary<string, Sprite[]>();
    [OnValueChanged("LoadFaceData", true)]
    public Dictionary<string, Sprite[]> mouth = new Dictionary<string, Sprite[]>();

    public void LoadFaceData()
    {
        foreach (var item in FindObjectsOfType<FaceCtrlBase>())
        {
            item.OnValidate();
        }
    }

}

public delegate T Getter<T>();
public class Face
{
    public Material matEye_L;
    public Material matEye_R;
    public Material matMouth;
    public FaceData sprites;
    public Getter<Dictionary<string, float>> valueGetterEye_L;
    public Getter<Dictionary<string, float>> valueGetterEye_R;
    public Getter<Dictionary<string, float>> valueGetterMouth;
    public Dictionary<string, float> lastValueEye_L;
    public Dictionary<string, float> lastValueEye_R;
    public Dictionary<string, float> lastValueMouth;

    public Face(Material matEye_L, Material matEye_R, Material matMouth, FaceData sprites, Getter<Dictionary<string, float>> valueGetterEye_L, Getter<Dictionary<string, float>> valueGetterEye_R, Getter<Dictionary<string, float>> valueGetterMouth)
    {
        this.matEye_L = matEye_L;
        this.matEye_R = matEye_R;
        this.matMouth = matMouth;
        this.sprites = sprites;
        this.valueGetterEye_L = valueGetterEye_L;
        this.valueGetterEye_R = valueGetterEye_R;
        this.valueGetterMouth = valueGetterMouth;
        this.lastValueEye_L = new Dictionary<string, float>();
        this.lastValueEye_R = new Dictionary<string, float>();
        this.lastValueMouth = new Dictionary<string, float>();
    }
}
