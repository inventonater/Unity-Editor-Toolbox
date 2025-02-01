using System.Collections.Generic;
using System.Collections.Specialized;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;


public class ObservableCollectionSample : MonoBehaviour
{
    public Button prefab;
    public GameObject root;
    ObservableRingBuffer<int> collection;
    ISynchronizedView<int, GameObject> view;

    void Start()
    {
        collection = new ObservableRingBuffer<int>();
        view = collection.CreateView(x =>
        {
            var item = GameObject.Instantiate(prefab);
            item.GetComponentInChildren<Text>().text = x.ToString();
            return item.gameObject;
        });
        view.AttachFilter(new GameObjectFilter(root));
    }

    void OnDestroy()
    {
        view.Dispose();
    }

    void DoThing()
    {
        var observableDictionary = new ObservableDictionary<int, string>();
        Observable<CollectionAddEvent<KeyValuePair<int, string>>> observeAdd = observableDictionary.ObserveAdd();
        observeAdd.Subscribe(collectionAddEvent =>
        {
            (int addEventKey, var keyValuePair) = collectionAddEvent;
            (int key, string value) = keyValuePair;

            Debug.Log($"Add [{key}]={value}");
        });

        observableDictionary.ObserveReplace(destroyCancellationToken)
            .Subscribe(replaceEvent =>
            {
                var key = replaceEvent.NewValue.Key;
                var newValue = replaceEvent.NewValue.Value;
                var oldValue = replaceEvent.OldValue.Value;
                Debug.Log($"Replace [{key}]={oldValue} -> {newValue}");
            });

        observableDictionary[1] = "hoge";
        observableDictionary[2] = "fuga";
        observableDictionary[1] = "piyo";
    }

    public class GameObjectFilter : ISynchronizedViewFilter<int, GameObject>
    {
        readonly GameObject root;

        public GameObjectFilter(GameObject root)
        {
            this.root = root;
        }

        public void OnCollectionChanged(in SynchronizedViewChangedEventArgs<int, GameObject> eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newView in eventArgs.NewViews)
                {
                    newView.transform.SetParent(root.transform);
                }
            }
            else if (eventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldView in eventArgs.OldViews)
                {
                    Destroy(oldView);
                }
            }
        }

        public bool IsMatch(int value, GameObject view)
        {
            return true;
        }

        public void WhenTrue(int value, GameObject view)
        {
            view.SetActive(true);
        }

        public void WhenFalse(int value, GameObject view)
        {
            view.SetActive(false);
        }
    }
}