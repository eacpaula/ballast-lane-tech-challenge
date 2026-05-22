import Button from '../../components/Button'
import ConfirmAction from '../../components/ConfirmAction'
import type { ManagedCategory } from './category.types'

type Props = {
  category: ManagedCategory
  isBusy?: boolean
  onEdit: (category: ManagedCategory) => void
  onDeactivate: (categoryId: number) => void | Promise<void>
}

export default function CategoryListItem({
  category,
  isBusy = false,
  onEdit,
  onDeactivate,
}: Props) {
  return (
    <tr className="category-admin-row">
      <td className="category-admin-cell">
        <div className="category-admin-name">
          <span className="category-admin-name-marker" aria-hidden="true" />
          <div>
            <div>{category.title}</div>
            {category.description ? (
              <p className="mt-2 text-sm leading-6 text-ink-soft">{category.description}</p>
            ) : null}
          </div>
        </div>
      </td>
      <td className="category-admin-cell">
        <span className="category-admin-id">#{category.id}</span>
      </td>
      <td className="category-admin-cell">
        <span className={category.isAvailable ? 'category-admin-status' : 'category-admin-status category-admin-status-off'}>
          {category.isAvailable ? 'Available' : 'Unavailable'}
        </span>
      </td>
      <td className="category-admin-cell">
        <div className="category-admin-row-actions">
          <Button type="button" variant="secondary" disabled={isBusy} onClick={() => onEdit(category)}>
            Edit
          </Button>
          {category.isAvailable ? (
            <ConfirmAction
              label={isBusy ? 'Deactivating…' : 'Deactivate'}
              confirmLabel="Confirm deactivate"
              onConfirm={() => onDeactivate(category.id)}
            />
          ) : null}
        </div>
      </td>
    </tr>
  )
}
