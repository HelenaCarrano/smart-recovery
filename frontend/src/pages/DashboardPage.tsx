import { DashboardSummarySection } from '@/components/DashboardSummarySection'
import { DeclineReasonsNotice } from '@/components/DeclineReasonsNotice'
import { RecoveryOverviewSection } from '@/components/RecoveryOverviewSection'
import { useDashboardSummary } from '@/hooks/useDashboardSummary'
import { useRecoveryOpportunities } from '@/hooks/useRecoveryOpportunities'

export function DashboardPage() {
  const summaryState = useDashboardSummary()
  const opportunitiesState = useRecoveryOpportunities()

  return (
    <div className="space-y-6">
      <DashboardSummarySection state={summaryState} />
      <DeclineReasonsNotice />
      <RecoveryOverviewSection state={opportunitiesState} />
    </div>
  )
}
