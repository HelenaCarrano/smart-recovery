import { Card } from '@/components/ui/Card'

export type RecoverySortOption = 'amount' | 'score' | 'action' | 'date'

const SORT_OPTIONS: { value: RecoverySortOption; label: string }[] = [
  { value: 'amount', label: 'Maior valor' },
  { value: 'score', label: 'Maior Recovery Score' },
  { value: 'action', label: 'Ação recomendada' },
  { value: 'date', label: 'Data da análise' },
]

interface RecoverySortControlProps {
  value: RecoverySortOption
  onChange: (value: RecoverySortOption) => void
}

export function RecoverySortControl({ value, onChange }: RecoverySortControlProps) {
  return (
    <Card className="flex items-center gap-3">
      <label className="text-xs font-medium text-slate-500" htmlFor="recovery-sort">
        Ordenar por
      </label>
      <select
        id="recovery-sort"
        className="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none focus:ring-1 focus:ring-brand-400"
        value={value}
        onChange={(e) => onChange(e.target.value as RecoverySortOption)}
      >
        {SORT_OPTIONS.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </Card>
  )
}
