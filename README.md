# Second Monitor (Web)

Second Monitor is being rebuilt as a web-based React app for a calm, low-distraction second screen. Expect modular layouts you can rearrange, a mix of productivity helpers, ambient companions, and quick mini-games—all running in the browser.

## Current status
- Rebooting from the prior WPF app; web scaffolding lands next.
- Target stack: React + TypeScript with Vite for dev/build.
- Styling approach to be finalized (leaning utility-first with a few polished presets).

## Planned pillars
- **Modular layouts**: drag/drop cards, resize, and save presets.
- **Productivity**: pomodoro, quick notes, focus timers, and status glanceables.
- **Ambient**: quotes, breathing coach, gentle motion, and optional audio cues.
- **Playful breaks**: lightweight games sized for a corner of your second screen.
- **Offline-friendly**: keep running without a network once loaded.

## Local development (after scaffold lands)
1. Install Node.js 18+.
2. Clone the repo and install deps:
   ```bash
   npm install
   ```
3. Start the dev server:
   ```bash
   npm run dev
   ```
4. Build for production:
   ```bash
   npm run build
   ```
5. Run tests (once added):
   ```bash
   npm test
   ```

## Contributing / next steps
- Add the React/Vite scaffold and baseline UI shell.
- Define the component system and layout primitives (cards, columns, grids).
- Bring forward the previous widgets where they make sense, redesigned for web.
- Keep accessibility and low-distraction visuals as first-class goals.
