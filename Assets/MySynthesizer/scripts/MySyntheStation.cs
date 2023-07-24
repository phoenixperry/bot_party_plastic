#if UNITY_WEBGL && !UNITY_EDITOR
#   define DISABLE_TASK
#   define USE_WEBGL_AUDIO_STREAMING
#endif
#if WINDOWS_UWP
#   define USE_SYSTEM_THREADING_TASKS
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if USE_SYSTEM_THREADING_TASKS
using System.Threading.Tasks;
#else
using MySpace.Tasks;
#endif
using MySpace.Synthesizer;

using Conditional = System.Diagnostics.ConditionalAttribute;

namespace MySpace
{
    public class MyMMLSequenceUnit
    {
        private ToneMap toneMap;
        private MyMMLSequence sequence;
        private string error;
        public MyMMLSequenceUnit(ToneMap toneMap, MyMMLSequence sequence)
        {
            this.toneMap = toneMap;
            this.sequence = sequence;
            this.error = null;
        }
        public MyMMLSequenceUnit(string error)
        {
            this.toneMap = null;
            this.sequence = null;
            this.error = error;
        }
        public ToneMap ToneMap
        {
            get
            {
                return toneMap;
            }
        }
        public MyMMLSequence Sequence
        {
            get
            {
                return sequence;
            }
        }
        public string Error
        {
            get
            {
                return (error != null) ? error : "";
            }
        }
    }
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(AudioListener))]
    [DisallowMultipleComponent]
    [AddComponentMenu("MySynthesizer/MySyntheStation")]
    public class MySyntheStation : MonoBehaviour
    {
        [SerializeField]
        private uint baseFrequency = 31250;
        [SerializeField]
        private uint numVoices = 8;
        [SerializeField, MyReadOnly]
        private uint mixingBufferLength = 200;

        [SerializeField]
        private TextAsset PresetTone = null;

        private IEnumerator Provider(LinkedList<IEnumerator> jobs)
        {
            for (;;)
            {
                if (jobs.Count != 0)
                {
#if true
#if UNITY_5_6_OR_NEWER
                    yield return jobs.First.Value;
#else
                    var ie = jobs.First.Value;
                    while (ie.MoveNext())
                    {
                        yield return ie.Current;
                    }
#endif
                    jobs.RemoveFirst();
#else
                    var ie = jobs.First.Value;
                    for (;;)
                    {
                        try
                        {
                            if (!ie.MoveNext())
                            {
                                break;
                            }
                        }
                        catch (Exception ec)
                        {
                            Debug.Log(ec.Message + ":" + ec.StackTrace.ToString());
                            break;
                        }
                        yield return ie.Current;
                    }
                    jobs.RemoveFirst();
#endif
                }
                yield return null;
            }
        }
        private struct AudioClipGeneratorState
        {
            public const int frequency = 44100;
            public const int numChannels = 2;
            public volatile MyMixer mixer;
            public List<MySynthesizer> synthesizers;
            public MyMMLSequencer sequencer;
        }
        private MyMixer mixer = null;
        private List<MySynthesizer> synthesizers = null;
        private List<MyMMLSequencer> sequencers = null;
        private LinkedList<IEnumerator> jobListA = null;
        private LinkedList<IEnumerator> jobListB = null;
        private IEnumerator coProviderA = null;
        private IEnumerator coProviderB = null;
        private AudioClipGeneratorState acgState;
        private volatile bool disabled = false;

#if USE_WEBGL_AUDIO_STREAMING
        private MyWebAudioStreamer myWebAudioFilter = null;
#endif
        private void Tick()
        {
            lock (sequencers)
            {
                for (int i = 0; i < sequencers.Count; i++)
                {
                    sequencers[i].Tick();
                }
            }
        }
        public void AddSequencer(MyMMLSequencer sequencer)
        {
            lock (sequencer)
            {
                sequencers.Add(sequencer);
            }
        }
        public void RemoveSequencer(MyMMLSequencer sequencer)
        {
            lock (sequencers)
            {
                sequencers.Remove(sequencer);
            }
        }
        public List<MySynthesizer> Synthesizers
        {
            get
            {
                return synthesizers;
            }
        }
        public float MixerVolume
        {
            get
            {
                return mixer.MasterVolume;
            }
            set
            {
                mixer.MasterVolume = value;
            }
        }
        public uint TickFrequency
        {
            get
            {
                return mixer.TickFrequency;
            }
        }
#if UNITY_EDITOR
        public bool LivingDead
        {
            get;
            private set;
        }
#endif
        public static event Action<string> DebugOut = null;

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        private static void DebugLog(string str)
        {
            if (DebugOut != null)
            {
                DebugOut(str);
            }
            //Debug.Log(str);
        }

        private void Awake()
        {
            //UnityEngine.Debug.Log("Awake");
            mixer = new MyMixer((uint)AudioSettings.outputSampleRate, false, mixingBufferLength, baseFrequency);
            sequencers = new List<MyMMLSequencer>();
            mixer.TickCallback += Tick;
            synthesizers = new List<MySynthesizer>();
            synthesizers.Add(new MySynthesizerPM8(mixer, numVoices));
            synthesizers.Add(new MySynthesizerSS8(mixer, numVoices));
            synthesizers.Add(new MySynthesizerCT8(mixer, numVoices));
            synthesizers.Add(new MySynthesizerSF2(mixer, numVoices));

            acgState.mixer = new MyMixer(AudioClipGeneratorState.frequency, true, mixingBufferLength, baseFrequency);
            acgState.sequencer = new MyMMLSequencer(acgState.mixer.TickFrequency);
            acgState.mixer.TickCallback += acgState.sequencer.Tick;
            acgState.synthesizers = new List<MySynthesizer>();
            acgState.synthesizers.Add(new MySynthesizerPM8(acgState.mixer, numVoices));
            acgState.synthesizers.Add(new MySynthesizerSS8(acgState.mixer, numVoices));
            acgState.synthesizers.Add(new MySynthesizerCT8(acgState.mixer, numVoices));
            acgState.synthesizers.Add(new MySynthesizerSF2(acgState.mixer, numVoices));

            jobListA = new LinkedList<IEnumerator>();
            jobListB = new LinkedList<IEnumerator>();
            coProviderA = Provider(jobListA);
            this.StartCoroutineEx(coProviderA);
            coProviderB = Provider(jobListB);
            this.StartCoroutineEx(coProviderB);

#if !DISABLE_TASK
            StartUpdateLoopTask();
#endif
#if UNITY_EDITOR
            LivingDead = true;
#endif

#if USE_WEBGL_AUDIO_STREAMING
            int bufferLength;
            int numBuffers;
            AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
#if true
            if(numBuffers < 4)
            {
                numBuffers = 4;
            }
            if(bufferLength < 1024)
            {
                bufferLength = 1024;
            }
#endif
            myWebAudioFilter = new MyWebAudioStreamer(bufferLength, numBuffers, AudioSettings.outputSampleRate, OnAudioFilterRead);
#endif

            SetupPresetTones();
        }
        private void SetupPresetTones()
        {
            if ((PresetTone == null) || string.IsNullOrEmpty(PresetTone.text))
            {
                return;
            }
            var clip = new MyMMLClip();
            clip.TextA = PresetTone;
            PrepareClip(clip, (c) =>
            {
                if (c.Unit == null)
                {
                    return;
                }
                var toneMap = c.Unit.ToneMap;
                if (toneMap == null)
                {
                    return;
                }
                if (toneMap.Count != 0)
                {
                    foreach (var syn in synthesizers)
                    {
                        syn.AddToneMap(toneMap);
                    }
                    foreach (var syn in acgState.synthesizers)
                    {
                        syn.AddToneMap(toneMap);
                    }
                }
            });
        }
        private void OnDestroy()
        {
#if USE_WEBGL_AUDIO_STREAMING
            myWebAudioFilter.Dispose();
            myWebAudioFilter = null;
#endif
            //UnityEngine.Debug.Log("OnDestroy");
            this.StopCoroutineEx(coProviderA);
            this.StopCoroutineEx(coProviderB);
            jobListB = null;
            jobListA = null;
            foreach (var ss in synthesizers)
            {
                ss.Terminate();
            }
            synthesizers = null;
            mixer.TickCallback -= Tick;
            mixer.Terminate();
            mixer = null;
            sequencers = null;
            foreach (var ss in acgState.synthesizers)
            {
                ss.Terminate();
            }
            acgState.mixer.TickCallback -= acgState.sequencer.Tick;
            acgState.synthesizers = null;
            acgState.mixer.Terminate();
            acgState.mixer = null;
            acgState.sequencer = null;
#if !DISABLE_TASK
            StopUpdateLoopTask();
#endif
            //UnityEngine.Debug.Log("OnDestroy: done");
        }
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (mixer == null)
            {
                Awake();
            }
#endif
            //UnityEngine.Debug.Log("OnEnable");
            //UnityEngine.Debug.Log("sampleRate=" + AudioSettings.outputSampleRate);
#if false
            int len;
            int num;
            AudioSettings.GetDSPBufferSize(out len, out num);
            UnityEngine.Debug.Log("len=" + len);
            UnityEngine.Debug.Log("num=" + num);
#endif
            if (disabled)
            {
                disabled = false;
            }
        }
        private void OnDisable()
        {
            //UnityEngine.Debug.Log("OnDisable");
            if (!disabled)
            {
                disabled = true;
                foreach (var ss in synthesizers)
                {
                    ss.Reset();
                }
                mixer.Update();
                mixer.Reset();
            }
        }
#if !DISABLE_TASK
        private System.Threading.AutoResetEvent updateEvent;
        private Task updateLoopTask;
        private volatile bool exitUpdateLoopTask;
        private void StartUpdateLoopTask()
        {
            exitUpdateLoopTask = false;
            updateEvent = new System.Threading.AutoResetEvent(false);
            updateLoopTask = Task.Run((Action)UpdateLoopTaskAction);
        }
        private void StopUpdateLoopTask()
        {
            exitUpdateLoopTask = true;
            updateEvent.Set();
            updateLoopTask.Wait();
            updateEvent = null;
        }
        private void UpdateLoopTaskAction()
        {
            for (;;)
            {
                updateEvent.WaitOne();
                if (exitUpdateLoopTask)
                {
                    break;
                }
                var mix = mixer;
                if (mix != null)
                {
                    mix.Update();
                }
            }
        }
#endif
#if DISABLE_TASK || USE_WEBGL_AUDIO_STREAMING
        private void Update()
        {
            var m = mixer;
            if (!disabled && (m != null))
            {
#if DISABLE_TASK
                m.Update();
#else
                updateEvent.Set();
#endif
#if USE_WEBGL_AUDIO_STREAMING
                myWebAudioFilter.Update();
#endif
            }
        }
#endif
        private void OnAudioFilterRead(float[] data, int channels)
        {
            var m = mixer;
            if (!disabled && (m != null))
            {
                m.Output(data, channels, data.Length / channels);
#if !DISABLE_TASK
                updateEvent.Set();
#endif
            }
#if UNITY_EDITOR
            LivingDead = false;
#endif
        }

        public void PrepareClip(MyMMLClip clip, Action<MyMMLClip> onFinished = null, bool dontGenerate = false)
        {
            if (!clip.Dirty && ((dontGenerate && clip.Ready) || clip.Valid))
            {
                return;
            }
            DebugLog("PrepareClip:" + clip.Name);
            var job = new ClipPreparingJob(this, clip, null, onFinished, dontGenerate);
            jobListA.AddLast(job.Prepare());
        }
        public void PrepareClip(List<AssetBundle> bundles, MyMMLClip clip, Action<MyMMLClip> onFinished = null, bool dontGenerate = false)
        {
            if (!clip.Dirty && ((dontGenerate && clip.Ready) || clip.Valid))
            {
                return;
            }
            DebugLog("PrepareClip:" + clip.Name);
            var job = new ClipPreparingJob(this, clip, bundles, onFinished, dontGenerate);
            jobListA.AddLast(job.Prepare());
        }
        private class ClipPreparingJob
        {
            private readonly string mmlText = null;
            private MyMMLSequence mml = null;
            private ToneMap toneMap = null;

            private readonly MySyntheStation station = null;
            private readonly MyMMLClip clip = null;
            private readonly List<AssetBundle> bundles = null;
            private readonly Action<MyMMLClip> onFinished = null;
            private readonly bool dontGenerate = false;

            public ClipPreparingJob(MySyntheStation station, MyMMLClip clip, List<AssetBundle> bundles, Action<MyMMLClip> onFinished, bool dontGenerate)
            {
                this.station = station;
                this.clip = clip;
                this.bundles = bundles;
                this.onFinished = onFinished;
                this.dontGenerate = dontGenerate;
                mmlText = clip.Text;
                clip.Dirty = false;
                clip.Unit = null;
            }
            public IEnumerator Prepare()
            {
                var name = string.IsNullOrEmpty(clip.Name) ? "NamelessClip" : clip.Name;
                DebugLog(name + ": start preparing clip");
                mml = null;
                {
#if !DISABLE_TASK
                    var task = Task.Run(() =>
                    {
                        mml = new MyMMLSequence(mmlText);
                    });
                    while (!task.IsCompleted)
                    {
                        yield return null;
                    }
#else
                    mml = new MyMMLSequence(mmlText);
                    yield return null;
#endif
                }
                DebugLog(name + ": parsing mml done.");
                //Debug.Assert(mml != null);
                if (mml.ErrorLine != 0)
                {
                    clip.Unit = new MyMMLSequenceUnit("Failed: parsing mml < " + mml.ErrorLine.ToString() + ":" + mml.ErrorPosition.ToString() + " > : " + mml.ErrorString + " <<<< " + mml.ErrorMessage);
                    DebugLog(clip.Unit.Error);
                    if (onFinished != null)
                    {
                        onFinished(clip);
                    }
                    yield break;
                }

                toneMap = new ToneMap();
                {
#if !DISABLE_TASK
                    var task = Task.Run(() =>
                    {
                        toneMap = SetupToneMap(toneMap, mml.ToneInfos);
                    });
                    while (!task.IsCompleted)
                    {
                        yield return null;
                    }
#else
                    toneMap = SetupToneMap(toneMap, mml.ToneInfos);
                    yield return null;
#endif
                }

                {
                    var ie = SetupSF2Tone(toneMap, bundles, mml);
                    while (ie.MoveNext())
                    {
#if false
                        if (ie.Current != null)
                        {
                            clip.Unit = new MyMMLSequenceUnit(ie.Current as string);
                            debugLog(clip.Unit.Error);
                            if (onFinished != null)
                            {
                                onFinished(clip);
                            }
                            yield break;
                        }
#endif
                        yield return null;
                    }
                }
                {
                    var ie = SetupSS8Tone(toneMap, bundles);
                    while (ie.MoveNext())
                    {
#if false
                        if (ie.Current != null)
                        {
                            clip.Unit = new MyMMLSequenceUnit(ie.Current as string);
                            debugLog(clip.Unit.Error);
                            if (onFinished != null)
                            {
                                onFinished(clip);
                            }
                            yield break;
                        }
#endif
                        yield return null;
                    }
                }

                DebugLog(name + ": tone setup finished.");
                clip.Unit = new MyMMLSequenceUnit(toneMap, mml);
                if (dontGenerate || !clip.GenerateAudioClip)
                {
                    DebugLog(name + ": clip prepared.");
                    if (onFinished != null)
                    {
                        onFinished(clip);
                    }
                    yield break;
                }
                station.jobListB.AddLast(Generate());
                yield break;
            }
            private static ToneMap SetupToneMap(ToneMap toneMap, List<MyMMLSequence.ToneInfo> toneInfos)
            {
                foreach (var toneInfo in toneInfos)
                {
                    var ts = MySynthesizer.CreateToneSet(toneInfo.Data);
                    if (ts == null)
                    {
                        continue;
                    }
                    ts.Name = toneInfo.Name;
                    ToneSet toneSet;
                    toneMap.TryGetValue(toneInfo.PresetNo, out toneSet);
                    if (toneSet == null)
                    {
                        toneSet = ts;
                    }
                    else
                    {
                        ts.AddRange(toneSet);
                        toneSet = ts;
                    }
                    if (toneSet.Count != 0)
                    {
                        toneMap[toneInfo.PresetNo] = toneSet;
                    }
                }
                return toneMap;
            }
            private static IEnumerator SetupSF2Tone(ToneMap toneMap, List<AssetBundle> bundles, MyMMLSequence seq)
            {
                string sf2prop;
                if (seq.Property.TryGetValue("SoundFont2", out sf2prop))
                {
                    var ss = sf2prop.Split('\n');
                    foreach (var sf2 in ss)
                    {
                        TextAsset ta = null;
                        var ie = LoadAssetAsync<TextAsset>(bundles, sf2);
                        while (ie.MoveNext())
                        {
                            if (ie.Current != null)
                            {
                                ta = ie.Current as TextAsset;
                            }
                            yield return null;
                        }
                        if (ta == null)
                        {
                            DebugLog("Failed: LoadAssetAsync(" + sf2 + "):");
                            continue;
                        }
                        var sf2image = ta.bytes;
#if !DISABLE_TASK
                        var task = Task.Run(() =>
                        {
                            if (!MySynthesizerSF2.SetupToneMap(toneMap, sf2image))
                            {
                                DebugLog("Failed: SetupToneMap(" + sf2 + "):");
                            }
                        });
                        while (!task.IsCompleted)
                        {
                            yield return null;
                        }
#else
                        if (!MySynthesizerSF2.SetupToneMap(toneMap, sf2image))
                        {
                            DebugLog("Failed: SetupToneMap(" + sf2 + "):");
                        }
                        yield return null;
#endif
                        DebugLog("SetupToneMap(" + sf2 + "): loaded");
                    }
                }
            }
            private static IEnumerator SetupSS8Tone(ToneMap toneMap, List<AssetBundle> bundles)
            {
                var resource = new Dictionary<string, float[]>();
                foreach (var it in toneMap)
                {
                    var toneSet = it.Value;
                    for (int i = 0; i < toneSet.Count; i++)
                    {
                        var tone = toneSet[i];
                        if (tone is MySynthesizerSS8.ToneParam)
                        {
                            var ssTone = tone as MySynthesizerSS8.ToneParam;
                            float[] samples;
                            if (!resource.TryGetValue(ssTone.ResourceName, out samples))
                            {
                                AudioClip ac = null;
                                DebugLog(ssTone.Name + ": load resource " + ssTone.ResourceName);
                                var ie = LoadAssetAsync<AudioClip>(bundles, ssTone.ResourceName);
                                while (ie.MoveNext())
                                {
                                    if (ie.Current != null)
                                    {
                                        ac = ie.Current as AudioClip;
                                    }
                                    yield return null;
                                }
                                if (ac == null)
                                {
                                    yield return "Failed: LoadAssetAsync(" + ssTone.ResourceName + "): toneData[" + ssTone.Name + "]";
                                    yield break;
                                }
                                while (ac.loadState == AudioDataLoadState.Loading)
                                {
                                    yield return null;
                                }
                                samples = new float[ac.samples * ac.channels];
                                if (!ac.GetData(samples, 0))
                                {
                                    yield return "Failed: AudioClip.GetData(): toneData[" + ssTone.Name[i] + "]";
                                    yield break;
                                }
                                if (ac.channels != 1)
                                {
                                    var mix = new float[ac.samples];
                                    for (int k = 0; k < ac.samples; k++)
                                    {
                                        float s = 0.0f;
                                        for (int l = 0; l < ac.channels; l++)
                                        {
                                            s += samples[k * ac.channels + l];
                                        }
                                        mix[k] = s;
                                    }
                                    samples = mix;
                                }
                                resource.Add(ssTone.ResourceName, samples);
                            }
                            ssTone.SetSamples(samples);
                        }
                    }
                }
            }
            private IEnumerator Generate()
            {
                var name = string.IsNullOrEmpty(clip.Name) ? "NamelessClip" : clip.Name;
                DebugLog(name + ": start generating audioclip.");
                LinkedList<float[]> samples = null;
                {
#if !DISABLE_TASK
                    var task = Task.Run(() =>
                    {
                        var ie = GenerateAudioSamples();
                        while (ie.MoveNext())
                        {
                            if (ie.Current != null)
                            {
                                samples = ie.Current;
                                break;
                            }
                        }
                    });
                    while (!task.IsCompleted)
                    {
                        yield return null;
                    }
#else
                    var ie = GenerateAudioSamples();
#if true
                    var t0 = 0.0f;
                    while (ie.MoveNext())
                    {
                        if(ie.Current != null)
                        {
                            samples = ie.Current;
                            break;
                        }
                        t0 += Time.deltaTime;
                        if (t0 > 0.02f)
                        {
                            t0 = 0.0f;
                            yield return null;
                        }
                    }
#else
                    while (ie.MoveNext())
                    {
                        if(ie.Current != null)
                        {
                            samples = ie.Current;
                            break;
                        }
                        yield return null;
                    }
#endif
#endif
                }
                if (samples != null)
                {
                    int totalSamples = 0;
                    for (var i = samples.First; i != null; i = i.Next)
                    {
                        var block = i.Value;
                        totalSamples += block.Length / AudioClipGeneratorState.numChannels;
                    }
                    var ac = AudioClip.Create(clip.Name, totalSamples, AudioClipGeneratorState.numChannels, AudioClipGeneratorState.frequency, false);
                    int pos = 0;
#if UNITY_WEBGL
                    var data = new float[totalSamples * AudioClipGeneratorState.numChannels];
                    for (var i = samples.First; i != null; i = i.Next)
                    {
                        var block = i.Value;
                        for (int j = 0; j < block.Length; j++)
                        {
                            data[pos++] = block[j];
                        }
                    }
                    ac.SetData(data, 0);
#else
                    for (var i = samples.First; i != null; i = i.Next)
                    {
                        var block = i.Value;
                        ac.SetData(block, pos);
                        pos += block.Length / AudioClipGeneratorState.numChannels;
                    }
#endif
                    clip.AudioClip = ac;
                }
                DebugLog(name + ": audioclip generated.");
                if (onFinished != null)
                {
                    onFinished(clip);
                }
                yield break;
            }
            private IEnumerator<LinkedList<float[]>> GenerateAudioSamples()
            {
                var seq = station.acgState.sequencer;
                var mix = station.acgState.mixer;
                var sss = station.acgState.synthesizers;
                if ((seq == null) || (mix == null) || (sss == null))
                {
                    yield break;
                }
                mix.Reset();
                for (int j = 0; j < sss.Count; j++)
                {
                    seq.SetSynthesizer(j, sss[j], 0xffffffffU);
                }
                seq.KeyShift = clip.Key;
                seq.VolumeScale = clip.Volume;
                seq.TempoScale = clip.TempoScale;
                seq.Play(mml, toneMap, 0.0f, false);

                const int workSize = 4096;
                var work = new float[workSize * AudioClipGeneratorState.numChannels];
                var temp = new LinkedList<float[]>();
                bool zeroCross = false;
                for (;;)
                {
                    if (station.disabled)
                    {
                        yield break;
                    }
                    if (seq.Playing)
                    {
                        mix.Update();
                    }
                    Array.Clear(work, 0, work.Length);
                    int numSamples = mix.Output(work, AudioClipGeneratorState.numChannels, workSize);
                    if (numSamples == 0)
                    {
                        if (!zeroCross)
                        {
                            mix.Update();
                            continue;
                        }
                        break;
                    }
                    {
                        var block = new float[numSamples * AudioClipGeneratorState.numChannels];
                        Array.Copy(work, block, numSamples * AudioClipGeneratorState.numChannels);
                        temp.AddLast(block);
                        var v0 = work[(numSamples - 1) * AudioClipGeneratorState.numChannels + 0];
                        var v1 = work[(numSamples - 1) * AudioClipGeneratorState.numChannels + 1];
                        zeroCross = (v0 == 0.0f) && (v1 == 0.0f);
                    }
                    yield return null;
                }
                yield return temp;
            }
            private static IEnumerator LoadAssetAsync<T>(List<AssetBundle> bundles, string resourceName)
            {
                if (bundles != null)
                {
                    for (int i = 0; i < bundles.Count; i++)
                    {
                        var bundle = bundles[i];
                        if (bundle == null)
                        {
                            continue;
                        }
                        var request = bundle.LoadAssetAsync(resourceName, typeof(T));
                        if (request == null)
                        {
                            continue;
                        }
                        while (!request.isDone && (request.progress != 1))
                        {
                            yield return null;
                        }
                        yield return request.asset;
                        yield break;
                    }
                }
                {
                    var request = Resources.LoadAsync(resourceName, typeof(T));
                    if (request == null)
                    {
                        yield break;
                    }
                    while (!request.isDone && (request.progress != 1))
                    {
                        yield return null;
                    }
                    yield return request.asset;
                    yield break;
                }
            }
        }
    }
}
