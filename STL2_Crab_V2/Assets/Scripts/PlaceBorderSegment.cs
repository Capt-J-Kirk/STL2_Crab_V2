using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBorderSegment : MonoBehaviour
{
    public bool runAgain = true;
    public GameObject objParent;
    public List<GameObject> borderObjects = new();
    public Terrain terrain;
    public GameObject terrainGO;
    private float borderObjMinHoriSize = 10f;
    private float borderObjMaxHoriSize = 15f;
    private Vector3 borderObjMaxRot = new Vector3(360, 360, 360);


    private int objectCount = 50;
    public GameObject startGO;
    public GameObject endGO;



    private void Awake()
    {
        if (!runAgain) return;
        Vector3 terrainOrigin = terrainGO.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 direction = endGO.transform.position - startGO.transform.position;

        for (float i=0; i<=objectCount; i++)
        {
            Vector3 nextPos = startGO.transform.position + (i/objectCount) * direction;
            HandleObject(nextPos.x, nextPos.z);
        }

        void HandleObject(float x, float z)
        {
            int nextObjIndex = Random.Range(0, borderObjects.Count);
            GameObject nextPrefab = borderObjects[nextObjIndex];
            Vector3 pos = new Vector3(x, 0, z);
            pos.y = terrain.SampleHeight(pos) + terrainOrigin.y;
            Quaternion rot = nextPrefab.transform.rotation;
            GameObject nextInstance = Instantiate(nextPrefab, pos, rot);
            RotAndScale(nextInstance);
            nextInstance.transform.parent = objParent.transform;
        }

        void RotAndScale(GameObject GO)
        {
            // Rotate
            Vector3 newRot = new(Random.Range(0, borderObjMaxRot.x), Random.Range(0, borderObjMaxRot.y), Random.Range(0, borderObjMaxRot.z));
            GO.transform.Rotate(newRot);
            // Set size
            GO.transform.localScale = new(1, 1, 1);
            Vector3 size = GO.GetComponent<Renderer>().bounds.extents * 2;
            float minHoriSize = Mathf.Min(size.x, size.z);

            //Debug.Log("Size of " + GO.name  + " = " + minHoriSize);

            float scaleModifier = Random.Range(minHoriSize / borderObjMinHoriSize, minHoriSize / borderObjMaxHoriSize);
            GO.transform.localScale /= scaleModifier;
        }
    }
}
