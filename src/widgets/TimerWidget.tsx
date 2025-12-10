import { useEffect, useState } from 'react'
import './widgets.css'

function format(seconds: number) {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')}`
}

export function TimerWidget() {
  const [seconds, setSeconds] = useState(0)
  const [running, setRunning] = useState(false)

  useEffect(() => {
    if (!running) return
    const id = window.setInterval(() => setSeconds((s) => s + 1), 1000)
    return () => window.clearInterval(id)
  }, [running])

  return (
    <div className="widget-shell">
      <div>
        <p className="widget-title">Pomodoro</p>
        <p className="widget-subtitle">Quick timer with pause/reset</p>
      </div>
      <div className="timer-display">{format(seconds)}</div>
      <div className="widget-row">
        <button className="widget-button" onClick={() => setRunning((v) => !v)}>
          {running ? 'Pause' : 'Start'}
        </button>
        <button
          className="widget-button"
          onClick={() => {
            setSeconds(0)
            setRunning(false)
          }}
        >
          Reset
        </button>
      </div>
    </div>
  )
}

export default TimerWidget
