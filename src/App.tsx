import { Link, Route, Routes, useLocation, useNavigate } from 'react-router-dom'
import LayoutDesigner from './components/LayoutDesigner'
import TimerWidget from './widgets/TimerWidget'
import NotesWidget from './widgets/NotesWidget'
import QuotesWidget from './widgets/QuotesWidget'
import './App.css'

function TopBar() {
  const location = useLocation()
  const navigate = useNavigate()
  const isSettings = location.pathname === '/settings'

  return (
    <div className="topbar">
      <button
        type="button"
        className="topbar-button"
        onClick={() => navigate('/settings')}
        aria-current={isSettings}
      >
        Settings
      </button>
      {isSettings && (
        <Link to="/" className="topbar-link">
          ← Back to widgets
        </Link>
      )}
    </div>
  )
}

function Home() {
  const renderWidget = (card: { kind: string }) => {
    switch (card.kind) {
      case 'timer':
        return <TimerWidget />
      case 'notes':
        return <NotesWidget />
      case 'quotes':
        return <QuotesWidget />
      default:
        return (
          <>
            <p className="card-title">{card.kind}</p>
            <p className="card-kind">Widget coming soon</p>
          </>
        )
    }
  }

  return (
    <div className="page">
      <div className="panel">
        <LayoutDesigner
          showControls={false}
          showPresetBar={false}
          showPreview={false}
          showWidthControls={false}
          showDropZone={false}
          renderCardContent={renderWidget}
        />
      </div>
    </div>
  )
}

function Settings() {
  return (
    <div className="page">
      <div className="panel">
        <div className="panel-head">
          <div>
            <p className="eyebrow">Settings</p>
            <h2>Change view</h2>
            <p className="hint">
              Pick a layout preset and reset widths. Drag-and-drop works here just like the main view.
            </p>
          </div>
        </div>
        <LayoutDesigner
          showControls
          showPresetBar
          showPreview
          title="Adjust columns and cards"
          description="Select a preset, tweak widths, and rearrange widgets. Changes persist locally."
        />
      </div>
    </div>
  )
}

function App() {
  return (
    <>
      <TopBar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/settings" element={<Settings />} />
      </Routes>
    </>
  )
}

export default App
