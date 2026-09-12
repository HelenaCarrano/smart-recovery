import { ScoreIndicator } from '@/components/ui/ScoreIndicator'
import type { RecoveryOpportunity } from '@/types/recovery'

interface RecoveryScoreSummaryProps {
  opportunities: RecoveryOpportunity[]
}

/**
 * Média calculada apenas sobre as oportunidades pendentes retornadas pela API
 * (não existe um endpoint de histórico completo de scores) — o texto deixa isso explícito.
 */
export function RecoveryScoreSummary({ opportunities }: RecoveryScoreSummaryProps) {
  const average = Math.round(
    opportunities.reduce((sum, opportunity) => sum + opportunity.recoveryScore, 0) / opportunities.length,
  )

  return (
    <div>
      <ScoreIndicator score={average} size="lg" />
      <p className="mt-3 text-sm text-slate-500">
        Média entre {opportunities.length}{' '}
        {opportunities.length === 1 ? 'oportunidade pendente' : 'oportunidades pendentes'} de recuperação.
      </p>
    </div>
  )
}
