using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }

    }
    public void PlayVFX(GameObject effectObject, Vector3 effectPosition)
    {
        GameObject vfxObject = Instantiate(effectObject, effectPosition, Quaternion.identity);
        ParticleSystem[] particleSystems = vfxObject.GetComponentsInChildren<ParticleSystem>();

        float maxLength = 0f;
        foreach(ParticleSystem individualPartcleSystem in particleSystems) 
        {
            float currentKnownMaxLength = individualPartcleSystem.main.duration + individualPartcleSystem.main.startLifetime.constantMax;
            if(currentKnownMaxLength > maxLength)
            {
                maxLength = currentKnownMaxLength;
            }
        }
        Destroy(vfxObject, maxLength);
    }
}