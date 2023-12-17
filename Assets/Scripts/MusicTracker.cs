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

    

    [SerializeField]
    private int currentFrameCount = 0;

    private int playCount = 16; // like 16 ish default, but 22 max
    [SerializeField]
    private int midiStep = 0;

    //[SerializeField]
    //private float timePassed = 0f;
    //[SerializeField]
    //private float eigthTime = .5f; // like 12 ish for fast music


    [SerializeField, Tooltip("the character controller to follow (within xr origin)")]
    private CharacterController characterController;
    private Vector3 currPosition;
    private Vector3 prevPosition;


    private bool walking = false;

    [SerializeField]
    private float distanceBetweenFootsteps;

    private float distanceTravelled = 0;


    // Start is called before the first frame update
    void Start()
    {
        currPosition = characterController.center;
        prevPosition = characterController.center;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // calculate player footstep timing
        if (characterController.velocity.magnitude > characterController.minMoveDistance) // only check when they are actually walking
        {
            currPosition = transform.TransformPoint(characterController.center); // transform to world coordinates
            distanceTravelled += Vector3.Distance(currPosition, prevPosition);

            if (distanceTravelled >= distanceBetweenFootsteps) // check if they have gone far enough
            {
                distanceTravelled %= distanceBetweenFootsteps;
                if (currPosition.z > -1.4) // check if they are actually in the cave (could be cleaner, just hoping 1.4 doesn't change for now)
                {
                    footsteps.Play();
                }
                
            }
            prevPosition = new Vector3(currPosition.x, currPosition.y, currPosition.z);
        }

        // calculate cave depth
        float depth = Mathf.Clamp(currPosition.z + 10, 0, 40);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("CaveDepth", depth);

        // determine song speed (yes this is ugly but it is fast and concise)
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
            midiStep = midiStep >= 63 ? 0 : (midiStep + 1); // faster than mod
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
