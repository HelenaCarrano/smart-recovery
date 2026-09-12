import { RecommendedActionCard } from '@/components/RecommendedActionCard'
import { RecoveryScoreBreakdownCard } from '@/components/RecoveryScoreBreakdownCard'
import { StatusBadge } from '@/components/StatusBadge'
import { Timeline } from '@/components/Timeline'
import { Modal } from '@/components/ui/Modal'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { usePaymentDetail } from '@/hooks/usePaymentDetail'
import { formatCurrency, formatDateTime } from '@/lib/format'
import { declineReasonLabel } from '@/lib/labels'
import { buildPaymentTimeline } from '@/lib/paymentTimeline'
import type { PaymentDetail } from '@/types/paymentDetail'
import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'

interface RecoveryOpportunityDetailModalProps {
  paymentId: string
  onClose: () => void
}

/** Mesma composição de cards/timeline de PaymentDetailPage, num modal — sem sair da tela de Recuperação. */
export function RecoveryOpportunityDetailModal({ paymentId, onClose }: RecoveryOpportunityDetailModalProps) {
  const state = usePaymentDetail(paymentId)

  return (
    <Modal title="Detalhe da oportunidade" onClose={onClose}>
      {state.status === 'loading' && <LoadingState label="Carregando pagamento…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' && <DetailContent data={state.data} onNavigate={onClose} />}
    </Modal>
  )
}

function DetailContent({ data, onNavigate }: { data: PaymentDetail; onNavigate: () => void }) {
  const { payment, customerName, planName, attempts, recoveryAnalysis } = data
  const timeline = buildPaymentTimeline(payment, attempts, recoveryAnalysis)

  return (
    <div className="space-y-5">
      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
        <Field label="Cliente" value={customerName} />
        <Field label="Valor" value={formatCurrency(payment.amount)} />
        <Field label="Status" value={<StatusBadge status={payment.status} attemptCount={payment.attemptCount} />} />
        <Field label="Plano" value={planName} />
        <Field label="Data" value={formatDateTime(payment.createdAt)} />
        <Field label="Motivo da recusa" value={declineReasonLabel(payment.declineReason)} />
      </div>

      <div>
        <h3 className="mb-3 text-sm font-semibold text-slate-900">Linha do tempo</h3>
        <Timeline steps={timeline} />
      </div>

      {recoveryAnalysis && (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <RecoveryScoreBreakdownCard analysis={recoveryAnalysis} />
          <RecommendedActionCard
            action={recoveryAnalysis.recommendedAction}
            declineReason={payment.declineReason}
            executedAt={recoveryAnalysis.executedAt}
            paymentStatus={payment.status}
          />
        </div>
      )}

      <Link
        to={`/payments/${payment.id}`}
        onClick={onNavigate}
        className="inline-block text-sm font-medium text-brand-600 hover:text-brand-700"
      >
        Ver página completa do pagamento →
      </Link>
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
