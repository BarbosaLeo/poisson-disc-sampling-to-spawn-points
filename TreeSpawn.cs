using System.Collections.Generic;
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
        // Here I have some math that I don't know how to explain in words. It's just to translate the points correcly to fit the chunk size.
        // Basically, the first terrainchunck V3(0,0,0) is created from the center to the borders, so points will have positive and negative values along the mesh.
        // The poisson disc is also created from the center but the sample area expands just at positive values.
        // That means the points location will never have negative values, so it populates only the positive quarter of the chunk.
        // I did this to solve that problem.
        float dstX = ((transform.position.x - point.x) + terrain.meshSettings.meshWorldSize / 2);
        float dstY = ((transform.position.z - point.y) + terrain.meshSettings.meshWorldSize / 2);

        //we create a new variable Vec2 to set the location properly.
        point = new Vector2(dstX, dstY);

        Vector3 raycastPos = new Vector3(point.x, castHeight, point.y);
        Ray ray = new Ray(raycastPos, Vector3.down);
        RaycastHit newPoint;
        LayerMask mask = LayerMask.GetMask("TerrainChunk");

        //Check if we hit the ground.
        if (Physics.Raycast(ray, out newPoint, castHeight+1, mask))
        {
            Debug.DrawLine(ray.origin, newPoint.point, UnityEngine.Color.green);
            
            //Here we check if the hit point height (Y Axis) is within the range that we set AND if the list does not contain the current point that we are checking.
            if (newPoint.point.y > shoreStartHeight && newPoint.point.y < mountainsStartHeight && !trees.Contains(newPoint.point))
            {
                //Add a tree element to the object list passing the current point and instantiate the gameObject.
                trees.Add(newPoint.point);
                GameObject tree = Instantiate(prefab, newPoint.point, Quaternion.identity);
                tree.transform.parent = transform;
                tree.transform.tag = "Resource";
                tree.layer = 9;
            }
        }
        //i dont know why but this is here and it works.
        else
        {
        }
    }
}
