using UnityEngine;
using System.Collections;

public class PistolGun : MonoBehaviour
{
    public int ammo = 23;
    public Camera playerCam;
    public float range = 60f;

    public Transform barrelLocation;
    public GameObject muzzleFlash;
    public Transform cameraRoot;

    Animator anim;
    Vector3 originalCamPos;
    float shakeDuration = 0f;
    float shakeStrength = 0.09f;   // أقوى بوضوح
    float shakeFadeSpeed = 8.5f;   // مناسب للسموث

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator not found! تأكد أن Animator مربوط على M1911 Handgun_Model");

        if (cameraRoot)
            originalCamPos = cameraRoot.localPosition;
    }

    void Update()
    {
        // اطلاق النار
        if (Input.GetMouseButtonDown(0) && ammo > 0)
        {
            Shoot();
            ammo--;
        }

        // Camera Shake سموث وقوي
        if (cameraRoot)
        {
            if (shakeDuration > 0)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeStrength;
                // تقدر تثبت الزد لو تحب الهزة أفقية فقط:
                shakeOffset.z = 0;
                cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, originalCamPos + shakeOffset, Time.deltaTime * shakeFadeSpeed);
                shakeDuration -= Time.deltaTime;
            }
            else
            {
                cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, originalCamPos, Time.deltaTime * shakeFadeSpeed);
            }
        }
    }

    void Shoot()
    {
        // تشغيل الفلاش
        if (muzzleFlash)
            StartCoroutine(DoMuzzleFlash());

        // تشغيل الانميشن
        if (anim)
            anim.SetTrigger("Fire");

        // Raycast للتصويب
        RaycastHit hit;
        Vector3 shootDir = playerCam.transform.forward;
        Vector3 shootOrigin = barrelLocation ? barrelLocation.position : playerCam.transform.position;

        if (Physics.Raycast(shootOrigin, shootDir, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            // تأثير إصابة الهدف هنا
        }
        else
        {
            Debug.Log("Miss!");
        }

        // فعّل الاهتزاز (مدة أطول)
        shakeDuration = 0.18f;
    }

    IEnumerator DoMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        muzzleFlash.SetActive(false);
    }
}
