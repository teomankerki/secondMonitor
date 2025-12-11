import './widgets.css'

export function AmbientWidget() {
  return (
    <div className="widget-shell">
      <div>
        <p className="widget-title">Ambient haze</p>
        <p className="widget-subtitle">Soft gradients for a calm glance</p>
      </div>
      <div className="ambient-block ambient-crisp">
        <div className="ambient-gradient crisp" />
      </div>
    </div>
  )
}

export default AmbientWidget
