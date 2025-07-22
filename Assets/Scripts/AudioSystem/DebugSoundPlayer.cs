using UnityEngine;

public class DebugSoundPlayer : MonoBehaviour
{
    public Sound soundToPlay;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.instance.Play(soundToPlay);
        }
    }
}
