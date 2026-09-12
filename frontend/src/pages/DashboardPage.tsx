import { DashboardIntro } from '@/components/DashboardIntro'
import { DashboardSummarySection } from '@/components/DashboardSummarySection'
import { DeclineReasonsChart } from '@/components/DeclineReasonsChart'
import { PaymentsTrendChart } from '@/components/PaymentsTrendChart'
import { RecoveryOverviewSection } from '@/components/RecoveryOverviewSection'
import { useDashboardSummary } from '@/hooks/useDashboardSummary'
import { useDeclineReasonBreakdown } from '@/hooks/useDeclineReasonBreakdown'
import { usePaymentsTrend } from '@/hooks/usePaymentsTrend'
import { useRecoveryOpportunities } from '@/hooks/useRecoveryOpportunities'

export function DashboardPage() {
  const summaryState = useDashboardSummary()
  const opportunitiesState = useRecoveryOpportunities()
  const declineReasonsState = useDeclineReasonBreakdown()
  const trendState = usePaymentsTrend(90)

  return (
    <div className="space-y-6">
      <DashboardIntro />
      <DashboardSummarySection state={summaryState} />
      <PaymentsTrendChart state={trendState} />
      <DeclineReasonsChart state={declineReasonsState} />
      <RecoveryOverviewSection state={opportunitiesState} />
    </div>
  )
}
