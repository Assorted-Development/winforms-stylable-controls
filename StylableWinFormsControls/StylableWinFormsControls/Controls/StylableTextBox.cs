using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;

namespace StylableWinFormsControls;

public class StylableTextBox : TextBox
{
    private Timer m_delayedTextChangedTimer;
    private string _hint;

    public event EventHandler DelayedTextChanged;

    private EventHandler _hintActiveChanged;

    /// <summary>
    /// will be set when the Text is being set because of Hint logic
    /// </summary>
    private bool _hintRefresh = false;

    /// <summary>
    /// will be triggered when IsHintActive changes.
    /// this event will detect when a callback is already registered and will not register again
    /// </summary>
    public event EventHandler HintActiveChanged
    {
        add
        {
            if (_hintActiveChanged == null || !_hintActiveChanged.GetInvocationList().Contains(value))
            {
                _hintActiveChanged += value;
            }
        }
        remove
        {
            _hintActiveChanged -= value;
        }
    }

    private Color borderColor = Color.Blue;

    public Color BorderColor
    {
        get => borderColor;
        set
        {
            borderColor = value;
        }
    }

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
        set
        {
            base.ForeColor = value;
        }
    }

    [Editor]
    [Description("If set, this text will be shown as grayed hint until the user enters the box")]
    public string Hint
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
        InitializeComponent();
        DelayedTextChangedTimeout = 900; // 0.9 seconds
        IsHintActive = true;
        BorderStyle = BorderStyle.None;
        this.TextChanged += OnTextChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (m_delayedTextChangedTimer != null)
        {
            m_delayedTextChangedTimer.Stop();
            if (disposing)
                m_delayedTextChangedTimer.Dispose();
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
            InitializeDelayedTextChangedEvent();
        }
        base.OnTextChanged(e);
    }

    private void InitializeDelayedTextChangedEvent()
    {
        m_delayedTextChangedTimer?.Stop();

        if (m_delayedTextChangedTimer == null || m_delayedTextChangedTimer.Interval != DelayedTextChangedTimeout)
        {
            m_delayedTextChangedTimer = new Timer();
            m_delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
            m_delayedTextChangedTimer.Interval = DelayedTextChangedTimeout;
        }

        m_delayedTextChangedTimer.Start();
    }

    private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e)
    {
        if (sender is Timer timer)
        {
            timer.Stop();
        }

        OnDelayedTextChanged(EventArgs.Empty);
    }

    #endregion timer events / methods

    #region hint events / methods

    private void OnTextChanged(Object? sender, EventArgs e)
    {
        if (!_hintRefresh)
        {
            //The text change either comes from the user or the application setting a default value
            IsHintActive = false;
#pragma warning disable CS0618 //Obsolete
            this.ForeColor = TextForeColor;
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
            this.ForeColor = HintForeColor;
#pragma warning restore CS0618 //Obsolete
            base.Text = Hint;
            _hintRefresh = false;
            if (_hintActiveChanged != null)
                _hintActiveChanged(this, EventArgs.Empty);
        }

        base.OnLostFocus(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        if (IsHintActive && !string.IsNullOrEmpty(base.Text))
        {
            IsHintActive = false;
#pragma warning disable CS0618 //Obsolete
            this.ForeColor = TextForeColor;
#pragma warning restore CS0618 //Obsolete
            _hintRefresh = true;
            base.Text = "";
            _hintRefresh = false;
            if (_hintActiveChanged != null)
                _hintActiveChanged(this, EventArgs.Empty);
        }

        base.OnGotFocus(e);
    }

    #endregion hint events / methods

    private void InitializeComponent()
    {
        SuspendLayout();
        //
        // StylableTextBox
        //
        VisibleChanged += StylableTextBox_VisibleChanged;
        ResumeLayout(false);
    }

    /// <summary>
    /// ensure the correct forecolor is set when the control is first shown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StylableTextBox_VisibleChanged(object sender, EventArgs e)
    {
        if (IsHintActive)
        {
#pragma warning disable CS0618 //Obsolete
            this.ForeColor = HintForeColor;
#pragma warning restore CS0618 //Obsolete
        }
        else
        {
#pragma warning disable CS0618 //Obsolete
            this.ForeColor = TextForeColor;
#pragma warning restore CS0618 //Obsolete
        }
    }
}