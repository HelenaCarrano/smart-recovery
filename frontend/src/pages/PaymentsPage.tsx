import { RecoveryActionBadge } from '@/components/RecoveryActionBadge'
import { StatusBadge } from '@/components/StatusBadge'
import { Card } from '@/components/ui/Card'
import { type DataTableColumn, DataTable } from '@/components/ui/DataTable'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { ScoreIndicator } from '@/components/ui/ScoreIndicator'
import {
  DEFAULT_PAYMENTS_FILTERS,
  PaymentsFilters,
  type PaymentsFiltersState,
} from '@/components/PaymentsFilters'
import { usePayments } from '@/hooks/usePayments'
import { declineReasonLabel } from '@/lib/labels'
import { formatCurrency, formatDateTime } from '@/lib/format'
import type { PaymentListItem } from '@/types/paymentListItem'
import { useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'

function applyFilters(payments: PaymentListItem[], filters: PaymentsFiltersState): PaymentListItem[] {
  return payments.filter((payment) => {
    if (filters.status !== 'All' && payment.status !== filters.status) return false
    if (filters.declineReason !== 'All' && payment.declineReason !== filters.declineReason) return false

    const paymentDate = payment.createdAt.slice(0, 10)
    if (filters.dateFrom && paymentDate < filters.dateFrom) return false
    if (filters.dateTo && paymentDate > filters.dateTo) return false

    return true
  })
}

export function PaymentsPage() {
  const state = usePayments()
  const [filters, setFilters] = useState<PaymentsFiltersState>(DEFAULT_PAYMENTS_FILTERS)
  const navigate = useNavigate()

  const filtered = useMemo(() => {
    if (state.status !== 'success') return []
    return applyFilters(state.data, filters)
  }, [state, filters])

  const columns: DataTableColumn<PaymentListItem>[] = [
    { key: 'customer', header: 'Cliente', render: (row) => <span className="font-medium text-slate-900">{row.customerName}</span> },
    { key: 'amount', header: 'Valor', render: (row) => formatCurrency(row.amount) },
    { key: 'status', header: 'Status', render: (row) => <StatusBadge status={row.status} /> },
    { key: 'declineReason', header: 'Motivo da recusa', render: (row) => declineReasonLabel(row.declineReason) },
    { key: 'date', header: 'Data', render: (row) => formatDateTime(row.createdAt) },
    {
      key: 'score',
      header: 'Recovery Score',
      render: (row) => (row.recoveryScore !== null ? <ScoreIndicator score={row.recoveryScore} /> : '—'),
    },
    {
      key: 'action',
      header: 'Ação',
      render: (row) =>
        row.recommendedAction ? <RecoveryActionBadge action={row.recommendedAction} /> : '—',
    },
  ]

  return (
    <div className="space-y-6">
      <PaymentsFilters filters={filters} onChange={setFilters} />

      {state.status === 'loading' && <LoadingState label="Carregando pagamentos…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' && (
        <Card>
          {filtered.length === 0 ? (
            <EmptyState
              title="Nenhum pagamento encontrado"
              description={
                state.data.length === 0
                  ? 'Ainda não há cobranças registradas na plataforma.'
                  : 'Nenhum pagamento corresponde aos filtros selecionados.'
              }
            />
          ) : (
            <DataTable
              columns={columns}
              data={filtered}
              keyExtractor={(row) => row.id}
              onRowClick={(row) => navigate(`/payments/${row.id}`)}
            />
          )}
        </Card>
      )}
    </div>
  )
}
