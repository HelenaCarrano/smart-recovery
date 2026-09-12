import { AlertIcon, CheckCircleIcon, PaymentsIcon, RecoveryIcon } from '@/components/icons'
import { KpiCard } from '@/components/KpiCard'
import { PaymentsBreakdownChart } from '@/components/PaymentsBreakdownChart'
import { Card } from '@/components/ui/Card'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import type { AsyncState } from '@/hooks/useAsyncData'
import { formatCurrency, formatPercent } from '@/lib/format'
import type { DashboardSummary } from '@/types/dashboard'

interface DashboardSummarySectionProps {
  state: AsyncState<DashboardSummary>
}

export function DashboardSummarySection({ state }: DashboardSummarySectionProps) {
  if (state.status === 'loading') return <LoadingState label="Carregando indicadores…" />
  if (state.status === 'error') return <ErrorState message={state.error} onRetry={state.refetch} />

  const summary = state.data

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <KpiCard
          label="Recovery Rate"
          value={formatPercent(summary.recoveryRate)}
          tone="attention"
          icon={RecoveryIcon}
        />
        <KpiCard
          label="Pagamentos recusados"
          value={String(summary.declinedPayments)}
          tone="declined"
          icon={PaymentsIcon}
        />
        <KpiCard
          label="Ações pendentes"
          value={String(summary.pendingRecoveryActions)}
          tone="attention"
          icon={AlertIcon}
        />
        <KpiCard
          label="Receita recuperada"
          value={formatCurrency(summary.recoveredRevenue)}
          tone="approved"
          icon={CheckCircleIcon}
          caption={`${summary.recoveredPayments} pagamento(s) recuperado(s) após mais de uma tentativa.`}
        />
      </div>

      <Card>
        <div className="mb-4 flex flex-wrap items-center justify-between gap-2">
          <h2 className="text-sm font-semibold text-slate-900">Resumo de pagamentos</h2>
          <p className="text-sm text-slate-500">
            Receita aprovada total:{' '}
            <span className="font-medium text-slate-700">{formatCurrency(summary.totalRevenue)}</span>
          </p>
        </div>
        <PaymentsBreakdownChart
          approved={summary.approvedPayments}
          declined={summary.declinedPayments}
          pending={summary.pendingPayments}
        />
      </Card>
    </div>
  )
}
