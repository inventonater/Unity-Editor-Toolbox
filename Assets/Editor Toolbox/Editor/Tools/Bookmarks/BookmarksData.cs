using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    [FilePath("Library/bookmarks.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BookmarksData : ScriptableSingleton<BookmarksData>
    {
        public event Action OnChanged = delegate { };
        [SerializeField] private List<Object> bookmarks = new List<Object>();
        public List<Object> Bookmarks => bookmarks;

        public void Add(Object obj)
        {
            if (obj == null) return;
            if (bookmarks.Contains(obj)) bookmarks.Remove(obj);
            bookmarks.Add(obj);
            OnChanged?.Invoke();
        }

        public void Remove(Object obj)
        {
            bookmarks.Remove(obj);
            OnChanged?.Invoke();
        }
    }
}
