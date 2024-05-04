using System;
using UnityEngine;
using DG.Tweening;


public class DayAndNightController : MonoBehaviour
{
    public Material skyboxMaterial;
  
    [SerializeField] private float transitionToNightDuration = 10f; 
    [SerializeField] private float transitionToDayDuration = 4f; 
    [SerializeField] private Light directionalLight;

    [SerializeField]private float blend = 0f;
    [SerializeField] private float blend2 = 0f;

    void Awake()
    {
        TransitionTweenToMax(); // go to night
        IntensityMultiplierTweenToMin(); // go to night
        DirectionalLightIntensityTweenToMin(); // go to night
    }

    void TransitionTweenToMax()
    {
        DOTween.To(() => blend, x => blend = x, 1f, transitionToNightDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => TransitionTweenToMin());
    }
    void TransitionTweenToMin()
    {
        DOTween.To(() => blend, x => blend = x, 0f, transitionToDayDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => TransitionTweenToMax());
    }
    /*
    void ExposureTweenToMax()
    {
        DOTween.To(() => blend2, y => blend2 = y, .5f, transitionToDayDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => ExposureTweenToMin());
    }
    void ExposureTweenToMin()
    {
        DOTween.To(() => blend2, y => blend2 = y, 0.01f, transitionToNightDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => ExposureTweenToMax());
    }
    */

    void IntensityMultiplierTweenToMin()
    {
        DOTween.To(() => RenderSettings.ambientIntensity, z => RenderSettings.ambientIntensity = z, 0.0f, transitionToNightDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => IntensityMultiplierTweenToMax());
    }
    void IntensityMultiplierTweenToMax()
    {
        DOTween.To(() => RenderSettings.ambientIntensity, z => RenderSettings.ambientIntensity = z, 1.0f, transitionToDayDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => IntensityMultiplierTweenToMin());
    }
    
    void DirectionalLightIntensityTweenToMin()
    {
        DOTween.To(() => directionalLight.intensity, q => directionalLight.intensity = q, 0.0f, transitionToNightDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => DirectionalLightIntensityTweenToMax());
    }
    void DirectionalLightIntensityTweenToMax()
    {
        DOTween.To(() => directionalLight.intensity, q => directionalLight.intensity = q, 0.7f, transitionToDayDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => DirectionalLightIntensityTweenToMin());
    }
    
    void Update()
    {
        skyboxMaterial.SetFloat("_CubemapTransition", blend);
    }
}