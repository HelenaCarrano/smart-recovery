import { clsx } from 'clsx'
import type { ReactNode } from 'react'

export interface DataTableColumn<T> {
  key: string
  header: string
  render: (row: T) => ReactNode
  className?: string
}

interface DataTableProps<T> {
  columns: DataTableColumn<T>[]
  data: T[]
  keyExtractor: (row: T) => string
  onRowClick?: (row: T) => void
}

export function DataTable<T>({ columns, data, keyExtractor, onRowClick }: DataTableProps<T>) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full min-w-[720px] text-left text-sm">
        <thead>
          <tr className="border-b border-slate-200 text-xs font-medium tracking-wide text-slate-400 uppercase">
            {columns.map((column) => (
              <th key={column.key} className={clsx('py-2.5 pr-4', column.className)}>
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {data.map((row) => (
            <tr
              key={keyExtractor(row)}
              onClick={onRowClick ? () => onRowClick(row) : undefined}
              className={clsx('text-slate-700', onRowClick && 'cursor-pointer hover:bg-slate-50')}
            >
              {columns.map((column) => (
                <td key={column.key} className={clsx('py-3 pr-4', column.className)}>
                  {column.render(row)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
