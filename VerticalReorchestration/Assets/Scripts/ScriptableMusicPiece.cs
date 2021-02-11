using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new piece of music", menuName ="music/MusicPiece")]
public class ScriptableMusicPiece : ScriptableObject
{
    [Header("Track Details")]
    public string TrackTitle;
    public double BPM;
    [Header("Audio Clips")]
    public MusicBlock[] intros;
    public MusicBlock[] normals;
    public MusicBlock[] outros;

}
