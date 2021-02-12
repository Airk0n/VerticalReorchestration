using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "new music block", menuName = "music/MusicBlock")]
public class MusicBlock : ScriptableObject
{
    public AudioClip clip;
    public float BPM;
    public int LoopLength;
    public MusicBlockType type;
}
