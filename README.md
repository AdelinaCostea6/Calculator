# Calculator

This C# WPF-based calculator application follows the MVVM (Model-View-ViewModel) pattern, designed to handle standard, scientific, and programmer modes. The application supports various mathematical operations, including basic arithmetic, square root, square, inverse, and modulus, alongside number base conversions (binary, octal, decimal, hexadecimal).

## Key Features

### 1. **Standard and Programmer Modes**
- The app switches between **Standard Mode** and **Programmer Mode**.
- In **Programmer Mode**, users can perform calculations in binary, octal, decimal, and hexadecimal bases.

### 2. **Basic Operations**
- Supports operations like addition, subtraction, multiplication, division, and modulus.
- Includes advanced operations like square root, square, inverse, and negation.

### 3. **Memory and Clipboard**
- **Memory Functions**: Store, recall, add, subtract, and clear memory values.
- **Clipboard Operations**: Copy, cut, and paste functionality.

### 4. **Number Base Conversion**
- Users can input numbers in various bases (binary, octal, decimal, hexadecimal).
- The application automatically converts the values and updates all related base displays.

### 5. **History**
- Displays the history of operations for easy tracking of calculations.

### 6. **Settings**
- The application saves settings such as digit grouping, selected base, and mode preference (programmer or standard).

## How It Works

### **ViewModel:**
- Handles user interaction, input validation, and performs operations based on the input.
- Commands are bound to buttons for calculator operations (numbers, operations, memory actions, etc.).

### **Model:**
- Contains methods for basic arithmetic operations (`Add`, `Subtract`, `Multiply`, `Divide`), scientific functions (`SquareRoot`, `Square`), and base conversion.

### **View:**
- The interface is built with **XAML** and is data-bound to the `ViewModel` for dynamic updates.
- Keyboard shortcuts (like `NumPad` and `Enter`) are supported for user convenience.

## Technologies Used
- **C#**
- **WPF (Windows Presentation Foundation)**
- **MVVM Pattern**
- **JSON** for saving user settings

