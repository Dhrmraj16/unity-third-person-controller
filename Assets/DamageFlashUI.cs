using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class DamageFlashUI : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.2f;

    private Coroutine flashRoutine;

    void OnEnable()
    {
        DamageSystem.OnDamageApplied += TriggerFlash;
    }

    void OnDisable()
    {
        DamageSystem.OnDamageApplied -= TriggerFlash;
    }

    void TriggerFlash(int amount, Vector3 hitSourcePosition)
    {
        if(flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        flashImage.color = new Color(1,0,0,0.6f);
        yield return new WaitForSeconds(flashDuration);
        flashImage.color = new Color(1, 0, 0, 0f);
    }

   
}
