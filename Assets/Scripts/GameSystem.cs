using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
/*using UnityEngine.Advertisements;*/

public class GameSystem : MonoBehaviour/*, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener*/
{
    //[SerializeField] string _androidGameId = "4281345";
    //[SerializeField] string _iOsGameId = "4281344";
    //[SerializeField] bool _testMode = false;
    //[SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId;
    //[SerializeField] string _androidAdUnitId = "Interstitial_Android";
    //[SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;

    float adUnitProbability = 0.5f;
    int adUnitCdSeconds = 30;
    int adUnitCdMax = 10;
    int adUnitCdMin = 3;
    public int adUnitCd = 0;

    public static GameSystem instance;
    public PlayerController player;
    public UIController fuelUi;
    public UIController scoreUi;
    public UIController finishUi;
    public FuelController fuelObject;
    public ScoreController scoreObject;
    public TutorialController tutorialObject;
    public FinishController finishObject;

    public Button musicButton;
    public bool isMusicEnabled = true;
    public bool isMusicActive = false;

    public bool isPaused = true;
    public bool isFinished = false;
    public float isFinishedTimer = 0.5f;

    private float score = 0.0f;

    int spawnInterval = 10;
    private GUIStyle fuelStyle = null;
    private Color fuelColor;

    float fuelTimer = 0;
    float fuelTimerMultiplier = 4;
    float scoreTimer = 0;
    float scoreTimerMultiplier = 3;

    string localeCurrent = "en";
    Dictionary<string, string> localeStrings = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Awake()
    {
        InitializeLang();

        InitializeAds();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        musicButton.onClick.AddListener(TaskOnClick);
    }
    void InitializeLang()
    {
        //Debug.Log("Language:" + Bridge.platform.language);
        if (Application.absoluteURL.IndexOf("lang=ru") != -1 || Application.absoluteURL.IndexOf("yandex.ru") != -1)
        {
            localeCurrent = "ru";
        }

        localeStrings.Add("ru_Score", "Счёт");
        localeStrings.Add("en_Score", "Score");
        localeStrings.Add("ru_TopScore", "Рекорд");
        localeStrings.Add("en_TopScore", "Top Score");
        localeStrings.Add("ru_Fuel", "Топливо");
        localeStrings.Add("en_Fuel", "Fuel");

        localeStrings.Add("ru_TutorialControl", "для полета нажимайте A/D или стрелки");
        localeStrings.Add("en_TutorialControl", "press A/D or Left/Right buttons to fly");
        localeStrings.Add("ru_TutorialFuel", "Заправляйте топливо");
        localeStrings.Add("en_TutorialFuel", "Refuel to fly longer");
        localeStrings.Add("ru_TutorialPressAny", "Нажмите любую кнопку управления чтобы начать");
        localeStrings.Add("en_TutorialPressAny", "Press any control button to start");
        localeStrings.Add("ru_TutorialGround", "Не разбейте корабль");
        localeStrings.Add("en_TutorialGround", "Don't touch the ground");
    }

    void TaskOnClick()
    {
        isMusicEnabled = isMusicEnabled ? false : true;

        if (isMusicActive)
        {
            if (isMusicEnabled)
            {
                this.GetComponent<AudioSource>().Play();
            }
            else
            {
                this.GetComponent<AudioSource>().Pause();
            }
        }

        SaveSystem.instance.isMusicEnabled = isMusicEnabled;
        SaveSystem.instance.Save();

        musicButton.GetComponent<Image>().color = isMusicEnabled ? Color.white : Color.red;
        musicButton.GetComponentInChildren<Text>().text = "mus\n"+ (isMusicEnabled ? "on" : "off");
    }

    public void InitializeAds()
    {
        //Bridge.advertisement.SetMinimumDelayBetweenInterstitial(adUnitCdSeconds);
        /*_adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, _enablePerPlacementMode, this);*/
    }
    public void OnInitializationComplete()
    {
        /*Debug.Log("Unity Ads initialization complete.");*/
        Debug.Log("Yandex Ads initialization complete.");
    }
    /*public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }*/
    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        //Debug.Log("Loading Ad: " + _adUnitId);
        //Advertisement.Load(_adUnitId, this);
    }
    // Show the loaded content in the Ad Unit: 
    public void ShowAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: ShowInterstitial");
        //Advertisement.Show(_adUnitId, this);
        Bridge.advertisement.ShowInterstitial();
    }
    // Implement Load Listener and Show Listener interface methods:  
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }
    /*public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execite code if the Ad Unit fails to load, such as attempting to try again.
    }
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execite code if the Ad Unit fails to show, such as loading another ad.
    }*/

    private void Start()
    {
        SaveSystem.instance.Load();
        adUnitCd = SaveSystem.instance.adUnitCd;

        if (Bridge.advertisement.isBannerSupported)
        {
            //Debug.Log("Showing Ad: ShowBanner");
            Bridge.advertisement.ShowBanner();
        }
        else
        {
            //Debug.Log("Showing Ad: false: !Bridge.advertisement.isBannerSupported");
        }

        isMusicEnabled = SaveSystem.instance.isMusicEnabled;
        musicButton.GetComponent<Image>().color = isMusicEnabled ? Color.white : Color.red;

        tutorialObject.transform.Find("TextTutorial (1)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialControl"];
        tutorialObject.transform.Find("TextTutorial (2)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialFuel"];
        tutorialObject.transform.Find("TextTutorial (3)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialPressAny"];
        tutorialObject.transform.Find("TextTutorial (4)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialGround"];
        updateScore(0);
    }

    public void updateFuel(float number)
    {
        fuelUi.GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_Fuel"]+": " + (int)number + "%";
    }

    public void updateScore(float number)
    {
        score += number;
        scoreUi.GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_Score"] + ": " + (int)score + "\n"+ localeStrings[localeCurrent + "_TopScore"] + ": "+(int)SaveSystem.instance.scoreTop;
    }

    //[Command]
    void SpawnFuel()
    {
        var spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.1f, 0.9f), 1.1f, 5));

        Instantiate(fuelObject, spawnPoint, Quaternion.identity);
    }

    //[Command]
    void SpawnScore()
    {
        var spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.1f, 0.9f), 0.2f, 4));

        Instantiate(scoreObject, spawnPoint, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinished && isFinishedTimer > 0) 
        {
            isFinishedTimer -= Time.deltaTime;
        }
        if (isFinishedTimer < 0) {
            isFinishedTimer = 0;
            finishUi.GetComponent<TextMeshProUGUI>().text = "\t"+localeStrings[localeCurrent + "_Score"] + ": " + (int)score + "\n\t"+ localeStrings[localeCurrent + "_TopScore"] + ": "+(int)SaveSystem.instance.scoreTop;

            //finishUi.GetComponent<TextMeshProUGUI>().text += "\nTap anywhere to restart";
            finishUi.GetComponent<TextMeshProUGUI>().text += "\n"+localeStrings[localeCurrent+"_TutorialPressAny"];

            //показывается не чаще раз в 3(?) игры, не реже раз в 8(?)
            //Debug.Log("adUnitCd" + adUnitCd + " vs prob"+ adUnitProbability + " : min"+ adUnitCdMin + " max" + adUnitCdMax);
            if (((Random.value < adUnitProbability) && (adUnitCd >= adUnitCdMin)) || (adUnitCd >= adUnitCdMax))
            {
                adUnitCd = 0;
                SaveSystem.instance.adUnitCd = adUnitCd;
                SaveSystem.instance.Save();
                //UnityAds - disable for gold and mobile
                //LoadAd();
                ShowAd();
            }
            else {
                Debug.Log("adUnitCd:" + adUnitCd);
            }
        }
                
        if (!player.isUncontrollable && !isPaused)
        {
            fuelTimer += fuelTimerMultiplier * Time.deltaTime;
            scoreTimer += scoreTimerMultiplier * Time.deltaTime;
        }
        else
        {
            if ((Input.touchCount > 0) || (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
            {
                if (!isFinished)
                {
                    SaveSystem.instance.Load();
                    isPaused = false;
                    player.isUncontrollable = false;
                    tutorialObject.GetComponent<Canvas>().enabled = false;
                    isMusicActive = true;
                    if (isMusicEnabled)
                    {
                        this.GetComponent<AudioSource>().Play();
                    }
                }
                else {
                    if (isFinishedTimer <= 0) SceneManager.LoadScene("sampleScene");
                }
            }
        }

        if (fuelTimer > spawnInterval)
        {
            fuelTimer = 0;
            SpawnFuel();
        }

        if (scoreTimer > spawnInterval)
        {
            scoreTimer = 0;
            SpawnScore();
        }
    }

    public void Finish()
    {
        isMusicActive = false;
        this.GetComponent<AudioSource>().Stop();
        isPaused = true;
        isFinished = true;
        isFinishedTimer = 0.5f;

        if (score > SaveSystem.instance.scoreTop)
        {
            SaveSystem.instance.scoreTop = score;
        }

        adUnitCd++;
        SaveSystem.instance.adUnitCd = adUnitCd;
        SaveSystem.instance.Save();

        finishUi.GetComponent<TextMeshProUGUI>().text = "\t"+localeStrings[localeCurrent + "_Score"] + ": " + (int)score + "\n\t" + localeStrings[localeCurrent + "_TopScore"] + ": " + (int)SaveSystem.instance.scoreTop;
        finishObject.GetComponent<Canvas>().enabled = true;
    }

    private Texture2D MakeFuelTexture(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public void OnGUI()
    {
        if (!player.isUncontrollable && !isPaused)
        {
            if (player.fuel > (player.fuelMax / 2))
            {
                fuelColor = Color.green;
            }
            else if (player.fuel > (player.fuelMax / 4))
            {
                fuelColor = Color.yellow;
            }
            else
            {
                fuelColor = Color.red;
            }
            if (fuelStyle == null)
            {
                fuelStyle = new GUIStyle(GUI.skin.box);
            }

            fuelStyle.normal.background = MakeFuelTexture(2, 2, fuelColor);
            GUI.Box(new Rect(0, Screen.height - Screen.height * 0.03f, Screen.width * (player.fuel / player.fuelMax), Screen.height * 0.03f), "", fuelStyle);
        }
    }
}
