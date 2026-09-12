import { SubscriptionMetricsSection } from '@/components/SubscriptionMetricsSection'
import { SubscriptionStatusBadge } from '@/components/SubscriptionStatusBadge'
import { Card } from '@/components/ui/Card'
import { type DataTableColumn, DataTable } from '@/components/ui/DataTable'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { Pagination } from '@/components/ui/Pagination'
import { useSubscriptions } from '@/hooks/useSubscriptions'
import { useSubscriptionsSummary } from '@/hooks/useSubscriptionsSummary'
import { formatCurrency, formatDateTime } from '@/lib/format'
import { periodicityLabel } from '@/lib/labels'
import type { SubscriptionListItem } from '@/types/subscriptionListItem'
import type { SubscriptionStatus } from '@/types/subscription'
import { useState } from 'react'
import { Link } from 'react-router-dom'

const PAGE_SIZE = 20

const STATUS_OPTIONS: { value: SubscriptionStatus | 'All'; label: string }[] = [
  { value: 'All', label: 'Todos os status' },
  { value: 'Active', label: 'Ativa' },
  { value: 'Paused', label: 'Pausada' },
  { value: 'Cancelled', label: 'Cancelada' },
]

const columns: DataTableColumn<SubscriptionListItem>[] = [
  {
    key: 'customer',
    header: 'Cliente',
    render: (row) => (
      <Link
        to={`/customers/${row.customerId}`}
        className="font-medium text-slate-900 hover:text-brand-600"
        onClick={(e) => e.stopPropagation()}
      >
        {row.customerName}
      </Link>
    ),
  },
  { key: 'plan', header: 'Plano', render: (row) => row.planName },
  { key: 'status', header: 'Status', render: (row) => <SubscriptionStatusBadge status={row.status} /> },
  { key: 'price', header: 'Valor', render: (row) => formatCurrency(row.planPrice) },
  { key: 'periodicity', header: 'Periodicidade', render: (row) => periodicityLabel(row.periodicity) },
  { key: 'nextBilling', header: 'Próxima cobrança', render: (row) => formatDateTime(row.nextBillingDate) },
]

export function SubscriptionsPage() {
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState<SubscriptionStatus | 'All'>('All')
  const state = useSubscriptions(page, PAGE_SIZE, status === 'All' ? undefined : status)
  const summaryState = useSubscriptionsSummary()

  function handleStatusChange(next: SubscriptionStatus | 'All') {
    setStatus(next)
    setPage(1)
  }

  return (
    <div className="space-y-6">
      <SubscriptionMetricsSection state={summaryState} />

      <Card className="flex items-center gap-3">
        <label className="text-xs font-medium text-slate-500" htmlFor="subscription-status">
          Status
        </label>
        <select
          id="subscription-status"
          className="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none focus:ring-1 focus:ring-brand-400"
          value={status}
          onChange={(e) => handleStatusChange(e.target.value as SubscriptionStatus | 'All')}
        >
          {STATUS_OPTIONS.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </Card>

      {state.status === 'loading' && <LoadingState label="Carregando assinaturas…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' &&
        (state.data.items.length === 0 ? (
          <Card>
            <EmptyState title="Nenhuma assinatura encontrada" description="Nenhuma assinatura corresponde ao filtro selecionado." />
          </Card>
        ) : (
          <Card>
            <DataTable columns={columns} data={state.data.items} keyExtractor={(row) => row.id} />
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
