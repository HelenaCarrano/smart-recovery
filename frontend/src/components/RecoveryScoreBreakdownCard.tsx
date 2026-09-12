import { Card } from '@/components/ui/Card'
import { ScoreIndicator } from '@/components/ui/ScoreIndicator'
import type { RecoveryAnalysis } from '@/types/recovery'
import { clsx } from 'clsx'

interface RecoveryScoreBreakdownCardProps {
  analysis: RecoveryAnalysis
}

/**
 * Puro apresentador: todo o cálculo já vem pronto da API (RecoveryScoreCalculator no
 * backend). Este componente só formata os números que RecoveryAnalysisDto já traz —
 * não reimplementa nenhuma regra de negócio.
 */
export function RecoveryScoreBreakdownCard({ analysis }: RecoveryScoreBreakdownCardProps) {
  const rows = [
    { label: 'Motivo da recusa', value: analysis.baseScore },
    { label: 'Histórico do cliente', value: analysis.historyAdjustment },
    { label: 'Recuperações anteriores', value: analysis.recoveryTrackRecordBonus },
    { label: 'Recusas recentes', value: analysis.recentDeclinesAdjustment },
    { label: 'Tentativas recentes', value: analysis.recentAttemptsAdjustment },
  ]

  return (
    <Card>
      <h2 className="mb-1 text-sm font-semibold text-slate-900">Recovery Score</h2>
      <ScoreIndicator score={analysis.recoveryScore} size="lg" />

      <ul className="mt-5">
        {rows.map((row) => (
          <li
            key={row.label}
            className="flex items-center justify-between border-b border-slate-100 py-2 text-sm last:border-0"
          >
            <span className="text-slate-500">{row.label}</span>
            <span
              className={clsx(
                'font-medium tabular-nums',
                row.value > 0 && 'text-status-approved',
                row.value < 0 && 'text-status-declined',
                row.value === 0 && 'text-slate-400',
              )}
            >
              {row.value > 0 ? '+' : ''}
              {row.value}
            </span>
          </li>
        ))}
      </ul>
    </Card>
  )
}
