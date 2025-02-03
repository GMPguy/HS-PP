using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryController : MonoBehaviour {
    
    public bool Won;

    public GameObject[] Flags;
    float flagCapture = 1f;
    bool flagCaptured;

    void Update () {

        if (!Won)
            return;

        // Capture flag
        if (flagCapture > -1f) {
            flagCapture -= Time.deltaTime / 6f;
            Flags[0].transform.parent.localPosition = Vector3.up * (-1f + Mathf.Abs(flagCapture) * 1.6f);

            if (!flagCaptured && flagCapture < 0f) {
                flagCaptured = true;
                Flags[0].SetActive(false);
                Flags[1].SetActive(true);
            }
        }
        
    }

}
