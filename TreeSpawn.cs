﻿using System.Collections.Generic;
using UnityEngine;

public class TreeSpawn : MonoBehaviour
{
    TerrainGenerator terrain;
    public Vector2 castRegionSize = Vector2.one;
    public float radius = 18;
    public int rejectionSamples = 30;
    public float shoreStartHeight = 3f;
    public float mountainsStartHeight = 4f;
    public float castHeight = 100;

    public GameObject prefab;

    List<Vector2> points;
    List<Vector3> trees = new List<Vector3>();

    void Start()
    {
        terrain = GameObject.FindObjectOfType<TerrainGenerator>();
        castRegionSize *= terrain.meshSettings.meshWorldSize;

        points = SpawnPoints.GeneratePoints(radius, castRegionSize, transform.localPosition, rejectionSamples);
    }

    void Update()
    {
        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                CastRay(point);
            }
        }
    }

    public void CastRay(Vector2 point)
    {
        float dstX = ((transform.position.x - point.x) + terrain.meshSettings.meshWorldSize / 2);
        float dstY = ((transform.position.z - point.y) + terrain.meshSettings.meshWorldSize / 2);

        point = new Vector2(dstX, dstY);

        Vector3 raycastPos = new Vector3(point.x, castHeight, point.y);
        Ray ray = new Ray(raycastPos, Vector3.down);
        RaycastHit newPoint;
        LayerMask mask = LayerMask.GetMask("TerrainChunk");

        if (Physics.Raycast(ray, out newPoint, castHeight+1, mask))
        {
            Debug.DrawLine(ray.origin, newPoint.point, UnityEngine.Color.green);
            
            if (newPoint.point.y > shoreStartHeight && newPoint.point.y < mountainsStartHeight && !trees.Contains(newPoint.point))
            {
                trees.Add(newPoint.point);
                GameObject tree = Instantiate(prefab, newPoint.point, Quaternion.identity);
                tree.transform.parent = transform;
                tree.transform.tag = "Resource";
                tree.layer = 9;
            }
        }
        else
        {
        }
    }
}
