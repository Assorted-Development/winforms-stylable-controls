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
            StylableMessageBox messageBox = StylableMessageBox.BUILDER
                .WithTitle("This is a text", MessageBoxIcon.Information)
                .WithText("This is an example of a stylable MessageBox")
                .WithButtons(MessageBoxButtons.YesNoCancel)
                .WithCheckBox("Do you like it?")
                .WithHelpButton(new Uri("https://github.com/Assorted-Development/winforms-stylable-controls"))
                .WithTimeout(TimeSpan.FromSeconds(30), DialogResult.Cancel)
                .Build();
            messageBox.ShowDialog();
        }

        private void stylableButton2_Click(object sender, EventArgs e)
        {
            StylableNumericInputBox inputBox = StylableNumericInputBox.BUILDER
                .WithTitle("Numeric Test", MessageBoxIcon.Question)
                .WithText("Please enter a random number between -100 and 100")
                .WithButtons(MessageBoxButtons.OKCancel)
                .WithHelpButton(new Uri("https://github.com/Assorted-Development/winforms-stylable-controls"))
                .WithTimeout(TimeSpan.FromSeconds(30), DialogResult.Cancel)
                .ForNumericValue(0, -100, 100);

            if (inputBox.ShowDialog() == DialogResult.OK)
            {
                StylableMessageBox mBox = StylableMessageBox.BUILDER
                    .WithTitle("Result value", MessageBoxIcon.Information)
                    .WithText($"You entered the following value: {inputBox.Value}")
                    .WithButtons(MessageBoxButtons.OK)
                    .Build();
                mBox.ShowDialog();
            }
        }

        private void stylableCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show($"CheckState={stylableCheckBox1.CheckState}. Checked={stylableCheckBox1.Checked}");
        }
    }
}
