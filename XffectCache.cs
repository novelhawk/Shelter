using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XffectCache : MonoBehaviour
{
    private readonly Dictionary<string, ArrayList> _objects = new Dictionary<string, ArrayList>();

    private Transform AddObject(string objectName)
    {
        Transform original = transform.Find(objectName);
        if (original == null)
            throw new NullReferenceException(nameof(original) + " is null @ " + nameof(XffectCache));
        
        Transform obj = Instantiate(original, Vector3.zero, Quaternion.identity) as Transform;
        if (obj == null)
            throw new NullReferenceException(nameof(obj) + " is null @ " + nameof(XffectCache));
        obj.gameObject.SetActive(false);
        _objects[objectName].Add(obj);
        
        Xffect component = obj.GetComponent<Xffect>();
        if (component != null)
            component.Initialize();
        return obj;
    }

    private void Awake()
    {
        foreach (Transform current in transform)
        {
            _objects[current.name] = new ArrayList
            {
                current
            };
            
            Xffect component = current.GetComponent<Xffect>();
            if (component != null)
                component.Initialize();

            current.gameObject.SetActive(false);
        }
    }

    public Transform GetObject(string name)
    {
        ArrayList list = this._objects[name];
        if (list == null)
        {
            Debug.LogError(name + ": cache doesnt exist!");
            return null;
        }

        foreach (Transform current in list)
        {
            if (current != null && !current.gameObject.activeInHierarchy)
            {
                current.gameObject.SetActive(true);
                return current;
            }
        }

        return this.AddObject(name);
    }

    public ArrayList GetObjectCache(string name)
    {
        ArrayList list = this._objects[name];
        if (list == null)
        {
            Debug.LogError(name + ": cache doesnt exist!");
            return null;
        }
        return list;
    }

    private void Start()
    {
    }
}

