using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{


    public int subdivisions = 4;
    private double tempo;
    private bool isRunning;

    // the length of a single tick in seconds
    private double _tickLength;

    // the next tick time, relative to AudioSettings.dspTime
    private double _nextTickTime;
    public delegate void Tick(double nextTick);
    public static event Tick Ticked;


    private void Awake()
    {
        Reset();
        isRunning = true;
    }

    public void SetTempo(double newTempo, int subs)
    {
        tempo = newTempo;
        subdivisions = subs;
        Reset();
    }

    /// <summary>
    /// Recalculate the tick length and reset the next tick time
    /// </summary>
    private void Reset()
    {
        Recalculate();
        // bump the next tick time ahead the length of one tick so we don't get a double trigger
        _nextTickTime = AudioSettings.dspTime + _tickLength;
    }

    private void Recalculate()
    {
        double beatsPerSecond = tempo / 60.0;
        double ticksPerSecond = beatsPerSecond * subdivisions;
        _tickLength = 1.0 / ticksPerSecond;
    }

    private void Update()
    {
        if (!isRunning) return;
        double currentTime = AudioSettings.dspTime;

        // look ahead the length of one frame (approximately), because we'll be scheduling samples
        currentTime += Time.deltaTime;

        // there may be more than one tick within the next frame, so this will catch them all
        while (currentTime > _nextTickTime)
        {
            // if someone has subscribed to ticks from the metronome, let them know we got a tick
            if (Ticked != null)
            {
                Ticked(_nextTickTime);
            }

            // increment the next tick time
            _nextTickTime += _tickLength;
        }
    }

}

