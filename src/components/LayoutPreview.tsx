import './LayoutPreview.css'

export type LayoutCard = {
  id: string
  title: string
  kind: 'timer' | 'notes' | 'quotes' | 'grid' | 'stats' | 'ambient' | 'game'
  size?: 's' | 'm' | 'l'
  accent?: string
}

export type LayoutColumn = {
  id: string
  title: string
  width: number
  cards: LayoutCard[]
}

export type LayoutPreset = {
  id: string
  name: string
  description: string
  columns: LayoutColumn[]
}

type LayoutPreviewProps = {
  preset: LayoutPreset
}

const cardIcons: Record<LayoutCard['kind'], string> = {
  timer: '⏱',
  notes: '✏️',
  quotes: '💬',
  grid: '🧩',
  stats: '📊',
  ambient: '🌫️',
  game: '🎮',
}

export function LayoutPreview({ preset }: LayoutPreviewProps) {
  return (
    <div className="layout-preview">
      <div className="layout-meta">
        <p className="eyebrow">Layout preset</p>
        <h3>{preset.name}</h3>
        <p className="hint">{preset.description}</p>
      </div>
      <div className="layout-grid">
        {preset.columns.map((column) => (
          <div
            key={column.id}
            className="layout-column"
            style={{ flex: column.width }}
          >
            <div className="column-head">
              <span>{column.title}</span>
              <span className="column-width">{column.width}u</span>
            </div>
            <div className="column-body">
              {column.cards.map((card) => (
                <div
                  key={card.id}
                  className={`layout-card size-${card.size || 'm'}`}
                  style={{ borderColor: card.accent }}
                >
                  <span className="card-icon">{cardIcons[card.kind]}</span>
                  <div>
                    <p className="card-title">{card.title}</p>
                    <p className="card-kind">{card.kind}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default LayoutPreview
