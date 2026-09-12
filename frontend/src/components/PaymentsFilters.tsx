import { Card } from '@/components/ui/Card'
import { declineReasonLabel } from '@/lib/labels'
import { DEFAULT_PAYMENTS_FILTERS, type PaymentsFiltersState } from '@/types/paymentsFilters'
import type { DeclineReason, PaymentStatus } from '@/types/payment'

const STATUS_OPTIONS: { value: PaymentStatus | 'All'; label: string }[] = [
  { value: 'All', label: 'Todos os status' },
  { value: 'Pending', label: 'Pendente' },
  { value: 'Approved', label: 'Aprovado' },
  { value: 'Declined', label: 'Recusado' },
  { value: 'Cancelled', label: 'Cancelado' },
  { value: 'Refunded', label: 'Estornado' },
]

const DECLINE_REASON_OPTIONS: DeclineReason[] = [
  'InsufficientFunds',
  'ExpiredCard',
  'InvalidCard',
  'TemporaryError',
  'BlockedCard',
  'Unknown',
]

function isDefaultFilters(filters: PaymentsFiltersState): boolean {
  return (
    filters.status === DEFAULT_PAYMENTS_FILTERS.status &&
    filters.declineReason === DEFAULT_PAYMENTS_FILTERS.declineReason &&
    filters.dateFrom === DEFAULT_PAYMENTS_FILTERS.dateFrom &&
    filters.dateTo === DEFAULT_PAYMENTS_FILTERS.dateTo
  )
}

interface PaymentsFiltersProps {
  filters: PaymentsFiltersState
  onChange: (filters: PaymentsFiltersState) => void
}

const selectClasses =
  'rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none focus:ring-1 focus:ring-brand-400'

export function PaymentsFilters({ filters, onChange }: PaymentsFiltersProps) {
  return (
    <Card className="flex flex-wrap items-end gap-4">
      <label className="flex flex-col gap-1 text-xs font-medium text-slate-500">
        Status
        <select
          className={selectClasses}
          value={filters.status}
          onChange={(e) => onChange({ ...filters, status: e.target.value as PaymentsFiltersState['status'] })}
        >
          {STATUS_OPTIONS.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      <label className="flex flex-col gap-1 text-xs font-medium text-slate-500">
        Motivo da recusa
        <select
          className={selectClasses}
          value={filters.declineReason}
          onChange={(e) =>
            onChange({ ...filters, declineReason: e.target.value as PaymentsFiltersState['declineReason'] })
          }
        >
          <option value="All">Todos os motivos</option>
          {DECLINE_REASON_OPTIONS.map((reason) => (
            <option key={reason} value={reason}>
              {declineReasonLabel(reason)}
            </option>
          ))}
        </select>
      </label>

      <label className="flex flex-col gap-1 text-xs font-medium text-slate-500">
        De
        <input
          type="date"
          className={selectClasses}
          value={filters.dateFrom}
          onChange={(e) => onChange({ ...filters, dateFrom: e.target.value })}
        />
      </label>

      <label className="flex flex-col gap-1 text-xs font-medium text-slate-500">
        Até
        <input
          type="date"
          className={selectClasses}
          value={filters.dateTo}
          onChange={(e) => onChange({ ...filters, dateTo: e.target.value })}
        />
      </label>

      {!isDefaultFilters(filters) && (
        <button
          type="button"
          onClick={() => onChange(DEFAULT_PAYMENTS_FILTERS)}
          className="text-sm font-medium text-brand-600 hover:text-brand-700"
        >
          Limpar filtros
        </button>
      )}
    </Card>
  )
}
