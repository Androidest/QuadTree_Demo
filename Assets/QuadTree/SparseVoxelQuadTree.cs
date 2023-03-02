using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.QuadTree
{
    public class SparseVoxelQuadTree<T>
    {
        public Rect Rect { get; private set; }
        public List<T> DataList { get; private set; }
        public int Level { get; private set; }

        Rect[] ChildRects;
        SparseVoxelQuadTree<T>[] ChildNodes;

        public SparseVoxelQuadTree(Rect rect, int maxDepth)
        {
            Rect = rect;
            ChildRects = new Rect[4];
            ChildNodes = new SparseVoxelQuadTree<T>[4];
            DataList = new List<T>();
            Level = maxDepth;

            if (Level > 0)
            {
                float x = Rect.xMin;
                float y = Rect.yMin;
                float w = Rect.width * 0.5f;
                float h = Rect.height * 0.5f;
                ChildRects[0] = new Rect(x, y, w, h); // bottom left 
                ChildRects[1] = new Rect(x + w, y, w, h); // bottom right
                ChildRects[2] = new Rect(x, y + h, w, h); // top left
                ChildRects[3] = new Rect(x + w, y + h, w, h); // top right
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

        public void Insert(Vector2 point, T data)
        {
            if (!Rect.Contains(point))
            {
                Debug.LogError("Out of bound");
                return;
            }
            _Insert(point, data);
        }

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
    }
}
