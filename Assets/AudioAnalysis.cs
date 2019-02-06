using UnityEngine;
using UnityEngine.Audio;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class AudioAnalysis : MonoBehaviour
{

    //Microphone Input
    public bool _useMicrophoneInput;
    public AudioClip _audioClip;
    [HideInInspector] public string _microphoneDevice;
    public AudioMixerGroup _audioMixerGroupMicrophone, _audioMixerGroupMaster;

    public TerrainProcedural _Terrain;
    public float _ampli = 1.0f;
    private float[] _freqBand = new float[8];
    
    // frame skiping update
    [Range(0, 10)] public int _delayTerrainUpdate = 10;
    private int _frame = 0;

    private AudioSource _audioSource;
    public static float[] band_buffer = new float[8];
    float[] bufferDecrease = new float[8];
    float[] highestFreq = new float[8];
    private float[] _spectrum;
    private int _numSample = 512;
    private float _totCoeff;
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    private void BBuffer()
    {
        for (int g = 0; g < 8; g++)
        {
            if (_freqBand[g] > band_buffer[g])
            {
                band_buffer[g] = _freqBand[g];
                bufferDecrease[g] = 0.0005f;
            }
            if (_freqBand[g] < band_buffer[g])
            {
                band_buffer[g] -= bufferDecrease[g];
                bufferDecrease[g] *= 1.2f;
            }
        }
    }


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if(_useMicrophoneInput)
        {
            // Test if any microphone device available
            if(Microphone.devices.Length > 0)
            {
                //disable playing microphone feedback via audioMixer
                _audioSource.outputAudioMixerGroup = _audioMixerGroupMicrophone;


                _microphoneDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);


            }
            else
            {
                _useMicrophoneInput = false;
            }

        }
        if(!_useMicrophoneInput)
        {
            _audioSource.clip = _audioClip;
            _audioSource.outputAudioMixerGroup = _audioMixerGroupMaster;
        }
        _audioSource.Play();
        float[] starter = SpectrumCoefficients();
        BBuffer();
    }

    void DebugDisplay()
    {
        for (int i = 1; i < _spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, _spectrum[i] * 20 + 10, 0), new Vector3(i, _spectrum[i + 1] * 20 + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(_spectrum[i - 1]) * 20 + 10, 2), new Vector3(i, Mathf.Log(_spectrum[i]) * 20 + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), _spectrum[i - 1] * 20 - 10, 1), new Vector3(Mathf.Log(i), _spectrum[i] * 20 - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(_spectrum[i - 1]) * 20, 3), new Vector3(Mathf.Log(i), Mathf.Log(_spectrum[i]) * 20, 3), Color.blue);
        }
    }

    float[] SpectrumCoefficients()
    {
        float[] _freqBand = new float[8];
        int count = 0;
        for (int k = 0; k < 8; k++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, k) * 2;
            if (k == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += _spectrum[j] * (count + 1);
                count++;
            }
            average /= count;
            _freqBand[k] = average ;

        }
        return _freqBand;
    }

    void UpdateTerrainCoeff()
    {
        BBuffer();
        float[] specCoef = SpectrumCoefficients();
        int part = _spectrum.Length / 4;
        /*_totCoeff = specCoeff.x + specCoeff.y + specCoeff.z + specCoeff.w;
        for (int i = 1; i < part; i++)
        {

            specCoef.x += _spectrum[i];
        }
        for (int i = part; i < 2 * part; i++)
        {
            specCoef.y += _spectrum[i];
        }
        for (int i = 2 * part; i < 3 * part; i++)
        {
            specCoef.z += _spectrum[i];
        }
        for (int i = 3 * part; i < 4 * part; i++)
        {
            specCoef.w += _spectrum[i];
        }
    }*/
        for (int k = 0; k < 8; k++)
        {
            _Terrain.perlin_power.SetValue(specCoef[k] * _ampli*8/(k*2+5), k);
        }
       // _Terrain.perlin_power.SetValue(specCoef[1] * _ampli, 1);
       // _Terrain.perlin_power.SetValue(specCoef[2] * _ampli, 2);// * (specCoeff.x/totCoeff);
      //  _Terrain.perlin_power[1] = specCoef.y * _ampli; // * ((specCoeff.y )/ totCoeff);
      //  _Terrain.perlin_power[2] = specCoef.z * _ampli; // * (specCoeff.z / totCoeff);

        
    }

    void UpdateTerrainDelayed()
    {
        _spectrum = new float[_numSample];
        _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.Hanning);

        UpdateTerrainCoeff();

    }

    void Update()
    {
        if ((float)Time.frameCount % _delayTerrainUpdate == 0 || _delayTerrainUpdate == 0)
        {
            UpdateTerrainDelayed();
        }
        //StartCoroutine("UpdateTerrainDelayed");
    }
}
