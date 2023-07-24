
using UnityEngine;
using UnityEngine.UI;
using MySpace;

namespace MySpace.Sample
{
    public class Keyboard : MonoBehaviour
    {
        private MySyntheStation syntheStation = null;

        [SerializeField]
        private KeyProperty blackKey = null;
        [SerializeField]
        private KeyProperty whiteKey = null;
        [SerializeField]
        private int baseNote = 60;

        public int BaseNote
        {
            get
            {
                return baseNote;
            }
            set
            {
                baseNote = value;
            }
        }
        public int PortNo
        {
            get;
            set;
        }
        public int ChNo
        {
            get;
            set;
        }

        private int numKeys;
        private int vel = 100;
        private int vol = 100;

        private void OnKeyDown(int index)
        {
            //UnityEngine.Debug.Log("key dw:" + (baseNote + index));
            syntheStation.Synthesizers[PortNo].Channel[ChNo].NoteOn((byte)(BaseNote + index), (byte)vel);
        }
        private void OnKeyUp(int index)
        {
            //UnityEngine.Debug.Log("key up:" + (baseNote + index));
            syntheStation.Synthesizers[PortNo].Channel[ChNo].NoteOff((byte)(BaseNote + index));
        }

        private void Awake()
        {
            syntheStation = GameObject.FindObjectOfType<MySyntheStation>();

            var p = GetComponent<RectTransform>();
            var r = whiteKey.GetComponent<RectTransform>();
            var width = r.rect.width;
            var basePos = whiteKey.transform.localPosition;
            int[] ofs = { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12 };
            numKeys = (int)((p.rect.width - (p.rect.width / 2 + (basePos.x - width / 2)) * 2) / width) * 12 / 7;

            for (int i = 0; i < numKeys; i++)
            {
                int o = ofs[i % 12];
                if ((o & 1) != 0)
                {
                    continue;
                }
                int index = i;
                var key = Instantiate(whiteKey);
                var pos = basePos;
                var scl = whiteKey.transform.localScale;
                var rot = whiteKey.transform.rotation;
                pos.x += ((i / 12) * 14 + o) * (width / 2);
                key.transform.SetParent(transform);
                key.transform.localPosition = pos;
                key.transform.localScale = scl;
                key.transform.rotation = rot;
                key.OnKeyDownEvent.AddListener(() => OnKeyDown(index));
                key.OnKeyUpEvent.AddListener(() => OnKeyUp(index));
            }
            for (int i = 0; i < numKeys; i++)
            {
                int o = ofs[i % 12];
                if ((o & 1) == 0)
                {
                    continue;
                }
                int index = i;
                var key = Instantiate(blackKey);
                var pos = basePos;
                var scl = blackKey.transform.localScale;
                var rot = blackKey.transform.rotation;
                pos.x += ((i / 12) * 14 + o) * (width / 2);
                key.transform.SetParent(transform);
                key.transform.localPosition = pos;
                key.transform.localScale = scl;
                key.transform.rotation = rot;
                key.OnKeyDownEvent.AddListener(() => OnKeyDown(index));
                key.OnKeyUpEvent.AddListener(() => OnKeyUp(index));
            }
            whiteKey.gameObject.SetActive(false);
            blackKey.gameObject.SetActive(false);

            var position = transform.Find("Position").GetComponent<Text>();
            var lshift = transform.Find("LShift").GetComponent<Button>();
            BaseNote -= BaseNote % 12;
            position.text = "^C" + (BaseNote / 12 - 2 + 1);
            lshift.onClick.AddListener(() =>
            {
                if (BaseNote - 12 >= 0)
                {
                    BaseNote -= 12;
                    position.text = "^C" + (BaseNote / 12 - 2 + 1);
                }
            });
            var rshift = transform.Find("RShift").GetComponent<Button>();
            rshift.onClick.AddListener(() =>
            {
                if (BaseNote + numKeys + 12 <= 128)
                {
                    BaseNote += 12;
                    position.text = "^C" + (BaseNote / 12 - 2 + 1);
                }
            });
            var velocity = transform.Find("Velocity").GetComponent<Slider>();
            velocity.onValueChanged.AddListener((value) =>
            {
                vel = (int)value;
                velocity.gameObject.transform.Find("Value").GetComponent<Text>().text = "" + vel;
            });

            var volume = transform.Find("Volume").GetComponent<Slider>();
            volume.onValueChanged.AddListener((value) =>
            {
                vol = (int)value;
                syntheStation.Synthesizers[PortNo].MasterVolume((byte)value);
                volume.gameObject.transform.Find("Value").GetComponent<Text>().text = "" + (byte)value;
            });

            var hold = transform.Find("Hold").GetComponent<KeyProperty>();
            hold.OnKeyDownEvent.AddListener(() => syntheStation.Synthesizers[PortNo].Channel[ChNo].Damper(+127));
            hold.OnKeyUpEvent.AddListener(() => syntheStation.Synthesizers[PortNo].Channel[ChNo].Damper(0));

            var damp = transform.Find("Damp").GetComponent<KeyProperty>();
            damp.OnKeyDownEvent.AddListener(() => syntheStation.Synthesizers[PortNo].Channel[ChNo].Damper(-127 + 256));
            damp.OnKeyUpEvent.AddListener(() => syntheStation.Synthesizers[PortNo].Channel[ChNo].Damper(0));
        }
        private void Start()
        {
            var velocity = transform.Find("Velocity").GetComponent<Slider>();
            velocity.value = vel;
            var volume = transform.Find("Volume").GetComponent<Slider>();
            volume.value = vol;
        }
    }

}
