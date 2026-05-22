import { useState } from 'react'
import Button from '../../components/Button'

type Props = {
  initialTitle?: string
  initialDescription?: string
  submitLabel: string
  isSubmitting?: boolean
  secondaryActionLabel?: string
  onSecondaryAction?: () => void
  onSubmit: (title: string, description: string) => void | Promise<void>
}

export default function CategoryForm({
  initialTitle = '',
  initialDescription = '',
  submitLabel,
  isSubmitting = false,
  secondaryActionLabel,
  onSecondaryAction,
  onSubmit,
}: Props) {
  const [title, setTitle] = useState(initialTitle)
  const [description, setDescription] = useState(initialDescription)

  return (
    <form
      className="category-admin-form"
      onSubmit={(event) => {
        event.preventDefault()
        void onSubmit(title, description)
      }}
    >
      <label className="form-field" htmlFor="categoryTitle">
        <span className="field-label">Name</span>
        <input
          id="categoryTitle"
          name="categoryTitle"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          placeholder="e.g. Distributed Systems"
          className="input-control category-admin-input"
          required
        />
        <span className="field-help">The name is how it appears on your site.</span>
      </label>

      <label className="form-field" htmlFor="categoryDescription">
        <span className="field-label">Description (Optional)</span>
        <textarea
          id="categoryDescription"
          name="categoryDescription"
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          placeholder="Add context for how this category should be used across posts."
          className="input-control input-textarea category-admin-input category-admin-textarea"
          rows={5}
        />
        <span className="field-help">Description adds optional context for administrators and authors.</span>
      </label>

      <div className="category-admin-actions">
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Saving…' : submitLabel}
        </Button>
        {secondaryActionLabel && onSecondaryAction ? (
          <Button type="button" variant="secondary" onClick={onSecondaryAction}>
            {secondaryActionLabel}
          </Button>
        ) : null}
      </div>
    </form>
  )
}
