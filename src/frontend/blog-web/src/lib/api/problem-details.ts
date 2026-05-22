export type ProblemDetails = {
  status?: number
  title?: string
  detail?: string
  instance?: string
  type?: string
  errors?: Record<string, string[]>
}

export function extractProblemMessage(problem: ProblemDetails | null | undefined) {
  if (!problem) {
    return 'The request could not be processed.'
  }

  if (problem.errors) {
    const [first] = Object.values(problem.errors).flat()
    if (first) {
      return first
    }
  }

  return problem.detail || problem.title || 'The request could not be processed.'
}
