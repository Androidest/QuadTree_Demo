using Assets.Common;
using Assets.QuadTree;
using System.Collections.Generic;
using UnityEngine;

public class Demo_with_point : MonoBehaviour
{
    [SerializeField] float MapSize;
    [SerializeField] int ObjectCount;
    [SerializeField] GameObject Prefabs;

    private List<GameObject> Objects;
    private SparseVoxelQuadTree<GameObject> QuadTree;
    private SparseVoxelQuadTree<GameObject> SelectedNode;

    void Start()
    {
        float range = MapSize * 0.5f;
        float quadRange = range + 2f;

        // Create a QuadTree
        QuadTree = new SparseVoxelQuadTree<GameObject>(new Rect(-quadRange, -quadRange, quadRange * 2, quadRange * 2), 5);
        Objects = new List<GameObject>();
        
        for(int i = 0; i < ObjectCount; ++i)
        {
            // randomly create some circles
            var obj = GameObject.Instantiate(Prefabs, transform);
            obj.transform.position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            obj.transform.localScale = Vector3.one * Random.Range(0.1f, 1f);
            obj.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
            Objects.Add(obj);

            // insert the circles with their center point into the QuadTree
            var point = obj.transform.position;
            QuadTree.Insert(point, obj);
        }
    }

    void Update()
    {
        // click to select a QuadTree's node
        if (Input.GetMouseButtonUp(0))
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // search a node by point
            SelectedNode = QuadTree.Search(point);
        }
    }

#if UNITY_EDITOR
    // For visualization
    private void OnDrawGizmos()
    {
        if (QuadTree == null)
            return;

        // visualize the nodes of the quad tree
        QuadTree.IteratePostOrder(node => Helper.DrewRectGizmo(node.Rect, Color.white));

        // Draw the selected QuadTree's node
        if (SelectedNode != null)
        {
            Helper.DrewRectGizmo(SelectedNode.Rect, Color.red);
        }
    }
#endif
}
