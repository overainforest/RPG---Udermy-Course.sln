using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallaxEffect;//视差系数

    private float xPosition;
    private float length;

    private void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    private void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);//相机视差余量
        float distanceToMave = cam.transform.position.x * parallaxEffect;//相机视差
        //若parallaxEffect为1，则背景跟随摄象机移动，若小于1，则背景会慢于摄象机
        transform.position = new Vector3(xPosition + distanceToMave, transform.position.y);

        if (distanceMoved > xPosition + length)//视差余量一旦超过摄象机（已超出背景），将背景位置更新（右）
            xPosition = xPosition + length * 2;
        else if (distanceMoved < xPosition - length)//视差余量一旦超过摄象机，将背景位置更新（左）
            xPosition = xPosition - length * 2;
    }
}
