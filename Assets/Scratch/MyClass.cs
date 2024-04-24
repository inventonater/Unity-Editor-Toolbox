using R3;
using UnityEngine;

[ExecuteInEditMode]
public class MyClass : MonoBehaviour
{
    public void Update()
    {
        _thingField++;
        _thingProperty++;

        Intr.Update();
    }

    private int _thingField = 33;
    private int _thingProperty { get; set; } = 33;
    public MyProp<Transform> _trans;

    [SerializeReference, ReferencePicker(ForceUninitializedInstance = true)]
    public IMyInterface Intr = new MyImpl();

    [SerializeField] private int myValue = 10;
    public int MyReadOnlyProperty
    {
        get { return myValue * 2; }
    }
}