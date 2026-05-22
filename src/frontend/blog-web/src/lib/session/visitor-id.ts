const visitorKey = 'blog-platform.visitor-id'

function createVisitorId() {
  if (typeof crypto !== 'undefined' && 'randomUUID' in crypto) {
    return crypto.randomUUID()
  }

  return `visitor-${Date.now()}-${Math.floor(Math.random() * 100000)}`
}

export function getVisitorId() {
  const existing = localStorage.getItem(visitorKey)
  if (existing) {
    return existing
  }

  const created = createVisitorId()
  localStorage.setItem(visitorKey, created)
  return created
}
