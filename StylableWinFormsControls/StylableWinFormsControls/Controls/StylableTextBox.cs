using System.ComponentModel;
using StylableWinFormsControls.Native;

namespace StylableWinFormsControls;

public class StylableTextBox : TextBox
{
    private System.Windows.Forms.Timer? _mDelayedTextChangedTimer;

    public event EventHandler? DelayedTextChanged;

    public Color BorderColor { get; set; } = Color.Blue;

    /// <summary>
    /// used to set the color of the hint text
    /// </summary>
    public Color PlaceholderForeColor { get; set; } = Color.Gray;

    [Editor]
    public int DelayedTextChangedTimeout { get; set; }

    public bool IsDelayActive { get; set; } = true;

    public StylableTextBox()
    {
        DelayedTextChangedTimeout = 900; // 0.9 seconds
        BorderStyle = BorderStyle.FixedSingle;
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

    private bool IsHintActive => !string.IsNullOrEmpty(PlaceholderText) && !Focused && TextLength == 0;

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case NativeConstants.Messages.WM_PAINT:
            {
                base.WndProc(ref m);
                if (IsHintActive)
                {
                    drawPlaceholderText();
                }
            }

            break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
    /// <summary>
    ///  Draws the <see cref="PlaceholderText"/> in the client area of the <see cref="TextBox"/> using the default font and color.
    /// </summary>
    private void drawPlaceholderText()
    {
        using Graphics g = CreateGraphics();
        using SolidBrush background = new(BackColor);
        TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Top |
                            TextFormatFlags.EndEllipsis;
        Rectangle rectangle = ClientRectangle;

        if (RightToLeft == RightToLeft.Yes)
        {
            flags |= TextFormatFlags.RightToLeft;
        }
        switch (TextAlign)
        {
            case HorizontalAlignment.Center:
                flags |= TextFormatFlags.HorizontalCenter;
                break;
            case HorizontalAlignment.Left:
                flags |= TextFormatFlags.Left;
                break;
            case HorizontalAlignment.Right:
                flags |= TextFormatFlags.Right;
                break;
        }
        g.FillRectangle(background, ClientRectangle);
        TextRenderer.DrawText(g, PlaceholderText, Font, rectangle, PlaceholderForeColor, flags);
    }
}
