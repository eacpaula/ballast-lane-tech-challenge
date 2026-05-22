export function parseTags(raw: string): string[] {
  const seen = new Set<string>()
  const result: string[] = []
  for (const part of raw.split(',')) {
    const trimmed = part.trim()
    if (trimmed.length === 0) continue
    const lower = trimmed.toLowerCase()
    if (seen.has(lower)) continue
    seen.add(lower)
    result.push(trimmed)
  }
  return result
}
