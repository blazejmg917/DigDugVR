using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCamera : MonoBehaviour
{
    [SerializeField, Tooltip("The renderer to fade in/out over the camera")]
    private Renderer fadeInOut; 
    
    [SerializeField, Tooltip("the default length of time it takes for the camera to fade in or out")]
    private float defaultFadeTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        if(fadeInOut == null){

                fadeInOut = GetComponentInChildren<Renderer>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FadeToBlack(float fadeTime = -1){
        yield return new WaitForSeconds(1);
        if(fadeTime < 0){
            fadeTime = defaultFadeTime;
        }
        float timeElapsed = 0;
        Color imageColor = fadeInOut.material.color;
        float fadeAmount = 0;
        imageColor.a = fadeAmount;
        fadeInOut.material.color = imageColor;
        while(fadeInOut.material.color.a < 1){
            fadeAmount = Mathf.Lerp(0, 1, timeElapsed / fadeTime);
            imageColor.a = fadeAmount;
            fadeInOut.material.color = imageColor;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

    }

    public IEnumerator FadeFromBlack(float fadeTime = -1){
        yield return new WaitForSeconds(1);
        Debug.Log("fading from black");
        if(fadeTime < 0){
            fadeTime = defaultFadeTime;
        }
        float timeElapsed = 0;
        Color imageColor = fadeInOut.material.color;
        float fadeAmount = 1;
        imageColor.a = fadeAmount;
        fadeInOut.material.color = imageColor;
        while(fadeInOut.material.color.a > 0){
            fadeAmount = Mathf.Lerp(1, 0, timeElapsed / fadeTime);
            imageColor.a = fadeAmount;
            fadeInOut.material.color = imageColor;
            //Debug.Log("alpha: " + fadeInOut.material.color.a);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

    }
}
