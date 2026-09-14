// -----------------------------------------------------------------------------
// ローカルのプレイヤーの移動速度を変更する。
// Walk Run Strafe Jump Gravityを扱う。
// 各値の上限と下限はUIスライダーで設定する。
// 外部からロックしている値は現在値、UIスライダー、UIテキストを変更しない。
//
// ◆UIから呼び出すメソッド名
// OnResetButton リセットボタン
// OnWalkSliderChanged スライダーWalk 
// OnRunSliderChanged スライダーRun
// OnStrafeSliderChanged スライダーStrafe
// OnJumpSliderChanged スライダーJump
// OnGravitySliderChanged スライダーGravity
//
// ◆外部メソッド用
// - 各停止
// LockWalk
// LockRun
// LockStrafe
// LockJump
// LockGravity
// - 各停止の解除
// UnlockWalk
// UnlockRun
// UnlockStrafe
// UnlockJump
// UnlockGravity
// -----------------------------------------------------------------------------
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerSpeedChanger : UdonSharpBehaviour
{
    //初期値
    [Header("初期値")]
    [SerializeField] [Tooltip("プレイヤーの歩き速度")]
    private float defaultWalkSpeed = 2f;
    [SerializeField] [Tooltip("プレイヤーの走り速度")]
    private float defaultRunSpeed = 4f;
    [SerializeField] [Tooltip("プレイヤーの横歩き速度")]
    private float defaultStrafeSpeed = 2f;
    [SerializeField] [Tooltip("プレイヤーのジャンプの強さ")]
    private float defaultJumpImpulse = 3f;
    [SerializeField] [Tooltip("プレイヤーの重力")]
    private float defaultGravity = 1f;
    [Space(15)]

    [Header("WalkとStrafeを統合して扱う")]
    [SerializeField]
    [Tooltip("Walkの値をStrafeに適用する。Strafeは不使用")]
    private bool WalkStrafeEnable = false;

    //UIスタイダー
    [Header("UIスライダー")]
    [SerializeField] private Slider walkSlider;
    [SerializeField] private Slider runSlider;
    [SerializeField] private Slider strafeSlider;
    [SerializeField] private Slider jumpSlider;
    [SerializeField] private Slider gravitySlider;   

    //UIText
    [Header("UI数値(テキスト)")]
    [SerializeField] private TextMeshProUGUI walkText;
    [SerializeField] private TextMeshProUGUI runText;
    [SerializeField] private TextMeshProUGUI strafeText;
    [SerializeField] private TextMeshProUGUI jumpText;
    [SerializeField] private TextMeshProUGUI gravityText;
    [Space(15)]

    [Header("--------デバッグ--------")]
    //外部から呼び出す一時停止処理
    [Header("外部ロック確認用")]
    [SerializeField] private bool isWalkLocked = false;
    [SerializeField] private bool isRunLocked = false;
    [SerializeField] private bool isStrafeLocked = false;
    [SerializeField] private bool isJumpLocked = false;
    [SerializeField] private bool isGravityLocked = false;

    //現在値
    private float currentWalkSpeed;
    private float currentRunSpeed;
    private float currentStrafeSpeed;
    private float currentJumpImpulse;
    private float currentGravity;

    //ログの有効化
    [Header("デバッグをコンソールに出力")]
    [SerializeField] private bool enableDebugLog = false;


    private void Start()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("PlayerSpeedChanger:参照不足のため初期化を中止します");
            return;
        }
        ApplyInitialState();
    }

    /// <summary>
    /// 参照のNullチェック。参照が無ければ警告ログを出す。
    /// </summary>
    private bool ValidateReferences()
    {
        bool isValid = true;
        //スライダーの参照チェック
        if (walkSlider == null)
        {
            Debug.LogError("Walkスライダーが未設定です");
            isValid = false;
        }
        if (runSlider == null)
        {
            Debug.LogError("Runスライダーが未設定です");
            isValid = false;
        }
        if (!WalkStrafeEnable && strafeSlider == null)
        {
            Debug.LogError("Strafeスライダーが未設定です");
            isValid = false;
        }
        if (jumpSlider == null)
        {
            Debug.LogError("Jumpスライダーが未設定です");
            isValid = false;
        }
        if (gravitySlider == null)
        {
            Debug.LogError("Gravityスライダーが未設定です");
            isValid = false;
        }
        //テキストの参照チェック
        if (walkText == null)
        {
            Debug.LogError("Walkテキストが未設定です");
            isValid = false;
        }
        if (runText == null)
        {
            Debug.LogError("Runテキストが未設定です");
            isValid = false;
        }
        if (!WalkStrafeEnable && strafeText == null)
        {
            Debug.LogError("Strafeテキストが未設定です");
            isValid = false;
        }
        if (jumpText == null)
        {
            Debug.LogError("Jumpテキストが未設定です");
            isValid = false;
        }
        if (gravityText == null)
        {
            Debug.LogError("Gravityテキストが未設定です");
            isValid = false;
        }
        return isValid;
    }


    //----------------------------------------
    //各パラメーター適用
    //----------------------------------------

    /// <summary>
    /// 今の速度変数と重力変数値をローカルプレイヤーへ適用する。ロックしている値は適用しない。
    /// </summary>
    private void ApplyCurrentSpeedsGravity()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null) return;

        if (!isWalkLocked) localPlayer.SetWalkSpeed(currentWalkSpeed);
        if (!isRunLocked) localPlayer.SetRunSpeed(currentRunSpeed);
        if (!isStrafeLocked) localPlayer.SetStrafeSpeed(currentStrafeSpeed);
        if (!isJumpLocked) localPlayer.SetJumpImpulse(currentJumpImpulse);
        if (!isGravityLocked) localPlayer.SetGravityStrength(currentGravity);

    }

    /// <summary>
    /// 初期化。インスペクターの値を現在値、UIスライダー、UIテキストに反映し、ローカルプレイヤーに適用する。
    /// </summary>
    private void ApplyInitialState()
    {
        SetInitialValuesForced();
        UpdateSlidersFromCurrentValues();
        UpdateAllTexts();
        ApplyCurrentSpeedsGravity();
    }

    /// <summary>
    /// インスペクターの値を各現在値へ適用。初期化用強制。
    /// </summary>
    private void SetInitialValuesForced()
    {
        currentWalkSpeed = defaultWalkSpeed;
        currentRunSpeed = defaultRunSpeed;
        if (WalkStrafeEnable)
        {
            currentStrafeSpeed = defaultWalkSpeed;
        }
        else
        {
            currentStrafeSpeed = defaultStrafeSpeed;
        }
        currentJumpImpulse = defaultJumpImpulse;
        currentGravity = defaultGravity;

        LogDebug("初期化のため初期値をセットします");
    }
    /// <summary>
    /// インスペクターの値を各現在値へ適用。ロックしている値は触らない。
    /// </summary>
    private void ResetUnlockedValuesToDefault()
    {
        if(!isWalkLocked) currentWalkSpeed = defaultWalkSpeed;
        if(!isRunLocked) currentRunSpeed = defaultRunSpeed;
        if (!isStrafeLocked)
        {
            if (WalkStrafeEnable)
            {
                currentStrafeSpeed = currentWalkSpeed;
            }
            else
            {
                currentStrafeSpeed = defaultStrafeSpeed;
            }
        }
        if(!isJumpLocked) currentJumpImpulse = defaultJumpImpulse;
        if(!isGravityLocked) currentGravity = defaultGravity;

        LogDebug("ロックしていない値を初期値にリセットします");
    }

    /// <summary>
    /// Walkの現在値を更新し、必要な反映を行う。
    /// </summary>
    /// <param name="value">反映するWalk速度。</param>
    private void SetCurrentWalkSpeed(float value)
    {
        if (isWalkLocked)
        {
            LogDebug("Walkは停止中のため変更をスキップ"); return;
        }
        currentWalkSpeed = value;

        if (WalkStrafeEnable && !isStrafeLocked)
        {
            currentStrafeSpeed = value;

            if (strafeSlider != null)
            {
                strafeSlider.value = currentStrafeSpeed;
            }

            UpdateStrafeText();
        }

        UpdateWalkText();
        ApplyCurrentSpeedsGravity();
        LogDebug("Walk = " + value);
    }
    /// <summary>
    /// Runの現在値を更新し、必要な反映を行う。
    /// </summary>
    /// <param name="value">反映するRun速度。</param>
    private void SetCurrentRunSpeed(float value)
    {
        if (isRunLocked)
        {
            LogDebug("Runは停止中のため変更をスキップ"); return;
        }
        currentRunSpeed = value;
        UpdateRunText();
        ApplyCurrentSpeedsGravity();
        LogDebug("Run = " + value);
    }
    /// <summary>
    /// Strafeの現在値を更新し、必要な反映を行う。
    /// </summary>
    /// <param name="value">反映するStrafe速度。</param>
    private void SetCurrentStrafeSpeed(float value)
    {
        if (isStrafeLocked)
        {
            LogDebug("Strafeは停止中のため変更をスキップ"); return;
        }
        currentStrafeSpeed = value;
        UpdateStrafeText();
        ApplyCurrentSpeedsGravity();
        LogDebug("Strafe = " + value);
    }
    /// <summary>
    /// Jumpの現在値を更新し、必要な反映を行う。
    /// </summary>
    /// <param name="value">反映するJump速度。</param>
    private void SetCurrentJumpImpulse(float value)
    {
        if (isJumpLocked)
        {
            LogDebug("Jumpは停止中のため変更をスキップ"); return;
        }
        currentJumpImpulse = value;
        UpdateJumpText();
        ApplyCurrentSpeedsGravity();
        LogDebug("Jump = " + value);
    }
    /// <summary>
    /// Gravityの現在値を更新し、必要な反映を行う。
    /// </summary>
    /// <param name="value">反映するGravityの強さ。</param>
    private void SetCurrentGravityStrength(float value)
    {
        if (isGravityLocked)
        {
            LogDebug("Gravityは停止中のため変更をスキップ"); return;
        }
        currentGravity = value;
        UpdateGravityText();
        ApplyCurrentSpeedsGravity();
        LogDebug("Gravity = " + value);
    }

    //----------------------------------------
    //デバッグログ表示トグル
    //----------------------------------------

    private void LogDebug(string message)
    {
        if (!enableDebugLog) return;
        Debug.Log(message);
    }


    //----------------------------------------
    //UIスライダー反映・UIテキスト更新
    //----------------------------------------

    /// <summary>
    /// 現在の値をUIスライダーに反映。
    /// </summary>
    private void UpdateSlidersFromCurrentValues()
    {
       if (walkSlider != null) walkSlider.value = currentWalkSpeed;
       if (runSlider != null) runSlider.value = currentRunSpeed;
       if (strafeSlider != null)
       {
           strafeSlider.value = currentStrafeSpeed;
           strafeSlider.interactable = !WalkStrafeEnable && !isStrafeLocked;
       }
       if (jumpSlider != null) jumpSlider.value = currentJumpImpulse;
       if (gravitySlider != null) gravitySlider.value = currentGravity;
    }

    /// <summary>
    /// Walkの現在値をUIテキストに反映。
    /// </summary>
    private void UpdateWalkText()
    {
        if (walkText != null) walkText.text = currentWalkSpeed.ToString("f1");
    }
    /// <summary>
    /// Runの現在値をUIテキストに反映。
    /// </summary>
    private void UpdateRunText()
    {
        if (runText != null) runText.text = currentRunSpeed.ToString("f1");
    }
    /// <summary>
    /// Strafeの現在値をUIテキストに反映。
    /// </summary>
    private void UpdateStrafeText()
    {
        if (strafeText != null) strafeText.text = currentStrafeSpeed.ToString("f1");
    }
    /// <summary>
    /// Jumpの現在値をUIテキストに反映。
    /// </summary>
    private void UpdateJumpText()
    {
        if (jumpText != null) jumpText.text = currentJumpImpulse.ToString("f1");
    }
    /// <summary>
    /// Gravityの現在値をUIテキストに反映。
    /// </summary>
    private void UpdateGravityText()
    {
        if (gravityText != null) gravityText.text = currentGravity.ToString("f1");
    }
    /// <summary>
    /// 全ての現在値をUIテキストに反映。
    /// </summary>
    private void UpdateAllTexts()
    {
        UpdateWalkText();
        UpdateRunText();
        UpdateStrafeText();
        UpdateJumpText();
        UpdateGravityText();
    }


    //----------------------------------------
    //UIから呼び出しメソッド
    //----------------------------------------

    /// <summary>
    /// UIリセットから呼ぶメソッド。変更可能な値を初期値へ戻す。
    /// </summary>
    public void OnResetButton()
    {
        ResetUnlockedValuesToDefault();
        UpdateSlidersFromCurrentValues();
        UpdateAllTexts();
        ApplyCurrentSpeedsGravity();
    }

    /// <summary>
    /// UIスライダーWalkから呼ぶメソッド。Walkの現在値を更新して反映する。
    /// </summary>
    public void OnWalkSliderChanged()
    {
        if (walkSlider == null) return;

        SetCurrentWalkSpeed(walkSlider.value);
    }
    public void OnRunSliderChanged()
    {
        if (runSlider == null) return;

        SetCurrentRunSpeed(runSlider.value); 
    }
    public void OnStrafeSliderChanged()
    {
        if (WalkStrafeEnable) return;
        if (strafeSlider == null) return;

        SetCurrentStrafeSpeed(strafeSlider.value);
    }
    public void OnJumpSliderChanged()
    {
        if (jumpSlider == null) return;

        SetCurrentJumpImpulse(jumpSlider.value);
    }
    public void OnGravitySliderChanged()
    {
        if (gravitySlider == null) return;

        SetCurrentGravityStrength(gravitySlider.value);
    }

    //----------------------------------------
    //外部から呼び出して一時停止するメソッド
    //----------------------------------------

    // Lock
    public void LockWalk()
    {
        isWalkLocked = true;

        if(walkSlider != null) walkSlider.interactable = false;
        LogDebug("Walkをロック");
    }
    public void LockRun()
    {
        isRunLocked = true;

        if (runSlider != null) runSlider.interactable = false;
        LogDebug("Runをロック");
    }
    public void LockStrafe()
    {
        isStrafeLocked = true;

        if (strafeSlider != null) strafeSlider.interactable = false;
        LogDebug("Strafeをロック");
    }
    public void LockJump()
    {
        isJumpLocked = true;

        if (jumpSlider != null) jumpSlider.interactable = false;
        LogDebug("Jumpをロック");
    }
    public void LockGravity()
    {
        isGravityLocked = true;

        if (gravitySlider != null) gravitySlider.interactable = false;
        LogDebug("Gravityをロック");
    }

    // Unlock
    public void UnlockWalk()
    {
        isWalkLocked = false;

        if(walkSlider != null) walkSlider.interactable = true;
        LogDebug("Walkロックを解除しました");
    }
    public void UnlockRun()
    {
        isRunLocked = false;

        if (runSlider != null) runSlider.interactable = true;
        LogDebug("Runロックを解除しました");
    }
    public void UnlockStrafe()
    {
        isStrafeLocked = false;

        if (strafeSlider != null) strafeSlider.interactable = !WalkStrafeEnable;
        LogDebug("Strafeロックを解除しました");
    }
    public void UnlockJump()
    {
        isJumpLocked=false;

        if (jumpSlider != null) jumpSlider.interactable = true;
        LogDebug("Jumpロックを解除しました");
    }
    public void UnlockGravity()
    {
        isGravityLocked=false;
        if (gravitySlider != null) gravitySlider.interactable = true;
        LogDebug("Gravityロックを解除しました");
    }
}
