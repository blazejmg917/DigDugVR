using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTracker : MonoBehaviour
{
    [SerializeField]
    private int currentFrameCount = 0;
    [SerializeField]
    private int playCount = 16; // like 12 ish for fast music
    [SerializeField]
    private int midiStep = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentFrameCount++;
        if (currentFrameCount >= playCount)
        {
            currentFrameCount = 0;
            midiStep = (midiStep + 1) % 64;
            GetComponent<FMODUnity.StudioEventEmitter>().SetParameter("eigth_note", midiStep);

        }

        // maybe have more methods that acts as signals to speed up/slow down the music

    }
}
