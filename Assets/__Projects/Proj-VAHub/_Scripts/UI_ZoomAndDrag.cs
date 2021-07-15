using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ZoomAndDrag : MonoBehaviour, IDragHandler, IScrollHandler, IPointerDownHandler
{
    public RectTransform TargetElement;

    //Zoom
    public float ZoomStep = 0.03f;
    public Vector2 zoomMultiplierRange = new Vector2(1, 3f);
    public Slider ZoomSlider;
    public Button BtnZoomUp;
    public Button BtnZoomDown;
    [Range(0f, 1f)]float zoomCtrl = 0f;
    float zoomLerpSpeed = 3f;
    Vector3 originalScale;


    // Drag
    Vector2 lastMousePos;

    private void Start()
    {
        originalScale = TargetElement.localScale;

        if (ZoomSlider)
            ZoomSlider.onValueChanged.AddListener(delegate { OnZoomSliderChanged(); });
        if (BtnZoomUp)
            BtnZoomUp.onClick.AddListener(OnBtnZoomUp);
        if (BtnZoomDown)
            BtnZoomDown.onClick.AddListener(OnBtnZoomDown);
    }

    private void OnEnable() {
        // Reset zoom on enabled
        zoomCtrl = 0f;
        TargetElement.localPosition = Vector3.zero;
    }

    private void Update()
    {

        ZoomUpdate();
    }

    void ZoomUpdate()
    {
        // apply zoom with smooth lerp
        zoomCtrl = Mathf.Clamp(zoomCtrl, 0f, 1f);
        float targetZoom = Mathf.Lerp(zoomMultiplierRange.x, zoomMultiplierRange.y, zoomCtrl);
        TargetElement.localScale = Vector3.Lerp(TargetElement.localScale, originalScale * targetZoom, Time.deltaTime * zoomLerpSpeed);
    }

    void OnZoomSliderChanged()
    {
        zoomCtrl = ZoomSlider.normalizedValue;
    }

    void OnBtnZoomUp()
    {
        zoomCtrl += 0.1f;
    }
    void OnBtnZoomDown()
    {
        zoomCtrl -= 0.1f;
    }



    // Drag
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnMouseDown " + TargetElement.localPosition);
        lastMousePos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dist = eventData.position - lastMousePos;

        // Apply position
        TargetElement.Translate(dist, Space.Self);

        lastMousePos = eventData.position;
        Debug.Log("OnDrag " + dist);
    }


    public void OnScroll(PointerEventData eventData)
    {
        Debug.Log("OnScroll " + eventData.scrollDelta.y);
        zoomCtrl += eventData.scrollDelta.y * ZoomStep;
        if (ZoomSlider) ZoomSlider.value = zoomCtrl;
    }
}