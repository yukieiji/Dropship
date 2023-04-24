using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Il2CppObject = Il2CppSystem.Object;
using Il2CppCollection = Il2CppSystem.Collections.Generic;

// from TOR : https://github.com/Eisbison/TheOtherRoles/blob/main/TheOtherRoles/Utilities/MapUtilities.cs

namespace Dropship.Il2Cpp.Extension;

public static class Il2CppEnumeratorExtension
{
    public static IEnumerable<T> GetFastEnumerator<T>(
        this Il2CppCollection.List<T> list) where T : Il2CppObject
    {
        return new Il2CppListEnumerable<T>(list);
    }
}

public unsafe class Il2CppListEnumerable<T> : 
    IEnumerable<T>, 
    IEnumerator<T> where T : Il2CppObject
{
    private struct Il2CppListStruct
    {
#pragma warning disable CS0169
        private IntPtr _unusedPtr1;
        private IntPtr _unusedPtr2;
#pragma warning restore CS0169

#pragma warning disable CS0649
        public IntPtr _items;
        public int _size;
#pragma warning restore CS0649
    }

    private static readonly int _elemSize;
    private static readonly int _offset;
    private static Func<IntPtr, T> _objFactory;

    static Il2CppListEnumerable()
    {
        _elemSize = IntPtr.Size;
        _offset = 4 * IntPtr.Size;

        var constructor = typeof(T).GetConstructor(new[] { typeof(IntPtr) });
        var ptr = Expression.Parameter(typeof(IntPtr));
        var create = Expression.New(constructor!, ptr);
        var lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
        _objFactory = lambda.Compile();
    }

    private readonly IntPtr _arrayPointer;
    private readonly int _count;
    private int _index = -1;

    public Il2CppListEnumerable(Il2CppCollection.List<T> list)
    {
        var listStruct = (Il2CppListStruct*)list.Pointer;
        _count = listStruct->_size;
        _arrayPointer = listStruct->_items;
    }

    object IEnumerator.Current => Current;
    public T Current { get; private set; }

    public bool MoveNext()
    {
        if (++_index >= _count) return false;
        var refPtr = *(IntPtr*)IntPtr.Add(IntPtr.Add(_arrayPointer, _offset), _index * _elemSize);
        Current = _objFactory(refPtr);
        return true;
    }

    public void Reset()
    {
        _index = -1;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }

    public void Dispose()
    {
    }
}
