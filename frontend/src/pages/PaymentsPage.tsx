import { RecoveryActionBadge } from '@/components/RecoveryActionBadge'
import { StatusBadge } from '@/components/StatusBadge'
import { Card } from '@/components/ui/Card'
import { type DataTableColumn, DataTable } from '@/components/ui/DataTable'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { Pagination } from '@/components/ui/Pagination'
import { ScoreIndicator } from '@/components/ui/ScoreIndicator'
import { PaymentsFilters } from '@/components/PaymentsFilters'
import { usePayments } from '@/hooks/usePayments'
import { declineReasonLabel } from '@/lib/labels'
import { formatCurrency, formatDateTime } from '@/lib/format'
import type { PaymentListItem } from '@/types/paymentListItem'
import { DEFAULT_PAYMENTS_FILTERS, type PaymentsFiltersState } from '@/types/paymentsFilters'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

const PAGE_SIZE = 20

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
    render: (row) => (row.recommendedAction ? <RecoveryActionBadge action={row.recommendedAction} /> : '—'),
  },
]

export function PaymentsPage() {
  const [page, setPage] = useState(1)
  const [filters, setFilters] = useState<PaymentsFiltersState>(DEFAULT_PAYMENTS_FILTERS)
  const state = usePayments(page, PAGE_SIZE, filters)
  const navigate = useNavigate()

  function handleFiltersChange(next: PaymentsFiltersState) {
    setFilters(next)
    setPage(1)
  }

  return (
    <div className="space-y-6">
      <PaymentsFilters filters={filters} onChange={handleFiltersChange} />

      {state.status === 'loading' && <LoadingState label="Carregando pagamentos…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' &&
        (state.data.items.length === 0 ? (
          <Card>
            <EmptyState
              title="Nenhum pagamento encontrado"
              description="Nenhum pagamento corresponde aos filtros selecionados."
            />
          </Card>
        ) : (
          <Card>
            <DataTable
              columns={columns}
              data={state.data.items}
              keyExtractor={(row) => row.id}
              onRowClick={(row) => navigate(`/payments/${row.id}`)}
            />
            <Pagination
              page={state.data.page}
              pageSize={state.data.pageSize}
              totalCount={state.data.totalCount}
              onPageChange={setPage}
            />
          </Card>
        ))}
    </div>
  )
}
