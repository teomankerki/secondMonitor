import { useEffect, useState } from 'react'
import './widgets.css'

const STORAGE_KEY = 'secondMonitor.notes'

export function NotesWidget() {
  const [text, setText] = useState('')

  useEffect(() => {
    try {
      const saved = localStorage.getItem(STORAGE_KEY)
      if (saved) setText(saved)
    } catch {
      // ignore read errors
    }
  }, [])

  useEffect(() => {
    try {
      localStorage.setItem(STORAGE_KEY, text)
    } catch {
      // ignore write errors
    }
  }, [text])

  return (
    <div className="widget-shell">
      <div>
        <p className="widget-title">Quick notes</p>
        <p className="widget-subtitle">Lightweight scratchpad</p>
      </div>
      <textarea
        className="notes-input"
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder="Jot something down..."
      />
    </div>
  )
}

export default NotesWidget
