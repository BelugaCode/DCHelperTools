/* *** *** *

MIT License

Copyright (c) 2020 BelugaCorp Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

* *** *** */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DCHelperTools
{

    public class DC {

        public static float MapRadial01(float input, float centre, float radius)
        {
            return Cos01(Fmod(Mathf.Clamp01((input - (centre - radius)) / (2f * radius)) - 0.5f,1f)) * 0.5f + 0.5f;
        }

        public static float Mean(float[] input)
        {
            float mean = 0f;
            for (int i = 0; i < input.Length; i++)
            {
                mean += input[i];
            }
            mean /= input.Length;
            return mean;
        }

        public static float RMS(float[] input)
        {
            float rms = 0f;
            for(int i=0;i<input.Length;i++)
            {
                rms += input[i] * input[i];
            }
            rms /= input.Length;
            rms = Mathf.Sqrt(input.Length);
            return rms;
        }

        public static float WaveshapeDistort(float val, float amount)
        {
            float foo = Mathf.Clamp(amount, 0, 0.999f);
            foo = 2f * foo / (1 - foo);
            float valc = Mathf.Clamp(val, -1f, 1f);
            return (1f + foo) * valc / (1f + foo * Mathf.Abs(valc));
        }

        public static float InverseWaveshapeDistort(float val, float amount)
        {
            float foo = Mathf.Clamp(amount, 0, 0.999f);
            foo = 2f * foo / (1 - foo);
            float valc = Mathf.Clamp(val, -1f, 1f);
            bool sgn = valc >= 0;
            valc = sgn ? 1f - valc : -1f - valc;
            valc = (1f + foo) * valc / (1f + foo * Mathf.Abs(valc));
            return sgn ? 1f - valc : -1f - valc;
        }

        public static float Tanh(float val)
        {
            val = Mathf.Clamp(val, -3f, 3f);
            val = val * (27 + val * val) / (27 + 9 * val * val);
            return val;
        }

        public static float Map(float inValue, float inMin, float inMax, float outMin, float outMax)
        {
            return ((inValue - inMin) / (inMax - inMin)) * (outMax - outMin) + outMin;
        }

        public static float LinearToDecibel(float linear)
        {
            float dB;
            if (linear != 0)
                dB = 20.0f * Mathf.Log10(linear);
            else
                dB = -144.0f;
            return dB;
        }

        public static float DecibelToLinear(float dB)
        {
            float linear = Mathf.Pow(10.0f, dB / 20.0f);
            return linear;
        }

        public static float MidiToFrequency(float midinote) {
            return 440f * Mathf.Pow(2, (midinote - 69f) / 12f);
        }

        // taylor series expansion
        public static float Cos01(float val)
        {
            val = (Mathf.Abs(val - 0.5f) - 0.25f) * 6.28319f;
            float val_ = val - (val * val * val * 0.166667f);
            val = val_ + (val * val * val * val * val * 0.00784314f);
            return val;
        }

        public static float Fmod(float a, float b)
        {
            return (a - b * Mathf.Floor(a / b));
        }


        public static void ShuffleList<E>(IList<E> list)
        {
            if (list.Count > 1)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    E tmp = list[i];

                    int randomIndex = Random.Range(0, i + 1);

                    //Swap elements
                    list[i] = list[randomIndex];
                    list[randomIndex] = tmp;
                }
            }
        }

        public static void ShuffleIntArray(int[] array)
        {
            if (array.Length > 1)
            {
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    int tmp = array[i];

                    int randomIndex = Random.Range(0, i + 1);

                    //Swap elements
                    array[i] = array[randomIndex];
                    array[randomIndex] = tmp;
                }
            }
        }

    }

    public class Osc
    {
        public float freq = 1;
        public float phaseOffset = 0;
        private float phase = 0;
        private bool fixedTime = false;

        public void SetFixed(bool val)
        {
            fixedTime = val;
        }

        public float Update()
        {
            if (fixedTime)
                phase += freq * Time.fixedDeltaTime;
            else
                phase += freq * Time.deltaTime;
            if (phase > 1f)
            {
                phase -= 1f;// fmod(phase,1f);
            }
            return Sin(Fmod(phase + phaseOffset, 1f));
        }

        // taylor series expansion
        private float Sin(float val)
        {
            val = (Mathf.Abs(val - 0.5f) - 0.25f) * 6.28319f;
            float val_ = val - (val * val * val * 0.166667f);
            val = val_ + (val * val * val * val * val * 0.00784314f);
            return val;
        }

        private float Fmod(float a, float b)
        {
            return (a - b * Mathf.Floor(a / b));
        }

        public void ResetPhase(float newPhase)
        {
            phase = newPhase;
        }

    }

    public class Smooth
    {
        public float smoothFactor = 0.5f;
        private float valPrev = 0;

        public Smooth(float smoothFactor = 0.5f)
        {
            this.smoothFactor = smoothFactor;
        }

        public float Update(float val)
        {
            valPrev = val * (1f - smoothFactor) + valPrev * smoothFactor;
            return valPrev;
        }

        public void Init(float val)
        {
            valPrev = val;
        }

        public float GetLastValue()
        {
            return valPrev;
        }
    }

    public class Delta
    {
        float valPrev = 0;

        public float Update(float val)
        {
            float result = val - valPrev;
            valPrev = val;
            return result;
        }
    }

    public class Deadzone
    {
        public float threshold;  // value between 0-1

        int currentValue = 0;

        public Deadzone(float threshold=0.5f, int init=0) {
            this.threshold = threshold;
            currentValue = init;
        }

        public int Update(float val)
        {
            float dec = val - currentValue;
            currentValue = (int) ( dec > 0 ? (dec > threshold ? Mathf.Round(val) : currentValue) : (dec < -threshold ? Mathf.Round(val) : currentValue) );
            return currentValue;
        }

        public float UpdateMagnetic(float val, float attraction)
        {
            float dec = val - currentValue;
            float lerp = dec >= 0 ? dec / threshold : dec / -threshold;
            lerp = DC.InverseWaveshapeDistort(lerp,attraction) * dec + currentValue;

            currentValue = (int)(dec > 0 ? (dec > threshold ? Mathf.Round(val) : currentValue) : (dec < -threshold ? Mathf.Round(val) : currentValue));

            return lerp;
        }
    }

    public class TemporalHysteresis
    {
        public float timeThreshold = 0.3f;

        private int activeValue = 0;
        private float activeTime = 0f;
        private float deviationTime = 0f;

        public TemporalHysteresis(float timeThreshold, int init=0)
        {
            this.timeThreshold = timeThreshold;
            activeValue = init;
        }

        public int Update(int value)
        {
            if (value == activeValue)
            {
                activeTime += Time.deltaTime;
                deviationTime = 0f;
            }
            else
            {
                deviationTime += Time.deltaTime;
                if (deviationTime >= timeThreshold)
                {
                    activeValue = value;
                    deviationTime = 0f;
                    activeTime = 0f;
                }
            }
            return activeValue;
        }

        public float GetActiveTime()
        {
            return activeTime;
        }
    }

    public class LowPassFilter
    {
        public float cutoff;
        public float resonance;

        const float CUTOFF_MAX = 128.0f;
        const float CUTOFF_MIN = 0.0f;
        const float RESONANCE_MAX = 128.0f;
        const float RESONANCE_MIN = 0.0f;

        float c;
        float r;
        float v0;
        float v1;

        int sampleRate;

        public LowPassFilter(float sampleRate)
        {
            cutoff = 107.0f;
            resonance = 0.0f;

            c = 0.0f;
            r = 0.0f;
            v0 = 0.0f;
            v1 = 0.0f;
        }

        public float Process(float input)
        {
            cutoff = Mathf.Clamp(cutoff, CUTOFF_MIN, CUTOFF_MAX);
            resonance = Mathf.Clamp(resonance, RESONANCE_MIN, RESONANCE_MAX);

            c = Mathf.Pow(0.5f, (128.0f - cutoff) / 16.0f);
            r = Mathf.Pow(0.5f, (resonance + 24.0f) / 16.0f);
            v0 = ((1.0f - r * c) * v0) - (c * v1) + (c * input);
            v1 = ((1.0f - r * c) * v1) + (c * v0);

            return Mathf.Clamp(v1, -1.0f, 1.0f);
        }
        
    }

    public class PseudoSine
    {
        public float frequency = 1;
        private float phase = 0;

        private float isr = 0f;
        private float sr;

        public PseudoSine(float sampleRate)
        {
            sr = sampleRate;
            isr = 1f / sr;
        }

        public float Process()
        {
            phase += frequency * isr;
            phase = Fmod(phase, 1f);
            return Sin(phase);
        }

        // taylor series expansion
        private float Sin(float val)
        {
            val = (Mathf.Abs(val - 0.5f) - 0.25f) * 6.28319f;
            float val_ = val - (val * val * val * 0.166667f);
            val = val_ + (val * val * val * val * val * 0.00784314f);
            return val;
        }

        private float Fmod(float a, float b)
        {
            return (a - b * Mathf.Floor(a / b));
        }
    }

    public class Polyblep
    {
        public float frequency = 333f;

        private float isr = 0f;
        private float sr;

        private float phase = 0f;

        public Polyblep(float sampleRate)
        {
            sr = sampleRate;
            isr = 1f / sr;
        }

        public float Process()
        {
            float freqStep = frequency * this.isr;
            
            phase += freqStep;
            if (phase > 1f)
                phase -= 1f;

            float saw = phase * 2f - 1f;

            float a = 0f;
            if (phase < freqStep)
            {
                a = phase / freqStep;
                a = (a + a) - (a * a) - 1f;
            }
            else if (phase > (1f - freqStep))
            {
                a = (phase - 1f) / freqStep;
                a = (a + a) + (a * a) + 1f;
            }

            return saw - a;
            
        }
    }

    public class Envelope
    {
        public float attack = 0.1f;
        public float release = 0.1f;

        private float isr = 0f;
        private float sr;

        private float time = 0f;

        private bool noteOffTriggered = true;
        private float valueAtNoteOff = 1f;
        private float timeAtNoteOff = 0f;
        private float valueAtNoteOn = 0f;

        private bool useDeltaTime = false;

        public Envelope(float sampleRate, bool useDeltaTime=false)
        {
            sr = sampleRate;
            isr = 1f / sr;
            time = attack + release;
            this.useDeltaTime = useDeltaTime;
        }

        public float Process()
        {
            float delta = (useDeltaTime ? Time.deltaTime : isr);
            time = noteOffTriggered ? (time < attack+release? time+delta : attack + release) : (time < attack) ? time+delta : attack;

            float output = 0f;

            if (!noteOffTriggered)
            {
                output = (time <= attack) ? valueAtNoteOn + (1f-valueAtNoteOn)*(time / attack) : 1f;
                valueAtNoteOff = output;
                timeAtNoteOff = time;
            } else
            {
                output = (time < timeAtNoteOff + release ? valueAtNoteOff - ((time - timeAtNoteOff) / release) * valueAtNoteOff : 0f);
                valueAtNoteOn = output;
            }
            
            output *= output;

            return output;
        }

        public void Trigger()
        {
            time = 0f;
            noteOffTriggered = true;
        }

        public void On()
        {
            time = 0f;
            noteOffTriggered = false;
        }

        public void Off()
        {
            noteOffTriggered = true;
        }
    }

    public class Tick
    {
        public float bpm;
        public float ticksPerBeat;

        private float isr = 0f;
        private float sr;

        private float time = 0f;

        public UnityEvent m_Tick = new UnityEvent();
        public UnityEvent m_Start = new UnityEvent();

        public Tick(float sampleRate, float bpm=120, float ticksPerBeat=1f)
        {
            sr = sampleRate;
            isr = 1f / sr;
            this.bpm = bpm;
            this.ticksPerBeat = ticksPerBeat;
            m_Start.Invoke();
        }

        public float Process()
        {
            float cycle = (60f / bpm) / ticksPerBeat;

            time += isr;

            if (time >= cycle)
            {
                time -= cycle;
                m_Tick.Invoke();
            }

            return time;
        }

    }

}