# winforms-stylable-controls

[![Nuget](https://img.shields.io/nuget/v/AssortedDevelopment.StylableWinFormsControls)](https://www.nuget.org/packages/AssortedDevelopment.StylableWinFormsControls)
[![Nuget](https://img.shields.io/nuget/dt/AssortedDevelopment.StylableWinFormsControls)](https://www.nuget.org/packages/AssortedDevelopment.StylableWinFormsControls)
[![Build Status](https://github.com/Assorted-Development/winforms-stylable-controls/actions/workflows/prerelease.yml/badge.svg)](https://github.com/Assorted-Development/winforms-stylable-controls/actions/workflows/prerelease.yml)

This project contains custom WinForms controls that support more styling options than the default ones. We use this project for better dark mode and theme support.  
(For themes, check out our other project [winforms-themes](https://github.com/Assorted-Development/winforms-themes)).

# License
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

# ToC
* [WinForms stylable controls](#winforms-stylable-controls)
  * [License](#license)
  * [Usage](#usage)
  * [Controls](#controls)
  * [Contributions](#contributions)

## Usage
To use this project, just add a reference to our nuget package (`dotnet add package AssortedDevelopment.StylableWinFormsControls`) and replace the standard controls with our custom controls. All the controls can be found in the namespace `StylableWinFormsControls`.

## Controls
This is the currently available list of custom controls and their extended features and known characteristics compared to the original WInForms controls.

### Button => StylableButton
Exposes the following additional properties:
* `Color EnabledHoverColor`: Gets/Sets the background color while the mouse is over the enabled Button.
* `Color EnabledBackColor`: Gets/Sets the background color of an enabled Button.
* `Color DisabledBackColor`: Gets/Sets the background color of a disabled Button.
* `Color EnabledForeColor`:  Gets/Sets the foreground color of an enabled Button.
* `Color DisabledForeColor`: Gets/Sets the foreground color of a disabled Button.
* `Color BorderColor`: Gets/sets the border color in any of the scenarios described above.

### CheckBox => StylableCheckBox
Exposes the following additional properties:
* `Color DisabledForeColor`: Gets/Sets the foreground color if the checkbox is disabled.

Has the following additional characteristics:
* Does not support AutoSize at the moment

### ComboBox => StylableComboBox
Exposes the following additional properties:
* `Color BorderColor`: Sets the color of the border around the combobox (not the item list box)
* `Color ItemHoverColor`: Sets the background color of the item in the list that's currently hovered/selected.

### DataGridView => StylableDataGridView
Exposes the following additional properties:
* `bool DoubleBuffered`: Exposes the underlying `Control.DoubleBuffered` to enable/disable double-buffering on the `DataGridView` control
* `bool EnableFirstColumnGrouping`: Gets/sets a value indicating whether multiple occurences of the same text in multiple rows of the first column should all be hidden except for the first one.

### DateTimePicker => StylableDateTimePicker
Exposes the following additional properties:
* `Color EnabledBackColor`: Gets/Sets the background color of an enabled DateTimePicker.
* `Color DisabledBackColor`: Gets/Sets the background color of a disabled DateTimePicker.
* `Color EnabledForeColor`:  Gets/Sets the foreground color of an enabled DateTimePicker.
* `Color DisabledForeColor`: Gets/Sets the foreground color of a disabled DateTimePicker.

Has the following additional characteristics:
* The control behavior via keyboard is currently unreliable (see #24)
* The `BackColor` property is additionally accessable via Designer

### GroupBox => StylableGroupBox
Exposes the following additional properties:
* `Color BorderColor`: Gets/sets the color of the border that surrounds the groupbox content.
* `Color EnabledForeColor`: Gets/sets the foreground color of the text/title painted inside the border when the groupbox is enabled.
* `Color DisabledForeColor`: Gets/sets the foreground color of the text/title painted inside the border when the groupbox is disabled.

Has the following additional characteristics:
* The `ForeColor` property is not accessable via Designer (to encourage use of the more specific color properties)

### Label => StylableLabel
Exposes the following additional properties:
* `Color DisabledForeColor`: Gets/Sets the foreground color of a disabled Label.

Has the following additional characteristics:
* Does not currently support AutoEllipsis on disabled labels

### ListView => StylableListView
Exposes the following additional properties:
* `Color GroupHeaderBackColor`: Sets the color that builds the background of any group header row.
* `Color GroupHeaderForeColor`: Sets the foreground color of any group header row.
* `Color SelectedItemBackColor`: Sets the color that builds the background of selected item rows.
* `Color SelectedItemForeColor`: Sets the foreground color of any selected item row.

Has the following additional characteristics:
* DoubleBuffering is forced to be enabled for technical reasons

### TabControl => StylableTabControl
Exposes the following additional properties:
* `Color BackgroundColor`: Sets the background color of the entire TabControl.
* `Color ActiveTabBackgroundColor`: Gets/sets the background color of the currently active tab rectangle.
* `Color ActiveTabForegroundColor`: Gets/sets the foreground color of the currently active tab rectangle.
* `Color BorderColor`: Sets color of the border in the TabControl itself and around the tabs
* `bool UseRoundedCorners`: Gets or sets a value indicating whether tab page controls have a corner radius or not.
* `Bitmap[] UpDownImages`: Gets or sets the currently used images for the UpDown control.
   [0] = Left, [1] = Right, [2] = Left (disabled), [3] = Right (disabled)
 
Has the following additional characteristics:
* Only supports `Alignment` values `Top` and `Bottom`
* `Multiline` is currently not supported

### TextBox => StylableTextBox
Exposes the following additional properties:
* `Color BorderColor`: Gets/Sets the color of the border around the TextBox
* `Color HintForeColor`: Gets/Sets the foreground color of the hint text
* `Color TextForeColor`: Gets/Sets the foreground color of non-hint text
* `string Hint`: Gets/Sets the value to be shown if no user-provided text or default text is shown
* `bool IsHintActive`: Gets a value indicating whether the text currently shows a hint text or not.
* `bool IsDelayActive`: Gets/Sets a value indicating whether the `TextChanged` event is to be triggered only after a specified amount of time.
* `DelayedTextChangedTimeout`: Gets/Sets the delay time for triggering the `TextChanged` event after the last user input

Has the following additional characteristics:
* Supports providing hints to be shown when the control does not have focus and the value is empty.
* Supports delaying `TextChanged` events

### Interaction.InputBox => StylableInputBox
With the `StylableInputBox`, you can create forms similar to VB.NETs `Interaction.InputBox` but the handling is a bit different as we allow you to style the form
before showing.

Let's create an input box first:  
```csharp
StylableInputBox<NumericUpDown> iBox = StylableInputBox<NumericUpDown>.BUILDER
                .WithTitle("Numeric Test", MessageBoxIcon.Information)
                .WithText("Please enter a random number between -100 and 100")
                .WithHelpButton(new Uri("https://github.com/Assorted-Development/winforms-stylable-controls"))
                .WithTimeout(TimeSpan.FromSeconds(30), DialogResult.Cancel)
                .ForNumericValue(0, -100, 100);
```
This will create an input box for numeric values (currently, we support text and numeric) with a title and a message and two buttons: Okay, Cancel
Now, let's style the form as we want to: `iBox.StylableControls.Text.ForeColor = Color.Red;`
At last, you can show the dialog using `iBox.ShowDialog()`

**Note:** If you change the size on the controls (e.g. increase the font size), please call `UpdateSize()` to update the UI to the new settings. Otherwise, the Ui may look weird.


## Contributions

Please view the [contributing guide](/CONTRIBUTING.md) for more information.
