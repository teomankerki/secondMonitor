# Mega App — Second Monitor Sanctuary

Mega App is a Windows‑only .NET 8 WPF dashboard designed to keep your second screen calm, playful, and informative. Build your own layout from the ground up, mix focus tools with ambient toys, and rearrange everything via drag‑and‑drop.

## Highlights
- **Custom builder** – start with zero widgets, add preset categories (Productivity, Games, Vibes), and reorder anything with drag‑and‑drop.
- **Productivity toolkit** – Pomodoro timer, system pulse, quick notes, and modular cards that can be resized or duplicated.
- **Ambient vibes** – breathing guide, quote carousel, particle drift, and other passive companions.
- **Playful breaks** – Idle Garden clicker, 2048, Mini Sudoku, Minesweeper, the new Endless Runner mini‑game, and a vector‑based Fidget Spinner with motion blur and palette swapping.
- **Self‑contained** – runs completely offline with a muted theme designed for low distraction.

## Running the App
1. Install the **.NET 8.0 Desktop SDK or runtime** on Windows.
2. Clone this repo and open a terminal at the project root.
3. Run:
   ```powershell
   dotnet run
   ```
   The WPF shell launches; press `Ctrl+C` or close the window to exit.

### Developing in Visual Studio
1. Open `MegaApp.sln` in Visual Studio 2022 or newer.
2. Set `MegaApp` as the startup project.
3. Press **F5** to build and debug.

## Customizing the Dashboard
- Use the **Add Category** button to insert one of the preset sections. Each card can be renamed inline.
- Within a category, pick any widget from the dropdown and click **Add Widget**. Modules can appear multiple times or in different categories.
- Drag a widget tile to reorder it or move it into another category. Drag whole categories to rearrange the high‑level layout.
- Many cards (spinner, runner, garden, etc.) expose in‑module controls to tweak colors, difficulty, or interactions.

Feel free to fork, reskin modules, or drop in entirely new cards—the project structure keeps each widget isolated so experimentation stays easy.
