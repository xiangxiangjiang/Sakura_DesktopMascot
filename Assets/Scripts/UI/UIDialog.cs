using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class UIDialog : MonoBehaviour
{
    static UIDialog _instance;
    public static UIDialog Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIDialog>();
            }
            return _instance;
        }
    }

    [SerializeField]
    Image _dialog;
    [SerializeField]
    Text _dialogText;
    [SerializeField]
    float _dialogOpenSpeed = 0.5f;

    UnityAction _diaCallback;

    public void ShowDialog(string str, float duration, UnityAction callback = null)
    {
        CancelInvoke("ClossDialog");
        _dialog.transform.DOPause();
        _dialogText.text = str;
        _dialog.gameObject.SetActive(true);
        _dialog.transform.localScale = new Vector3(1, 0, 1);
        _dialog.transform.DOScaleY(1, _dialogOpenSpeed).onComplete = () =>
        {
            _diaCallback = callback;
            Invoke("ClossDialog", duration);
        };
    }

    void ClossDialog()
    {
        if (_diaCallback != null)
            _diaCallback();
        _diaCallback = null;
        _dialog.transform.DOScaleY(0, _dialogOpenSpeed).onComplete = () =>
        {
            _dialog.gameObject.SetActive(false);
        };
    }
}
