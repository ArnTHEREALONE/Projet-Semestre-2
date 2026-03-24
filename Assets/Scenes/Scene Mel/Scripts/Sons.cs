using System;
using UnityEngine;

public class Sons : MonoBehaviour
{
    
    public AudioSource Jump;
    public AudioSource Dash;
    public AudioSource Run;
    public AudioSource Rewind;
    public AudioSource Freeze;

    // public bool run;
    // public bool dJump;
    // public bool dash;
    public bool isRewinding;
    public bool isFreezing;


    private void Start()
    {
        // run = GetComponent<PlayerController>().run;
        // dJump = GetComponent<PlayerController>().dJump;
        // dash = GetComponent<PlayerController>().dash;
        
        //Là je peux pas recup les bools parce que tu les a mis en private et je préfère pas toucher à ton code
        //isRewinding = GetComponent<TimeControlMovableObject>().isRewinding;
        //isFreezing = GetComponent<TimeControlMovableObject>().isFreezing;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Run"))
        {
            Run.Play();
        }        
        else if (Input.GetButtonDown("Jump"))
        {
            Jump.Play();
        }        
        else if (Input.GetButtonDown("Dash"))
        {
            Dash.Play();
        }

        if (isRewinding == true)
        {
            Rewind.Play();
        }

        if (isFreezing == true)
        {
            Freeze.Play();
        }
    }
}
