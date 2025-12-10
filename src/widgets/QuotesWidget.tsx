import { useMemo, useState } from 'react'
import './widgets.css'

const quotes = [
  { text: 'One good thing about music, when it hits you, you feel no pain.', author: 'Bob Marley' },
  {
    text: 'Until the end of the world, all whys will be answered, but now, you can only ask!',
    author: 'Bob Marley',
  },
  {
    text: 'A hero is someone who understands the degree of responsibility that comes with his freedom.',
    author: 'Bob Dylan',
  },
  {
    text: 'A man is a success if he gets up in the morning and gets to bed at night and in between he does what he wants to do.',
    author: 'Bob Dylan',
  },
  { text: "Let's add some happy little trees.", author: 'Bob Ross' },
  { text: 'They say everything in your world seems to be happy.', author: 'Bob Ross' },
  { text: 'Can we fix it? Yes we can!', author: 'Bob the Builder' },
  { text: 'No job too big, no job too small.', author: 'Bob the Builder' },
  {
    text: 'I’m not a criminal, I’m an attorney. I do respect the law. But I bend it sometimes to get things done.',
    author: 'Bob Odenkirk (as Saul Goodman)',
  },
  {
    text: 'You wanna do business? I’m your guy. Because if you\'re gonna go down — I want to go down with you.',
    author: 'Bob Odenkirk (as Saul Goodman)',
  },
]

function pickNext(current: number, total: number) {
  const next = current + 1
  return next >= total ? 0 : next
}

export function QuotesWidget() {
  const [index, setIndex] = useState(0)
  const total = quotes.length
  const quote = useMemo(() => quotes[index], [index])

  return (
    <div className="widget-shell">
      <div>
        <p className="widget-title">Bob's Quotes</p>
        <p className="widget-subtitle">Ambient motivation</p>
      </div>
      <p className="quote-text">“{quote.text}”</p>
      <p className="quote-author">— {quote.author}</p>
      <div className="widget-row">
        <button className="widget-button" onClick={() => setIndex((i) => pickNext(i, total))}>
          Next quote
        </button>
      </div>
    </div>
  )
}

export default QuotesWidget
