using Framework;
using UnityEngine;

[CreateAssetMenu]
public class TestScriptable : ScriptableObject
{
    [Button]
    private void Test()
    {
        this.Log("Hello");
    }
}
