import { RecoveryOpportunityItem } from '@/components/RecoveryOpportunityItem'
import type { RecoveryOpportunity } from '@/types/recovery'

interface RecoveryOpportunitiesListProps {
  opportunities: RecoveryOpportunity[]
}

/** Maior valor em risco primeiro — destaca onde a recuperação tem mais impacto financeiro. */
export function RecoveryOpportunitiesList({ opportunities }: RecoveryOpportunitiesListProps) {
  const sorted = [...opportunities].sort((a, b) => b.amount - a.amount)

  return (
    <div className="space-y-3">
      {sorted.map((opportunity, index) => (
        <RecoveryOpportunityItem key={opportunity.id} opportunity={opportunity} highlight={index === 0} />
      ))}
    </div>
  )
}
