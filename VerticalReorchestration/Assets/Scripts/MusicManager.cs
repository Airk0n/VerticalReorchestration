using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    [SerializeField] private ScriptableMusicPiece Track;

    private MusicEngine _musicEngine;

    private MusicEngine[] allEngines = new MusicEngine[1];
    private Dictionary<MusicBlockType, int> BlockIndexes = new Dictionary<MusicBlockType, int>();
    private Metronome metronome;
    private MusicBlock _currentBlock;
    private MusicBlockType _queuedBlockType;
    private int _queuedBlockIndex;
    public MusicState currentMode = MusicState.Explicit;
    private int BlockWaitingSinceBeat;




    private void Awake()
    {
        metronome = gameObject.AddComponent<Metronome>();

        _musicEngine = gameObject.AddComponent<MusicEngine>();
        _musicEngine.Init(this,MusicBlockType.intro);
        MusicEngine.onFlipSource += EngineFlippedSource;

        allEngines[0] = _musicEngine;

        BlockIndexes.Add(MusicBlockType.intro, 0);
        BlockIndexes.Add(MusicBlockType.normal, 0);
        BlockIndexes.Add(MusicBlockType.outro, 0);
    }

    private void Update()
    {
        if(_queuedBlockType == MusicBlockType.normal && _musicEngine.beats > BlockWaitingSinceBeat)
        {
            GiveNewBlock(_queuedBlockType, _queuedBlockIndex);
            _queuedBlockType = MusicBlockType.none;
        }
    }

    public void EngineFlippedSource()
    {
        if (_musicEngine.engineStatus == Source.Neither) return;
        if (_currentBlock?.type != MusicBlockType.normal)
        {
            return;
        }
        int currentIndex = BlockIndexes[MusicBlockType.normal];
        switch (currentMode)
        {
            case MusicState.Explicit:
                break;

            case MusicState.Climb:
                    if (currentIndex != Track.normals.Length-1)
                    {
                        GiveNewBlock(MusicBlockType.normal, currentIndex + 1);
                    }
                break;

            case MusicState.Random:
                    GiveNewBlock(MusicBlockType.normal, Random.Range(0, Track.normals.Length - 1));
                break;

            case MusicState.Drift:
                    if (currentIndex != Track.normals.Length - 1 || currentIndex != 0)
                    {
                    GiveNewBlock(MusicBlockType.normal, Random.Range(currentIndex -1, currentIndex +1));
                    }
                    else
                    {
                    GiveNewBlock(MusicBlockType.normal, Random.Range(0, Track.normals.Length - 1));
                    }
                break;

            case MusicState.None:
                break;
        }

    }

    /// <summary>
    /// Plays a random intro MusicBlock on the Intro Engine.
    /// </summary>
    public void PlayIntro()
    {
        if (Track.intros == null) return;
        StartMetronome();

        int newIndex = Random.Range(0, Track.intros.Length);
        int currentIndex;
        BlockIndexes.TryGetValue(MusicBlockType.intro, out currentIndex);
        if (currentIndex != newIndex) BlockIndexes[MusicBlockType.intro] = newIndex;

        _musicEngine.NextBlock(Track.intros[newIndex]);
        _musicEngine.Play();

        switch (currentMode)
        {
            case MusicState.Explicit:
                QueueThisBlock(MusicBlockType.normal, 0);
                break;

            case MusicState.Climb:
                QueueThisBlock(MusicBlockType.normal,0);
                break;

            case MusicState.Random:
                QueueThisBlock(MusicBlockType.normal, Random.Range(0,Track.normals.Length-1));
                break;

            case MusicState.Drift:
                if(BlockIndexes[MusicBlockType.normal]== 0 || BlockIndexes[MusicBlockType.normal] == Track.normals.Length - 1)
                {
                    QueueThisBlock(MusicBlockType.normal, Random.Range(0, Track.normals.Length - 1));
                }
                else
                {
                    QueueThisBlock(MusicBlockType.normal,
                        Random.Range(
                            BlockIndexes[MusicBlockType.normal] - 1,
                            BlockIndexes[MusicBlockType.normal] + 1));
                }
                break;

            case MusicState.None:
                break;
        }



    }


    public void QueueThisBlock(MusicBlockType musicType, int index)
    {
        _queuedBlockType = musicType;
        _queuedBlockIndex = index;
        BlockWaitingSinceBeat = _musicEngine.beats;
    }
    /// <summary>
    /// Plays a specific Intro based on the index.
    /// </summary>
    /// <param name="index"></param>
    public void PlayIntro(int index)
    {
        if (Track.intros == null) return;
        if (index > Track.intros.Length - 1) return;

        StartMetronome();

        BlockIndexes[MusicBlockType.intro] = index;
        _musicEngine.NextBlock(Track.intros[index]);
        _musicEngine.Play();

    }
    /// <summary>
    /// All engines will stop playing audio after the current one has ended.
    /// </summary>
    public void StopAfterLoop()
    {
        foreach(MusicEngine  i in allEngines)
        {
            i.StopAfterLoop();
        }
        metronome.StopClicking();
    }
    /// <summary>
    /// All engines stop playing audio imediately.
    /// </summary>
    public void StopImediate()
    {
        foreach (MusicEngine i in allEngines)
        {
            i.StopImediate();
        }
        metronome.StopClicking();
    }
    /// <summary>
    /// Gives this specific engine a new audio block to work with.
    /// </summary>
    public void GiveNewBlock(MusicBlockType musicType, int index)
    {
        if (musicType == MusicBlockType.none) return;

        BlockIndexes[musicType] = index;
        switch (musicType)
        {
            case MusicBlockType.intro:
                if (index > Track.intros.Length - 1) return;
                _musicEngine.NextBlock(Track.intros[index]);
                _currentBlock = Track.intros[index];
                break;

            case MusicBlockType.normal:
                if (index > Track.normals.Length - 1) return;
                _musicEngine.NextBlock(Track.normals[index]);
                _currentBlock = Track.normals[index];
                break;

            case MusicBlockType.outro:
                if (index > Track.outros.Length - 1) return;
                _musicEngine.NextBlock(Track.outros[index]);
                _currentBlock = Track.outros[index];
                break;

            case MusicBlockType.none:
                return;
        }

    }

    public void ChangeTension()
    {
        
    }

    public void StartMetronome()
    {
        metronome.SetTempo(Track.BPM);
        metronome.StartClicking();
    }

    public void NewTrack(ScriptableMusicPiece newTrack)
    {
        Track = newTrack;
        metronome.SetTempo(newTrack.BPM);
    }

    private void OnDisable()
    {
        MusicEngine.onFlipSource -= EngineFlippedSource;
    }
}

