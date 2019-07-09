using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NozomiModeToggle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueChanged(bool isOn)
    {
        GlobalControl.Instance.NozomiMode = isOn;
    }
}
