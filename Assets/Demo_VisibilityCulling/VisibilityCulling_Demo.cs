using Assets.QuadTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCulling_Demo : MonoBehaviour
{
    [SerializeField] float MapSize;
    [SerializeField] int ObjectCount;
    [SerializeField] GameObject Prefabs;
    List<GameObject> Objects;
    SparseVoxelQuadTree<GameObject> QuadTree;

    void Start()
    {
        float range = MapSize * 0.5f;

        QuadTree = new SparseVoxelQuadTree<GameObject>(new Rect(-range,-range, range * 2, range * 2), 5);
        Objects = new List<GameObject>();
        
        for(int i = 0; i < ObjectCount; ++i)
        {
            var obj = GameObject.Instantiate(Prefabs, transform);
            obj.transform.position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            obj.transform.localScale = Vector3.one * Random.Range(0.1f, 1f);
            obj.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
            Objects.Add(obj);

            var point = obj.transform.position;
            QuadTree.Insert(point, obj);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        QuadTree?.IteratePostOrder(node =>
        {
            // Draw the rectangle area for every quadrant
            var r = node.Rect;
            Gizmos.DrawLine(new Vector3(r.xMin, r.yMin, 0), new Vector3(r.xMax, r.yMin, 0));
            Gizmos.DrawLine(new Vector3(r.xMax, r.yMin, 0), new Vector3(r.xMax, r.yMax, 0));
            Gizmos.DrawLine(new Vector3(r.xMax, r.yMax, 0), new Vector3(r.xMin, r.yMax, 0));
            Gizmos.DrawLine(new Vector3(r.xMin, r.yMax, 0), new Vector3(r.xMin, r.yMin, 0));
        });
    }
    #endif

    void Update()
    {
        
    }
}
