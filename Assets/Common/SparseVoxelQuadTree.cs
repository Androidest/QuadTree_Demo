using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.QuadTree
{
    /// <summary>
    /// Extends the Unity Rect class
    /// </summary>
    public static class Rect_Extension
    {
        /// <summary>
        /// test whether a rect contains the target rect 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Contains(this Rect rect, Rect target)
        {
            return (rect.xMin <= target.xMin && 
                    rect.xMax >= target.xMax &&
                    rect.yMin <= target.yMin &&
                    rect.yMax >= target.yMax);
        }
    }

    public class SparseVoxelQuadTree<T>
    {
        SparseVoxelQuadTree<T>[] ChildNodes;
        Rect[] ChildRects;
        public Rect Rect { get; private set; }
        public List<T> DataList { get; private set; }
        public List<Rect> DataRectList { get; private set; }
        public int Level { get; private set; }

        public SparseVoxelQuadTree(Rect rect, int maxDepth)
        {
            Rect = rect;
            ChildNodes = new SparseVoxelQuadTree<T>[4];
            DataList = new List<T>();
            DataRectList = new List<Rect>();
            Level = maxDepth; // the maximum depth of this tree, 0 means this is a leaf node

            if (Level > 0)
            {
                ChildRects = new Rect[4];
                float x = Rect.xMin;
                float y = Rect.yMin;
                float w = Rect.width * 0.5f;
                float h = Rect.height * 0.5f;
                ChildRects[0] = new Rect(x, y, w, h); // bottom left 
                ChildRects[1] = new Rect(x + w, y, w, h); // bottom right
                ChildRects[2] = new Rect(x, y + h, w, h); // top left
                ChildRects[3] = new Rect(x + w, y + h, w, h); // top right
            }
            else
            {
                ChildRects = new Rect[0];
            }
        }

        public void IteratePostOrder(Action<SparseVoxelQuadTree<T>> callback)
        {
            foreach (var child in ChildNodes)
            {
                child?.IteratePostOrder(callback);
            }
            callback(this);
        }

        // get all the data recursively from this tree
        public List<T> GetAllData()
        {
            var data = new List<T>();
            _GetAllData(data);
            return data;
        }

        private void _GetAllData(List<T> data)
        {
            foreach (var child in ChildNodes)
            {
                child?._GetAllData(data);
            }
            data.AddRange(DataList);
        }

        // ===================== for point data ==============================
        public void Insert(Vector2 point, T data)
        {
            if (!Rect.Contains(point))
            {
                Debug.LogError("Out of bound");
                return;
            }
            _Insert(point, data);
        }

        // recursive insert a point data
        private void _Insert(Vector2 point, T data)
        {
            if (Level != 0)
            {
                for (int i = 0; i < ChildRects.Length; ++i)
                {
                    if (ChildRects[i].Contains(point))
                    {
                        if (ChildNodes[i] == null)
                            ChildNodes[i] = new SparseVoxelQuadTree<T>(ChildRects[i], Level - 1);

                        ChildNodes[i]._Insert(point, data);
                        return;
                    }
                }
            }
            DataList.Add(data);
        }

        public SparseVoxelQuadTree<T> Search(Vector2 point)
        {
            if (!Rect.Contains(point))
            {
                return null;
            }
            return _Search(point);
        }

        // recursive search a node by a point
        private SparseVoxelQuadTree<T> _Search(Vector2 point)
        {
            if (Level != 0)
            {
                for (int i = 0; i < ChildRects.Length; ++i)
                {
                    if (ChildNodes[i] != null && ChildRects[i].Contains(point))
                    {
                        return ChildNodes[i].Search(point);
                    }
                }
            }
            return this;
        }

        // ========================= for rect data ==============================
        /// <summary>
        /// Inset an object with rectangle bounds
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="data"></param>
        public void Insert(Rect rect, T data)
        {
            if (!Rect.Contains(rect))
            {
                Debug.LogError("Out of bound");
                return;
            }
            _Insert(rect, data);
        }

        // recursive insert
        private void _Insert(Rect rect, T data)
        {
            if (Level != 0)
            {
                for (int i = 0; i < ChildRects.Length; ++i)
                {
                    if (ChildRects[i].Contains(rect))
                    {
                        // create a new quadrant node if it doesn't exist
                        if (ChildNodes[i] == null)
                            ChildNodes[i] = new SparseVoxelQuadTree<T>(ChildRects[i], Level - 1);

                        // if this child contains the entire rect, then insert the object recursively
                        ChildNodes[i]._Insert(rect, data);
                        return;
                    }
                }
            }

            // if Level == 0 or this node does not have children or no children contain this rect
            // then add the data to this node
            DataList.Add(data);
            DataRectList.Add(rect);
        }

        /// <summary>
        /// Search objects within a specified rectangle
        /// </summary>
        /// <param name="targetRect"></param>
        /// <returns></returns>
        public List<T> Search(Rect targetRect)
        {
            var data = new List<T>();
            if (Rect.Overlaps(targetRect))
                _Search(data, targetRect);
            return data;
        }

        // recursive search
        private void _Search(List<T> data, Rect targetRect)
        {
            // target rect doesn't contains this rect
            if (!targetRect.Contains(Rect))
            {
                // recursive search the overlaped subtrees
                foreach (var child in ChildNodes)
                {
                    if (child != null && child.Rect.Overlaps(targetRect))
                        child._Search(data, targetRect);
                }

                // test overlaped objects that belong to this node
                for (int i = 0; i < DataList.Count; ++i)
                {
                    if (DataRectList[i].Overlaps(targetRect))
                        data.Add(DataList[i]);
                }
                return;
            }
            // if target rect contains this rect, then get all the data of this entire subtree
            data.AddRange(GetAllData()); 
        }
    }
}
