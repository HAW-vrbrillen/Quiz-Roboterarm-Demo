using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onTriggerReact01 : MonoBehaviour
{
    public SkinnedMeshRenderer SkinnedMeshRenderer1;
    public float weight1;
    public float weight2;
    public float weight3;

    public int blendshapeNo1;
    public int blendshapeNo2;
    public int blendshapeNo3;
    public int duration;
    private float originalWeight1;
    private float originalWeight2;
    private float originalWeight3;

    private void Start()
    {
        originalWeight1 = SkinnedMeshRenderer1.GetBlendShapeWeight(blendshapeNo1);
        originalWeight2 = SkinnedMeshRenderer1.GetBlendShapeWeight(blendshapeNo2);
        originalWeight3 = SkinnedMeshRenderer1.GetBlendShapeWeight(blendshapeNo3);

        //StartCoroutine(SetBlendShapeForDuration());
    }

    
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(SetBlendShapeForDuration());
    }
    

    IEnumerator SetBlendShapeForDuration()
    {
        SkinnedMeshRenderer1.SetBlendShapeWeight(blendshapeNo1, weight1);
        SkinnedMeshRenderer1.SetBlendShapeWeight(blendshapeNo2, weight2);
        SkinnedMeshRenderer1.SetBlendShapeWeight(blendshapeNo3, weight3);

        yield return new WaitForSeconds(duration);
        SkinnedMeshRenderer1.SetBlendShapeWeight(blendshapeNo1, originalWeight1);
        SkinnedMeshRenderer1.SetBlendShapeWeight(blendshapeNo2, originalWeight2);
        SkinnedMeshRenderer1.SetBlendShapeWeight(blendshapeNo3, originalWeight3);
    }
}
