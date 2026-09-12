export type PlanPeriodicity = 'Monthly' | 'Quarterly' | 'Annual'

/** Espelha o JSON retornado por GET /api/plans e GET /api/plans/{id}. */
export interface Plan {
  id: string
  name: string
  description: string
  price: number
  periodicity: PlanPeriodicity
  isActive: boolean
}
