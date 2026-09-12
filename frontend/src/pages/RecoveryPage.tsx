import { RecoveryOpportunityItem } from '@/components/RecoveryOpportunityItem'
import { RecoverySortControl, type RecoverySortOption } from '@/components/RecoverySortControl'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { useRecoveryOpportunities } from '@/hooks/useRecoveryOpportunities'
import { recoveryActionLabel } from '@/lib/labels'
import type { RecoveryOpportunity } from '@/types/recovery'
import { useMemo, useState } from 'react'

/** Referência estável para quando ainda não há dados — evita recriar [] a cada render e invalidar os useMemo abaixo. */
const EMPTY_OPPORTUNITIES: RecoveryOpportunity[] = []

function sortOpportunities(opportunities: RecoveryOpportunity[], sortBy: RecoverySortOption): RecoveryOpportunity[] {
  const sorted = [...opportunities]

  switch (sortBy) {
    case 'amount':
      return sorted.sort((a, b) => b.amount - a.amount)
    case 'score':
      return sorted.sort((a, b) => b.recoveryScore - a.recoveryScore)
    case 'action':
      return sorted.sort((a, b) =>
        recoveryActionLabel(a.recommendedAction).localeCompare(recoveryActionLabel(b.recommendedAction)),
      )
    case 'date':
      return sorted.sort((a, b) => b.analyzedAt.localeCompare(a.analyzedAt))
  }
}

export function RecoveryPage() {
  const state = useRecoveryOpportunities()
  const [sortBy, setSortBy] = useState<RecoverySortOption>('amount')

  const opportunities = state.status === 'success' ? state.data : EMPTY_OPPORTUNITIES

  // Destaque de maior impacto financeiro é sempre pelo valor, independente da ordenação escolhida.
  const highestValueId = useMemo(() => {
    if (opportunities.length === 0) return null
    return opportunities.reduce((max, current) => (current.amount > max.amount ? current : max)).id
  }, [opportunities])

  const sorted = useMemo(() => sortOpportunities(opportunities, sortBy), [opportunities, sortBy])

  return (
    <div className="space-y-6">
      <RecoverySortControl value={sortBy} onChange={setSortBy} />

      {state.status === 'loading' && <LoadingState label="Carregando oportunidades de recuperação…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' &&
        (sorted.length === 0 ? (
          <Card>
            <EmptyState
              title="Nenhuma oportunidade pendente"
              description="Não há pagamentos recusados aguardando ação no momento."
            />
          </Card>
        ) : (
          <div className="space-y-3">
            {sorted.map((opportunity) => (
              <RecoveryOpportunityItem
                key={opportunity.id}
                opportunity={opportunity}
                highlight={opportunity.id === highestValueId}
              />
            ))}
          </div>
        ))}
    </div>
  )
}
