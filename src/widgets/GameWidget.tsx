import { useState } from 'react'
import './widgets.css'

export function GameWidget() {
  const [score, setScore] = useState(0)

  const handleTap = () => {
    setScore((s) => s + 1)
  }

  const handleReset = () => {
    setScore(0)
  }

  return (
    <div className="widget-shell">
      <div>
        <p className="widget-title">Break: mini tap</p>
        <p className="widget-subtitle">Tiny score-tapper for a quick reset</p>
      </div>
      <div className="game-block">
        <div className="game-score">
          <div>
            <span className="stat-label">Score</span>
            <span className="stat-value">{score}</span>
          </div>
        </div>
        <button className="widget-button game-button" onClick={handleTap}>
          Tap to score
        </button>
        <button className="widget-button ghost" onClick={handleReset}>
          Reset
        </button>
      </div>
    </div>
  )
}

export default GameWidget
