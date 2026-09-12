import { RecoveryActionBadge } from '@/components/RecoveryActionBadge'
import { StatusBadge } from '@/components/StatusBadge'
import { SubscriptionStatusBadge } from '@/components/SubscriptionStatusBadge'
import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { type DataTableColumn, DataTable } from '@/components/ui/DataTable'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { ScoreIndicator } from '@/components/ui/ScoreIndicator'
import { useCustomerDetail } from '@/hooks/useCustomerDetail'
import { formatCurrency, formatDateTime } from '@/lib/format'
import { declineReasonLabel } from '@/lib/labels'
import type { RecoveryAnalysis } from '@/types/recovery'
import type { Payment } from '@/types/payment'
import type { Subscription } from '@/types/subscription'
import type { ReactNode } from 'react'
import { useMemo } from 'react'
import { Link, useParams } from 'react-router-dom'

const subscriptionColumns: DataTableColumn<Subscription>[] = [
  { key: 'plan', header: 'Plano', render: (row) => <span className="font-medium text-slate-900">{row.planName}</span> },
  { key: 'price', header: 'Valor', render: (row) => formatCurrency(row.planPrice) },
  { key: 'status', header: 'Status', render: (row) => <SubscriptionStatusBadge status={row.status} /> },
  { key: 'nextBilling', header: 'Próxima cobrança', render: (row) => formatDateTime(row.nextBillingDate) },
]

/**
 * Score/Ação vêm de recoveryByPaymentId (só existe para pagamentos já recusados alguma vez) — em vez de
 * uma seção "Histórico de recuperação" separada repetindo valor/motivo/data que já estão aqui.
 */
function buildPaymentColumns(recoveryByPaymentId: Map<string, RecoveryAnalysis>): DataTableColumn<Payment>[] {
  return [
    { key: 'amount', header: 'Valor', render: (row) => formatCurrency(row.amount) },
    { key: 'status', header: 'Status', render: (row) => <StatusBadge status={row.status} attemptCount={row.attemptCount} /> },
    { key: 'declineReason', header: 'Motivo da recusa', render: (row) => declineReasonLabel(row.declineReason) },
    { key: 'date', header: 'Data', render: (row) => formatDateTime(row.createdAt) },
    {
      key: 'recoveryScore',
      header: 'Recovery Score',
      render: (row) => {
        const analysis = recoveryByPaymentId.get(row.id)
        return analysis ? <ScoreIndicator score={analysis.recoveryScore} /> : '—'
      },
    },
    {
      key: 'recommendedAction',
      header: 'Ação',
      render: (row) => {
        const analysis = recoveryByPaymentId.get(row.id)
        return analysis ? <RecoveryActionBadge action={analysis.recommendedAction} /> : '—'
      },
    },
  ]
}

export function CustomerDetailPage() {
  const { id } = useParams<{ id: string }>()
  const state = useCustomerDetail(id ?? '')

  const recoveryByPaymentId = useMemo(() => {
    if (state.status !== 'success') return new Map<string, RecoveryAnalysis>()
    return new Map(state.data.recoveryHistory.map(({ payment, analysis }) => [payment.id, analysis]))
  }, [state])

  const paymentColumns = useMemo(() => buildPaymentColumns(recoveryByPaymentId), [recoveryByPaymentId])

  if (state.status === 'loading') return <LoadingState label="Carregando cliente…" />
  if (state.status === 'error') return <ErrorState message={state.error} onRetry={state.refetch} />

  const { customer, subscriptions, payments } = state.data

  return (
    <div className="space-y-6">
      <Link to="/customers" className="text-sm font-medium text-brand-600 hover:text-brand-700">
        ← Voltar para Clientes
      </Link>

      <Card>
        <div className="grid grid-cols-2 gap-6 sm:grid-cols-4">
          <Field label="Nome" value={customer.name} />
          <Field label="Email" value={customer.email} />
          <Field label="Telefone" value={customer.phone} />
          <Field label="Documento" value={customer.document} />
          <Field
            label="Status"
            value={<Badge tone={customer.isActive ? 'approved' : 'neutral'}>{customer.isActive ? 'Ativo' : 'Inativo'}</Badge>}
          />
        </div>
      </Card>

      <Card>
        <h2 className="mb-4 text-sm font-semibold text-slate-900">Assinaturas</h2>
        {subscriptions.length === 0 ? (
          <EmptyState title="Nenhuma assinatura" description="Este cliente ainda não tem nenhuma assinatura." />
        ) : (
          <DataTable columns={subscriptionColumns} data={subscriptions} keyExtractor={(row) => row.id} />
        )}
      </Card>

      <Card>
        <h2 className="mb-4 text-sm font-semibold text-slate-900">Histórico de pagamentos</h2>
        {payments.length === 0 ? (
          <EmptyState title="Nenhum pagamento" description="Este cliente ainda não tem nenhuma cobrança registrada." />
        ) : (
          <DataTable columns={paymentColumns} data={payments} keyExtractor={(row) => row.id} />
        )}
      </Card>

    </div>
  )
}

function Field({ label, value }: { label: string; value: ReactNode }) {
  return (
    <div>
      <p className="text-xs text-slate-400">{label}</p>
      <p className="mt-0.5 text-sm font-medium text-slate-900">{value}</p>
    </div>
  )
}
