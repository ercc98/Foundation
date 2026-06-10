using NUnit.Framework;
using UnityEngine;
using Unity.Cinemachine;
using ErccDev.Foundation.Cameras;

public class CameraShakerBaseTests
{
    private GameObject _go;
    private CameraShakerBase _shaker;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private CameraShakeProfile _profile;

    [SetUp]
    public void SetUp()
    {
        // RequireComponent pulls in CinemachineCamera + the perlin noise component.
        _go     = new GameObject("CameraShaker_Test");
        _shaker = _go.AddComponent<CameraShakerBase>(); // active GO => Awake wires up perlin
        _perlin = _go.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)      UnityEngine.Object.DestroyImmediate(_go);
        if (_profile != null) UnityEngine.Object.DestroyImmediate(_profile);
    }

    [Test]
    public void NewShaker_IsNotShaking()
    {
        Assert.IsFalse(_shaker.IsShaking, "A shaker should be idle right after Awake.");
    }

    [Test]
    public void IntensityMultiplier_ClampsNegativeToZero()
    {
        _shaker.IntensityMultiplier = -3f;
        Assert.AreEqual(0f, _shaker.IntensityMultiplier, "Negative intensity should clamp to 0.");
    }

    [Test]
    public void Shake_StartsShaking_AndAppliesNoiseGains()
    {
        _shaker.IntensityMultiplier = 1f;
        _shaker.Shake(2f, 3f, 0.5f);

        Assert.IsTrue(_shaker.IsShaking, "Shake should leave the shaker active.");
        Assert.AreEqual(2f, _perlin.AmplitudeGain, 0.001f, "Amplitude gain should match the requested amplitude.");
        Assert.AreEqual(3f, _perlin.FrequencyGain, 0.001f, "Frequency gain should match the requested frequency.");
    }

    [Test]
    public void Shake_ScalesAmplitudeByIntensityMultiplier()
    {
        _shaker.IntensityMultiplier = 2f;
        _shaker.Shake(2f, 1f, 0.5f);

        Assert.AreEqual(4f, _perlin.AmplitudeGain, 0.001f, "Amplitude should be scaled by the intensity multiplier.");
    }

    [Test]
    public void Shake_WithNullProfile_FallsBackToDefaults()
    {
        _shaker.Shake((CameraShakeProfile)null);
        Assert.IsTrue(_shaker.IsShaking, "A null profile should fall back to the default shake.");
    }

    [Test]
    public void Shake_WithProfile_UsesProfileValues()
    {
        _profile = ScriptableObject.CreateInstance<CameraShakeProfile>();
        _profile.amplitude = 1.5f;
        _profile.frequency = 4f;
        _profile.duration  = 0.2f;

        _shaker.IntensityMultiplier = 1f;
        _shaker.Shake(_profile);

        Assert.AreEqual(1.5f, _perlin.AmplitudeGain, 0.001f);
        Assert.AreEqual(4f,   _perlin.FrequencyGain, 0.001f);
    }

    [Test]
    public void StopShake_ClearsNoise_AndStops()
    {
        _shaker.Shake(2f, 3f, 1f);
        _shaker.StopShake();

        Assert.IsFalse(_shaker.IsShaking);
        Assert.AreEqual(0f, _perlin.AmplitudeGain, 0.001f, "Stopping should zero the amplitude gain.");
        Assert.AreEqual(0f, _perlin.FrequencyGain, 0.001f, "Stopping should zero the frequency gain.");
    }
}
