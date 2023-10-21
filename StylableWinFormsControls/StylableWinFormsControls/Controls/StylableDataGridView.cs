namespace StylableWinFormsControls;

public class StylableDataGridView : DataGridView
{
    private Form? _parentForm;

    public new bool DoubleBuffered
    {
        get => base.DoubleBuffered;
        set => base.DoubleBuffered = value;
    }

    /// <summary>
    /// Specifies which ScrollBars are visible on the DGV.
    /// </summary>
    /// <remarks> Overrides default scrollbar to make styling it possible.</remarks>
    public new ScrollBars ScrollBars { get; set; }

    public bool EnableFirstColumnGrouping { get; set; } = true;

    protected Form ParentForm => _parentForm ??= FindForm();

    public StylableDataGridView()
    {
        DoubleBuffered = true;
    }

    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnCellPainting(e);

        if (!EnableFirstColumnGrouping)
        {
            return;
        }

        // Ignore column and row headers, first row, not sorted columns and not-frozen columns
        if (e.RowIndex < 1 || e.RowIndex >= Rows.Count - 1 || e.ColumnIndex < 0 || e.ColumnIndex != SortedColumn?.Index || !Columns[e.ColumnIndex].Frozen)
        {
            return;
        }

        e.AdvancedBorderStyle.Top =
            DataGridViewAdvancedCellBorderStyle.None;

        e.AdvancedBorderStyle.Bottom =
            isRepeatedCellValue(e.RowIndex + 1, e.ColumnIndex)
                ? DataGridViewAdvancedCellBorderStyle.None
                : AdvancedCellBorderStyle.Bottom;
    }

    protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        // Call home to base
        base.OnCellFormatting(e);

        // First row always gets displayed
        if (e.RowIndex == 0 || e.ColumnIndex != SortedColumn?.Index || !Columns[e.ColumnIndex].Frozen)
        {
            return;
        }

        if (isRepeatedCellValue(e.RowIndex, e.ColumnIndex))
        {
            e.Value = string.Empty;

            e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            e.FormattingApplied = true;
        }
    }

    private bool isRepeatedCellValue(int rowIndex, int colIndex)
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
            (currentCell.Value is not null && prevCell.Value is not null &&
            currentCell.Value.ToString() == prevCell.Value.ToString()))
        {
            return true;
        }

        return false;
    }
}
