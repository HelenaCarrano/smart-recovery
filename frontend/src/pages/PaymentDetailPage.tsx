import { RecommendedActionCard } from '@/components/RecommendedActionCard'
import { RecoveryScoreBreakdownCard } from '@/components/RecoveryScoreBreakdownCard'
import { StatusBadge } from '@/components/StatusBadge'
import { Timeline } from '@/components/Timeline'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { usePaymentDetail } from '@/hooks/usePaymentDetail'
import { formatCurrency, formatDateTime } from '@/lib/format'
import { declineReasonLabel } from '@/lib/labels'
import { buildPaymentTimeline } from '@/lib/paymentTimeline'
import type { ReactNode } from 'react'
import { Link, useParams } from 'react-router-dom'

export function PaymentDetailPage() {
  const { id } = useParams<{ id: string }>()
  const state = usePaymentDetail(id ?? '')

  if (state.status === 'loading') return <LoadingState label="Carregando pagamento…" />
  if (state.status === 'error') return <ErrorState message={state.error} onRetry={state.refetch} />

  const { payment, customerName, planName, attempts, recoveryAnalysis } = state.data
  const timeline = buildPaymentTimeline(payment, attempts, recoveryAnalysis)

  return (
    <div className="space-y-6">
      <Link to="/payments" className="text-sm font-medium text-brand-600 hover:text-brand-700">
        ← Voltar para Pagamentos
      </Link>

      <Card>
        <div className="grid grid-cols-2 gap-6 sm:grid-cols-3 lg:grid-cols-6">
          <Field label="Valor" value={formatCurrency(payment.amount)} />
          <Field label="Status" value={<StatusBadge status={payment.status} />} />
          <Field label="Cliente" value={customerName} />
          <Field label="Data" value={formatDateTime(payment.createdAt)} />
          <Field label="Plano" value={planName} />
          <Field label="Motivo da recusa" value={declineReasonLabel(payment.declineReason)} />
        </div>
      </Card>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <h2 className="mb-4 text-sm font-semibold text-slate-900">Linha do tempo</h2>
          <Timeline steps={timeline} />
        </Card>

        <div className="space-y-6">
          {recoveryAnalysis ? (
            <>
              <RecoveryScoreBreakdownCard analysis={recoveryAnalysis} />
              <RecommendedActionCard
                action={recoveryAnalysis.recommendedAction}
                declineReason={payment.declineReason}
                executedAt={recoveryAnalysis.executedAt}
              />
            </>
          ) : (
            <Card>
              <EmptyState
                title="Sem análise de recuperação"
                description="Este pagamento nunca foi recusado, então não há Recovery Score nem ação recomendada para ele."
              />
            </Card>
          )}
        </div>
      </div>
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
