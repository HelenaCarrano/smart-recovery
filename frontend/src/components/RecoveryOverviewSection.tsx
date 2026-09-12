import { RecoveryOpportunitiesList } from '@/components/RecoveryOpportunitiesList'
import { RecoveryScoreSummary } from '@/components/RecoveryScoreSummary'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import type { AsyncState } from '@/hooks/useAsyncData'
import type { RecoveryOpportunity } from '@/types/recovery'

interface RecoveryOverviewSectionProps {
  state: AsyncState<RecoveryOpportunity[]>
}

/** Busca as oportunidades uma única vez e alimenta tanto a lista quanto o resumo de score. */
export function RecoveryOverviewSection({ state }: RecoveryOverviewSectionProps) {
  if (state.status === 'loading') return <LoadingState label="Carregando oportunidades de recuperação…" />
  if (state.status === 'error') return <ErrorState message={state.error} onRetry={state.refetch} />

  const opportunities = state.data

  return (
    <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <Card className="lg:col-span-2">
        <h2 className="mb-4 text-sm font-semibold text-slate-900">Recovery Opportunities</h2>
        {opportunities.length === 0 ? (
          <EmptyState
            title="Nenhuma oportunidade pendente"
            description="Não há pagamentos recusados aguardando ação no momento."
          />
        ) : (
          <RecoveryOpportunitiesList opportunities={opportunities} />
        )}
      </Card>

      <Card>
        <h2 className="mb-4 text-sm font-semibold text-slate-900">Recovery Score médio</h2>
        {opportunities.length === 0 ? (
          <EmptyState title="Sem scores para exibir" />
        ) : (
          <RecoveryScoreSummary opportunities={opportunities} />
        )}
      </Card>
    </div>
  )
}
