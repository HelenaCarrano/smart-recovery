import { EmptyState } from '@/components/ui/EmptyState'
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from 'recharts'

interface PaymentsBreakdownChartProps {
  approved: number
  declined: number
  pending: number
}

/** Mesmas cores dos tokens semânticos de status (emerald/rose/amber-600), em hex porque o Recharts não lê classes Tailwind. */
const COLORS = {
  approved: '#059669',
  declined: '#e11d48',
  pending: '#d97706',
} as const

export function PaymentsBreakdownChart({ approved, declined, pending }: PaymentsBreakdownChartProps) {
  const total = approved + declined + pending

  if (total === 0) {
    return (
      <EmptyState
        title="Sem pagamentos registrados"
        description="Ainda não há cobranças suficientes para exibir esta distribuição."
      />
    )
  }

  const data = [
    { name: 'Aprovados', value: approved, color: COLORS.approved },
    { name: 'Recusados', value: declined, color: COLORS.declined },
    { name: 'Pendentes', value: pending, color: COLORS.pending },
  ].filter((entry) => entry.value > 0)

  return (
    <div className="flex flex-col items-center gap-6 sm:flex-row">
      <div className="h-40 w-40 shrink-0">
        <ResponsiveContainer>
          <PieChart>
            <Pie data={data} dataKey="value" nameKey="name" innerRadius={45} outerRadius={70} paddingAngle={2}>
              {data.map((entry) => (
                <Cell key={entry.name} fill={entry.color} />
              ))}
            </Pie>
            <Tooltip />
          </PieChart>
        </ResponsiveContainer>
      </div>

      <ul className="space-y-2 text-sm">
        {data.map((entry) => (
          <li key={entry.name} className="flex items-center gap-2">
            <span className="h-2.5 w-2.5 shrink-0 rounded-full" style={{ backgroundColor: entry.color }} />
            <span className="text-slate-600">{entry.name}</span>
            <span className="font-medium text-slate-900">{entry.value}</span>
            <span className="text-slate-400">({Math.round((entry.value / total) * 100)}%)</span>
          </li>
        ))}
      </ul>
    </div>
  )
}
