using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTracker : MonoBehaviour
{
    [SerializeField]
    private StudioEventEmitter music;
    [SerializeField]
    private StudioEventEmitter cavenoise;
    [SerializeField]
    private StudioEventEmitter footsteps;

    private bool walking = false;

    [SerializeField]
    private int currentFrameCount = 0;

    private int playCount = 16; // like 16 ish default, but 22 max
    [SerializeField]
    private int midiStep = 0;

    //[SerializeField]
    //private float timePassed = 0f;
    //[SerializeField]
    //private float eigthTime = .5f; // like 12 ish for fast music

    [SerializeField]
    private Transform currentPlayerLocation;

    [SerializeField]
    private CharacterController characterController;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // velocity tracking stuff
        if (!walking)
        { 
            if(characterController.velocity.magnitude > 0.1f)
            {
                footsteps.enabled = true;
                walking = true;
                footsteps.SetParameter("end1", 0.0f);
                footsteps.Play();
            }
        }
        else 
        {
            if (characterController.velocity.magnitude < 0.1f)
            {
                walking = false;
                footsteps.SetParameter("end1", 1.0f);
            }
        }

        // player tracking stuff
        float depth = Mathf.Clamp(currentPlayerLocation.position.z + 10, 0, 40);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("CaveDepth", depth);

        if (depth < 20)
        {
            playCount = 16;
        } else if (depth < 25)
        {
            playCount = 17;
        }
        else if (depth < 30)
        {
            playCount = 18;
        }
        else if (depth < 33)
        {
            playCount = 19;
        }
        else if (depth < 35)
        {
            playCount = 20;
        }
        else if (depth < 38)
        {
            playCount = 21;
        }
        else if (depth < 40)
        {
            playCount = 22;
        }
     

        // music tracking stuff
        currentFrameCount++;
        if (currentFrameCount >= playCount)
        {
            currentFrameCount = 0;
            midiStep = (midiStep + 1) % 64;
            music.SetParameter("eigth_note", midiStep);
        } // have more methods that acts as signals to speed up/slow down the music

    }

    // // this was a small attempt to use a coroutine instead, maybe worth it, maybe not
    //IEnumerator CoroMusic()
    //{
    //    timePassed += Time.deltaTime;
    //    if (currentFrameCount >= playCount)
    //    {
    //        currentFrameCount = 0;
    //        midiStep = (midiStep + 1) % 64;
    //        GetComponent<FMODUnity.StudioEventEmitter>().SetParameter("eigth_note", midiStep);

    //    }
    //    yield return new WaitForSeconds(.1f); 
    //}
}
