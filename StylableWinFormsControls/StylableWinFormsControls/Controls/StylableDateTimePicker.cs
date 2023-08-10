﻿using StylableWinFormsControls.Extensions;
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
    private readonly List<DatePartInfo> _dateParts = new();

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
        this.KeyPress += StylableDateTimePicker_KeyPress;
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
    /// replace a part of the date with the manually entered input
    /// </summary>
    /// <param name="date">the old date</param>
    /// <param name="format">the full format</param>
    /// <param name="partFormat">the part format</param>
    /// <param name="value">the new part value</param>
    /// <returns></returns>
    private static DateTime Replace(DateTime date, string format, string partFormat, string value)
    {
        //remove the day of week from the format as DateTime.ParseExact throws if date and day of week do not match
        format = format.Replace("dddd", "");
        format = format.Replace("ddd", "");
        //remove the ful month name as this is not numeric and we do not support it currently
        format = format.Replace("MMMM", "");
        format = format.Replace("MMM", "");
        //convert the old date to a string where the part to replace is XXXX
        string partFormatRegex = @"\b" + Regex.Replace(partFormat, "[^a-zA-Z0-9]+", "") + @"\b";
        string formattedDate = date.ToString(Regex.Replace(format, partFormatRegex, @"\X\X\X\X"));
        //now we can simply replace the XXXX with the new value and reparse the date
        string replacedDateString = formattedDate.Replace("XXXX", value);
        var replacedDate = DateTime.ParseExact(replacedDateString, format, CultureInfo.CurrentCulture);
        return replacedDate;
    }

    /// <summary>
    /// this draw the datetime text manually so we can save the information where day, month and year is
    /// </summary>
    /// <param name="g"></param>
    private void DrawDateTime(Graphics g)
    {
        // Drawing the datetime text
        string format = GetFormat();
        //recalculate the positions of the DateTime parts within the DateTimePicker
        RecalcPartPositions(format);
        //draw the DateTime parts
        var brush = new SolidBrush(this.ForeColor);
        var highlightBrush = new SolidBrush(this.ForeColor.Highlight());
        foreach (var part in _dateParts)
        {
            Rectangle bounds = new(part.Start, 2, part.End - part.Start, this.Height - 2);
            if (part.Selected)
                TextRenderer.DrawText(g, this.Value.ToString(part.Format), Font, bounds, this.ForeColor.Highlight(), TextFormatFlags.EndEllipsis);
            else
                TextRenderer.DrawText(g, this.Value.ToString(part.Format), Font, bounds, this.ForeColor, TextFormatFlags.EndEllipsis);
        }
    }

    private string GetFormat()
    {
        DateTimeFormatInfo dtfi = (CultureInfo.CurrentCulture).DateTimeFormat;
        string format = Format switch
        {
            DateTimePickerFormat.Long => dtfi.LongDatePattern,
            DateTimePickerFormat.Short => dtfi.ShortDatePattern,
            DateTimePickerFormat.Time => dtfi.LongTimePattern,
            _ => CustomFormat,
        };
        return format;
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
    /// move the selection to the next part
    /// </summary>
    /// <param name="part"></param>
    /// <returns>the newly selected part</returns>
    private DatePartInfo NavigatePartRight(DatePartInfo part)
    {
        part.Reset();
        var index = _dateParts.IndexOf(part);
        DatePartInfo newPart;
        if (index == _dateParts.Count - 1)
            newPart = _dateParts.First();
        else
            newPart = _dateParts[index + 1];
        newPart.Selected = true;
        return newPart;
    }

    /// <summary>
    /// navigate through the DateTime parts with the arrow keys
    /// </summary>
    /// <param name="keyCode"></param>
    private void NavigateParts(Keys keyCode)
    {
        var part = _dateParts.FirstOrDefault(p => p.Selected);
        if (part == null && keyCode == Keys.Left)
        {
            _dateParts.Last().Selected = true;
        }
        else if (part != null && keyCode == Keys.Left)
        {
            part.Reset();
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
            NavigatePartRight(part);
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
    private void StylableDateTimePicker_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
        {
            IncrementDecrement(e.KeyCode);
        }
        else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
        {
            NavigateParts(e.KeyCode);
            this.Invalidate();
        }
        e.Handled = true;
    }

    /// <summary>
    /// allow directly editing a value with the number keys
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StylableDateTimePicker_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (Char.IsNumber(e.KeyChar))
        {
            var part = _dateParts.FirstOrDefault(p => p.Selected);
            if (part != null)
            {
                part.NewValue += e.KeyChar;
                if (part.NewValue.Length >= part.Format.Length)
                {
                    this.Value = Replace(this.Value, GetFormat(), part.Format, part.NewValue);
                    part = NavigatePartRight(part);
                }
                this.Invalidate();
            }
        }
        e.Handled = true;
    }

    /// <summary>
    /// reset select part of the format when the control lost focus
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StylableDateTimePicker_Leave(object? sender, EventArgs e)
    {
        _dateParts.ForEach(p => p.Reset());
        this.Invalidate();
    }

    /// <summary>
    /// support selecting the DateTime parts with the mouse to change them
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StylableDateTimePicker_MouseDown(object? sender, MouseEventArgs e)
    {
        _dateParts.ForEach(p => p.Reset());
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
            if (part.Format.StartsWith("M"))
            {
                UpdatePartMonth(num);
            }
            else if (part.Format.StartsWith("y"))
            {
                UpdatePartYear(num);
            }
            else
            {
                UpdatePartGeneric(num, part);
            }
            this.Invalidate();
        }
    }

    /// <summary>
    /// generically increment or decrement the selected part of the DateTime. Does not work for months and years
    /// </summary>
    /// <param name="num"></param>
    /// <param name="part"></param>
    private void UpdatePartGeneric(int num, DatePartInfo part)
    {
        //create a timespan with the given number of days, hours, minutes or seconds
        int absNumber = Math.Abs(num);
        DateTime tmp = new(absNumber, absNumber, absNumber, absNumber, absNumber, absNumber);
        var dFmt = part.Format;
        var tsFmt = Regex.Replace(dFmt, @"([^a-zA-Z0-9]+)", "\\$1").ToLower();
        //special handling for fullname of days as we want to get a numeric value to add or subtract
        if (dFmt.StartsWith("d") && dFmt.Length > 3)
        {
            dFmt = "dd";
            tsFmt = "dd";
        }
        TimeSpan ts = TimeSpan.ParseExact(tmp.ToString(dFmt, CultureInfo.CurrentCulture), tsFmt, CultureInfo.CurrentCulture);
        if (num > 0)
            this.Value = this.Value.Add(ts);
        else
            this.Value = this.Value.Subtract(ts);
    }

    /// <summary>
    /// increment or decrement the month part of the DateTime
    /// </summary>
    /// <param name="num"></param>
    private void UpdatePartMonth(int num)
    {
        this.Value = this.Value.AddMonths(num);
    }

    /// <summary>
    /// increment or decrement the year part of the DateTime
    /// </summary>
    /// <param name="num"></param>
    private void UpdatePartYear(int num)
    {
        this.Value = this.Value.AddYears(num);
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
        public string Format { get; set; } = string.Empty;

        public string NewValue { get; set; } = string.Empty;

        /// <summary>
        /// if true, this part is selected and can be changed by the user
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// the start X position of the DateTime part
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        ///
        /// </summary>
        public void Reset()
        {
            NewValue = string.Empty;
            Selected = false;
        }
    }
}