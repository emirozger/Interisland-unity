using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNightCycleManager : MonoBehaviour
{
   // [SerializeField] private Image blackImage;
    [SerializeField] private float timeMultiplier = 1f;
    [SerializeField] private float startHour = 8f;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Light sunLight;
    [SerializeField] private float sunriseHour = 6f;
    [SerializeField] private float sunsetHour = 18f;
    [SerializeField] private Color dayAmbientLight;
    [SerializeField] private Color nightAmbientLight;
    [SerializeField] private AnimationCurve lightChangeCurve;
    [SerializeField] private float maxSunLightIntensity = 1f;
    [SerializeField] private Light moonLight;
    [SerializeField] private float maxMoonLightIntensity = 1f;

    private DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;
    
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

   
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        //UpdateBlackImage();
    }

    private void UpdateTimeOfDay()
    {
        float deltaTime = Time.deltaTime;
        if (currentTime.TimeOfDay > sunsetTime || currentTime.TimeOfDay < sunriseTime)
        {
            deltaTime *= timeMultiplier * 5f;
        }
        else
        {
            deltaTime *= timeMultiplier;
        }

        currentTime = currentTime.AddSeconds(deltaTime);

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun()
    {
        float sunLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);
            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);
            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;
            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientIntensity = Mathf.Lerp(nightAmbientLight.maxColorComponent, dayAmbientLight.maxColorComponent, lightChangeCurve.Evaluate(dotProduct));
    }
/*
    private void UpdateBlackImage()
    {
        if (currentTime.TimeOfDay >= sunsetTime || currentTime.TimeOfDay <= sunriseTime)
        {
            blackImage.gameObject.SetActive(true);
        }
        else
        {
            blackImage.gameObject.SetActive(false);
        }
    }
*/
    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
}
