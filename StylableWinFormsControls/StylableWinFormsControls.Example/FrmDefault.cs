namespace StylableWinFormsControls.Example
{
    public partial class FrmDefault : Form
    {
        public FrmDefault()
        {
            InitializeComponent();
        }

        private void stylableButton1_Click(object sender, EventArgs e)
        {
            StylableMessageBox mBox = StylableMessageBox.BUILDER
                .WithTitle("This is a text", MessageBoxIcon.Information)
                .WithText("This is an example of a stylable MessageBox")
                .WithButtons(MessageBoxButtons.YesNoCancel)
                .WithCheckBox("Do you like it?")
                .WithHelpButton(new Uri("https://github.com/Assorted-Development/winforms-stylable-controls"))
                .WithTimeout(TimeSpan.FromSeconds(30), DialogResult.Cancel)
                .Build();
            mBox.ShowDialog();
        }

        private void stylableButton2_Click(object sender, EventArgs e)
        {
            StylableInputBox<NumericUpDown> iBox = StylableInputBox<NumericUpDown>.BUILDER
                .WithTitle("Numeric Test", MessageBoxIcon.Information)
                .WithText("Please enter a random number between -100 and 100")
                .WithHelpButton(new Uri("https://github.com/Assorted-Development/winforms-stylable-controls"))
                .WithTimeout(TimeSpan.FromSeconds(30), DialogResult.Cancel)
                .ForNumericValue(0, -100, 100);
            iBox.ShowDialog();
        }
    }
}
