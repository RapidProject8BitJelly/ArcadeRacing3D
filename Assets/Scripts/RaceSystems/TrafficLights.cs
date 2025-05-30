using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLights : MonoBehaviour
{
    [SerializeField] private Image[] lights;
    [SerializeField] private Color[] lightColors;
    [SerializeField] private TMP_Text countdownText;
    
    private Color defaultColor;

    private void Awake()
    {
        defaultColor = lights[0].color;
    }

    private void OnEnable()
    {
        TrafficLightsEvents.BeginCountdown += BeginCountdown;
    }

    private void OnDisable()
    {
        TrafficLightsEvents.BeginCountdown -= BeginCountdown;
    }
    
    private IEnumerator StartCountDown()
    {
        for (int i = 2; i >= 0; i--)
        {
            lights[lightColors.Length - 1 - i].color = lightColors[lightColors.Length - 1- i];
            countdownText.text = (i+1).ToString();
            yield return new WaitForSeconds(1f);
        }

        for (int i = 0; i < 3; i++)
        {
            lights[i].color = defaultColor;
        }
        countdownText.text = "Start!";
        MatchController.EnablePlayersCars();
        
        yield return new WaitForSeconds(1f);
        countdownText.text = "";
        gameObject.SetActive(false);
    }

    private void BeginCountdown()
    {
        StartCoroutine(StartCountDown());
    }

    public static class TrafficLightsEvents
    {
        public static Action BeginCountdown;
    }
}
