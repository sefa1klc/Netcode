using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider _slider;
    private RectTransform rectTransform;

    [Header("Bar Options")]
    [SerializeField] protected bool _scaleBarLenghtWithStats = true;
    [SerializeField] protected float _widthScaleMultipier;


    protected virtual void Awake()
    {
        _slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void SetStat(int newValue)
    {
        _slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        _slider.maxValue = maxValue;
        _slider.value = maxValue;

        if (_scaleBarLenghtWithStats)
        {
            rectTransform.sizeDelta = new Vector2(maxValue * _widthScaleMultipier, rectTransform.sizeDelta.y);
            PlayerUIManager.Instance.HudManager.RefreshHUD();
                
        }
    }
}
