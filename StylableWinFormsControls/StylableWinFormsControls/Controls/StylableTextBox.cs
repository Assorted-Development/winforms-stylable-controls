﻿using System.ComponentModel;
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
    [Editor]
    public int DelayedTextChangedTimeout { get; set; }

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
    #endregion
    #region hint events / methods
    private void OnTextChanged(Object? sender, EventArgs e)
    {
        if (!_hintRefresh)
        {
            //The text change either comes from the user or the application setting a default value
            IsHintActive = false;
        }
    }
    protected override void OnLostFocus(EventArgs e)
    {
        if (!IsHintActive && string.IsNullOrEmpty(base.Text))
        {
            IsHintActive = true;
            _hintRefresh = true;
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
            _hintRefresh = true;
            base.Text = "";
            _hintRefresh = false;
            if (_hintActiveChanged != null)
                _hintActiveChanged(this, EventArgs.Empty);
        }

        base.OnGotFocus(e);
    }
    #endregion
}