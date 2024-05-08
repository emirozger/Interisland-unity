using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;


public class DayAndNightController : MonoBehaviour
{
    public Material skyboxMaterial;

    [SerializeField] private float transitionToNightDuration = 10f;
    [SerializeField] private float transitionToDayDuration = 4f;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Light[] levelLights;
    [SerializeField] private float blend = 0f;
    [SerializeField] private float blend2 = 0f;
    private float[] lightIntensities;
    private Color[] lightEmissionIntensities;
    bool isDayTime = true;

    [SerializeField] private Material[] emissionMaterials;

    void Awake()
    {
        lightIntensities = new float[levelLights.Length];
        lightEmissionIntensities = new Color[emissionMaterials.Length];

        foreach (var var in emissionMaterials)
        {
            if (var == null)
                return;
            int index = Array.IndexOf(emissionMaterials, var);
            lightEmissionIntensities[index] = var.GetColor("_EmissionColor");
            var.SetColor("_EmissionColor", lightEmissionIntensities[Array.IndexOf(emissionMaterials, var)]);
        }

        foreach (var light in levelLights)
        {
            if (levelLights == null)
                return;
            int index = Array.IndexOf(levelLights, light);
            lightIntensities[index] = light.intensity;
            light.intensity = 0;
        }

        TransitionTweenToMax(); // go to night
        IntensityMultiplierTweenToMin(); // go to night
        DirectionalLightIntensityTweenToMin(); // go to night
        CubemapPosTweenToMax();
        StartCoroutine(ManageLights());
    }

    [ContextMenu("InitialLights")]
    public void InitialLights()
    {
        var lights = FindObjectsOfType<Light>();
        levelLights = new Light[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            levelLights[i] = lights[i];
        }
    }

    IEnumerator ManageLights()
    {
        while (true)
        {
            if (isDayTime)
            {
                foreach (var light in levelLights)
                {
                    int index = Array.IndexOf(levelLights, light);
                    light.DOIntensity(lightIntensities[index], transitionToDayDuration)
                        .SetEase(Ease.InQuint);
                }

                yield return new WaitForSeconds(transitionToDayDuration);
            }
            else
            {
                foreach (var light in levelLights)
                {
                    light.DOIntensity(0f, transitionToNightDuration).SetEase(Ease.InQuint);
                }


                yield return new WaitForSeconds(transitionToNightDuration);
            }

            isDayTime = !isDayTime;
        }
    }

    void CubemapPosTweenToMax()
    {
        DOTween.To(() => blend2, x => blend2 = x, 1f, transitionToNightDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => CubemapPosTweenToMin());
    }

    void CubemapPosTweenToMin()
    {
        DOTween.To(() => blend2, x => blend2 = x, 0f, transitionToDayDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => CubemapPosTweenToMax());
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
        DOTween.To(() => RenderSettings.ambientIntensity, z => RenderSettings.ambientIntensity = z, 0.0f,
                transitionToNightDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => IntensityMultiplierTweenToMax());
    }

    void IntensityMultiplierTweenToMax()
    {
        DOTween.To(() => RenderSettings.ambientIntensity, z => RenderSettings.ambientIntensity = z, 1.0f,
                transitionToDayDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => IntensityMultiplierTweenToMin());
    }

    void DirectionalLightIntensityTweenToMin()
    {
        DOTween.To(() => directionalLight.intensity, q => directionalLight.intensity = q, 0.0f,
                transitionToNightDuration)
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
        skyboxMaterial.SetFloat("_CubemapPosition", blend2);
        foreach (var mat in emissionMaterials)
        {
            mat.SetColor("_EmissionColor", lightEmissionIntensities[Array.IndexOf(emissionMaterials, mat)] * blend);
        }
    }
}