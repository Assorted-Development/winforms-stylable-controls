# winforms-stylable-controls

[![Nuget](https://img.shields.io/nuget/v/AssortedDevelopment.StylableWinFormsControls)](https://www.nuget.org/packages/AssortedDevelopment.StylableWinFormsControls)
[![Nuget](https://img.shields.io/nuget/dt/AssortedDevelopment.StylableWinFormsControls)](https://www.nuget.org/packages/AssortedDevelopment.StylableWinFormsControls)
[![Build Status](https://github.com/Assorted-Development/winforms-stylable-controls/actions/workflows/prerelease.yml/badge.svg)](https://github.com/Assorted-Development/winforms-stylable-controls/actions/workflows/prerelease.yml)

This project contains custom WinForms controls that support more styling options than the default ones. We use this project for better dark mode and theme support (For themes, check out our other project [winforms-themes](https://github.com/Assorted-Development/winforms-themes) ).

# License
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

# ToC
* [WinForms stylable controls](#winforms-stylable-controls)
  * [License](#license)
  * [Usage](#usage)
  * [Controls](#controls)
  * [Contributions](#contributions)

## Usage
To use this project, just add a reference to our nuget package (`dotnet add package AssortedDevelopment.StylableWinFormsControls`) and replace the standard controls by our custom controls. All the controls can be found in the namespace `StylableWinFormsControls`.

## Controls
This is the currently available list of custom controls.

### Button => StylableButton

### CheckBox => StylableCheckBox

### ComboBox => StylableComboBox

### DataGridView => StylableDataGridView

### DateTimePicker => StylableDateTimePicker

### Label => StylableLabel

### ListView => StylableListView

### TabControl => StylableTabControl

### TextBox => StylableTextBox
This Textbox supports providing hints to be shown when the control does not have focus and the value is empty.

## Contributions

Please view the [contributing guide](/CONTRIBUTING.md) for more information.
