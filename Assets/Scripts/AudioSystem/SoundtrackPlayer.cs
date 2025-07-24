using System;
using UnityEngine;

public class SoundtrackPlayer : MonoBehaviour
{
    [SerializeField] private Soundtrack soundtrackName;
    private void Start()
    {
        SoundtrackManager.instance.PlaySoundtrack(soundtrackName);
    }
}
