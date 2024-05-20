using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiControlScript : MonoBehaviour
{
    public AmplifiedTurning a;
    public TranslationGain g;
    public StrafingGain strafe;
    public ScaledHand sR;
    public ScaledHand sL;
    
    public Slider AmpTurningSlider;
    public TextMeshProUGUI AmpTurningText;

    public Slider TranslationSlider;
    public TextMeshProUGUI TranslationText;

    public Slider StrafingSlider;
    public TextMeshProUGUI StrafingText;

    public Slider ScaledRHandSlider;
    public TextMeshProUGUI ScaledRHandText;

    public Slider ScaledLHandSlider;
    public TextMeshProUGUI ScaledLHandText;


    public GameObject RealRHand, RealLHand;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValues()
    {        
        a.GainValue = AmpTurningSlider.value;
        AmpTurningText.text = "Rotation Gain: " + a.GainValue.ToString("#.##");

        g.GainValue = TranslationSlider.value;
        TranslationText.text = "Translation Gain: " + g.GainValue.ToString("#.##");

        strafe.GainAngle = StrafingSlider.value;
        StrafingText.text = "Strafing Gain: " + strafe.GainAngle.ToString("#.##") + " deg";

        sR.s = ScaledRHandSlider.value;
        ScaledRHandText.text = "Right Hand Speed: " + sR.s.ToString("#.##");

        sL.s = ScaledLHandSlider.value;
        ScaledLHandText.text = "Left Hand Speed: " + sL.s.ToString("#.##");
       
    }

    public void ResetToNormal()
    {
        AmpTurningSlider.value = 1.0f;
        TranslationSlider.value = 1.0f;
        StrafingSlider.value = 0.0f;
        ScaledRHandSlider.value = 1.0f;
        ScaledLHandSlider.value = 1.0f;
        UpdateValues();
    }

    public void SetToPosThreshold()
    {
        AmpTurningSlider.value = 1.23f;//Steinicke 2010
        TranslationSlider.value = 1.26f;//Steinick 2009
        StrafingSlider.value = -4.68f;//You 2022
        ScaledRHandSlider.value = 1.47f;//Esmaeili 2020
        ScaledLHandSlider.value = 1.47f;//Esmaeili 2020
        UpdateValues();
    }

    public void SetToNegThreshold()
    {
        AmpTurningSlider.value = 0.67f;//Steinicke 2010
        TranslationSlider.value = 0.86f;//Steinick 2009
        StrafingSlider.value = 5.57f;//You 2022
        ScaledRHandSlider.value = 0.76f;//Esmaeili 2020
        ScaledLHandSlider.value = 0.76f;//Esmaeili 2020
        UpdateValues();
    }

    public void ToggleHands()
    {
        RealRHand.SetActive(!RealRHand.activeSelf);
        RealLHand.SetActive(!RealLHand.activeSelf);
    }

    public void LoadStudyScene()
    {
        SceneManager.LoadScene("Scenes/StudyScene");
    }

    public void LoadStudySceneYesNo()
    {
        //Guidance.condition = 3;
        SceneManager.LoadScene("Scenes/StudyScene");
    }

    public void LoadStudySceneMiddle()
    {
        //Guidance.condition = 2;
        SceneManager.LoadScene("Scenes/StudyScene");
    }

    public void SetRotationGain(float value)
    {
        AmpTurningSlider.value = value;
        UpdateValues();
    }

    public void SetTranlationGain(float value)
    {
        TranslationSlider.value = value;
        UpdateValues();
    }

    public void SetStrafingGain(float value)
    {
        StrafingSlider.value = value;
        UpdateValues();
    }

    public void SetRHScaleGain(float value)
    {
        ScaledRHandSlider.value = value;
        UpdateValues();
    }

    public void SetLHScaleGain(float value)
    {
        Debug.Log(value);
        ScaledLHandSlider.value = value;
        UpdateValues();
    }
}
