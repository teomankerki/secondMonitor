# Module Catalogue

Use this page as a quick reference for everything currently shipped inside Mega App. Each widget is a self-contained WPF user control that you can insert into any category of the dashboard builder.

## Productivity Cards
| Widget | Purpose | Notes |
| --- | --- | --- |
| **Pomodoro Timer** | Classic 25/5 focus loop with pause/reset controls and live progress bar. | Readable `mm:ss` or `hh:mm:ss` output and auto phase switching. |
| **System Pulse** | Shows day, time, and app memory usage. | Lightweight clock that updates every second. |
| **Quick Notes** | Scratchpad with add/delete/clear actions. | Notes are stacked with inline delete buttons. |
| **Quick Controls** | Category headers and drag handles for layout management. | Available via the dashboard UI (not a standalone widget). |

## Games & Toys
| Widget | Summary | Extra Detail |
| --- | --- | --- |
| **Idle Garden** | Clicker that levels up a plant spirit via energy taps. | Progress bar resets per level with increasing requirements. |
| **Mini Sudoku** | 4x4 grid with multiple puzzle presets. | “Reset” restores the current board; “Restart” loads a new puzzle. |
| **2048 Clone** | Arrow-key inspired tile sliding game with on-card controls. | Includes reset and directional buttons. |
| **Minesweeper** | 6x6 grid with flags, restart, and reset. | Reset keeps the layout; Restart generates a new mine map. |
| **Fidget Spinner** | Vector spinner with palette toggle, motion blur, and spark flashes. | Spin/Stop buttons plus drag-driven velocity (bound to angle animation). |
| **Endless Runner** | Side-view track with jump/reset buttons. | Obstacles spawn along a 14-tile path; score increments over time. |

## Vibes & Ambient
| Widget | Mood | Notes |
| --- | --- | --- |
| **Quote Carousel** | Rotates uplifting affirmations. | Fires on a timer, styled for low glare. |
| **Calm Breathing** | Expanding orb guiding inhale/exhale cadence. | Configurable via app resources. |
| **Ambient Drift** | Particle-based animation for subtle motion. | Great filler for empty layout cells. |

## Builder Features
- **Preset Categories** – Productivity, Games, and Vibes templates can be added multiple times.
- **Drag-and-drop layout** – Rearrange entire categories or individual widgets by dragging their cards.
- **Duplicate modules** – Same widget can appear in several sections (e.g., two Pomodoro timers with different durations).
- **Theme aware** – Each module respects the global palette, but some (spinner) expose additional theme toggles.

To add new modules, create a user control inside `src/MegaApp/Modules`, back it with a view model under `src/MegaApp/ViewModels`, and register it in `DashboardViewModel` so it appears in the widget picker.
