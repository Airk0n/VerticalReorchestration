using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MusicEngine : MonoBehaviour
{
    private AudioSource[] sources = new AudioSource[2];
    public bool isPlaying;


    private AudioClip clip;
    private double nextEventTime;
    private int flip = 0;
    private bool clipWaiting;
    public Source engineStatus;
    public Source sourceWaiting = Source.Neither;
    public MusicBlockType EngineType;


    [SerializeField] private MusicBlock currentBlock;

    public void NextBlock(MusicBlock block)
    {
        if (block == currentBlock) return;
        currentBlock = block;

        UpdateSources(block);

    }

    private void UpdateSources(MusicBlock block)
    {
        switch (engineStatus)
        {
            case Source.JustZero:
                sources[1].clip = block.clip;
                clip = block.clip;
                clipWaiting = true;
                sourceWaiting = Source.JustZero;
                break;
            case Source.JustOne:
                sources[0].clip = block.clip;
                clip = block.clip;
                clipWaiting = true;
                sourceWaiting = Source.JustOne;
                break;
            case Source.Both:
                clipWaiting = true;
                sourceWaiting = Source.Both;
                break;
            case Source.Neither:
                clip = block.clip;
                for (int i = 0; i < 2; i++)
                {
                    sources[i].clip = clip;
                }
                clipWaiting = false;
                sourceWaiting = Source.Neither;
                break;
        }
    }

    public void Play()
    {
        isPlaying = true;
    }

    public void StopAfterLoop()
    {
        isPlaying = false;
    }

    public void StopImediate()
    {
        isPlaying = false;
        for (int i = 0; i < 2; i++)
        {
            sources[i].Stop();
        }
    }

    public void Init(MusicManager manager, MusicBlockType type)
    {
        EngineType = type;
        nextEventTime = AudioSettings.dspTime + 2.0f;

        string objectName = " ";
        switch (type)
        {
            case MusicBlockType.intro:
                objectName = "IntroSource";
                break;
            case MusicBlockType.normal:
                objectName = "NormalSource";
                break;
            case MusicBlockType.outro:
                objectName = "OutroSource";
                break;
            case MusicBlockType.none:
                objectName = " _ ";
                break;
        }
        for (int i = 0; i < 2; i++)
        {
            GameObject child = new GameObject(objectName);
            child.transform.parent = gameObject.transform;
            sources[i] = child.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
        }

    }

    private void Update()
    {

        StatusCheck();
        UpdateClips();

        if (!isPlaying)
        {
            return;
        }

        double time = AudioSettings.dspTime;

        if (time + 1.0f > nextEventTime)
        {
            sources[flip].clip = clip;
            sources[flip].PlayScheduled(nextEventTime);

            //Debug.Log("Scheduled source " + flip + " to start at time " + nextEventTime);

            nextEventTime += 60.0f / currentBlock.BPM * (currentBlock.LoopLengthInBeats * 4);

            flip = 1 - flip;
        }


    }



    public void StatusCheck()
    {
        int howManyPlaying = 0;
        int whichOne = -1;
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].isPlaying)
            {
                howManyPlaying++;
                whichOne = i;
            }
        }

        if (howManyPlaying == 2) engineStatus = Source.Both;
        if (howManyPlaying == 0) engineStatus = Source.Neither;
        if (howManyPlaying == 1 && whichOne == 0) engineStatus = Source.JustZero;
        if (howManyPlaying == 1 && whichOne == 1) engineStatus = Source.JustOne;

        switch (engineStatus)
        {
            case Source.JustZero:
                if(EngineType == MusicBlockType.intro || EngineType == MusicBlockType.outro)
                {
                    isPlaying = false;
                }
                break;
            case Source.JustOne:
                if (EngineType == MusicBlockType.intro || EngineType == MusicBlockType.outro)
                {
                    isPlaying = false;
                }
                break;
            case Source.Both:
                if (EngineType == MusicBlockType.intro || EngineType == MusicBlockType.outro)
                {
                    isPlaying = false;
                }
                break;
            case Source.Neither:
                break;
        }

    }




    private void UpdateClips()
    {
        if (clipWaiting == false) return;

        switch (sourceWaiting)
        {
            case Source.JustZero:
                if (sources[0].isPlaying) return;
                sources[0].clip = currentBlock.clip;
                sourceWaiting = Source.Neither;
                clipWaiting = false;
                break;
            case Source.JustOne:
                if (sources[1].isPlaying) return;
                sources[1].clip = currentBlock.clip;
                sourceWaiting = Source.Neither;
                clipWaiting = false;
                break;
            case Source.Both:
                UpdateSources(currentBlock);
                break;
            case Source.Neither:
                clipWaiting = false;
                break;
        }





    }


}



