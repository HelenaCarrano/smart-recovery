import { AlertIcon, CheckCircleIcon, SubscriptionsIcon } from '@/components/icons'
import { KpiCard } from '@/components/KpiCard'
import { Card } from '@/components/ui/Card'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import type { AsyncState } from '@/hooks/useAsyncData'
import { formatCurrency, formatPercent } from '@/lib/format'
import type { SubscriptionsSummary } from '@/types/subscriptionsSummary'

interface SubscriptionMetricsSectionProps {
  state: AsyncState<SubscriptionsSummary>
}

export function SubscriptionMetricsSection({ state }: SubscriptionMetricsSectionProps) {
  if (state.status === 'loading') return <LoadingState label="Carregando indicadores…" />
  if (state.status === 'error') return <ErrorState message={state.error} onRetry={state.refetch} />

  const summary = state.data

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <KpiCard
          label="Assinaturas ativas"
          value={String(summary.activeSubscriptions)}
          tone="approved"
          icon={SubscriptionsIcon}
          caption={`${formatPercent(summary.activePercentage / 100)} do total de ${summary.totalSubscriptions}`}
        />
        <KpiCard
          label="MRR"
          value={formatCurrency(summary.monthlyRecurringRevenue)}
          tone="attention"
          icon={CheckCircleIcon}
          caption="Receita mensal recorrente das assinaturas ativas"
        />
        <KpiCard
          label="Cancelamento"
          value={formatPercent(summary.cancellationRate / 100)}
          tone="declined"
          icon={AlertIcon}
          caption={`${summary.cancelledSubscriptions} cancelada(s) · ${summary.pausedSubscriptions} pausada(s)`}
        />
      </div>

      <Card>
        <h2 className="mb-4 text-sm font-semibold text-slate-900">Distribuição por plano</h2>
        {summary.planDistribution.length === 0 ? (
          <p className="text-sm text-slate-500">Nenhuma assinatura ativa no momento.</p>
        ) : (
          <div className="space-y-4">
            {summary.planDistribution.map((plan) => (
              <div key={plan.planId}>
                <div className="flex flex-wrap items-baseline justify-between gap-x-4 gap-y-1">
                  <p className="text-sm font-medium text-slate-900">{plan.planName}</p>
                  <p className="text-xs text-slate-500">
                    {plan.activeSubscriptionsCount} assinante(s) · {formatCurrency(plan.monthlyRevenue)}/mês ·{' '}
                    {formatPercent(plan.sharePercentage / 100)} da base
                  </p>
                </div>
                <div className="mt-1.5 h-2 w-full overflow-hidden rounded-full bg-slate-100">
                  <div
                    className="h-full rounded-full bg-brand-500"
                    style={{ width: `${Math.min(plan.sharePercentage, 100)}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        )}
      </Card>
    </div>
  )
}
