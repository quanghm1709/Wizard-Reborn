using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private Transform player;

    public Tilemap theMap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private float halfHeight;
    private float halfWidth;

    private void Start()
    {
        instance = this;
        //halfHeight = Camera.main.orthographicSize;
        //halfWidth = halfHeight * Camera.main.aspect;

        //theMap.CompressBounds();
        //bottomLeftLimit = theMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
        //topRightLimit = theMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);
    }
    private void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
    }
    

    public void Shake(float shakeVibrato, float shakeTime, float shakeRandomness)
    {
        StartCoroutine(IEShake(shakeVibrato, shakeTime, shakeRandomness));
    }

    private IEnumerator IEShake(float shakeVibrato, float shakeTime, float shakeRandomness)
    {
        Vector3 currentPosition = transform.position;
        for (int i = 0; i < shakeVibrato; i++)
        {
            Vector3 shakePosition = currentPosition + Random.onUnitSphere * shakeRandomness;
            yield return new WaitForSeconds(shakeTime);
            transform.position = shakePosition;
        }
    }

    public void GetCurrentRoom(GameObject room)
    {
        theMap = room.GetComponent<Tilemap>();
    }
}
