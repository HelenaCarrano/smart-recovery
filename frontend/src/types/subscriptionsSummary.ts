/** Espelha PlanDistributionDto — participação de um plano na base de assinaturas ativas. */
export interface PlanDistribution {
  planId: string
  planName: string
  activeSubscriptionsCount: number
  monthlyRevenue: number
  sharePercentage: number
}

/** Espelha o JSON retornado por GET /api/subscriptions/summary. */
export interface SubscriptionsSummary {
  totalSubscriptions: number
  activeSubscriptions: number
  activePercentage: number
  pausedSubscriptions: number
  cancelledSubscriptions: number
  cancellationRate: number
  monthlyRecurringRevenue: number
  planDistribution: PlanDistribution[]
}
