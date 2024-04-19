using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaceOuterBorder : MonoBehaviour
{
    public GameObject objParent;
    public List<GameObject> outerBorderObjects = new();
    public Terrain terrain;
    public GameObject terrainGO;
    private float borderObjMinHoriSize = 40f;
    private float borderObjMaxHoriSize = 80f;
    private Vector3 borderObjMaxRot = new Vector3(10, 360, 10);


    private void Awake()
    {
        Vector3 terrainOrigin = terrainGO.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;
        //Debug.Log("Terrain size: " + terrainSize);

        for (int x = 0; x <= terrainSize.x; x += 20)
        {
            if (x == 0 || x == terrainSize.x)
            {
                for (int z = 0; z <= terrainSize.z; z += 20)
                {
                    HandleObject(x, z);                                        
                }
            }
            else
            {
                HandleObject(x, 0);
                HandleObject(x, (int)terrainSize.z);
            }
        }

        void HandleObject(int x, int z)
        {
            int nextObjIndex = Random.Range(0, outerBorderObjects.Count);
            GameObject nextPrefab = outerBorderObjects[nextObjIndex];
            Vector3 pos = terrainOrigin + new Vector3(x, 0, z);
            pos.y = terrain.SampleHeight(pos) + terrainOrigin.y;
            Quaternion rot = nextPrefab.transform.rotation;
            GameObject nextInstance = Instantiate(nextPrefab, pos, rot);
            RotAndScale(nextInstance);
            nextInstance.transform.parent = objParent.transform;
            MeshCollider coll = nextInstance.AddComponent<MeshCollider>();
            coll.convex = true;
        }

        void RotAndScale(GameObject GO)
        {
            // Rotate
            Vector3 newRot = new(Random.Range(0, borderObjMaxRot.x), Random.Range(0, borderObjMaxRot.y), Random.Range(0, borderObjMaxRot.z));
            GO.transform.Rotate(newRot);
            // Set size
            GO.transform.localScale = new(1,1,1);
            Vector3 size = GO.GetComponent<Renderer>().bounds.extents * 2;
            float minHoriSize = Mathf.Min(size.x, size.z);

            //Debug.Log("Size of " + GO.name  + " = " + minHoriSize);

            float scaleModifier = Random.Range(minHoriSize/borderObjMinHoriSize, minHoriSize/borderObjMaxHoriSize);
            GO.transform.localScale /= scaleModifier;
        }
    }

}
