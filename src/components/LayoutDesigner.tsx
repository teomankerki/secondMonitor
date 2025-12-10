import { useEffect, useMemo, useState, type ReactNode } from 'react'
import LayoutPreview, { type LayoutPreset, type LayoutColumn } from './LayoutPreview'
import './LayoutDesigner.css'

const STORAGE_KEY = 'secondMonitor.layout.v1'

const defaultPresets: LayoutPreset[] = [
  {
    id: 'focus-stack',
    name: 'Focus stack',
    description: 'Utility rail plus a wide work surface.',
    columns: [
      {
        id: 'rail',
        title: 'Utility rail',
        width: 1,
        cards: [
          { id: 'timer', title: 'Pomodoro', kind: 'timer', size: 'm', accent: '#6fdcc2' },
          { id: 'notes', title: 'Quick notes', kind: 'notes', size: 'm', accent: '#e1c16e' },
          { id: 'quotes', title: 'Quotes', kind: 'quotes', size: 's', accent: '#90b4ff' },
        ],
      },
      {
        id: 'main',
        title: 'Work surface',
        width: 2,
        cards: [
          { id: 'grid', title: 'Card grid', kind: 'grid', size: 'l', accent: '#c49bff' },
          { id: 'stats', title: 'Status glance', kind: 'stats', size: 'm', accent: '#6fdcc2' },
          { id: 'ambient', title: 'Ambient haze', kind: 'ambient', size: 'm', accent: '#9cd7ff' },
          { id: 'game', title: 'Break: mini runner', kind: 'game', size: 's', accent: '#f17a7a' },
        ],
      },
    ],
  },
  {
    id: 'ambient-dual',
    name: 'Ambient dual',
    description: 'Even split for two slow-changing panels with a slim widget rail.',
    columns: [
      {
        id: 'left',
        title: 'Left ambient',
        width: 1,
        cards: [
          { id: 'ambient', title: 'Ambient haze', kind: 'ambient', size: 'l', accent: '#9cd7ff' },
          { id: 'quotes', title: 'Quotes', kind: 'quotes', size: 'm', accent: '#90b4ff' },
        ],
      },
      {
        id: 'right',
        title: 'Right ambient',
        width: 1,
        cards: [
          { id: 'stats', title: 'Status glance', kind: 'stats', size: 'm', accent: '#6fdcc2' },
          { id: 'grid', title: 'Card grid', kind: 'grid', size: 'l', accent: '#c49bff' },
        ],
      },
      {
        id: 'rail',
        title: 'Widget rail',
        width: 1,
        cards: [
          { id: 'timer', title: 'Pomodoro', kind: 'timer', size: 'm', accent: '#6fdcc2' },
          { id: 'notes', title: 'Quick notes', kind: 'notes', size: 's', accent: '#e1c16e' },
          { id: 'game', title: 'Break mini', kind: 'game', size: 's', accent: '#f17a7a' },
        ],
      },
    ],
  },
]

const clonePreset = (preset: LayoutPreset): LayoutPreset => ({
  ...preset,
  columns: preset.columns.map((col) => ({
    ...col,
    cards: col.cards.map((card) => ({ ...card })),
  })),
})

function clampWidth(width: number) {
  return Math.min(Math.max(width, 1), 3)
}

type DesignerColumnProps = {
  column: LayoutColumn
  index: number
  showWidthControls: boolean
  renderCardContent?: (card: LayoutColumn['cards'][number]) => ReactNode
  showDropZone?: boolean
  onResize: (id: string, delta: number) => void
  onDropCard: (sourceColumn: string, cardId: string, targetColumn: string, targetIndex: number) => void
  onDropColumn: (sourceColumn: string, targetIndex: number) => void
  draggingCard: { columnId: string; cardId: string } | null
  draggingColumn: string | null
  onCardDragStart: (columnId: string, cardId: string) => void
  onCardDragEnd: () => void
  onColumnDragStart: (columnId: string) => void
  onColumnDragEnd: () => void
}

function DesignerColumn({
  column,
  index,
  showWidthControls,
  renderCardContent,
  showDropZone = true,
  onResize,
  onDropCard,
  onDropColumn,
  draggingCard,
  draggingColumn,
  onCardDragStart,
  onCardDragEnd,
  onColumnDragStart,
  onColumnDragEnd,
}: DesignerColumnProps) {
  return (
    <div
      className={`designer-column${draggingColumn === column.id ? ' is-dragging' : ''}`}
      draggable
      onDragStart={(event) => {
        onColumnDragStart(column.id)
        event.dataTransfer.setData('text/column', column.id)
        event.dataTransfer.effectAllowed = 'move'
      }}
      onDragOver={(event) => {
        const types = Array.from(event.dataTransfer.types)
        const isCard = types.includes('text/card')
        const isColumn = types.includes('text/column')
        if (isColumn && !isCard) {
          event.preventDefault()
        }
      }}
      onDrop={(event) => {
        const types = Array.from(event.dataTransfer.types)
        if (types.includes('text/card')) return
        const source = event.dataTransfer.getData('text/column')
        if (source && source !== column.id) {
          onDropColumn(source, index)
        }
      }}
      onDragEnd={onColumnDragEnd}
    >
      <div className="designer-column-head" aria-label={`Column ${column.title}`}>
        <div>
          <p className="eyebrow">Column {index + 1}</p>
          <h4>{column.title}</h4>
        </div>
        {showWidthControls && (
          <div className="col-actions">
            <button
              type="button"
              onClick={() => onResize(column.id, -1)}
              aria-label="Shrink column"
            >
              -
            </button>
            <span className="width-chip">{column.width}u</span>
            <button
              type="button"
              onClick={() => onResize(column.id, 1)}
              aria-label="Grow column"
            >
              +
            </button>
          </div>
        )}
      </div>
      <div className="designer-cards">
        {column.cards.map((card, cardIndex) => (
          <div key={card.id} className="designer-card" style={{ borderColor: card.accent }}>
            <div
              className={`drag-card${draggingCard?.cardId === card.id ? ' is-dragging' : ''}`}
              draggable
              onDragStart={(event) => {
                onCardDragStart(column.id, card.id)
                event.dataTransfer.setData('text/card', card.id)
                event.dataTransfer.setData('text/column', column.id)
                event.dataTransfer.effectAllowed = 'move'
              }}
              onDragOver={(event) => {
                if (event.dataTransfer.types.includes('text/card')) {
                  event.preventDefault()
                }
              }}
              onDrop={(event) => {
                const sourceCard = event.dataTransfer.getData('text/card')
                const sourceColumn = event.dataTransfer.getData('text/column')
                if (sourceCard && sourceColumn) {
                  onDropCard(sourceColumn, sourceCard, column.id, cardIndex)
                }
              }}
              onDragEnd={onCardDragEnd}
            >
              <div>
                {renderCardContent ? (
                  renderCardContent(card)
                ) : (
                  <>
                    <p className="card-title">{card.title}</p>
                    <p className="card-kind">{card.kind}</p>
                  </>
                )}
              </div>
            </div>
          </div>
        ))}
        {showDropZone && (
          <div
            className="drop-zone"
            onDragOver={(event) => {
              if (event.dataTransfer.types.includes('text/card')) {
                event.preventDefault()
              }
            }}
            onDrop={(event) => {
              const sourceCard = event.dataTransfer.getData('text/card')
              const sourceColumn = event.dataTransfer.getData('text/column')
              if (sourceCard && sourceColumn) {
                onDropCard(sourceColumn, sourceCard, column.id, column.cards.length)
              }
            }}
          >
            Drop here to append
          </div>
        )}
      </div>
    </div>
  )
}

type LayoutDesignerProps = {
  showControls?: boolean
  showPresetBar?: boolean
  showPreview?: boolean
  showWidthControls?: boolean
  showDropZone?: boolean
  renderCardContent?: (card: LayoutColumn['cards'][number]) => ReactNode
  title?: string
  description?: string
}

export function LayoutDesigner({
  showControls = true,
  showPresetBar = true,
  showPreview = true,
  showWidthControls = true,
  showDropZone = true,
  renderCardContent,
  title = 'Layout sandbox',
  description = 'Drag and drop to change positions. Column widths use unit math—ideal for ultrawide snapping and saving presets. Width buttons stay for quick resizing.',
}: LayoutDesignerProps) {
  const [preset, setPreset] = useState<LayoutPreset>(() => clonePreset(defaultPresets[0]))
  const [activePresetId, setActivePresetId] = useState<string>(defaultPresets[0].id)
  const [draggingCard, setDraggingCard] = useState<{ columnId: string; cardId: string } | null>(
    null,
  )
  const [draggingColumn, setDraggingColumn] = useState<string | null>(null)
  const [loadedFromStorage, setLoadedFromStorage] = useState(false)

  const columnTotals = useMemo(
    () => preset.columns.reduce((sum, col) => sum + col.width, 0),
    [preset.columns],
  )

  useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY)
      if (raw) {
        const parsed = JSON.parse(raw) as { presetId?: string; preset: LayoutPreset }
        if (parsed?.preset) {
          setPreset(parsed.preset)
          setActivePresetId(parsed.presetId || 'saved')
        }
      }
    } catch {
      // ignore parse failures and fall back to defaults
    } finally {
      setLoadedFromStorage(true)
    }
  }, [])

  useEffect(() => {
    if (!loadedFromStorage) return
    localStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({
        presetId: activePresetId,
        preset,
      }),
    )
  }, [preset, activePresetId, loadedFromStorage])

  const resizeColumn = (columnId: string, delta: number) => {
    setPreset((prev) => ({
      ...prev,
      columns: prev.columns.map((col) =>
        col.id === columnId ? { ...col, width: clampWidth(col.width + delta) } : col,
      ),
    }))
  }

  const moveCardToColumn = (
    fromColumnId: string,
    cardId: string,
    toColumnId: string,
    targetIndex: number,
  ) => {
    setPreset((prev) => {
      const nextColumns = prev.columns.map((col) => ({ ...col, cards: [...col.cards] }))
      const fromIdx = nextColumns.findIndex((col) => col.id === fromColumnId)
      const toIdx = nextColumns.findIndex((col) => col.id === toColumnId)
      if (fromIdx < 0 || toIdx < 0) return prev
      const cardIndex = nextColumns[fromIdx].cards.findIndex((c) => c.id === cardId)
      if (cardIndex < 0) return prev
      const [card] = nextColumns[fromIdx].cards.splice(cardIndex, 1)
      const insertAt = Math.min(Math.max(targetIndex, 0), nextColumns[toIdx].cards.length)
      nextColumns[toIdx].cards.splice(insertAt, 0, card)
      return { ...prev, columns: nextColumns }
    })
  }

  const resetLayout = () => {
    const defaultPreset = defaultPresets.find((p) => p.id === activePresetId) || defaultPresets[0]
    setPreset(clonePreset(defaultPreset))
  }

  const applyPreset = (presetId: string) => {
    const presetToApply =
      defaultPresets.find((p) => p.id === presetId) ||
      defaultPresets.find((p) => p.id === 'focus-stack')
    if (!presetToApply) return
    setActivePresetId(presetId)
    setPreset(clonePreset(presetToApply))
  }

  return (
    <div className="layout-designer">
      {showControls && (
        <div className="designer-top">
          <div>
            <p className="eyebrow">Layout sandbox</p>
            <h3>{title}</h3>
            <p className="hint">{description}</p>
          </div>
          <div className="designer-meta">
            <div>
              <span className="meta-label">Columns</span>
              <strong>{preset.columns.length}</strong>
            </div>
            <div>
              <span className="meta-label">Width sum</span>
              <strong>{columnTotals}u</strong>
            </div>
            <button type="button" onClick={resetLayout} className="ghost-button">
              Restore preset
            </button>
          </div>
        </div>
      )}

      {showPresetBar && (
        <div className="preset-bar">
          <label className="preset-label" htmlFor="preset-select">
            Preset
          </label>
          <select
            id="preset-select"
            value={activePresetId}
            onChange={(event) => applyPreset(event.target.value)}
          >
            {defaultPresets.map((presetOption) => (
              <option key={presetOption.id} value={presetOption.id}>
                {presetOption.name}
              </option>
            ))}
            <option value="saved">Recovered session</option>
          </select>
          <span className="autosave-hint">Autosaves to your browser</span>
        </div>
      )}

      <div className="designer-grid">
        {preset.columns.map((col, idx) => (
          <DesignerColumn
            key={col.id}
            column={col}
            index={idx}
            showWidthControls={showWidthControls}
            renderCardContent={renderCardContent}
            showDropZone={showDropZone}
            onResize={resizeColumn}
            onDropCard={(sourceCol, cardId, targetCol, targetIndex) => {
              setDraggingCard(null)
              moveCardToColumn(sourceCol, cardId, targetCol, targetIndex)
            }}
            onDropColumn={(sourceCol, targetIndex) => {
              setDraggingColumn(null)
              setPreset((prev) => {
                const currentIndex = prev.columns.findIndex((c) => c.id === sourceCol)
                if (currentIndex < 0 || targetIndex < 0 || targetIndex >= prev.columns.length) {
                  return prev
                }
                const nextColumns = [...prev.columns]
                const [col] = nextColumns.splice(currentIndex, 1)
                nextColumns.splice(targetIndex, 0, col)
                return { ...prev, columns: nextColumns }
              })
            }}
            draggingCard={draggingCard}
            draggingColumn={draggingColumn}
            onCardDragStart={(colId, cardId) => setDraggingCard({ columnId: colId, cardId })}
            onCardDragEnd={() => setDraggingCard(null)}
            onColumnDragStart={(colId) => setDraggingColumn(colId)}
            onColumnDragEnd={() => setDraggingColumn(null)}
          />
        ))}
      </div>

      {showPreview && (
        <div className="designer-preview">
          <LayoutPreview preset={preset} />
        </div>
      )}
    </div>
  )
}

export default LayoutDesigner
