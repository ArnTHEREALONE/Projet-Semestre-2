using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class SplineTimeControl : MonoBehaviour
{
    public List<MovingObjectSpline> followers = new List<MovingObjectSpline>();

    [Header("Settings")]
    public float timeSpeed = 1f;
    public float returnToNormalDuration = 2f;
    private float deadZone = 0.1f;

    [Header("Time Limits")]
    public float maxControlDuration = 3f;
    public float controlCooldown = 2f;
    private float controlTimer = 0f;
    private float cooldownTimer = 0f;

    [Header("Gauge")]
    public Slider gaugeSlider;
    public float maxGauge = 100f;
    public float gaugeRegenPerSecond = 15f;
    public float gaugeDrainPerSecond = 10f;
    public float gaugeStartCost = 10f;
    private float currentGauge;

    private Dictionary<MovingObjectSpline, float> positions = new Dictionary<MovingObjectSpline, float>();

    public AudioSource Rewind;
    public AudioClip rewind1;

    private bool isManualControl = false;
    private bool isReturning = false;
    private float returnTimer = 0f;

    void Start()
    {
        foreach (var f in followers)
        {
            if (f == null) continue;

            positions[f] = f.GetNormalizedPosition();
            f.SetSpeedMultiplier(1f);
        }

        currentGauge = maxGauge;
        UpdateGaugeUI();
    }

    void Update()
    {
        if (Gamepad.current == null)
            return;

        // Cooldown timer
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        float right = Gamepad.current.rightTrigger.ReadValue();
        float left = Gamepad.current.leftTrigger.ReadValue();

        float direction = 0f;

        if (right > deadZone)
            direction += right;

        if (left > deadZone)
            direction -= left;

        bool canUseControl = cooldownTimer <= 0f && currentGauge > 0f;

        if (direction != 0f && canUseControl)
        {
            if (!isManualControl)
            {
                // coût initial
                if (currentGauge >= gaugeStartCost)
                {
                    currentGauge -= gaugeStartCost;
                    EnterManualMode();
                    controlTimer = 0f;
                }
                else
                {
                    direction = 0f;
                }
            }

            if (isManualControl)
            {
                controlTimer += Time.deltaTime;

                // drain continu
                currentGauge -= gaugeDrainPerSecond * Time.deltaTime;
                currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);

                if (controlTimer >= maxControlDuration || currentGauge <= 0f)
                {
                    ForceStopControl();
                }
                else
                {
                    ManualUpdate(direction);
                }
            }
        }
        else
        {
            if (isManualControl)
                ExitManualMode();

            if (isReturning)
                SmoothReturnToNormal();

            // regen gauge
            if (!isManualControl)
            {
                currentGauge += gaugeRegenPerSecond * Time.deltaTime;
                currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);
            }
        }

        UpdateGaugeUI();
    }

    void EnterManualMode()
    {
        isManualControl = true;
        isReturning = false;

        foreach (var f in followers)
        {
            if (f == null) continue;

            positions[f] = f.GetNormalizedPosition();
            f.SetSpeedMultiplier(0f);
        }
    }

    void ManualUpdate(float direction)
    {
        foreach (var f in followers)
        {
            if (f == null) continue;

            positions[f] += direction * timeSpeed * Time.deltaTime;

            if (f.loop)
                positions[f] = Mathf.Repeat(positions[f], 1f);
            else
                positions[f] = Mathf.Clamp01(positions[f]);

            f.SetNormalizedPosition(positions[f]);
        }
    }

    void ExitManualMode()
    {
        isManualControl = false;
        isReturning = true;
        returnTimer = 0f;

        foreach (var f in followers)
        {
            if (f == null) continue;
            f.SetSpeedMultiplier(0f);
        }
    }

    void ForceStopControl()
    {
        ExitManualMode();
        cooldownTimer = controlCooldown;
    }

    void SmoothReturnToNormal()
    {
        returnTimer += Time.deltaTime;

        float t = returnTimer / returnToNormalDuration;
        float speed = Mathf.SmoothStep(0f, 1f, t);

        foreach (var f in followers)
        {
            if (f == null) continue;
            f.SetSpeedMultiplier(speed);
        }

        if (t >= 1f)
            isReturning = false;
    }

    void UpdateGaugeUI()
    {
        if (gaugeSlider != null)
            gaugeSlider.value = currentGauge / maxGauge;
    }
}