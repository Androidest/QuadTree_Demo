using Assets.Common;
using Assets.QuadTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo_with_image : MonoBehaviour
{
    [SerializeField] Texture2D[] Images;
    [SerializeField] RawImage LeftImage;
    [SerializeField] RawImage RightImage;

    [SerializeField] Dropdown ImageDropdown;
    [SerializeField] InputField Threshold;
    [SerializeField] Toggle HasBorder;
    [SerializeField] Dropdown ColorDropdown;

    Color[] BorderColors = new Color[]{ Color.black, Color.red, Color.white };

    Texture2D Image;
    Texture2D ResultImage;
    private SparseVoxelQuadTree<Color> quadTree;


    void Start()
    {
        // UI
        ImageDropdown.onValueChanged.AddListener((val)=>MergeImagePixels());
        Threshold.onEndEdit.AddListener((val)=>MergeImagePixels());
        HasBorder.onValueChanged.AddListener((val)=>MergeImagePixels());
        ColorDropdown.onValueChanged.AddListener((val)=>MergeImagePixels());

        MergeImagePixels();
    }

    void MergeImagePixels()
    {
        if (string.IsNullOrEmpty(Threshold.text))
            Threshold.text = "0";

        // parameters
        float threshold = float.Parse(Threshold.text);
        bool hasBorder = HasBorder.isOn;
        Color borderColor = BorderColors[ColorDropdown.value];
        Image = Images[ImageDropdown.value];

        // Create a QuadTree
        var w = Image.width;
        var h = Image.height;
        int maxDepth = 0;
        float minSize = Mathf.Min(w, h);
        while (minSize > 1)
        {
            minSize *= 0.5f;
            ++maxDepth;
        }
        quadTree = new SparseVoxelQuadTree<Color>(new Rect(0, 0, w, h), maxDepth);

        // insert pixels to the QuadTree along with a merge method
        var pixels = Image.GetPixels();
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                quadTree.Insert(new Vector2(x, y), pixels[x + y * w], pixels => OnMergePixels(pixels, threshold));
            }
        }

        // After merging, draw all of the leaf nodes from the QuadTree to the result image
        quadTree.IterateLeafNode(node =>
        {
            DrawRectOnImage(pixels, w, node.Rect, node.DataList[0], borderColor, hasBorder);
        });

        ResultImage = new Texture2D(Image.width, Image.height);
        ResultImage.filterMode = FilterMode.Point;
        ResultImage.SetPixels(pixels);
        ResultImage.Apply();

        // Render the images 
        LeftImage.texture = Image;
        RightImage.texture = ResultImage;
    }

    // Merge method: thresholded average variance of color distance
    void OnMergePixels(List<Color> list, float threshold)
    {
        Color sum = new Color(0,0,0,0);
        foreach(var c in list)
            sum += c;

        var mean = sum / list.Count;
        foreach (var c in list)
        {
            var dir = mean - c;
            var dist = dir.r * dir.r + dir.g * dir.g + dir.b * dir.b;
            if (Mathf.Sqrt(dist) > threshold)
                return;
        }

        list.Clear();
        list.Add(mean);
    }

    // draw a colored rect on an image in pixel array format
    void DrawRectOnImage(Color[] image, int imWidth, Rect rect, Color color, Color borderColor, bool hasBorder)
    {
        var h = (int)(rect.height + rect.y);
        var w = (int)(rect.width + rect.x);
        var w_1 = w - 1;
        var h_1 = h - 1;
        for (int y = (int)rect.y; y < h; ++y)
        {
            for (int x = (int)rect.x; x < w; ++x)
            {
                bool isBorder = hasBorder && (x == 0 || x == w_1 || y == 0 || y == h_1);
                image[x + y * imWidth] = !isBorder ? color : borderColor;
            }
        }
    }
}
