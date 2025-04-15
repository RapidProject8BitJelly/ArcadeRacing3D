using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    public List<RaceProgressTracker> racers = new List<RaceProgressTracker>();
    public TMP_Text textPrefab;
    public Transform leaderboardParent;
    public float verticalSpacing = 40f;
    public float animationDuration = 0.3f;

    private Dictionary<RaceProgressTracker, TMP_Text> racerToText = new Dictionary<RaceProgressTracker, TMP_Text>();
    private bool initialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!initialized && racers.Count > 0)
        {
            InitializeLeaderboardTexts();
            initialized = true;
        }

        if (!initialized) return;

        var sorted = racers.OrderByDescending(r => r.NormalizedProgress).ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            var racer = sorted[i];
            var text = racerToText[racer];
            text.text = $"{i + 1}. {racer.name}";

            Vector2 targetPos = new Vector2(text.rectTransform.anchoredPosition.x, -i * verticalSpacing - 20);

            text.rectTransform.DOAnchorPos(targetPos, animationDuration).SetEase(Ease.InOutSine);
        }
    }

    private void InitializeLeaderboardTexts()
    {
        foreach (Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }

        racerToText.Clear();

        for (int i = 0; i < racers.Count; i++)
        {
            var racer = racers[i];
            var text = Instantiate(textPrefab, leaderboardParent);
            text.text = "";

            var rectTransform = text.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * verticalSpacing - 20);

            racerToText[racer] = text;
        }
    }
    
    public GameObject GetCurrentLeaderObject()
    {
        if (racers.Count == 0) return null;

        var sorted = racers.OrderByDescending(r => r.NormalizedProgress).ToList();
        return sorted[0].gameObject;
    }
}
