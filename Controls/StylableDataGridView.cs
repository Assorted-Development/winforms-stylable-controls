using System;
using System.Drawing;
using System.Windows.Forms;

namespace MFBot_1701_E.CustomControls
{
    public class StylableDataGridView : DataGridView
    {
        private FlatScrollBar _vScrollBar;
        private FlatScrollBar _hScrollBar;
        private Form _parentForm;

        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        // Override default scrollbar to make styling it possible.
        public new ScrollBars ScrollBars { get; set; }

        public bool EnableFirstColumnGrouping { get; set; } = true;

        protected Form ParentForm => _parentForm ??= FindForm();

        public StylableDataGridView()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_vScrollBar is not null)
            {
                if (IsScrollbarVisible(true))
                {
                    int columnHeadersHeight = (ColumnHeadersHeight / Rows[0].Height);
                    int rowTotalCount = RowCount - columnHeadersHeight;

                    _vScrollBar.Minimum = 0;
                    _vScrollBar.Maximum = rowTotalCount - 1 - columnHeadersHeight;
                    _vScrollBar.LargeChange = 5;
                    _vScrollBar.SmallChange = 1;

                    AdjustVScrollBarSize();
                    AdjustVScrollBarLocation();

                    _vScrollBar.Visible = true;
                }
                else
                {
                    _vScrollBar.Visible = false;
                }
            }

            if (_hScrollBar is not null)
            {
                if (IsScrollbarVisible(false))
                {
                    int frozenColumnsWidth = Columns.GetColumnsWidth(DataGridViewElementStates.Frozen);
                    int columnTotalWidth =
                        Columns.GetColumnsWidth(DataGridViewElementStates.Visible);

                    AdjustHScrollBarSize();
                    AdjustHScrollBarLocation();

                    _hScrollBar.Minimum = 0;
                    _hScrollBar.Maximum = columnTotalWidth - _hScrollBar.Width;
                    _hScrollBar.LargeChange = Math.Max(2, _hScrollBar.Maximum / 2);
                    _hScrollBar.SmallChange = 1;

                    _hScrollBar.Visible = true;
                }
                else
                {
                    _hScrollBar.Visible = false;
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            const int minimum = 0;
            _vScrollBar = new FlatScrollBar() { Orientation = ScrollOrientation.VerticalScroll, Value = minimum, Minimum = minimum };
            ParentForm.Controls.Add(_vScrollBar);
            _vScrollBar.BringToFront();
            _vScrollBar.Visible = false;

            _vScrollBar.Scroll += this.VerticalScrollBar_Scroll;

            _hScrollBar = new FlatScrollBar() { Orientation = ScrollOrientation.HorizontalScroll, Height = SystemInformation.HorizontalScrollBarHeight };
            ParentForm.Controls.Add(_hScrollBar);
            _hScrollBar.BringToFront();
            _hScrollBar.Visible = false;

            _hScrollBar.Scroll += this.HorizontalScrollBar_Scroll;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (_vScrollBar is not null)
            {
                if (ParentForm?.Controls.Contains(_vScrollBar) == true)
                {
                    ParentForm.Controls.Remove(_vScrollBar);
                }
                _vScrollBar.Dispose();
            }

            if (_hScrollBar is not null)
            {
                if (ParentForm?.Controls.Contains(_hScrollBar) == true)
                {
                    ParentForm.Controls.Remove(_hScrollBar);
                }

                _hScrollBar.Dispose();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            AdjustVScrollBarSize();
            AdjustVScrollBarLocation();

            AdjustHScrollBarSize();
            AdjustHScrollBarLocation();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            AdjustVScrollBarLocation();
            AdjustHScrollBarLocation();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            if (se.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                SetVScrollBarValue(se.NewValue);
            }
            else if (_hScrollBar is not null)
            {
                SetHScrollBarValue(se.NewValue);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (_vScrollBar is not null)
                _vScrollBar.Visible = Visible;
            if (_hScrollBar is not null)
                _hScrollBar.Visible = Visible;
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs args)
        {
            base.OnCellPainting(args);

            if (!EnableFirstColumnGrouping)
            {
                return;
            }

            // Ignore column and row headers, first row, not sorted columns and not-frozen columns 
            if (args.RowIndex < 1 || args.ColumnIndex < 0 || args.ColumnIndex != SortedColumn?.Index || !Columns[args.ColumnIndex].Frozen)
                return;

            args.AdvancedBorderStyle.Bottom =
              DataGridViewAdvancedCellBorderStyle.None;

            args.AdvancedBorderStyle.Top =
                IsRepeatedCellValue(args.RowIndex, args.ColumnIndex)
                    ? DataGridViewAdvancedCellBorderStyle.None
                    : AdvancedCellBorderStyle.Top;
        }


        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs args)
        {
            // Call home to base
            base.OnCellFormatting(args);

            // First row always gets displayed
            if (args.RowIndex == 0 || args.ColumnIndex != SortedColumn?.Index || !Columns[args.ColumnIndex].Frozen)
                return;

            if (IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
            {
                args.Value = string.Empty;

                args.CellStyle.Font = new Font(args.CellStyle.Font, FontStyle.Bold);
                args.FormattingApplied = true;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (_vScrollBar is not null)
                    {
                        ParentForm.Controls.Remove(_vScrollBar);
                        _vScrollBar.Dispose();
                    }

                    if (_hScrollBar is not null)
                    {
                        ParentForm.Controls.Remove(_hScrollBar);
                        _hScrollBar.Dispose();
                    }
                }

                catch (Exception)
                {
                    // Do nothing
                }
            }

            base.Dispose(disposing);
        }

        private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SetVScrollBarValue(e.NewValue);
        }

        private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SetHScrollBarValue(e.NewValue);
        }

        private void AdjustVScrollBarLocation()
        {
            if (_vScrollBar is not null)
            {
                Point positionOnForm = GetLocationOnForm(this);
                _vScrollBar.Location = BorderStyle == BorderStyle.None
                    ? new Point(positionOnForm.X + Width - SystemInformation.VerticalScrollBarWidth, positionOnForm.Y)
                    : new Point(positionOnForm.X + Width - (SystemInformation.VerticalScrollBarWidth + 1), positionOnForm.Y + 1);
            }
        }

        private void AdjustVScrollBarSize()
        {
            if (_vScrollBar is not null)
            {
                int h = Height;

                if (BorderStyle != BorderStyle.None)
                    h -= 2;
                if (IsScrollbarVisible(false))
                    h -= SystemInformation.HorizontalScrollBarHeight;

                _vScrollBar.Height = h;
            }
        }

        private void SetVScrollBarValue(int value)
        {
            if (_vScrollBar is not null)
            {
                _vScrollBar.Value = value;
                FirstDisplayedScrollingRowIndex = Math.Min(value, Rows.Count - 1);
            }
        }

        private void AdjustHScrollBarLocation()
        {
            Point positionOnForm = GetLocationOnForm(this);
            if (_hScrollBar is not null)
            {
                _hScrollBar.Location = BorderStyle == BorderStyle.None
                    ? new Point(positionOnForm.X, positionOnForm.Y + Height - SystemInformation.HorizontalScrollBarHeight)
                    : new Point(positionOnForm.X + 1, positionOnForm.Y + Height - (SystemInformation.HorizontalScrollBarHeight + 1));
            }
        }

        private void AdjustHScrollBarSize()
        {
            if (_hScrollBar is not null)
            {
                int w = Width;

                if (BorderStyle != BorderStyle.None)
                    w -= 2;
                if (IsScrollbarVisible(true))
                    w -= SystemInformation.VerticalScrollBarWidth;

                _hScrollBar.Width = w;
            }
        }

        private void SetHScrollBarValue(int value)
        {
            if (_hScrollBar is not null)
            {
                _hScrollBar.Value = value;
                HorizontalScrollingOffset = value;
            }
        }

        private bool IsScrollbarVisible(bool vertical)
        {
            if (vertical)
            {
                return VerticalScrollBar?.Visible ?? false;
            }

            return HorizontalScrollBar?.Visible ?? false;
        }

        private bool IsRepeatedCellValue(int rowIndex, int colIndex)
        {
            if (!EnableFirstColumnGrouping)
            {
                return false;
            }

            DataGridViewCell currentCell =
                Rows[rowIndex].Cells[colIndex];
            DataGridViewCell prevCell =
                Rows[rowIndex - 1].Cells[colIndex];

            if (currentCell.Value == prevCell.Value ||
                currentCell.Value != null && prevCell.Value != null &&
                currentCell.Value.ToString() == prevCell.Value.ToString())
            {
                return true;
            }

            return false;
        }

        private Point GetLocationOnForm(Control control)
        {
            // Skip this if the controls handle isn't yet created as this creates buggy behaviour
            if (!control.IsHandleCreated)
            {
                return Point.Empty;
            }

            // Get position relative to screen and from that the position relative to the form
            return ParentForm.PointToClient(control.PointToScreen(Point.Empty));
        }
    }
}
