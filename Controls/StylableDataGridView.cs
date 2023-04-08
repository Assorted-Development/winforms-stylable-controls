using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MFBot_1701_E.CustomControls
{
    public class StylableDataGridView : DataGridView
    {
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
    }
}
