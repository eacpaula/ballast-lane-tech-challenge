import { useMemo } from 'react'
import Button from '../../components/Button'
import FieldMessage from '../../components/FieldMessage'
import Select from '../../components/Select'
import Textarea from '../../components/Textarea'
import TextInput from '../../components/TextInput'
import type { AvailableCategory, PostEditorDraft } from './post.types'

type Props = {
  value: PostEditorDraft
  categories: AvailableCategory[]
  isSubmitting: boolean
  submitLabel: string
  error?: string | null
  mode: 'create' | 'edit'
  disableCategorySelection?: boolean
  onChange: (value: PostEditorDraft) => void
  onSubmit: () => void | Promise<void>
}

export default function PostForm({
  value,
  categories,
  isSubmitting,
  submitLabel,
  error,
  mode,
  disableCategorySelection = false,
  onChange,
  onSubmit,
}: Props) {
  const categoryOptions = useMemo(() => categories, [categories])
  const metadataMessage =
    mode === 'edit'
      ? 'Visibility, availability, tags, and schedule dates are shown for planning, but the current backend edit flow still persists only title, summary, and content.'
      : 'Tags, visibility, availability, and schedule dates are shown in the approved editor design. The current backend create flow still persists category, title, summary, and content only.'

  return (
    <form
      className="post-editor-form"
      onSubmit={(event) => {
        event.preventDefault()
        void onSubmit()
      }}
    >
      <div className="post-editor-card">
        <div className="post-editor-grid">
          <TextInput
            label="Title"
            name="title"
            value={value.title}
            onChange={(event) => onChange({ ...value, title: event.target.value })}
            placeholder="e.g. Architecting Scalable Microservices with Rust"
            className="post-editor-input"
            required
          />

          <div className="post-editor-row">
            <Select
              label="Category"
              name="categoryId"
              value={String(value.categoryId || '')}
              onChange={(event) =>
                onChange({
                  ...value,
                  categoryId: Number(event.target.value),
                })
              }
              disabled={disableCategorySelection}
              className="post-editor-input"
              required
            >
              <option value="">Select a category</option>
              {categoryOptions.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.title}
                </option>
              ))}
            </Select>

            <TextInput
              label="Tags"
              name="tags"
              value={value.tags}
              onChange={(event) => onChange({ ...value, tags: event.target.value })}
              placeholder="rust, cloud-native, aws"
              className="post-editor-input"
            />
          </div>

          <Textarea
            label="Short Description"
            name="summary"
            value={value.summary}
            onChange={(event) => onChange({ ...value, summary: event.target.value })}
            placeholder="Summarize your post in 2-3 sentences..."
            className="post-editor-input"
            rows={3}
          />

          <div className="post-editor-content-head">
            <span className="field-label">Content</span>
            <span className="post-editor-support-text">Markdown Supported</span>
          </div>
          <label className="form-field" htmlFor="content">
            <textarea
              id="content"
              aria-label="Content"
              name="content"
              value={value.content}
              onChange={(event) => onChange({ ...value, content: event.target.value })}
              placeholder="Write your technical content here..."
              className="input-control input-textarea post-editor-input post-editor-textarea"
              rows={12}
              required
            />
          </label>

          <div className="post-editor-row">
            <label className="form-field" htmlFor="publishDate">
              <span className="field-label">Publish Date</span>
              <input
                id="publishDate"
                name="publishDate"
                type="date"
                value={value.publishDate}
                onChange={(event) => onChange({ ...value, publishDate: event.target.value })}
                className="input-control post-editor-input"
              />
            </label>

            <label className="form-field" htmlFor="expireDate">
              <span className="field-label">Expire Date</span>
              <input
                id="expireDate"
                name="expireDate"
                type="date"
                value={value.expireDate}
                onChange={(event) => onChange({ ...value, expireDate: event.target.value })}
                className="input-control post-editor-input"
              />
            </label>
          </div>

          <div className="post-editor-meta-bar">
            <div className="post-editor-toggles">
              <label className="post-editor-toggle">
                <input
                  type="checkbox"
                  checked={value.isPublic}
                  onChange={(event) => onChange({ ...value, isPublic: event.target.checked })}
                />
                <span>Publicly Visible</span>
              </label>
              <label className="post-editor-toggle">
                <input
                  type="checkbox"
                  checked={value.isAvailable}
                  onChange={(event) => onChange({ ...value, isAvailable: event.target.checked })}
                />
                <span>Available</span>
              </label>
            </div>

            <div className="post-editor-status-note">
              <svg viewBox="0 0 20 20" aria-hidden="true" className="post-editor-status-icon">
                <path
                  d="M10 3.3a6.7 6.7 0 1 0 0 13.4 6.7 6.7 0 0 0 0-13.4Zm0 2a.9.9 0 1 1 0 1.8.9.9 0 0 1 0-1.8Zm1 8H9V8.2h2v5.1Z"
                  fill="currentColor"
                />
              </svg>
              <span>Metadata fields are ready in the UI and awaiting backend support.</span>
            </div>
          </div>

          <FieldMessage message={disableCategorySelection ? 'Category is fixed during edit because the current API only updates title, summary, and content.' : null} />
          <FieldMessage message={metadataMessage} />
          <FieldMessage message={error} tone={error ? 'error' : 'neutral'} />

          <div className="post-editor-actions">
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Saving…' : submitLabel}
            </Button>
          </div>
        </div>
      </div>

      <div className="post-editor-tip-card">
        <p className="post-editor-tip-title">Writing Tips</p>
        <ul className="post-editor-tip-list">
          <li>Start with a clear technical problem statement.</li>
          <li>Use code blocks and concrete examples when they improve clarity.</li>
          <li>Keep tags focused so other engineers can find the post faster.</li>
        </ul>
      </div>
    </form>
  )
}
