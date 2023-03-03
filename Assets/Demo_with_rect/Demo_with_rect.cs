using Assets.Common;
using Assets.QuadTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// move the mouse to select circles within the rectangle
/// </summary>
public class Demo_with_rect : MonoBehaviour
{
    [SerializeField] float MapSize;
    [SerializeField] int ObjectCount;
    [SerializeField] GameObject Prefabs;

    private List<GameObject> Objects;
    private SparseVoxelQuadTree<(Rect, GameObject)> QuadTree;
    private Rect SelectionRect;
    private float Size;
    private List<(Rect, GameObject)> SelectedObjects;

    void Start()
    {
        float range = MapSize * 0.5f;
        float quadRange = range + 2f;

        // Create a QuadTree
        QuadTree = new SparseVoxelQuadTree<(Rect,GameObject)>(new Rect(-quadRange, -quadRange, quadRange * 2, quadRange * 2), 5);
        Objects = new List<GameObject>();
        SelectionRect = new Rect();
        SelectedObjects = new List<(Rect, GameObject)>();
        Size = 3;

        for (int i = 0; i < ObjectCount; ++i)
        {
            // randomly create some circles
            var obj = GameObject.Instantiate(Prefabs, transform);
            obj.transform.position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            obj.transform.localScale = Vector3.one * Random.Range(0.1f, 1.5f);
            obj.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
            Objects.Add(obj);

            // insert the circles with their rect into the QuadTree
            var bounds = obj.GetComponent<SpriteRenderer>().bounds;
            Rect rect = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
            QuadTree.Insert(rect, (rect,obj));
        }
    }

    void Update()
    {
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // set the position of the big red rectangle, and also size
        SelectionRect.Set(point.x, point.y, 3 * Size, 2 * Size);

        // search for the objects within this rectangle
        SelectedObjects = QuadTree.Search(SelectionRect);  
    }

#if UNITY_EDITOR
    // For visualization
    private void OnDrawGizmos()
    {
        if (QuadTree == null)
            return;

        // Draw the big red rectangle controlled by the mouse for spatial area selection.
        Helper.DrewRectGizmo(SelectionRect, Color.red);

        // visualize the nodes of the quad tree
        QuadTree.IteratePostOrder(node => Helper.DrewRectGizmo(node.Rect, Color.white));

        // Draw the bounds of the circles that are within the big red rectangle controlled by the mouse
        foreach(var (rect,_) in SelectedObjects)
        {
            Helper.DrewRectGizmo(rect, Color.red);
        }
    }
#endif
}
