using StylableWinFormsControls.Extensions;
using System.ComponentModel;
using System.Drawing.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

namespace StylableWinFormsControls;

/// <summary>
/// the default datetime picker does not support styling
/// </summary>
public class StylableDateTimePicker : DateTimePicker
{
    /// <summary>
    /// separate two spaces with the given width
    /// </summary>
    public const int PART_SPACE = 5;

    /// <summary>
    /// contains all DateTime parts and their positions
    /// </summary>
    private readonly List<DatePartInfo> _dateParts = new List<DatePartInfo>();

    /// <summary>
    /// when the format is changed we need to recalculate the positions of the parts
    /// </summary>
    private string _oldFormat = "";

    /// <summary>
    /// when the width is changed we need to recalculate the positions of the parts
    /// </summary>
    private int _oldWidth = 0;

    public StylableDateTimePicker()
    {
        this.SetStyle(ControlStyles.UserPaint, true);
        this.MouseDown += StylableDateTimePicker_MouseDown;
        this.Leave += StylableDateTimePicker_Leave;
        this.MouseWheel += StylableDateTimePicker_MouseWheel;
        this.KeyDown += StylableDateTimePicker_KeyDown;
    }

    /// <summary>
    ///  Gets or sets the background color of the control
    ///  </summary>
    [Browsable(true)]
    public override Color BackColor
    {
        get
        {
            return base.BackColor;
        }
        set
        {
            base.BackColor = value;
        }
    }

    public Color DisabledBackColor { get; set; } = Color.Gray;
    public Color DisabledForeColor { get; set; } = Color.Black;
    public Color EnabledBackColor { get; set; } = Color.White;
    public Color EnabledForeColor { get; set; } = Color.Black;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams handleParam = base.CreateParams;
            //prevent flickering of the control
            handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
            return handleParam;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = this.CreateGraphics();
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // Dropdownbutton rectangle
        Rectangle ddb_rect = new Rectangle(ClientRectangle.Width - 17, 0, 17, ClientRectangle.Height);
        // Background brush
        Brush bb;
        //foreground brush
        Brush fb;

        ComboBoxState visual_state;

        // When enabled the brush is set to Backcolor,
        // otherwise to color stored in _disabled_back_Color
        if (this.Enabled)
        {
            bb = new SolidBrush(EnabledBackColor);
            fb = new SolidBrush(EnabledForeColor);
            visual_state = ComboBoxState.Normal;
        }
        else
        {
            bb = new SolidBrush(DisabledBackColor);
            fb = new SolidBrush(DisabledForeColor);
            visual_state = ComboBoxState.Disabled;
        }

        // Filling the background
        g.FillRectangle(bb, 0, 0, ClientRectangle.Width, ClientRectangle.Height);

        // Drawing the datetime
        DrawDateTime(g);

        // Drawing the dropdownbutton using ComboBoxRenderer
        ComboBoxRenderer.DrawDropDownButton(g, ddb_rect, visual_state);

        g.Dispose();
        bb.Dispose();
        fb.Dispose();
    }

    /// <summary>
    /// this draw the datetime text manually so we can save the information where day, month and year is
    /// </summary>
    /// <param name="g"></param>
    private void DrawDateTime(Graphics g)
    {
        // Drawing the datetime text
        string format;
        DateTimeFormatInfo dtfi = (new CultureInfo("hr-HR")).DateTimeFormat;
        switch (Format)
        {
            case DateTimePickerFormat.Long:
                format = dtfi.LongDatePattern;
                break;

            case DateTimePickerFormat.Short:
                format = dtfi.ShortDatePattern;
                break;

            case DateTimePickerFormat.Time:
                format = dtfi.LongTimePattern;
                break;

            default:
                format = CustomFormat;
                break;
        }
        //recalculate the positions of the DateTime parts within the DateTimePicker
        RecalcPartPositions(format);
        //draw the DateTime parts
        var brush = new SolidBrush(this.ForeColor);
        var highlightBrush = new SolidBrush(this.ForeColor.Highlight());
        foreach (var part in _dateParts)
        {
            if (part.Selected)
                g.DrawString(this.Value.ToString(part.Format), this.Font, highlightBrush, part.Start, 2);
            else
                g.DrawString(this.Value.ToString(part.Format), this.Font, brush, part.Start, 2);
        }
    }

    /// <summary>
    /// increment/decrement the selected part
    /// </summary>
    /// <param name="key"></param>
    private void IncrementDecrement(Keys key)
    {
        if (key == Keys.Up)
            UpdatePart(1);
        else if (key == Keys.Down)
            UpdatePart(-1);
    }

    /// <summary>
    /// navigate through the DateTime parts with the arrow keys
    /// </summary>
    /// <param name="keyCode"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void NavigateParts(Keys keyCode)
    {
        var part = _dateParts.FirstOrDefault(p => p.Selected);
        if (part == null && keyCode == Keys.Left)
        {
            _dateParts.Last().Selected = true;
        }
        else if (part != null && keyCode == Keys.Left)
        {
            part.Selected = false;
            var index = _dateParts.IndexOf(part);
            if (index == 0)
                _dateParts.Last().Selected = true;
            else
                _dateParts[index - 1].Selected = true;
        }
        else if (part == null && keyCode == Keys.Right)
        {
            _dateParts.First().Selected = true;
        }
        else if (part != null && keyCode == Keys.Right)
        {
            part.Selected = false;
            var index = _dateParts.IndexOf(part);
            if (index == _dateParts.Count - 1)
                _dateParts.First().Selected = true;
            else
                _dateParts[index + 1].Selected = true;
        }
    }

    /// <summary>
    /// recalculate the positions of the DateTime parts
    /// </summary>
    /// <param name="format"></param>
    private void RecalcPartPositions(string format)
    {
        if (_oldFormat != format || _oldWidth != this.Width)
        {
            //reset old list
            _dateParts.Clear();

            //split the format into parts
            var parts = Regex.Split(format, @"(?<=[^0-9a-zA-Z]+)");
            int startPos = 5;
            foreach (var part in parts)
            {
                if (String.IsNullOrWhiteSpace(part)) continue;
                //get the size of the part
                var size = TextRenderer.MeasureText(this.Value.ToString(part), this.Font);
                //add the part to the list
                _dateParts.Add(new DatePartInfo()
                {
                    Format = part,
                    Start = startPos,
                    End = startPos + size.Width + PART_SPACE
                });
                startPos += size.Width + PART_SPACE;
            }

            //store the format and width so we do not have to recalculate the positions if nothing changed
            _oldFormat = format;
            _oldWidth = this.Width;
        }
    }

    /// <summary>
    /// allow navigating through the DateTime parts with the arrow keys and increment/decrement the selected part or
    /// specify the value with the number keys
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void StylableDateTimePicker_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
        {
            IncrementDecrement(e.KeyCode);
        }
        else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
        {
            NavigateParts(e.KeyCode);
        }
    }

    /// <summary>
    /// reset select part of the format when the control lost focus
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void StylableDateTimePicker_Leave(object? sender, EventArgs e)
    {
        _dateParts.ForEach(p => p.Selected = false);
        this.Invalidate();
    }

    /// <summary>
    /// support selecting the DateTime parts with the mouse to change them
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void StylableDateTimePicker_MouseDown(object? sender, MouseEventArgs e)
    {
        _dateParts.ForEach(p => p.Selected = false);
        var part = _dateParts.FirstOrDefault(p => p.Start <= e.X && p.End >= e.X);
        if (part != null)
        {
            part.Selected = true;
            this.Invalidate();
        }
    }

    /// <summary>
    /// Allow changing the current value with the mouse wheel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void StylableDateTimePicker_MouseWheel(object? sender, MouseEventArgs e)
    {
        UpdatePart(e.Delta / 100);
    }

    /// <summary>
    /// update the selected part of the DateTime after user change
    /// </summary>
    /// <param name="num"></param>
    private void UpdatePart(int num = 1)
    {
        var part = _dateParts.FirstOrDefault(p => p.Selected);
        if (num != 0 && part != null)
        {
            //create a timespan with the given number of days, months, years, hours, minutes or seconds
            int absNumber = Math.Abs(num);
            DateTime tmp = new(absNumber, absNumber, absNumber, absNumber, absNumber, absNumber);
            var tsFmt = Regex.Replace(part.Format, @"([^a-zA-Z0-9]+)", "\\$1").ToLower();
            TimeSpan ts = TimeSpan.ParseExact(tmp.ToString(part.Format, CultureInfo.CurrentCulture), tsFmt, CultureInfo.CurrentCulture);
            if (num > 0)
                this.Value = this.Value.Add(ts);
            else
                this.Value = this.Value.Subtract(ts);
        }
    }

    /// <summary>
    /// contains the information about the DateTime part and its position
    /// </summary>
    private class DatePartInfo
    {
        /// <summary>
        /// the end X position of the DateTime part
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// the format of the DateTime part
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// if true, this part is selected and can be changed by the user
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// the start X position of the DateTime part
        /// </summary>
        public int Start { get; set; }
    }
}