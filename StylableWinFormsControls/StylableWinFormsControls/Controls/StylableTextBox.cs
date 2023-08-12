using System.ComponentModel;

namespace StylableWinFormsControls;

public class StylableTextBox : TextBox
{
    private System.Windows.Forms.Timer? _mDelayedTextChangedTimer;
    private string? _hint;

    public event EventHandler? DelayedTextChanged;

    private EventHandler? _hintActiveChanged;

    /// <summary>
    /// will be set when the Text is being set because of Hint logic
    /// </summary>
    private bool _hintRefresh;

    /// <summary>
    /// will be triggered when IsHintActive changes.
    /// this event will detect when a callback is already registered and will not register again
    /// </summary>
    public event EventHandler HintActiveChanged
    {
        add
        {
            if (_hintActiveChanged is null || !_hintActiveChanged.GetInvocationList().Contains(value))
            {
                _hintActiveChanged += value;
            }
        }
        remove => _hintActiveChanged -= value;
    }

    public Color BorderColor { get; set; } = Color.Blue;

    /// <summary>
    /// used to set the color of the hint text
    /// </summary>
    public Color HintForeColor { get; set; } = Color.Gray;

    /// <summary>
    /// used to set the color of the hint text
    /// </summary>
    public Color TextForeColor { get; set; } = Color.Black;

    [Editor]
    public int DelayedTextChangedTimeout { get; set; }

    /// <summary>
    /// Gets or sets the foreground color of the control.
    /// We are hiding this as it is only the current color of the text.
    /// Use <see cref="TextForeColor"/> to set the color of the text or <see cref="HintForeColor"/> for the hint.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use TextForeColor or HintForeColor instead")]
    public new Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    [Editor]
    [Description("If set, this text will be shown as grayed hint until the user enters the box")]
    public string? Hint
    {
        get => _hint;
        set
        {
            _hint = value;
            if (string.IsNullOrEmpty(base.Text))
            {
                _hintRefresh = true;
                base.Text = value;
                _hintRefresh = false;
            }
        }
    }

    public bool IsHintActive { get; private set; }

    public bool IsDelayActive { get; set; } = true;

    public StylableTextBox()
    {
        initializeComponent();
        DelayedTextChangedTimeout = 900; // 0.9 seconds
        IsHintActive = true;
        BorderStyle = BorderStyle.FixedSingle;
        TextChanged += onTextChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (_mDelayedTextChangedTimer is not null)
        {
            _mDelayedTextChangedTimer.Stop();
            if (disposing)
            {
                _mDelayedTextChangedTimer.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    #region timer events / methods

    protected virtual void OnDelayedTextChanged(EventArgs e)
    {
        DelayedTextChanged?.Invoke(this, e);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        if (IsDelayActive && DelayedTextChangedTimeout > 0)
        {
            initializeDelayedTextChangedEvent();
        }
        base.OnTextChanged(e);
    }

    private void initializeDelayedTextChangedEvent()
    {
        _mDelayedTextChangedTimer?.Stop();

        if (_mDelayedTextChangedTimer is null || _mDelayedTextChangedTimer.Interval != DelayedTextChangedTimeout)
        {
            _mDelayedTextChangedTimer = new System.Windows.Forms.Timer();
            _mDelayedTextChangedTimer.Tick += handleDelayedTextChangedTimerTick;
            _mDelayedTextChangedTimer.Interval = DelayedTextChangedTimeout;
        }

        _mDelayedTextChangedTimer.Start();
    }

    private void handleDelayedTextChangedTimerTick(object? sender, EventArgs e)
    {
        if (sender is System.Windows.Forms.Timer timer)
        {
            timer.Stop();
        }

        OnDelayedTextChanged(EventArgs.Empty);
    }

    #endregion timer events / methods

    #region hint events / methods

    private void onTextChanged(object? sender, EventArgs e)
    {
        if (!_hintRefresh)
        {
            //The text change either comes from the user or the application setting a default value
            IsHintActive = false;
#pragma warning disable CS0618 //Obsolete
            ForeColor = TextForeColor;
#pragma warning restore CS0618 //Obsolete
        }
    }

    protected override void OnLostFocus(EventArgs e)
    {
        if (!IsHintActive && string.IsNullOrEmpty(base.Text))
        {
            IsHintActive = true;
            _hintRefresh = true;
#pragma warning disable CS0618 //Obsolete
            ForeColor = HintForeColor;
#pragma warning restore CS0618 //Obsolete
            base.Text = Hint;
            _hintRefresh = false;
            if (_hintActiveChanged is not null)
            {
                _hintActiveChanged(this, EventArgs.Empty);
            }
        }

        base.OnLostFocus(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        if (IsHintActive && !string.IsNullOrEmpty(base.Text))
        {
            IsHintActive = false;
#pragma warning disable CS0618 //Obsolete
            ForeColor = TextForeColor;
#pragma warning restore CS0618 //Obsolete
            _hintRefresh = true;

            base.Text = string.Empty;
            _hintRefresh = false;
            if (_hintActiveChanged is not null)
            {
                _hintActiveChanged(this, EventArgs.Empty);
            }
        }

        base.OnGotFocus(e);
    }

    #endregion hint events / methods

    private void initializeComponent()
    {
        SuspendLayout();
        //
        // StylableTextBox
        //
        VisibleChanged += stylableTextBox_VisibleChanged;
        ResumeLayout(false);
    }

    /// <summary>
    /// ensure the correct forecolor is set when the control is first shown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void stylableTextBox_VisibleChanged(object? sender, EventArgs e)
    {
        if (IsHintActive)
        {
#pragma warning disable CS0618 //Obsolete
            ForeColor = HintForeColor;
#pragma warning restore CS0618 //Obsolete
        }
        else
        {
#pragma warning disable CS0618 //Obsolete
            ForeColor = TextForeColor;
#pragma warning restore CS0618 //Obsolete
        }
    }
}
