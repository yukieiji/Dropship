using System.Collections.Generic;

using UnityEngine;

namespace Dropship.Unity;

public static class ObjectHeler
{
    public static void DisableCollider<T>(GameObject obj) where T : Collider2D
    {
        T comp = obj.GetComponent<T>();
        if (comp != null)
        {
            comp.enabled = false;
        }
    }

    public static void DestoryAll<T>(IEnumerable<T> items) where T : Object
    {
        if (items == null) { return; }
        foreach (T item in items)
        {
            Object.Destroy(item);
        }
    }
}
