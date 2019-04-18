using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a line renderer visualization of an AudioSource
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class AudioLine : MonoBehaviour {

    public AudioSource aSource;

    public LineRenderer lineRenderer;

    //this must be a power of 2!!!
    public int numLinePoints = 128;
    public Transform lineStartLocation;
    public float lineSpacing = 1f;
    public float audioScaling = 10f;

    public float linearSmoothing = 0.1f;

    public float[] spectrumData;
    
    // Start is called before the first frame update
    void Start() {
        spectrumData = new float[numLinePoints];
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numLinePoints;
        for (int i = 0; i < numLinePoints; i++) {
            Vector3 pointPosition = new Vector3(
                lineStartLocation.position.x + (i*lineSpacing),
                0,
                0
                );
            lineRenderer.SetPosition(i, pointPosition);
        }
    }

    // Update is called once per frame
    void Update() {
        aSource.GetOutputData(spectrumData, 0);
        
        for (int i = 0; i < numLinePoints; i++) {

            //lerp between the old position and the new position via the linear smoothing
            float newYPosition = Mathf.Lerp(
                lineRenderer.GetPosition(i).y,
                spectrumData[i] * audioScaling,
                linearSmoothing
            ); 
            
            Vector3 pointPosition = new Vector3(
                lineStartLocation.position.x + (i*lineSpacing),
                lineStartLocation.position.y + newYPosition,
                lineStartLocation.position.z
            );
            
            lineRenderer.SetPosition(i, pointPosition);

            
        }
        
    }
}
