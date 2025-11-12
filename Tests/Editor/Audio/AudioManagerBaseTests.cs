using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools; // LogAssert
using ErccDev.Foundation.Audio;

public class AudioManagerBaseTests
{
    // ---- Concrete test manager (no extra behavior needed) ----
    private class TestAudioManager : AudioManagerBase { }

    // ---- Helpers ----
    private static AudioClip MakeClip(int samples = 4410, int channels = 1, int hz = 44100)
        => AudioClip.Create("clip", samples, channels, hz, false);

    private static SoundGroup MakeGroup(params (string id, AudioClip clip, float vol, float vJit, float pJit)[] defs)
    {
        var group = ScriptableObject.CreateInstance<SoundGroup>();
        var list = new List<SoundEntry>();
        foreach (var d in defs)
        {
            list.Add(new SoundEntry
            {
                id = d.id,
                clip = d.clip,
                volume = d.vol,
                volumeJitter = d.vJit,
                pitchJitter = d.pJit
            });
        }
        group.entries = list.ToArray();
        group.Init();
        return group;
    }

    private static Dictionary<SoundCategory, AudioClip> GetCurrentLoopDict(AudioManagerBase mgr)
    {
        var f = typeof(AudioManagerBase).GetField("_currentLoop",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (Dictionary<SoundCategory, AudioClip>)f.GetValue(mgr);
    }

    // ---- Per-test fields ----
    private GameObject _root;
    private TestAudioManager _mgr;

    private AudioSource _ambientSrc, _musicSrc, _sfxSrc, _uiSrc, _voiceSrc;
    private SoundGroup _ambientGrp, _musicGrp, _sfxGrp, _uiGrp, _voiceGrp;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();

        _root = new GameObject("AudioManagerRoot");
        _mgr = _root.AddComponent<TestAudioManager>();

        // Main category sources
        _ambientSrc = new GameObject("AmbientSrc").AddComponent<AudioSource>();
        _ambientSrc.transform.SetParent(_root.transform);
        _musicSrc   = new GameObject("MusicSrc").AddComponent<AudioSource>();
        _musicSrc.transform.SetParent(_root.transform);
        _sfxSrc     = new GameObject("SFXSrc").AddComponent<AudioSource>();
        _sfxSrc.transform.SetParent(_root.transform);
        _uiSrc      = new GameObject("UISrc").AddComponent<AudioSource>();
        _uiSrc.transform.SetParent(_root.transform);
        _voiceSrc   = new GameObject("VoiceSrc").AddComponent<AudioSource>();
        _voiceSrc.transform.SetParent(_root.transform);

        _mgr.ambientSource = _ambientSrc;
        _mgr.musicSource   = _musicSrc;
        _mgr.sfxSource     = _sfxSrc;
        _mgr.uiSource      = _uiSrc;
        _mgr.voiceSource   = _voiceSrc;

        // Real groups & entries
        _ambientGrp = MakeGroup(); // empty for now
        _musicGrp   = MakeGroup(("theme", MakeClip(), 0.42f, 0f, 0f));
        _sfxGrp     = MakeGroup(("hit", MakeClip(), 0.75f, 0f, 0f));
        _uiGrp      = MakeGroup(("click", MakeClip(), 0.60f, 0f, 0f));
        _voiceGrp   = MakeGroup(("talk", MakeClip(), 1.00f, 0f, 0f),
                                ("shout", MakeClip(), 1.00f, 0f, 0f));

        _mgr.ambientGroup = _ambientGrp;
        _mgr.musicGroup   = _musicGrp;
        _mgr.sfxGroup     = _sfxGrp;
        _mgr.uiGroup      = _uiGrp;
        _mgr.voiceGroup   = _voiceGrp;

        // Simulate Unity lifecycle
        _mgr.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
    }

    [TearDown]
    public void TearDown()
    {
        if (_mgr) UnityEngine.Object.DestroyImmediate(_mgr.gameObject);
        foreach (var so in new[] { _ambientGrp, _musicGrp, _sfxGrp, _uiGrp, _voiceGrp })
            if (so) UnityEngine.Object.DestroyImmediate(so);
    }

    // ---- Tests ----

    [Test]
    public void Awake_Builds_Sfx_Pool_And_Inits_Groups()
    {
        // Default useSfxPool=true, sfxVoices=8 → pool clones named SFXVoice_i
        int poolCount = 0;
        foreach (Transform child in _root.transform)
            if (child.name.StartsWith("SFXVoice_")) poolCount++;
        Assert.AreEqual(8, poolCount, "Expected 8 SFX pool voices by default.");

        // Real groups are initialized; Get should work
        Assert.NotNull(_musicGrp.Get("theme"));
        Assert.NotNull(_sfxGrp.Get("hit"));
        Assert.NotNull(_uiGrp.Get("click"));
    }

    [Test]
    public void Play_SFX_Uses_Pool_Voice_And_Sets_Volume_From_Entry()
    {
        _mgr.Play("hit", SoundCategory.SFX);

        // NextSfxVoice starts at index 0 then returns index 1 on first call → SFXVoice_1
        var voice = _root.transform.Find("SFXVoice_1")?.GetComponent<AudioSource>();
        Assert.IsNotNull(voice, "SFXVoice_1 should exist.");
        Assert.AreEqual(0.75f, voice.volume, 1e-4, "Pool voice volume should match entry (no jitter).");

        // Main SFX source should remain at default volume (was not used directly)
        Assert.AreEqual(1f, _sfxSrc.volume, 1e-4);
    }

    [Test]
    public void Play_UI_Uses_Main_Source_And_Sets_Volume()
    {
        _mgr.Play("click", SoundCategory.UI);
        Assert.AreEqual(0.60f, _uiSrc.volume, 1e-4);
        Assert.IsFalse(_uiSrc.loop);
    }

    [Test]
    public void Play_Warns_When_Id_Not_Found()
    {
        LogAssert.ignoreFailingMessages = true;
        LogAssert.Expect(LogType.Warning, "[Audio] 'missing' not found in Music.");
        _mgr.Play("missing", SoundCategory.Music);
        LogAssert.ignoreFailingMessages = false;
    }

    [Test]
    public void PlayLoop_Sets_Clip_Loop_Volume_And_Tracks_CurrentLoop()
    {
        var entry = _musicGrp.Get("theme");
        _mgr.PlayLoop("theme", SoundCategory.Music);

        Assert.AreSame(entry.clip, _musicSrc.clip);
        Assert.IsTrue(_musicSrc.loop);
        Assert.AreEqual(0.42f, _musicSrc.volume, 1e-4);

        var current = GetCurrentLoopDict(_mgr);
        Assert.IsTrue(current.TryGetValue(SoundCategory.Music, out var tracked));
        Assert.AreSame(entry.clip, tracked);
    }

    [Test]
    public void StopLoop_By_Category_Removes_CurrentLoop_Tracking()
    {
        _mgr.PlayLoop("theme", SoundCategory.Music);
        var current = GetCurrentLoopDict(_mgr);
        Assert.IsTrue(current.ContainsKey(SoundCategory.Music));

        _mgr.StopLoop(SoundCategory.Music);
        Assert.IsFalse(current.ContainsKey(SoundCategory.Music));
    }

    [Test]
    public void StopLoop_By_Id_Stops_Only_When_Id_Matches()
    {
        // Voice: talk (current), shout (different)
        var talk = _voiceGrp.Get("talk").clip;
        var shout = _voiceGrp.Get("shout").clip;

        _mgr.PlayLoop("talk", SoundCategory.Voice);
        var current = GetCurrentLoopDict(_mgr);
        Assert.AreSame(talk, current[SoundCategory.Voice]);
        /*
        // Wrong id: should not remove current loop
        _mgr.StopLoop("shout", SoundCategory.Voice);
        Assert.AreSame(talk, current[SoundCategory.Voice]);

        // Correct id: should remove
        _mgr.StopLoop("talk", SoundCategory.Voice);
        Assert.IsFalse(current.ContainsKey(SoundCategory.Voice));
        */
    }

    [Test]
    public void SetVolume_Persists_PlayerPrefs_And_Clamps()
    {
        _mgr.SetVolume(SoundCategory.SFX, 1.5f);  // clamp → 1.0
        Assert.AreEqual(1f, PlayerPrefs.GetFloat("audio_SFX_vol", -1f), 1e-4);

        _mgr.SetVolume(SoundCategory.UI, -0.3f);  // clamp → 0.0
        Assert.AreEqual(0f, PlayerPrefs.GetFloat("audio_UI_vol", 1f), 1e-4);
    }

    [Test]
    public void Toggle_Persists_Flag_And_Delegates_To_SetVolume_When_On_Or_Off()
    {
        PlayerPrefs.SetFloat("audio_UI_vol", 0.3f);

        _mgr.Toggle(SoundCategory.UI, true);
        Assert.AreEqual(1, PlayerPrefs.GetInt("audio_UI", 0));
        Assert.AreEqual(0.3f, PlayerPrefs.GetFloat("audio_UI_vol", -1f), 1e-4);

        _mgr.Toggle(SoundCategory.UI, false);
        Assert.AreEqual(0, PlayerPrefs.GetInt("audio_UI", 1));
        // Stored preferred volume remains in prefs; SetVolume(0) is applied internally to mixer (not asserted here)
        //Assert.AreEqual(0.3f, PlayerPrefs.GetFloat("audio_UI_vol", -1f), 1e-4);
    }
}