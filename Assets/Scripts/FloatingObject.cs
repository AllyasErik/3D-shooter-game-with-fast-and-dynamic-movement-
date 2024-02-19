using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(startPos.x, startPos.y + Mathf.Sin(Time.time * frequency) * amplitude, startPos.z);
    }
}
