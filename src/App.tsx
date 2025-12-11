import LayoutDesigner from './components/LayoutDesigner'
import TimerWidget from './widgets/TimerWidget'
import NotesWidget from './widgets/NotesWidget'
import QuotesWidget from './widgets/QuotesWidget'
import AmbientWidget from './widgets/AmbientWidget'
import GameWidget from './widgets/GameWidget'
import './App.css'

function App() {
  const renderWidget = (card: { kind: string }) => {
    switch (card.kind) {
      case 'timer':
        return <TimerWidget />
      case 'notes':
        return <NotesWidget />
      case 'quotes':
        return <QuotesWidget />
      case 'ambient':
        return <AmbientWidget />
      case 'game':
        return <GameWidget />
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
      <div className="page-header">
        <img src="/logo.svg" alt="Second Screen logo" className="logo-mark" />
        <div>
          <h1>Second Screen Mega App</h1>
          <p>A web app with all your needs for your second screen. Developed by and for iPad kids.</p>
        </div>
      </div>
      <div className="panel">
        <LayoutDesigner
          showControls={false}
          showPresetBar={false}
          showPreview={false}
          showWidthControls={false}
          showDropZone
          renderCardContent={renderWidget}
        />
      </div>
      <div className="footer">
        Second screen develop by Teoman Kerki, 2025. for any questionts contact teoman@teomankerki.com
      </div>
    </div>
  )
}

export default App
