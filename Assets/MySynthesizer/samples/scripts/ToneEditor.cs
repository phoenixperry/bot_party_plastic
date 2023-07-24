
using System;

using UnityEngine;
using UnityEngine.UI;


namespace MySpace.Sample
{
    using ToneParamPM8 = Synthesizer.MySynthesizerPM8.ToneParam;

    public class ToneEditor : MonoBehaviour
    {
        private MySyntheStation syntheStation = null;
        private SampleTools sampleTools = null;
        private MyMMLBox mmlBox = null;

        private ToneParamPM8 param = null;
        private InputField inputField = null;
        private bool disabledEvent = false;

        [SerializeField]
        private int portNo = 0;
        [SerializeField]
        private int chNo = 15;

        private void setSliderValue(GameObject obj, string name, string label, float min, float max, int val)
        {
            var text = obj.transform.Find(name + "/Text").GetComponent<Text>();
            var slider = obj.transform.Find(name + "/Slider").GetComponent<Slider>();
            disabledEvent = true;
            text.text = label + val;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = val;
            disabledEvent = false;
        }
        private void setupSlider(GameObject obj, string name, string label, Action<float> apply)
        {
            var text = obj.transform.Find(name + "/Text").GetComponent<Text>();
            var slider = obj.transform.Find(name + "/Slider").GetComponent<Slider>();
            slider.wholeNumbers = true;
            slider.onValueChanged.AddListener((value) =>
            {
                if (!disabledEvent)
                {
                    text.text = label + (int)value;
                    apply(value);
                }
            });
        }
        private void applyTone()
        {
            var ts = new Synthesizer.ToneSet();
            ts.Add(param.Clone());
            syntheStation.Synthesizers[portNo].Channel[chNo].ProgramChange(ts);
            inputField.text = param.ToString();
        }

        private void loadTone(ToneParamPM8 tone)
        {
            param = (ToneParamPM8)tone.Clone();
            {
                var obj = transform.Find("Panel/FM").gameObject;
                setSliderValue(obj, "Algorithm", "al:", 0, 7, param.Al);
                setSliderValue(obj, "Feedback", "fb:", 0, 7, param.Fb);
                setSliderValue(obj, "WaveForm", "lw:", 0, 7, param.Lfo.WS);
                setSliderValue(obj, "Frequency", "lf:", 0, 127, param.Lfo.LF);
                setSliderValue(obj, "PMPower", "lp:", 0, 127, param.Lfo.LP);
                setSliderValue(obj, "AMPower", "la:", 0, 127, param.Lfo.LA);
                if (param.Extended)
                {
                    setSliderValue(obj, "AttackRate", "ar:", 0, 127, param.Lfo.Env.xAR);
                    setSliderValue(obj, "DecayRate", "dr:", 0, 127, param.Lfo.Env.xDR);
                    setSliderValue(obj, "SustainLevel", "sl:", 0, 127, param.Lfo.Env.xSL);
                    setSliderValue(obj, "SustainRate", "sr:", 0, 127, param.Lfo.Env.xSR);
                    setSliderValue(obj, "ReleaseRate", "rr:", 0, 127, param.Lfo.Env.xRR);
                }
                else
                {
                    setSliderValue(obj, "AttackRate", "ar:", 0, 31, param.Lfo.Env.AR);
                    setSliderValue(obj, "DecayRate", "dr:", 0, 31, param.Lfo.Env.DR);
                    setSliderValue(obj, "SustainLevel", "sl:", 0, 15, param.Lfo.Env.SL);
                    setSliderValue(obj, "SustainRate", "sr:", 0, 31, param.Lfo.Env.SR);
                    setSliderValue(obj, "ReleaseRate", "rr:", 0, 15, param.Lfo.Env.RR);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                int n = i;
                var obj = transform.Find("Panel/OP" + (i + 1)).gameObject;
                setSliderValue(obj, "WaveStyle", "ws:", 0, 7, param.Op[n].WS);
                setSliderValue(obj, "AMEnable", "ae:", 0, 1, param.Op[n].AE);
                setSliderValue(obj, "Multiple", "ml:", 0, 15, param.Op[n].Ml);
                setSliderValue(obj, "Detune", "dt:", 0, 7, param.Op[n].Dt);
                setSliderValue(obj, "KeyScale", "ks:", 0, 3, param.Op[n].Env.KS);
                setSliderValue(obj, "VelocitySense", "vs:", 0, 7, param.Op[n].Env.VS);
                setSliderValue(obj, "TotalLevel", "tl:", 0, 127, param.Op[n].Env.TL);
                if (param.Extended)
                {
                    setSliderValue(obj, "AttackRate", "ar:", 0, 127, param.Op[n].Env.xAR);
                    setSliderValue(obj, "DecayRate", "dr:", 0, 127, param.Op[n].Env.xDR);
                    setSliderValue(obj, "SustainLevel", "sl:", 0, 127, param.Op[n].Env.xSL);
                    setSliderValue(obj, "SustainRate", "sr:", 0, 127, param.Op[n].Env.xSR);
                    setSliderValue(obj, "ReleaseRate", "rr:", 0, 127, param.Op[n].Env.xRR);
                }
                else
                {
                    setSliderValue(obj, "AttackRate", "ar:", 0, 31, param.Op[n].Env.AR);
                    setSliderValue(obj, "DecayRate", "dr:", 0, 31, param.Op[n].Env.DR);
                    setSliderValue(obj, "SustainLevel", "sl:", 0, 15, param.Op[n].Env.SL);
                    setSliderValue(obj, "SustainRate", "sr:", 0, 31, param.Op[n].Env.SR);
                    setSliderValue(obj, "ReleaseRate", "rr:", 0, 15, param.Op[n].Env.RR);
                }
            }
            applyTone();
        }
        private void Awake()
        {
            syntheStation = GameObject.FindObjectOfType<MySyntheStation>();
            sampleTools = GameObject.FindObjectOfType<SampleTools>();
            mmlBox = sampleTools.GetComponent<MyMMLBox>();
            {
                Button extend = transform.Find("Panel/Extend").GetComponent<Button>();
                extend.onClick.AddListener(() =>
                {
                    param.Extended = !param.Extended;
                    loadTone(param);
                });
            }
            {
                inputField = transform.Find("Panel/InputField").GetComponent<InputField>();
                inputField.onEndEdit.AddListener((v) =>
                {
                    var tone = new ToneParamPM8();
                    if (tone.LoadFromString(v))
                    {
                        loadTone(tone);
                    }
                });

                var presets = GetComponentsInChildren<PresetButton>();
                foreach (var pb in presets)
                {
                    var btn = pb.GetComponent<Button>();
                    var tp = new ToneParamPM8(pb.ToneData);
                    btn.onClick.AddListener(() =>
                    {
                        loadTone(tp);
                        inputField.text = param.ToString();
                    });
                }
            }
            {
                var obj = transform.Find("Panel/FM").gameObject;
                setupSlider(obj, "Algorithm", "al:", (val) => { param.Al = (Byte)val; applyTone(); });
                setupSlider(obj, "Feedback", "fb:", (val) => { param.Fb = (Byte)val; applyTone(); });
                setupSlider(obj, "WaveForm", "lw:", (val) => { param.Lfo.WS = (Byte)val; applyTone(); });
                setupSlider(obj, "Frequency", "lf:", (val) => { param.Lfo.LF = (Byte)val; applyTone(); });
                setupSlider(obj, "PMPower", "lp:", (val) => { param.Lfo.LP = (Byte)val; applyTone(); });
                setupSlider(obj, "AMPower", "la:", (val) => { param.Lfo.LA = (Byte)val; applyTone(); });
                setupSlider(obj, "AttackRate", "ar:", (val) => { if (param.Extended) { param.Lfo.Env.xAR = (Byte)val; } else { param.Lfo.Env.AR = (Byte)val; }; applyTone(); });
                setupSlider(obj, "DecayRate", "dr:", (val) => { if (param.Extended) { param.Lfo.Env.xDR = (Byte)val; } else { param.Lfo.Env.DR = (Byte)val; }; applyTone(); });
                setupSlider(obj, "SustainLevel", "sl:", (val) => { if (param.Extended) { param.Lfo.Env.xSL = (Byte)val; } else { param.Lfo.Env.SL = (Byte)val; }; applyTone(); });
                setupSlider(obj, "SustainRate", "sr:", (val) => { if (param.Extended) { param.Lfo.Env.xSR = (Byte)val; } else { param.Lfo.Env.SR = (Byte)val; }; applyTone(); });
                setupSlider(obj, "ReleaseRate", "rr:", (val) => { if (param.Extended) { param.Lfo.Env.xRR = (Byte)val; } else { param.Lfo.Env.RR = (Byte)val; }; applyTone(); });
            }
            for (int i = 0; i < 4; i++)
            {
                int n = i;
                var obj = transform.Find("Panel/OP" + (i + 1)).gameObject;
                setupSlider(obj, "WaveStyle", "ws:", (val) => { param.Op[n].WS = (Byte)val; applyTone(); });
                setupSlider(obj, "AMEnable", "ae:", (val) => { param.Op[n].AE = (Byte)val; applyTone(); });
                setupSlider(obj, "Multiple", "ml:", (val) => { param.Op[n].Ml = (Byte)val; applyTone(); });
                setupSlider(obj, "Detune", "dt:", (val) => { param.Op[n].Dt = (Byte)val; applyTone(); });
                setupSlider(obj, "KeyScale", "ks:", (val) => { param.Op[n].Env.KS = (Byte)val; applyTone(); });
                setupSlider(obj, "VelocitySense", "vs:", (val) => { param.Op[n].Env.VS = (Byte)val; applyTone(); });
                setupSlider(obj, "TotalLevel", "tl:", (val) => { param.Op[n].Env.TL = (Byte)val; applyTone(); });
                setupSlider(obj, "AttackRate", "ar:", (val) => { if (param.Extended) { param.Op[n].Env.xAR = (Byte)val; } else { param.Op[n].Env.AR = (Byte)val; }; applyTone(); });
                setupSlider(obj, "DecayRate", "dr:", (val) => { if (param.Extended) { param.Op[n].Env.xDR = (Byte)val; } else { param.Op[n].Env.DR = (Byte)val; }; applyTone(); });
                setupSlider(obj, "SustainLevel", "sl:", (val) => { if (param.Extended) { param.Op[n].Env.xSL = (Byte)val; } else { param.Op[n].Env.SL = (Byte)val; }; applyTone(); });
                setupSlider(obj, "SustainRate", "sr:", (val) => { if (param.Extended) { param.Op[n].Env.xSR = (Byte)val; } else { param.Op[n].Env.SR = (Byte)val; }; applyTone(); });
                setupSlider(obj, "ReleaseRate", "rr:", (val) => { if (param.Extended) { param.Op[n].Env.xRR = (Byte)val; } else { param.Op[n].Env.RR = (Byte)val; }; applyTone(); });
            }
            {
                var keyboard = transform.Find("Panel/Keyboard").GetComponent<Keyboard>();
                keyboard.PortNo = portNo;
                keyboard.ChNo = chNo;
            }
            {
                var mmlEditorCanvas = transform.parent.Find("MMLPlayerCanvas");
                var button = transform.Find("Panel/MMLEditorButton").GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    mmlBox.Play("Click");
                    sampleTools.MoveCameraTo(mmlEditorCanvas.localPosition);
                });
            }
        }
        private void Start()
        {
            var test = "$@(Piano)=@pm8[4 5 op1[0 0 1 0 7 0 1 1 23 25 5 0 0 0] op2[6 1 1 0 7 0 1 1 0 21 10 8 9 5] op3[0 0 1 0 2 0 0 1 22 31 3 6 5 0] op4[0 1 1 0 3 0 0 1 3 31 12 10 8 7] lfo[0 127 0 0 31 0 0 0 0]]";
            loadTone(new ToneParamPM8(test));
        }
    }

}
