using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using CrazyGames;

public class GameSystem : MonoBehaviour/*, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener*/
{
    private string _gameId;
    string _adUnitId;

    float adUnitProbability = 0.6f;
    int adUnitCdMax = 10;
    int adUnitCdMin = 3;
    public int adUnitCd = 0;
    bool adActive = false;

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
        localeCurrent = "en";

        localeStrings.Add("ru_Score", "����");
        localeStrings.Add("en_Score", "Score");
        localeStrings.Add("ru_TopScore", "������");
        localeStrings.Add("en_TopScore", "Top Score");
        localeStrings.Add("ru_Fuel", "�������");
        localeStrings.Add("en_Fuel", "Fuel");

        localeStrings.Add("ru_TutorialControl", "��� ������ ����������� A/D ��� �������");
        localeStrings.Add("en_TutorialControl", "press and hold A/D or Left/Right buttons to fly");
        localeStrings.Add("ru_TutorialControlMobile", "��� ������ ������� � ������� ������/����� ����� ������");
        localeStrings.Add("en_TutorialControlMobile", "press and hold left/right side of the screen to fly");
        localeStrings.Add("ru_TutorialFuel", "����������� �������");
        localeStrings.Add("en_TutorialFuel", "Refuel to fly longer");
        localeStrings.Add("ru_TutorialPressAny", "������� ����� ������ ���������� ����� ������");
        localeStrings.Add("en_TutorialPressAny", "Press any control button to start");
        localeStrings.Add("ru_TutorialPressAnyMobile", "������� �� ����� ����� ������");
        localeStrings.Add("en_TutorialPressAnyMobile", "Touch screen to start");
        localeStrings.Add("ru_TutorialGround", "�� �������� �������");
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
        CrazySDK.Init(() => { OnInitializationComplete(); });
    }

    public void OnInitializationComplete()
    {
        //Debug.Log("Crazy Games Ads initialization complete.");
        CrazySDK.Banner.RefreshBanners();
    }

    public void ShowAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        //Debug.Log("Showing Ad: ShowInterstitial");
        CrazySDK.Ad.RequestAd(CrazyAdType.Midgame, () =>
        {
            adActive = true;
        }, (error) =>
        {
            adActive = false;
        }, () =>
        {
            adActive = false;
        });
    }

    private void Start()
    {
        SaveSystem.instance.Load();
        adUnitCd = SaveSystem.instance.adUnitCd;
        var systemInfo = CrazySDK.User.SystemInfo;

        isMusicEnabled = SaveSystem.instance.isMusicEnabled;
        musicButton.GetComponent<Image>().color = isMusicEnabled ? Color.white : Color.red;
        musicButton.GetComponentInChildren<Text>().text = "mus\n" + (isMusicEnabled ? "on" : "off");

        //Debug.Log("systemInfo.device.type:" + systemInfo.device.type);
        if (systemInfo.device.type == "desktop")
        {
            tutorialObject.transform.Find("TextTutorial (3)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialControl"] + "\n" + localeStrings[localeCurrent + "_TutorialPressAny"];
        }
        else
        {
            tutorialObject.transform.Find("TextTutorial (3)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialControlMobile"] + "\n" + localeStrings[localeCurrent + "_TutorialPressAnyMobile"];
        }

        tutorialObject.transform.Find("TextTutorial (1)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialControl"];
        tutorialObject.transform.Find("TextTutorial (2)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialFuel"];
        //tutorialObject.transform.Find("TextTutorial (3)").GetComponent<TextMeshProUGUI>().text = localeStrings[localeCurrent + "_TutorialPressAny"];
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
        if (isFinishedTimer < 0)
        {
            var systemInfo = CrazySDK.User.SystemInfo;
            isFinishedTimer = 0;
            finishUi.GetComponent<TextMeshProUGUI>().text = "\t"+localeStrings[localeCurrent + "_Score"] + ": " + (int)score + "\n\t"+ localeStrings[localeCurrent + "_TopScore"] + ": "+(int)SaveSystem.instance.scoreTop;

            if (systemInfo.device.type == "desktop")
            {
                finishUi.GetComponent<TextMeshProUGUI>().text += "\n" + localeStrings[localeCurrent + "_TutorialPressAny"];
            }
            else
            {
                finishUi.GetComponent<TextMeshProUGUI>().text += "\n" + localeStrings[localeCurrent + "_TutorialPressAnyMobile"];
            }

            //������������ �� ���� ��� � 3(?) ����, �� ���� ��� � 8(?)
            //Debug.Log("adUnitCd" + adUnitCd + " vs prob"+ adUnitProbability + " : min"+ adUnitCdMin + " max" + adUnitCdMax);
            if (((Random.value < adUnitProbability) && (adUnitCd > adUnitCdMin)) || (adUnitCd > adUnitCdMax))
            {
                adUnitCd = 0;
                SaveSystem.instance.adUnitCd = adUnitCd;
                SaveSystem.instance.Save();
                
                ShowAd();
            }
            else {
                //Debug.Log("adUnitCd:" + adUnitCd);
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
                if (!isFinished && !adActive)
                {
                    SaveSystem.instance.Load();
                    CrazySDK.Game.GameplayStart();
                    CrazySDK.Banner.RefreshBanners();
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

        CrazySDK.Game.GameplayStop();

        if (score > SaveSystem.instance.scoreTop)
        {
            CrazySDK.Game.HappyTime();

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
