using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrack : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    public int bars;
    public int beats;
    public int subs;

    private void OnEnable()
    {
        Metronome.Ticked += Play; 
    }

    public void Play(double nextClick)
    {
        SubsUp();
        if(subs == 1)
        {
            source.PlayScheduled(nextClick);
        }


    }

    public void SubsUp()
    {
        if(subs ==  4)
        {
            subs = 1;
            BeatsUp();

            
        }
        else
        {
            subs++;
        }

    }
    public void BeatsUp()
    {
        if (beats == 4)
        {
            beats = 1;
            bars++;


        }
        else
        {
            beats++;
        }
    }



}
