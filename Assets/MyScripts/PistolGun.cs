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

    private Animator anim;
    private Vector3 originalCamPos;
    private float shakeDuration = 0f;
    private float shakeStrength = 0.09f;
    private float shakeFadeSpeed = 8.5f;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator not found! تأكد أن Animator مربوط على M1911 Handgun_Model");

        if (cameraRoot)
            originalCamPos = cameraRoot.localPosition;
    }

    void Start()
    {
        // إصلاح مهم: امسح أي تريقر Fire في البداية
        if (anim) anim.ResetTrigger("Fire");
    }

    void Update()
    {
        // اطلاق النار
        if (Input.GetMouseButtonDown(0) && ammo > 0)
        {
            Shoot();
            ammo--;
        }

        // Camera Shake
        if (cameraRoot)
        {
            if (shakeDuration > 0)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeStrength;
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
        {
            anim.ResetTrigger("Fire"); // تأكد أنه غير مفعّل أولاً (احتياط)
            anim.SetTrigger("Fire");
        }

        // Raycast
        RaycastHit hit;
        Vector3 shootDir = playerCam.transform.forward;
        Vector3 shootOrigin = barrelLocation ? barrelLocation.position : playerCam.transform.position;

        if (Physics.Raycast(shootOrigin, shootDir, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            // أضف هنا التأثير أو الضرر
        }
        else
        {
            Debug.Log("Miss!");
        }

        shakeDuration = 0.18f;
    }

    IEnumerator DoMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        muzzleFlash.SetActive(false);
    }
}
