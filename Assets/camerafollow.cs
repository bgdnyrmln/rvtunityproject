using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollow : MonoBehaviour
{

    public float FollowSpeed = 2f;
    public float yoffset = 1;
    public Transform  target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y + yoffset,-10f);
        transform.position = Vector3.Lerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
    }
}
