using UnityEngine;
using UnityEngine.UI;

//Manages the Button UI interactions of this demo.
public class ButtonController : MonoBehaviour
{
    [Header("Mode Selection Buttons")]
    [SerializeField] private Button button_A;
    [SerializeField] private Button button_B;
    [SerializeField] private Button button_C;

    [Header("Action Buttons")]
    [SerializeField] private Button button_Shoot;
    [SerializeField] private Button button_Quit;

    [Header("References")]
    [SerializeField] private ReticleController reticleController;
    [SerializeField] private ReticleCharger reticleCharger;
    
    private bool canCharge = false;
    private bool chargeComplete = false;

    //Subscribe to events notifying reticle mode changes and charge completion.
    private void OnEnable()
    {
        if (reticleController != null)
        {
            reticleController.onReticleModeChanged += HandleModeChanged;
        }
        else
        {
            Debug.LogError("Reticle Controller not assigned in ButtonController");
            return;
        }

        if (reticleCharger != null)
        {
            reticleCharger.OnChargeComplete += HandleReticleCharged;
        }
        else
        {
            Debug.LogError("Reticle Charger not assigned in ButtonController");
            return;
        }
    }

    //Unsubscribe from event to prevent memory leaks.
    private void OnDisable()
    {
        reticleController.onReticleModeChanged -= HandleModeChanged;
        reticleCharger.OnChargeComplete -= HandleReticleCharged;
    }

    //Assign functionality to buttons.
    private void Start()
    {
        button_A.onClick.AddListener(() => reticleController.ChangeReticuleMode(ReticleMode.A));
        button_B.onClick.AddListener(() => reticleController.ChangeReticuleMode(ReticleMode.B));
        button_C.onClick.AddListener(() => reticleController.ChangeReticuleMode(ReticleMode.C));
        button_Shoot.onClick.AddListener(() => HandleShootButton());
        button_Quit.onClick.AddListener(() => QuitGame());
    }

    //Shoot button only plays hit react animation for reticle modes that have charge progress bars if the charge is fully complete, else it just resets the charge.
    private void HandleShootButton()
    {
        if (canCharge)
        {
            if (chargeComplete)
            {
                reticleController.TriggerHitReact();
                chargeComplete = false;
            }
        }
        else
        {
            reticleController.TriggerHitReact();
        }

        reticleController.ResetReticleCharge();
    }

    //Caches whether current reticle mode can charge progress bar.
    private void HandleModeChanged(ReticleProfileData newProfile)
    {
        canCharge = newProfile.canCharge;
    }

    //Caches whether a charge progress has completed.
    private void HandleReticleCharged()
    {
        chargeComplete = true;
    }

    //Quits the application.
    private void QuitGame()
    {
        Application.Quit();
    }
}
