using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    [SerializeField] private ScriptableMusicPiece Track;

    private MusicEngine _introEngine;
    private MusicEngine _normalEngine;
    private MusicEngine _outroEngine;
    private MusicEngine[] allEngines = new MusicEngine[3];
    private Dictionary<MusicBlockType, int> BlockIndexes = new Dictionary<MusicBlockType, int>();
    private Metronome metronome;


    private void Awake()
    {
        metronome = gameObject.AddComponent<Metronome>();

        _introEngine = gameObject.AddComponent<MusicEngine>();
        _introEngine.Init(this,MusicBlockType.intro);

        _normalEngine = gameObject.AddComponent<MusicEngine>();
        _normalEngine.Init(this, MusicBlockType.normal);

        _outroEngine = gameObject.AddComponent<MusicEngine>();
        _outroEngine.Init(this, MusicBlockType.outro);

        allEngines[0] = _introEngine;
        allEngines[1] = _normalEngine;
        allEngines[2] = _outroEngine;

        BlockIndexes.Add(MusicBlockType.intro, 0);
        BlockIndexes.Add(MusicBlockType.normal, 0);
        BlockIndexes.Add(MusicBlockType.outro, 0);

    }
    private void Start()
    {
        metronome.SetTempo(Track.BPM, 4);
    }

    /// <summary>
    /// Plays a random intro MusicBlock.
    /// </summary>
    public void PlayIntro()
    {
        if (Track.intros == null) return;

        int newIndex = Random.Range(0, Track.intros.Length);
        int currentIndex;
        BlockIndexes.TryGetValue(MusicBlockType.intro, out currentIndex);
        if (currentIndex != newIndex) BlockIndexes[MusicBlockType.intro] = newIndex;

        _introEngine.NextBlock(Track.intros[newIndex]);
        _introEngine.Play();

    }
    public void PlayIntro(int index)
    {
        if (Track.intros == null) return;
        if (index > Track.intros.Length - 1) return;
        BlockIndexes[MusicBlockType.intro] = index;
        _introEngine.NextBlock(Track.intros[index]);
        _introEngine.Play();

    }
    public void StopAfterLoop()
    {
        foreach(MusicEngine  i in allEngines)
        {
            i.StopAfterLoop();
        }
    }
    public void StopAfterLoop(MusicBlockType type)
    {
        switch (type)
        {
            case MusicBlockType.intro:
                _introEngine.StopAfterLoop();
                break;
            case MusicBlockType.normal:
                _normalEngine.StopAfterLoop();
                break;
            case MusicBlockType.outro:
                _outroEngine.StopAfterLoop();
                break;
            case MusicBlockType.none:
                return;
        }

    }
    public void StopImediate()
    {
        foreach (MusicEngine i in allEngines)
        {
            i.StopImediate();
        }
    }
    public void StopImediate(MusicBlockType type)
    {
        switch (type)
        {
            case MusicBlockType.intro:
                _introEngine.StopImediate();
                break;
            case MusicBlockType.normal:
                _normalEngine.StopImediate();
                break;
            case MusicBlockType.outro:
                _outroEngine.StopImediate();
                break;
            case MusicBlockType.none:
                return;
        }
    }
    public void GiveNewBlock(MusicBlockType engine, int index)
    {
        if (engine == MusicBlockType.none) return;
        BlockIndexes[engine] = index;
        switch (engine)
        {
            case MusicBlockType.intro:
                if (index > Track.intros.Length - 1) return;
                _introEngine.NextBlock(Track.intros[index]);
                break;

            case MusicBlockType.normal:
                if (index > Track.normals.Length - 1) return;
                _normalEngine.NextBlock(Track.normals[index]);
                break;

            case MusicBlockType.outro:
                if (index > Track.outros.Length - 1) return;
                _outroEngine.NextBlock(Track.outros[index]);
                break;

            case MusicBlockType.none:
                return;
        }

    }

    public void ChangeTension()
    {
        
    }


}

