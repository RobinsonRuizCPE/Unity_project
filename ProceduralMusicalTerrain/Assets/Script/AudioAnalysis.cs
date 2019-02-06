using UnityEngine;
using UnityEngine.Audio;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class AudioAnalysis : MonoBehaviour
{

    public TerrainProcedural Terrain;
    public float ampli = 20f;
    [Range(0, 10)] public int delayTerrainUpdate = 0;

    private int frame = 0;

    private float[] spectrum;
    private int numSample = 256;
    private float totCoeff;

    void DebugDisplay()
    {
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] * 20 + 10, 0), new Vector3(i, spectrum[i + 1] * 20 + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) * 20 + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) * 20 + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] * 20 - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] * 20 - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]) * 20, 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]) * 20, 3), Color.blue);
        }
    }

    Vector4 SpectrumCoefficients()
    {
        Vector4 specCoef = new Vector4();

        int part = spectrum.Length / 4;
        
        for(int i = 1; i < part; i++)
        {
            specCoef.x += spectrum[i];
        }
        for (int i = part; i < 2*part; i++)
        {
            specCoef.y += spectrum[i];
        }
        for (int i = 2*part; i < 3*part; i++)
        {
            specCoef.z += spectrum[i];
        }
        for (int i = 3*part; i < 4*part; i++)
        {
            specCoef.w += spectrum[i];
        }
        
        return specCoef;
    }

    void UpdateTerrainCoeff()
    {
        Vector4 specCoeff = SpectrumCoefficients();

        totCoeff = specCoeff.x + specCoeff.y + specCoeff.z + specCoeff.w;

        Terrain.ampliOctave0 = specCoeff.x * ampli; // * (specCoeff.x/totCoeff);
        Terrain.ampliOctave1 = specCoeff.y * ampli; // * ((specCoeff.y )/ totCoeff);
        Terrain.ampliOctave2 = specCoeff.z * ampli; // * (specCoeff.z / totCoeff);

        /*
        Terrain.scaleOctave0 = Mathf.PingPong(Time.time,5f);
        Terrain.scaleOctave1 = Random.Range(10f, 40f);
        Terrain.scaleOctave2 = Random.Range(60f, 100f);
        */
        /*
        Terrain.scaleOctave0 = Random.Range(0f,5f);
        Terrain.scaleOctave1 = Random.Range(10f, 40f);
        Terrain.scaleOctave2 = Random.Range(60f, 100f);
        */
    }
    /*
    IEnumerator UpdateTerrainDelayed()
    {
        spectrum = new float[numSample];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        UpdateTerrainCoeff();

        yield return new WaitForSeconds(delayTerrainUpdate);
    }
    */

    void UpdateTerrainDelayed()
    {
        spectrum = new float[numSample];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        UpdateTerrainCoeff();

    }

    void Update()
    {
        if ((float)Time.frameCount % delayTerrainUpdate == 0 || delayTerrainUpdate == 0)
        {
            UpdateTerrainDelayed();
        }
        //StartCoroutine("UpdateTerrainDelayed");
    }
}
