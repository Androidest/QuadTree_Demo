using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCulling_Demo : MonoBehaviour
{
    [SerializeField] int ObjectCount = 100000;
    [SerializeField] GameObject Prefabs;
    List<GameObject> Objects;
    void Start()
    {
        Objects = new List<GameObject>();
        float range = 300f;
        for(int i = 0; i < ObjectCount; ++i)
        {
            var obj = GameObject.Instantiate(Prefabs, transform);
            obj.transform.position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            obj.transform.localScale = Vector3.one * Random.Range(0.1f, 1f);
            obj.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
            Objects.Add(obj);
        }
    }

    void Update()
    {
        
    }
}
