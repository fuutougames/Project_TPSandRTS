using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mono Behaviour Object Pool, not thread safe;
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoObjPool<T> where T : MonoBehaviour, IMonoPoolItem
{
    private T m_Template;

    private LinkedList<T> m_LinkArray; 
    private int m_MaxSize;


    public MonoObjPool(T template, int maxSize = 64)
    {
        if (maxSize < 8)
            maxSize = 8;
        m_MaxSize = maxSize;
        m_Template = template;
        m_LinkArray = new LinkedList<T>();
        m_Template.OnRetrun();
    }

    private T CreateNewItem()
    {
        T item = GameObject.Instantiate(m_Template.gameObject).GetComponent<T>();
        item.transform.SetParent(m_Template.transform.parent, false);
        return item;
    }

    public T Pop()
    {
        if (m_LinkArray.Count > 0)
        {
            T t = m_LinkArray.First.Value;
            m_LinkArray.RemoveFirst();
            t.OnGet();
            return t;
        }
        else
        {
            T t = CreateNewItem();
            t.OnGet();
            return t;
        }
    }

    public void Push(T t)
    {
        // out of capacity, destroy it immediately
        // Debug.Log("Pushing into pool: " + t.GetInstanceID());
        if (m_LinkArray.Count >= m_MaxSize)
        {
            t.OnRetrun();
            GameObject.DestroyImmediate(t.gameObject);
            return;
        }

        t.OnRetrun();
        m_LinkArray.AddLast(t);
    }
}
